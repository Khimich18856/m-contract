using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MContract.AppCode
{
	public class C
	{
		public static string SiteUrlClear
		{
			get
			{
				return ConfigurationManager.AppSettings["production"] == "true" ? "http://m-contract.ru" : "http://localhost:3254";
			}
		}

		public static string SiteUrl
		{
			get
			{
				return ConfigurationManager.AppSettings["production"] == "true" ? "http://m-contract.ru/" : "http://localhost:3254/";
			}
		}

		public static bool IsProduction
		{
			get
			{
				return ConfigurationManager.AppSettings["production"] == "true";
			}
		}

		public static int ChatBoxMaxDialogs
		{
			get
			{
				return 10;
			}
		}

		public static int ChatBoxSecondsBetweenMessagesRefresh
		{
			get
			{
				return 5;
			}
		}
	}
}