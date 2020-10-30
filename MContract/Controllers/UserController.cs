using MContract.AppCode;
using MContract.DAL;
using MContract.Models;
using MContract.Models.Enums;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web;
using System.Web.UI.WebControls;
using ImageiTextSharp = iTextSharp.text.Image;
using System.Net.Mail;
using System.Net.Mime;

namespace MContract.Controllers
{
    public class UserController : Controller
    {
        #region
        public UserController()
        {
            #region Общий код для всех контроллеров
            SM.TryToLoginByCookiesIfNeed();
            ViewBag.L = LayoutViewModel.GetLayoutViewModel();
            #endregion
        }

        [MyAuthorize]
        public ActionResult MyAds()
        {
            var viewModel = new UserMyAdsViewModel();
            var currentUser = SM.CurrentUser;
            var leftMenuViewModel = LeftMenuViewModel.Get(currentUser, "Мои объявления");
            viewModel.LeftMenuViewModel = leftMenuViewModel;
            viewModel.MobileMenuViewModel = MobileMenuViewModel.Get(leftMenuViewModel);
            var currentUserId = currentUser.Id;
            var adStatusIds = new List<int>() { (int)AdStatuses.Published, (int)AdStatuses.Expired, (int)AdStatuses.Finished };
            var ads = AdsDAL.GetAds(adStatusIds: adStatusIds, senderId: currentUserId);
            ads = ads.Where(a => a.ModerateResult == ModerateResults.Accepted).OrderByDescending(a => a.Id).ToList();
            viewModel.Ads = AdHelper.GetAdsForView(currentUserId, ads: ads, needInvitedUserIds: true, needSender: true);

            //заполнение модели для отображения блока "Вы недавно смотрели"
            var recentlyViewedAds = AdHelper.GetRecentlyViewedAds(currentUserId);
            viewModel.AdsSliderViewModel = new AdsSliderViewModel("Вы недавно смотрели", currentUserId, recentlyViewedAds);

            ViewBag.L.HideHead = true;

            return View(viewModel);
        }

        public ActionResult Help()
        {
            //ViewBag.L.HideHead = true;

            return View();
        }

        public ActionResult Feedback()
        {
            //ViewBag.L.HideHead = true;

            return View();
        }

        public ActionResult Rules()
        {
            //ViewBag.L.HideHead = true;

            return View();
        }



        [MyAuthorize]
        public ActionResult Favorites()
        {
            var viewModel = new UserFavoritesViewModel();
            var currentUser = SM.CurrentUser;
            var leftMenuViewModel = LeftMenuViewModel.Get(currentUser, "Избранное");
            viewModel.LeftMenuViewModel = leftMenuViewModel;
            viewModel.MobileMenuViewModel = MobileMenuViewModel.Get(leftMenuViewModel);
            var currentUserId = currentUser.Id;
            viewModel.CurrentUserId = currentUserId;
            var ads = AdsDAL.GetFavoriteAds(currentUserId);

            viewModel.Ads = AdHelper.GetAdsForView(currentUserId, ads: ads, needSender: true).OrderByDescending(a => a.Id).ToList();

            //заполнение модели для отображения блока "Похожие объявления"
            var recentlyViewedAds = AdHelper.GetRecentlyViewedAds(currentUserId);//TODO переделать, когда будет алгоритм получения похожих объявлений
            viewModel.AdsSliderViewModel = new AdsSliderViewModel("Похожие объявления", currentUserId, recentlyViewedAds);

            ViewBag.L.HideHead = true;

            return View(viewModel);
        }

        [MyAuthorize]
        public ActionResult MyOffers()
        {
            var viewModel = new UserMyOffersViewModel();
            var currentUser = SM.CurrentUser;
            var leftMenuViewModel = LeftMenuViewModel.Get(currentUser, "Мои отклики");
            viewModel.LeftMenuViewModel = leftMenuViewModel;
            viewModel.MobileMenuViewModel = MobileMenuViewModel.Get(leftMenuViewModel);
            viewModel.OutgoingOffers = OffersDAL.GetOffersFromUser(currentUser.Id).OrderByDescending(o => (o.Modified ?? o.DateOfPosting)).ToList();
            if (viewModel.OutgoingOffers.Any())
            {
                var allProductCategories = ProductCategoriesDAL.GetCategories();
                var allProducts = AdProductsDAL.GetAdProducts();
                //viewModel.OutgoingOffers = viewModel.OutgoingOffers.Where(o => o.OfferStatus != OfferStatuses.Expired).ToList();
                var dollarRate = TickersHelper.GetTodayUsdQuote();
                foreach (var offer in viewModel.OutgoingOffers)
                {
                    offer.Sender = UsersDAL.GetUser(offer.SenderId);
                    offer.Ad = AdsDAL.GetAd(offer.AdId);
                    if (offer.Ad != null)
                    {
                        offer.Ad.Sender = UsersDAL.GetUser(offer.Ad.SenderId);
                        if (offer.Ad.Sender != null)
                            offer.Ad.Sender.Town = TownsDAL.GetTown(offer.Ad.Sender.CityId);
                    }
                    if (offer.CityId != null)
                        offer.City = TownsDAL.GetTown(offer.CityId.Value);
                    offer.SumProduct = new List<float>();
                    offer.ProductOffers = ProductOffersDAL.GetProductOffers(offer.Id);
                    if (offer.ProductOffers.Any())
                    {
                        offer.ProductOffersDescription = "";
                        var i = 0;
                        var numOfPreviewedProducts = 3;
                        foreach (var productOffer in offer.ProductOffers)
                        {
                            var product = allProducts.Find(p => p.Id == productOffer.ProductId);
                            if (product != null)
                            {
                                var sumProduct = product.Weight * productOffer.PricePerWeight * (product.Currency == Currencies.Dollar ? dollarRate : 1);
                                offer.SumProduct.Add(sumProduct);
                                if (i < numOfPreviewedProducts)
                                {
                                    product.ProductCategoryName = allProductCategories.Find(c => c.Id == product.ProductCategoryId)?.Name;
                                    if (product.Name != null)
                                        product.Name = product.Name.Substring(0, 1).ToLower() + product.Name.Substring(1);

                                    if (i > 0)
                                    {
                                        offer.ProductOffersDescription += ", ";
                                        product.ProductCategoryName = product.ProductCategoryName.Substring(0, 1).ToLower() + product.ProductCategoryName.Substring(1);
                                    }
                                    if (product.Name != null)
                                        offer.ProductOffersDescription += product.ProductCategoryName + " (" + product.Name + ") – " + offer.SumProduct[i] + " руб.";
                                    else
                                        offer.ProductOffersDescription += product.ProductCategoryName + " – " + offer.SumProduct[i] + " руб.";
                                }
                                else if (i == numOfPreviewedProducts)
                                    offer.ProductOffersDescription += ", ...";
                                i++;
                            }
                        }
                    }
                }
                viewModel.OutgoingOffers = viewModel.OutgoingOffers
                    .Where(o =>
                    o.Ad != null
                    && o.Ad.AdStatus != AdStatuses.Finished
                    && (o.Ad.AdStatus != AdStatuses.Expired || (DateTime.Now.ToUniversalTime() - o.Ad.ActiveToDate).Value.Days < 60))
                    .ToList();
            }

            var ads = viewModel.OutgoingOffers.Select(o => o.Ad).ToList();
            ads = AdHelper.GetAdsForView(currentUser.Id, ads: ads);
            foreach (var offer in viewModel.OutgoingOffers)
            {
                offer.Ad = ads.FirstOrDefault(a => a.Id == offer.AdId);
            }

            ViewBag.L.HideHead = true;

            ViewBag.Heading = "Мои отклики";

            return View(viewModel);
        }

        [MyAuthorize]
        public ActionResult Drafts()
        {
            var viewModel = new UserDraftsViewModel();
            var currentUser = SM.CurrentUser;
            var leftMenuViewModel = LeftMenuViewModel.Get(currentUser, "Черновики");
            viewModel.LeftMenuViewModel = leftMenuViewModel;
            viewModel.MobileMenuViewModel = MobileMenuViewModel.Get(leftMenuViewModel);
            viewModel.CurrentUser = currentUser;
            var currentUserId = currentUser.Id;
            var ads = AdsDAL.GetAds(adStatusId: (int)AdStatuses.Draft, senderId: currentUserId);

            viewModel.Ads = AdHelper.GetAdsForView(currentUserId, ads: ads).OrderByDescending(a => a.Id).ToList();

            //заполнение модели для отображения блока "Похожие объявления"
            var similarAds = AdHelper.GetRecentlyViewedAds(currentUserId);//TODO сделать когда будет алгоритм получения похожих объявлений
            viewModel.AdsSliderViewModel = new AdsSliderViewModel("Похожие объявления", currentUserId, similarAds);

            ViewBag.L.HideHead = true;

            return View(viewModel);
        }

        [MyAuthorize]
        public ActionResult MyProfile()
        {
            var viewModel = new UserMyProfileViewModel();
            var currentUser = SM.CurrentUser;
            if (currentUser.Town == null)
                currentUser.Town = TownsDAL.GetTown(currentUser.CityId);

            var leftMenuViewModel = LeftMenuViewModel.Get(currentUser, "Данные профиля");
            viewModel.LeftMenuViewModel = leftMenuViewModel;
            viewModel.MobileMenuViewModel = MobileMenuViewModel.Get(leftMenuViewModel);
            viewModel.CurrentUser = currentUser;

            //заполнение модели для отображения блока "Вы недавно смотрели"
            var recentlyViewedAds = AdHelper.GetRecentlyViewedAds(currentUser.Id);
            viewModel.AdsSliderViewModel = new AdsSliderViewModel("Вы недавно смотрели", currentUser.Id, recentlyViewedAds);

            ViewBag.L.HideHead = true;

            return View(viewModel);
        }

        [MyAuthorize]
        public ActionResult CounteragentProfile()
        {
            var viewModel = new UserCounteragentProfileViewModel();
            var currentUser = SM.CurrentUser;
            var leftMenuViewModel = LeftMenuViewModel.Get(currentUser, "Данные контрагента");
            viewModel.LeftMenuViewModel = leftMenuViewModel;
            viewModel.MobileMenuViewModel = MobileMenuViewModel.Get(leftMenuViewModel);
            viewModel.CurrentUser = currentUser;


            ViewBag.L.HideHead = true;

            return View(viewModel);
        }

        [MyAuthorize]
        public ActionResult RegularClients() //бывшая User/Companies?isRegularClients=true
        {
            var viewModel = new UserRegularClients();
            var currentUser = SM.CurrentUser;
            var leftMenuViewModel = LeftMenuViewModel.Get(currentUser, "Постоянные клиенты");
            viewModel.LeftMenuViewModel = leftMenuViewModel;
            viewModel.MobileMenuViewModel = MobileMenuViewModel.Get(leftMenuViewModel);
            var currentUserId = currentUser.Id;
            viewModel.CurrentUserId = currentUserId;
            var regularClients = UsersDAL.GetRegularClients(currentUserId);
            if (regularClients.Any())
            {
                var towns = TownsDAL.GetTowns();
                var regularClientUserIds = regularClients.Select(rc => rc.Id).ToList();
                var logoPhotosAll = PhotosDAL.GetCompanyLogoGroup(userIds: regularClientUserIds);
                foreach (var user in regularClients)
                {
                    if (user.CityId > 0)
                        user.Town = towns.Find(t => user.CityId == t.Id);

                    var logoPhoto = logoPhotosAll.Where(p => p.UserId == user.Id).OrderBy(p => p.Width).FirstOrDefault();
                    user.SmallPhotoUrl = logoPhoto != null ? logoPhoto.Url : PhotoHelper.NoLogoImageUrl;
                }
            }
            viewModel.RegularClients = regularClients;

            //заполнение модели для отображения блока "Вы недавно смотрели"
            var recentlyViewedAds = AdHelper.GetRecentlyViewedAds(currentUserId);
            viewModel.AdsSliderViewModel = new AdsSliderViewModel("Вы недавно смотрели", currentUserId, recentlyViewedAds);

            ViewBag.L.HideHead = true;

            return View(viewModel);
        }

