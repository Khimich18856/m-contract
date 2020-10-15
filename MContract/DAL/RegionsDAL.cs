using MContract.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MContract.DAL
{
	public class RegionsDAL : BaseDataAccess
	{
		private static List<Region> _cachedRegions = null;

		private static Region ReadRegionInfo(SqlDataReader reader)
		{
			var result = new Region
			{
				Id = (int)reader["Id"],
				Name = ((string)reader["Name"]).Trim()
			};

			return result;
		}

		public static List<Region> GetRegions()
		{
			if (_cachedRegions != null)
				return _cachedRegions;

			var result = new List<Region>();
			string query = "select * from dbo.Regions";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var region = ReadRegionInfo(reader);
					result.Add(region);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in RegionsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			_cachedRegions = result;

			return result;
		}
	}
}