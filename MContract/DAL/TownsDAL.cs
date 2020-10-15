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
	public class TownsDAL : BaseDataAccess
	{
		private static Town ReadTown(SqlDataReader reader)
		{
			var result = new Town
			{
				Id = (int)reader["Id"],
				Name = ((string)reader["Name"]).Trim(),
				RegionName = ((string)reader["RegionName"]).Trim()
			};

			return result;
		}

		private static List<Town> _townsCache = null;

		public static List<Town> GetTowns()
		{
			if (_townsCache != null)
				return _townsCache;

			var result = new List<Town>();
			const string query = "select * from dbo.Towns";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var town = ReadTown(reader);
					result.Add(town);
				}
				reader.Close();
				_townsCache = result;
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in TownsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static Town GetTown(int id)
		{
			if (_townsCache != null)
				return _townsCache.Find(t => t.Id == id);

			Town result = null;
			const string query = "select * from dbo.Towns where Id=@Id";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Id", id);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result = ReadTown(reader);

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in TownsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static Town GetTown(string townName, string regionName)
		{
			if (_townsCache != null)
				return _townsCache.Find(t => t.Name == townName && t.RegionName == regionName);

			Town result = null;
			const string query = "select * from dbo.Towns where Name=@Name and RegionName=@RegionName";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Name", townName);
			sqlCommand.Parameters.AddWithValue("RegionName", regionName);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result = ReadTown(reader);
				
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in TownsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static int AddTown(Town town)
		{
			int newTownId = 0;
			const string query = @"insert into dbo.Towns (Name, Level, ParentId) 
values (@Name, @Level, @ParentId);
DECLARE @newTownID int;
   SELECT @newTownID = SCOPE_IDENTITY();
   SELECT @newTownID";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);
			var parameters = sqlCommand.Parameters;

			AddAddOrUpdateSqlParameters(parameters, town);

			try
			{
				connect.Open();
				newTownId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in TownsDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}
			return newTownId;
		}
		private static void AddAddOrUpdateSqlParameters(SqlParameterCollection parameters, Town town)
		{
			parameters.AddWithValue("Name", town.Name);
		}
	}
}