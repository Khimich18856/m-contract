using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
    public class UserDealsHistoryViewModel
    {
		public User PersonalAreaUser { get; set; }

		public List<UserDealsHistoryItem> Deals { get; set; }

		public List<User> Counteragents { get; set; }

		public List<ProductCategory> ProductCategories { get; set; }

		public LeftMenuViewModel LeftMenuViewModel { get; set; }
		public MobileMenuViewModel MobileMenuViewModel { get; set; }
	}
}