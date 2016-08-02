using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KBS.CHANDRA.SSC.DATAMODEL
{
    public class User
    {
        public enum UserStatus
        {
            Active = 1,
            Frozen,
            Delete,
        };

        public string Username { get; set; }
        public string Password { get; set; }
        public UserStatus Status { get; set; }
        public string ProfileID { get; set; }
        public string UserID { get; set; }
    }
}