        public List<string> GetCompanyNamesByPart(string partName)
        {
            var allUsers = UsersDAL.GetUsers();
            var result = allUsers.Where(u => u.CompanyName.Contains(partName)).Select(u => u.CompanyName).ToList();
            return result;
        }

        #endregion

        #region Проверка нового ЮЗЕРА на уникальность по e-mail и ИНН

        public string UniqueEmail(string newEmail)
        {
            /*  проверяем существование пользователя в базе данных с только что введенным e-mail               
              если такого пользователя нет то ничего не делаем
              если есть то сообщаем пользователю об этом 
              блокируем внесение данных ... 
          */
            if (UsersDAL.UserEmailUnique(newEmail) == false)
                return "false";
            return "true";
        }

        public string UniqueINN(string newINN)
        {
            if (UsersDAL.UserINNUnique(newINN) == false)
                return "false";
            return "true";
        }
        #endregion

        #region Повторная отправка запроса для подтверждения регистрации

        public string Resendemail(string email)
        {
            /* 
              НЕОБХОДИМО ПЕРЕДАТЬ e-mail USERA на клиенте == button
             $.ajax('/User/Resendemail',
             */
            Guid g;
            g = Guid.NewGuid();
            string _g = Convert.ToString(g);
            var dbUser = UsersDAL.GetUserByEmail(email);

            if (dbUser != null)
            {
                int IdUser = dbUser.Id;

                if (UsersDAL.UpdateVerificationCode(IdUser, _g) == true)
                {
                    string sendTo = dbUser.Email;
                    #region Повторное потдверждение почтового ящика зарегистрированного ЮЗЕРА
                    string subscription = C.SiteUrl + "User/VerificationEmail?token=" + _g;
                    string subject = "Повторное подтверждение регистрации";
                    string body = "Уважаемый новый пользователь портала M-contract" + "<br/>" + "<br/>" +
                    "Вам необходимо подтвердить данные Вашего почтового ящика" + "<br/>" + "<br/>" +
                    "для подтверждения - перейдите по ссылке - " + "<a href=\'" + subscription + "'>Подтвердить e-mail</a>." + "<br/>"
                    + "<br/>" + "<i>" + "С уважением команда портала M-contract" + "</i>";
                    MailHelper.SendMail(sendTo, subject, body);
                    #endregion
                    return "Письмо подтверждения отправлено повторно на Ваш e-mail - " + sendTo + ", указанный Вами при регистрации";
                }
                else
                    return "Ошибка отправки почты на  Ваш e-mail - " + email;
            }
            else
                return "Пользователя с данным именем " + email + " не существует. Проверьте правильность ввода или зарегистрируйтесь.";
        }
        #endregion

        #region Верификация e-mail 
        [HttpGet]
        public ActionResult VerificationEmail(string token)
        {
            var viewModel = new UserLogoutViewModel();

            ViewBag.L.ShowSearchbar = false;
            if (Response.Cookies != null && Response.Cookies["ep"] != null)
                Response.Cookies["ep"].Expires = DateTime.Now.AddDays(-1);
            else
                LogsDAL.AddError("Response.Cookies = null или Response.Cookies[ep] = null в UserController.Logout()");

            Session.Clear();

            ViewBag.Heading = "Изменения e-mail пользователя";
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>
            {
                new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true }
            };
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion

