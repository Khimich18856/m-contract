using MContract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using MContract.DAL;
using MContract.AppCode;
using MContract.Models.Enums;
using System.IO;
using System.Threading;

namespace MContract.Controllers
{
    public class AdsController : Controller
    {
		public AdsController()
		{
			#region Общий код для всех контроллеров
			SM.TryToLoginByCookiesIfNeed();
			ViewBag.L = LayoutViewModel.GetLayoutViewModel();
			#endregion
		}

		public ActionResult IndexOld(int adStatusId = (int)AdStatuses.Published, string searchQuery = null, List<int> categoriesId = null, List<int> citiesId = null, bool? isBuy = null)
		{
			ViewBag.L.SearchbarCategoriesId = categoriesId;
			ViewBag.L.SearchbarCitiesId = citiesId;
			return Index(adStatusId, searchQuery, isBuy);
		}

		///?selectedCategoryIds=&selectedTownIds=&selectedRegionIds=
		public ActionResult Index(int adStatusId = (int)AdStatuses.Published, string searchQuery = null, bool? isBuy = null, string selectedCategoryIds = null, string selectedTownIds = null, string selectedRegionIds = null)
        {


            ViewBag.L.SearchbarIsBuy = isBuy;
            var viewModel = new AdsIndexViewModel();
			var currentUser = SM.GetPersonalAreaUser();
			viewModel.PersonalAreaUser = currentUser;
			int currentUserId = currentUser != null ? currentUser.Id : 0;

			#region Получение List<int> categoryIds, townIds, regionIds из строк selectedCategoryIds, selectedTownIds, selectedRegionIds

			List<int> categoryIds = null;
			if (!String.IsNullOrWhiteSpace(selectedCategoryIds))
			{
				categoryIds = new List<int>();
				var categoryIdsParts = selectedCategoryIds.Split(',');
				foreach (var categoryIdStr in categoryIdsParts)
				{
					categoryIds.Add(Convert.ToInt32(categoryIdStr));
				}
			}

			List<int> townIds = null;
			if (!String.IsNullOrWhiteSpace(selectedTownIds))
			{
				townIds = new List<int>();
				var townIdsParts = selectedTownIds.Split(',');
				foreach (var townIdStr in townIdsParts)
				{
					townIds.Add(Convert.ToInt32(townIdStr));
				}
			}

			var allTowns = TownsDAL.GetTowns();
			var allRegions = RegionsDAL.GetRegions();
			
			if (!String.IsNullOrWhiteSpace(selectedRegionIds))
			{
				if (townIds == null)
					townIds = new List<int>();

				var regionIdsParts = selectedRegionIds.Split(',');
				foreach (var regionIdStr in regionIdsParts)
				{
					int regionId = Convert.ToInt32(regionIdStr);
					var region = allRegions.FirstOrDefault(r => r.Id == regionId);
					if (region != null)
					{
						var townsInRegion = allTowns.Where(t => t.RegionName == region.Name).ToList();
						var townIdsForRegion = townsInRegion.Select(t => t.Id).ToList();
						townIds.AddRange(townIdsForRegion);
					}
				}
			}

			#endregion

			#region Запрос в DAL по входным параметрам

			viewModel.Ads = new List<Ad>();
            viewModel.IsSearch = true;
            if (townIds != null && townIds.Any())
            {
                
                var regions = allTowns.GroupBy(t => t.RegionName);
                var towns = allTowns.Where(t => townIds.Any(id => id == t.Id));
				townIds = regions.Where(r => towns.Any(t => t.RegionName == r.First().RegionName)).SelectMany(g => g.ToList()).Select(t => t.Id).ToList();
            }
            viewModel.Ads = AdsDAL.GetAdsForSearchPage(adStatusId, categoryIds, townIds, isBuy).Where(a => a.DaysTillExpiration >= 0).ToList();
            #endregion

            if (viewModel.Ads.Any())
            {
				var adIds = viewModel.Ads.Select(a => a.Id).Distinct().ToList();
				viewModel.Ads = viewModel.Ads
					.GroupBy(a => a.Id).Select(g => g.FirstOrDefault())
					.OrderByDescending(a => a.DateOfPosting).ToList();

			    viewModel.Ads = viewModel.Ads.Where(a => a.ModerateResult == ModerateResults.Accepted).ToList();

                var searchQueries = new string[] { };
                var matchingAds = new List<Ad>();
                if (searchQuery != null)
                {
                    searchQuery = System.Text.RegularExpressions.Regex.Replace(searchQuery.ToLower(), @"[^а-яa-z0-9]", ",");
                    searchQueries = searchQuery.Split(',');
                }

				viewModel.Ads = AdHelper.GetAdsForView(currentUserId, ads: viewModel.Ads, needInvitedUserIds: true, needSender: true);

                foreach (var ad in viewModel.Ads)
                {
					#region Заполнение UrlForAdsIndex, OnClickJsScript
					string adUrl = "#";
					string adOnclick = "";
					if (ad.AvailableForAllUsers/*если открытый запрос предложений*/ || ad.SenderId == currentUserId)
						adUrl = ad.Url;
					else//иначе закрытый запрос предложений
					{
						if (currentUser != null && ad.InvitedUsersId != null && ad.InvitedUsersId.Contains(currentUser.Id))
							adUrl = ad.Url;
						else
						{
							if (currentUser == null || currentUser.Id == 0)
								adOnclick = "alert('Это закрытое объявление, чтобы отправить свою заявку к участию, сначала авторизуйтесь')";
							else
								adOnclick = $"sendRequestForJoinAd({ad.Id})";
						}
					}
					ad.UrlForAdsIndex = adUrl;
					ad.OnClickJsScript = adOnclick;
					#endregion

					if (searchQueries.Any())
                    {
                        if (searchQueries.Any(q => string.IsNullOrEmpty(ad.Description) ? false : ad.Description.Contains(q) || 
                                                   string.IsNullOrEmpty(ad.ProductCategoryNames) ? false : ad.ProductCategoryNames.Contains(q)))
                            matchingAds.Add(ad);
                    }
                }
                if (searchQueries.Any())
                    viewModel.Ads = matchingAds;
            }

            #region Заполнение заголовка
   //         var heading = "";
   //         if (isBuy == true)
   //             heading += "Объявления о закупках";
   //         else if (isBuy == false)
   //             heading += "Объявления о продажах";
   //         else
   //             heading += "Все объявления";
   //         ViewBag.HeadingMainPart = heading;
   //         if (citiesId != null && citiesId.Any())
   //         {
   //             heading += " в ";
   //             var allTowns = TownsDAL.GetTowns();
   //             var allTownGroups = allTowns.GroupBy(t => t.RegionName).ToList();
   //             var towns = allTowns.Where(t => citiesId.Any(id => id == t.Id)).ToList();
   //             var regions = towns.GroupBy(t => t.RegionName).Select(g => new Town { Name = g.First().RegionName });
   //             heading += string.Join(", ", regions.Take(3).Select(t => t.Name));
   //             if (regions.Count() > 3)
   //                 heading += " и еще " + (regions.Count() - 3) + " регионах";
   //         }
   //         if (categoriesId != null && categoriesId.Any())
   //         {
   //             heading += " категории ";
   //             var allCategories = ProductCategoriesDAL.GetCategories();
   //             var categories = allCategories.Where(c => categoriesId.Any(id => id == c.Id) && !categoriesId.Any(id => id == c.ParentId));
   //             heading += string.Join(", ", categories.Take(3).Select(c => c.Name));
   //             if (categories.Count() > 3)
   //                 heading += " и еще " + (categories.Count() - 3) + " категорий";
   //         }
   //         ViewBag.HeadingExtraPart = heading.Substring(ViewBag.HeadingMainPart.Length);

			//heading = "Все объявления";
			//if (isBuy.HasValue)
			//	heading = isBuy.Value ? "Закупки" : "Продажи";
			//viewModel.Heading = heading;
			#endregion

			return View(viewModel);
        }
        public ActionResult Show(int id, int? offerId)
        {
            var ad = AdsDAL.GetAd(id);
            if (ad == null)
            {
                return RedirectToAction("Index");
            }
            #region Заполнение полей объявления
            if (offerId != null)
            {
                ad.OfferIdToShow = offerId;
            }
            ad.Sender = UsersDAL.GetUser(ad.SenderId);
            if (ad.Sender != null)
			{
				ad.Sender.Town = TownsDAL.GetTown(ad.Sender.CityId);
				ad.Sender.SmallPhotoUrl = ad.Sender.GetBestFitLogoPhoto(200).Url;
			}
                
            ad.City = TownsDAL.GetTown(ad.CityId);
            ad.Products = AdProductsDAL.GetAdProducts(ad.Id);
            if (ad.Products.Any())
            {
                foreach (var product in ad.Products)
                {
                    product.ProductCategoryName = ProductCategoriesDAL.GetProductCategory(product.ProductCategoryId).Name;
                }
            }
			ad.FavoritesCount = AdsDAL.GetFavoritesCount(ad.Id);
			if (SM.CurrentUserIsNotNull)
				ad.IsInFavorites = AdsDAL.IsInFavorites(SM.CurrentUserId, ad.Id);
            #region Заполнение полей предложений
            ad.Offers = OffersDAL.GetOffers(ad.Id).Where(o => o.OfferStatus != OfferStatuses.Draft && o.OfferStatus != OfferStatuses.Deleted).ToList();
            if (ad.Offers.Any())
            {
                ad.Offers = ad.Offers.OrderByDescending(o => (o.Modified ?? o.DateOfPosting)).ToList();
                var dollarRate = TickersHelper.GetTodayUsdQuote();
                foreach (var offer in ad.Offers)
                {
                    offer.Sender = UsersDAL.GetUser(offer.SenderId);
                    if (offer.Sender != null)
                        offer.Sender.Town = TownsDAL.GetTown(offer.Sender.CityId);
                    if (offer.City == null && ad.City != null)
                        offer.City = ad.City;
                    #region Рассчет суммы предложения
                    offer.ProductOffers = ProductOffersDAL.GetProductOffers(offer.Id);
                    if (offer.ProductOffers.Any())
                    {
                        offer.SumProduct = new List<float>();
                        var allProducts = AdProductsDAL.GetAdProducts();
                        foreach (var productOffer in offer.ProductOffers)
                        {
                            var product = allProducts.Find(p => p.Id == productOffer.ProductId);
                            if (product != null)
                            {
                                var sumProduct = product.Weight * productOffer.PricePerWeight * (product.Currency == Currencies.Dollar ? dollarRate : 1);
                                offer.SumProduct.Add(sumProduct);
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion
            ad.Photos = PhotosDAL.GetPhotos(ad.Id);
            if (ad.Photos.Any())
            {
                ad.MainPhotoGroup = ad.Photos.Where(photo => photo.IsMain == true).ToList();
                if (!ad.MainPhotoGroup.Any())
                    ad.MainPhotoGroup = ad.Photos.GroupBy(p => p.GroupId.ToString()).FirstOrDefault().Select(p => p).ToList();
                ad.MainPhotoGroup.ForEach(p => ad.Photos.Remove(p));
                ad.Photos.InsertRange(0, ad.MainPhotoGroup);
            }
            #endregion
            #region Заполнение полей пользователя
			
            //ad.PersonalAreaUser = SM.GetPersonalAreaUser();
			var currentUserId = SM.CurrentUserId;
			//ad.PersonalAreaUser.FavoriteAds = AdsDAL.GetFavoriteAds(currentUserId);
			#endregion
			#region Заполнение полей для приглашения участников
			ad.InvitedUsersId = AdsDAL.GetInvitedUsers(ad.Id);
            ad.RegularClients = UsersDAL.GetRegularClients(ad.SenderId);
            ad.AllUsers = UsersDAL.GetUsers();
            if (!ad.IsMy)
                AdsDAL.UpdateAdAddOneView(ad.Id);
			#endregion

			//заполнение модели для отображения блока "Похожие объявления"
			var recentlyViewedAds = AdHelper.GetRecentlyViewedAds(currentUserId);//TODO переделать, когда будет алгоритм получения похожих объявлений
			ad.AdsSliderViewModel = new AdsSliderViewModel("Похожие объявления", currentUserId, recentlyViewedAds);

			ViewBag.HeadingMainPart = ad.Name.Substring(0, ad.Name.IndexOf(" ("));
            ViewBag.HeadingExtraPart = ad.Name.Substring(ad.Name.IndexOf(" ("));
            #region Хлебные крошки
            /*var breadCrumbs = new List<BreadCrumbLink>();
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Объявления", Url = Urls.Ads, Title = "Перейти к списку всех объявлений" });
            if (ad.SenderId == ad.PersonalAreaUser?.Id)
            {
                breadCrumbs.Add(new BreadCrumbLink() { Text = "Мои объявления", Url = C.SiteUrl + "User/MyAds", Title = "Перейти к списку своих объявлений" });
            }
            breadCrumbs.Add(new BreadCrumbLink() { Text = ad.Name, EndPoint = true });
            ViewBag.BreadCrumbs = breadCrumbs;*/
            //var breadCrumbs = new List<BreadCrumbLink>();
            //if (ad.SenderId == ad.PersonalAreaUser?.Id)
            //{
            //    breadCrumbs.Add(new BreadCrumbLink() { Text = "Мои объявления", Url = C.SiteUrl + "User/MyAds", Title = "Перейти к моим объявлениям" });
            //}
            //if (ad.IsBuy == true)
            //{
            //    breadCrumbs.Add(new BreadCrumbLink() { 
            //        Text = "Закупки", 
            //        Url = C.SiteUrl + "Ads/Index?isBuy=true" + (ad.SenderId == ad.PersonalAreaUser?.Id ? "&my=true" : ""), 
            //        Title = "Перейти к списку закупок" });
            //}
            //else if (ad.IsBuy == false)
            //{
            //    breadCrumbs.Add(new BreadCrumbLink() { 
            //        Text = "Продажи", 
            //        Url = C.SiteUrl + "Ads/Index?isBuy=false" + (ad.SenderId == ad.PersonalAreaUser?.Id ? "&my=true" : ""), 
            //        Title = "Перейти к списку продаж" });
            //}
            //var breadCrumbsAdName = ad.Name.Substring(0, ad.Name.IndexOf('('));
            //breadCrumbs.Add(new BreadCrumbLink() { Text = breadCrumbsAdName, EndPoint = true });
            //ViewBag.BreadCrumbs = breadCrumbs;
			#endregion

			#region если смотрим чужое объявление, добавляем 1 просмотр
			if (ad.SenderId != currentUserId)
			{
				var adView = new AdView()
				{
					UserId = currentUserId,
					AdId = ad.Id,
					Created = DateTime.Now.ToUniversalTime()
				};
				AdViewsDAL.AddAdView(adView);
			}
			#endregion

			return View(ad);
        }

		public ActionResult ShowOld(int id, int? offerId)
		{
			return Show(id, offerId);
		}


		public ActionResult EditAd(int id = 0)
        {
            if (id == 0)
                return RedirectToAction("Index");
            var ad = AdsDAL.GetAd(id);
            if (ad == null)
                return Redirect(C.SiteUrl + "Ads/Show/" + id);
            var viewModel = new AdsEditAdViewModel();
            viewModel.PersonalAreaUser = SM.GetPersonalAreaUser();
            viewModel.ProductCategories = ProductCategoriesDAL.GetCategories();
            ad.City = TownsDAL.GetTown(ad.CityId);
            ad.Sender = UsersDAL.GetUser(ad.SenderId);
            if (ad.Sender != null)
                ad.Sender.Town = TownsDAL.GetTown(ad.Sender.CityId);
            ad.Offers = OffersDAL.GetOffers(ad.Id);
            ad.Products = AdProductsDAL.GetAdProducts(ad.Id);
            if (ad.Products.Any())
            {
                foreach (var product in ad.Products)
                {
                    product.ProductCategoryName = ProductCategoriesDAL.GetProductCategory(product.ProductCategoryId).Name;
                }
            }
            ad.Photos = PhotosDAL.GetPhotos(ad.Id);
            if (ad.Photos.Any())
            {
                ad.MainPhotoGroup = ad.Photos.Where(photo => photo.IsMain == true).ToList();
                if (!ad.MainPhotoGroup.Any())
                    ad.MainPhotoGroup = ad.Photos.GroupBy(p => p.GroupId.ToString()).FirstOrDefault().Select(p => p).ToList();
                ad.MainPhotoGroup.ForEach(p => ad.Photos.Remove(p));
                ad.Photos.InsertRange(0, ad.MainPhotoGroup);
            }
            ad.InvitedUsersId = AdsDAL.GetInvitedUsers(ad.Id);
            ad.RegularClients = UsersDAL.GetRegularClients(ad.SenderId);
            ad.AllUsers = UsersDAL.GetUsers();
            ViewBag.Heading = ad.Name;
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>();
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Личный кабинет", Url = Urls.PersonalArea, Title = "Перейти в личный кабинет" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Объявления", Url = Urls.Ads, Title = "Перейти к списку всех объявлений" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = ad.Name, Url = ad.Url, Title = "Перейти к объявлению" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Редактирование", EndPoint = true });
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            viewModel.Ad = ad;
            return View(viewModel);
        }

		[MyAuthorize]
        public ActionResult NewOffer(int id)
        {
			

            var existingOffers = OffersDAL.GetOffers(id);
            if (existingOffers.Any())
            {
                var existingOffer = existingOffers.Find(o => o.SenderId == SM.CurrentUserId);
                if (existingOffer != null)
                    return Redirect(existingOffer.Url);
            }
            var ad = AdsDAL.GetAd(id);
            if (ad == null)
                return RedirectToAction("Index");
            ad.Sender = UsersDAL.GetUser(ad.SenderId);
            if (ad.Sender != null)
                ad.Sender.Town = TownsDAL.GetTown(ad.Sender.CityId);
            ad.Products = AdProductsDAL.GetAdProducts(ad.Id);
            if (ad.Products.Any())
            {
                var productCategories = ProductCategoriesDAL.GetCategories();
                foreach (var product in ad.Products)
                {
                    product.ProductCategoryName = productCategories.Find(c => product.ProductCategoryId == c.Id).Name;
                }
            }
            ad.City = TownsDAL.GetTown(ad.CityId);
            ad.Photos = PhotosDAL.GetPhotos(ad.Id);
            if (ad.Photos.Any())
            {
                ad.MainPhotoGroup = ad.Photos.Where(photo => photo.IsMain == true).ToList();
                if (!ad.MainPhotoGroup.Any())
                    ad.MainPhotoGroup = ad.Photos.GroupBy(p => p.GroupId.ToString()).FirstOrDefault().Select(p => p).ToList();
                ad.MainPhotoGroup.ForEach(p => ad.Photos.Remove(p));
                ad.Photos.InsertRange(0, ad.MainPhotoGroup);
            }
			var me = SM.CurrentUser;
			//var me = SM.GetPersonalAreaUser();
			//ad.PersonalAreaUser = me;
			//ad.PersonalAreaUser.Town = TownsDAL.GetTown(ad.PersonalAreaUser.CityId);
            ViewBag.Heading = "Новое предложение";
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>();
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Объявления", Url = Urls.Ads, Title = "Перейти к списку всех объявлений" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = ad.Name, Url = ad.Url, Title = "Перейти к объявлению" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true });
            ViewBag.BreadCrumbs = breadCrumbs;
			#endregion

			if (!ad.IsBuy && me != null)
			{
				if (!String.IsNullOrWhiteSpace(me.FactualAddress))
					ad.DeliveryAddress = me.FactualAddress;
				else if (!String.IsNullOrWhiteSpace(me.Address))
					ad.DeliveryAddress = me.Address;
				else if (me.TownName != null)
					ad.DeliveryAddress = "г. " + me.TownName + ", ";
			}

			var viewModel = new AdsNewOfferViewModel()
			{
				Ad = ad,
				CurrentUser = SM.CurrentUser
			};

			return View(viewModel);
        }

        public ActionResult Offer(int id, bool isFromAd = false)
        {
            Offer offer = OffersDAL.GetOffer(id);
            if (offer == null)
                return RedirectToAction("Index");
            offer.IsFromAd = isFromAd;
            offer.Sender = UsersDAL.GetUser(offer.SenderId);
            if (offer.Sender != null)
                offer.Sender.Town = TownsDAL.GetTown(offer.Sender.CityId);
            offer.Ad = AdsDAL.GetAd(offer.AdId);
            if (offer.Ad == null)
                return RedirectToAction("Index");
            offer.Ad.City = TownsDAL.GetTown(offer.Ad.CityId);
            offer.Ad.Sender = UsersDAL.GetUser(offer.Ad.SenderId);
            if (offer.Ad.Sender != null)
                offer.Ad.Sender.Town = TownsDAL.GetTown(offer.Ad.Sender.CityId);
            offer.Ad.Offers = OffersDAL.GetOffers(offer.Ad.Id);
            offer.Ad.Products = AdProductsDAL.GetAdProducts(offer.Ad.Id);
            if (offer.Ad.Products.Any())
            {
                foreach (var product in offer.Ad.Products)
                {
                    product.ProductCategoryName = ProductCategoriesDAL.GetProductCategory(product.ProductCategoryId).Name;
                }
            }
            offer.Ad.Photos = PhotosDAL.GetPhotos(offer.Ad.Id);
            if (offer.Ad.Photos.Any())
            {
                offer.Ad.MainPhotoGroup = offer.Ad.Photos.Where(photo => photo.IsMain == true).ToList();
                if (!offer.Ad.MainPhotoGroup.Any())
                    offer.Ad.MainPhotoGroup = offer.Ad.Photos.GroupBy(p => p.GroupId.ToString()).FirstOrDefault().Select(p => p).ToList();
                offer.Ad.MainPhotoGroup.ForEach(p => offer.Ad.Photos.Remove(p));
                offer.Ad.Photos.InsertRange(0, offer.Ad.MainPhotoGroup);
            }
            offer.PersonalAreaUser = SM.GetPersonalAreaUser();
            offer.ProductOffers = ProductOffersDAL.GetProductOffers(offer.Id);
            if (offer.ProductOffers.Any())
            {
                var dollarRate = TickersHelper.GetTodayUsdQuote();
                offer.SumProduct = new List<float>();
                var allProducts = AdProductsDAL.GetAdProducts();
                foreach (var productOffer in offer.ProductOffers)
                {
                    var product = allProducts.Find(p => p.Id == productOffer.ProductId);
                    if (product != null)
                    {
                        var sumProduct = product.Weight * productOffer.PricePerWeight * (product.Currency == Currencies.Dollar ? dollarRate : 1);
                        offer.SumProduct.Add(sumProduct);
                    }
                }
            }

            ViewBag.Heading = offer.Name + " от " + offer.Sender?.CompanyNameWithTypeOfOwnership + (offer.Sender?.Town != null ? ", " + offer.Sender.TownName : "");
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>();
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Личный кабинет", Url = Urls.PersonalArea, Title = "Перейти в личный кабинет" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Объявления", Url = Urls.Ads, Title = "Перейти к списку всех объявлений" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = offer.Ad.Name, Url = offer.Ad.Url, Title = "Перейти к объявлению" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = offer.Name, EndPoint = true });
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(offer);
        }

        public ActionResult EditOffer(int id)
        {
            Offer offer = OffersDAL.GetOffer(id);
            if (offer == null)
                return Redirect(C.SiteUrl + "Offer/" + id);
            offer.Ad = AdsDAL.GetAd(offer.AdId);
            if (offer.Ad == null)
                return Redirect(C.SiteUrl + "Offer/" + id);
            offer.Ad.City = TownsDAL.GetTown(offer.Ad.CityId);
            offer.Ad.Sender = UsersDAL.GetUser(offer.Ad.SenderId);
            if (offer.Ad.Sender != null)
                offer.Ad.Sender.Town = TownsDAL.GetTown(offer.Ad.Sender.CityId);
            offer.Ad.Offers = OffersDAL.GetOffers(offer.Ad.Id);
            offer.Ad.Products = AdProductsDAL.GetAdProducts(offer.Ad.Id);
            if (offer.Ad.Products.Any())
            {
                foreach (var product in offer.Ad.Products)
                {
                    product.ProductCategoryName = ProductCategoriesDAL.GetProductCategory(product.ProductCategoryId).Name;
                }
            }
            offer.Ad.City = TownsDAL.GetTown(offer.Ad.CityId);
            offer.Ad.Photos = PhotosDAL.GetPhotos(offer.Ad.Id);
            if (offer.Ad.Photos.Any())
            {
                offer.Ad.MainPhotoGroup = offer.Ad.Photos.Where(photo => photo.IsMain == true).ToList();
                if (!offer.Ad.MainPhotoGroup.Any())
                    offer.Ad.MainPhotoGroup = offer.Ad.Photos.GroupBy(p => p.GroupId.ToString()).FirstOrDefault().Select(p => p).ToList();
                offer.Ad.MainPhotoGroup.ForEach(p => offer.Ad.Photos.Remove(p));
                offer.Ad.Photos.InsertRange(0, offer.Ad.MainPhotoGroup);
            }
            offer.PersonalAreaUser = SM.GetPersonalAreaUser();
            offer.ProductOffers = ProductOffersDAL.GetProductOffers(offer.Id);
            if (offer.ProductOffers.Any())
            {
                var dollarRate = TickersHelper.GetTodayUsdQuote();
                offer.SumProduct = new List<float>();
                var allProducts = AdProductsDAL.GetAdProducts();
                foreach (var productOffer in offer.ProductOffers)
                {
                    var product = allProducts.Find(p => p.Id == productOffer.ProductId);
                    if (product != null)
                    {
                        var sumProduct = product.Weight * productOffer.PricePerWeight * (product.Currency == Currencies.Dollar ? dollarRate : 1);
                        offer.SumProduct.Add(sumProduct);
                    }
                }
            }
            ViewBag.Heading = offer.Name;
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>();
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Личный кабинет", Url = Urls.PersonalArea, Title = "Перейти в личный кабинет" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Объявления", Url = Urls.Ads, Title = "Перейти к списку всех объявлений" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = offer.Ad.Name, Url = offer.Ad.Url, Title = "Перейти к объявлению" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = offer.Name, Url = offer.Url, Title = "Перейти к предложению" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Редактирование", EndPoint = true });
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(offer);
        }
		
    //    public ActionResult Offers()
    //    {
    //        var viewModel = new AdsOffersViewModel();
    //        viewModel.PersonalAreaUser = SM.GetPersonalAreaUser();
    //        viewModel.OutgoingOffers = OffersDAL.GetOffersFromUser(viewModel.PersonalAreaUser.Id).OrderByDescending(o => (o.Modified ?? o.DateOfPosting)).ToList();
    //        if (viewModel.OutgoingOffers.Any())
    //        {
				//var allProductCategories = ProductCategoriesDAL.GetCategories();
				//var allProducts = AdProductsDAL.GetAdProducts();
    //            //viewModel.OutgoingOffers = viewModel.OutgoingOffers.Where(o => o.OfferStatus != OfferStatuses.Expired).ToList();
    //            var dollarRate = TickersHelper.GetTodayUsdQuote();
				//var adIds = viewModel.OutgoingOffers.Select(o => o.AdId).Distinct().ToList();
				////var ads = AdHelper.GetAdsForView(currentUserId, ads: viewModel.Ads, needInvitedUserIds: true, needSender: true);
				////var photos =
				//foreach (var offer in viewModel.OutgoingOffers)
    //            {
    //                offer.Sender = UsersDAL.GetUser(offer.SenderId);
    //                offer.Ad = AdsDAL.GetAd(offer.AdId);
    //                if (offer.Ad != null)
    //                {
    //                    offer.Ad.Sender = UsersDAL.GetUser(offer.Ad.SenderId);
    //                    if (offer.Ad.Sender != null)
    //                        offer.Ad.Sender.Town = TownsDAL.GetTown(offer.Ad.Sender.CityId);
    //                }
    //                if (offer.CityId != null)
    //                    offer.City = TownsDAL.GetTown(offer.CityId.Value);
    //                offer.SumProduct = new List<float>();
    //                offer.ProductOffers = ProductOffersDAL.GetProductOffers(offer.Id);
    //                if (offer.ProductOffers.Any())
    //                {
    //                    offer.ProductOffersDescription = "";
    //                    var i = 0;
    //                    var numOfPreviewedProducts = 3;
    //                    foreach (var productOffer in offer.ProductOffers)
    //                    {
    //                        var product = allProducts.Find(p => p.Id == productOffer.ProductId);
    //                        if (product != null)
    //                        {
    //                            var sumProduct = product.Weight * productOffer.PricePerWeight * (product.Currency == Currencies.Dollar ? dollarRate : 1);
    //                            offer.SumProduct.Add(sumProduct);
    //                            if (i < numOfPreviewedProducts)
    //                            {
    //                                product.ProductCategoryName = allProductCategories.Find(c => c.Id == product.ProductCategoryId)?.Name;
    //                                if (product.Name != null)
    //                                    product.Name = product.Name.Substring(0, 1).ToLower() + product.Name.Substring(1);
                                    
    //                                if (i > 0)
    //                                {
    //                                    offer.ProductOffersDescription += ", ";
    //                                    product.ProductCategoryName = product.ProductCategoryName.Substring(0, 1).ToLower() + product.ProductCategoryName.Substring(1);
    //                                }
    //                                if (product.Name != null)
    //                                    offer.ProductOffersDescription += product.ProductCategoryName + " (" + product.Name + ") – " + offer.SumProduct[i] + " руб.";
    //                                else
    //                                    offer.ProductOffersDescription += product.ProductCategoryName + " – " + offer.SumProduct[i] + " руб.";
    //                            }
    //                            else if (i == numOfPreviewedProducts)
    //                                offer.ProductOffersDescription += ", ...";
    //                            i++;
    //                        }
    //                    }
    //                }
    //            }
    //            viewModel.OutgoingOffers = viewModel.OutgoingOffers
    //                .Where(o => 
    //                o.Ad != null 
    //                && o.Ad.AdStatus != AdStatuses.Finished
    //                && (o.Ad.AdStatus != AdStatuses.Expired || (DateTime.Now.ToUniversalTime() - o.Ad.ActiveToDate).Value.Days < 60))
    //                .ToList();
    //        }
    //        ViewBag.Heading = "Мои отклики";
    //        #region Хлебные крошки
    //        var breadCrumbs = new List<BreadCrumbLink>();
    //        breadCrumbs.Add(new BreadCrumbLink() { Text = "Личный кабинет", Url = Urls.PersonalArea, Title = "Перейти в личный кабинет" });
    //        breadCrumbs.Add(new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true });
    //        ViewBag.BreadCrumbs = breadCrumbs;
    //        #endregion
    //        return View(viewModel);
    //    }

        [MyAuthorize]
        public ActionResult NewAdStep0()
        {
            var ad = new AdsNewAdStep0ViewModel();
            ad.PersonalAreaUser = SM.GetPersonalAreaUser();
            ViewBag.Heading = "Новое объявление";
            var breadCrumbs = new List<BreadCrumbLink>();
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Личный кабинет", Url = Urls.PersonalArea, Title = "Перейти в личный кабинет" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Новое объявление", Url = C.SiteUrl + "Ads/NewAdStep0", Title = "Перейти к созданию нового объявления" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Выбор процедуры", EndPoint = true });
            ViewBag.BreadCrumbs = breadCrumbs;
            return View(ad);
        }

        [MyAuthorize]
		public ActionResult NewAd(bool isBuy = false, bool availableForAllUsers = true)
        {
			ViewBag.L.HideHead = true;

			var viewModel = new AdsNewAdViewModel();
			viewModel.ProductCategories = ProductCategoriesDAL.GetCategories();
            viewModel.IsBuy = isBuy;
            viewModel.AvailableForAllUsers = availableForAllUsers;
            viewModel.Ad = new Ad
            {
                IsBuy = viewModel.IsBuy,
                AvailableForAllUsers = viewModel.AvailableForAllUsers
            };
            viewModel.PersonalAreaUser = SM.GetPersonalAreaUser();
            ViewBag.Heading = viewModel.Ad.Name;
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>();
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Личный кабинет", Url = Urls.PersonalArea, Title = "Перейти в личный кабинет" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Новое объявление", Url = C.SiteUrl + "Ads/NewAdStep0", Title = "Перейти к созданию нового объявления" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Добавление нового объявления", EndPoint = true });
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(viewModel);
        }

		[MyAuthorize]
		public ActionResult NewAdOld(bool isBuy = false, bool availableForAllUsers = true)
		{
			return NewAd(isBuy, availableForAllUsers);
		}

		[MyAuthorize]
        public ActionResult NewAdFinalStep(int id)
        {
            var viewModel = AdsDAL.GetAd(id);
            if (viewModel == null)
                return RedirectToAction("NewAdStep0");
            viewModel.Sender = UsersDAL.GetUser(viewModel.SenderId);
            if (viewModel.Sender != null)
                viewModel.Sender.Town = TownsDAL.GetTown(viewModel.Sender.CityId);
            viewModel.City = TownsDAL.GetTown(viewModel.CityId);
            viewModel.Products = AdProductsDAL.GetAdProducts(id);
            if (viewModel.Products.Any())
            {
                var productCategories = ProductCategoriesDAL.GetCategories();
                foreach (var product in viewModel.Products)
                {
                    product.ProductCategoryName = productCategories.Find(c => product.ProductCategoryId == c.Id).Name;
                }
            }
            else
                return Redirect(C.SiteUrl + "EditAd/" + id);
            viewModel.Photos = PhotosDAL.GetPhotos(viewModel.Id);
            if (viewModel.Photos.Any())
            {
                viewModel.MainPhotoGroup = viewModel.Photos.Where(photo => photo.IsMain == true).ToList();
                if (!viewModel.MainPhotoGroup.Any())
                    viewModel.MainPhotoGroup = viewModel.Photos.GroupBy(p => p.GroupId.ToString()).FirstOrDefault().Select(p => p).ToList();
                viewModel.MainPhotoGroup.ForEach(p => viewModel.Photos.Remove(p));
                viewModel.Photos.InsertRange(0, viewModel.MainPhotoGroup);
            }
            //viewModel.PersonalAreaUser = SM.GetPersonalAreaUser();
            ViewBag.Heading = viewModel.Name;
            #region Заполнение полей для приглашения участников
            viewModel.InvitedUsersId = AdsDAL.GetInvitedUsers(viewModel.Id);
            viewModel.RegularClients = UsersDAL.GetRegularClients(viewModel.SenderId);
            viewModel.AllUsers = UsersDAL.GetUsers();
            var towns = TownsDAL.GetTowns();
            if (viewModel.AllUsers.Any())
            {
                foreach (var user in viewModel.AllUsers)
                {
                    user.Town = towns.Find(t => user.CityId == t.Id);
                }
            }
            #endregion
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>();
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Личный кабинет", Url = Urls.PersonalArea, Title = "Перейти в личный кабинет" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Новое объявление", Url = C.SiteUrl + "Ads/NewAdStep0", Title = "Перейти к созданию нового объявления" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Финальный этап", EndPoint = true });
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            
            return View(viewModel);
        }

        [HttpPost]
        public string NewAdFinalStep(Ad ad)
        {
            var adFromDb = AdsDAL.GetAd(ad.Id);
            adFromDb.Products = AdProductsDAL.GetAdProducts(ad.Id);
            if (AdminController.DoesNeedModeration(adFromDb) || (adFromDb.Products != null && adFromDb.Products.Any(p => AdminController.DoesNeedModeration(p))))
            {
                adFromDb.ModerateResult = ModerateResults.NotChecked;
                var notificationsUserId = Models.User.SystemNotificationsUserId;
                var messageText = "Ваше объявление " + adFromDb.Name.ToLower() +
                                  " не прошло автоматическую модерацию и было отправлено на ручную проверку. " +
                                  "Объявление будет опубликовано сразу после проверки модератором. " +
                                  "Вы получите оповещение о результате проверки.";
                var message = new Message
                {
                    SenderId = notificationsUserId,
                    RecipientId = adFromDb.SenderId,
                    Text = messageText,
                    AdId = adFromDb.Id
                };
                UserHelper.AddMessage(message);
            }
            else
            {
                adFromDb.ModerateResult = ModerateResults.Accepted;
            }
            adFromDb.DateOfPosting = DateTime.Now.ToUniversalTime();
            adFromDb.ActiveToDate = ad.ActiveToDate.Value.AddDays(1).AddSeconds(-1).ToUniversalTime();
            adFromDb.OffersVisibleToOtherUsers = ad.OffersVisibleToOtherUsers;
            adFromDb.NotifyRegularClients = ad.NotifyRegularClients;
            adFromDb.ViewsCount = 0;
            adFromDb.AdStatus = AdStatuses.Published;
			adFromDb.Description = ad.Description;
            var id = AdsDAL.UpdateAd(adFromDb);
            var invitedUsersToAdd = new List<int>();
            if (!adFromDb.AvailableForAllUsers)
            {
                if (ad.InvitedUsersId.Any())
                {
                    foreach (var userId in ad.InvitedUsersId)
                    {
                        invitedUsersToAdd.Add(userId);
                    }
                }
            }
            AdsDAL.AddInvitedUsers(id, invitedUsersToAdd);
            return C.SiteUrl + "Ads/Show/" + adFromDb.Id;
            //throw new Exception("Не удалось опубликовать объявление, пожалуйста, попробуйте позже");
        }

        [HttpPost]
        public string NewAd(Ad ad)
        {
            ad.Sender = UsersDAL.GetUser(ad.SenderId);
			ad.DateOfPosting = DateTime.Now.ToUniversalTime();
            var id = AdsDAL.AddAd(ad);
            if (id > 0)
            {
                if (ad.Products.Any())
                {
                    ad.Products.ForEach(p => p.AdId = id);
                    if (AdProductsDAL.AddAdProducts(ad.Products))
                        return C.SiteUrl + "Ads/NewAdFinalStep/" + id;
                }
            }
            throw new Exception("Не удалось сохранить объявление, пожалуйста, попробуйте позже");
        }

        [HttpPost]
        public string EditAd(Ad updatedAd)
        {
            var adFromDb = AdsDAL.GetAd(updatedAd.Id);
            adFromDb.Products = AdProductsDAL.GetAdProducts(updatedAd.Id);
            var adProductsToDelete = new List<int>();
            var adProductsToAdd = new List<AdProduct>();
            var adProductsToUpdate = new List<AdProduct>();
            if (adFromDb.Products.Any())
            {
                foreach (var product in adFromDb.Products)
                {
                    if (!updatedAd.Products.Any(p => p.Id == product.Id))
                    {
                        adProductsToDelete.Add(product.Id);
                    }
                }
            }
            if (updatedAd.Products.Any())
            {
                foreach (var product in updatedAd.Products)
                {
                    if (!adFromDb.Products.Any(p => p.Id == product.Id))
                    {
                        product.AdId = updatedAd.Id;
                        adProductsToAdd.Add(product);
                    }
                    else
                    {
                        adProductsToUpdate.Add(product);
                    }
                }
            }
            adFromDb.Products = updatedAd.Products;
            AdProductsDAL.DeleteAdProducts(adProductsToDelete);
            AdProductsDAL.AddAdProducts(adProductsToAdd);
            AdProductsDAL.UpdateAdProducts(adProductsToUpdate);
            if (adFromDb.Description != updatedAd.Description
                || adFromDb.CityId != updatedAd.CityId
                || adFromDb.DeliveryType != updatedAd.DeliveryType
                || (adFromDb.DeliveryAddress != null && adFromDb.DeliveryAddress.Trim(' ') != updatedAd.DeliveryAddress)
                || adFromDb.DeliveryLoadType != updatedAd.DeliveryLoadType
                || adFromDb.DeliveryWay != updatedAd.DeliveryWay
                || adFromDb.TermsOfPayments != updatedAd.TermsOfPayments
                || adFromDb.DefermentPeriod != updatedAd.DefermentPeriod
                || adFromDb.Nds != updatedAd.Nds
                || adFromDb.PartOffersAllowed != updatedAd.PartOffersAllowed
                || adFromDb.OffersVisibleToOtherUsers != updatedAd.OffersVisibleToOtherUsers
                || adFromDb.IsBuy != updatedAd.IsBuy
                || adFromDb.AvailableForAllUsers != updatedAd.AvailableForAllUsers
                || adProductsToDelete.Count != 0
                || adProductsToAdd.Count != 0)
            {
                var offers = OffersDAL.GetOffers(adFromDb.Id);
                if (offers.Any())
                {
                    var adSender = UsersDAL.GetUser(adFromDb.SenderId);
                    if (adSender != null)
                    {
                        adSender.Town = TownsDAL.GetTown(adSender.CityId);
                        var notificationsUserId = MContract.Models.User.SystemNotificationsUserId;
                        foreach (var offer in offers)
                        {
                            var messageText = "Организатор " + adSender.CompanyNameWithTypeOfOwnership + (adSender.Town != null ? ", " + adSender.TownName : "") +
                                              " изменил условия объявления " + adFromDb.Name + ". Ваше предложение было аннулировано.";
                            var message = new Message
                            {
                                SenderId = notificationsUserId,
                                RecipientId = offer.SenderId,
                                Text = messageText,
                                AdId = adFromDb.Id
                            };
                            MContract.AppCode.UserHelper.AddMessage(message);
                            OffersDAL.DeleteOffer(offer.Id);
                        }
                    }
                }
            }
            adFromDb.Description = updatedAd.Description;
            adFromDb.CityId = updatedAd.CityId;
            adFromDb.DeliveryType = updatedAd.DeliveryType;
            adFromDb.DeliveryAddress = updatedAd.DeliveryAddress;
            adFromDb.DeliveryLoadType = updatedAd.DeliveryLoadType;
            adFromDb.DeliveryWay = updatedAd.DeliveryWay;
            adFromDb.TermsOfPayments = updatedAd.TermsOfPayments;
            adFromDb.DefermentPeriod = updatedAd.DefermentPeriod;
            adFromDb.Nds = updatedAd.Nds;
            adFromDb.PartOffersAllowed = updatedAd.PartOffersAllowed;
            adFromDb.OffersVisibleToOtherUsers = updatedAd.OffersVisibleToOtherUsers;
            adFromDb.IsBuy = updatedAd.IsBuy;
            adFromDb.AvailableForAllUsers = updatedAd.AvailableForAllUsers;
            adFromDb.AdStatus = AdStatuses.Published;

			if (updatedAd.ActiveToDate == null)
				updatedAd.ActiveToDate = DateTime.Now.AddDays(7);

			var newActiveToDate = updatedAd.ActiveToDate.Value.AddDays(1).AddSeconds(-1).ToUniversalTime();
			if (newActiveToDate > adFromDb.ActiveToDate && newActiveToDate > DateTime.Now.ToUniversalTime() && adFromDb.IsSendedExpiredMessage)
				AdsDAL.UpdateAdSetIsSendedExpiredMessage(adFromDb.Id, false);

			adFromDb.ActiveToDate = newActiveToDate;

			if (!adFromDb.AvailableForAllUsers)
            {
                //var invitedUsersToDelete = new List<int>();
                var invitedUsersToAdd = new List<int>();
                adFromDb.InvitedUsersId = AdsDAL.GetInvitedUsers(updatedAd.Id);
                /*foreach (var userId in ad.InvitedUsersId)
                {
                    if (!updatedAd.InvitedUsersId.Contains(userId))
                    {
                        invitedUsersToDelete.Add(userId);
                    }
                }*/
                if (updatedAd.InvitedUsersId == null)
                    updatedAd.InvitedUsersId = new List<int>();
                if (updatedAd.InvitedUsersId.Any())
                {
                    foreach (var userId in updatedAd.InvitedUsersId)
                    {
                        if (!adFromDb.InvitedUsersId.Any(id => id == userId))
                        {
                            invitedUsersToAdd.Add(userId);
                        }
                    }
                }
                //AdsDAL.DeleteInvitedUsers(updatedAd.Id, invitedUsersToDelete);
                AdsDAL.AddInvitedUsers(updatedAd.Id, invitedUsersToAdd);
            }
            if (AdminController.DoesNeedModeration(adFromDb) || adFromDb.Products.Any(p => AdminController.DoesNeedModeration(p)))
            {
                adFromDb.ModerateResult = ModerateResults.NotChecked;
                var notificationsUserId = MContract.Models.User.SystemNotificationsUserId;
                var messageText = "Ваше объявление \"" + adFromDb.Name +
                                  "\" не прошло автоматическую модерацию и было отправлено на ручную проверку. " +
                                  "Объявление будет опубликовано сразу после проверки модератором. " +
                                  "Вы получите оповещение о результате проверки.";
                var message = new Message
                {
                    SenderId = notificationsUserId,
                    RecipientId = adFromDb.SenderId,
                    Text = messageText,
                    AdId = adFromDb.Id
                };
                MContract.AppCode.UserHelper.AddMessage(message);
            }
            else
            {
                adFromDb.ModerateResult = ModerateResults.Accepted;
            }
            AdsDAL.UpdateAd(adFromDb);
            return C.SiteUrl + "Ads/" + (adFromDb.AdStatus == AdStatuses.Draft ? "NewAdFinalStep" : "Show") + "/" + adFromDb.Id;
        }

		[HttpPost]
		public int RepeatAd(int id)
		{
			var ad = AdsDAL.GetAd(id);
			if (ad == null || ad.SenderId != SM.CurrentUserId)
				return 0;

			var newAd = new Ad()
			{
				ActivePeriod = 0,
				ActiveToDate = null,
				DateOfPosting = DateTime.Now.ToUniversalTime(),
				AdStatus = AdStatuses.Draft
			};
            
            var newId = AdsDAL.AddAd(ad);
            var newAdProducts = AdProductsDAL.GetAdProducts(id);
            if (newAdProducts.Any())
            {
                foreach (var product in newAdProducts)
                {
                    product.AdId = newId;
                }
                AdProductsDAL.AddAdProducts(newAdProducts);
            }
            var photos = PhotosDAL.GetPhotos(id);
            var newPhotos = PhotosController.CopyAdPhotos(photos, newId);
            return newId;
        }

        [HttpPost]
        public bool DeleteAd(int id)
        {
            if (id == 0)
                return false;
            var ad = AdsDAL.GetAd(id);
            if (ad == null || ad.SenderId != SM.CurrentUserId)
                return false;

            return AdsDAL.DeleteAd(id);
        }

        [HttpPost]
        public string DeleteAds(List<int> ids)
        {
            if (ids == null || !ids.Any())
                return "Передан пустой список";

            var ads = AdsDAL.GetAds(ids);
            if (!ads.Any())
                return "В БД не найдено объявлений по переданным Id";

            var curUserId = SM.CurrentUserId;
            var adsAllowedToDelete = ads.All(a => a.SenderId == curUserId);
            if (adsAllowedToDelete)
                AdsDAL.DeleteAds(ids);
            else
                return "Попытка удалить чужое объявление";

            return "ok";
        }

		public string RequestAdInvitation(int adId, int userId)
		{
			var ad = AdsDAL.GetAd(adId);

			if (ad == null || ad.AdStatus == AdStatuses.Expired || ad.AdStatus == AdStatuses.Finished || ad.AdStatus == AdStatuses.Deleted)
                return "Объявление не актуально. Перезагрузите страницу.";

            var user = UsersDAL.GetUser(userId);
            user.Town = TownsDAL.GetTown(user.CityId);
            var message = new Message
            {
                SenderId = MContract.Models.User.SystemNotificationsUserId,
                RecipientId = ad.SenderId,
                AdId = adId,
				RequestJoinAdFromUserId = userId,
                Text = "Пользователь " + user.CompanyNameWithTypeOfOwnership + (user.Town != null ? ", " + user.TownName : "") + " отправил заявку на участие в объявлении " + ad.Name + "."
            };
            UserHelper.AddMessage(message);
            return "Заявка отправлена организатору. Вы получите уведомление о результате.";
        }

        public bool ReviewAdInvitationRequest(int adId, int userId, bool isAccept)
        {
            var ad = AdsDAL.GetAd(adId);
            var user = UsersDAL.GetUser(userId);
            var systemNotificationsUserId = MContract.Models.User.SystemNotificationsUserId;
            var message = new Message
            {
                SenderId = systemNotificationsUserId,
                RecipientId = userId,
                AdId = adId
            };
            if (isAccept)
            {
                message.Text = "Ваша заявка на участие в " + ad.Name + " была принята.";
                UserHelper.AddMessage(message);
                AdsDAL.AddInvitedUser(adId, userId);
            }
            else
            {
                message.Text = "Ваша заявка на участие в " + ad.Name + " была отклонена.";
                UserHelper.AddMessage(message);
            }

			MessagesDAL.UpdateMessageSetRequestJoinAdFromUserId0(adId, userId);

            return true;
        }

        /// <summary>
        /// Добавляет предложение и выполняет все необходимые действия в связи с новым предложением
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        private int AddOffer(Offer offer, bool sendMessageToAdCreator = true)
		{
            var adId = offer.AdId;
            var ad = AdsDAL.GetAd(adId);

            if (ad == null)
                return 0;

			offer.ShowInDealsHistory = true;
			var id = OffersDAL.AddOffer(offer);

			if (sendMessageToAdCreator)
				SendMessageToAdCreator(offer, true);

			return id;
		}

		/// <summary>
		/// Отправляет сообщение организатору (создателю объявления)
		/// </summary>
		/// <param name="offer"></param>
		/// <param name="isOfferAdded">Предложение добавлено? Да (добавлено), Нет (обновлено)</param>
		public void SendMessageToAdCreator(Offer offer, bool isOfferAdded)
		{
			var offerSender = UsersDAL.GetUser(offer.SenderId);
			var ad = AdsDAL.GetAd(offer.AdId);
			User adCreator = null;
			if (ad != null)
			{
				adCreator = UsersDAL.GetUser(ad.SenderId);
				if (offerSender != null && adCreator != null)
				{
					var townName = "";
					var town = TownsDAL.GetTown(offerSender.CityId);
					if (town != null)
						townName = "г. " + town.Name;

					var sendOrUpdated = isOfferAdded ? "отправил" : "обновил";
					var text = offerSender.CompanyNameWithTypeOfOwnership + ", " + townName + $" {sendOrUpdated} предложение по объявлению №" + ad.Id
						+ " " + (ad.IsBuy ? "Покупка" : "Продажа") + " " + (ad.AvailableForAllUsers ? "открытый" : "закрытый") + " запрос предложений";

					var message = new Message
					{
						SenderId = offer.SenderId,
                        RecipientId = ad.SenderId,
                        Text = text,
						AdId = ad.Id
					};
					UserHelper.AddMessage(message);
				}
			}
		}

        [HttpPost]
        public bool SaveDraftOffer(Offer offer)//??????Зачем три метода SaveOffer, NewOffer, EditOffer?
		{
            offer.OfferStatus = OfferStatuses.Draft;
            offer.DateOfPosting = DateTime.Now.ToUniversalTime();
            offer.ActiveUntilDate = offer.DateOfPosting.AddDays(1).AddSeconds(-1).ToUniversalTime();
			var id = AddOffer(offer, false);
            if (id > 0)
            {
                foreach (var productOffer in offer.ProductOffers)
                {
                    productOffer.OfferId = id;
                }
                ProductOffersDAL.AddProductOffers(offer.ProductOffers);
                return true;
            }
            else
                return false;
        }

        [HttpPost]
        public string NewOffer(Offer offer)
        {
			var error = CheckOnlineConflictsForOffer(offer);
			if (error != null)
				return error;

            offer.OfferStatus = OfferStatuses.Published;
            offer.ContractStatus = ContractStatuses.NotSent;
            offer.DateOfPosting = DateTime.Now.ToUniversalTime();
            offer.ActiveUntilDate = offer.ActiveUntilDate.AddDays(1).AddSeconds(-1).ToUniversalTime();
            if (AdminController.DoesNeedModeration(offer))
            {
                offer.ModerateResult = ModerateResults.NotChecked;
                var notificationsUserId = MContract.Models.User.SystemNotificationsUserId;
                var messageText = "Ваше предложение по объявлению №" + offer.AdId +
                                  " не прошло автоматическую модерацию и было отправлено на ручную модерацию. " +
                                  "Оно будет опубликовано сразу после проверки модератором. Вы получите оповещение о результате проверки.";
                var message = new Message
                {
                    SenderId = notificationsUserId,
                    RecipientId = offer.SenderId,
                    Text = messageText,
                    OfferId = offer.Id
                };
                MContract.AppCode.UserHelper.AddMessage(message);
            }
            var id = AddOffer(offer);
            if (offer.ProductOffers.Any())
            {
                foreach (var productOffer in offer.ProductOffers)
                {
                    productOffer.OfferId = id;
                }
            }
            ProductOffersDAL.AddProductOffers(offer.ProductOffers);
            return C.SiteUrl + "Ads/Show/" + offer.AdId;
        }

		private string CheckOnlineConflictsForOffer(Offer offer)
		{
			if (offer == null)
				return "не передано предложение (offer = null)";

			var ad = AdsDAL.GetAd(offer.AdId);
			if (ad == null)
				return "не удалось найти объявление по adId = " + offer.AdId;

			switch (ad.AdStatus)
			{
				case AdStatuses.Expired: return "Невозможно отредактировать предложение, по истекшему объявлению";
				case AdStatuses.Finished: return "Невозможно отредактировать предложение, по объявлению заключен контракт";
				case AdStatuses.Deleted: return "Невозможно отредактировать предложение, по удаленному объявлению";
			}

			var offers = OffersDAL.GetOffers(ad.Id);

			if (offers.Any(o => o.ContractStatus == ContractStatuses.Sent))
				return "Невозможно отредактировать предложение по объявлению с отправленным контрактом";

			return null;
		}

		[HttpPost]
        public string EditOffer(Offer offer)
        {
			var error = CheckOnlineConflictsForOffer(offer);
			if (error != null)
				return error;

			var updatedOffer = OffersDAL.GetOffer(offer.Id);
            updatedOffer.DeliveryType = offer.DeliveryType;
            updatedOffer.DeliveryAddress = offer.DeliveryAddress;
            updatedOffer.DeliveryLoadType = offer.DeliveryLoadType;
            updatedOffer.DeliveryWay = offer.DeliveryWay;
            updatedOffer.TermsOfPayments = offer.TermsOfPayments;
            updatedOffer.DefermentPeriod = offer.DefermentPeriod;
            updatedOffer.Nds = offer.Nds;
            updatedOffer.ActiveUntilDate = offer.ActiveUntilDate.AddDays(1).AddSeconds(-1).ToUniversalTime();
            updatedOffer.Comment = offer.Comment;
            updatedOffer.Modified = DateTime.Now.ToUniversalTime();
            updatedOffer.OfferStatus = OfferStatuses.Published;
            updatedOffer.ContractStatus = ContractStatuses.NotSent;
            OffersDAL.UpdateOffer(updatedOffer);
            updatedOffer.ProductOffers = ProductOffersDAL.GetProductOffers(offer.Id);
            var productOffersToDelete = new List<int>();
            var productOffersToAdd = new List<ProductOffer>();
            var productOffersToUpdate = new List<ProductOffer>();
            if (updatedOffer.ProductOffers.Any())
            {
                foreach (var productOffer in updatedOffer.ProductOffers)
                {
                    if (!offer.ProductOffers.Any(p => p.Id == productOffer.Id))
                    {
                        productOffersToDelete.Add(productOffer.Id);
                    }
                }
            }
            if (offer.ProductOffers.Any())
            {
                foreach (var productOffer in offer.ProductOffers)
                {
                    if (!updatedOffer.ProductOffers.Any(p => p.Id == productOffer.Id))
                    {
                        productOffer.OfferId = offer.Id;
                        productOffersToAdd.Add(productOffer);
                    }
                    else
                    {
                        productOffersToUpdate.Add(productOffer);
                    }
                }
            }
            updatedOffer.ProductOffers = offer.ProductOffers;
            ProductOffersDAL.DeleteProductOffers(productOffersToDelete);
            ProductOffersDAL.AddProductOffers(productOffersToAdd);
            ProductOffersDAL.UpdateProductOffers(productOffersToUpdate);
			SendMessageToAdCreator(offer, false);
            return C.SiteUrl + "Ads/Offer/" + updatedOffer.Id;
        }

        [HttpPost]
        public string DeleteOffer(int id, bool isFromAd = false)
        {
            var offer = OffersDAL.GetOffer(id);
            if (offer == null)
                return C.SiteUrl + "Ads/Offers";
            if (offer.SenderId != SM.CurrentUserId)
                return "Невозможно удалить предложение, отправленное другим пользователем.";
            var adId = offer.AdId;
            var ad = AdsDAL.GetAd(offer.AdId);
            if (ad != null)
            {
                var notificationsUserId = MContract.Models.User.SystemNotificationsUserId;
                var offerSender = UsersDAL.GetUser(offer.SenderId);
                if (offerSender != null)
                {
                    offerSender.Town = TownsDAL.GetTown(offerSender.CityId);
                    var messageText = "Пользователь " + offerSender.CompanyNameWithTypeOfOwnership + (offerSender.Town != null ? ", " + offerSender.TownName : "") + 
                                      " отменил свое предложение по вашему объявлению " + ad.Name + ".";
                    var message = new Message
                    {
                        SenderId = notificationsUserId,
                        RecipientId = ad.SenderId,
                        Text = messageText,
                        AdId = offer.AdId
                    };
                    MContract.AppCode.UserHelper.AddMessage(message);
                }
            }
            OffersDAL.DeleteOffer(id);
            ProductOffersDAL.DeleteProductOffers(id);
            if (isFromAd)
                return C.SiteUrl + "Ads/Show/" + adId;
            else
                return C.SiteUrl + "Ads/Offers";
        }

        [HttpPost]
        public bool CheckIfOfferHasContract(int id)
        {
            var offer = OffersDAL.GetOffer(id);
            if (offer == null)
                return false;

            var offerHasContract = offer.ContractStatus == ContractStatuses.Sent || offer.ContractStatus == ContractStatuses.Accepted;
            return offerHasContract;
        }

        [HttpPost]
        public string ProlongAd(int id, DateTime date)
        {
            var ad = AdsDAL.GetAd(id);
            if (ad == null)
                return "Объявление не найдено. Обновите страницу.";
            if (ad.SenderId != SM.CurrentUserId)
                return "Невозможно продлить объявление другого пользователя.";
            if (date == DateTime.MinValue)
                return "Ошибка при сохранении даты.";

            date = date.AddHours(23).AddMinutes(59).AddSeconds(59).ToUniversalTime();
            if (AdsDAL.UpdateAdActiveUntilDate(id, date, false))
                return $"Объявление продлено до {date.Day}.{date.Month}.{date.Year} 23:59 МСК.";
            else
                return "Произошла ошибка. Объявление не было продлено. Попробуйте позже.";
        }

        [HttpPost]
        public string ProlongOffer(int id, DateTime date)
        {
            var offer = OffersDAL.GetOffer(id);
            if (offer == null)
                return "Предложение не найдено. Обновите страницу.";
            if (offer.SenderId != SM.CurrentUserId)
                return "Невозможно продлить предложение другого пользователя.";
            if (date == DateTime.MinValue)
                return "Ошибка при сохранении даты.";

            date = date.AddHours(23).AddMinutes(59).AddSeconds(59).ToUniversalTime();
            if (OffersDAL.UpdateOfferActiveUntilDate(id, date))
                return $"Предложение продлено до {date.Day}.{date.Month}.{date.Year} 23:59 МСК.";
            else
                return "Произошла ошибка. Предложение не было продлено. Попробуйте позже.";
        }

        [HttpPost]
        public bool SendContract(int offerId)
        {
            return UserHelper.SendContract(offerId);
        }

		/// <summary>
		/// Изменяет статус контракта
		/// </summary>
		/// <param name="offerId"></param>
		/// <param name="isAccept"></param>
		/// <param name="isByAdCreator">Изменяет организатор (создатель объявления)?</param>
		/// <returns></returns>
		[HttpPost]
        public string ReviewContract(int offerId, bool isAccept, bool isByAdCreator)
        {
            var offer = OffersDAL.GetOffer(offerId);
            if (offer == null || offer.ContractStatus == ContractStatuses.Declined)
            {
                return "Невозможно " + (isAccept ? "подтвердить" : "отменить") + " контракт, потому что он уже отменен организатором.";
            }
            else if (offer.ContractStatus == ContractStatuses.Accepted)
            {
                return "Невозможно " + (isAccept ? "подтвердить" : "отменить") + " контракт, потому что он уже подтвержден.";
            }
            return UserHelper.ReviewContract(offerId, isAccept, isByAdCreator).ToString();
        }

		[MyAuthorize]
        [HttpPost]
        public bool ChangeFavoriteAd(int userId, int adId, bool isDelete = false)
        {
			if (userId != SM.CurrentUserId)
				return false;

            if (isDelete == false)
                AdsDAL.AddFavoriteAd(userId, adId);
            else
                AdsDAL.DeleteFavoriteAd(userId, adId);

			return true;
        }

        [HttpPost]
        public string UploadPhoto(int userId, int? adId = null)
        {
            return PhotoHelper.UploadPhoto(Request, userId, adId);
        }

        [HttpPost]
        public bool DeletePhoto(int photoId)
        {
            return PhotoHelper.DeletePhoto(photoId);
        }

        [HttpPost]
        public bool MakePhotoMain(int photoId)
        {
            return PhotoHelper.MakePhotoMain(photoId);
        }

		/// <summary>
		/// Снимает объявление с публикации (проставляет дату истечения = текущей дате и времени)
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public string UnpublishAd(int adId)
		{
			var ad = AdsDAL.GetAd(adId);
			if (ad == null)
				return "Не удалось найти объявление по Id = " + adId;

			if (ad.SenderId != SM.CurrentUserId)
				return "Попытка снять с публикации чужое объявление";

			AdsDAL.UpdateAdActiveUntilDate(adId, DateTime.Now.ToUniversalTime(), true);

			return "ok";
		}

		public static void SendExpiredAdAndOffersNotificationMessage()
		{
			Thread.Sleep(5 * 60 * 1000);//5 минут

			while (true)
			{
				try
				{
					//возьмем истекшие объявления
					var ads = AdsDAL.GetAds(adStatusId: (int)AdStatuses.Expired, isUnpublishedByUser: false, isSendedExpiredMessage: false);
					if (ads.Any())
					{
						var adIdsWithSendedMessage = new List<int>();
						foreach (var ad in ads)
						{
							var message = new Message()
							{
								Text = "Ваше объявление " + ad.Name + " истекло. Пожалуйста, продлите его, если оно актуально.",
								SenderId = Models.User.SystemNotificationsUserId,
								RecipientId = ad.SenderId,
								AdId = ad.Id
							};
							bool messageSended = UserHelper.AddMessage(message) > 0;
							if (messageSended)
								adIdsWithSendedMessage.Add(ad.Id);
						}

						if (adIdsWithSendedMessage.Any())
							AdsDAL.UpdateAdSetIsSendedExpiredMessage(adIdsWithSendedMessage, true);
					}

					//возьмем истекшие предложения
					var offers = OffersDAL.GetExpiredOffersForActiveAdsForSendMessage();
					if (offers.Any())
					{
						var adIds = offers.Select(o => o.AdId).Distinct().ToList();
						var adsForExpiredOffers = AdsDAL.GetAds(adIds);
						var offerIdsWithSendedMessage = new List<int>();
						foreach (var offer in offers)
						{
							var ad = adsForExpiredOffers.FirstOrDefault(a => a.Id == offer.AdId);
							if (ad != null)
							{
								var message = new Message()
								{
									Text = "Ваше предложение по объявлению " + ad.Name + " истекло. Пожалуйста, обновите его, если оно актуально.",
									AdId = ad.Id,
									OfferId = offer.Id,
									SenderId = Models.User.SystemNotificationsUserId,
									RecipientId = offer.SenderId
								};
								bool messageSended = UserHelper.AddMessage(message) > 0;
								if (messageSended)
									offerIdsWithSendedMessage.Add(offer.Id);
							}
							else
								LogsDAL.AddError($"В AdsController.SendExpiredAdAndOffersNotificationMessage не найдено объявление по Id = {offer.AdId} для offer.Id = {offer.Id}");
						}

						if (offerIdsWithSendedMessage.Any())
							OffersDAL.UpdateOfferSetIsSendedExpiredMessage(offerIdsWithSendedMessage, true);
					}
				}
				catch
				{

				}
				Thread.Sleep(5 * 60 * 1000);//5 минут
			}
		}
    }
}