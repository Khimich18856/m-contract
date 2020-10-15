using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class UserCounteragentProfileViewModel
	{
		public LeftMenuViewModel LeftMenuViewModel { get; set; }
		public MobileMenuViewModel MobileMenuViewModel { get; set; }
		public User CurrentUser { get; set; }
	}
}