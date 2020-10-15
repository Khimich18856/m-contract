using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.DAL
{
	public class BaseDataAccess
	{
		protected static string connStr;
		public static void SetConnectionString(string connectionString)
		{
			connStr = connectionString;
		}
	}
}