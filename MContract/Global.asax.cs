using MContract.AppCode;
using MContract.Controllers;
using MContract.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MContract
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
			BaseDataAccess.SetConnectionString(connStr);

			if (ConfigurationManager.AppSettings["production"] == "true")
			{
				var refreshQuotesFromInvestingComThread = new System.Threading.Thread(P.RefreshQuotesFromInvestingCom);
				refreshQuotesFromInvestingComThread.Start();

				var refreshQuotesFromCbr = new System.Threading.Thread(P.RefreshQuotesFromCbr);
				refreshQuotesFromCbr.Start();

				var refreshQuotesFromLme = new System.Threading.Thread(P.RefreshQuotesFromLme);
				refreshQuotesFromLme.Start();

				var recalculateUserRatings = new System.Threading.Thread(UserHelper.RecalculateUserRatings);
				recalculateUserRatings.Start();
			}

			var sendExpiredAdAndOffersMessage = new System.Threading.Thread(AdsController.SendExpiredAdAndOffersNotificationMessage);
			sendExpiredAdAndOffersMessage.Start();
		}
	}
}
