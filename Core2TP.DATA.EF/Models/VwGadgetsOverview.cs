using System;
using System.Collections.Generic;

namespace Core2TP.DATA.EF.Models
{
    public partial class VwGadgetsOverview
    {
        public string ProductName { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public short UnitsInStock { get; set; }
        public short UnitsOnOrder { get; set; }
        public string CategoryName { get; set; } = null!;
        public string SupplierName { get; set; } = null!;
        public bool IsDiscontinued { get; set; }
    }
}
