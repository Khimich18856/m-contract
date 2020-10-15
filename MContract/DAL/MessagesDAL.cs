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
	public class MessagesDAL : BaseDataAccess
	{
		private static Message ReadMessageInfo(SqlDataReader reader)
		{
			var result = new Message
			{
				Id = (int)reader["Id"],
				SenderId = (int)reader["SenderId"],
				RecipientId = (int)reader["RecipientId"],
				Text = (string)reader["Text"],
				Date = (DateTime)reader["Created"],
				IsRead = (bool)reader["IsRead"],
				IsReviewContractNotification = (bool)reader["IsReviewContractNotification"],
				IsContractReviewed = (bool)reader["IsContractReviewed"],
				OfferId = (int)reader["OfferId"],
				AdId = (int)reader["AdId"],
				RequestJoinAdFromUserId = (int)reader["RequestJoinAdFromUserId"]
			};
			return result;
		}

		public static Message GetMessage(int id)
		{
			Message result = null;
			const string query = @"select * from dbo.Messages where Id=@Id";

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
						result = ReadMessageInfo(reader);
					}
				}
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in MessagesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		/*public static List<Message> GetMessages(int dialogId)
		{
			var result = new List<Message>();
			string query = "select * from dbo.Messages where DialogId=" + dialogId;

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
							var message = ReadMessageInfo(reader);
							result.Add(message);
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in MessagesDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}*/

		public static List<Message> GetMessages(int senderId, int recipientId = 0)
		{
			var result = new List<Message>();
			if (senderId == 0)
			{
				return result;
			}
			string query = "";
			if (recipientId != 0)
			{
				query = "select * from dbo.Messages" +
							   " where SenderId=" + senderId + " and RecipientId=" + recipientId +
							   " or SenderId=" + recipientId + " and RecipientId=" + senderId;
			}
			else
			{
				query = "select * from dbo.Messages" +
							   " where SenderId=" + senderId + " or RecipientId=" + senderId;
			}

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
							var message = ReadMessageInfo(reader);
							result.Add(message);
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in MessagesDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}

		public static List<Message> GetUnreadMessages(int recipientId, int senderId = 0)
		{
			var result = new List<Message>();
			string query = "select * from dbo.Messages" +
						   $" where RecipientId = {recipientId} and IsRead=0" + 
						   (senderId != 0 ? " and SenderId=" + senderId : "");

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
							var message = ReadMessageInfo(reader);
							result.Add(message);
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in MessagesDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}
		public static int GetUnreadMessagesCount(int recipientId, int senderId = 0)
		{
			int result = 0;
			string query = "select count(*) from dbo.Messages" +
						   $" where RecipientId = {recipientId} and IsRead=0" +
						   (senderId != 0 ? " and SenderId=" + senderId : "");

			using (var conn = new SqlConnection(connStr))
			{
				var sqlCommand = new SqlCommand(query, conn);
				sqlCommand.CommandTimeout = 180;

				try
				{
					conn.Open();

					result = (int)sqlCommand.ExecuteScalar();
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in MessagesDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}
		public static List<Message> GetNewMessagesForChatBox(int recipientId, int lastMessageId = 0, int senderId = 0)
		{
			var result = new List<Message>();
			string query = @"select * from dbo.Messages where 
							 (RecipientId = @RecipientId or RecipientId = @SenderId and SenderId = @RecipientId) and Id > @LastMessageId";
						//and Created > @Created";// +
						//" and cast(Created as datetime)>cast(@Created as datetime)";
			//else
				//query = "select * from dbo.Messages where " +
						//"(RecipientId=@RecipientId or SenderId=@RecipientId) and cast(Created as datetime)>cast(@Created as datetime)";


			using (var conn = new SqlConnection(connStr))
			{
				var sqlCommand = new SqlCommand(query, conn);
				sqlCommand.CommandTimeout = 180;
				sqlCommand.Parameters.AddWithValue("RecipientId", recipientId);
				//sqlCommand.Parameters.AddWithValue("Created", DateTime.Now.ToUniversalTime().AddSeconds(C.ChatBoxSecondsBetweenMessagesRefresh));
				sqlCommand.Parameters.AddWithValue("SenderId", senderId);
				sqlCommand.Parameters.AddWithValue("LastMessageId", lastMessageId);

				try
				{
					conn.Open();

					using (var reader = sqlCommand.ExecuteReader())
					{
						while (reader.Read())
						{
							var message = ReadMessageInfo(reader);
							result.Add(message);
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in MessagesDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}

		public static int AddMessage(Message message)
		{
			int newMessageId = 0;
			const string query = @"insert into dbo.Messages (SenderId, RecipientId, Text, Created, IsRead, IsReviewContractNotification, IsContractReviewed, OfferId, AdId, RequestJoinAdFromUserId) 
values (@SenderId, @RecipientId, @Text, @Created, @IsRead, @IsReviewContractNotification, @IsContractReviewed, @OfferId, @AdId, @RequestJoinAdFromUserId);
DECLARE @newMessageId int;
   SELECT @newMessageId = SCOPE_IDENTITY();
   SELECT @newMessageId";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("SenderId", message.SenderId);
			sqlCommand.Parameters.AddWithValue("RecipientId", message.RecipientId);
			sqlCommand.Parameters.AddWithValue("Text", message.Text);
			sqlCommand.Parameters.AddWithValue("Created", message.Date);
			sqlCommand.Parameters.AddWithValue("IsRead", message.IsRead);
			sqlCommand.Parameters.AddWithValue("IsReviewContractNotification", message.IsReviewContractNotification);
			sqlCommand.Parameters.AddWithValue("IsContractReviewed", message.IsContractReviewed);
			sqlCommand.Parameters.AddWithValue("OfferId", message.OfferId);
			sqlCommand.Parameters.AddWithValue("AdId", message.AdId);
			sqlCommand.Parameters.AddWithValue("RequestJoinAdFromUserId", message.RequestJoinAdFromUserId);

			try
			{
				connect.Open();
				newMessageId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				LogsDAL.AddMessage(1, "in MessagesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newMessageId;
		}

		/*public static bool MarkMessagesAsRead(int dialogId)
		{
			string query = @"update dbo.Messages set IsRead=1 where IsRead=0 and DialogId = " + dialogId;

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				LogsDAL.AddError("Exception in MessagesDAL." + methodName + "(...): " + ex.ToString());
			}
			finally
			{
				connect.Close();
			}

			return true;
		}*/

		public static bool MarkMessagesAsRead(int recipientId, int senderId = 0)
		{
			string query = "update dbo.Messages set IsRead=1 where IsRead=0 and RecipientId=" + recipientId;
			if (senderId != 0)
			{
				query += " and SenderId=" + senderId;
			} 

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				LogsDAL.AddError("Exception in MessagesDAL." + methodName + "(...): " + ex.ToString());
			}
			finally
			{
				connect.Close();
			}

			//SM.NeedRefreshUnreadMessagesCount = true;

			return true;
		}

		public static bool MarkContractAsReviewed(int offerId)
		{
			string query = @"update dbo.Messages set IsContractReviewed=1 where OfferId = " + offerId;

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				LogsDAL.AddError("Exception in MessagesDAL." + methodName + "(...): " + ex.ToString());
			}
			finally
			{
				connect.Close();
			}

			return true;
		}

		public static bool UpdateMessageSetRequestJoinAdFromUserId0(int adId, int userId)
		{
			string query = $"update dbo.Messages set RequestJoinAdFromUserId = 0 where AdId = {adId} and RequestJoinAdFromUserId = {userId}";

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				LogsDAL.AddError("Exception in MessagesDAL." + methodName + "(...): " + ex.ToString());
			}
			finally
			{
				connect.Close();
			}

			return true;
		}

		/// <summary>
		/// Помечает сообщения прочитанными
		/// </summary>
		/// <param name="ids">Список Id сообщений</param>
		public static bool UpdateMessageSetIsReadTrue(List<int> ids)
		{
			if (ids == null && !ids.Any())
				return false;

			string query = $"update dbo.Messages set IsRead = 1 where Id in (" + String.Join(", ", ids) + ")";

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				LogsDAL.AddError("Exception in MessagesDAL." + methodName + "(...): " + ex.ToString());
			}
			finally
			{
				connect.Close();
			}

			return true;
		}

		public static bool DeleteMessage(int id)
		{
			const string query = "delete from dbo.Messages where Id = @Id";

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
				throw new Exception("in MessagesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool DeleteMessagesFromChat(int userId1, int userId2)
		{
			string query = $"delete from dbo.Messages where SenderId = {userId1} and RecipientId = {userId2} or SenderId = {userId2} and RecipientId = {userId1}";

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
				throw new Exception("in MessagesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool DeleteAllMessages(int userId)
		{
			const string query = "delete from dbo.Messages where SenderId=@UserId or RecipientId=@UserId";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("UserId", userId);

			int result = 0;
			try
			{
				connect.Open();
				result = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in MessagesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static int GetLastMessageId(int userId)
		{
			int result = 0;
			const string query = @"select top 1 Id from dbo.Messages where SenderId=@UserId or RecipientId=@UserId order by Id desc";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("UserId", userId);

			try
			{
				connection.Open();

				using (var reader = sqlCommand.ExecuteReader())
				{
					if (reader.Read())
					{
						result = (int)reader["Id"];
					}
				}
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in MessagesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}
	}
}