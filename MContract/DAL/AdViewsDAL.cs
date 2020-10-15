using MContract.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MContract.DAL
{
	public class AdViewsDAL : BaseDataAccess
	{
		private static AdView ReadAdViewInfo(SqlDataReader reader, bool getAdViewTypeInfo = false)
		{
			var result = new AdView
			{
				Id = (int)reader["Id"],
				UserId = (int)reader["UserId"],
				AdId = (int)reader["AdId"],
				Created = (DateTime)reader["Created"]
			};
			
			return result;
		}

		public static int AddAdView(AdView adView)
		{
			int newUserId = 0;
			const string query = @"insert into dbo.AdViews (UserId, AdId, Created) 
values (@UserId, @AdId, @Created);
DECLARE @newUserID int;
   SELECT @newUserID = SCOPE_IDENTITY();
   SELECT @newUserID";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			var parameters = sqlCommand.Parameters;

			parameters.AddWithValue("UserId", adView.UserId);
			parameters.AddWithValue("AdId", adView.AdId);
			parameters.AddWithValue("Created", adView.Created);

			try
			{
				connect.Open();
				newUserId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdViewsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newUserId;
		}

		public static List<AdView> GetAdViews(int? userId = null, int? adId = null, DateTime? createdFrom = null)
		{
			var result = new List<AdView>();
			string query = "select * from dbo.AdViews where 1 = 1";

			if (userId.HasValue)
				query += $" and UserId = {userId.Value}";

			if (adId.HasValue)
				query += $" and AdId = {adId.Value}";

			if (createdFrom.HasValue)
				query += " and Created > @CreatedFrom";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			if (createdFrom.HasValue)
				sqlCommand.Parameters.AddWithValue("CreatedFrom", createdFrom.Value);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var adView = ReadAdViewInfo(reader);
					result.Add(adView);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdViewsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static bool DeleteAdView(int id)
		{
			const string query = "delete from dbo.AdViews where Id = @Id";

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
				throw new Exception("in AdViewsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}
	}
}