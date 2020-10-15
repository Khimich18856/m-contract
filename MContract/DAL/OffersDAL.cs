using MContract.Models;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Configuration;

namespace MContract.DAL
{
	public class OffersDAL : BaseDataAccess
	{
		private static Offer ReadOfferInfo(SqlDataReader reader)
		{
			DateTime? modified = null;
			if (reader["Modified"] != DBNull.Value)
			{
				modified = (DateTime)reader["Modified"];
			}
			int? cityId = null;
			if (reader["CityId"] != DBNull.Value)
			{
				cityId = (int)reader["CityId"];
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
			string comment = null;
			if (reader["Comment"] != DBNull.Value)
			{
				comment = (string)reader["Comment"];
			}

			var result = new Offer
			{
				Id = (int)reader["Id"],
				SenderId = (int)reader["SenderId"],
				AdId = (int)reader["AdId"],
				DateOfPosting = (DateTime)reader["DateOfPosting"],
				Modified = modified,
				ActiveUntilDate = (DateTime)reader["ActiveUntilDate"],
				CityId = cityId,
				DeliveryType = (DeliveryTypes)(int)reader["DeliveryTypeId"],
				DeliveryAddress = deliveryAddress,
				DeliveryLoadType = (DeliveryLoadTypes)(int)reader["DeliveryLoadTypeId"],
				DeliveryWay = (DeliveryWays)(int)reader["DeliveryWayId"],
				TermsOfPayments = (TermsOfPayments)(int)reader["TermsOfPaymentsId"],
				DefermentPeriod = defermentPeriod,
				//FormOfPayment = (FormOfPayment)(int)reader["FormOfPayment"],
				Nds = (Nds)(int)reader["Nds"],
				Comment = comment,
				ModerateResult = (ModerateResults)(int)reader["ModerateResultId"],
				OfferStatus = (OfferStatuses)(int)reader["OfferStatusId"],
				ContractStatus = (ContractStatuses)(int)reader["ContractStatusId"],
				ShowInDealsHistory = (bool)reader["ShowInDealsHistory"]
			};

			if (reader["ContractSendDate"] != DBNull.Value)
				result.ContractSendDate = (DateTime)reader["ContractSendDate"];

			return result;
		}

		public static Offer GetOffer(int id)
		{
			Offer result = null;
			const string query = @"select * from dbo.Offers where Id=@Id";
			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Id", id);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result = ReadOfferInfo(reader);

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		
		public static List<Offer> GetOffers(List<int> adIds = null, int? senderId = null, int? offerStatusId = null, int? contractStatusId = null, int? adId = null)
		{
			var result = new List<Offer>();
			string query = @"select * from dbo.Offers where 1 = 1";

			if (adIds != null && adIds.Any())
				query += " and AdId in (" + string.Join(", ", adIds) + ")";

			if (adId.HasValue)
				query += " and AdId = " + adId;

			if (senderId.HasValue)
				query += " and SenderId = " + senderId.Value;

			if (offerStatusId.HasValue)
				query += " and OfferStatusId = " + offerStatusId.Value;

			if (contractStatusId.HasValue)
				query += " and ContractStatusId = " + contractStatusId.Value;

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var offer = ReadOfferInfo(reader);
					result.Add(offer);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<Offer> GetOffers(int adId)
		{
			var result = new List<Offer>();
			const string query = @"select * from dbo.Offers where AdId=@AdId";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("AdId", adId);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var offer = ReadOfferInfo(reader);
					result.Add(offer);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<Offer> GetOffersFromUser(int senderId)
		{
			var result = new List<Offer>();
			const string query = @"select * from dbo.Offers where SenderId=@SenderId";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("SenderId", senderId);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var offer = ReadOfferInfo(reader);
					result.Add(offer);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<Offer> GetOffersForModeration()
		{
			var result = new List<Offer>();
			string query = @"select * from dbo.Offers where ModerateResultId=" + (int)ModerateResults.NotChecked;

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var offer = ReadOfferInfo(reader);
					result.Add(offer);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		/// <summary>
		/// Получает истекшие предложения по активным объявления, по которым нужно отправить сообщение, что предложение истекло
		/// </summary>
		/// <returns></returns>
		public static List<Offer> GetExpiredOffersForActiveAdsForSendMessage()
		{
			var result = new List<Offer>();
			string query =
@"select o.* from dbo.Offers o 
			 join dbo.Ads a on a.Id = o.AdId and a.AdStatusId = " + (int)AdStatuses.Published + @" and a.ActiveToDate > @Now
             where o.OfferStatusId in (" + (int)OfferStatuses.Published + ", " + (int)OfferStatuses.Expired + @") 
					and o.ActiveUntilDate < @Now and o.IsSendedExpiredMessage = 0";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Now", DateTime.Now.ToUniversalTime());

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var offer = ReadOfferInfo(reader);
					result.Add(offer);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}


		public static int AddOffer(Offer offer)
		{
			int newOfferId = 0;
			const string query = 
"insert into dbo.Offers (SenderId, AdId, DateOfPosting, Modified, ActiveUntilDate, CityId, DeliveryTypeId, " +
@"DeliveryAddress, DeliveryLoadTypeId, DeliveryWayId, TermsOfPaymentsId, DefermentPeriod, Nds, Comment, ModerateResultId, OfferStatusId, ContractStatusId, ShowInDealsHistory) 
values (@SenderId, @AdId, @DateOfPosting, @Modified, @ActiveUntilDate, @CityId, @DeliveryTypeId, @DeliveryAddress, @DeliveryLoadTypeId, " +
@"@DeliveryWayId, @TermsOfPaymentsId, @DefermentPeriod, @Nds, @Comment, @ModerateResultId, @OfferStatusId, @ContractStatusId, @ShowInDealsHistory);
DECLARE @newOfferID int;
   SELECT @newOfferID = SCOPE_IDENTITY();
   SELECT @newOfferID";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			var parameters = sqlCommand.Parameters;

			AddAddOrUpdateSqlParameters(parameters, offer);

			try
			{
				connect.Open();
				newOfferId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newOfferId;
		}

		private static void AddAddOrUpdateSqlParameters(SqlParameterCollection parameters, Offer offer)
		{
			parameters.AddWithValue("SenderId", offer.SenderId);
			parameters.AddWithValue("AdId", offer.AdId);
			if (offer.DateOfPosting != null)
			{
				parameters.AddWithValue("DateOfPosting", offer.DateOfPosting);
			}
			if (offer.Modified != null)
				parameters.AddWithValue("Modified", offer.Modified);
			else
				parameters.AddWithValue("Modified", DBNull.Value);
			parameters.AddWithValue("ActiveUntilDate", offer.ActiveUntilDate);
			if (offer.CityId != null)
				parameters.AddWithValue("CityId", offer.CityId);
			else
				parameters.AddWithValue("CityId", DBNull.Value);
			parameters.AddWithValue("DeliveryTypeId", offer.DeliveryType);
			if (!String.IsNullOrEmpty(offer.DeliveryAddress))
			{
				parameters.AddWithValue("DeliveryAddress", offer.DeliveryAddress);
			}
			else
			{
				parameters.AddWithValue("DeliveryAddress", DBNull.Value);
			}
			parameters.AddWithValue("DeliveryLoadTypeId", offer.DeliveryLoadType);
			parameters.AddWithValue("DeliveryWayId", offer.DeliveryWay);
			parameters.AddWithValue("TermsOfPaymentsId", offer.TermsOfPayments);
			if (offer.DefermentPeriod != null)
			{
				parameters.AddWithValue("DefermentPeriod", offer.DefermentPeriod);
			}
			else
			{
				parameters.AddWithValue("DefermentPeriod", DBNull.Value);
			}
			parameters.AddWithValue("Nds", offer.Nds);
			if (offer.Comment != null)
			{
				parameters.AddWithValue("Comment", offer.Comment);
			}
			else
			{
				parameters.AddWithValue("Comment", DBNull.Value);
			}
			parameters.AddWithValue("ModerateResultId", offer.ModerateResult);
			parameters.AddWithValue("OfferStatusId", offer.OfferStatus);
			parameters.AddWithValue("ContractStatusId", offer.ContractStatus);
			parameters.AddWithValue("ShowInDealsHistory", offer.ShowInDealsHistory);
		}

		public static int UpdateOffer(Offer offer)
		{
			const string query = "update dbo.Offers set SenderId=@SenderId, AdId=@AdId, Modified=@Modified, ActiveUntilDate=@ActiveUntilDate, CityId=@CityId, " +
				"DeliveryTypeId=@DeliveryTypeId, DeliveryAddress=@DeliveryAddress, DeliveryLoadTypeId=@DeliveryLoadTypeId, " +
				"DeliveryWayId=@DeliveryWayId, TermsOfPaymentsId=@TermsOfPaymentsId, DefermentPeriod=@DefermentPeriod, Nds=@Nds, " +
				"Comment=@Comment, ModerateResultId=@ModerateResultId, OfferStatusId=@OfferStatusId, ContractStatusId=@ContractStatusId where Id=@Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", offer.Id);
			AddAddOrUpdateSqlParameters(sqlCommand.Parameters, offer);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return offer.Id;
		}
		public static bool ChangeOfferContractStatus(int offerId, int contractStatusId)
		{
			string query = "update dbo.Offers set ContractStatusId=" + contractStatusId + " where Id=" + offerId;

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
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return true;
		}
		public static bool UpdateOfferModerateResult(int offerId, int moderateResultId)
		{
			const string query = "update dbo.Offers set ModerateResultId=@ModerateResultId where Id=@Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", offerId);
			sqlCommand.Parameters.AddWithValue("ModerateResultId", moderateResultId);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return true;
		}

		public static bool UpdateOfferContractSendDate(int offerId, DateTime contractSendDate)
		{
			const string query = "update dbo.Offers set ContractSendDate=@ContractSendDate where Id=@Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", offerId);
			sqlCommand.Parameters.AddWithValue("ContractSendDate", contractSendDate);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return true;
		}

		public static bool UpdateOfferSetIsSendedExpiredMessage(int offerId, bool isSendedExpiredMessage)
		{
			const string query = "update dbo.Offers set IsSendedExpiredMessage=@IsSendedExpiredMessage where Id=@Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", offerId);
			sqlCommand.Parameters.AddWithValue("IsSendedExpiredMessage", isSendedExpiredMessage);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return true;
		}

		public static bool UpdateOfferSetIsSendedExpiredMessage(List<int> offerIds, bool isSendedExpiredMessage)
		{
			string query = "update dbo.Offers set IsSendedExpiredMessage=@IsSendedExpiredMessage where Id in (" + String.Join(", ", offerIds) + ")";

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
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return true;
		}
		public static bool UpdateOfferActiveUntilDate(int id, DateTime date)
		{
			const string query = @"update dbo.Offers set ActiveUntilDate=@ActiveUntilDate, Modified=@Modified where Id=@Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", id);
			sqlCommand.Parameters.AddWithValue("ActiveUntilDate", date);
			sqlCommand.Parameters.AddWithValue("Modified", DateTime.Now.ToUniversalTime());

			int result = 0;
			try
			{
				connect.Open();
				result = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool DeleteOffer(int id)
		{
			const string query = @"delete from dbo.Offers where Id = @Id";

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
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool UpdateOfferDoNotShowInDealsHistory(int offerId)
		{
			const string query = @"update dbo.Offers set ShowInDealsHistory=0 where Id=@offerId";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("OfferId", offerId);

			int result = 0;
			try
			{
				connect.Open();
				result = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool UpdateNotFinishedOffersToDeleted(int senderId)
		{
			string query = $"update dbo.Offers set OfferStatusId = {(int)OfferStatuses.Deleted} where SenderId = {senderId} and OfferStatusId in ({(int)OfferStatuses.Draft}, {(int)OfferStatuses.Published}, {(int)OfferStatuses.Expired})";

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
				throw new Exception("in OffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}
	}
}