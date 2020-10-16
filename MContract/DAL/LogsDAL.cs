using MContract.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MContract.DAL
{
    public class LogsDAL : BaseDataAccess
	{
		public static void AddError(string message)
		{
			AddMessage(1, message);
		}

		public static void AddMessage(string message)
		{
			AddMessage(2, message);
		}

		public static int AddMessage(int logTypeID, string message)
		{
			int result = 0;

			string query =
	@"INSERT INTO dbo.Logs
           (LogTypeID, Message, Time)
     VALUES
           (@LogTypeID, @Message, @Time)";

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);
			sqlCommand.Parameters.AddWithValue("LogTypeID", logTypeID);
			sqlCommand.Parameters.AddWithValue("Message", message);
			sqlCommand.Parameters.AddWithValue("Time", DateTime.Now);


			try
			{
				connect.Open();
				result = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				connect.Close();
			}

			return result;
		}

		internal static int GetTodayErrorsCount()
		{
			int result = 9999;

			string query = "select count(*) from dbo.Logs where LogTypeID = 1 And [Time] > @Time";

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Time", DateTime.Now.Date);

			try
			{
				connect.Open();
				SqlDataReader reader = sqlCommand.ExecuteReader();
				if (reader.Read())
				{
					result = (int)reader[0];
				}
				reader.Close();
			}
			catch (Exception)
			{
				//AddMessage("Exception in LogsDataAccess.GetTodayErrorsCount(): " + ex.ToString());
			}
			finally
			{
				connect.Close();
			}

			return result;
		}

		public static List<Log> GetLogs(DateTime from, int logTypeID)
		{
			List<Log> result = new List<Log>();

			string query = "select * from dbo.Logs where [Time] > @Time";

			if (logTypeID > 0)
				query += " and LogTypeID = " + logTypeID;

			query += " order by ID desc";

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("Time", from);

			try
			{
				connect.Open();
				SqlDataReader reader = sqlCommand.ExecuteReader();
				while (reader.Read())
				{
                    Log log = new Log
                    {
                        ID = (int)reader["ID"],
                        LogTypeID = (int)reader["LogTypeID"],
                        Time = (DateTime)reader["Time"],
                        Message = (string)reader["Message"]
                    };
                    result.Add(log);
				}
				reader.Close();
			}
			catch (Exception)
			{
				//LogsController.AddError("Exception in LogsDataAccess.GetLogs(): " + ex.ToString());
			}
			finally
			{
				connect.Close();
			}

			return result;
		}

		public static bool DeleteLogsToDate(DateTime dateAndTime)
		{
			string query = "delete from dbo.Logs where Time < @DateAndTime";

			SqlConnection connect = new SqlConnection(connStr);
			SqlCommand sqlCommand = new SqlCommand(query, connect);

			sqlCommand.Parameters.AddWithValue("DateAndTime", dateAndTime);

			int result = 0;
			try
			{
				connect.Open();
				result = sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
			finally
			{
				connect.Close();
			}

			return result > 0;
		}
	}
}