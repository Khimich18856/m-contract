using MContract.DAL;
using MContract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MContract.AppCode
{
	public class CookiesHelper
	{
		public static void SaveCookiesForHideAuthorization(string email, string socId, string password)
		{
			var hashPassword = GetHash(password);

			HttpContext.Current.Response.Cookies["ep"]["e"] = email ?? "";
			HttpContext.Current.Response.Cookies["ep"]["p"] = hashPassword;
			HttpContext.Current.Response.Cookies["ep"].Expires = DateTime.Now.AddYears(1);
		}

		public static UserCookie GetUserFromCookies()
		{
			if (HttpContext.Current != null && HttpContext.Current.Request.Cookies["ep"] != null)
			{
				return new UserCookie()
				{
					Email = HttpContext.Current.Request.Cookies["ep"]["e"],
					HashPassword = HttpContext.Current.Request.Cookies["ep"]["p"]
				};
			}

			return null;
		}

		public static string GetHash(string input)
		{
			var algorithm = new MD5CryptoServiceProvider();

			Byte[] inputBytes = Encoding.UTF8.GetBytes(input);

			Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

			return BitConverter.ToString(hashedBytes);
		}
		
	}
}