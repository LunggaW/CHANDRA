using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KBS.CHANDRA.SSC.DATAMODEL
{
    public class InvHeader
    {
        public string IDH{ get; set; }
        public string KODE{ get; set; }
        public string IDPENGUSAHA{ get; set; }
        public string IDPEMBELI{ get; set; }
        public string CREATEDBY{ get; set; }
        public string MODIFIEDBY{ get; set; }
        public string CREATEDDATE{ get; set; }
        public string MODIFIEDDATE{ get; set; }
        public string NOMODIFIED{ get; set; }
        public string STATUS { get; set; }

        public string STARTDATE { get; set; }
        public string ENDDATE { get; set; }
        public string LASTTOTAL { get; set; }

        public string TOTALDATAINV { get; set; }
        public string TOTALINV { get; set; }
        public string EXPDETAIL { get; set; }
        public string TOTALINVWT { get; set; }
        public string TOTALINVT { get; set; }
        public string TOTALINVNT { get; set; }
        public string EXPHEADER { get; set; }
        public string TOTALFACWT { get; set; }
        public string TOTALFACT { get; set; }
        public string TOTALFACNT { get; set; }
        public string TERBILANGTotalFacNT { get; set; }
        public string FComment { get; set; }
        public string HComment { get; set; }
        
    }
}
