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
	public class FilesDAL : BaseDataAccess
	{
		private static UserFile ReadFileInfo(SqlDataReader reader)
		{
			var result = new UserFile
			{
				Id = (int)reader["Id"],
				UserId = (int)reader["UserId"],
				MessageId = (int)reader["MessageId"],
				Name = (string)reader["Name"],
				Extension = (string)reader["Extension"],
				Added = (DateTime)reader["Added"],
				ModerateResult = (ModerateResults)(int)reader["ModerateResult"]
			};

			if (reader["Changed"] != DBNull.Value)
				result.Changed = (DateTime)reader["Changed"];

			return result;
		}

		public static UserFile GetFile(int id)
		{
			UserFile result = null;
			const string query = @"select * from dbo.Files where Id=@Id";

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
						result = ReadFileInfo(reader);
					}
				}
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in FilesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<UserFile> GetFiles(int userId)
		{
			var result = new List<UserFile>();
			string query = $"select * from dbo.Files where UserId={userId}";

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
							var file = ReadFileInfo(reader);
							result.Add(file);
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in FilesDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}

		public static List<UserFile> GetFiles(List<int> userIds = null, List<int> messageIds = null)
		{
			var result = new List<UserFile>();
			string query = "select * from dbo.Files where 1=1";
			if (userIds != null && userIds.Any())
				query += $" and UserId in ({string.Join(",", userIds)})";
			else if (messageIds != null && messageIds.Any())
				query += $" and MessageId in ({string.Join(",", messageIds)})";

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
							var file = ReadFileInfo(reader);
							result.Add(file);
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in FilesDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}

		public static List<UserFile> GetFilesForModerate()
		{
			var result = new List<UserFile>();
			string query = "select * from dbo.Files where ModerateResult=" + (int)ModerateResults.NotChecked;

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
							var file = ReadFileInfo(reader);
							result.Add(file);
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in FilesDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}

		public static int AddFile(UserFile file)
		{
			int newFileId = 0;
			const string query = @"insert into dbo.Files (UserId, MessageId, Name, Extension, Added, Changed, ModerateResult) 
values (@UserId, @MessageId, @Name, @Extension, @Added, @Changed, @ModerateResult);
DECLARE @newFileId int;
   SELECT @newFileId = SCOPE_IDENTITY();
   SELECT @newFileId";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", file.Id);
			sqlCommand.Parameters.AddWithValue("UserId", file.UserId);
			sqlCommand.Parameters.AddWithValue("MessageId", file.MessageId);
			sqlCommand.Parameters.AddWithValue("Name", file.Name);
			sqlCommand.Parameters.AddWithValue("Extension", file.Extension);
			sqlCommand.Parameters.AddWithValue("Added", file.Added);
			sqlCommand.Parameters.AddWithValue("ModerateResult", file.ModerateResult);
			if (file.Changed != null)
				sqlCommand.Parameters.AddWithValue("Changed", file.Changed);
			else
				sqlCommand.Parameters.AddWithValue("Changed", DBNull.Value);

			try
			{
				connect.Open();
				newFileId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				LogsDAL.AddMessage(1, "in FilesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newFileId;
		}

		public static int AddFiles(List<UserFile> files)
		{
			if (!files.Any())
				return 0;

			var result = 0;

			string query = "";

			var i = 0;
			for (i = 0; i < files.Count; i++)
			{
				query += $@"
insert into dbo.Files (UserId, MessageId, Name, Extension, Added, Changed, ModerateResult) 
values (@UserId{i}, @MessageId{i}, @Name{i}, @Extension{i}, @Added{i}, @Changed{i}, @ModerateResult{i});";
			}

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			i = 0;
			foreach (var file in files)
			{
				sqlCommand.Parameters.AddWithValue($"Id{i}", file.Id);
				sqlCommand.Parameters.AddWithValue($"UserId{i}", file.UserId);
				sqlCommand.Parameters.AddWithValue($"MessageId{i}", file.MessageId);
				sqlCommand.Parameters.AddWithValue($"Name{i}", file.Name);
				sqlCommand.Parameters.AddWithValue($"Extension{i}", file.Extension);
				sqlCommand.Parameters.AddWithValue($"Added{i}", file.Added);
				sqlCommand.Parameters.AddWithValue($"ModerateResult{i}", file.ModerateResult);
				if (file.Changed != null)
					sqlCommand.Parameters.AddWithValue($"Changed{i}", file.Changed);
				else
					sqlCommand.Parameters.AddWithValue($"Changed{i}", DBNull.Value);

				i++;
			}

			try
			{
				connect.Open();
				result = (int)sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				LogsDAL.AddMessage(1, "in FilesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result;
		}
		public static bool DeleteFile(int id)
		{
			const string query = "delete from dbo.Files where Id=@Id";

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
				throw new Exception("in FilesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static void UpdateModerateResult(int id, int moderateResult)
		{
			string query = "update dbo.Files set ModerateResult=@ModerateResult, Changed=@Changed where Id=@Id";

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);
			sqlCommand.Parameters.AddWithValue("Id", id);
			sqlCommand.Parameters.AddWithValue("ModerateResult", moderateResult);
			sqlCommand.Parameters.AddWithValue("Changed", DateTime.Now.ToUniversalTime());

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				LogsDAL.AddError("Exception in FilesDAL.UpdateModerateResult(int id, int moderateResult): " + ex.ToString());
			}
			finally
			{
				connect.Close();
			}
		}
	}
}