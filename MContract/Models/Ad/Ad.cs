using MContract.AppCode;
using MContract.DAL;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MContract.Models
{
	public class Ad
	{
		//поля из БД:
		public int Id { get; set; }
		public int SenderId { get; set; }
		public int? ActivePeriod { get; set; }
		public DateTime DateOfPosting { get; set; }
		public DateTime? ActiveToDate { get; set; }
		public string Description { get; set; }
		public int CityId { get; set; }
		public DeliveryTypes DeliveryType { get; set; }
		public string DeliveryAddress { get; set; }
		public DeliveryLoadTypes DeliveryLoadType { get; set; }
		public DeliveryWays DeliveryWay { get; set; }
		public TermsOfPayments TermsOfPayments { get; set; }
		public int? DefermentPeriod { get; set; }
		public FormOfPayment? FormOfPayment { get; set; }
		public Nds Nds { get; set; }
		public bool PartOffersAllowed { get; set; }
		public bool OffersVisibleToOtherUsers { get; set; }
		public bool IsBuy { get; set; }
		public bool IsSendedExpiredMessage { get; set; }
		public bool ShowInDealsHistory { get; set; } = true;

		/// <summary>
		/// Объявления снято с публикации самим пользователем? Иначе истекло
		/// </summary>
		public bool IsUnpublishedByUser { get; set; }

		/// <summary>
		/// Открытый (true) или Закрытый (false) запрос предложений
		/// </summary>
		public bool AvailableForAllUsers { get; set; }
		public int ViewsCount { get; set; }
		public AdStatuses _adStatus;
		public AdStatuses AdStatus
		{
			get
			{
				if (_adStatus == AdStatuses.Published && ActiveToDate < DateTime.Now.ToUniversalTime())
				{
					return AdStatuses.Expired;
				}
				else
				{
					return _adStatus;
				}
			}
			set
			{
				_adStatus = value;
			}
		}
		public ModerateResults ModerateResult { get; set; }
		public List<int> InvitedUsersId { get; set; }

		//вычисляемые и заполняемые поля:
		public User Sender { get; set; }
		public User PersonalAreaUser { get; set; }

		public string Url
		{
			get
			{
				return Urls.Ads + "/" + (this.AdStatus == AdStatuses.Draft ? "NewAdFinalStep" : "Show") + "/" + this.Id;
			}
		}

		/// <summary>
		/// Урл для страницы Ads/Index. Если текущий пользователь не должен проваливаться в объявление, возвращаем пустой урл
		/// </summary>
		public string UrlForAdsIndex { get; set; }
		public string OnClickJsScript { get; set; }

		public string DealCardUrl
		{
			get
			{
				return C.SiteUrl + "User/DealCard?adId=" + Id;
			}
		}
		public List<Photo> Photos { get; set; }
		public List<Photo> MainPhotoGroup { get; set; }
		public List<Offer> Offers { get; set; }
		public List<AdProduct> Products { get; set; }
		public float SumOfProductWeights {
			get
			{
				if (Products != null && Products.Any())
					return Products.Sum(p => p.Weight);
				else
					return 0;
			}
		}
		public List<ProductCategory> ProductCategoriesLevel1 { get; set; }
		public string ProductCategoryNames { get; set; }
		public bool NotifyRegularClients { get; set; }
		public Town City { get; set; }
		
		/// <summary>
		/// модель для передачи в контрол _PersonalArea на странице Ads/Show - 
		/// </summary>
		//public User PersonalAreaUser { get; set; }

		/// <summary>
		/// Id пользователя, просматривающего страницу
		/// </summary>
		public int CurrentUserId
		{
			get
			{
				return SM.CurrentUserId;
			}
		}

		

		/// <summary>
		/// Является ли объявление моим (при просмотре на странице Ads/Show например)
		/// </summary>
		public bool IsMy
		{
			get
			{
				return SenderId == CurrentUserId;
			}
		}

		/// <summary>
		/// Количество пользователей, добавивших объявление в избранное
		/// </summary>
		public int FavoritesCount { get; set; }

		/// <summary>
		/// Является ли просматриваемое объявление (например на странице Ads/Show) добавленным текущим пользователем в избранное
		/// </summary>
		public bool IsInFavorites { get; set; }

		/// <summary>
		/// Показывать ли кнопку Повторить
		/// </summary>
		public bool ShowRepeatBtn { get; set; }

		/// <summary>
		/// для отображения предложения на странице Ads/Show
		/// </summary>
		public int? OfferIdToShow { get; set; }

		/// <summary>
		/// Похожие объявления для страницы Ads/Show
		/// </summary>
		public List<Ad> SimilarAds { get; set; }
		public string SmallPhotoUrl { get; set; }

		public string StatusTextForAdsIndex
		{
			get
			{
				if (AdStatus == AdStatuses.Finished)
					return "Заключен контракт";
				else if (DaysTillExpiration < 0)
					return "Объявление истекло";
				else
					return $"Истекает через: {DaysTillExpiration} дней";
			}
		}

		public string StatusTextForAdsShow
		{
			get
			{
				string result = String.Empty;
				if (Offers != null && Offers.Any(o => o.ContractStatus == ContractStatuses.Sent))
					result = "Ожидает подтверждения контракта";
				else if (AdStatus == AdStatuses.Published)
					result = "Активное";
				else if (AdStatus == AdStatuses.Finished)
					result = "Завершено (заключен контракт)";
				else if (AdStatus == AdStatuses.Expired)
					result = "Завершено (истек срок действия)";
				if (ModerateResult == ModerateResults.NotChecked)
					result += " (на модерации)";

				return result;
			}
		}

		public string AdStatusCssClass
		{
			get
			{
				string result = String.Empty;
				if (Offers != null && Offers.Any(o => o.ContractStatus == ContractStatuses.Sent))//ожидает подтверждения контракта
					return "wait";

				if (AdStatus == AdStatuses.Finished || AdStatus == AdStatuses.Expired)
					return "closed";

				return "";
			}
		}

		public int OffersCount
		{
			get
			{
				if (Offers == null)
					return 0;

				return Offers.Count;
			}
		}

		public bool IsContractSent
		{
			get
			{
				return Offers != null && Offers.Any(o => o.ContractStatus == ContractStatuses.Sent);
			}
		}

		public TimeSpan? TimeFromDateOfPosting
		{
			get
			{
				return DateTime.Now - DateOfPosting;
			}
		}

		public string TimeFromDateOfPostingDescription
		{
			get
			{
				if (TimeFromDateOfPosting != null)
				{
					var timeDifference = TimeFromDateOfPosting.Value;
					var description = "";
					if (timeDifference.TotalDays < 1)
						description += $"{timeDifference.Hours} часов назад";
					else if (timeDifference.TotalDays < 2)
						description += "Вчера";
					else
					{
						description += timeDifference.Days;
						if (timeDifference.Days < 5)
							description += " дня";
						else
							description += " дней";
						description += " назад";
					}

					return description;
				}
				else
					return "";
			}
		}

		public int DaysTillExpiration
		{
			get
			{
				if (ActiveToDate == null)
					return 999;

				var result = (ActiveToDate.Value - DateTime.Now).Days;

				return (ActiveToDate.Value - DateTime.Now).Days;
			}
		}

		public string NameWithoutOpenOrClose
		{
			get
			{
				return (IsBuy ? "Покупка" : "Продажа") + (Id > 0 ? $" №{Id}" : "");
			}
		}

		public string OpenOrClose
		{
			get
			{
				return AvailableForAllUsers ? "открытый запрос предложений" : "закрытый запрос предложений";
			}
		}
		/// <summary>
		/// Название объявления, пример: Продажа №1 (открытый запрос предложений)
		/// </summary>
		public string Name
		{
			get
			{
				return $"{NameWithoutOpenOrClose} ({OpenOrClose})";
			}
		}

		/// <summary>
		/// для приглашения участников в закрытые объявления
		/// </summary>
		public List<User> RegularClients { get; set; }

		/// <summary>
		/// Все-все пользователи из БД
		/// </summary>
		public List<User> AllUsers { get; set; }
		public List<Photo> GetPhotoGroupFromGroupId(Guid groupId)
		{
			if (Photos != null && Photos.Any())
			{
				return Photos.GroupBy(p => p.GroupId.ToString())
				.Select(g => new { Id = g.Key, Photos = g.Select(p => p).ToList() })
				.Where(g => g.Id == groupId.ToString())
				.FirstOrDefault()
				.Photos;
			} else
			{
				return new List<Photo>();
			}
		}
		public Photo GetBestFitPhotoFromGroupId(Guid groupId, int requiredDimension)
		{
			if (Photos != null && Photos.Any())
			{
				var group = GetPhotoGroupFromGroupId(groupId);
				var smallestDimensionDifference = group.Min(p => Math.Abs(p.HigherDimension.Value - requiredDimension));
				return group.Find(p => Math.Abs(p.HigherDimension.Value - requiredDimension) == smallestDimensionDifference);
			}
			else
			{
				return null;
			}
		}
		public Photo GetBestFitPhotoFromPhotoId(int photoId, int requiredDimension)
		{
			if (Photos != null && Photos.Any())
			{
				var groupId = Photos.Find(p => p.Id == photoId).GroupId;
				return GetBestFitPhotoFromGroupId(groupId, requiredDimension);
			}
			else
			{
				return null;
			}
		}
		public Photo GetOriginalPhotoFromGroupId(Guid groupId)
		{
			if (Photos != null && Photos.Any())
			{
				var group = GetPhotoGroupFromGroupId(groupId);
				var maxDimension = group.Max(p => p.HigherDimension.Value);
				return group.Find(p => p.HigherDimension.Value == maxDimension);
			}
			else
			{
				return null;
			}
		}
		public Photo GetOriginalPhotoFromPhotoId(int photoId)
		{
			if (Photos != null && Photos.Any())
			{
				var groupId = Photos.Find(p => p.Id == photoId).GroupId;
				return GetOriginalPhotoFromGroupId(groupId);
			}
			else
			{
				return null;
			}
		}

		public bool IsCreateOfferAvailable
		{
			get
			{
				bool result = true;
				switch (AdStatus)
				{
					case AdStatuses.Finished:
					case AdStatuses.Expired:
					case AdStatuses.Deleted:
						result = false;
						break;
				}

				return result;
			}
		}

		public string CreateOfferNotAvailableMessage
		{
			get
			{
				string result = "Невозможно отправить предложение - ";
				switch (AdStatus)
				{
					case AdStatuses.Finished:
						result += "по объявлению заключен контракт";
						break;
					case AdStatuses.Expired:
						result += "объявление истекло";
						break;
					case AdStatuses.Deleted:
						result += "объявление удалено";
						break;
				}

				return result;
			}
		}

		public string CategoriesMobileHtml
		{
			get
			{
				if (ProductCategoriesLevel1 == null)
					return "";

				var result = new StringBuilder();
				foreach (var parentCategory in ProductCategoriesLevel1)
				{
					result.Append($@"<span class=""name"">
										{parentCategory.Name}{(parentCategory.ChildCategories.Any() ? ": " : "")}<br>
									  </span>");

					int i = 0;
					foreach (var childCategory in parentCategory.ChildCategories)
					{
						//отображаем только 3 внутренние категории, если больше, то троеточие
						if (++i >= 4)
						{
							result.Append("...<br/>");
							break;
						}

						var isLast = i == parentCategory.ChildCategories.Count();
						var separator = isLast ? "<br/>" : "; ";

						result.Append($@"<span class=""consist"">{childCategory.Name}{separator}</span>");
					}
				}
				return result.ToString();
			}
		}

		public string CategoriesDesktopHtml
		{
			get
			{
				return CategoriesMobileHtml.Replace("\"name\"", "\"title\"").Replace("\"consist\"", "\"text\"");
			}
		}

		public string CategoriesIn1String
		{
			get
			{
				if (ProductCategoriesLevel1 == null)
					return "";

				var result = new StringBuilder();
				foreach (var parentCategory in ProductCategoriesLevel1)
				{
					result.Append(parentCategory.Name + (parentCategory.ChildCategories.Any() ? ": " : ""));

					if (parentCategory.ChildCategories.Any())
					{
						var lastChildCategory = parentCategory.ChildCategories.Last();
						int i = 0;
						foreach (var childCategory in parentCategory.ChildCategories)
						{
							//отображаем только 3 внутренние категории, если больше, то троеточие
							if (++i >= 4)
							{
								result.Append("...");
								break;
							}

							var separator = i == parentCategory.ChildCategories.Count ? "" : "; ";
							result.Append(childCategory.Name + separator);
						}
					}
				}
				return result.ToString();
			}
		}

		public string CategoriesIn1StringWithCosts
		{
			get
			{
				if (ProductCategoriesLevel1 == null)
					return "";

				var result = new StringBuilder();
				foreach (var parentCategory in ProductCategoriesLevel1)
				{
					result.Append(parentCategory.Name + (parentCategory.ChildCategories.Any() ? ": " : ""));

					if (parentCategory.ChildCategories.Any())
					{
						var lastChildCategory = parentCategory.ChildCategories.Last();
						foreach (var childCategory in parentCategory.ChildCategories)
						{
							var isChildLast = lastChildCategory == childCategory;
							result.Append(childCategory.Name + (isChildLast ? "" : "; "));
						}
					}
				}
				return result.ToString();
			}
		}

		public AdsSliderViewModel AdsSliderViewModel { get; set; }
	}
}