            if (token != null && token.Length == 36)
            {
                var user = UsersDAL.GetUserByToken(token);

                if (user != null)
                {
                    Guid g;
                    g = Guid.NewGuid();
                    string _g = Convert.ToString(g);

                    if (UsersDAL.UpdateUserEmailConfirmed(user.Id, _g) == true)
                    {
                        ViewData["tempdata"] = user.Email;
                        ViewData["tempdata1"] = "Ваш e-mail - подтвержден успешно";

                        return View(viewModel);
                    }
                    else
                        return View(viewModel);
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Отправка запроса - напоминаем пароль

        public string Resendpassword(string email)
        {
            Guid g;
            g = Guid.NewGuid();
            string _g = Convert.ToString(g);

            var dbUser = UsersDAL.GetUserByEmail(email);

            if (dbUser != null)
            {
                int IdUser = dbUser.Id;
                if (UsersDAL.UpdateVerificationCode(IdUser, _g) == true)
                {
                    #region Отправка письма - напоминаем пароль зарегистрированного ЮЗЕРА
                    string sendTo = dbUser.Email;
                    string subscription = C.SiteUrl + "User/ResetPassword?token=" + _g;
                    string subject = "Восстановление доступа";
                    string body = "Уважаемый пользователь портала M-contract" + "<br/>" + "<br/>" +
                    "Для восстановления доступа к порталу - перейдите по ссылке - " + "<a href=\'" + subscription + "'>Восстановить пароль</a>." + "<br/>"
                    + "<br/>" + "<i>" + "С уважением команда портала M-contract" + "</i>"
                    + "<br/>" + "<br/>" + "Если это письмо пришло Вам ошибочно - просто проигнорируйте это письмо";
                    MailHelper.SendMail(sendTo, subject, body);
                    #endregion 
                    return "Для восстановления доступа к порталу - перейдите по ссылке в письме, отправленном на Ваш e-mail - " + sendTo;
                }
                else
                    return "Ошибка отправки почты на  Ваш e-mail - " + email;
            }
            else
                return "Пользователя с данным именем " + email + " не существует. Проверьте правильность ввода или зарегистрируйтесь.";
        }
        #endregion

        #region Задать новый пароль Забыли - При получении писсьма от ЮЗЕРА с Токеном
        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            ViewBag.token = token;

            if (token != null && token.Length == 36)
            {
                var user = UsersDAL.GetUserByToken(token);

                if (user != null)
                {
                    ViewBag.ContactName = user.ContactName;
                    ViewBag.Email = user.Email;
                    return View();
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        public string ResetPassword(string token, string newPassword)
        {
            ViewData["tempdata"] = "";
            ViewData["tempdata1"] = "";

            if (token != null)
            {
                var user = UsersDAL.GetUserByToken(token);

                Guid g;
                g = Guid.NewGuid();
                string _g = Convert.ToString(g);

                bool success = UsersDAL.UpdateUserResetPassword(user.Id, _g, SecurityHelper.Encryption(newPassword));
                if (success)
                {
                    //ViewData["tempdata"] = user.ContactName + " - ваш пароль успешно обновлен";
                    //ViewData["tempdata1"] = "Для завершения процедуры - перейдите по ссылке отправленной на Ваш e-mail - " + user.Email;
                }
                else
                    ViewData["tempdata"] = "Произошла ошибка при обновлениие пароля, попробуйте позже";
                ViewData["tempdata1"] = "";


            }
            return "ОК";
        }

        #endregion

        #region проверить текущий пароль В личном кабинете при вводе
        [MyAuthorize]
        public string СurretnPassword(string curretnPassword)
        {
            User currentUser = SM.CurrentUser;
            if (SecurityHelper.Decryption(currentUser.Password) != curretnPassword)
                return "false";
            return "true";
        }
        #endregion

        #region Регистрация нового ЮЗЕРА 
        [HttpGet]
        public ActionResult Signup()
        {
            ViewBag.L.ShowSearchbar = false;
            ViewBag.Heading = "Регистрация";
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>
            {
                new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true }
            };
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View();
        }

        [HttpPost]
        public string Signup(User user)
        {
            //делаем запрос в СБИС, заполняем данные пользователя из СБИС
            P.FillInformationFromSbis(user);
            #region проверяем данные из СБИС с данными, предоставленными пользователем, и подтверждаем пользователя либо ничего не делаем и он автоматически появится на странице модерации
            if (user.OGRN == null)
                user.OGRN = "";

            if (user.OGRN.Replace("-", "") == user.SbisOGRN && user.CompanyName == user.SbisCompanyName && (int)user.TypeOfOwnership == user.SbisTypeOfOwnershipId)
                user.ModerateResult = ModerateResults.Accepted;
            #endregion

            if (user.ModerateResult == ModerateResults.Accepted)
            {
                #region Добавление пользователя в БД в таблицу Users
                user.Created = DateTime.Now.ToUniversalTime();
                user.LastOnline = DateTime.Now.ToUniversalTime();
                user.Password = SecurityHelper.Encryption(user.Password);
                #region Определение Id-города
                //if (user.CityId != 0)
                //{
                //	user.Town = TownsDAL.GetTown(user.CityId);
                //	int ind1 = user.TownName.IndexOf(" (");
                //	if (ind1 != -1)
                //	{
                //		string townName = user.TownName.Remove(ind1);
                //		string regionName = user.TownName.Substring(ind1 + " (".Length);
                //		regionName = regionName.Remove(regionName.Length - 1);
                //		var town = TownsDAL.GetTown(townName, regionName);
                //		if (town != null)
                //			user.CityId = town.Id;
                //		else
                //			LogsDAL.AddError("User/SignUp[post]: Не удалось определить город, user.SingUpCityStr = " + user.TownName);
                //	}
                //	else
                //		LogsDAL.AddError("User/SignUp[post]: Не удалось определить город, user.SingUpCityStr = " + user.TownName);
                //}
                //else
                //	LogsDAL.AddError("User/SignUp[post]: user.SingUpCityStr = null");
                #endregion
                var id = UsersDAL.AddUser(user);
                if (id > 0)
                {
                    /*
                     * Заремарено чтобы после регистрации при переходе на страницу SignupChecking 
                     * у нового Юзера так как он не подтвердил данные своего почтового ящика 
                     * не было доступа в Личный кабинет
                    */
                    //CookiesHelper.SaveCookiesForHideAuthorization(user.Email, "", SecurityHelper.Decryption(user.Password).TrimEnd());

                    #region Потдверждение почтого ящика нового ЮЗЕРА
                    var userNew = UsersDAL.GetUser(id);

                    string subscription = C.SiteUrl + "User/VerificationEmail?token=" + userNew.VerificationCode;
                    string sendTo = userNew.Email;
                    string subject = "Подтверждение регистрации";
                    string body = "Уважаемый новый пользователь портала M-contract" + "<br/>" + "<br/>" +
                    "Вам необходимо подтвердить данные Вашего почтового ящика" + "<br/>" + "<br/>" +
                    "для подтверждения - перейдите по ссылке - " + "<a href=\'" + subscription + "'>Подтвердить e-mail</a>." + "<br/>"
                    + "<br/>" + "<i>" + "С уважением команда портала M-contract" + "</i>";
                    MailHelper.SendMail(sendTo, subject, body);
                    #endregion

                    return "user created";
                }
                else
                    throw new Exception("Не удалось создать пользователя, пожалуйста, попробуйте позже");
                #endregion
            }
            else
            {
                #region Добавление незарегистрированного пользователя в БД в таблицу UnregisteredUsers

                var unregisteredUser = user.ToUnregisteredUser();
                unregisteredUser.Created = DateTime.Now.ToUniversalTime();
                var id = UnregisteredUsersDAL.AddUnregisteredUser(unregisteredUser);
                if (id > 0)
                {
                    return "unregisteredUser created";
                }

                #endregion

                return "Не удалось записать ваши данные, пожалуйста, попробуйте позже";
            }
        }
        #endregion

        #region После Регистрация нового ЮЗЕРА 20201024
        [HttpGet]
        public ActionResult SignupChecking()
        {
            var viewModel = new UserLogoutViewModel();

            ViewBag.L.ShowSearchbar = false;
            if (Response.Cookies != null && Response.Cookies["ep"] != null)
                Response.Cookies["ep"].Expires = DateTime.Now.AddDays(-1);
            else
                LogsDAL.AddError("Response.Cookies = null или Response.Cookies[ep] = null в UserController.Logout()");

            Session.Clear();

            ViewBag.Heading = "Подтверждение регистрации";
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>
            {
                new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true }
            };
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(viewModel);
        }
        #endregion

        #region После изменения e-mail существующего ЮЗЕРА 20201025
        [HttpGet]
        public ActionResult UpdateEmail()
        {
            var viewModel = new UserLogoutViewModel();

            ViewBag.L.ShowSearchbar = false;
            if (Response.Cookies != null && Response.Cookies["ep"] != null)
                Response.Cookies["ep"].Expires = DateTime.Now.AddDays(-1);
            else
                LogsDAL.AddError("Response.Cookies = null или Response.Cookies[ep] = null в UserController.Logout()");

            Session.Clear();

            ViewBag.Heading = "Изменения e-mail пользователя";
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>
            {
                new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true }
            };
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(viewModel);
        }

        #endregion

        #region Задать новый пароль В личном кабинете
        [HttpPost]
        [MyAuthorize]
        public string ChangePassword(string currentPassword, string newPassword)
        {
            var currentUser = SM.CurrentUser;
            if (SecurityHelper.Decryption(currentUser.Password) != currentPassword)
                return "Текущий пароль указан неверно";

            var success = UsersDAL.UpdateUserPassword(currentUser.Id, SecurityHelper.Encryption(newPassword));
            if (success)
            {
                currentUser.Password = newPassword;
                SM.CurrentUser = currentUser;
                CookiesHelper.SaveCookiesForHideAuthorization(currentUser.Email, "", currentUser.Password);
                return "ok";
            }
            else
                return "Произошла ошибка при обновлениие пароля, попробуйте позже";
        }
        #endregion

        #region Изменения данных ЮЗЕРА
        [HttpPost]
        [MyAuthorize]
        public string SaveUserData(User formUser)
        {
            var currentUser = SM.CurrentUser;
            //var emailChanged = formUser.Email != currentUser.Email;
            #region Если НЕ изменили Email
            if (formUser.Email == currentUser.Email)
            {
                currentUser.ContactName = formUser.ContactName;
                currentUser.Email = formUser.Email;
                currentUser.PhoneNumber = formUser.PhoneNumber;
                currentUser.PhoneNumberCity = formUser.PhoneNumberCity;
                currentUser.Address = formUser.Address;
                currentUser.FactualAddress = formUser.FactualAddress;

                var success = UsersDAL.UpdateUser(currentUser);
                if (success)
                {
                    CookiesHelper.SaveCookiesForHideAuthorization(currentUser.Email, "", currentUser.Password.TrimEnd());
                    return "ok";
                }
                else
                    return "Произошла ошибка при сохранении, попробуйте позже";
            }
            #endregion
            #region Если ИЗМЕНИЛИ Email
            else
            {
                var successemail = UsersDAL.UserEmailUnique(formUser.Email);
                if (successemail)
                {
                    currentUser.ContactName = formUser.ContactName;
                    currentUser.Email = formUser.Email;
                    currentUser.PhoneNumber = formUser.PhoneNumber;
                    currentUser.PhoneNumberCity = formUser.PhoneNumberCity;
                    currentUser.Address = formUser.Address;
                    currentUser.FactualAddress = formUser.FactualAddress;

                    var success = UsersDAL.UpdateUser(currentUser);
                    if (success)
                    {
                        /*
                     * Заремарено чтобы после изменения e-mail 
                     * у ЮЗЕРа так как он не подтвердил данные своего НОВОГО e-mail 
                     * не было доступа в Личный кабинет
                    */
                        CookiesHelper.SaveCookiesForHideAuthorization("", "", "");
                        Guid g;
                        g = Guid.NewGuid();
                        string _g = Convert.ToString(g);
                        var dbUser = UsersDAL.GetUserByEmail(currentUser.Email);

                        if (dbUser != null)
                        {
                            int IdUser = dbUser.Id;

                            if (UsersDAL.UpdateUserEmailNOConfirmed(IdUser, _g) == true)
                            {
                                string sendTo = dbUser.Email;
                                #region Подтверждение Вашего нового почтового ящика зарегистрированного ЮЗЕРА
                                string subscription = C.SiteUrl + "User/VerificationEmail?token=" + _g;
                                string subject = "Подтверждение Вашего нового почтового ящика";
                                string body = "Уважаемый пользователь портала M-contract" + "<br/>" + "<br/>" +
                                "Вам необходимо подтвердить данные Вашего нового почтового ящика" + "<br/>" + "<br/>" +
                                "для подтверждения - перейдите по ссылке - " + "<a href=\'" + subscription + "'>Подтвердить e-mail</a>." + "<br/>"
                                + "<br/>" + "<i>" + "С уважением команда портала M-contract" + "</i>";
                                MailHelper.SendMail(sendTo, subject, body);
                                #endregion
                                return "yes";
                            }
                            else
                                return "Произошла ошибка при сохранении, попробуйте позже";

                            /* 20201024 
                             * и необходимо отправить письмо 
                             * ПОДТВЕРДИТЬ НОВЫЙ e-mail 
                             * Поле e-mail ВЕРИФИКАЦИЯ в таблице юзер == false 
                             * обновить Token 
                             * и после подтверждения e-mail с новым Token 
                             * Поле e-mail ВЕРИФИКАЦИЯ в таблице юзер == true 
                             * и желательно выкинуть Юзера на страницу входа ... 
                             * Это все надо реализовать завтра ... 
                             */
                        }
                        else
                            return "Произошла ошибка при сохранении, попробуйте позже";
                    }
                    else
                        return "Произошла ошибка при сохранении, попробуйте позже";
                }
                else
                    return "Пользователь с данным e-mail - " + formUser.Email + " уже зарегистрирован в базе данных";
            }
            #endregion
        }
        #endregion

        #region Проверка существующего ЮЗЕРА на уникальность по e-mail при изменении
        [MyAuthorize]
        public string UniqueEmailUpdate(string newEmail)
        {
            var currentUser = SM.CurrentUser;

            if (newEmail != currentUser.Email)
            {
                if (UsersDAL.UserEmailUnique(newEmail) == false)

                    return "false";

                return "true";
            }
            return "true";
        }
        #endregion

        #region Удаление ЮЗЕРА из системы
        [HttpPost]
        [MyAuthorize]
        public string RemoveCompanyFromSite()
        {
            var currentUser = SM.CurrentUser;

            var success = UsersDAL.UpdateUserDeleted(currentUser.Id, true);

            AdsDAL.UpdateNotFinishedAdsToDeleted(currentUser.Id);
            OffersDAL.UpdateNotFinishedOffersToDeleted(currentUser.Id);

            return success ? "ok" : "Произошла ошибка при удалении, попробуйте позже";
        }
        #endregion

        #region LOGIN Вход в систему
        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.L.ShowSearchbar = false;
            var viewModel = new User();
            ViewBag.Heading = "Вход";
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>
            {
                new BreadCrumbLink() { Text = "Личный кабинет", Url = Urls.PersonalArea, Title = "Перейти в личный кабинет" },
                new BreadCrumbLink() { Text = "Вход", EndPoint = true }
            };
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Login(User formUser)
        {
            var dbUser = UsersDAL.GetUserByEmail(formUser.Email);

            if (dbUser != null)
            {
                if (SecurityHelper.Decryption(dbUser.Password) == formUser.Password)
                {
                    //успешная авторизация
                    if (dbUser.EmailConfirmed == true)
                    {
                        //EMAIL подтвержден
                        SM.CurrentUser = dbUser;
                        SM.LoginTime = DateTime.Now;
                        CookiesHelper.SaveCookiesForHideAuthorization(dbUser.Email, "", SecurityHelper.Decryption(dbUser.Password).TrimEnd());
                        var returnUrl = Request["ReturnUrl"];
                        if (!string.IsNullOrWhiteSpace(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return Redirect(Urls.PersonalArea);
                    }
                    else
                        formUser.ErrorMessage = "Вы не подтвердили Ваш e-mail. Хотите получить письмо подтверждения повторно?";
                    //#region Повторная отправка запроса для подтверждения регистрации == 330
                }

                else
                    formUser.ErrorMessage = "Неверный пароль";
            }
            else
                formUser.ErrorMessage = "Пользователь с данным e-mail не найден. Хотите <a href=\"" + Urls.Registration + "\">зарегистрироваться?</a>";

            return View(formUser);
        }

        public ActionResult Logout(bool deletedCompany = false)
        {
            var viewModel = new UserLogoutViewModel()
            {
                DeletedCompany = deletedCompany
            };

            ViewBag.L.ShowSearchbar = false;
            if (Response.Cookies != null && Response.Cookies["ep"] != null)
                Response.Cookies["ep"].Expires = DateTime.Now.AddDays(-1);
            else
                LogsDAL.AddError("Response.Cookies = null или Response.Cookies[ep] = null в UserController.Logout()");

            Session.Clear();

            ViewBag.Heading = "Выход";
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>
            {
                new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true }
            };
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(viewModel);
        }
        #endregion

        #region
        public ActionResult UserAgreement()
        {
            return View();
        }

        public ActionResult ProcessingPersonalData()
        {
            return View();
        }

        [MyAuthorize]
        public new ActionResult Profile(int? id)
        {
            ViewBag.L.ShowSearchbar = false;
            if (id == null)
            {
                id = SM.CurrentUser.Id;
            }
            var user = UsersDAL.GetUser((int)id);
            if (user == null)
                throw new Exception("user == null");
            user.Town = TownsDAL.GetTown(user.CityId);
            user.LogoGroup = PhotosDAL.GetCompanyLogoGroup(user.Id);
            user.PersonalAreaUser = SM.GetPersonalAreaUser();
            user.PersonalAreaUser.RegularClients = UsersDAL.GetRegularClients(user.PersonalAreaUser.Id);
            user.AllUsers = UsersDAL.GetUsers();
            var allTowns = TownsDAL.GetTowns();
            foreach (var u in user.AllUsers)
            {
                u.Town = allTowns.Find(t => t.Id == u.CityId);
            }
            ViewBag.Heading = user.CompanyNameWithTypeOfOwnership;
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>();
            if (user.Id == user.PersonalAreaUser.Id)
            {
                breadCrumbs.Add(new BreadCrumbLink() { Text = "Личный кабинет", EndPoint = true });
            }
            else
            {
                breadCrumbs.Add(new BreadCrumbLink() { Text = "Личный кабинет", Url = Urls.PersonalArea, Title = "Перейти в личный кабинет" });
                breadCrumbs.Add(new BreadCrumbLink() { Text = "Профиль", EndPoint = true });
            }
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(user);
        }

        [MyAuthorize]
        public ActionResult EditProfile()
        {
            ViewBag.L.ShowSearchbar = false;
            var user = SM.GetPersonalAreaUser();
            if (user.Id == 0)
                return Redirect("Signup");
            user.Town = TownsDAL.GetTown(user.CityId);
            ViewBag.Heading = user.CompanyNameWithTypeOfOwnership;
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>
            {
                new BreadCrumbLink() { Text = "Личный кабинет", Url = Urls.PersonalArea, Title = "Перейти в личный кабинет" },
                new BreadCrumbLink() { Text = "Редактирование", EndPoint = true }
            };
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(user);
        }

        public ActionResult Search()
        {
            var companyNames = new List<string>();
            var allUsers = UsersDAL.GetUsers();
            foreach (var user in allUsers)
            {
                companyNames.Add(user.CompanyName);
            }
            var viewModel = new UserSearchViewModel
            {
                companyNames = companyNames
            };
            ViewBag.Heading = "Поиск";
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>
            {
                new BreadCrumbLink() { Text = "Объявления", Url = Urls.Ads, Title = "Перейти к списку всех объявлений" },
                new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true }
            };
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(viewModel);
        }
        public ActionResult Companies(bool isRegularClients = false)
        {
            ViewBag.L.ShowSearchbar = false;
            var viewModel = new UserCompaniesViewModel
            {
                PersonalAreaUser = SM.GetPersonalAreaUser()
            };
            viewModel.PersonalAreaUser.RegularClients = UsersDAL.GetRegularClients(viewModel.PersonalAreaUser.Id);
            if (isRegularClients == false)
            {
                viewModel.Companies = UsersDAL.GetUsers();
            }
            else
            {
                viewModel.Companies = UsersDAL.GetRegularClients(viewModel.PersonalAreaUser.Id);
                viewModel.IsRegularClients = true;
            }
            var towns = TownsDAL.GetTowns();
            if (viewModel.Companies.Any())
            {
                foreach (var company in viewModel.Companies)
                {
                    if (company.CityId > 0)
                    {
                        company.Town = towns.Find(t => company.CityId == t.Id);
                    }
                    company.LogoGroup = PhotosDAL.GetCompanyLogoGroup(company.Id);
                    company.SmallPhotoUrl = company.GetBestFitLogoPhoto(1).Url;
                }
            }
            var heading = "";
            if (isRegularClients == true)
            {
                heading = "мои постоянные клиенты";
            }
            else
            {
                heading = "компании";
            }
            ViewBag.Heading = heading.Substring(0, 1).ToUpper() + heading.Substring(1);
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>
            {
                new BreadCrumbLink() { Text = "Личный кабинет", Url = Urls.PersonalArea, Title = "Перейти в личный кабинет" }
            };
            if (isRegularClients == true)
            {
                breadCrumbs.Add(new BreadCrumbLink() { Text = "Компании", Url = Urls.Companies, Title = "Перейти к списку компаний" });
            }
            breadCrumbs.Add(new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true });
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(viewModel);
        }
        #endregion

