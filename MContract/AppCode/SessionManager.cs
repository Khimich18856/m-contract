using krakosssecurity;
using MContract.DAL;
using MContract.Models;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.AppCode
{
	public class SM
	{
		public static List<User> GetAllUsers()
		{
			var allUsers = (List<User>)HttpContext.Current.Session["AllUsers"];
			if (allUsers != null && allUsers.Any())
				return allUsers;
			allUsers = UsersDAL.GetUsers();
			foreach (var user in allUsers)
			{
				user.Town = TownsDAL.GetTown(user.CityId);
				user.LogoGroup = PhotosDAL.GetCompanyLogoGroup(user.Id);
			}
			HttpContext.Current.Session["AllUsers"] = allUsers;
			return allUsers;
		}

		/// <summary>
		///  НЕ ИСПОЛЬЗОВАТЬ. ПО МЕРЕ ВОЗМОЖНОСТИ УДАЛИТЬ ЭТОТ МЕТОД. Вместо него сейчас LeftMenuViewModel
		/// </summary>
		/// <param name="selectedMenu"></param>
		/// <returns></returns>
		public static User GetPersonalAreaUser(string selectedMenu = null)
		{
			if (CurrentUserIsNull)
				return new User { Id = 0 };

			var user = (User)HttpContext.Current.Session["PersonalAreaUser"];
			if (user != null)
			{
				//if (NeedRefreshUnreadMessagesCount)
				//{
					user.UnreadMessagesCount = MessagesDAL.GetUnreadMessagesCount(user.Id);
					//NeedRefreshUnreadMessagesCount = false;
				//}
				user.AdsCount = AdsDAL.GetAdsCount((int)AdStatuses.Published, user.Id);
				//user.Dialogs = SM.CurrentDialogs;
				//if (user.Dialogs != null && user.Dialogs.Any())
				//{
				//	user.OpenDialog = user.Dialogs.FirstOrDefault(d => d.Respondent?.Id == user.CurrentRespondentId);
				//	if (user.OpenDialog != null)
				//	{
				//		if (user.OpenDialog.Messages == null || !user.OpenDialog.Messages.Any())
				//			user.OpenDialog.Messages = MessagesDAL.GetMessages(user.Id, user.CurrentRespondentId);
				//	}
				//	MessagesDAL.MarkMessagesAsRead(SM.CurrentUserId, user.CurrentRespondentId);
				//	/*else if (user.CurrentRespondentId > 0)
				//	{
				//		user.OpenDialog = new Dialog { Respondent = UsersDAL.GetUser(user.CurrentRespondentId), Messages = MessagesDAL.GetMessages(user.Id, user.CurrentRespondentId) };
				//		user.Dialogs.Add(user.OpenDialog);
				//		SM.CurrentDialogs = user.Dialogs;
				//	}*/
				//}
			}
			else
			{
				user = CurrentUser;
				user.Town = TownsDAL.GetTown(user.CityId);
				user.LogoGroup = PhotosDAL.GetCompanyLogoGroup(user.Id);
				user.SmallPhotoUrl = user.GetBestFitLogoPhoto(1).Url;
				user.AdsCount = AdsDAL.GetAdsCount((int)AdStatuses.Published, user.Id);
				user.UnreadMessagesCount = MessagesDAL.GetUnreadMessages(user.Id).Count;
				//user.Dialogs = SM.CurrentDialogs;
				//if (user.Dialogs != null && user.Dialogs.Any())
				//{
				//	user.OpenDialog = user.Dialogs.FirstOrDefault(d => d.Respondent?.Id == user.CurrentRespondentId);
				//	if (user.OpenDialog != null)
				//	{
				//		if (user.OpenDialog.Messages == null || !user.OpenDialog.Messages.Any())
				//			user.OpenDialog.Messages = MessagesDAL.GetMessages(user.Id, user.CurrentRespondentId);
				//	}
				//}
				user.RegularClients = UsersDAL.GetRegularClients(user.Id);
				if (user.RegularClients.Any())
				{
					var regularClientIds = user.RegularClients.Select(c => c.Id).ToList();
					var clientPhotos = PhotosDAL.GetPhotos(userIds: regularClientIds);
					foreach (var client in user.RegularClients)
					{
						client.Town = TownsDAL.GetTown(client.CityId);
						if (clientPhotos.Any())
						{
							client.LogoGroup = clientPhotos.Where(p => p.UserId == client.Id && p.AdId == 0).ToList();
						}
					}
				}
				HttpContext.Current.Session["PersonalAreaUser"] = user;
			}

			user.RegularClientsSelected = "";
			user.SelectedMenu = selectedMenu;
			
			return user;
		}

		public static bool UpdatePersonalAreaUser(User user)
		{
			HttpContext.Current.Session["PersonalAreaUser"] = user;
			return true;
		}

		//public static bool NeedRefreshUnreadMessagesCount
		//{
		//	get
		//	{
		//		return (bool?)HttpContext.Current.Session["NeedRefreshUnreadMessagesCount"] ?? true;
		//	}
		//	set
		//	{
		//		HttpContext.Current.Session["NeedRefreshUnreadMessagesCount"] = value;
		//	}
		//}

		public static List<Dialog> CurrentDialogs
		{
			get
			{
				var sessionDialogs = (List<Dialog>)HttpContext.Current.Session["CurrentDialogs"];
				if (sessionDialogs != null && sessionDialogs.Any())
				{
					return sessionDialogs;
				}

				var currentUserId = CurrentUserId;
				var openDialogRespondentIds = UsersDAL.GetOpenDialogRespondentIds(currentUserId);
				/*sessionDialogs = MessagesDAL.GetMessages(currentUserId).GroupBy(m => m.SenderId)
					.Where(g => openDialogRespondentIds.Any(id => id == g.Key) && g.Key != currentUserId)
					.Select(g => new Dialog { SenderId = g.Key }).ToList();*/
				if (openDialogRespondentIds.Any())
					sessionDialogs = openDialogRespondentIds.Select(id => new Dialog { SenderId = id }).ToList();
				/*foreach (var company in UsersDAL.GetRegularClients(currentUserId))
				{
					if (!sessionDialogs.Any(d => d.SenderId == company.Id))
						sessionDialogs.Add(new Dialog { SenderId = company.Id, RecipientId = currentUserId });
				}*/
				if (sessionDialogs != null && sessionDialogs.Any())
				{
					var userIds = sessionDialogs.Select(d => d.SenderId).ToList();
					var users = UsersDAL.GetUsers(userIds);
					var messages = MessagesDAL.GetMessages(currentUserId);
					var messageIds = messages.Select(m => m.Id).ToList();
					var messageFiles = FilesDAL.GetFiles(messageIds: messageIds);
					if (messageFiles.Any())
						messages.ForEach(m => m.Files = messageFiles.Where(f => f.MessageId == m.Id).ToList());
					foreach (var dialog in sessionDialogs)
					{
						var respondentId = dialog.SenderId;
						dialog.Respondent = users.FirstOrDefault(u => u.Id == respondentId);
						if (dialog.Respondent == null)
							continue;
						dialog.Respondent.LogoGroup = PhotosDAL.GetCompanyLogoGroup(respondentId);
						dialog.Respondent.SmallPhotoUrl = dialog.Respondent.GetBestFitLogoPhoto(1).Url;
						dialog.Messages = messages.Where(m => m.SenderId == respondentId || m.RecipientId == respondentId).ToList();
					}
					sessionDialogs = sessionDialogs.OrderBy(d => 
							(d.Messages.LastOrDefault() != null 
								? d.Messages.LastOrDefault().Date 
								: DateTime.MinValue)).ToList();
				}
				CurrentDialogs = sessionDialogs;

				return sessionDialogs;
			}
			set
			{
				HttpContext.Current.Session["CurrentDialogs"] = value;
			}
		}

		public static bool AddMessageToCurrentDialogs(Message message)
		{

			var dialogs = CurrentDialogs;
			var dialog = dialogs.FirstOrDefault(d => d.Respondent?.Id == message.RecipientId);
			if (dialog != null)
			{
				if (dialog.Messages.Any())
					dialog.Messages.Add(message);
				CurrentDialogs = dialogs;
				return true;
			} else
				return false;
		}

		public static List<int> ChatBoxRemovedDialogIds
		{
			get
			{
				var joinedIds = (string)HttpContext.Current.Session["DialogsRemovedFromChatBox"];
				if (string.IsNullOrEmpty(joinedIds))
					return new List<int>();

				var ids = joinedIds.Split(',').Select(id => int.Parse(id)).ToList();
				return ids;
			}
			set
			{
				HttpContext.Current.Session["DialogsRemovedFromChatBox"] = string.Join(",", value);
			}
		}

		public static User CurrentUser
		{
			get
			{
				if (HttpContext.Current != null && HttpContext.Current.Session["CurrentUserId"] != null)
				{
					User result = UsersDAL.GetUser((int)HttpContext.Current.Session["CurrentUserId"]);
					if (result == null)
					{
						//LogsDAL.AddError("in App_Code/BLL/SessionManager.cs: CurrentUser.get(): cleared Session for UserID = " + HttpContext.Current.Session["CurrentUserId"]);
						HttpContext.Current.Session.Clear();
					}
					return result;
				}
				else
					return null;
			}
			set
			{
				if (value != null)
				{
					HttpContext.Current.Session["CurrentUserId"] = value.Id;
					HttpContext.Current.Session["CurrentUserContactName"] = value.ContactName;
				}
				else
				{
					HttpContext.Current.Session["CurrentUserId"] = null;
					HttpContext.Current.Session["CurrentUserContactName"] = null;
				}
			}
		}

		public static bool CurrentUserIsNotNull
		{
			get
			{
				return HttpContext.Current.Session["CurrentUserId"] != null;
			}
		}

		public static bool CurrentUserIsNull
		{
			get
			{
				return HttpContext.Current.Session["CurrentUserId"] == null;
			}
		}

		public static int CurrentUserId
		{
			get
			{
				if (HttpContext.Current.Session["CurrentUserId"] != null)
					return (int)HttpContext.Current.Session["CurrentUserId"];

				return 0;
			}

			set
			{
				HttpContext.Current.Session["CurrentUserId"] = value;
			}
		}

		public static string CurrentUserContactName
		{
			get
			{
				if (HttpContext.Current.Session["CurrentUserContactName"] != null)
					return (string)HttpContext.Current.Session["CurrentUserContactName"];

				if (CurrentUserId > 0)
				{
					var currentUser = CurrentUser;
					if (currentUser != null)
					{
						HttpContext.Current.Session["CurrentUserContactName"] = currentUser.ContactName;
						return currentUser.ContactName;
					}
				}

				return null;
			}
		}

		public static string CurrentUserUrl
		{
			get
			{
				if (HttpContext.Current.Session["CurrentUserUrl"] != null)
					return (string)HttpContext.Current.Session["CurrentUserUrl"];

				if (CurrentUserId > 0)
				{
					var currentUser = CurrentUser;
					if (currentUser != null)
					{
						HttpContext.Current.Session["CurrentUserUrl"] = currentUser.Url;
						return currentUser.Url;
					}
				}

				return null;
			}
		}

		public static bool CurrentUserIsSupport
		{
			get
			{
				return (int?)HttpContext.Current.Session["CurrentUserId"] == 1 || (int?)HttpContext.Current.Session["CurrentUserId"] == 121;//support - User with id = 1
			}
		}

		public static DateTime LoginTime
		{
			get
			{
				if (HttpContext.Current.Session["LoginTime"] != null)
					return (DateTime)HttpContext.Current.Session["LoginTime"];
				else
					return new DateTime(1900, 1, 1);
			}
			set
			{
				HttpContext.Current.Session["LoginTime"] = value;
			}
		}

		public static bool JustLogined
		{
			get
			{
				return (DateTime.Now - LoginTime).TotalMinutes < 1;
			}
		}

		public static void TryToLoginByCookiesIfNeed()
		{
			if (CurrentUserIsNull)
			{
				var userCookie = CookiesHelper.GetUserFromCookies();
				if (userCookie != null)
				{
					User user = null;
					if (!string.IsNullOrWhiteSpace(userCookie.Email))
						user = UsersDAL.GetUserByEmail(userCookie.Email);

					if (user != null)
					{
						var realHashPassword = CookiesHelper.GetHash(Krakoss.Decryption(user.Password).TrimEnd());
						if (userCookie.HashPassword == realHashPassword)
							CurrentUser = user;
					}
				}
			}
		}

		public static int? AllNewThemesCount
		{
			get
			{
				if (HttpContext.Current.Session["AllNewThemesCount"] != null)
					return (int)HttpContext.Current.Session["AllNewThemesCount"];

				return null;
			}

			set
			{
				HttpContext.Current.Session["AllNewThemesCount"] = value;
			}
		}

		public static string Message
		{
			get
			{
				var message = (string)HttpContext.Current.Session["Message"];
				HttpContext.Current.Session["Message"] = null;
				return message;
			}

			set
			{
				HttpContext.Current.Session["Message"] = value;
			}
		}

		public static string MessageColor
		{
			get
			{
				var messageColor = (string)HttpContext.Current.Session["MessageColor"];
				HttpContext.Current.Session["MessageColor"] = null;
				return messageColor;
			}

			set
			{
				HttpContext.Current.Session["MessageColor"] = value;
			}
		}

		public static int? NotificationsCount
		{
			get
			{
				if (HttpContext.Current.Session["NotificationsCount"] != null)
					return (int)HttpContext.Current.Session["NotificationsCount"];

				return null;
			}

			set
			{
				HttpContext.Current.Session["NotificationsCount"] = value;
			}
		}

		public static void Debug(string message)
		{
			if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request["debug"] == "true")
			{
				DateTime now = DateTime.Now;
				HttpContext.Current.Session["debug"] = (HttpContext.Current.Session["debug"] as String) + "<br/>" + now.ToString() + "." + now.Millisecond + " " + message;
			}
		}
	}
}