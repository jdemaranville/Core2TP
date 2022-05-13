using Core2TP.DATA.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace Core2TP.UI.MVC.Models
{
    public class CartItemViewModel
    {
        public int Qty { get; set; }
        public Product Product { get; set; }

        public CartItemViewModel(int qty, Product product)
        {
            Qty = qty;
            Product = product;
        }
    }
}
