using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KBS.CHANDRA.SSC.DATAMODEL;
using KBS.CHANDRA.SSC.FUNCTION;
using NUnit.Framework;

namespace KBS.CHANDRA.SSC.UNITTEST
{

    [TestFixture]
    class LoginUnderTest
    {
        User user = new User();
        SSCFunction function = new SSCFunction();

        [Test]
        public void Login_Return_Success()
        {
            user = function.Login("admin", "adm123");

            StringAssert.AreEqualIgnoringCase("admin", user.Username);
            
            
        }

        

    }
}
