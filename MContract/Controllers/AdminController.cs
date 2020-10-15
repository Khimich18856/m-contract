using MContract.AppCode;
using MContract.DAL;
using MContract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MContract.Controllers
{
    public class AdminController : Controller
    {
        #region Список запрещенных слов
        public static List<string> Profanities = new List<string> { "блядь", "гандон", "говно", "дрочить", "дрочить", "ебать", "ебаться", "жопа", "елда", "елдак", "хуй", "залупа", "залупа", "малофья", "молофья", "мудак", "мудями", "муди", "муде", "пизда", "серить", "срать", "срать", "ссать", "манда", "пизда", "хер", "хуй", "хер", "хуй", "целка", "блядь", "гандон", "говно", "дрочить", "ебать", "ебаться", "жопа", "залупа", "мудак", "пизда", "срать", "ссать", "хер", "хуй", "хуй", "целка", "бля", "блядь", "блядло", "блядина", "блядища", "блядище", "блядь", "блядки", "блядовать", "блядун", "выблядок", "блядоход", "блядки", "блядство", "блядский", "говённый", "говняный", "говнёвый", "говнюк", "говноед", "задроченный", "придрочиться", "ебальник", "ебало", "ебло", "ёбарь", "ебать", "ёбнуть", "ебануть", "ёбнуться", "ебануться", "наебнуться", "ёбнутый", "ебанутый", "выёбываться", "долбоёб", "въёбывать", "заебись", "заебать", "настоебать", "доебать", "ебаквакнуться", "наебать", "объебать", "заёбанный", "задроченный", "ебысь", "еблысь", "ёбс", "наёбка", "подъебать", "подъёб", "подъёбка", "поебать", "поебень", "поеботина", "коноебля", "коноёбиться", "ебля", "коноебля", "уёбище", "бомбоуёбище", "уёбывать", "съёбывать", "уёбывать", "изъебнуться", "изъёб", "невъебенно", "заебатый", "заебательский", "разъебай", "разъёба", "поёбка", "ебля", "еблан", "ебанат", "туебень", "ёбово", "еботятина", "ебливый", "ебучий", "злоебучий", "косоёбиться", "перекосоёбиться", "шароёбиться", "проебать", "просрать", "жопак", "жопорванец", "зажопить", "жополиз", "хитрожопый", "залупаться", "выёбываться", "залупатый", "заебатый", "залупить", "мудила", "мудак", "мудозвон", "мудила", "мудохать", "мудофель", "мудель", "мудень", "мудила", "мудильник", "мандовуха", "мандовка", "прошмандовка", "блядь", "мандавошка", "пиздец", "пиздец", "пиздатый", "заебатый", "пиздобол", "опиздол", "опездол", "пиздить", "спиздить", "пиздануть", "пиздить", "пиздеть", "пиздёж", "пиздун", "пиздяной", "пиздной", "говённый", "пиздюля", "пиздюлина", "пиздюшка", "пиздюлина", "распиздяй", "пиздовать", "пиздануться", "пиздюк", "пездолочь", "спиздометр", "пизданутый", "припизднутый", "пиздячить", "пиздюхать", "пиздовать", "пиздёхать", "пиздорванец", "пиздобратия", "припиздь", "пиздотекарь", "пиздёныш", "чепиздон", "распиздон", "срака", "жопа", "сральник", "жопа", "засранец", "срун", "обосраться", "пересрать", "обосраться", "просрать", "насрать", "засрать", "обосрать", "срач", "усираться", "подосрать", "высрать", "обоссаться", "обосраться", "проссывать", "ссыкун ", "ссыкло", "срун", "уссываться", "уссачка", "уссывон", "похерить", "хуйня", "поебень", "хуета", "хуетень", "хуёвина", "хуётина", "хуйня", "хули", "хуй ли", "хуячить", "хуярить", "хуярыжить", "пиздячить", "хуёвый", "охуенный", "охуительный", "охуевательный", "охуенный", "охуеть", "ни хуя", "не хуя", "не хуй", "по хую", "по хуй", "хуяк", "ебысь", "похуист", "похуизм", "на хуй", "блядь", "хуила", "хуебяка", "хуйнуть", "семихуй", "хуемполбия", "хуесос", "бля буду", "говно поднять", "ёб (еби", "ебить) твою мать", "блядь", "ёбаный в рот", "ёб твою мать", "ёбть", "ёбтыть", "ебёнть", "ёб твою мать", "ебёна мать", "ёбаный кракатук", "ёбаный стос", "ёбаный карась", "ёбаный экстаз", "ёбаный по голове", "я ебу", "я не ебался", "ебаться-сраться", "ебать-колотить", "ебать-копать", "ебать ту люсю", "ебать мои старые кости", "ебать мой лысый череп", "ебать мой хуй ", "ебитская сила", "ёбнутым привет", "в рот ебать", "ёбом токнутый", "конь не ебался", "муха не еблась", "ебальником (еблом", "ебалом) щёлкать", "ебись всё в рот", "ебись оно всё конём", "мозги ебать", "мозгоёб", "заебать в доску", "поебень-хуета", "поебень", "муму ебать", "вола ебать", "ёбу даться", "до ебени матери", "до ебени бабушки", "до ебени ебуков", "до ебени ебеней", "до ебени фени", "до ебени матери", "к ебени матери", "ебуки пускать", "разъеби-колотун", "да заебись ты по нотам", "нас ебут", "а мы крепчаем", "не учи отца ебаться", "поебень-трава", "муда-с-пруда", "мудовые рыдания", "муде-колёса", "мудоёб", "мудила", "из-под муда", "хозяйство вести - не мудями трясти", "уплыли муде по полой воде", "муде к бороде", "в жопака", "до жопы", "жопкин хор", "жопой чуять", "через жопу", "через жопу накосяк", "через жопу раком", "рвать жопу", "зажопкины выселки", "жопу лизать", "в жопе торчать", "хоть жопой ешь", "в жопу чистить провода", "и в рот", "и в жопу", "как из жопы достатый", "засунуть в жопу", "сравнить жопу с пальцем", "и на ёлку залезть", "и жопу не ободрать", "в жопу жареный петух клюнул", "до пизды", "по хую", "пизды давать", "пиздюлей навешать", "не в пизду", "ни в пизду", "ни в красную армию", "не-пришей-к-пизде-рукав", "не в пизду", "ни к жопе рукав", "ни к пизде заплатка", "не в пизду", "пиздой накрыться", "опиздохуительно", "от пизды", "пиздец ворам", "ёбаный в рот", "пиздец котёнку", "больше срать не будет", "пиздоблядство", "в пизде на верхней полке", "где ебутся волки", "в пизде", "в пизде гвозди дёргает", "опиздол", "опездол", "пиздоебля", "коноебля", "пиздоплюйство", "пиздопроёбина", "пиздопроушина", "без пизды", "курочка в гнезде", "а яичко в пизде", "не хочешь срать - не мучай жопу", "до усрачки", "до жопы", "хоть усрись", "хоть жопой ешь", "срать да срать", "всю малину обосрать", "как два пальца обоссать", "чуть свет", "не срамши", "ссаными тряпками закидать", "кипятком ссать", "не бай", "кума", "уссусь", "с похмелья не обсеришь", "идти на хуй", "до хуя", "на хуя", "хуй сосать", "хуева туча", "хуево море", "хуева гора", "хуёв тачка", "хуем груши околачивать", "хуи валять и к стенке приставлять", "хуй положить", "хуй забить", "хуй задвинуть", "хуйнёй страдать", "хуйню пороть", "хуй проссышь", "и на хуй не обосраться", "ни хуя себе", "однохуйственно", "один хуй ", "с лихуем", "хуль ты бродишь", "жопой водишь", "на мой хуй тоску наводишь", "хуеплёт", "не хуй срать", "не хуй собачий", "хуй с бугра", "хуиная голова", "сам-хуй", "сто хуёв тебе в жопу и якорь для равновесия", "хуй тебе в жопу вместо укропу", "хуи пинать", "и на хуй сесть", "и рыбку съесть", "и на ёлку залезть", "и жопу не ободрать", "хуи бросать", "ебуки пускать", "хуями обкладывать", "хуем по лбу", "по хуй море", "по хую мороз", "по пизде жара", "по хуй море", "прикинь хуй к носу", "хуй в кожаном пальто", "доигрался хуй на скрипке", "пиздец котёнку", "больше срать не будет", "весь в поту и хуй во рту", "и дом сгорел", "и хуй не стоит", "хуй и пизда - из одного гнезда", "чтоб хуй стоял и деньги были", "за просто хуй", "на хуя попу гармонь", "на хуя козе баян", "пидарас", "пидораз", "пидорас", "пидор", "пидарасить", "пидормот", "пидорюга", "разъебай", "пидор гнойный", "пидорище", "уёбище" };
        #endregion
        public static bool DoesStringContainProfanities(string input)
        {
            return Profanities.Any(p => input.Contains(p));
        }
        public static bool DoesStringContainUrl(string input)
        {
            return Regex.IsMatch(input, @"(?i)http(s)?://\w*\.\w*");
        }
        public static bool DoesStringContainEmail(string input)
        {
            return Regex.IsMatch(input, @"(?i)@\w*\.\w*");
        }
        public static bool DoesStringNeedModeration(string input)
        {
            input = input.ToLower();
            if (DoesStringContainProfanities(input))
                return true;
            if (DoesStringContainUrl(input))
                return true;
            if (DoesStringContainEmail(input))
                return true;
            return false;
        }
        public static bool DoesNeedModeration<T>(T obj)
        {
            var properties = obj.GetType().GetProperties()
                .Where(p => 
                    p.Name != "Url" && 
                    p.CanRead && 
                    p.PropertyType == typeof(String))
                .Select(p => (string)p.GetValue(obj))
                .Where(p => p != null).ToList();
            foreach (var property in properties)
            {
                if (DoesStringNeedModeration(property))
                    return true;
            }
            return false;
        }
        public AdminController()
        {
			var currentUser = SM.CurrentUser;
            if (currentUser == null || !currentUser.IsAdmin)
                throw new Exception("Доступ запрещен");
            #region Общий код для всех контроллеров
            SM.TryToLoginByCookiesIfNeed();
            ViewBag.L = LayoutViewModel.GetLayoutViewModel();
            #endregion
        }

        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.Heading = "Админка";
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>();
            breadCrumbs.Add(new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true });
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View();
        }

        public ActionResult ModerateAds()
        {
            var viewModel = new AdminModerateAdsViewModel();
            viewModel.Ads = AdsDAL.GetAdsForModeration();
            if (viewModel.Ads.Any())
            {
                foreach (var ad in viewModel.Ads)
                {
                    ad.Photos = PhotosDAL.GetPhotos(ad.Id);
                    ad.Products = AdProductsDAL.GetAdProducts(ad.Id);
                }
            }
            ViewBag.Heading = "Модерация объявлений";
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>();
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Админка", Url = C.SiteUrl + "Admin", Title = "Перейти на главную админки" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true });
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(viewModel);
        }

        public ActionResult ModerateOffers()
        {
            var viewModel = new AdminModerateOffersViewModel();
            viewModel.Offers = OffersDAL.GetOffersForModeration();
            ViewBag.Heading = "Модерация предложений";
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>();
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Админка", Url = C.SiteUrl + "Admin", Title = "Перейти на главную админки" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true });
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(viewModel);
        }
        public ActionResult ModerateUsers()
        {
            var viewModel = new AdminModerateUsersViewModel();
			viewModel.Users = UsersDAL.GetUsers(moderateResultId: (int)ModerateResults.NotChecked);
    //        viewModel.SbisInfoUsers = new List<User>();
    //        foreach (var user in viewModel.Users)
    //        {
    //            var sbisInfoUser = P.GetSbisInformationFromInn(user.INN);
				//if (sbisInfoUser != null)
				//{
				//	sbisInfoUser.Id = user.Id;
				//	viewModel.SbisInfoUsers.Add(sbisInfoUser);
				//}
    //        }
            ViewBag.Heading = "Модерация пользователей";
            #region Хлебные крошки
            var breadCrumbs = new List<BreadCrumbLink>();
            breadCrumbs.Add(new BreadCrumbLink() { Text = "Админка", Url = C.SiteUrl + "Admin", Title = "Перейти на главную админки" });
            breadCrumbs.Add(new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true });
            ViewBag.BreadCrumbs = breadCrumbs;
            #endregion
            return View(viewModel);
        }

		public ActionResult UnregisteredUsers()
		{
			var viewModel = new AdminUnregisteredUsersViewModel();
			viewModel.UnregisteredUsers = UnregisteredUsersDAL.GetUnregisteredUsers();
			return View(viewModel);
		}

		[HttpPost]
		public string DeleteUnregisteredUser(int id)
		{
			var result = UnregisteredUsersDAL.DeleteUnregisteredUser(id);

			return result ? "ok" : "Не удалось удалить незарегистрированного пользователя";
		}

		[HttpPost]
        public bool UpdateAdModerateResult(int adId, int moderateResultId)
        {
            var result = AdsDAL.UpdateAdModerateResult(adId, moderateResultId);
            if (result == true)
            {
                var ad = AdsDAL.GetAd(adId);
                if (ad != null)
                {
                    var notificationsUserId = MContract.Models.User.SystemNotificationsUserId;
                    var messageText = "Ваше " + (ad.AvailableForAllUsers ? "открытое" : "закрытое") + " объявление о " + (ad.IsBuy ? "покупке" : "продаже") +
                                      " №" + ad.Id;
                    if (moderateResultId == (int)ModerateResults.Accepted)
                    {
                        messageText += " прошло ручную модерацию и было опубликовано.";
                    } else
                    {
                        messageText += " не прошло ручную модерацию.";
                    }
                    var message = new Message
                    {
                        SenderId = notificationsUserId,
                        RecipientId = ad.SenderId,
                        Text = messageText
                    };
                    MContract.AppCode.UserHelper.AddMessage(message);
                }
            }
            return result;
        }

		[HttpPost]
		public bool UpdateUserModerateResult(int userId, int moderateResultId)
		{
			var result = UsersDAL.UpdateUserModerateResult(userId, moderateResultId);
			return result;
		}
	}
}