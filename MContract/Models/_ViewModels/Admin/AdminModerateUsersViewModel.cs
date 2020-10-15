using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
    public class AdminModerateUsersViewModel
    {
        public List<User> Users { get; set; }
        public List<User> SbisInfoUsers { get; set; }
    }
}