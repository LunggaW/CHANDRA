using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KBS.CHANDRA.SSC.DATAMODEL
{
    public class Item
    {
        public string NomorNota { get; set; }
        public string UserID { get; set; }
        public string Barcode { get; set; }
        public string Article { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string BrandName { get; set; }
        public string Size { get; set; }
        public int Qty { get; set; }
        public string Color { get; set; }
        public int VariantID { get; set; }
        public string Store { get; set; }
        public decimal Price { get; set; }
        public decimal Discount1 { get; set; }
        public decimal Discount2 { get; set; }
        public decimal Discount3 { get; set; }
        public decimal DiscountRP { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal FixPrice { get; set; }
        public int StatusSales { get; set; }
    }
}
