using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KBS.CHANDRA.SSC.DATAMODEL
{
    public class FakturPajakSearch
    {
        public string KODE { get; set; }
        public string IDPENGUSAHA { get; set; }
        public string IDPEMBELI { get; set; }
        public string STATUS { get; set; }
        public string COMMENTHEADER { get; set; }
        public string COMMENTFOOTER { get; set; }
        public string Total { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String InvoiceNo { get; set; }
        public string No { get; set; }
        public string Pengirim { get; set; }
        public string NPWP { get; set; }
        public string AdPengirim { get; set; }
        public string NoRekPenerima { get; set; }
        public string ANPenerima { get; set; }
        public string BankPenerima { get; set; }
        public string AdPenerima { get; set; }
        public string Penerima { get; set; }
        public string TotalTerbilang { get; set; }
        public string DataPenerima { get; set; }
        public string DataPengirim { get; set; }
        public string Biaya { get; set; }

    }
}
