using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using MContract.Models;

namespace MContract.DAL
{
	public class ProductCategoriesDAL : BaseDataAccess
	{
		private static List<ProductCategory> _productCategoriesCache = null;
		private static ProductCategory ReadProductCategoryInfo(SqlDataReader reader)
		{
			var result = new ProductCategory
			{
				Id = (int)reader["Id"],
				Name = ((string)reader["Name"]).Trim(),
				Level = (int)reader["Level"],
				ParentId = (int)reader["ParentId"],
			};

			return result;
		}
		public static ProductCategory GetProductCategory(int id)
		{
			ProductCategory result = null;
			const string query = @"select * from dbo.ProductCategories where Id=@Id";
			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Id", id);
			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result = ReadProductCategoryInfo(reader);

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in ProductCategoriesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}
		public static List<ProductCategory> GetCategories()
		{
			if (_productCategoriesCache != null)
				return _productCategoriesCache;
			var result = new List<ProductCategory>();
			const string query = "select * from dbo.ProductCategories";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var category = ReadProductCategoryInfo(reader);
					result.Add(category);
				}
				reader.Close();
				_productCategoriesCache = result;
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in ProductCategoriesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			foreach (var category in result)
			{
				foreach (var childCategory in result)
				{
					if (childCategory.ParentId == category.Id)
					{
						category.ChildrenId.Add(childCategory.Id);
					}
				}
			}

			return result;
		}
		public static int AddCategory(ProductCategory category)
		{
			int newCategoryId = 0;
			const string query = @"insert into dbo.ProductCategories (Name, Level, ParentId) 
values (@Name, @Level, @ParentId);
DECLARE @newCategoryID int;
   SELECT @newCategoryID = SCOPE_IDENTITY();
   SELECT @newCategoryID";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			var parameters = sqlCommand.Parameters;

			AddAddOrUpdateSqlParameters(parameters, category);

			try
			{
				connect.Open();
				newCategoryId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in ProductCategoriesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}
			return newCategoryId;
		}
		private static void AddAddOrUpdateSqlParameters(SqlParameterCollection parameters, ProductCategory category)
		{
			parameters.AddWithValue("Name", category.Name);
			parameters.AddWithValue("Level", category.Level);
			parameters.AddWithValue("Password", category.ParentId);
		}
	}
}