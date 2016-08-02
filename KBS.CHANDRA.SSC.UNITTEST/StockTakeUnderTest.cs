using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using KBS.CHANDRA.SSC.DATAMODEL;
using KBS.CHANDRA.SSC.FUNCTION;
using NUnit.Framework;

namespace KBS.CHANDRA.SSC.UNITTEST
{


    [TestFixture]
    class StockTakeUnderTest
    {
        SSCFunction function;

        [TestFixtureSetUp]
        public void SetUp()
        {
            function = new SSCFunction();
        }


        [Test]
        public void StockTakeScanProcess_Return_Success()
        {

            //StringAssert.AreEqualIgnoringCase("Success Uploading Data", function.deleteFromINTINV(UlInv.IVFCEXINV));

            //StringAssert.AreEqualIgnoringCase("Success Uploading Data", function.processUploadInvent(UlInv));
        }
    }
}