        #region История сделок
        [MyAuthorize]
        public ActionResult DealsHistory()
        {
            ViewBag.L.HideHead = true;
            var viewModel = new UserDealsHistoryViewModel();
            var currentUser = SM.CurrentUser;
            var leftMenuViewModel = LeftMenuViewModel.Get(currentUser, "История сделок");
            viewModel.LeftMenuViewModel = leftMenuViewModel;
            viewModel.MobileMenuViewModel = MobileMenuViewModel.Get(leftMenuViewModel);
            var currentUserId = currentUser.Id;

            //возьмем мои объявления, по которым заключен контракт
            var ads = AdsDAL.GetAds(adStatusId: (int)AdStatuses.Finished, senderId: SM.CurrentUserId);
            var adIds = ads.Select(a => a.Id).ToList();
            //возьмем предложения со статусом Заключен контракт по этим объявлениям
            var offers = OffersDAL.GetOffers(adIds: adIds, contractStatusId: (int)ContractStatuses.Accepted);

            //возьмем мои предложения со статусом Заключен контракт
            var myWinOffers = OffersDAL.GetOffers(senderId: currentUser.Id, contractStatusId: (int)ContractStatuses.Accepted);
            //возьмем объявления по этим предложениям
            var myWinOfferIds = myWinOffers.Select(o => o.AdId).ToList();
            var myWinOfferAds = AdsDAL.GetAds(myWinOfferIds);

            //соберем свои объявления в один список, и свои предложения в один список
            ads = ads.Where(a => offers.Any(o => o.AdId == a.Id)).ToList();
            ads = ads.Union(myWinOfferAds).ToList();
            adIds = ads.Select(a => a.Id).ToList();
            offers = offers.Union(myWinOffers).ToList();


            //возьмем все необходимые объекты для заполнения полей сделок
            var adSenderIds = ads.Select(a => a.SenderId);
            var offerSenderIds = offers.Select(o => o.SenderId);
            var userIds = adSenderIds.Union(offerSenderIds).ToList();
            var users = UsersDAL.GetUsers(userIds);

            var offerIds = offers.Select(o => o.Id).ToList();
            var productOffers = ProductOffersDAL.GetProductOffers(offerIds);

            var adProducts = AdProductsDAL.GetAdProducts(adIds: adIds);

            var allProductCategories = ProductCategoriesDAL.GetCategories();

            var photos = PhotosDAL.GetPhotos(adIds: adIds, userIds: userIds);

            var deals = new List<UserDealsHistoryItem>();
            // основываясь на объявлениях соберем сделки
            foreach (var ad in ads)
            {
                // возьмем соответствующее предложение
                var offer = offers.FirstOrDefault(o => o.AdId == ad.Id);
                if (offer == null)
                {
                    LogsDAL.AddError($"В методе UserController.DealsHistory() не удалось найти выигравшее предложение для объявления с Id = {ad.Id}");
                    continue;
                }
                // отбросим сделки, удаленные из истории
                var isDealForMyAd = ad.SenderId == currentUserId;
                if (isDealForMyAd == true && ad.ShowInDealsHistory == false ||
                    isDealForMyAd == false && offer.ShowInDealsHistory == false)
                    continue;
                // возьмем соответствующего контрагента
                var counteragentId = ad.SenderId != currentUserId ? ad.SenderId : offer.SenderId;
                var counteragent = users.FirstOrDefault(u => u.Id == counteragentId);
                if (counteragent == null)
                {
                    LogsDAL.AddError($"В методе UserController.DealsHistory() не удалось найти контрагента для объявления с Id = {ad.Id}, counteragentId = {counteragentId}");
                    continue;
                }

                //заполним поля объявления
                ad.Sender = ad.SenderId == counteragent.Id ? counteragent : currentUser;
                ad.Products = adProducts.Where(p => p.AdId == ad.Id).ToList();
                ad.Photos = photos.Where(p => p.AdId == ad.Id && p.PhotoType == PhotoTypes.AdPhoto).ToList();
                if (ad.Photos.Any())
                {
                    var mainPhoto = ad.Photos.Where(p => p.IsMain).OrderBy(p => p.HigherDimension).FirstOrDefault();
                    ad.SmallPhotoUrl = mainPhoto != null ? mainPhoto.Url : PhotoHelper.NoLogoImageUrl;
                }

                ad.City = TownsDAL.GetTown(ad.CityId);

                var productCategories = new List<ProductCategory>(allProductCategories.Select(c => c.Clone()));
                var adProductCategoryIds = ad.Products.Select(p => p.ProductCategoryId).ToList();
                var adProductCategories =
                    productCategories
                        .Where(c => adProductCategoryIds.Any(id => c.Id == id))
                        .OrderBy(c => c.Id).ToList();
                ad.ProductCategoriesLevel1 = adProductCategories.Where(c => c.Level == 1).ToList();
                ad.ProductCategoriesLevel1.AddRange(productCategories.Where(
                                                        parentCategory =>
                                                        parentCategory.Level == 1 &&
                                                        !ad.ProductCategoriesLevel1.Any(c => c.Id == parentCategory.Id) &&
                                                        parentCategory.ChildrenId.Any(
                                                            childCategoryId =>
                                                            adProductCategories.Any(
                                                                childCategory =>
                                                                childCategory.Id == childCategoryId ||
                                                                childCategory.ParentId == childCategoryId))).ToList());
                ad.ProductCategoriesLevel1.ForEach(parentCategory =>
                                                   parentCategory.ChildCategories = adProductCategories
                                                                                    .Where(c => c.ParentId == parentCategory.Id ||
                                                                                           parentCategory.ChildrenId.Any(id => id == c.ParentId)).ToList());
                /*ad.ProductCategoryNames = "";
                if (adProductCategories.Any())
                {
                    var parentCategoryNameSeparator = ":<br>";
                    var childCategoryNameSeparator = "; ";
                    var parentCategories = adProductCategories.Where(c => c.Level == 1).ToList();
                    //если в объявлении не выбрано категорий 1 уровня, выбираем те категории 1 уровня,
                    //которые являются родительскими категориями любой выбранной категории или ее родителя
                    if (!parentCategories.Any())
                    {
                        parentCategories = 
                            productCategories.Where(
                                parentCategory => 
                                parentCategory.Level == 1 && 
                                parentCategory.ChildrenId.Any(
                                    childCategoryId =>
                                    adProductCategories.Any(
                                        childCategory => 
                                        childCategory.Id == childCategoryId ||
                                        childCategory.ParentId == childCategoryId))).ToList();
                    }
                    var firstParentCategory = parentCategories.First();
                    foreach (var parentCategory in parentCategories)
                    {
                        if (parentCategory != firstParentCategory)
                            ad.ProductCategoryNames += "<br>";
                        ad.ProductCategoryNames += $"<b>{parentCategory.Name}</b>";
                        //выбираем дочерние категории и дочерние категории дочерних категорий
                        var childCategories = adProductCategories.Where(c => c.ParentId == parentCategory.Id || parentCategory.ChildrenId.Any(id => id == c.ParentId)).ToList();
                        if (childCategories.Any())
                        {
                            ad.ProductCategoryNames += parentCategoryNameSeparator;
                            var lastChildCategory = childCategories.Last();
                            foreach (var childCategory in childCategories)
                            {
                                ad.ProductCategoryNames += childCategory.Name;
                                if (childCategory != lastChildCategory)
                                    ad.ProductCategoryNames += childCategoryNameSeparator;
                            }
                        }
                    }
                }*/

                //заполним поля предложения
                offer.ProductOffers = productOffers.Where(o => o.OfferId == offer.Id).ToList();
                offer.SumProduct = new List<float>();
                foreach (var productOffer in offer.ProductOffers)
                {
                    var product = ad.Products.FirstOrDefault(p => p.Id == productOffer.ProductId);
                    if (product == null)
                        continue;
                    var sumProduct = productOffer.PricePerWeight * product.Weight * (product.Currency == Currencies.Dollar ? TickersHelper.GetTodayUsdQuote() : 1);
                    offer.SumProduct.Add(sumProduct);
                }
                offer.City = TownsDAL.GetTown(offer.CityId.Value);

                //заполним поля контрагента
                counteragent.LogoGroup = photos.Where(p => p.UserId == counteragent.Id && p.PhotoType == PhotoTypes.CompanyLogo).ToList();
                counteragent.Town = TownsDAL.GetTown(counteragent.CityId);

                var productCategoryIds =
                    ad.ProductCategoriesLevel1.Union(ad.ProductCategoriesLevel1.SelectMany(c => c.ChildCategories)).Select(c => c.Id).ToList();

                var totalWeight =
                    ad.Products
                    .Where(
                        p =>
                        offer.ProductOffers
                        .Any(o => o.ProductId == p.Id))
                    .Select(p => p.Weight).Sum();

                var deal = new UserDealsHistoryItem()
                {
                    Ad = ad,
                    Offer = offer,
                    Counteragent = counteragent,
                    Date = offer.ContractSendDate ?? DateTime.MinValue,
                    IsDealForMyAd = isDealForMyAd,
                    ProductCategoryIds = productCategoryIds,
                    TotalWeight = totalWeight
                };

                deals.Add(deal);
            }

            deals = deals.OrderByDescending(d =>
                d.Date != DateTime.MinValue ? d.Date :
                d.Offer.DateOfPosting != DateTime.MinValue ? d.Offer.DateOfPosting :
                d.Ad.DateOfPosting != DateTime.MinValue ? d.Ad.DateOfPosting :
                DateTime.MinValue).ToList();
            viewModel.Deals = deals;

            viewModel.Counteragents = deals.Select(d => d.Counteragent).Distinct().ToList();

            viewModel.ProductCategories = allProductCategories.Where(c => c.Level == 1).ToList();

            return View(viewModel);
        }

        #endregion

