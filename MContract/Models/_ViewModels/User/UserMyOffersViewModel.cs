using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class UserMyOffersViewModel
	{
		public List<Offer> IncomingOffers { get; set; }
		public List<Offer> OutgoingOffers { get; set; }
		//public User PersonalAreaUser { get; set; }

		public LeftMenuViewModel LeftMenuViewModel { get; set; }
		public MobileMenuViewModel MobileMenuViewModel { get; set; }
	}
}