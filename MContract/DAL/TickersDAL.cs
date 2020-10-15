using MContract.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MContract.DAL
{
	public class TickersDAL : BaseDataAccess
	{
		private static Ticker ReadTickerInfo(SqlDataReader reader, bool getTickerTypeInfo = false)
		{
			var result = new Ticker
			{
				Id = (int)reader["Id"],
				Name = ((string)reader["Name"]).Trim(),
				Description = (string)reader["Description"],
				Old = (bool)reader["Old"],
				DigitsAfterComma = (int)reader["DigitsAfterComma"]
			};

			if (reader["ChangeFromYesterdayClose"] != DBNull.Value)
				result.ChangeFromYesterdayClose = Convert.ToSingle(reader["ChangeFromYesterdayClose"].ToString());

			if (reader["ChangeFromYesterdayClosePercent"] != DBNull.Value)
				result.ChangeFromYesterdayClosePercent = Convert.ToSingle(reader["ChangeFromYesterdayClosePercent"].ToString());

			if (reader["InDollars"] != DBNull.Value)
				result.InDollars = (bool)reader["InDollars"];

			if (reader["LastQuote"] != DBNull.Value)
				result.LastQuote = float.Parse(reader["LastQuote"].ToString());

			if (reader["LastQuoteDate"] != DBNull.Value)
				result.LastQuoteDate = (DateTime)reader["LastQuoteDate"];

			if (reader["TickerType"] != DBNull.Value)
				result.TickerType = (int)reader["TickerType"];

			if (reader["InvestingComPairId"] != DBNull.Value)
				result.InvestingComPairId = (int)reader["InvestingComPairId"];

			if (reader["LmeName"] != DBNull.Value)
				result.LmeName = (string)reader["LmeName"];

			if (reader["TimeStr"] != DBNull.Value)
				result.TimeStr = (string)reader["TimeStr"];

			if (reader["CbrDate"] != DBNull.Value)
				result.CbrDate = (DateTime)reader["CbrDate"];

			//if (reader["NameForUrl"] != DBNull.Value)
			//	result.NameForUrl = (string)reader["NameForUrl"];

			return result;
		}

		public static Ticker GetTicker(int id)
		{
			Ticker result = null;
			const string query = @"SELECT * FROM dbo.Tickers t
									where Id=@Id";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Id", id);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result = ReadTickerInfo(reader, getTickerTypeInfo: true);

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in TickersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static Ticker GetTicker(string name)
		{
			Ticker result = null;
			const string query = @"SELECT * FROM dbo.Tickers t
									where Name=@Name";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Name", name);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result = ReadTickerInfo(reader, getTickerTypeInfo: true);

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in TickersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}
		

		public static List<Ticker> GetActiveTickers()
		{
			return GetTickers().Where(t => !t.Old).ToList();
		}

		public static List<Ticker> GetTickers()
		{
			var result = new List<Ticker>();
			string query = "select * from dbo.Tickers";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var ticker = ReadTickerInfo(reader);
					result.Add(ticker);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in TickersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static bool UpdateTicker(int id, float lastQuote)
		{
			const string query = "update dbo.Tickers set LastQuote=@LastQuote where Id = @Id";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("LastQuote", lastQuote);
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
				throw new Exception("in TickersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}

		public static void UpdateTickers(string sql)
		{
			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(sql, connect);

			try
			{
				connect.Open();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				//throw new Exception("in TickersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}
		}

		public static bool DeleteTicker(int id)
		{
			const string query = "delete from dbo.Tickers where Id = @Id";

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
				throw new Exception("in TickersDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}
	}
}