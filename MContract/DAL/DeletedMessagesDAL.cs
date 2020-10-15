using MContract.AppCode;
using MContract.DAL;
using MContract.Models;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MContract.DAL
{
	public class DeletedMessagesDAL : BaseDataAccess
	{
		private static DeletedMessage ReadDeletedMessageInfo(SqlDataReader reader)
		{
			var result = new DeletedMessage
			{
				Id = (int)reader["Id"],
				UserId = (int)reader["UserId"],
				DialogWithUserId = (int)reader["DialogWithUserId"],
				MessageId = (int)reader["MessageId"]
			};
			return result;
		}

		public static DeletedMessage GetDeletedMessage(int id)
		{
			DeletedMessage result = null;
			const string query = @"select * from dbo.DeletedMessages where Id=@Id";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Id", id);

			try
			{
				connection.Open();

				using (var reader = sqlCommand.ExecuteReader())
				{
					if (reader.Read())
					{
						result = ReadDeletedMessageInfo(reader);
					}
				}
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in DeletedMessagesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<DeletedMessage> GetDeletedMessages(int? userId = null, int? dialogWithUserId = null)
		{
			var result = new List<DeletedMessage>();
			string query = "select * from dbo.DeletedMessages where 1 = 1";

			if (userId.HasValue)
				query += $" and UserId = {userId.Value}";

			if (dialogWithUserId.HasValue)
				query += $" and DialogWithUserId = {dialogWithUserId.Value}";

			using (var conn = new SqlConnection(connStr))
			{
				var sqlCommand = new SqlCommand(query, conn);
				sqlCommand.CommandTimeout = 180;

				try
				{
					conn.Open();

					using (var reader = sqlCommand.ExecuteReader())
					{
						while (reader.Read())
						{
							var deletedMessage = ReadDeletedMessageInfo(reader);
							result.Add(deletedMessage);
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in DeletedMessagesDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}

		public static int AddDeletedMessage(DeletedMessage deletedMessage)
		{
			int newDeletedMessageId = 0;
			const string query = @"insert into dbo.DeletedMessages (UserId, DialogWithUserId, MessageId) 
values (@UserId, @DialogWithUserId, @MessageId);
DECLARE @newDeletedMessageId int;
   SELECT @newDeletedMessageId = SCOPE_IDENTITY();
   SELECT @newDeletedMessageId";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("UserId", deletedMessage.UserId);
			sqlCommand.Parameters.AddWithValue("DialogWithUserId", deletedMessage.DialogWithUserId);
			sqlCommand.Parameters.AddWithValue("MessageId", deletedMessage.MessageId);

			try
			{
				connect.Open();
				newDeletedMessageId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in DeletedMessagesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newDeletedMessageId;
		}
	}
}