using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class AdsNewAdViewModel
	{
		public List<ProductCategory> ProductCategories { get; set; }
		public User PersonalAreaUser { get; set; }
		public Ad Ad { get; set; }
		public bool IsBuy { get; set; }
		public bool AvailableForAllUsers { get; set; }
	}
}