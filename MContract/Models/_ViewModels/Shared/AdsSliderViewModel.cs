using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class AdsSliderViewModel
	{
		public List<Ad> Ads { get; private set; }
		public string Title { get; private set; }
		public int CurrentUserId { get; private set; }

		public AdsSliderViewModel(string title, int currentUserId, List<Ad> ads)
		{
			Title = title;
			CurrentUserId = currentUserId;
			Ads = ads;
		}
	}
}