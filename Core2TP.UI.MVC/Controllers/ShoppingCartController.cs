using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core2TP.DATA.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Core2TP.UI.MVC.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace Core2TP.UI.MVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly gadgetstoreContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ShoppingCartController(UserManager<IdentityUser> userManager, gadgetstoreContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            //retrieve the shopping cart from session
            var sessionCart = HttpContext.Session.GetString("cart");

            Dictionary<int, CartItemViewModel> shoppingCart = null;

            if (sessionCart == null || sessionCart.Count() == 0)
            {
                //user either hasn't put anything in the cart, or removed all, or session expired...
                //set cart to an empty object (can still send that to the view, unlike NULL)
                shoppingCart = new Dictionary<int, CartItemViewModel>();

                ViewBag.Message = "There are no items in your cart.";
            }
            else
            {
                ViewBag.Message = null;
                //deserialize JSONified session cart
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }
            return View(shoppingCart);
        }

        public IActionResult AddToCart(int qty, int productId)
        {
            //Empty shell for LOCAL shopping cart variable
            //int -> product Id
            //civm -> qty & product
            Dictionary<int, CartItemViewModel> shoppingCart = null;

            //Check if session cart exists; if so, copy it's contents to the local version
            var sessionCart = HttpContext.Session.GetString("cart");

            //If cart DOES NOT exist
            if (String.IsNullOrEmpty(sessionCart))
            {
                shoppingCart = new Dictionary<int, CartItemViewModel>();
            }
            else
            {
                //cart already exists... set Dictionary to existing session cart value
                //convert JSON string cart into Dictionary<>
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }

            Product product = _context.Products.Find(productId);

            if (product == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                //Instantiate the item for the cart
                CartItemViewModel item = new CartItemViewModel(qty, product);

                //If the product was already in the cart, increase the quantity.
                //Otherwise, add the item to the cart.
                if (shoppingCart.ContainsKey(product.ProductId))
                {
                    shoppingCart[product.ProductId].Qty += qty;
                }
                else
                {
                    shoppingCart.Add(product.ProductId, item);
                }

                //update session version of the cart
                //added using Newtonsoft.Json
                //session can only store int, string, or byte[]
                //in order to store the Dictionary<>, we must STRINGIFY that beast
                string jsonCart = JsonConvert.SerializeObject(shoppingCart);
                HttpContext.Session.SetString("cart", jsonCart);

            }

            return RedirectToAction("Index");
        }

        public IActionResult UpdateCart(int productId, int qty)
        {
            //retrieve the cart
            var sessionCart = HttpContext.Session.GetString("cart");
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //update qty for key
            shoppingCart[productId].Qty = qty;

            //update session
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);
            HttpContext.Session.SetString("cart", jsonCart);

            //redirect back to index
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            //retrieve the cart
            var sessionCart = HttpContext.Session.GetString("cart");
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //remove cart item
            shoppingCart.Remove(id);

            //update cart
            if (shoppingCart.Count == 0)
            {
                HttpContext.Session.Remove("cart");
            }
            else
            {
                string jsonCart = JsonConvert.SerializeObject(shoppingCart);
                HttpContext.Session.SetString("cart", jsonCart);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SubmitOrder()
        {
            //Retrieve User Details info
            //To do this, need async handling with the UserManager provided by Identity
            //Must also add a prop and update teh ctor in this controller to include the userManager

            #region Big Picture Plan
            //BIG PICTURE PLAN
            //Create Order record for the DB
            //need to:
            //- create Order object
            //- assign prop values (think "where can we get these values?")
            //Order props needed:
            //OrderDate
            //UserId
            //ShipCity
            //ShipState
            //ShipToName
            //ShipZip
            //- save to DB
            //use _context to add record


            //Create OrderProducts record for the DB
            //need to:
            //- Create OrderProduct object
            //- assign prop values
            //OrderId
            //ProductId
            //ProductPrice
            //Quantity
            //- save to DB
            //use _context to add record
            #endregion


            //FIRST, get current user's Id
            string? userId = (await _userManager.GetUserAsync(HttpContext.User))?.Id;
            //THEN, retrieve the UserDetails object from the DB
            UserDetail user = _context.UserDetails.Find(userId);

            //Create the order object and assign values
            Order o = new Order()
            {
                OrderDate = DateTime.Now,
                UserId = userId,
                ShipCity = user.City,
                ShipState = user.State,
                ShipToName = user.FullName,
                ShipZip = user.Zip
            };

            //Queue the object up to be added to the DB
            _context.Orders.Add(o);


            //Create OrderProducts record - will need product info from the cart
            //retrieve cart
            var sessionCart = HttpContext.Session.GetString("cart");
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //each item in the cart represents an OrderProduct record as it is ONE of the products on an order
            //loop through the cart, and create OrderProduct record for each
            foreach (var item in shoppingCart)
            {
                OrderProduct op = new OrderProduct()
                {
                    OrderId = o.OrderId,
                    ProductId = item.Value.Product.ProductId,
                    ProductPrice = item.Value.Product.ProductPrice,
                    Quantity = (short)item.Value.Qty
                };
                //Create the lookup table record by attaching it to our Order
                o.OrderProducts.Add(op);
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Orders");

        }

        
    }
}
