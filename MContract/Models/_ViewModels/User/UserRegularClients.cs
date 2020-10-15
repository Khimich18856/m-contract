using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class UserRegularClients
	{
		public LeftMenuViewModel LeftMenuViewModel { get; set; }
		public MobileMenuViewModel MobileMenuViewModel { get; set; }

		public List<User> RegularClients { get; set; }

		/// <summary>
		/// Модель для отображения блока "Вы недавно смотрели"
		/// </summary>
		public AdsSliderViewModel AdsSliderViewModel { get; set; }

		public int CurrentUserId { get; set; }
	}
}