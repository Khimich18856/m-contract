using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class MobileMenuViewModel
	{
		public int AdsCount { get; set; }
		public int UnreadMessagesCount { get; set; }

		#region выбранный пункт меню
		public string SelectedMenu { get; set; }

		public string ProfileDataSelected
		{
			get
			{
				return SelectedMenu == "Данные профиля" ? "selected" : "";
			}
		}

		public string MyAdsSelected
		{
			get
			{
				return SelectedMenu == "Мои объявления" ? "selected" : "";
			}
		}

		public string DealsHistorySelected
		{
			get
			{
				return SelectedMenu == "История сделок" ? "selected" : "";
			}
		}

		public string FavoritesSelected
		{
			get
			{
				return SelectedMenu == "Избранное" ? "selected" : "";
			}
		}

		public string MessagesSelected
		{
			get
			{
				return SelectedMenu == "Сообщения" ? "selected" : "";
			}
		}

		public string DraftsSelected
		{
			get
			{
				return SelectedMenu == "Черновики" ? "selected" : "";
			}
		}

		public string MyOffersSelected
		{
			get
			{
				return SelectedMenu == "Мои отклики" ? "selected" : "";
			}
		}

		public string RegularClientsSelected
		{
			get
			{
				return SelectedMenu == "Постоянные клиенты" ? "selected" : "";
			}
		}
		#endregion

		public static MobileMenuViewModel Get(LeftMenuViewModel leftMenuViewModel)
		{
			return new MobileMenuViewModel()
			{
				AdsCount = leftMenuViewModel.AdsCount,
				UnreadMessagesCount = leftMenuViewModel.UnreadMessagesCount,
				SelectedMenu = leftMenuViewModel.SelectedMenu
			};
		}
	}
}