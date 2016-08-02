using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KBS.CHANDRA.SSC.DATAMODEL
{
    public class InvoiceDetailDetailHeaderData
    {
        public string SupplierName { get; set; }
        public string Cabang { get; set; }
        public string NamaBarang { get; set; }
        public string PeriodePenjualan { get; set; }
        public string AttTo { get; set; }
        public string SubTotal { get; set; }
        public string Tax { get; set; }
        public string TotalAfterTax { get; set; }
        public string TotalAkhir { get; set; }
    }
}

