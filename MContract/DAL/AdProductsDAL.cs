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
	public class AdProductsDAL : BaseDataAccess
	{
		private static AdProduct ReadAdProductInfo(SqlDataReader reader)
		{
			var result = new AdProduct
			{
				Id = (int)reader["Id"],
				AdId = (int)reader["AdId"],
				ProductCategoryId = (int)reader["ProductCategoryId"],
				Currency = (Currencies)(int)reader["CurrencyId"]
			};
			if (reader["Weight"] != DBNull.Value)
			{
				result.Weight = Convert.ToSingle(reader["Weight"].ToString());
			}
			if (reader["PricePerWeight"] != DBNull.Value)
			{
				result.PricePerWeight = Convert.ToSingle(reader["PricePerWeight"].ToString());
			}
			if (reader["Name"] != DBNull.Value)
			{
				result.Name = ((string)reader["Name"]).Trim();
			}
			return result;
		}

		public static AdProduct GetAdProduct(int id)
		{
			AdProduct result = null;
			const string query = @"select * from dbo.AdProducts where Id=@Id";
			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Id", id);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result = ReadAdProductInfo(reader);

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdProductsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<AdProduct> GetAdProducts()
		{
			var result = new List<AdProduct>();
			const string query = "select * from dbo.AdProducts";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var product = ReadAdProductInfo(reader);
					result.Add(product);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdProductsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<AdProduct> GetAdProducts(int? adId = null, List<int> adIds = null)
		{
			var result = new List<AdProduct>();
			string query = "select * from dbo.AdProducts where 1 = 1 ";

			if (adId.HasValue)
				query += " and AdId = " + adId.Value;

			if (adIds != null && adIds.Any())
				query += " and AdId in (" + string.Join(", ", adIds) + ")";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var product = ReadAdProductInfo(reader);
					result.Add(product);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdProductsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<AdProduct> GetAdProductsByCategory(int categoryId)
		{
			var result = new List<AdProduct>();
			string query = "select * from dbo.AdProducts where ProductCategoryId = " + categoryId;

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var product = ReadAdProductInfo(reader);
					result.Add(product);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdProductsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static int AddAdProduct(AdProduct product)
		{
			int newAdProductId = 0;
			const string query = @"insert into dbo.AdProducts (Name, AdId, ProductCategoryId, Weight, PricePerWeight, CurrencyId) 
values (@Name, @AdId, @ProductCategoryId, @Weight, @PricePerWeight, @CurrencyId);
DECLARE @newAdProductID int;
   SELECT @newAdProductID = SCOPE_IDENTITY();
   SELECT @newAdProductID";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			var parameters = sqlCommand.Parameters;

			AddAddOrUpdateSqlParameters(parameters, product);

			try
			{
				connect.Open();
				newAdProductId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdProductsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newAdProductId;
		}

		public static bool AddAdProducts(List<AdProduct> products)
		{
			if (!products.Any())
				return true;
			var result = 0;
			string query = "";
			var i = 0;
			foreach (var product in products)
			{
				query += @"
insert into dbo.AdProducts (Name, AdId, ProductCategoryId, Weight, PricePerWeight, CurrencyId) 
values (
@Name" + i + ", " + 
product.AdId + ", " + 
product.ProductCategoryId + ", " + 
product.Weight.ToString().Replace(",", ".") + ", " + 
product.PricePerWeight.ToString().Replace(",", ".") + ", " + 
(int)product.Currency + ")";
				i++;
			}

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			i = 0;
			foreach (var product in products)
			{
				if (product.Name != null)
					sqlCommand.Parameters.AddWithValue("Name" + i, product.Name);
				else
					sqlCommand.Parameters.AddWithValue("Name" + i, DBNull.Value);
				i++;
			}

			try
			{
				connect.Open();
				sqlCommand.ExecuteScalar();
				result++;
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdProductsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		private static void AddAddOrUpdateSqlParameters(SqlParameterCollection parameters, AdProduct product)
		{
			if (product.Name != null)
				parameters.AddWithValue("Name", product.Name);
			else
				parameters.AddWithValue("Name", DBNull.Value);

			parameters.AddWithValue("AdId", product.AdId);
			parameters.AddWithValue("ProductCategoryId", product.ProductCategoryId);
			parameters.AddWithValue("Weight", product.Weight);
			parameters.AddWithValue("PricePerWeight", product.PricePerWeight);
			parameters.AddWithValue("CurrencyId", product.Currency);
		}

		public static bool UpdateAdProduct(AdProduct product)
		{
			const string query = "update dbo.AdProducts set Name=@Name, AdId=@AdId, ProductCategoryId=@ProductCategoryId, " +
				"Weight=@Weight, PricePerWeight=@PricePerWeight, CurrencyId=@Currency where Id = @Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", product.Id);
			AddAddOrUpdateSqlParameters(sqlCommand.Parameters, product);

			int result = 0;
			try
			{
				connect.Open();
				result = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in AdProductsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool UpdateAdProducts(List<AdProduct> products)
		{
			if (!products.Any())
				return true;
			string query = "";
			var i = 0;
			foreach (var product in products)
			{
				query += @"
update dbo.AdProducts set Name=@Name" + i + ", AdId=" + product.AdId + ", ProductCategoryId=" + product.ProductCategoryId +
", Weight=" + product.Weight.ToString().Replace(",", ".") + ", PricePerWeight=" + product.PricePerWeight.ToString().Replace(",", ".") + 
", CurrencyId=" + (int)product.Currency + " where Id=" + product.Id;
				i++;
			}

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			i = 0;
			foreach (var product in products)
			{
				if (product.Name != null)
					sqlCommand.Parameters.AddWithValue("Name" + i, product.Name);
				else
					sqlCommand.Parameters.AddWithValue("Name" + i, DBNull.Value);
				i++;
			}

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
				throw new Exception("in AdProductsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool DeleteAdProduct(int id)
		{
			const string query = "delete from dbo.AdProducts where Id = @Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Id", id);

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
				throw new Exception("in AdProductsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static bool DeleteAdProducts(List<int> ids)
		{
			if (!ids.Any())
				return true;
			string query = "delete from dbo.AdProducts where Id=";
			query += string.Join(" or Id=", ids);

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
				throw new Exception("in AdProductsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}
	}
}