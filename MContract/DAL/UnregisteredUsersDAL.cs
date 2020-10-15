using MContract.Models;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Configuration;
using System.Text;

namespace MContract.DAL
{
	public class UnregisteredUsersDAL : BaseDataAccess
	{
		private static UnregisteredUser ReadUnregisteredUserInfo(SqlDataReader reader)
		{
			var result = new UnregisteredUser
			{
				Id = (int)reader["Id"],
				CompanyName = ((string)reader["CompanyName"]).Trim(),
				ContactName = ((string)reader["ContactName"]).Trim(),
				Email = ((string)reader["Email"]).Trim(),
				TypeOfOwnership = (TypesOfOwnership)(int)reader["TypeOfOwnershipId"],
				CityId = (int)reader["CityId"],
				INN = ((string)reader["INN"]).Trim(),
				OGRN = ((string)reader["OGRN"]).Trim(),
				PhoneNumber = ((string)reader["PhoneNumber"]).Trim(),
				Created = (DateTime)reader["Created"]
			};

			if (reader["PhoneNumberCity"] != DBNull.Value)
				result.PhoneNumberCity = ((string)reader["PhoneNumberCity"]).Trim();

			#region поля СБИС
			if (reader["SbisCompanyName"] != DBNull.Value)
				result.SbisCompanyName = ((string)reader["SbisCompanyName"]).Trim();

			if (reader["SbisTypeOfOwnershipId"] != DBNull.Value)
				result.SbisTypeOfOwnershipId = (int)reader["SbisTypeOfOwnershipId"];

			if (reader["SbisOGRN"] != DBNull.Value)
				result.SbisOGRN = ((string)reader["SbisOGRN"]).Trim();

			if (reader["SbisWorksFrom"] != DBNull.Value)
				result.SbisWorksFrom = (DateTime)reader["SbisWorksFrom"];

			#endregion

			return result;
		}

		public static UnregisteredUser GetUnregisteredUser(int id)
		{
			UnregisteredUser result = null;
			const string query = @"select * from dbo.UnregisteredUsers where Id=@Id";
			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Id", id);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result = ReadUnregisteredUserInfo(reader);

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in UnregisteredUsersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<UnregisteredUser> GetUnregisteredUsers()
		{
			var result = new List<UnregisteredUser>();
			string query = "select * from dbo.UnregisteredUsers where 1 = 1";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var unregisteredUser = ReadUnregisteredUserInfo(reader);
					result.Add(unregisteredUser);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in UnregisteredUsersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static int AddUnregisteredUser(UnregisteredUser unregisteredUser)
		{
			int newUnregisteredUserId = 0;
			const string query = @"insert into dbo.UnregisteredUsers (ContactName, CompanyName, Email, TypeOfOwnershipId, CityId, INN, OGRN, PhoneNumber, Created, SbisCompanyName, SbisTypeOfOwnershipId, SbisOGRN, SbisWorksFrom) 
values (@ContactName, @CompanyName, @Email, @TypeOfOwnershipId, @CityId, @INN, @OGRN, @PhoneNumber, @Created, @SbisCompanyName, @SbisTypeOfOwnershipId, @SbisOGRN, @SbisWorksFrom);
DECLARE @newUnregisteredUserID int;
   SELECT @newUnregisteredUserID = SCOPE_IDENTITY();
   SELECT @newUnregisteredUserID";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			var parameters = sqlCommand.Parameters;

			AddAddOrUpdateSqlParameters(parameters, unregisteredUser);
			parameters.AddWithValue("OpenDialogRespondentIds", "");
			parameters.AddWithValue("CurrentRespondentId", 0);

			try
			{
				connect.Open();
				newUnregisteredUserId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in UnregisteredUsersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newUnregisteredUserId;
		}

		private static void AddAddOrUpdateSqlParameters(SqlParameterCollection parameters, UnregisteredUser unregisteredUser)
		{
			parameters.AddWithValue("ContactName", unregisteredUser.ContactName);
			parameters.AddWithValue("CompanyName", unregisteredUser.CompanyName);
			parameters.AddWithValue("Email", unregisteredUser.Email);
			parameters.AddWithValue("TypeOfOwnershipId", unregisteredUser.TypeOfOwnership);
			parameters.AddWithValue("CityId", unregisteredUser.CityId);
			parameters.AddWithValue("INN", unregisteredUser.INN);
			parameters.AddWithValue("OGRN", unregisteredUser.OGRN);
			parameters.AddWithValue("PhoneNumber", unregisteredUser.PhoneNumber);

			if (unregisteredUser.PhoneNumberCity != null)
				parameters.AddWithValue("PhoneNumberCity", unregisteredUser.PhoneNumberCity);
			else
				parameters.AddWithValue("PhoneNumberCity", DBNull.Value);

			parameters.AddWithValue("Created", unregisteredUser.Created);

			if (unregisteredUser.SbisCompanyName != null)
				parameters.AddWithValue("SbisCompanyName", unregisteredUser.SbisCompanyName);
			else
				parameters.AddWithValue("SbisCompanyName", DBNull.Value);

			if (unregisteredUser.SbisTypeOfOwnershipId.HasValue)
				parameters.AddWithValue("SbisTypeOfOwnershipId", unregisteredUser.SbisTypeOfOwnershipId.Value);
			else
				parameters.AddWithValue("SbisTypeOfOwnershipId", DBNull.Value);

			if (unregisteredUser.SbisOGRN != null)
				parameters.AddWithValue("SbisOGRN", unregisteredUser.SbisOGRN);
			else
				parameters.AddWithValue("SbisOGRN", DBNull.Value);

			if (unregisteredUser.SbisWorksFrom != null)
				parameters.AddWithValue("SbisWorksFrom", unregisteredUser.SbisWorksFrom);
			else
				parameters.AddWithValue("SbisWorksFrom", DBNull.Value);
		}

		public static bool DeleteUnregisteredUser(int id)
		{
			const string query = "delete from dbo.UnregisteredUsers where Id = @Id";

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
				throw new Exception("in UnregisteredUsersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool UpdateUnregisteredUserChangeCurrentRespondentId(int unregisteredUserId, int respondentId)
		{
			string query = $"update dbo.UnregisteredUsers set CurrentRespondentId={respondentId} where Id={unregisteredUserId}";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			int result = 0;
			try
			{
				connect.Open();
				result = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in UnregisteredUsersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}
	}
}