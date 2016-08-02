using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KBS.CHANDRA.SSC.DATAMODEL
{
    public class MemoDiscountHeader
    {
            public string PromoCode { get; set; }
            public string Description { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
    }
}
