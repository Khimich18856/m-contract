using MContract.DAL;
using MContract.Models;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.AppCode
{
	public class AdHelper
	{
		/// <summary>
		/// Возвращает объявления с всей необходимой информацией для представлений
		/// </summary>
		/// <param name="adIds"></param>
		/// <returns></returns>
		public static List<Ad> GetAdsForView(int currentUserId, List<int> adIds = null, List<Ad> ads = null, bool needInvitedUserIds = false, bool needSender = false)
		{
			if (adIds == null && ads == null)
				throw new ArgumentNullException("Нужно передать или adIds или ads");

			if (ads == null)
				ads = AdsDAL.GetAds(ids: adIds);
			else
				adIds = ads.Select(a => a.Id).Distinct().ToList();

			ads.ForEach(f => f.City = TownsDAL.GetTown(f.CityId));//города кэшируются внутри TownsDAL, запросов в БД нет
			var smallMainPhotos = PhotosDAL.GetPhotos(adIds: adIds, maxWidth: 300);
			var userFavoriteAds = UserFavoriteAdsDAL.GetUserFavoriteAds(adIds: adIds);
			var offersForAds = OffersDAL.GetOffers(adIds: adIds);
			var adProductsForads = AdProductsDAL.GetAdProducts(adIds: adIds);
			var allProductCategories = ProductCategoriesDAL.GetCategories();
			foreach (var ad in ads)
			{
				var smallMainPhoto = smallMainPhotos.FirstOrDefault(p => p.AdId == ad.Id);
				ad.SmallPhotoUrl = smallMainPhoto != null ? smallMainPhoto.Url : PhotoHelper.NoLogoImageUrl;

				ad.FavoritesCount = userFavoriteAds.Count(a => a.AdId == ad.Id);
				ad.IsInFavorites = userFavoriteAds.Any(f => f.AdId == ad.Id && f.UserId == currentUserId);
				ad.Offers = offersForAds.Where(o => o.AdId == ad.Id).ToList();
				ad.Products = adProductsForads.Where(ap => ap.AdId == ad.Id).ToList();

				if (ad.Products.Any())
				{
					#region Заполнение описания
					System.Text.StringBuilder productCategoryNames = new System.Text.StringBuilder();
					var adProductCategoryIds = ad.Products.Select(p => p.ProductCategoryId).ToList();
					var adProductCategories =
						allProductCategories
							.Where(c => adProductCategoryIds.Any(id => c.Id == id))
							.OrderBy(c => c.Id).ToList();
					ad.ProductCategoryNames = "";
					if (adProductCategories.Any())
					{
						var level1Categories = new List<ProductCategory>(); //категории 1-го уровня для всех категорий, выбранных в объявлении
						#region Заполним  level1Categories
						foreach (var category in adProductCategories)
						{
							var level1CategoryIds = level1Categories.Select(c => c.Id).ToList();
							if (category.Level == 1)
							{
								if (!level1CategoryIds.Contains(category.Id))
									level1Categories.Add(category);
							}
							else //Level = 2 или Level = 3
							{
								var parentCategory = allProductCategories.FirstOrDefault(c => c.Id == category.ParentId);
								if (parentCategory != null)
								{
									if (category.Level == 2)
									{
										if (!level1CategoryIds.Contains(parentCategory.Id))
											level1Categories.Add(parentCategory);
									}
									else //Level = 3
									{
										var parentParentCategory = allProductCategories.FirstOrDefault(c => c.Id == parentCategory.ParentId);
										if (parentParentCategory != null && !level1CategoryIds.Contains(parentParentCategory.Id))
											level1Categories.Add(parentParentCategory);
									}
								}
							}
						}
						#endregion

						ad.ProductCategoriesLevel1 = new List<ProductCategory>();
						foreach (var level1Category in level1Categories)
						{
							//нужно клонировать, иначе при обработке следующих объявлений в цикле эти данные затрутся
							var level1CategoryClone = level1Category.Clone();
							//выбираем дочерние категории и дочерние категории дочерних категорий
							var childCategories = adProductCategories.Where(c => c.ParentId == level1CategoryClone.Id || level1CategoryClone.ChildrenId.Any(id => id == c.ParentId)).ToList();
							level1CategoryClone.ChildCategories = new List<ProductCategory>();
							foreach (var childCategory in childCategories)
							{
								var childCategoryClone = childCategory.Clone();
								level1CategoryClone.ChildCategories.Add(childCategoryClone);
							}
							ad.ProductCategoriesLevel1.Add(level1CategoryClone);
						}
					}
					#endregion
				}

				if (needInvitedUserIds)
					ad.InvitedUsersId = AdsDAL.GetInvitedUsers(ad.Id);

				if (needSender)
				{
					ad.Sender = UsersDAL.GetUser(ad.SenderId);
					if (ad.Sender != null)
						ad.Sender.Town = TownsDAL.GetTown(ad.Sender.CityId);
				}
			}

			return ads;
		}

		/// <summary>
		/// Возвращает список объявлений для блока "Вы недавно смотрели"
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static List<Ad> GetRecentlyViewedAds(int userId)
		{
			var adViews = AdViewsDAL.GetAdViews(userId: userId, createdFrom: DateTime.Now.AddDays(-30)).OrderByDescending(v => v.Created).ToList();
			var recentlyViewedAdIds = adViews.Select(v => v.AdId).Distinct().Take(5).ToList();
			return GetAdsForView(userId, recentlyViewedAdIds);
		}

		/// <summary>
		/// Получает строку Условия поставки
		/// </summary>
		public static string GetDeliveryTypeString(DeliveryTypes deliveryType)
		{
			switch (deliveryType)
			{
				case DeliveryTypes.Any: return "Не выбрано";
				case DeliveryTypes.DeliveryBySeller: return "Доставка продавцом";
				case DeliveryTypes.PickupByBuyer: return "Самовывоз покупателем";
				default: throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Получает строку Погрузка
		/// </summary>
		public static string GetDeliveryLoadTypeString(DeliveryLoadTypes deliveryLoadType)
		{
			switch (deliveryLoadType)
			{
				case DeliveryLoadTypes.Any: return "Не выбрано";
				case DeliveryLoadTypes.LoadBySeller: return "Силами продавца";
				case DeliveryLoadTypes.LoadByBuyer: return "Силами покупателя";
				default: throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Получает строку Способ доставки
		/// </summary>
		public static string GetDeliveryWayString(DeliveryWays deliveryWay)
		{
			switch (deliveryWay)
			{
				case DeliveryWays.Any: return "Не выбрано";
				case DeliveryWays.Auto: return "Авто";
				case DeliveryWays.Railroad: return "Ж/Д";
				default: throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Получает строку Цена (С НДС/Без НДС)
		/// </summary>
		public static string GetNdsString(Nds nds)
		{
			switch (nds)
			{
				case Nds.Any: return "Не выбрано";
				case Nds.IsIncluded: return "С НДС";
				case Nds.IsNotIncluded: return "Без НДС";
				default: throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Получает строку Условия оплаты
		/// </summary>
		public static string GetTermsOfPaymentsString(TermsOfPayments termOfPayment)
		{
			switch(termOfPayment)
			{
				case TermsOfPayments.Any: return "Не выбрано";
				case TermsOfPayments.DefermentOfPayment: return "Отсрочка платежа";
				case TermsOfPayments.FullPrePayment: return "100% предоплата";
				case TermsOfPayments.PartialPrePayment: return "Частичная предоплата";
				case TermsOfPayments.OnArrival: return "По факту поставки";
				default: throw new NotImplementedException();
			}
		}
	}
}