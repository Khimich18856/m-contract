using MContract.Models;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MContract.DAL
{
    public class AdsDAL : BaseDataAccess
	{
		private static Ad ReadAdInfo(SqlDataReader reader)
		{
			string description = null;
			if (reader["Description"] != DBNull.Value)
			{
				description = ((string)reader["Description"]).Trim();
			}
			DateTime? activeToDate = null;
			if (reader["ActiveToDate"] != DBNull.Value)
			{
				activeToDate = (DateTime)reader["ActiveToDate"];
			}
			string deliveryAddress = null;
			if (reader["DeliveryAddress"] != DBNull.Value)
			{
				deliveryAddress = (string)reader["DeliveryAddress"];
			}
			int? defermentPeriod = null;
			if (reader["DefermentPeriod"] != DBNull.Value)
			{
				defermentPeriod = (int)reader["DefermentPeriod"];
			}
			/*List<DeliveryTypes> deliveryType = new List<DeliveryTypes>();
			var deliveryTypeRead = ((string)reader["DeliveryTypeId"]).Split(',');
			foreach (var item in deliveryTypeRead)
			{
				deliveryType.Add((DeliveryTypes)(Convert.ToInt32(item.Trim())));
			}
			List<DeliveryLoadTypes> deliveryLoadType = new List<DeliveryLoadTypes>();
			var deliveryLoadTypeRead = ((string)reader["DeliveryLoadTypeId"]).Split(',');
			foreach (var item in deliveryLoadTypeRead)
			{
				deliveryLoadType.Add((DeliveryLoadTypes)(Convert.ToInt32(item.Trim())));
			}
			List<DeliveryWays> deliveryWay = new List<DeliveryWays>();
			var deliveryWayRead = ((string)reader["DeliveryWayId"]).Split(',');
			foreach (var item in deliveryWayRead)
			{
				deliveryWay.Add((DeliveryWays)(Convert.ToInt32(item.Trim())));
			}
			List<bool> nds = new List<bool>();
			var ndsRead = ((string)reader["nds"]).Split(',');
			foreach (var item in ndsRead)
			{
				nds.Add(bool.Parse(item.Trim()));
			}*/
			var result = new Ad
			{
				Id = (int)reader["Id"],
				SenderId = (int)reader["SenderId"],
				DateOfPosting = (DateTime)reader["DateOfPosting"],
				ActiveToDate = activeToDate,
				Description = description,
				CityId = (int)reader["CityId"],
				//DeliveryType = deliveryType,
				//DeliveryLoadType = deliveryLoadType,
				//DeliveryWay = deliveryWay,
				DeliveryType = (DeliveryTypes)(int)reader["DeliveryTypeId"],
				DeliveryAddress = deliveryAddress,
				DeliveryLoadType = (DeliveryLoadTypes)(int)reader["DeliveryLoadTypeId"],
				DeliveryWay = (DeliveryWays)(int)reader["DeliveryWayId"],
				TermsOfPayments = (TermsOfPayments)(int)reader["TermsOfPaymentsId"],
				DefermentPeriod = defermentPeriod,
				//FormOfPayment = (FormOfPayment)(int)reader["FormOfPayment"],
				//Nds = nds,
				Nds = (Nds)(int)reader["Nds"],
				PartOffersAllowed = (bool)reader["PartOffersAllowed"],
				OffersVisibleToOtherUsers = (bool)reader["OffersVisibleToOtherUsers"],
				ViewsCount = (int)reader["ViewsCount"],
				AdStatus = (AdStatuses)(int)reader["AdStatusId"],
				IsBuy = (bool)reader["IsBuy"],
				AvailableForAllUsers = (bool)reader["AvailableForAllUsers"],
				ModerateResult = (ModerateResults)reader["ModerateResultId"],
				ShowInDealsHistory = (bool)reader["ShowInDealsHistory"],
				IsUnpublishedByUser = (bool)reader["IsUnpublishedByUser"]
			};
			return result;
		}

		public static Ad GetAd(int id)
		{
			Ad result = null;
			const string query = @"select * from dbo.Ads where Id=@Id";
			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Id", id);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result = ReadAdInfo(reader);

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<Ad> GetAds(List<int> ids)
		{
			var result = new List<Ad>();

			if (ids == null || !ids.Any())
				return result;

			var idsString = string.Join(",", ids);
			string query = $@"select * from dbo.Ads where AdStatusId <> {(int)MContract.Models.Enums.AdStatuses.Deleted} and Id in ({idsString})";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var ad = ReadAdInfo(reader);
					result.Add(ad);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<Ad> GetAds(int? adStatusId = (int)AdStatuses.Published, int? senderId = null, List<int> adStatusIds = null, bool? isSendedExpiredMessage = null, 
										List<int> ids = null, bool? isUnpublishedByUser = null)
		{
			var result = new List<Ad>();
			string query = $@"select * from dbo.Ads where AdStatusId != {(int)AdStatuses.Deleted}";
			if (adStatusIds != null)
			{
				if (!adStatusIds.Any())
					return result;

				query += " and AdStatusId in (" + String.Join(", ", adStatusIds) + ")";
			}
			else if (adStatusId != null)
			{
				if (adStatusId == (int)AdStatuses.Published)
					query += " and AdStatusId=" + (int)AdStatuses.Published + " and ActiveToDate > @Now";
				else if (adStatusId == (int)AdStatuses.Expired)
					query += " and AdStatusId=" + (int)AdStatuses.Published + " and ActiveToDate < @Now";
				else
					query += " and AdStatusId=" + adStatusId;
			}
			if (senderId != null)
				query += " and SenderId=" + senderId;

			if (isSendedExpiredMessage.HasValue)
				query += " and IsSendedExpiredMessage = " + (isSendedExpiredMessage.Value ? 1 : 0);

			if (ids != null)
			{
				if (!ids.Any())
					return result;

				query += " and Id in (" + String.Join(", ", ids) + ")";
			}

			if (isUnpublishedByUser.HasValue)
				query += " and IsUnpublishedByUser = " + (isUnpublishedByUser.Value ? 1 : 0);

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Now", DateTime.Now.ToUniversalTime());

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var ad = ReadAdInfo(reader);
					result.Add(ad);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static int GetAdsCount(int? adStatusId = (int)AdStatuses.Published, int? senderId = null, List<int> adStatusIds = null, bool? isSendedExpiredMessage = null)
		{
			int result = 0;
			string query = $@"select count(*) from dbo.Ads where AdStatusId != {(int)MContract.Models.Enums.AdStatuses.Deleted}";
			if (adStatusIds != null)
			{
				if (adStatusIds.Any())
					return result;

				query += " and AdStatusId in (" + String.Join(", ", adStatusIds) + ")";
			}
			else if (adStatusId != null)
			{
				if (adStatusId == (int)AdStatuses.Published)
					query += " and AdStatusId=" + (int)AdStatuses.Published + " and ActiveToDate > @Now";
				else if (adStatusId == (int)AdStatuses.Expired)
					query += " and AdStatusId=" + (int)AdStatuses.Published + " and ActiveToDate < @Now";
				else
					query += " and AdStatusId=" + adStatusId;
			}
			if (senderId != null)
				query += " and SenderId=" + senderId;

			if (isSendedExpiredMessage.HasValue)
				query += " and IsSendedExpiredMessage = " + (isSendedExpiredMessage.Value ? 1 : 0);

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Now", DateTime.Now.ToUniversalTime());

			try
			{
				connection.Open();
				result = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<Ad> GetAdsForSearchPage(int? adStatusId, List<int> categoriesId = null, List<int> citiesId = null, bool? isBuy = null)
		{
			var result = new List<Ad>();
			string query = "";
			if (categoriesId != null && categoriesId.Any())
			{
				query += @"select ap.ProductCategoryId, ap.AdId, Ads.*
							from dbo.AdProducts as ap
							join dbo.Ads on ap.AdId=Ads.Id and ap.ProductCategoryId in (" + string.Join(", ", categoriesId) + ")";
			}
			else
				query += @"select * from dbo.Ads where 1=1";

			if (citiesId != null)
				query += " and " + (categoriesId != null && categoriesId.Any() ? "Ads." : "") + "CityId in (" + string.Join(", ", citiesId) + ")";

			if (isBuy != null)
				query += " and " + (categoriesId != null && categoriesId.Any() ? "Ads." : "") + "IsBuy=" + (isBuy.Value ? "1" : "0");

			if (adStatusId == (int)AdStatuses.Published)
				query += " and " + (categoriesId != null && categoriesId.Any() ? "Ads." : "") + "AdStatusId=" + (int)AdStatuses.Published +
						 " and " + (categoriesId != null && categoriesId.Any() ? "Ads." : "") + "ActiveToDate > @DateTimeNow";
			else if (adStatusId == (int)AdStatuses.Expired)
				query += " and " + (categoriesId != null && categoriesId.Any() ? "Ads." : "") + "AdStatusId=" + (int)AdStatuses.Published +
						 " and " + (categoriesId != null && categoriesId.Any() ? "Ads." : "") + "ActiveToDate < @DateTimeNow";
			else
				query += " and " + (categoriesId != null && categoriesId.Any() ? "Ads." : "") + "AdStatusId=" + adStatusId;

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("DateTimeNow", DateTime.Now);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var ad = ReadAdInfo(reader);
					result.Add(ad);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}
			return result;
		}

		public static List<Ad> GetAdsForModeration()
		{
			var result = new List<Ad>();
			string query = @"select * from dbo.Ads where ModerateResultId=" + (int)ModerateResults.NotChecked;

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var ad = ReadAdInfo(reader);
					result.Add(ad);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static int AddAd(Ad ad)
		{
			int newAdId = 0;
			const string query = @"insert into dbo.Ads (SenderId, DateOfPosting, ActiveToDate, Description, CityId, DeliveryTypeId, DeliveryAddress, DeliveryLoadTypeId, DeliveryWayId, TermsOfPaymentsId, DefermentPeriod, Nds, PartOffersAllowed, OffersVisibleToOtherUsers, ViewsCount, AdStatusId, IsBuy, AvailableForAllUsers, ModerateResultId, IsSendedExpiredMessage, ShowInDealsHistory) 
values (@SenderId, @DateOfPosting, @ActiveToDate, @Description, @CityId, @DeliveryTypeId, @DeliveryAddress, @DeliveryLoadTypeId, @DeliveryWayId, @TermsOfPaymentsId, @DefermentPeriod, @Nds, @PartOffersAllowed, @OffersVisibleToOtherUsers, @ViewsCount, @AdStatusId, @IsBuy, @AvailableForAllUsers, @ModerateResultId, @IsSendedExpiredMessage, @ShowInDealsHistory);
DECLARE @newAdID int;
   SELECT @newAdID = SCOPE_IDENTITY();
   SELECT @newAdID";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			var parameters = sqlCommand.Parameters;

			AddAddOrUpdateSqlParameters(parameters, ad);

			try
			{
				connect.Open();
				newAdId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newAdId;
		}

		private static void AddAddOrUpdateSqlParameters(SqlParameterCollection parameters, Ad ad)
		{
			parameters.AddWithValue("SenderId", ad.SenderId);
			if (ad.DateOfPosting == null)
			{
				parameters.AddWithValue("DateOfPosting", DBNull.Value);
			}
			else
			{
				parameters.AddWithValue("DateOfPosting", ad.DateOfPosting);
			}
			if (ad.ActiveToDate == null)
			{
				parameters.AddWithValue("ActiveToDate", DBNull.Value);
			}
			else
			{
				parameters.AddWithValue("ActiveToDate", ad.ActiveToDate);
			}
			if (ad.Description == null)
			{
				parameters.AddWithValue("Description", DBNull.Value);
			}
			else
			{
				parameters.AddWithValue("Description", ad.Description);
			}
			parameters.AddWithValue("CityId", ad.CityId);
			/*if (ad.CityId == null)
			{
				parameters.AddWithValue("CityId", DBNull.Value);
			} else
			{
				parameters.AddWithValue("CityId", ad.CityId);
			}
			List<int> deliveryType = new List<int>();
			foreach (var item in ad.DeliveryType)
			{
				deliveryType.Add((int)item);
			}
			List<int> deliveryLoadType = new List<int>();
			foreach (var item in ad.DeliveryLoadType)
			{
				deliveryLoadType.Add((int)item);
			}
			List<int> deliveryWay = new List<int>();
			foreach (var item in ad.DeliveryWay)
			{
				deliveryWay.Add((int)item);
			}
			parameters.AddWithValue("DeliveryTypeId", string.Join(",", deliveryType));
			parameters.AddWithValue("DeliveryLoadTypeId", string.Join(",", deliveryLoadType));
			parameters.AddWithValue("DeliveryWayId", string.Join(",", deliveryWay));*/
			parameters.AddWithValue("DeliveryTypeId", ad.DeliveryType);
			if (ad.DeliveryAddress == null)
			{
				parameters.AddWithValue("DeliveryAddress", DBNull.Value);
			}
			else
			{
				parameters.AddWithValue("DeliveryAddress", ad.DeliveryAddress);
			}
			parameters.AddWithValue("DeliveryLoadTypeId", ad.DeliveryLoadType);
			parameters.AddWithValue("DeliveryWayId", ad.DeliveryWay);
			if (ad.DefermentPeriod == null)
			{
				parameters.AddWithValue("DefermentPeriod", DBNull.Value);
			}
			else
			{
				parameters.AddWithValue("DefermentPeriod", ad.DefermentPeriod);
			}
			parameters.AddWithValue("TermsOfPaymentsId", ad.TermsOfPayments);
			//parameters.AddWithValue("FormOfPaymentId", ad.FormOfPayment);
			//parameters.AddWithValue("Nds", string.Join(",", ad.Nds));
			parameters.AddWithValue("Nds", ad.Nds);
			parameters.AddWithValue("PartOffersAllowed", ad.PartOffersAllowed);
			parameters.AddWithValue("OffersVisibleToOtherUsers", ad.OffersVisibleToOtherUsers);
			parameters.AddWithValue("ViewsCount", ad.ViewsCount);
			parameters.AddWithValue("AdStatusId", ad.AdStatus);
			parameters.AddWithValue("IsBuy", ad.IsBuy);
			parameters.AddWithValue("AvailableForAllUsers", ad.AvailableForAllUsers);
			parameters.AddWithValue("ModerateResultId", ad.ModerateResult);
			parameters.AddWithValue("IsSendedExpiredMessage", ad.IsSendedExpiredMessage);
			parameters.AddWithValue("ShowInDealsHistory", ad.ShowInDealsHistory);
		}

		public static int UpdateAd(Ad ad)
		{
			const string query = "update dbo.Ads set SenderId=@SenderId, DateOfPosting=@DateOfPosting, ActiveToDate=@ActiveToDate, Description=@Description, CityId=@CityId, " +
				"DeliveryTypeId=@DeliveryTypeId, DeliveryAddress=@DeliveryAddress, DeliveryLoadTypeId=@DeliveryLoadTypeId, DeliveryWayId=@DeliveryWayId, TermsOfPaymentsId=@TermsOfPaymentsId, DefermentPeriod=@DefermentPeriod, " +
				"Nds=@Nds, PartOffersAllowed=@PartOffersAllowed, OffersVisibleToOtherUsers=@OffersVisibleToOtherUsers, ViewsCount=@ViewsCount, AdStatusId=@AdStatusId, " +
				"IsBuy=@IsBuy, AvailableForAllUsers=@AvailableForAllUsers, ModerateResultId=@ModerateResultId, IsSendedExpiredMessage=@IsSendedExpiredMessage where Id=@Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", ad.Id);
			AddAddOrUpdateSqlParameters(sqlCommand.Parameters, ad);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return ad.Id;
		}

		public static bool ChangeAdStatus(int adId, int adStatusId)
		{
			string query = "update dbo.Ads set AdStatusId=" + adStatusId + " where Id=" + adId;

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return true;
		}

		public static bool ChangeAdStatusToFinishedAndActiveUntilDateToNow(int adId)
		{
			string query = "update dbo.Ads set AdStatusId=@AdStatusId, ActiveToDate=@ActiveUntilDate where Id=@Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			sqlCommand.Parameters.AddWithValue("AdStatusId", (int)AdStatuses.Finished);
			sqlCommand.Parameters.AddWithValue("ActiveUntilDate", DateTime.Now.ToUniversalTime());
			sqlCommand.Parameters.AddWithValue("Id", adId);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();

			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return true;
		}

		public static bool UpdateAdModerateResult(int adId, int moderateResultId)
		{
			const string query = "update dbo.Ads set ModerateResultId=@ModerateResultId where Id=@Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", adId);
			sqlCommand.Parameters.AddWithValue("ModerateResultId", moderateResultId);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return true;
		}

		public static bool UpdateAdSetIsSendedExpiredMessage(int adId, bool isSendedExpiredMessage)
		{
			const string query = "update dbo.Ads set IsSendedExpiredMessage=@IsSendedExpiredMessage where Id=@Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", adId);
			sqlCommand.Parameters.AddWithValue("IsSendedExpiredMessage", isSendedExpiredMessage);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return true;
		}

		public static bool UpdateAdSetIsSendedExpiredMessage(List<int> adIds, bool isSendedExpiredMessage)
		{
			string query = "update dbo.Ads set IsSendedExpiredMessage=@IsSendedExpiredMessage where Id in (" + String.Join(", ", adIds) + ")";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("IsSendedExpiredMessage", isSendedExpiredMessage);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return true;
		}

		public static void UpdateAdAddOneView(int adId)
		{
			string query = @"update dbo.Ads set ViewsCount = ViewsCount + 1
                                where Id=@Id";

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", adId);
			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				LogsDAL.AddError("Exception in ForumThemesDAL." + methodName + "(...): " + ex.ToString());
			}
			finally
			{
				connect.Close();
			}
		}

		public static bool DeleteAd(int id)
		{
			//const string query = @"delete from dbo.Ads where Id = @Id";
			string query = $@"update dbo.Ads set AdStatusId={(int)MContract.Models.Enums.AdStatuses.Deleted} where Id=@Id";

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
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool DeleteAds(List<int> ids)
		{
			string query = @"delete from dbo.Ads where Id in (" + string.Join(", ", ids) + ")";

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
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static int AddInvitedUser(int adId, int userId)
		{
			int newInvitedUserEntryId = 0;
			const string query = @"insert into dbo.AdsInvitedUsers (AdId, UserId) 
values (@AdId, @UserId);
DECLARE @newInvitedUserEntryId int;
   SELECT @newInvitedUserEntryId = SCOPE_IDENTITY();
   SELECT @newInvitedUserEntryId";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			var parameters = sqlCommand.Parameters;
			parameters.AddWithValue("AdId", adId);
			parameters.AddWithValue("UserId", userId);

			try
			{
				connect.Open();
				newInvitedUserEntryId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newInvitedUserEntryId;
		}

		public static bool AddInvitedUsers(int adId, List<int> usersId)
		{
			if (!usersId.Any())
				return true;
			var result = 0;
			string query = "";
			//var i = 0;
			foreach (var userId in usersId)
			{
				query += @"
insert into dbo.AdsInvitedUsers (AdId, UserId) 
values (" + adId + ", " + userId + ")";
			}

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			//var parameters = sqlCommand.Parameters;

			try
			{
				connect.Open();
				sqlCommand.ExecuteScalar();
				result++;
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static List<int> GetInvitedUsers(int adId)
		{
			var result = new List<int>();
			const string query = @"select * from dbo.AdsInvitedUsers where AdId=@AdId";
			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("AdId", adId);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					result.Add((int)reader["UserId"]);
				}

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<int> GetAdsUserIsInvitedTo(int userId)
		{
			var result = new List<int>();
			const string query = @"select * from dbo.AdsInvitedUsers where UserId=@UserId";
			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("UserId", userId);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result.Add((int)reader["AdId"]);

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static bool DeleteInvitedUser(int adId, int userId)
		{
			const string query = @"delete from dbo.AdsInvitedUsers where AdId=@AdId and UserId=@UserId";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("AdId", adId);
			sqlCommand.Parameters.AddWithValue("UserId", userId);

			int result = 0;
			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
				result++;
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool DeleteInvitedUsers(int adId, List<int> userId)
		{
			if (!userId.Any())
				return true;
			string query = @"delete from dbo.AdsInvitedUsers where AdId=" + adId + " and (UserId=" + string.Join(" or UserId=", userId) + ")";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			int result = 0;
			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
				result++;
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static int AddFavoriteAd(int userId, int adId)
		{
			int newFavoriteAdEntryId = 0;
			const string query = @"insert into dbo.UsersFavoriteAds (UserId, AdId) 
values (@UserId, @AdId);
DECLARE @newFavoriteAdEntryId int;
   SELECT @newFavoriteAdEntryId = SCOPE_IDENTITY();
   SELECT @newFavoriteAdEntryId";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			var parameters = sqlCommand.Parameters;
			parameters.AddWithValue("UserId", userId);
			parameters.AddWithValue("AdId", adId);

			try
			{
				connect.Open();
				newFavoriteAdEntryId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newFavoriteAdEntryId;
		}

		public static List<Ad> GetFavoriteAds(int userId)
		{
			var result = new List<Ad>();
			string query = $@"select * from Ads where Id in 
									(select AdId from UsersFavoriteAds where UserId = {userId})";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("UserId", userId);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var ad = ReadAdInfo(reader);
					result.Add(ad);
				}

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		

		public static int GetFavoritesCount(int adId)
		{
			int result = 0;
			string query = "select count(*) as FavoritesCount from dbo.UsersFavoriteAds where AdId = " + adId;
			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result = (int)reader["FavoritesCount"];

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static bool IsInFavorites(int userId, int adId)
		{
			bool result = false;
			string query = $"select * from dbo.UsersFavoriteAds where UserId = {userId} and AdId = {adId}";
			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result = true;

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static bool DeleteFavoriteAd(int userId, int adId)
		{
			const string query = @"delete from dbo.UsersFavoriteAds where UserId=@UserId and AdId=@AdId";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("UserId", userId);
			sqlCommand.Parameters.AddWithValue("AdId", adId);

			int result = 0;
			try
			{
				connect.Open();
				result = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool UpdateAdDoNotShowInDealsHistory(int adId)
		{
			const string query = @"update dbo.Ads set ShowInDealsHistory=0 where Id=@adId";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("AdId", adId);

			int result = 0;
			try
			{
				connect.Open();
				result = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}
		public static bool UpdateAdActiveUntilDate(int id, DateTime date, bool isUnpublishedByUser)
		{
			const string query = @"update dbo.Ads set ActiveToDate=@ActiveUntilDate, IsUnpublishedByUser=@IsUnpublishedByUser where Id=@Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", id);
			sqlCommand.Parameters.AddWithValue("ActiveUntilDate", date);
			sqlCommand.Parameters.AddWithValue("IsUnpublishedByUser", isUnpublishedByUser);

			int result = 0;
			try
			{
				connect.Open();
				result = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool UpdateNotFinishedAdsToDeleted(int senderId)
		{
			string query = $"update dbo.Ads set AdStatusId={(int)AdStatuses.Deleted} where SenderId = {senderId} and AdStatusId in ({(int)AdStatuses.Draft}, {(int)AdStatuses.Published}, {(int)AdStatuses.Expired})";

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
				throw new Exception("in AdsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}
	}
}