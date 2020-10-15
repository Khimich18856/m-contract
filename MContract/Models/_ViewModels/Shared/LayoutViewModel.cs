using MContract.AppCode;
using MContract.Controllers;
using MContract.DAL;
using MContract.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace MContract.AppCode
{
	public class LayoutViewModel
	{
		public int CurrentUserId { get; set; }
		public string CurrentUserContactName { get; set; }
		public string SiteUrlClear { get; set; }
		public string Message { get; set; }
		public string MessageColor { get; set; }
		public bool Production { get; set; }
		public bool ShowSearchbar { get; set; }

		/// <summary>
		/// Для сохранения фильтров поиска
		/// </summary>
		public bool? SearchbarIsBuy { get; set; }
		public List<int> SearchbarCategoriesId { get; set; }
		public List<int> SearchbarCitiesId { get; set; }

		/// <summary>
		/// Скрыть шапку (в layout.cshtml)
		/// </summary>
		public bool HideHead { get; set; }
		public List<ProductCategory> ProductCategories { get; set; }
		public List<Town> Towns { get; set; }
		public List<Region> Regions { get; set; }
		public MContract.Models.User PersonalAreaUser {
			get
			{
				return SM.GetPersonalAreaUser();
			}
		}

		public List<User> Users
		{
			get
			{
				var currentUserId = SM.CurrentUserId;
				var users = SM.GetAllUsers().Where(u => u.Id != currentUserId && u.Id != User.SystemNotificationsUserId).ToList();
				var regularClients = UsersDAL.GetRegularClients(currentUserId);
				if (regularClients.Any())
				{
					var regularClientIds = regularClients.Select(u => u.Id).ToList();
					users = users.Where(u => regularClientIds.Any(id => id != u.Id)).ToList();
				}
				/*var dialogRespondentIds = MessagesDAL.GetMessages(currentUserId);
				if (dialogRespondentIds.Any())
				{
					foreach (var userId in dialogRespondentIds.GroupBy(m => m.SenderId).Select(g => g.Key).Where(id => id != currentUserId))
					{
						var user = users.FirstOrDefault(u => u.Id == userId);
						if (user == null)
							continue;
						var i = users.IndexOf(user);
						users.RemoveAt(i);
						users.Insert(0, user);
					}
				}
				var regularClientIds = UsersDAL.GetRegularClients(currentUserId);
				if (regularClientIds.Any())
				{
					foreach (var userId in regularClientIds.Select(u => u.Id))
					{
						var user = users.FirstOrDefault(u => u.Id == userId);
						if (user == null)
							continue;
						var i = users.IndexOf(user);
						users.RemoveAt(i);
						users.Insert(0, user);
					}
				}*/

				return users;
			}
		}

		//урлы:
		public string LoginUrl
		{
			get
			{
				return Urls.Login;
			}
		}

		public string LogoutUrl
		{
			get
			{
				return Urls.Logout;
			}
		}

		public string RegistrationUrl
		{
			get
			{
				return Urls.Registration;
			}
		}

		public string PersonalAreaUrl
		{
			get
			{
				return Urls.PersonalArea;
			}
		}

		public static LayoutViewModel GetLayoutViewModel()
		{
			var result = new LayoutViewModel()
			{
				CurrentUserId = SM.CurrentUserId,
				CurrentUserContactName = SM.CurrentUserContactName,
				SiteUrlClear = C.SiteUrlClear,
				Message = SM.Message,
				MessageColor = SM.MessageColor,
				Production = ConfigurationManager.AppSettings["production"] == "true",
				ShowSearchbar = true,
				SearchbarIsBuy = null,
				SearchbarCategoriesId = new List<int>(),
				SearchbarCitiesId = new List<int>(),
				// закэшировано, не бьет по производительности
				ProductCategories = ProductCategoriesDAL.GetCategories(),
				Towns = TownsDAL.GetTowns(),
				Regions = RegionsDAL.GetRegions()
			};
			return result;
		}
	}
}