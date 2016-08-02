using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KBS.CHANDRA.SSC.DATAMODEL
{
    public class StockTake
    {
        public string Barcode { get; set; }
        public string Description { get; set; }
        public string Qty { get; set; }
        public int SiteCode { get; set; }
        public DateTime? InvDate { get; set; }
        public int? Type { get; set; }
    }
}
