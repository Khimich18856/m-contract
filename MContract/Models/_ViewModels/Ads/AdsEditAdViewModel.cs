using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
    public class AdsEditAdViewModel
    {
        public Ad Ad { get; set; }
        public User PersonalAreaUser { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}