        #region История выбранной сделки
        [MyAuthorize]
        public ActionResult DealCard(int adId)
        {
            #region Create DCard 20201028 NEW

            ViewBag.L.HideHead = true;
            var viewModel = new UserDealCardViewModel();
            var currentUser = SM.GetPersonalAreaUser();
            var currentUserId = currentUser.Id;
            currentUser.SelectedMenu = "История сделок";
            viewModel.PersonalAreaUser = currentUser;

            var ad = AdsDAL.GetAd(adId);
            if (ad == null)
                throw new Exception("Не найдено объявление по Id = " + adId);

            ad.City = TownsDAL.GetTown(ad.CityId);
            if (ad.City == null)
                throw new Exception("Не удалось найти город фактического нахождения груза по Id города = " + ad.CityId);


            viewModel.DealDirection = ad.IsBuy ? "покупка" : "продажа";

            var contractOffers = OffersDAL.GetOffers(adId: adId, contractStatusId: (int)ContractStatuses.Accepted);
            if (!contractOffers.Any())
                throw new Exception("Не найдено предложение, по которому заключен контракт, для объявления с Id = " + adId);

            var contractOffer = contractOffers.First();

            viewModel.ContractOffer = contractOffer;

            var adProducts = AdProductsDAL.GetAdProducts(adId);
            var productCategoriesIds = adProducts.Select(c => c.ProductCategoryId).Distinct().ToList();
            var productCategories = ProductCategoriesDAL.GetCategories()/*из кэша*/.Where(c => productCategoriesIds.Contains(c.Id)).ToList();
            var offerProducts = ProductOffersDAL.GetProductOffers(contractOffer.Id);
            foreach (var adProduct in adProducts)
            {
                adProduct.ProductCategoryName = productCategories.FirstOrDefault(c => c.Id == adProduct.ProductCategoryId)?.Name;
                adProduct.OfferProduct = offerProducts.FirstOrDefault(op => op.ProductId == adProduct.Id);
            }

            ad.Products = adProducts;

            viewModel.Ad = ad;

            viewModel.DealDate = contractOffer.ContractSendDate ?? DateTime.MinValue;



            var adCreator = UsersDAL.GetUser(ad.SenderId);
            if (adCreator == null)
                throw new Exception("Не удалось найти пользователя - организатора торгов по Id пользователя = " + ad.SenderId);

            var offerCreator = UsersDAL.GetUser(contractOffer.SenderId);
            if (offerCreator == null)
                throw new Exception("Не удалось найти пользователя - отправившего выигравшее предложение по Id пользователя = " + contractOffer.SenderId);

            var buyer = ad.IsBuy ? adCreator : offerCreator;
            var seller = ad.IsBuy ? offerCreator : adCreator;

            buyer.Town = TownsDAL.GetTown(buyer.CityId);
            if (buyer.Town == null)
                throw new Exception("Не удалось найти город покупателя по Id города = " + buyer.CityId);

            seller.Town = TownsDAL.GetTown(seller.CityId);
            if (seller.Town == null)
                throw new Exception("Не удалось найти город продавца по Id города = " + seller.CityId);

            if (seller.Id == currentUserId)
                buyer.Rating = UsersDAL.GetUserRating(buyer.Id, seller.Id, adId);
            else
                seller.Rating = UsersDAL.GetUserRating(seller.Id, buyer.Id, adId);

            viewModel.Buyer = buyer;
            viewModel.Seller = seller;


            //Заполним строку Условия поставки
            var deliveryType = ad.DeliveryType != DeliveryTypes.Any ? ad.DeliveryType : contractOffer.DeliveryType;
            viewModel.DeliveryType = AdHelper.GetDeliveryTypeString(deliveryType);

            //Заполним строку Погрузка
            var deliveryLoadType = ad.DeliveryLoadType != DeliveryLoadTypes.Any ? ad.DeliveryLoadType : contractOffer.DeliveryLoadType;
            viewModel.DeliveryLoadType = AdHelper.GetDeliveryLoadTypeString(deliveryLoadType);

            //Заполним строку Способ доставки
            var deliveryWay = ad.DeliveryWay != DeliveryWays.Any ? ad.DeliveryWay : contractOffer.DeliveryWay;
            viewModel.DeliveryWay = AdHelper.GetDeliveryWayString(deliveryWay);

            //Заполним строку Цена (с НДС/без НДС)
            var nds = ad.Nds != Nds.Any ? ad.Nds : contractOffer.Nds;
            viewModel.Nds = AdHelper.GetNdsString(nds);

            //Заполним строку Условия оплаты
            var termOfPayment = ad.TermsOfPayments != TermsOfPayments.Any ? ad.TermsOfPayments : contractOffer.TermsOfPayments;
            viewModel.TermsOfPayments = AdHelper.GetTermsOfPaymentsString(termOfPayment);

            //Заполним строку Цена действительна до
            viewModel.ActiveUntilDate = contractOffer.ActiveUntilDate;

            var defermentPeriod = ad.DefermentPeriod != null ? ad.DefermentPeriod : 0;
            viewModel.deferMentPeriod = Convert.ToInt32(defermentPeriod);



            #endregion
            #region Create DCard OLD 20201028
            //ViewBag.L.HideHead = true;
            //var viewModel = new UserDealCardViewModel();
            //var currentUser = SM.GetPersonalAreaUser();
            //var currentUserId = currentUser.Id;
            //currentUser.SelectedMenu = "История сделок";
            //viewModel.PersonalAreaUser = currentUser;

            //var ad = AdsDAL.GetAd(adId);
            //if (ad == null)
            //    throw new Exception("Не найдено объявление по Id = " + adId);

            //ad.City = TownsDAL.GetTown(ad.CityId);
            //if (ad.City == null)
            //    throw new Exception("Не удалось найти город фактического нахождения груза по Id города = " + ad.CityId);


            //viewModel.DealDirection = ad.IsBuy ? "покупка" : "продажа";

            //var contractOffers = OffersDAL.GetOffers(adId: adId, contractStatusId: (int)ContractStatuses.Accepted);
            //if (!contractOffers.Any())
            //    throw new Exception("Не найдено предложение, по которому заключен контракт, для объявления с Id = " + adId);

            //var contractOffer = contractOffers.First();

            //viewModel.ContractOffer = contractOffer;

            //var adProducts = AdProductsDAL.GetAdProducts(adId);
            //var productCategoriesIds = adProducts.Select(c => c.ProductCategoryId).Distinct().ToList();
            //var productCategories = ProductCategoriesDAL.GetCategories()/*из кэша*/.Where(c => productCategoriesIds.Contains(c.Id)).ToList();
            //var offerProducts = ProductOffersDAL.GetProductOffers(contractOffer.Id);
            //foreach (var adProduct in adProducts)
            //{
            //    adProduct.ProductCategoryName = productCategories.FirstOrDefault(c => c.Id == adProduct.ProductCategoryId)?.Name;
            //    adProduct.OfferProduct = offerProducts.FirstOrDefault(op => op.ProductId == adProduct.Id);
            //}

            //ad.Products = adProducts;

            //viewModel.Ad = ad;

            //viewModel.DealDate = contractOffer.ContractSendDate ?? DateTime.MinValue;



            //var adCreator = UsersDAL.GetUser(ad.SenderId);
            //if (adCreator == null)
            //    throw new Exception("Не удалось найти пользователя - организатора торгов по Id пользователя = " + ad.SenderId);

            //var offerCreator = UsersDAL.GetUser(contractOffer.SenderId);
            //if (offerCreator == null)
            //    throw new Exception("Не удалось найти пользователя - отправившего выигравшее предложение по Id пользователя = " + contractOffer.SenderId);

            //var buyer = ad.IsBuy ? adCreator : offerCreator;
            //var seller = ad.IsBuy ? offerCreator : adCreator;

            //buyer.Town = TownsDAL.GetTown(buyer.CityId);
            //if (buyer.Town == null)
            //    throw new Exception("Не удалось найти город покупателя по Id города = " + buyer.CityId);

            //seller.Town = TownsDAL.GetTown(seller.CityId);
            //if (seller.Town == null)
            //    throw new Exception("Не удалось найти город продавца по Id города = " + seller.CityId);

            //if (seller.Id == currentUserId)
            //    buyer.Rating = UsersDAL.GetUserRating(buyer.Id, seller.Id, adId);
            //else
            //    seller.Rating = UsersDAL.GetUserRating(seller.Id, buyer.Id, adId);

            //viewModel.Buyer = buyer;
            //viewModel.Seller = seller;

            ////Заполним строку Условия поставки
            //var deliveryType = ad.DeliveryType != DeliveryTypes.Any ? ad.DeliveryType : contractOffer.DeliveryType;
            //viewModel.DeliveryType = AdHelper.GetDeliveryTypeString(deliveryType);

            ////Заполним строку Погрузка
            //var deliveryLoadType = ad.DeliveryLoadType != DeliveryLoadTypes.Any ? ad.DeliveryLoadType : contractOffer.DeliveryLoadType;
            //viewModel.DeliveryLoadType = AdHelper.GetDeliveryLoadTypeString(deliveryLoadType);

            ////Заполним строку Способ доставки
            //var deliveryWay = ad.DeliveryWay != DeliveryWays.Any ? ad.DeliveryWay : contractOffer.DeliveryWay;
            //viewModel.DeliveryWay = AdHelper.GetDeliveryWayString(deliveryWay);

            ////Заполним строку Цена (с НДС/без НДС)
            //var nds = ad.Nds != Nds.Any ? ad.Nds : contractOffer.Nds;
            //viewModel.Nds = AdHelper.GetNdsString(nds);

            ////Заполним строку Условия оплаты
            //var termOfPayment = ad.TermsOfPayments != TermsOfPayments.Any ? ad.TermsOfPayments : contractOffer.TermsOfPayments;
            //viewModel.TermsOfPayments = AdHelper.GetTermsOfPaymentsString(termOfPayment);
            #endregion

            #region Create PDF
            string filename;
            /// 1: создаем документ
            Document docCard = new Document();
            try
            {
                /// 2: задаем фон и размеры для главной страницы 
                ///  Rectangle rec = new Rectangle(PageSize.A4.Rotate())  // Альбомная == (842, 595);
                Rectangle rec = new Rectangle(PageSize.A4)                // Книжная
                {
                    BackgroundColor = new BaseColor(248, 249, 250)
                };
                docCard.SetPageSize(rec);
                docCard.SetMargins(20, 10, 25, 30);

                /// 3: подключаем русский шрифт
                BaseFont baseFont;

                if (System.IO.File.Exists(GetCurrent().Server.MapPath("~/Files/arial.ttf")))
                {
                    // русский
                    string fontUrl = GetCurrent().Server.MapPath("~/Files/arial.ttf");
                    baseFont = BaseFont.CreateFont(fontUrl, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                }
                else
                    baseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);

                Font arial = new Font(baseFont, Font.DEFAULTSIZE, Font.NORMAL);
                /// 4: Подключаем шрифт для ИТОГО 
                Font arialbold = new Font(baseFont, 12, Font.BOLD);

                /// 5: указываем папку на сервере где будут храниться документы
                /// Прежде проверить наличии папки == если ее нет то создать 

                string path = GetCurrent().Server.MapPath("~/"); ;

                if (Directory.Exists(GetCurrent().Server.MapPath("~/Files")))
                {
                    path = GetCurrent().Server.MapPath("~/Files");
                }

                /// 6: Название файла pdf
                Guid g;
                g = Guid.NewGuid();
                string _g = Convert.ToString(g);
                /*
                string filename = "Контракт_№_" + viewModel.Ad.Id.ToString() + "_-_" + viewModel.DealDirection.ToString() + "_-_" +_g +".pdf";
                */
                filename = _g + ".pdf";
                ViewBag.FilePDF = filename;
                /// 7: создаем документ
                PdfWriter.GetInstance(docCard, new FileStream(path + "/" + filename, FileMode.Create));

                /// 8: открываем дозданный документ
                docCard.Open();

                docCard.Add(new Paragraph("Контракт № " + viewModel.Ad.Id.ToString() + " (" + viewModel.DealDirection.ToString() + ")", arial));
                docCard.Add(new Paragraph(" ", arial));
                docCard.Add(new Paragraph(" ", arial));


                /// 12: создание талицы с данным User...ов
                PdfPTable userTable = new PdfPTable(2);    //2 columns
                userTable.SetWidths(new float[] { 200, 290 }); // 595
                userTable.TotalWidth = docCard.PageSize.Width - docCard.RightMargin - docCard.LeftMargin;
                userTable.LockedWidth = true;
                userTable.DefaultCell.PaddingTop = 5;
                userTable.DefaultCell.PaddingRight = 5;
                userTable.DefaultCell.PaddingBottom = 5;
                userTable.DefaultCell.PaddingLeft = 5;
                userTable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                userTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                /// 13: зададим свойства ячеек таблицы
                PdfPCell usercell = new PdfPCell
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Border = 1,
                    BackgroundColor = new BaseColor(37, 168, 81)
                };

                /// 14: добавляем текст в ячейки таблицы 
                userTable.AddCell(new Phrase("Дата сделки:", arial));
                userTable.AddCell(new Phrase(viewModel.DealDate.ToShortDateString(), arial));

                userTable.AddCell(new Phrase("Покупатель:", arial));
                userTable.AddCell(new Phrase(viewModel.Buyer.CompanyNameWithTypeOfOwnership.ToString() + " " + viewModel.Buyer.Town.NameAndRegionNameWithComma.ToString(), arial));

                userTable.AddCell(new Phrase("Продавец:", arial));
                userTable.AddCell(new Phrase(viewModel.Seller.CompanyNameWithTypeOfOwnership.ToString() + " " + viewModel.Seller.Town.NameAndRegionNameWithComma.ToString(), arial));

                userTable.AddCell(new Phrase("Условия поставки:", arial));
                userTable.AddCell(new Phrase(viewModel.DeliveryType.ToString(), arial));

                userTable.AddCell(new Phrase("Погрузка:", arial));
                userTable.AddCell(new Phrase(viewModel.DeliveryLoadType.ToString(), arial));

                userTable.AddCell(new Phrase("Способ доставки:", arial));
                userTable.AddCell(new Phrase(viewModel.DeliveryWay.ToString(), arial));

                userTable.AddCell(new Phrase("Цена:", arial));
                userTable.AddCell(new Phrase(viewModel.Nds.ToString(), arial));

                userTable.AddCell(new Phrase("Условия оплаты:", arial));
                userTable.AddCell(new Phrase(viewModel.TermsOfPayments.ToString(), arial));

                if (viewModel.deferMentPeriod > 0)
                {
                    userTable.AddCell(new Phrase("Цена действительна до:", arial));
                    userTable.AddCell(new Phrase(viewModel.deferMentPeriod.ToString(), arial));
                }

                docCard.Add(userTable);
                userTable.AddCell(usercell);

                docCard.Add(new Paragraph(" ", arial));
                docCard.Add(new Paragraph(" ", arial));


                /// 15: создание талицы с данным товара Header
                PdfPTable productTableHeader = new PdfPTable(6);    // 6 columns
                productTableHeader.SetWidths(new float[] { 20, 100, 55, 90, 145, 145 }); // 555 == 595
                productTableHeader.TotalWidth = docCard.PageSize.Width - docCard.RightMargin - docCard.LeftMargin;
                productTableHeader.LockedWidth = true;
                productTableHeader.DefaultCell.PaddingTop = 5;
                productTableHeader.DefaultCell.PaddingRight = 5;
                productTableHeader.DefaultCell.PaddingBottom = 5;
                productTableHeader.DefaultCell.PaddingLeft = 5;
                productTableHeader.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                productTableHeader.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                /// 16: зададим свойства ячеек таблицы Header
                PdfPCell productcellHeader = new PdfPCell
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Border = 1,
                    BackgroundColor = new BaseColor(37, 168, 81)
                };

