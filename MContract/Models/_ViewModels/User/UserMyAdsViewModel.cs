using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class UserMyAdsViewModel
	{
		public List<Ad> Ads { get; set; }
		public string Heading { get; set; }

		public AdsSliderViewModel AdsSliderViewModel { get; set; }

		public LeftMenuViewModel LeftMenuViewModel { get; set; }
		public MobileMenuViewModel MobileMenuViewModel { get; set; }
	}
}