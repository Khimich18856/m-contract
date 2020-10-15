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
	public class PhotosDAL : BaseDataAccess
	{
		private static Photo ReadPhotoInfo(SqlDataReader reader)
		{
			var result = new Photo
			{
				Id = (int)reader["Id"],
				GroupId = (Guid)reader["GroupId"],
				FileNameWithExtension = (string)reader["FileNameWithExtension"],
				IsMain = (bool)reader["IsMain"],
				ModerateResult = (ModerateResults)(int)reader["ModerateResult"],
				Added = (DateTime)reader["Added"],
				Changed = (DateTime)reader["Changed"],
				PhotoType = (PhotoTypes)reader["PhotoTypeId"]
			};

			if (reader["UserId"] != DBNull.Value)
				result.UserId = (int)reader["UserId"];

			if (reader["AdId"] != DBNull.Value)
				result.AdId = (int)reader["AdId"];

			if (reader["Width"] != DBNull.Value)
				result.Width = (int)reader["Width"];

			if (reader["Height"] != DBNull.Value)
				result.Height = (int)reader["Height"];

			return result;
		}

		public static List<Photo> GetCompanyLogoGroup(int? userId = null, List<int> userIds = null)
		{
			var result = new List<Photo>();
			string query = "select * from dbo.Photos where PhotoTypeId = " + (int)PhotoTypes.CompanyLogo;

			if (userId.HasValue)
				query += $" and UserId={userId.Value}";

			if (userIds != null)
			{
				if (!userIds.Any())
					return result;

				query += " and UserId in (" + String.Join(", ", userIds) + ")";
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
							var photo = ReadPhotoInfo(reader);
							result.Add(photo);
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in PhotosDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}

		public static List<Photo> GetPhotoGroup(Guid groupId)
		{
			var result = new List<Photo>();
			string query = "select * from dbo.Photos where GroupId=@GroupId";

			using (var conn = new SqlConnection(connStr))
			{
				var sqlCommand = new SqlCommand(query, conn);
				sqlCommand.CommandTimeout = 180;
				sqlCommand.Parameters.AddWithValue("GroupId", groupId);

				try
				{
					conn.Open();

					using (var reader = sqlCommand.ExecuteReader())
					{
						while (reader.Read())
						{
							var photo = ReadPhotoInfo(reader);
							result.Add(photo);
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in PhotosDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}

		public static Photo GetPhoto(int id)
		{
			Photo result = null;
			const string query = @"select * from dbo.Photos where Id=@Id";

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
						result = ReadPhotoInfo(reader);
					}
				}
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in PhotosDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<Photo> GetPhotos(int adId)
		{
			var result = new List<Photo>();
			string query = "select * from dbo.Photos where AdId = " + adId;

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
							var photo = ReadPhotoInfo(reader);
							result.Add(photo);
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in PhotosDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}


		public static List<Photo> GetPhotos(List<int> adIds = null, List<int> userIds = null, bool? isMain = null, int? maxWidth = null)
		{
			var result = new List<Photo>();
			string query = "select * from dbo.Photos where 1=1";
			if (adIds != null && adIds.Any() && userIds != null && userIds.Any())
				query += $"and AdId in ({string.Join(",", adIds)}) or UserId in ({string.Join(",", userIds)})";
			else if (adIds != null && adIds.Any())
				query += $" and AdId in ({string.Join(",", adIds)})";
			else if (userIds != null && userIds.Any())
				query += $" and UserId in ({string.Join(",", userIds)})";

			if (isMain.HasValue)
				query += " and IsMain = " + (isMain.Value ? 1 : 0);

			if (maxWidth.HasValue)
				query += $" and Width <= {maxWidth.Value}";

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
							var photo = ReadPhotoInfo(reader);
							result.Add(photo);
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in PhotosDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}

		public static List<Photo> GetPhotosForModerate()
		{
			var result = new List<Photo>();
			string query = "select * from dbo.Photos where ModerateResult=" + (int)ModerateResults.NotChecked;

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
							var photo = ReadPhotoInfo(reader);
							result.Add(photo);
						}
					}
				}
				catch (Exception ex)
				{
					string methodName = MethodBase.GetCurrentMethod().Name;
					throw new Exception("in PhotosDAL." + methodName + "(): " + ex);
				}
			}

			return result;
		}

		public static int AddPhoto(Photo photo)
		{
			int newPhotoId = 0;
			const string query = @"insert into dbo.Photos (GroupId, UserId, AdId, FileNameWithExtension, IsMain, ModerateResult, Added, Changed, Width, Height, PhotoTypeId) 
values (@GroupId, @UserId, @AdId, @FileNameWithExtension, @IsMain, @ModerateResult, @Added, @Changed, @Width, @Height, @PhotoTypeId);
DECLARE @newPhotoId int;
   SELECT @newPhotoId = SCOPE_IDENTITY();
   SELECT @newPhotoId";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			if (photo.UserId != null)
				sqlCommand.Parameters.AddWithValue("UserId", photo.UserId);
			else
				sqlCommand.Parameters.AddWithValue("UserId", DBNull.Value);
			if (photo.AdId != null)
				sqlCommand.Parameters.AddWithValue("AdId", photo.AdId);
			else
				sqlCommand.Parameters.AddWithValue("AdId", DBNull.Value);
			sqlCommand.Parameters.AddWithValue("GroupId", photo.GroupId);
			sqlCommand.Parameters.AddWithValue("FileNameWithExtension", photo.FileNameWithExtension);
			sqlCommand.Parameters.AddWithValue("IsMain", photo.IsMain);
			sqlCommand.Parameters.AddWithValue("ModerateResult", (int)photo.ModerateResult);
			sqlCommand.Parameters.AddWithValue("Added", photo.Added);
			sqlCommand.Parameters.AddWithValue("Changed", photo.Changed);

			if (photo.Width != null)
				sqlCommand.Parameters.AddWithValue("Width", photo.Width);
			else
				sqlCommand.Parameters.AddWithValue("Width", DBNull.Value);

			if (photo.Height != null)
				sqlCommand.Parameters.AddWithValue("Height", photo.Height);
			else
				sqlCommand.Parameters.AddWithValue("Height", DBNull.Value);

			sqlCommand.Parameters.AddWithValue("PhotoTypeId", (int)photo.PhotoType);

			try
			{
				connect.Open();
				newPhotoId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				LogsDAL.AddMessage(1, "in PhotosDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newPhotoId;
		}

		public static bool DeletePhotoGroup(Guid groupId)
		{
			const string query = "delete from dbo.Photos where GroupId=@GroupId";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("GroupId", groupId);

			int result = 0;
			try
			{
				connect.Open();
				result = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in PhotosDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static void UpdateModerateResult(int groupId, int moderateResult)
		{
			string query = "update dbo.Photos set ModerateResult=@ModerateResult, Changed=@Changed where GroupId=@GroupId";

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);
			sqlCommand.Parameters.AddWithValue("GroupId", groupId);
			sqlCommand.Parameters.AddWithValue("ModerateResult", moderateResult);
			sqlCommand.Parameters.AddWithValue("Changed", DateTime.Now.ToUniversalTime());

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				LogsDAL.AddError("Exception in PhotosDAL.UpdateModerateResult(int photoId, int moderateResult): " + ex.ToString());
			}
			finally
			{
				connect.Close();
			}
		}

		public static void UpdateSize(int photoId, int width, int height)
		{
			string query = "update dbo.Photos set Width=@Width, Height=@Height, Changed=@Changed where Id=@Id";

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);
			sqlCommand.Parameters.AddWithValue("Id", photoId);
			sqlCommand.Parameters.AddWithValue("Width", width);
			sqlCommand.Parameters.AddWithValue("Height", height);
			sqlCommand.Parameters.AddWithValue("Changed", DateTime.Now.ToUniversalTime());

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				LogsDAL.AddError("Exception in PhotosDAL.UpdateSize(int photoId, int width, int height): " + ex.ToString());
			}
			finally
			{
				connect.Close();
			}
		}

		public static void UpdateIsMainGroup(Guid groupId, bool isMain)
		{
			string query = "update dbo.Photos set IsMain=@IsMain where GroupId=@GroupId";

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);
			sqlCommand.Parameters.AddWithValue("IsMain", isMain);
			sqlCommand.Parameters.AddWithValue("GroupId", groupId);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				LogsDAL.AddError("Exception in PhotosDAL.UpdateIsMain(int photoId, bool isMain): " + ex.ToString());
			}
			finally
			{
				connect.Close();
			}
		}
	}
}