                /// заголовки таблицы                 
                productTableHeader.AddCell(new Phrase("№", arial));
                productTableHeader.AddCell(new Phrase("Категория товара", arial));
                productTableHeader.AddCell(new Phrase("Вес (тн.)", arial));
                productTableHeader.AddCell(new Phrase("Валюта", arial));
                productTableHeader.AddCell(new Phrase("Цена за 1 тн. (предложение)", arial));
                productTableHeader.AddCell(new Phrase("Цена за всю позицию (предложение)", arial));


                /// тело таблицы Body
                PdfPTable productTableBody = new PdfPTable(6);    // 6 columns
                productTableBody.SetWidths(new float[] { 20, 100, 55, 90, 145, 145 }); // 555 == 595
                productTableBody.TotalWidth = docCard.PageSize.Width - docCard.RightMargin - docCard.LeftMargin;
                productTableBody.LockedWidth = true;
                productTableBody.DefaultCell.PaddingTop = 5;
                productTableBody.DefaultCell.PaddingRight = 5;
                productTableBody.DefaultCell.PaddingBottom = 5;
                productTableBody.DefaultCell.PaddingLeft = 5;
                productTableBody.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                /// 16: зададим свойства ячеек таблицы Body
                PdfPCell productcellBody = new PdfPCell
                {
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Border = 1,
                    BackgroundColor = new BaseColor(37, 168, 81)
                };

                int rowNum = 0;
                float sumTotal = 0;

                foreach (var adProduct in viewModel.Ad.Products)
                {
                    rowNum++;
                    var currency = adProduct.Currency == Currencies.Dollar ? "Доллар" : "Рубль";
                    string pricePerWeightOffer = "Не указано";
                    var priceForWholePositionStr = "Не указано";
                    if (adProduct.OfferProduct != null)
                    {
                        pricePerWeightOffer = adProduct.OfferProduct.PricePerWeight.ToString();
                        var priceForWholePosition = adProduct.Weight * adProduct.OfferProduct.PricePerWeight;
                        priceForWholePositionStr = priceForWholePosition.ToString();
                        sumTotal += priceForWholePosition;
                    }
                    productTableBody.AddCell(new Phrase(rowNum.ToString(), arial));
                    productTableBody.AddCell(new Phrase(adProduct.ProductCategoryName.ToString(), arial));
                    productTableBody.AddCell(new Phrase(adProduct.Weight.ToString(), arial));
                    productTableBody.AddCell(new Phrase(currency.ToString(), arial));
                    productTableBody.AddCell(new Phrase(pricePerWeightOffer.ToString(), arial));
                    productTableBody.AddCell(new Phrase(priceForWholePositionStr.ToString(), arial));

                }

                docCard.Add(productTableHeader);
                productTableHeader.AddCell(productcellHeader);

                docCard.Add(productTableBody);
                productTableBody.AddCell(productcellBody);

                /// Footer table Product
                PdfPTable productTableFooter = new PdfPTable(3);    // 7 columns
                productTableFooter.SetWidths(new float[] { 20, 390, 145 }); // 555 == 595
                productTableFooter.TotalWidth = docCard.PageSize.Width - docCard.RightMargin - docCard.LeftMargin;
                productTableFooter.LockedWidth = true;
                productTableFooter.DefaultCell.PaddingTop = 5;
                productTableFooter.DefaultCell.PaddingRight = 5;
                productTableFooter.DefaultCell.PaddingBottom = 5;
                productTableFooter.DefaultCell.PaddingLeft = 5;
                productTableFooter.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                productTableFooter.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                /// 16: зададим свойства ячеек таблицы Footer
                PdfPCell productcellFooter = new PdfPCell
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Border = 0,
                    BackgroundColor = new BaseColor(37, 168, 81)
                };

                /// заголовки таблицы Footer               
                productTableFooter.AddCell(new Phrase(" ", arialbold));
                productTableFooter.AddCell(new Phrase("Итого:", arialbold));
                productTableFooter.AddCell(new Phrase(sumTotal.ToString(), arialbold));

                docCard.Add(productTableFooter);
                productTableFooter.AddCell(productcellFooter);

                if (!string.IsNullOrWhiteSpace(viewModel.Ad.Description))
                {
                    /// 17: создание Описание
                    docCard.Add(new Paragraph(" ", arial));
                    docCard.Add(new Paragraph(" ", arial));
                    docCard.Add(new Paragraph("Описание:", arial));
                    docCard.Add(new Paragraph(""));

                    string description = !string.IsNullOrWhiteSpace(viewModel.Ad.Description) ? viewModel.Ad.Description : "Не указано";
                    docCard.Add(new Paragraph(" ", arial));
                    docCard.Add(new Paragraph(description, arial));
                }

                //docCard.Add(new Paragraph("M-Contract", arial));
                //docCard.Add(new Paragraph("Подтверждено", arial));

                /// 18. добавляем ссылку на сайт
                /*
                Anchor anchor = new Anchor("M-contract.ru", new Font(
                    BaseFont.CreateFont(fontUrl,
                    BaseFont.IDENTITY_H,
                    BaseFont.NOT_EMBEDDED),
                    10, Font.UNDERLINE,
                    BaseColor.BLUE));
                anchor.Reference = "http://m-contract.ru/";
                docCard.Add(new Paragraph(anchor));
                */
                docCard.Add(new Paragraph(" ", arial));
                /// 10: добавляем картинку и расположим ее внизу вправо 
                /*
                ALIGN_LEFT = 0
                ALIGN_RIGHT = 2
                TEXTWRAP = 4
                Image d = Image.getInstance(DOG);
                d.setScaleToFitHeight(false);
                Image f = Image.getInstance(FOX);
                f.setScaleToFitHeight(false);
                Chunk dog = new Chunk(d, 0, 0, false);
                Chunk fox = new Chunk(f, 0, 0, false);
                PdfContentByte canvas = writer.getDirectContent();
                ColumnText.showTextAligned(canvas, Element.ALIGN_LEFT, new Phrase(dog), 250, 750f, 0);
                ColumnText.showTextAligned(canvas, Element.ALIGN_RIGHT, new Phrase(fox), 250, 750f, 0);

                */
                if (System.IO.File.Exists(GetCurrent().Server.MapPath("~/Files/counter-accepted.png")))
                {
                    ImageiTextSharp imageAccepted = ImageiTextSharp.GetInstance(GetCurrent().Server.MapPath("~/Files/counter-accepted.png"));
                    imageAccepted.ScalePercent(75);



                    /// Accepted table Accepted
                    PdfPTable acceptedTable = new PdfPTable(2);    // 2 columns
                    acceptedTable.SetWidths(new float[] { 390, 165 }); // 555 == 595
                    acceptedTable.TotalWidth = docCard.PageSize.Width - docCard.RightMargin - docCard.LeftMargin;
                    acceptedTable.LockedWidth = true;
                    acceptedTable.DefaultCell.PaddingTop = 0;
                    acceptedTable.DefaultCell.PaddingRight = 25;
                    acceptedTable.DefaultCell.PaddingBottom = 0;
                    acceptedTable.DefaultCell.PaddingLeft = 0;
                    acceptedTable.DefaultCell.Border = 0;
                    acceptedTable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    acceptedTable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                    /// 16: зададим свойства ячеек таблицы acceptedTable
                    PdfPCell acceptedcell = new PdfPCell
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Border = 0,
                        BackgroundColor = new BaseColor(37, 168, 81)
                    };

                    PdfPCell imageCell = new PdfPCell(imageAccepted)
                    {
                        Colspan = 2, // either 1 if you need to insert one cell == либо 1, если вам нужно вставить одну ячейку
                        Border = 0,
                        PaddingRight = 30,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };

                    /// строки таблицы acceptedTable               
                    acceptedTable.AddCell(imageCell);

