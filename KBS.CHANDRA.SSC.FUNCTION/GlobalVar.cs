using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KBS.CHANDRA.SSC.FUNCTION
{
    public static class GlobalVar
    {
        private static string username = "";
        private static string password = "";
        private static string userid = "";
        private static string profileid = "";
        private static string site = "";
        private static string kode = "";
        private static Panel panel = null;

        public static string GlobalVarUsername
        {
            get { return username; }
            set { username = value; }
        }

        public static string GlobalVarPassword
        {
            get { return password; }
            set { password = value; }
        }

        public static string GlobalVarUserID
        {
            get { return userid; }
            set { userid = value; }
        }

        public static string GlobalVarProfileID
        {
            get { return profileid; }
            set { profileid = value; }
        }

        public static string GlobalVarSite
        {
            get { return site; }
            set { site = value; }
        }

        public static string GlobalVarKodeInvoice
        {
            get { return kode; }
            set { kode = value; }
        }

        public static Panel GlobalVarPanel
        {
            get { return panel; }
            set { panel = value; }
        }
    }
}
