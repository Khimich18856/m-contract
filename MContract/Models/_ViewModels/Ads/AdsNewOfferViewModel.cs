using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class AdsNewOfferViewModel
	{
		public Ad Ad { get; set; }

		public User CurrentUser { get; set; }
	}
}