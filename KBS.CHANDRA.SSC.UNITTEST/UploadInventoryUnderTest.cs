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
    class UploadInventoryUnderTest
    {
        SSCFunction function;
        UploadInventory UlInv;

        [TestFixtureSetUp]
        public void SetUp()
        {
             function = new SSCFunction();
             UlInv = new UploadInventory();
        }
            

        [Test]
        public void UploadInventoryProcess_Return_Success()
        {
            DateTime date = new DateTime(2015, 5, 29, 0, 00, 0);

            UlInv.IVFCACT = "3";
            UlInv.IVFCEXINV = "201";
            UlInv.IVFCEXV = null;
            UlInv.IVFCEXVL = null;
            UlInv.IVFCODE = "0077257";
            UlInv.IVFDCPPREV = null;
            UlInv.IVFDCRE = date;
            UlInv.IVFDINV = date;
            UlInv.IVFDMAJ = date;
            UlInv.IVFDTRT = date;
            UlInv.IVFEMPL = "";
            UlInv.IVFFICH = "KSM";
            UlInv.IVFGRPS = "";
            UlInv.IVFIDSTR = "";
            UlInv.IVFLGFI = "1";
            UlInv.IVFLIBL = "KSM-20101-20150529";
            UlInv.IVFMESS = "";
            UlInv.IVFMODE = "4";
            UlInv.IVFNERR = "";
            UlInv.IVFNLIG = "1";
            UlInv.IVFNLIS = "";
            UlInv.IVFNODE = "";
            UlInv.IVFNORDRE = "1";
            UlInv.IVFNPORT = "";
            UlInv.IVFORIGCEXINV = "0";
            UlInv.IVFPDSINV = "";
            UlInv.IVFPV = "";
            UlInv.IVFQTER = "1";
            UlInv.IVFSITE = "20101";
            UlInv.IVFTINV = "240";
            UlInv.IVFTPOS = "0";
            UlInv.IVFTRT = "0";
            UlInv.IVFUTIL = "INTF";

            StringAssert.AreEqualIgnoringCase("Success Uploading Data", function.deleteFromINTINV(UlInv.IVFCEXINV));

            StringAssert.AreEqualIgnoringCase("Success Uploading Data", function.processUploadInvent(UlInv));
        }
    }
}
