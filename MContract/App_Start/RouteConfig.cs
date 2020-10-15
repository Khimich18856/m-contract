using MContract.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MContract
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Registration Page",
				url: Urls.RegistrationShort,
				defaults: new { controller = "User", action = "SignUp" }
			);

			routes.MapRoute(
				name: "Login Page",
				url: Urls.LoginShort,
				defaults: new { controller = "User", action = "Login" }
			);

			routes.MapRoute(
				name: "Logout Page",
				url: Urls.LogoutShort,
				defaults: new { controller = "User", action = "Logout" }
			);

			routes.MapRoute(
				name: "My Personal Area Page",
				url: Urls.PersonalAreaShort,
				defaults: new { controller = "User", action = "MyProfile" }
			);

			routes.MapRoute(
				name: "Companies Page",
				url: Urls.CompaniesShort,
				defaults: new { controller = "User", action = "Companies" }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
