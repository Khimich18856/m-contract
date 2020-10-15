using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class UserFavoritesViewModel
	{
		public List<Ad> Ads { get; set; }

		/// <summary>
		/// Объявления для блока "Вы недавно смотрели"
		/// </summary>
		public AdsSliderViewModel AdsSliderViewModel { get; set; }

		public LeftMenuViewModel LeftMenuViewModel { get; set; }
		public MobileMenuViewModel MobileMenuViewModel { get; set; }

		public int CurrentUserId { get; set; }
	}
}