using MContract.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MContract.DAL
{
	public class UserFavoriteAdsDAL : BaseDataAccess
	{
		private static UserFavoriteAd ReadUserFavoriteAdInfo(SqlDataReader reader)
		{
			var result = new UserFavoriteAd
			{
				Id = (int)reader["Id"],
				UserId = (int)reader["UserId"],
				AdId = (int)reader["AdId"]
			};
			
			return result;
		}

		public static List<UserFavoriteAd> GetUserFavoriteAds(int? adId = null, List<int> adIds = null)
		{
			var result = new List<UserFavoriteAd>();
			string query = "select * from dbo.UsersFavoriteAds where 1 = 1";

			if (adId.HasValue)
				query += $" and AdId = {adId.Value}";

			if (adIds != null)
			{
				if (!adIds.Any())
					return result;

				query += $" and AdId in ({String.Join(",", adIds)})";
			}

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var ticker = ReadUserFavoriteAdInfo(reader);
					result.Add(ticker);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in UserFavoriteAdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static bool DeleteUserFavoriteAd(int id)
		{
			const string query = "delete from dbo.UserFavoriteAds where Id = @Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", id);

			int result = 0;
			try
			{
				connect.Open();
				result = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in UserFavoriteAdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}
	}
}