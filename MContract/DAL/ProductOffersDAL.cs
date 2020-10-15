using MContract.Models;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Configuration;
using System.Drawing.Design;

namespace MContract.DAL
{
	public class ProductOffersDAL : BaseDataAccess
	{
		private static ProductOffer ReadProductOfferInfo(SqlDataReader reader)
		{
			var result = new ProductOffer
			{
				Id = (int)reader["Id"],
				OfferId = (int)reader["OfferId"],
				ProductId = (int)reader["ProductId"],
				PricePerWeight = float.Parse(reader["PricePerWeight"].ToString())
			};
			return result;
		}

		/*public static Offer GetOffer(int id)
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
				throw new Exception("in ProductOffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}*/
		public static List<ProductOffer> GetProductOffers(int offerId)
		{
			var result = new List<ProductOffer>();
			const string query = @"select * from dbo.ProductOffers where OfferId=@OfferId";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("OfferId", offerId);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var productOffer = ReadProductOfferInfo(reader);
					result.Add(productOffer);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in ProductOffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}
		public static List<ProductOffer> GetProductOffers(List<int> offerIds)
		{
			var result = new List<ProductOffer>();

			if (offerIds == null || !offerIds.Any())
				return result;

			string query = $"select * from dbo.ProductOffers where OfferId in ({string.Join(",", offerIds)})";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var productOffer = ReadProductOfferInfo(reader);
					result.Add(productOffer);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in ProductOffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}
		public static int AddProductOffer(ProductOffer productOffer)
		{
			int newProductOfferId = 0;
			const string query = @"insert into dbo.ProductOffers (OfferId, ProductId, PricePerWeight) 
values (@OfferId, @ProductId, @PricePerWeight);
DECLARE @newProductOfferID int;
   SELECT @newProductOfferID = SCOPE_IDENTITY();
   SELECT @newProductOfferID";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			var parameters = sqlCommand.Parameters;

			AddAddOrUpdateSqlParameters(parameters, productOffer);

			try
			{
				connect.Open();
				newProductOfferId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in ProductOffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newProductOfferId;
		}
		public static bool AddProductOffers(List<ProductOffer> productOffers)
		{
			if (!productOffers.Any())
				return true;
			int result = 0;
			var query = "";
			foreach (var productOffer in productOffers)
			{
				query += @"
insert into dbo.ProductOffers (OfferId, ProductId, PricePerWeight) 
values (" + 
productOffer.OfferId + ", " +
productOffer.ProductId + ", " +
productOffer.PricePerWeight.ToString().Replace(",", ".") + ")";
			}

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			var parameters = sqlCommand.Parameters;

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in ProductOffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		private static void AddAddOrUpdateSqlParameters(SqlParameterCollection parameters, ProductOffer productOffer)
		{
			parameters.AddWithValue("OfferId", productOffer.OfferId);
			parameters.AddWithValue("ProductId", productOffer.ProductId);
			parameters.AddWithValue("PricePerWeight", productOffer.PricePerWeight);
		}

		public static int UpdateProductOffer(ProductOffer productOffer)
		{
			const string query = "update dbo.ProductOffers set OfferId=@OfferId, ProductId=@ProductId, PricePerWeight=@PricePerWeight where Id=@Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", productOffer.Id);
			AddAddOrUpdateSqlParameters(sqlCommand.Parameters, productOffer);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in ProductOffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return productOffer.Id;
		}
		public static bool UpdateProductOffers(List<ProductOffer> productOffers)
		{
			if (!productOffers.Any())
				return true;
			string query = "";

			foreach (var productOffer in productOffers)
			{
				query += @"
update dbo.ProductOffers set " +
"OfferId=" + productOffer.OfferId + ", " +
"ProductId=" + productOffer.ProductId +", " +
"PricePerWeight=" + productOffer.PricePerWeight.ToString().Replace(",", ".") + " " +
"where Id=" + productOffer.Id;
			}

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
				throw new Exception("in ProductOffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return true;
		}
		public static bool DeleteProductOffer(int id)
		{
			const string query = @"delete from dbo.ProductOffers where Id = @Id";

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
				throw new Exception("in ProductOffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool DeleteProductOffers(int offerId)
		{
			string query = @"delete from dbo.ProductOffers where OfferId=" + offerId;

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
				throw new Exception("in ProductOffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool DeleteProductOffers(List<int> productOffersId)
		{
			if (!productOffersId.Any())
				return true;

			string query = @"delete from dbo.ProductOffers where Id in (" + string.Join(", ", productOffersId) + ")";

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
				throw new Exception("in ProductOffersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}
	}
}