using MContract.AppCode;
using MContract.DAL;
using MContract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MContract.Controllers
{
	public class HomeController : Controller
	{
		public HomeController()
		{
			#region Общий код для всех контроллеров
			SM.TryToLoginByCookiesIfNeed();
			ViewBag.L = LayoutViewModel.GetLayoutViewModel();
			#endregion
		}

		public ActionResult Index()
		{
			var usdTickerId = 7;
			var eurTickerId = 8;

			var tickers = TickersDAL.GetTickers();
			var investingComTickers = tickers.Where(t => t.InvestingComPairId != null).ToList();
			var lmeTickers = tickers.Where(t => t.LmeName != null).ToList();

			var quotes = QuotesDAL.GetQuotes(fromDate: DateTime.Now.AddDays(-14));
			var usdQuotes = quotes.Where(q => q.TickerId == usdTickerId).OrderByDescending(q => q.CbrDate).Take(2).ToList();
			var eurQuotes = quotes.Where(q => q.TickerId == eurTickerId).OrderByDescending(q => q.CbrDate).Take(2).ToList();

			var viewModel = new HomeIndexViewModel
			{
				InvestingComQuotes = new List<QuoteItemViewModel>(),
				LmeQuotes = new List<QuoteItemViewModel>(),
				TodayUsdQuote = usdQuotes.Count > 1 
								? usdQuotes[1] 
								: usdQuotes.Count > 0 
								? usdQuotes[0] 
								: null,
				TomorrowUsdQuote = usdQuotes.Count > 0
								   ? usdQuotes[0]
								   : null,
				TodayEuroQuote = eurQuotes.Count > 1 
								? eurQuotes[1] 
								: eurQuotes.Count > 0 
								? eurQuotes[0] 
								: null,
				TomorrowEuroQuote = eurQuotes.Count > 0 
									? eurQuotes[0] 
									: null
			};

			if (usdQuotes.Count > 0 && DateTime.Now >= usdQuotes[0].CbrDate)
			{
				viewModel.TodayUsdQuote = viewModel.TomorrowUsdQuote;
				viewModel.TodayEuroQuote = viewModel.TomorrowEuroQuote;
			}

			foreach (var ticker in investingComTickers)
			{
				viewModel.InvestingComQuotes.Add(new QuoteItemViewModel()
				{
					TickerName = ticker.Description,
					Change = (decimal)ticker.ChangeFromYesterdayClose,
					ChangePercent = (decimal)ticker.ChangeFromYesterdayClosePercent,
					Quote = (decimal)ticker.LastQuote,
					TickerId = ticker.Id,
					TimeStr = ticker.TimeStr
					//ChartUrl = Urls.Quotes + "/" + tickerType.NameForUrl + "/" + ticker.NameForUrl,
					//ATitle = "Открыть график котировок акций " + ticker.Description
				});
			}

			foreach (var ticker in lmeTickers)
			{
				viewModel.LmeQuotes.Add(new QuoteItemViewModel()
				{
					TickerName = ticker.Description,
					Quote = (decimal)ticker.LastQuote,
					TickerId = ticker.Id
					//ChartUrl = Urls.Quotes + "/" + tickerType.NameForUrl + "/" + ticker.NameForUrl,
					//ATitle = "Открыть график котировок акций " + ticker.Description
				});
			}

			ViewBag.Heading = "М-Контракт";
			return View(viewModel);
		}

		public ActionResult About()
		{
			ViewBag.Heading = "Сведения о компании";
			#region Хлебные крошки
			var breadCrumbs = new List<BreadCrumbLink>();
			breadCrumbs.Add(new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true });
			ViewBag.BreadCrumbs = breadCrumbs;
			#endregion
			ViewBag.Message = "Сведения сведения сведения сведения";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Heading = "Обратная связь";
			#region Хлебные крошки
			var breadCrumbs = new List<BreadCrumbLink>();
			breadCrumbs.Add(new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true });
			ViewBag.BreadCrumbs = breadCrumbs;
			#endregion
			ViewBag.Message = "Your contact page.";

			return View();
		}
		public ActionResult Calculator()
		{
			ViewBag.Heading = "Ломокалькулятор";
			#region Хлебные крошки
			var breadCrumbs = new List<BreadCrumbLink>();
			breadCrumbs.Add(new BreadCrumbLink() { Text = ViewBag.Heading, EndPoint = true });
			ViewBag.BreadCrumbs = breadCrumbs;
			#endregion
			return View();
		}
	}
}