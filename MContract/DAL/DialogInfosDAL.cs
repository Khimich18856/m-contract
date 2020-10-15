using MContract.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MContract.DAL
{
	public class DialogInfosDAL : BaseDataAccess
	{
		private static DialogInfo ReadDialogInfoInfo(SqlDataReader reader, bool getDialogInfoTypeInfo = false)
		{
			var result = new DialogInfo
			{
				Id = (int)reader["Id"],
				UserId = (int)reader["UserId"],
				RespondentId = (int)reader["RespondentId"],
				ShowMessagesFromId = (int)reader["ShowMessagesFromId"]
			};

			return result;
		}

		public static DialogInfo GetDialogInfo(int userId, int respondentId)
		{
			DialogInfo result = null;
			string query = $"select * from dbo.DialogInfos where UserId = {userId} and RespondentId = {respondentId}";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result = ReadDialogInfoInfo(reader);
				
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in DialogInfosDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<DialogInfo> GetDialogInfos(int userId)
		{
			var result = new List<DialogInfo>();
			string query = $"select * from dbo.DialogInfos where UserId = {userId}";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var dialogInfo = ReadDialogInfoInfo(reader);
					result.Add(dialogInfo);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in DialogInfosDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static int AddDialogInfo(DialogInfo dialogInfo)
		{
			int newMessageId = 0;
			string query = $@"insert into dbo.DialogInfos (UserId, RespondentId, ShowMessagesFromId) 
values (@UserId, @RespondentId, @ShowMessagesFromId);
DECLARE @newDialogInfoId int;
   SELECT @newDialogInfoId = SCOPE_IDENTITY();
   SELECT @newDialogInfoId";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			sqlCommand.Parameters.AddWithValue("UserId", dialogInfo.UserId);
			sqlCommand.Parameters.AddWithValue("RespondentId", dialogInfo.RespondentId);
			sqlCommand.Parameters.AddWithValue("ShowMessagesFromId", dialogInfo.ShowMessagesFromId);

			try
			{
				connect.Open();
				newMessageId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in DialogInfosDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newMessageId;
		}

		public static bool UpdateDialogInfo(DialogInfo dialogInfo)
		{
			string query = $"update dbo.DialogInfos set ShowMessagesFromId={dialogInfo.ShowMessagesFromId} where Id = {dialogInfo.Id}";

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
				throw new Exception("in DialogInfosDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool DeleteDialogInfo(int id)
		{
			const string query = "delete from dbo.DialogInfos where Id = @Id";

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
				throw new Exception("in DialogInfosDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}
	}
}