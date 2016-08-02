using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KBS.CHANDRA.SSC.DATAMODEL
{
    public class TransactionInvoice
    {
        public string IDH { get; set; }
        public string SKUID { get; set; }
        public string BRUTO { get; set; }
        public string NETTO { get; set; }
        public string COMMENT { get; set; }
        public string DISCBRUTO { get; set; }
        public string DISCNETTO { get; set; }
    }
}
