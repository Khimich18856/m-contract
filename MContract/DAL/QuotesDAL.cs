using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using MContract.Models;

namespace MContract.DAL
{
	public class QuotesDAL : BaseDataAccess
	{
		private static Quote ReadQuoteInfo(SqlDataReader reader, bool getQuoteTypeInfo = false)
		{
			var result = new Quote
			{
				Id = (int)reader["Id"],
				TickerId = (int)reader["TickerId"],
				CbrDate = (DateTime)reader["CbrDate"],
				Value = Convert.ToSingle(reader["Value"].ToString())
			};
			
			return result;
		}

		public static Quote GetQuote(int id)
		{
			Quote result = null;
			const string query = @"SELECT * FROM dbo.Quotes where Id = @Id";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);
			sqlCommand.Parameters.AddWithValue("Id", id);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				if (reader.Read())
					result = ReadQuoteInfo(reader, getQuoteTypeInfo: true);

				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in QuotesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static List<Quote> GetQuotes(int? tickerId = null, DateTime? fromDate = null)
		{
			var result = new List<Quote>();
			string query =
@"SELECT * FROM dbo.Quotes  
  where 1 = 1";

			if (tickerId.HasValue)
				query += " and TickerId = " + tickerId.Value;

			if (fromDate.HasValue)
				query += " and CbrDate >= @FromDate";

			var connection = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connection);

			if (fromDate.HasValue)
				sqlCommand.Parameters.AddWithValue("FromDate", fromDate.Value);

			try
			{
				connection.Open();
				var reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
					var quote = ReadQuoteInfo(reader, getQuoteTypeInfo: true);
					result.Add(quote);
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in QuotesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connection.Close();
			}

			return result;
		}

		public static int AddQuote(Quote quote)
		{
			int newQuoteId = 0;
			const string query = @"insert into dbo.Quotes (TickerId, CbrDate, Value) 
values (@TickerId, @CbrDate, @Value);
DECLARE @newQuoteID int;
   SELECT @newQuoteID = SCOPE_IDENTITY();
   SELECT @newQuoteID";

			var connect = new SqlConnection(connStr);
			var sqlCommand = new SqlCommand(query, connect);

			AddAddOrUpdateSqlParameters(sqlCommand.Parameters, quote);

			try
			{
				connect.Open();
				newQuoteId = (int)sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				string methodName = MethodBase.GetCurrentMethod().Name;
				throw new Exception("in QuotesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return newQuoteId;
		}

		private static void AddAddOrUpdateSqlParameters(SqlParameterCollection parameters, Quote quote)
		{
			parameters.AddWithValue("TickerId", quote.TickerId);
			parameters.AddWithValue("CbrDate", quote.CbrDate);
			parameters.AddWithValue("Value", quote.Value);
		}
		
		public static bool DeleteQuote(int id)
		{
			const string query = "delete from dbo.Quotes where Id = @Id";

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
				throw new Exception("in QuotesDAL." + methodName + "(): " + ex);
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}
	}
}