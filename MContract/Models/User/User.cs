using MContract.AppCode;
using MContract.DAL;
using MContract.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class User 
	{
		public int Id { get; set; }
		public string CompanyName { get; set; }
		public string ContactName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public TypesOfOwnership TypeOfOwnership { get; set; }
		public int CityId { get; set; }
		public string INN { get; set; }
		public string OGRN { get; set; }
		public string PhoneNumber { get; set; }
		public string PhoneNumberCity { get; set; }
		public double Rating { get; set; }
		public string Address { get; set; }
		public string FactualAddress { get; set; }
		public List<User> RegularClients { get; set; }
		public ModerateResults ModerateResult { get; set; }
		public int CurrentRespondentId { get; set; }
		public DateTime Created { get; set; }
		public bool EmailConfirmed { get; set; }
		public DateTime? SubscribeUntilDate { get; set; }
		public bool Deleted { get; set; }

		#region Поля СБИС
		public bool CheckedInSbis { get; set; }
		public string SbisCompanyName { get; set; }
		public int? SbisTypeOfOwnershipId { get; set; }
		public string SbisOGRN { get; set; }
		public DateTime? SbisWorksFrom { get; set; }
		public int? SbisTownId { get; set; }

		/// <summary>
		/// Сфера деятельности компании, указанная в СБИС
		/// </summary>
		public string SbisScope { get; set; }
		#endregion

		/// <summary>
		/// Дата и время, когда пользователь был на сайте
		/// </summary>
		public DateTime LastOnline { get; set; }

        /// <summary>
        /// Krakoss.Encryption(user.INN).ToString() для подтверждения емайл нового пользователя
        /// </summary>
        public string VerificationCode { get; set; }

		//вычисляемые поля:
		public bool IsOnline
		{
			get
			{
				return (DateTime.Now.ToUniversalTime() - LastOnline).Minutes <= 1;
			}
		}

		public string SbisTypeOfOwnershipStr
		{
			get
			{
				if (!SbisTypeOfOwnershipId.HasValue)
					return String.Empty;

				return UserHelper.GetTypeOfOwnershipString((TypesOfOwnership)SbisTypeOfOwnershipId.Value);
			}
		}

		public string TypeOfOwnershipStr
		{
			get
			{
				return UserHelper.GetTypeOfOwnershipString(TypeOfOwnership);
			}
		}

		public bool SubscribeIsActive
		{
			get
			{
				if (SubscribeUntilDate == null)
					return false;

				return DateTime.Now.ToUniversalTime() <= SubscribeUntilDate.Value;
			}
		}

		private string _companyNameWithTypeOfOwnership;
		public string CompanyNameWithTypeOfOwnership
		{
			get
			{
				if (_companyNameWithTypeOfOwnership == null)
				{
					if (Id == SystemNotificationsUserId)
						_companyNameWithTypeOfOwnership = CompanyName;
					else
						_companyNameWithTypeOfOwnership = TypeOfOwnershipStr + " \"" + CompanyName + "\"";
				}

				return _companyNameWithTypeOfOwnership;
			}
			set
			{
				_companyNameWithTypeOfOwnership = value;
			}
		}
		public int AdsCount { get; set; }
		public int UnreadMessagesCount { get; set; }
		public string Url
		{
			get
			{
				return C.SiteUrlClear + "/User/Profile/" + Id;
			}
		}

		public Town Town { get; set; }

		public string TownName
		{
			get
			{
				return Town?.NameAndRegionName;
			}
		}

		public string SmallPhotoUrl { get; set; }

		public List<Photo> LogoGroup { get; set; }

		public bool IsAdmin {
			get
			{
				var separatorCharacter = ',';
				var adminUserIds = System.Configuration.ConfigurationManager.AppSettings["AdminUserIds"].Split(separatorCharacter);
				return adminUserIds.Contains(Id.ToString());
			}
		}
		public static int SystemNotificationsUserId {
			get {
				return 1011;
			}
		}
		public bool IsSystemNotifications {
			get {
				return Id == SystemNotificationsUserId;
			}
		}

		public List<Ad> FavoriteAds { get; set; }

		//для страницы логина
		public string ErrorMessage { get; set; }

		/// <summary>
		/// для поиска в профиле
		/// </summary>
		public List<User> AllUsers { get; set; }

		/// <summary>
		/// модель для передачи в контрол _PersonalArea на странице User/Profile - 
		/// </summary>
		public User PersonalAreaUser { get; set; }
		public Photo GetBestFitLogoPhoto(int requiredDimension)
		{
			if (LogoGroup != null && LogoGroup.Any())
			{
				var smallestDimensionDifference = LogoGroup.Min(p => Math.Abs(p.HigherDimension.Value - requiredDimension));
				return LogoGroup.Find(p => Math.Abs(p.HigherDimension.Value - requiredDimension) == smallestDimensionDifference);
			} else
			{
				return new Photo { IsNoLogoPlaceholder = true };
			}
		}
		public Photo GetOriginalLogoPhoto
		{
			get
			{
				if (LogoGroup != null && LogoGroup.Any())
				{
					var maxDimension = LogoGroup.Max(p => p.HigherDimension.Value);
					return LogoGroup.Find(p => p.HigherDimension.Value == maxDimension);
				} else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// мини-чаты в беззвучном режиме
		/// </summary>
		public bool ChatBoxIsSilent { get; set; }
		
		public int AchatBoxLastMessageId { get; set; }
		public int ChatBoxLastMessageId
        {
			get
			{
				if (AchatBoxLastMessageId == 0)
					AchatBoxLastMessageId = MessagesDAL.GetLastMessageId(SM.CurrentUserId);

				return AchatBoxLastMessageId;
            }
			set
			{
				AchatBoxLastMessageId = value;
			}
		}

		[JsonIgnore]
		public Dialog OpenDialog { get; set; }

		[JsonIgnore]
		public List<Dialog> Dialogs { get; set; }

		#region Для _PersonalArea.cshtml
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

		public string RegularClientsSelected { get; set; }
		//{
		//	get
		//	{
		//		return SelectedMenu == "Постоянные клиенты" ? "selected" : "";
		//	}
		//}

		public string RatingImagesHtml
		{
			get
			{
				var result = "";
				int ratingInt = (int)Math.Floor(Rating);
				for(int i = 1; i <= ratingInt && i <= 5; i++)
					result += "<img src=\"/ico/redStar.png\" alt = \"\" >";

				if(Rating - ratingInt >= 0.5)
					result += "<img src=\"/ico/redStar-half.png\" alt = \"\">";

				for (int i = 0; i < 5 - (ratingInt + 0.999999); i++)
					result += "<img src=\"/ico/redStar-none.png\" alt = \"\">";

				return result;
			}
		}

		public string RatingImagesViaDivsHtml
		{
			get
			{
				var result = "";
				int ratingInt = (int)Math.Floor(Rating);
				for (int i = 1; i <= ratingInt && i <= 5; i++)
					result += "<div class=\"ico-pc\"></div>";

				if (Rating - ratingInt >= 0.5)
					result += "<div class=\"ico-pc ico-half\"></div>";

				for (int i = 0; i < 5 - (ratingInt + 0.999999); i++)
					result += "<div class=\"ico-pc ico-none\"></div>";

				return result;
			}
		}
		#endregion

		public UnregisteredUser ToUnregisteredUser()
		{
			return new UnregisteredUser()
			{
				CompanyName = CompanyName,
				ContactName = ContactName,
				Email = Email,
				TypeOfOwnership = TypeOfOwnership,
				CityId = CityId,
				INN = INN,
				OGRN = OGRN,
				PhoneNumber = PhoneNumber,
				PhoneNumberCity = PhoneNumberCity,
				SbisCompanyName = SbisCompanyName,
				SbisTypeOfOwnershipId = SbisTypeOfOwnershipId,
				SbisOGRN = SbisOGRN,
				SbisWorksFrom = SbisWorksFrom
			};
		}
	}
}