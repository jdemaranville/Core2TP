using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core2TP.DATA.EF.Models
{
    #region Category
    public class CategoryMetadata
    {
        public int CategoryId { get; set; }
        [StringLength(50, ErrorMessage = "Must not exceed 50 characters")]
        [Display(Name = "Category")]
        public string CategoryName { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Must not exceed 500 characters")]
        [Display(Name = "Description")]
        public string? CategoryDescription { get; set; }
    }
    #endregion

    #region Order
    public class OrderMetadata
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } = null!;

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [StringLength(100, ErrorMessage = "Must not exceed 100 characters")]
        [Display(Name = "Ship To")]
        public string ShipToName { get; set; } = null!;
        [StringLength(50, ErrorMessage = "Must not exceed 50 characters")]

        [Display(Name = "City")]
        public string ShipCity { get; set; } = null!;

        [StringLength(2, ErrorMessage = "Must not exceed 2 characters")]
        [Display(Name = "State")]
        public string? ShipState { get; set; }

        [StringLength(5, ErrorMessage = "Must not exceed 5 characters")]
        [Display(Name = "Zip")]
        public string ShipZip { get; set; } = null!;
    }
    #endregion


    #region Product
    public class ProductMetadata
    {
        public int ProductId { get; set; }

        [StringLength(200, ErrorMessage = "Must not exceed 200 characters")]
        [Display(Name = "Product")]
        public string ProductName { get; set; } = null!;

        [DisplayFormat(DataFormatString = "{0:c}")]
        [Display(Name = "Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be greater than 0")]
        public decimal ProductPrice { get; set; }

        [StringLength(500, ErrorMessage = "Must not exceed 500 characters")]
        [Display(Name = "Description")]
        public string? ProductDescription { get; set; }

        [Display(Name = "In Stock")]
        public short UnitsInStock { get; set; }

        [Display(Name = "On Order")]
        public short UnitsOnOrder { get; set; }

        [Display(Name = "Discontinued?")]
        public bool IsDiscontinued { get; set; }

        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }

        [Display(Name = "Image")]
        public string? ProductImage { get; set; }

    }
    #endregion


    #region Supplier
    public class SupplierMetadata
    {
        public int SupplierId { get; set; }

        [StringLength(100, ErrorMessage = "Must not exceed 100 characters")]
        [Display(Name = "Supplier")]
        public string SupplierName { get; set; } = null!;

        [StringLength(150, ErrorMessage = "Must not exceed 150 characters")]
        public string Address { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Must not exceed 100 characters")]
        public string City { get; set; } = null!;

        [StringLength(2, ErrorMessage = "Must not exceed 2 characters")]
        public string? State { get; set; }

        [StringLength(5, ErrorMessage = "Must not exceed 5 characters")]
        public string? Zip { get; set; }

        [StringLength(24, ErrorMessage = "Must not exceed 24 characters")]
        public string? Phone { get; set; }
    }
    #endregion

    #region User Detail
    public class UserDetailMetadata
    {
        [StringLength(50, ErrorMessage = "Must not exceed 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Must not exceed 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [StringLength(150, ErrorMessage = "Must not exceed 150 characters")]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "Must not exceed 50 characters")]
        public string? City { get; set; }

        [StringLength(2, ErrorMessage = "Must not exceed 2 characters")]
        public string? State { get; set; }

        [StringLength(5, ErrorMessage = "Must not exceed 5 characters")]
        public string? Zip { get; set; }

        [StringLength(24, ErrorMessage = "Must not exceed 24 characters")]
        public string? Phone { get; set; }
    }
    #endregion

}
