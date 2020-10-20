using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.AppCode
{
	/// <summary>
	/// Урлы, используемые в проекте
	/// </summary>
	public class Urls
	{
		public static string Settings { get { return C.SiteUrl + "settings"; } }
        public static string RegistrationShort { get { return "registration"; } }
        public static string Registration { get { return C.SiteUrl + RegistrationShort; } }


        public static string ResendemailShort { get { return "resendemail"; } }
        public static string Resendemail { get { return C.SiteUrl + ResendemailShort; } }


        public static string LoginShort { get { return "login"; } }
		public static string Login { get { return C.SiteUrl + LoginShort; } }

		public static string LogoutShort { get { return "logout"; } }
		public static string Logout { get { return C.SiteUrl + LogoutShort; } }

		public static string PersonalAreaShort { get { return "my"; } }
		public static string PersonalArea { get { return C.SiteUrl + PersonalAreaShort; } }

		public static string AdsShort { get { return "ads"; } }
		public static string Ads { get { return C.SiteUrl + AdsShort; } }

		public static string CompaniesShort { get { return "companies"; } }
		public static string Companies { get { return C.SiteUrl + CompaniesShort; } }
	}
	
}