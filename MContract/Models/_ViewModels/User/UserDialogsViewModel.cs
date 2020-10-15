using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
    public class UserDialogsViewModel
    {
        public LeftMenuViewModel LeftMenuViewModel { get; set; }
        public MobileMenuViewModel MobileMenuViewModel { get; set; }
        public List<Dialog> Dialogs { get; set; }
        public List<User> AllUsers { get; set; }
    }
}