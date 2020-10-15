using MContract.AppCode;
using MContract.DAL;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class LeftMenuViewModel
	{
		public string CompanyNameWithTypeOfOwnership { get; set; }
		public string Rating { get; set; }
		public string RatingImagesHtml { get; set; }
		public string SmallPhotoUrl { get; set; }
		public int AdsCount { get; set; }
		public int UnreadMessagesCount { get; set; }
		public int CurrentUserId { get; set; }

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

		public static LeftMenuViewModel Get(User user, string selectedMenu)
		{
			var result = new LeftMenuViewModel()
			{
				CompanyNameWithTypeOfOwnership = user.CompanyNameWithTypeOfOwnership,
				Rating = user.Rating.ToString(),
				RatingImagesHtml = user.RatingImagesHtml,
				SelectedMenu = selectedMenu,
				CurrentUserId = user.Id
			};

			if (user.SmallPhotoUrl != null)
				result.SmallPhotoUrl = user.SmallPhotoUrl;
			else
			{
				//var photos = PhotosDAL.GetCompanyLogoGroup(user.Id).OrderBy(p => p.Width).ToList();
				//result.SmallPhotoUrl = photos.Any() ? photos.First().Url : PhotoHelper.NoLogoImageUrl;
				user.LogoGroup = PhotosDAL.GetCompanyLogoGroup(user.Id);
				result.SmallPhotoUrl = user.GetBestFitLogoPhoto(1).Url;
			}

			result.AdsCount = AdsDAL.GetAdsCount((int)AdStatuses.Published, user.Id);
			result.UnreadMessagesCount = MessagesDAL.GetUnreadMessagesCount(user.Id);

			return result;
		}
	}
}