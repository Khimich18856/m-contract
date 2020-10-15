using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
    public class UserCompaniesViewModel
    {
        public List<User> Companies { get; set; }
        public User PersonalAreaUser { get; set; }
        public string Heading { get; set; }
        public bool IsRegularClients { get; set; }
    }
}