                    docCard.Add(acceptedTable);
                    acceptedTable.AddCell(acceptedcell);

                }
            }
            catch (DocumentException de)
            {
                Console.WriteLine(de.ToString());
            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe.ToString());
            }
            docCard.Close();
            #endregion
            return View(viewModel);


        }
        HttpContext GetCurrent()
        {
            return System.Web.HttpContext.Current;
        }
        #endregion

        #region Отправка файла (PDF) с данными о выбранной сделке на емайл 

        [MyAuthorize]
        public string SendDealsHistory(string file, string emailUser, string emailUserNew, string history)
        {

            if (ValidHelper.emailRus(emailUser) && ValidHelper.emailRus(emailUserNew))

                try
                {
                    string Subject = "История сделки - " + history;
                    string Body = "Уважаемый пользователь портала M-contract" + "<br />" +
                        "Во вложенном файле история сделки - " + history + "<br />"
                        + "<i>" + "С уважением команда портала M-contract" + "</i>";

                    string dirName = "Files";

                    MailHelper.MailSendAttachment("info@m-contract.ru", emailUser, emailUserNew, Subject, Body, dirName, file);

                    return "История сделки - " + history + " отправлена на e-mail: " + emailUser + ", " + emailUserNew + ", с вложенным файлом -  " + file;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return ex.ToString() + "почта не может быть доставлена на фальшивые e-mail адреса";
                }
            else if (ValidHelper.emailRus(emailUser))

                try
                {
                    string Subject = "История сделки - " + history;
                    string Body = "Уважаемый пользователь портала M-contract" + "<br />" +
                        "Во вложенном файле история сделки - " + history + "<br />"
                        + "<i>" + "С уважением команда портала M-contract" + "</i>";

                    string dirName = "Files";

                    MailHelper.MailSendAttachment("info@m-contract.ru", emailUser, emailUserNew, Subject, Body, dirName, file);
                    return "История сделки - " + history + " отправлена на e-mail: " + emailUser + ", с вложенным файлом -  " + file;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return ex.ToString() + "почта не может быть доставлена на фальшивый e-mail адрес";
                }
            #endregion
            return "почта не может быть доставлена на фальшивые e-mail адреса";

        }

        #region
        public ActionResult RateRules()
        {
            return View();
        }

        [MyAuthorize]
        //public ActionResult Messages(int respondentId, bool isFromNewOffer = false, int adId = 0)
        public ActionResult Messages(int respondentId = 0, string lastPageUrl = "")
        {
            if (respondentId == 0)
                return RedirectToAction("Dialogs");
            ViewBag.L.HideHead = true;
            var currentUser = SM.CurrentUser;
            var viewModel = new UserMessagesViewModel
            {
                LastPageUrl = lastPageUrl,
                LeftMenuViewModel = LeftMenuViewModel.Get(currentUser, "Сообщения")
            };

            var respondent = UsersDAL.GetUser(respondentId) ?? new User();
            respondent.SmallPhotoUrl = UserHelper.GetSmallPhotoUrl(respondentId);
            currentUser.SmallPhotoUrl = UserHelper.GetSmallPhotoUrl(currentUser.Id);

            respondent.Town = TownsDAL.GetTown(respondent.CityId);

            var messages = MessagesDAL.GetMessages(respondentId, currentUser.Id);

            var deletedMessageIds = DeletedMessagesDAL.GetDeletedMessages(currentUser.Id, respondentId).Select(m => m.MessageId).ToList();

            messages = messages.Where(m => !deletedMessageIds.Contains(m.Id)).ToList();

            if (messages.Any())
            {
                var messageIds = messages.Select(m => m.Id).ToList();
                var files = FilesDAL.GetFiles(messageIds: messageIds);
                foreach (var message in messages)
                {
                    if (message.SenderId == currentUser.Id)
                    {
                        message.Sender = currentUser;
                        message.Direction = "outgoing";
                    }
                    else
                    {
                        message.Sender = respondent;
                        message.Direction = "incoming";
                    }

                    message.Files = files.Where(f => f.MessageId == message.Id).ToList();
                }

                MessagesDAL.MarkMessagesAsRead(currentUser.Id, respondent.Id);
            }

            viewModel.CurrentUser = currentUser;
            viewModel.Respondent = respondent;
            viewModel.Messages = messages;

            return View(viewModel);
        }

        [MyAuthorize]
        //public ActionResult Messages(int respondentId, bool isFromNewOffer = false, int adId = 0)
        public ActionResult MessagesOld(int? respondentId, string lastPageUrl = "")
        {
            ViewBag.L.ShowSearchbar = false;
            if (respondentId == null || respondentId == 0)
                return RedirectToAction("Dialogs");
            var dialog = new Dialog { SenderId = respondentId.Value, RecipientId = SM.CurrentUserId };
            dialog.LastPageUrl = lastPageUrl;
            /*dialog.IsFromNewOffer = isFromNewOffer;
            if (isFromNewOffer == true)
                dialog.TopicId = adId;*/
            dialog.PersonalAreaUser = SM.GetPersonalAreaUser();
            dialog.PersonalAreaUser.SelectedMenu = "Сообщения";
            dialog.Sender = UsersDAL.GetUser(dialog.SenderId);
            if (dialog.Sender != null)
            {
                dialog.Sender.LogoGroup = PhotosDAL.GetCompanyLogoGroup(dialog.Sender.Id);
                dialog.Sender.SmallPhotoUrl = dialog.Sender.GetBestFitLogoPhoto(1).Url;
            }
            dialog.Recipient = dialog.PersonalAreaUser;
            if (dialog.SenderId == dialog.PersonalAreaUser.Id)
                dialog.Respondent = dialog.Recipient;
            else
                dialog.Respondent = dialog.Sender;
            dialog.Messages = MessagesDAL.GetMessages(respondentId.Value, SM.CurrentUserId);
            if (dialog.Messages.Any())
            {
                var messageIds = dialog.Messages.Select(m => m.Id).ToList();
                var files = FilesDAL.GetFiles(messageIds: messageIds);
                foreach (var message in dialog.Messages)
                {
                    if (message.SenderId == dialog.Sender?.Id)
                        message.Sender = dialog.Sender;
                    else
                        message.Sender = dialog.Recipient;

                    var logoGroup = message.Sender.LogoGroup;
                    message.SenderLogoUrl = logoGroup != null && logoGroup.Any() ? logoGroup[0].Url : PhotoHelper.NoLogoImageUrl;

                    if (message.SenderId == dialog.Respondent?.Id)
                        message.Direction = "incoming";
                    else
                        message.Direction = "outgoing";

                    message.Files = files.Where(f => f.MessageId == message.Id).ToList();
                }

                var lastMessage = dialog.Messages.Last();

                if (lastMessage.Direction == "incoming" && !lastMessage.IsRead)
                    MessagesDAL.MarkMessagesAsRead(dialog.RecipientId, dialog.SenderId);
            }
            if (dialog.DialogType == DialogTypes.Ad)
            {
                dialog.Ad = AdsDAL.GetAd(dialog.TopicId);
                if (dialog.Ad != null)
                    dialog.Ad.City = TownsDAL.GetTown(dialog.Ad.CityId);
            }
            ViewBag.Heading = (dialog.Respondent?.IsSystemNotifications == true ? dialog.Respondent?.CompanyName : "Диалог с " + dialog.Respondent?.CompanyNameWithTypeOfOwnership) +
                (dialog.DialogType == DialogTypes.Ad && dialog.Ad != null ? ", " + dialog.Ad.City?.Name + ", " + dialog.Ad.Name.ToLower() : "");
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>
            {
                new BreadCrumbLink() { Text = "Диалоги", Url = C.SiteUrl + "User/Dialogs", Title = "Перейти к списку диалогов" },
                new BreadCrumbLink() { Text = (dialog.Respondent?.IsSystemNotifications == true ? dialog.Respondent?.CompanyName : "Диалог с " + dialog.Respondent?.CompanyNameWithTypeOfOwnership), EndPoint = true }
            };
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(dialog);
        }

        [MyAuthorize]
        public ActionResult Dialogs()
        {
            ViewBag.L.HideHead = true;
            var viewModel = new UserDialogsViewModel();
            var currentUser = SM.CurrentUser;
            var currentUserId = currentUser.Id;
            var leftMenuViewModel = LeftMenuViewModel.Get(currentUser, "Сообщения");
            viewModel.LeftMenuViewModel = leftMenuViewModel;
            viewModel.MobileMenuViewModel = MobileMenuViewModel.Get(leftMenuViewModel);

            //все сообщения, отправленные или полученные текущим пользователем
            var allMessages = MessagesDAL.GetMessages(currentUserId);

            //Отфильтруем сообщения с учетом нажатий кнопок "Удалить чат", "Удалить все чаты"
            var filteredMessages = UserHelper.FilterMessagesFromDialogInfo(currentUserId, allMessages);

            viewModel.Dialogs = filteredMessages.GroupBy(m => m.SenderId).Where(g => g.Key != currentUserId)
                .Select(g => new Dialog { SenderId = g.Key, RecipientId = currentUserId, Messages = g.ToList() }).ToList();
            viewModel.AllUsers = UsersDAL.GetUsers().Where(u => u.Id != currentUserId).ToList();
            var towns = TownsDAL.GetTowns();
            foreach (var user in viewModel.AllUsers)
            {
                user.Town = towns.FirstOrDefault(t => t.Id == user.CityId);
            }
            if (viewModel.Dialogs.Any())
            {
                foreach (var dialog in viewModel.Dialogs)
                {
                    if (!dialog.Messages.Any())
                        continue;
                    if (dialog.SenderId == currentUserId)
                        dialog.Respondent = viewModel.AllUsers.FirstOrDefault(u => u.Id == dialog.RecipientId);
                    else
                        dialog.Respondent = viewModel.AllUsers.FirstOrDefault(u => u.Id == dialog.SenderId);
                    if (dialog.Respondent != null)
                    {
                        dialog.Respondent.LogoGroup = PhotosDAL.GetCompanyLogoGroup(dialog.Respondent.Id);
                        dialog.Respondent.SmallPhotoUrl = dialog.Respondent.GetBestFitLogoPhoto(1).Url;
                        dialog.Respondent.Town = TownsDAL.GetTown(dialog.Respondent.CityId);
                    }
                    if (dialog.DialogType == DialogTypes.Ad)
                    {
                        dialog.Ad = AdsDAL.GetAd(dialog.TopicId);
                    }
                    dialog.AllMessagesText = string.Join(" ", dialog.Messages.Select(m => m.Text));
                    dialog.NewMessagesCount = dialog.Messages.Where(m => m.RecipientId == currentUserId && !m.IsRead).ToList().Count;
                }
                viewModel.Dialogs = viewModel.Dialogs.Where(d => d.Messages.Any()).OrderByDescending(d => d.Messages.LastOrDefault().Date).ToList();
            }
            ViewBag.Heading = "Диалоги";
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>
            {
                new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true }
            };
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(viewModel);
        }

        [HttpPost]
        public string GetNewMessagesForChatBox(int lastMessageId = 0, int respondentId = 0)
        {
            var currentUserId = SM.CurrentUserId;
            UsersDAL.UpdateUserLastOnline(currentUserId, DateTime.Now.ToUniversalTime());

            var messages = MessagesDAL.GetNewMessagesForChatBox(currentUserId, lastMessageId, respondentId);
            if (!messages.Any())
                return "";

            var chatBoxRemovedDialogIds = SM.ChatBoxRemovedDialogIds;


            var user = SM.GetPersonalAreaUser();
            if (messages.Any(m => m.SenderId != respondentId && m.SenderId != currentUserId))
            {
                user.UnreadMessagesCount -= messages.Where(m => m.SenderId != respondentId).ToList().Count;
                SM.UpdatePersonalAreaUser(user);
            }

            if (messages.Any(m => m.SenderId == respondentId))
            {
                MessagesDAL.MarkMessagesAsRead(currentUserId, respondentId);
            }

            var messageIds = messages.Select(m => m.Id).ToList();
            var messageFiles = FilesDAL.GetFiles(messageIds: messageIds);
            if (messageFiles.Any())
                messages.ForEach(m => m.Files = messageFiles.Where(f => f.MessageId == m.Id).ToList());

            var dialogs = SM.CurrentDialogs;
            if (dialogs == null || !dialogs.Any())
            {
                dialogs = new List<Dialog>();
            }
            Dialog dialog = null;
            foreach (var messageGroup in messages.GroupBy(m => m.SenderId != currentUserId ? m.SenderId : m.RecipientId).OrderBy(g => g.Last().Date).ToList())
            {
                var lastMessageInGroup = messageGroup.Last();
                var dialogRespondentId = lastMessageInGroup.SenderId != currentUserId ? lastMessageInGroup.SenderId : lastMessageInGroup.RecipientId;

                if (chatBoxRemovedDialogIds.Contains(dialogRespondentId))
                    continue;

                dialog = dialogs.FirstOrDefault(d => d.Respondent?.Id == dialogRespondentId);
                if (dialog == null)
                {
                    // если диалог не найден в списке открытых, создаем новый
                    dialog = new Dialog
                    {
                        // практически никогда не будет выполняться больше одного раза в цикле
                        Messages = MessagesDAL.GetMessages(currentUserId, dialogRespondentId)
                    };
                    var respondent = UsersDAL.GetUser(dialogRespondentId);
                    if (respondent == null)
                        continue;
                    respondent.LogoGroup = PhotosDAL.GetCompanyLogoGroup(dialogRespondentId);
                    dialog.Respondent = new User
                    {
                        Id = respondent.Id,
                        CompanyName = respondent.CompanyName,
                        TypeOfOwnership = respondent.TypeOfOwnership,
                        LogoGroup = respondent.LogoGroup
                    };
                    // если количество диалогов превышает лимит, выбросить самые старые
                    if (dialogs.Count >= C.ChatBoxMaxDialogs)
                    {
                        dialogs = dialogs.Skip(dialogs.Count - C.ChatBoxMaxDialogs + 1).ToList();
                    }

                    dialogs.Add(dialog);
                }
                else
                {
                    // если найден, добавляем новые сообщения
                    var lastMessageInDialog = dialog.Messages.LastOrDefault(m => m.SenderId != currentUserId);
                    if (lastMessageInDialog != null && lastMessageInGroup.Id > lastMessageInDialog.Id)
                    {
                        dialog.Messages.AddRange(messageGroup.Where(m => m.Id > lastMessageInDialog.Id));
                    }
                }

                foreach (var message in messageGroup)
                {
                    var sender = message.SenderId == dialogRespondentId ? dialog.Respondent : user;

                    message.Sender = sender;
                    message.SenderLogoUrl = sender.LogoGroup.Any() ? sender.LogoGroup[0].Url : PhotoHelper.NoLogoImageUrl;
                }

            }

            var openDialog = dialogs.FirstOrDefault(d => d.Respondent?.Id == user.CurrentRespondentId);
            if (openDialog != null && openDialog.Messages != null && openDialog.Messages.Any())
            {
                var unreadIncomingMessages = openDialog.Messages.Where(m => m.SenderId != currentUserId && m.IsRead == false).ToList();
                if (unreadIncomingMessages.Any())
                    unreadIncomingMessages.ForEach(m => m.IsRead = true);
                MessagesDAL.MarkMessagesAsRead(currentUserId, user.CurrentRespondentId);
            }

            SM.CurrentDialogs = dialogs;

            return Newtonsoft.Json.JsonConvert.SerializeObject(messages);
        }

        [HttpPost]
        public string GetDialogForChatBox(int respondentId)
        {
            var personalAreaUser = SM.GetPersonalAreaUser();
            var currentUserId = personalAreaUser.Id;
            personalAreaUser.CurrentRespondentId = respondentId;
            UsersDAL.UpdateUserChangeCurrentRespondentId(currentUserId, respondentId);
            var respondentIds = UsersDAL.GetOpenDialogRespondentIds(currentUserId);
            if (!respondentIds.Contains(respondentId))
            {
                respondentIds.Add(respondentId);
                UsersDAL.UpdateOpenDialogRespondentIds(currentUserId, respondentIds);
            }
            MessagesDAL.MarkMessagesAsRead(currentUserId, respondentId);
            var dialogs = SM.CurrentDialogs;
            var dialog = dialogs.FirstOrDefault(d => d.Respondent?.Id == respondentId);
            if (dialog == null)
            {
                var messages = MessagesDAL.GetMessages(respondentId, currentUserId);
                dialog = new Dialog { Messages = messages };
                var messageIds = messages.Select(m => m.Id).ToList();
                var messageFiles = FilesDAL.GetFiles(messageIds: messageIds);
                if (messageFiles.Any())
                    messages.ForEach(m => m.Files = messageFiles.Where(f => f.MessageId == m.Id).ToList());
            }
            else
            {
                var lastMessageId = dialog.Messages.LastOrDefault() != null ? dialog.Messages.Last().Id : 0;
                var messages = MessagesDAL.GetNewMessagesForChatBox(SM.CurrentUserId, lastMessageId, respondentId)
                    .Where(m => m.SenderId == respondentId).ToList();
                if (messages.Any())
                {
                    var messageIds = messages.Select(m => m.Id).ToList();
                    var messageFiles = FilesDAL.GetFiles(messageIds: messageIds);
                    if (messageFiles.Any())
                        messages.ForEach(m => m.Files = messageFiles.Where(f => f.MessageId == m.Id).ToList());
                }
                if (dialog.Messages != null && dialog.Messages.Any())
                    dialog.Messages.AddRange(messages);
                else
                    dialog.Messages = messages;
            }

            if (dialog.Messages != null && dialog.Messages.Any())
                dialog.Messages.Where(m => m.SenderId != SM.CurrentUserId && m.IsRead == false).ForEach(m => m.IsRead = true);

            if (dialog.Respondent == null)
            {
                var respondent = UsersDAL.GetUser(respondentId);

                if (respondent == null)
                    return "";

                respondent.LogoGroup = PhotosDAL.GetCompanyLogoGroup(respondent.Id);
                dialog.Respondent = new User
                {
                    Id = respondent.Id,
                    CompanyName = respondent.CompanyName,
                    TypeOfOwnership = respondent.TypeOfOwnership,
                    LogoGroup = respondent.LogoGroup
                };
            }

            if (dialogs.FirstOrDefault(d => d.Respondent?.Id == respondentId) == null)
                dialogs.Add(dialog);
            SM.CurrentDialogs = dialogs;

            var chatBoxRemovedDialogIds = SM.ChatBoxRemovedDialogIds;
            chatBoxRemovedDialogIds.Remove(respondentId);
            SM.ChatBoxRemovedDialogIds = chatBoxRemovedDialogIds;

            return Newtonsoft.Json.JsonConvert.SerializeObject(dialog,
                new Newtonsoft.Json.JsonSerializerSettings { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });
        }

        [HttpPost]
        public bool CloseDialogInChatBox()
        {
            var personalAreaUser = SM.GetPersonalAreaUser();
            personalAreaUser.CurrentRespondentId = 0;
            SM.UpdatePersonalAreaUser(personalAreaUser);
            return UsersDAL.UpdateUserChangeCurrentRespondentId(SM.CurrentUserId, 0);
        }

        [HttpPost]
        public bool RemoveDialogFromChatBox(int respondentId)
        {
            var openDialogRespondentIds = UsersDAL.GetOpenDialogRespondentIds(SM.CurrentUserId);
            var dialogs = SM.CurrentDialogs;
            if (openDialogRespondentIds.Contains(respondentId) || dialogs.Any(d => d.Respondent != null && d.Respondent.Id == respondentId))
            {
                openDialogRespondentIds.Remove(respondentId);
                UsersDAL.UpdateOpenDialogRespondentIds(SM.CurrentUserId, openDialogRespondentIds);
                dialogs = dialogs.Where(d => d.Respondent?.Id != respondentId).ToList();
                SM.CurrentDialogs = dialogs;
                var chatBoxRemovedDialogIds = SM.ChatBoxRemovedDialogIds;
                chatBoxRemovedDialogIds.Add(respondentId);
                SM.ChatBoxRemovedDialogIds = chatBoxRemovedDialogIds;
            }
            else
                return false;
            return true;
        }

        [HttpPost]
        public bool ToggleChatBoxIsSilent(bool chatBoxIsSilent)
        {
            var user = SM.GetPersonalAreaUser();
            user.ChatBoxIsSilent = chatBoxIsSilent;
            SM.UpdatePersonalAreaUser(user);
            return true;
        }

        public int GetUnreadMessagesCount()
        {
            return MessagesDAL.GetUnreadMessages(SM.CurrentUserId).Count;
        }

        [HttpPost]
        public bool AddMessage(Message message)
        {
            var result = UserHelper.AddMessage(message) > 0;
            if (result)
                SM.CurrentDialogs = null;

            return result;
        }

        [HttpPost]
        public string UploadPhoto(int userId)
        {
            return PhotoHelper.UploadPhoto(Request, userId);
        }

        [HttpPost]
        public bool EditProfile(User updatedUser)
        {
            var user = UsersDAL.GetUser(updatedUser.Id);
            user.CityId = updatedUser.CityId;
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.PhoneNumberCity = updatedUser.PhoneNumberCity;
            user.Address = updatedUser.Address;
            user.FactualAddress = updatedUser.FactualAddress;
            UsersDAL.UpdateUser(user);
            return true;
        }

        [HttpPost]
        public bool ChangeRegularClient(int userId, int clientId, bool isDelete = false)
        {
            if (isDelete == false)
            {
                if (UsersDAL.AddRegularClient(userId, clientId) != 0)
                    return true;
                else
                    return false;
            }
            else
            {
                if (UsersDAL.DeleteRegularClient(userId, clientId))
                    return true;
                else
                    return false;
            }
        }

        [HttpPost]
        public bool RemoveDealFromDealsHistory(int adId, int offerId)
        {
            if (adId == 0 || offerId == 0)
                return false;

            var ad = AdsDAL.GetAd(adId);
            var offer = OffersDAL.GetOffer(offerId);
            if (ad.SenderId == SM.CurrentUserId)
                return AdsDAL.UpdateAdDoNotShowInDealsHistory(adId);
            else if (offer.SenderId == SM.CurrentUserId)
                return OffersDAL.UpdateOfferDoNotShowInDealsHistory(offerId);
            else
                return false;
        }

        /// <summary>
        /// Добавляет оценки контрагенту за сделку по объявлению
        /// </summary>
        /// <param name="userId">Id-пользователя, которому выставляем оценку</param>
        /// <param name="rating">Оценка</param>
        /// <param name="adId">Id объявления</param>
        /// <returns></returns>
        [HttpPost]
        public bool AddUserRating(int userId, int rating, int adId)
        {
            return UsersDAL.AddUserRating(userId, SM.CurrentUserId, rating, adId) > 0;
        }

        public static void CheckUsersInSbis()
        {
            Thread.Sleep(11 * 60 * 1000);//11 минут
            while (true)
            {
                try
                {
                    var usersToCheck = UsersDAL.GetUsers(checkedInSbis: false);
                    foreach (var user in usersToCheck)
                    {
                        if (!String.IsNullOrWhiteSpace(user.INN))
                        {
                            var sbisUser = P.GetSbisInformationFromInn(user.INN);
                        }
                    }
                }
                catch (Exception)
                {

                }
                Thread.Sleep(10 * 60 * 1000);//10 минут
            }
        }

        [HttpPost]
        public string MarkAllMessagesAsRead()
        {
            var userId = SM.CurrentUserId;
            if (MessagesDAL.MarkMessagesAsRead(userId))
                return "Все сообщения отмечены как прочитанные.";
            else
                return "Ошибка. Сообщения не были отмечены как прочитанные.";
        }

        [HttpPost]
        public string DeleteAllChats(string respondentIdsStr)
        {
            if (String.IsNullOrWhiteSpace(respondentIdsStr))
                return "Передан пустой список respondentIds";

            var respondentIdsStrParts = respondentIdsStr.Split(',');
            //var respondentIds = new List<int>();
            foreach (var respondentIdStr in respondentIdsStrParts)
            {
                var respondentId = Convert.ToInt32(respondentIdStr);
                var result = DeleteChat(respondentId);
                if (result != "ok")
                    return result;
            }

            return "ok";
        }

        [HttpPost]
        public string DeleteChat(int respondentId)
        {
            if (respondentId == 0)
                return "Необходимо передать respondentId";

            var currentUserId = SM.CurrentUserId;
            if (currentUserId == 0)
                return "Необходимо авторизоваться";

            var messages = MessagesDAL.GetMessages(currentUserId, respondentId);
            if (!messages.Any())
                return "Не найдены сообщения в чате, нечего удалять";

            var lastMessageId = messages.Last().Id;

            //пометим в "Удаляемом чате" входящие непрочитанные сообщения прочитанными
            var unreadMessageIds = messages.Where(m => m.RecipientId == currentUserId && !m.IsRead).Select(m => m.Id).ToList();
            if (unreadMessageIds.Any())
                MessagesDAL.UpdateMessageSetIsReadTrue(unreadMessageIds);

            var dialogInfo = DialogInfosDAL.GetDialogInfo(currentUserId, respondentId);
            if (dialogInfo != null)
            {
                dialogInfo.ShowMessagesFromId = lastMessageId + 1;
                var result = DialogInfosDAL.UpdateDialogInfo(dialogInfo);
                if (!result)
                    return "Не удалось удалить чат, попробуйте позже";
            }
            else
            {
                var newDialogInfo = new DialogInfo()
                {
                    UserId = currentUserId,
                    RespondentId = respondentId,
                    ShowMessagesFromId = lastMessageId + 1
                };
                DialogInfosDAL.AddDialogInfo(newDialogInfo);
            }

            return "ok";
        }

        [MyAuthorize]
        [HttpPost]
        public string DeleteMessage(int messageId)
        {
            if (messageId == 0)
                return "Необходимо передать id сообщения";

            var message = MessagesDAL.GetMessage(messageId);
            if (message == null)
                return "Не удалось найти сообщение по Id = " + messageId;

            var currentUserId = SM.CurrentUserId;

            var deletedMessage = new DeletedMessage()
            {
                UserId = currentUserId,
                DialogWithUserId = message.SenderId == currentUserId ? message.RecipientId : message.SenderId,
                MessageId = messageId
            };

            var result = DeletedMessagesDAL.AddDeletedMessage(deletedMessage);
            if (result > 0)
                return "ok";
            else
                return "Не удалось удалить сообщение, попробуйте позже";
        }
        #endregion
    }
}