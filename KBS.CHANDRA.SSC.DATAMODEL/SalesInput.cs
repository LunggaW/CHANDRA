using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KBS.CHANDRA.SSC.DATAMODEL
{
    public class SalesInput
    {
        public enum ItemStatus
        {
            Reserved = 1,
            Sold,
            Cancelled,
            Cleared
        };
    }
}
