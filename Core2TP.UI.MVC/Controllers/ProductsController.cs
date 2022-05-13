#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core2TP.DATA.EF.Models;
using System.Drawing;
using Core2TP.UI.MVC.Utilities;
using X.PagedList;

namespace Core2TP.UI.MVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly gadgetstoreContext _context;
        //added for access to wwwroot folder
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductsController(gadgetstoreContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        //Table view
        public async Task<IActionResult> Index()
        {
            #region LINQ Notes
            //_context.Products --> returns a ***collection*** of ALL products from the table

            /*
             * LINQ - 2 Syntaxes
             * Method Syntax: uses C# methods and lambda expressions
             * Query (Keyword) Syntax: uses SQL keywords
             * - Ultimately, it doesn't matter which syntax you choose - you can accomplish whatever you need with either.
             * - For learning effectiveness, practice primarily with the one that makes most sense to you.  Once you feel you have a good grasp,
             * try the same problems with the other syntax.
             * 
             * METHOD SYNTAX EXAMPLE
             * var [filteredCollection] = _context.[Entity].Where([otf] => [otf].[Property] [[Condition]]).ToListAsync();
             * 
             * 
             * KEYWORD SYNTAX EXAMPLE
             * var [filteredCollection] = (from [otf] in _context.[Entity]
             *                            where [otf].[Property] [[Condition]]
             *                            select [otf]).ToListAsync();
             * 
             * - filteredCollection --> the resulting collection once you filter out records you do not need
             * - Entity --> the specific DB table you are retrieving results from
             * - otf --> an on-the-fly variable created by you to represent a single Entity from the table
             * - Property --> the property from the Entity to be evaluated in the condition
             * - Condition --> your filter criteria
             * */
            #endregion

            //Original code scaffolded by EF
            var products = _context.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.OrderProducts);

            //Use LINQ to filter this so we only see 'active' Products (aka products that are NOT discontinued)
            //Filtered collection using LINQ
            //var products = _context.Products.Where(p => !p.IsDiscontinued).Include(p => p.Category).Include(p => p.Supplier);

            #region More LINQ Examples
            //More examples of filtering products
            //Typically, these filters would be applied dynamically based on user input (search form, checkboxes, drop down lists...)
            //We will add that functionality in next week.  For now, we are going to focus on JUST delivering filtered results

            //Only show Products in the 'weapon' category
            //var products = _context.Products.Where(p => p.Category.CategoryName.ToLower() == "weapon").Include(p => p.Category).Include(p => p.Supplier);

            //Only show Products made by 'Marvelous Artifacts'
            //var products = _context.Products.Where(p => p.Supplier.SupplierName.ToLower().Contains("marvelous"))
            //    .Include(p => p.Category)
            //    .Include(p => p.Supplier);

            //Show products that are out of stock and pending reorder
            //var products = _context.Products.Where(p => p.UnitsInStock == 0 && p.UnitsOnOrder > 0)
            //    .Include(p => p.Category)
            //    .Include(p => p.Supplier);

            //MINI LAB!
            //Show products that cost more than $500

            //Show all products that are a 'Tool' from 'United Federation of Planets'

            //Show all products that contain the word 'star' in the name
            #endregion

            return View(await products.ToListAsync());
        }



        #region Filter/Paging Steps
        //---- SEARCH TEXTBOX ----//
        //1) Create form in the view (for the SEARCH portion, only need 1 textbox and a submit button - <select> will be added later)
        //2) Update controller Action ([A] add param, [B]add search filter logic)

        //---- CATEGORY DDL ----//
        //3) Create ViewData[] object in Controller action (this sends DDL list to the View)
        //4) Add <select> inside of <form>
        //5) Update Controller Action ([A] add param, [B] add category filter logic)

        //---- PAGED LIST ----//
        //6) Install package for X.PagedList.Mvc.Core
        //7) Add using statements and update model declaration in the View
        //8) Add param to Controller Action
        //9) Add page size variable in Action
        //10) Update return statement in Controller Action
        //11) Add Counter in View

        // 12) Create a new CSS file in wwwroot/css named 'PagedList.css'
        //      - NOTE: may need to go to the package's NuGet page and copy the CSS directly OR copy from an existing project :)
        //      - X.PagedList CSS file link (go here to copy the code): https://github.com/dncuug/X.PagedList/blob/master/examples/X.PagedList.Mvc.Example.Core/wwwroot/css/PagedList.css
        // 13) Add a <link> to the _Layout referencing 'PagedList.css'
        #endregion


        //Tile view
        //                                            **** STEP 2.A ****,  **** STEP 5.A ****, **** STEP 8 ****
        public async Task<IActionResult> TiledProducts(string searchTerm, int categoryId = 0, int page = 1)//added params for query
        {
            //**** STEP 9 ****//
            int pageSize = 6;//allows us to set how many records per page

            //**** STEP 3 ****//
            //List of Categories to return to the view for DDL population
            //NOTE: copied code from Products/Create to generate DDL
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewBag.Category = 0;

            //Main list of products - anything that is not discontinued. 
            //If filters are incorporated, we will whittle this list down
            var products = _context.Products.Where(p => !p.IsDiscontinued).Include(p => p.Category).Include(p => p.Supplier).OrderBy(p => p.ProductName).ToList();

            //**** STEP 5.B ****/
            //Filter by category first
            #region Optional Category Filter
            if (categoryId != 0)
            {
                products = products.Where(p => p.CategoryId == categoryId).ToList();
                //Regenerate ddl so that the current item is still 'selected'
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);
                ViewBag.Category = categoryId;
            }
            #endregion

            //**** Step 2.B ****/
            //Filter by search term second
            #region Optional Search Filter
            if (!String.IsNullOrEmpty(searchTerm))
            {
                //if optional search is used, then filter results by the searchTerm - like a fuzzy search
                products = products
                    .Where(p => p.ProductName.ToLower().Contains(searchTerm.ToLower()) || 
                                p.ProductDescription.ToLower().Contains(searchTerm.ToLower()) ||
                                p.Category.CategoryName.ToLower().Contains(searchTerm.ToLower()) ||
                                p.Supplier.SupplierName.ToLower().Contains(searchTerm.ToLower()))
                    .ToList();

                //Pass the searchTerm back to the view to verify what the user searched for
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                ViewBag.SearchTerm = null;
            }
            #endregion

            //OG
            //return View(products.ToList());

            //**** STEP 10 *****//
            return View(products.ToPagedList(page, pageSize));
        }
        

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Address");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductPrice,ProductDescription,UnitsInStock,UnitsOnOrder,IsDiscontinued,CategoryId,SupplierId,ProductImage,Image")] Product product)
        {
            //NOTE: RUN THROUGH THIS WITHOUT MODIFYING THE Product.cs MODEL FIRST - walk through debugger and show why we are not passing model validation
            //Set ProductImage prop to empty string to pass validation
            product.ProductImage = "noimage.png";

            if (ModelState.IsValid)
            {
                #region File Upload - CREATE
                //.Image holds any file if it was uploaded with the form
                if (product.Image != null)
                {
                    //Find the extension to validate file upload
                    string ext = Path.GetExtension(product.Image.FileName);

                    //List valid extensions in string array
                    string[] validExts = { ".jpeg", ".jpg", ".gif", ".png" };

                    //Check extension of uploaded file against list of valid extensions
                    if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4194303)//default max file size for .NET
                    {
                        //if we pass the valid extension check, generate a unique filename
                        product.ProductImage = Guid.NewGuid() + ext;

                        //Save the file into a folder in the UI project
                        //To access the wwwroot (aka the webroot path), add a prop to this controller class and update the ctor in this controller to accept 
                        //the hosting env param
                        //retrieve path to wwwroot
                        string webRootPath = _webHostEnvironment.WebRootPath;
                        //Full path to the Images folder inside of wwwroot
                        string fullImagePath = webRootPath + "/images/";

                        //Fire up a stream to read the image
                        using (var memoryStream = new MemoryStream())
                        {
                            await product.Image.CopyToAsync(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {
                                //items needed for the upload utility
                                //1) maximum size for image
                                //2) maximum size for thumbnail
                                //3) full path to save the image file (already have - fullImagePath var)
                                //4) an image (already have -- img var)
                                //5) filename for the image (already have -- product.ProductImage)

                                int maxImageSize = 500;
                                int maxThumbSize = 100;
                                ImageUtility.ResizeImage(fullImagePath, product.ProductImage, img, maxImageSize, maxThumbSize);

                            }
                        }
                    }
                }
                #endregion

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Address", product.SupplierId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Address", product.SupplierId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductPrice,ProductDescription,UnitsInStock,UnitsOnOrder,IsDiscontinued,CategoryId,SupplierId,ProductImage,Image")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                #region EDIT File Upload
                string imgName = product.ProductImage;
                if (product.Image != null)
                {
                    string ext = Path.GetExtension(product.Image.FileName);
                    string[] validExts = { ".jpeg", ".jpg", ".gif", ".png" };
                    if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4194303)
                    {
                        imgName = Guid.NewGuid() + ext;
                        string webRootPath = _webHostEnvironment.WebRootPath;
                        string path = webRootPath + "/images/";

                        //Delete the old image
                        if (product.ProductImage != null && product.ProductImage != "noimage.png")
                        {
                            ImageUtility.Delete(path, product.ProductImage);
                        }

                        //Save new image to webroot
                        using (var memoryStream = new MemoryStream())
                        {
                            await product.Image.CopyToAsync(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {
                                int maxImageSize = 500;
                                int maxThumbSize = 100;
                                ImageUtility.ResizeImage(path, imgName, img, maxImageSize, maxThumbSize);
                            }
                        }
                    }

                    //only update if an image was uploaded
                    product.ProductImage = imgName;
                }
                #endregion

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Address", product.SupplierId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            #region Return Error Message if record is in use
            if (product.OrderProducts.Count > 0)
            {
                ViewBag.DeleteMessage = "This product is referenced by one or more orders, and cannot be deleted.";
            }
            #endregion

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            #region Delete Image File
            string webRootPath = _webHostEnvironment.WebRootPath;
            string path = webRootPath + "/images/";

            if (product.ProductImage != null && product.ProductImage != "noimage.png")
            {
                ImageUtility.Delete(path, product.ProductImage);
            }
            #endregion

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
