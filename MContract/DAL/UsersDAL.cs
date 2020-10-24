using MContract.Models;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Configuration;
using System.Text;
using MContract.AppCode;


namespace MContract.DAL
{
    public class UsersDAL : BaseDataAccess
    {
        private static User ReadUserInfo(SqlDataReader reader)
        {
            var result = new User
            {
                Id = (int)reader["Id"],
                CompanyName = ((string)reader["CompanyName"]).Trim(),
                ContactName = ((string)reader["ContactName"]).Trim(),
                Email = ((string)reader["Email"]).Trim(),
                Password = ((string)reader["Password"]).Trim(),
                TypeOfOwnership = (TypesOfOwnership)(int)reader["TypeOfOwnershipId"],
                CityId = (int)reader["CityId"],
                INN = ((string)reader["INN"]).Trim(),
                OGRN = ((string)reader["OGRN"]).Trim(),
                PhoneNumber = ((string)reader["PhoneNumber"]).Trim(),
                ModerateResult = (ModerateResults)(int)reader["ModerateResultId"],
                CurrentRespondentId = (int)reader["CurrentRespondentId"],
                Created = (DateTime)reader["Created"],
                EmailConfirmed = (bool)reader["EmailConfirmed"],
                Deleted = (bool)reader["Deleted"],
                CheckedInSbis = (bool)reader["CheckedInSbis"],
                LastOnline = (DateTime)reader["LastOnline"],
                VerificationCode = ((string)reader["VerificationCode"]).Trim()
            };

            if (reader["PhoneNumberCity"] != DBNull.Value)
                result.PhoneNumberCity = ((string)reader["PhoneNumberCity"]).Trim();

            if (reader["Rating"] != DBNull.Value)
                result.Rating = (double)reader["Rating"];

            if (reader["Address"] != DBNull.Value)
                result.Address = ((string)reader["Address"]).Trim();

            if (reader["FactualAddress"] != DBNull.Value)
                result.FactualAddress = ((string)reader["FactualAddress"]).Trim();

            if (reader["SubscribeUntilDate"] != DBNull.Value)
                result.SubscribeUntilDate = (DateTime)reader["SubscribeUntilDate"];

            #region поля СБИС
            result.CheckedInSbis = (bool)reader["CheckedInSbis"];

            if (reader["SbisCompanyName"] != DBNull.Value)
                result.SbisCompanyName = ((string)reader["SbisCompanyName"]).Trim();

            if (reader["SbisTypeOfOwnershipId"] != DBNull.Value)
                result.SbisTypeOfOwnershipId = (int)reader["SbisTypeOfOwnershipId"];

            if (reader["SbisOGRN"] != DBNull.Value)
                result.SbisOGRN = ((string)reader["SbisOGRN"]).Trim();

            if (reader["SbisWorksFrom"] != DBNull.Value)
                result.SbisWorksFrom = (DateTime)reader["SbisWorksFrom"];

            if (reader["SbisTownId"] != DBNull.Value)
                result.SbisTownId = (int)reader["SbisTownId"];
            #endregion

            return result;
        }

        public static User GetUser(int id)
        {
            User result = null;
            const string query = @"select * from dbo.Users where Id=@Id";
            var connection = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connection);
            sqlCommand.Parameters.AddWithValue("Id", id);

            try
            {
                connection.Open();
                var reader = sqlCommand.ExecuteReader();
                if (reader.Read())
                    result = ReadUserInfo(reader);

                reader.Close();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        #region
        public static List<User> GetUsers(List<int> ids = null, bool? checkedInSbis = null, int? moderateResultId = null)
        {
            var result = new List<User>();
            string query = "select * from dbo.Users where 1 = 1";
            if (ids != null)
            {
                if (ids.Any())
                    query += " and Id in (" + string.Join(",", ids) + ")";
                else
                    return result;
            }

            if (checkedInSbis.HasValue)
                query += " and CheckedInSbis = " + (checkedInSbis.Value ? 1 : 0);

            if (moderateResultId.HasValue)
                query += $" and ModerateResultId = {moderateResultId.Value}";

            var connection = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                var reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    var user = ReadUserInfo(reader);
                    result.Add(user);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connection.Close();
            }

            return result;
        }
        #endregion

        #region Добавлление нового ЮЗЕРА
        public static int AddUser(User user)
        {
            Guid g;
            g = Guid.NewGuid();
            string _g = Convert.ToString(g);
            int newUserId = 0;
            const string query = @"insert into dbo.Users (ContactName, CompanyName, Email, Password, TypeOfOwnershipId, CityId, INN, OGRN, PhoneNumber, Rating, Address, FactualAddress, ModerateResultId, OpenDialogRespondentIds, CurrentRespondentId, Created, EmailConfirmed, SubscribeUntilDate, CheckedInSbis, SbisCompanyName, SbisTypeOfOwnershipId, SbisOGRN, SbisWorksFrom, SbisTownId, LastOnline, VerificationCode) 
values (@ContactName, @CompanyName, @Email, @Password, @TypeOfOwnershipId, @CityId, @INN, @OGRN, @PhoneNumber, @Rating, @Address, @FactualAddress, @ModerateResultId, @OpenDialogRespondentIds, @CurrentRespondentId, @Created, @EmailConfirmed, @SubscribeUntilDate, @CheckedInSbis, @SbisCompanyName, @SbisTypeOfOwnershipId, @SbisOGRN, @SbisWorksFrom, @SbisTownId, @LastOnline, @VerificationCode);
DECLARE @newUserID int;
   SELECT @newUserID = SCOPE_IDENTITY();
   SELECT @newUserID";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);
            var parameters = sqlCommand.Parameters;

            AddAddOrUpdateSqlParameters(parameters, user);
            parameters.AddWithValue("OpenDialogRespondentIds", "");
            parameters.AddWithValue("CurrentRespondentId", 0);
            parameters.AddWithValue("VerificationCode", _g);

            try
            {
                connect.Open();
                newUserId = (int)sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return newUserId;
        }
        #endregion

        #region
        private static void AddAddOrUpdateSqlParameters(SqlParameterCollection parameters, User user)
        {
            parameters.AddWithValue("ContactName", user.ContactName);
            parameters.AddWithValue("CompanyName", user.CompanyName);
            parameters.AddWithValue("Email", user.Email);
            parameters.AddWithValue("Password", user.Password);
            parameters.AddWithValue("TypeOfOwnershipId", user.TypeOfOwnership);
            parameters.AddWithValue("CityId", user.CityId);
            parameters.AddWithValue("INN", user.INN);
            parameters.AddWithValue("OGRN", user.OGRN);
            parameters.AddWithValue("PhoneNumber", user.PhoneNumber);
            if (user.PhoneNumberCity != null)
                parameters.AddWithValue("PhoneNumberCity", user.PhoneNumberCity);
            else
                parameters.AddWithValue("PhoneNumberCity", DBNull.Value);

            parameters.AddWithValue("Rating", user.Rating);

            if (user.Address != null)
                parameters.AddWithValue("Address", user.Address);
            else
                parameters.AddWithValue("Address", DBNull.Value);

            if (user.FactualAddress != null)
                parameters.AddWithValue("FactualAddress", user.FactualAddress);
            else
                parameters.AddWithValue("FactualAddress", DBNull.Value);
            parameters.AddWithValue("ModerateResultId", user.ModerateResult);

            parameters.AddWithValue("Created", user.Created);
            parameters.AddWithValue("EmailConfirmed", user.EmailConfirmed);

            if (user.SubscribeUntilDate != null)
                parameters.AddWithValue("SubscribeUntilDate", user.SubscribeUntilDate);
            else
                parameters.AddWithValue("SubscribeUntilDate", DBNull.Value);

            parameters.AddWithValue("CheckedInSbis", user.CheckedInSbis);

            if (user.SbisCompanyName != null)
                parameters.AddWithValue("SbisCompanyName", user.SbisCompanyName);
            else
                parameters.AddWithValue("SbisCompanyName", DBNull.Value);

            if (user.SbisTypeOfOwnershipId != null)
                parameters.AddWithValue("SbisTypeOfOwnershipId", user.SbisTypeOfOwnershipId);
            else
                parameters.AddWithValue("SbisTypeOfOwnershipId", DBNull.Value);

            if (user.SbisOGRN != null)
                parameters.AddWithValue("SbisOGRN", user.SbisOGRN);
            else
                parameters.AddWithValue("SbisOGRN", DBNull.Value);

            if (user.SbisWorksFrom != null)
                parameters.AddWithValue("SbisWorksFrom", user.SbisWorksFrom);
            else
                parameters.AddWithValue("SbisWorksFrom", DBNull.Value);

            if (user.SbisTownId != null)
                parameters.AddWithValue("SbisTownId", user.SbisTownId);
            else
                parameters.AddWithValue("SbisTownId", DBNull.Value);

            parameters.AddWithValue("LastOnline", user.LastOnline);
        }

        public static bool UpdateUser(User user)
        {
            const string query =
@"update dbo.Users set ContactName=@ContactName, CompanyName=@CompanyName, Email=@Email, 
				Password=@Password, TypeOfOwnershipId=@TypeOfOwnershipId, CityId=@CityId, INN=@INN, OGRN=@OGRN, 
				PhoneNumber=@PhoneNumber, PhonenumberCity=@PhonenumberCity, Rating=@Rating, Address=@Address, FactualAddress=@FactualAddress,
				ModerateResultId=@ModerateResultId, Created=@Created, EmailConfirmed=@EmailConfirmed, SubscribeUntilDate=@SubscribeUntilDate,
			    CheckedInSbis=@CheckedInSbis, SbisCompanyName=@SbisCompanyName, SbisTypeOfOwnershipId=@SbisTypeOfOwnershipId, SbisOGRN=@SbisOGRN, 
				SbisWorksFrom=@SbisWorksFrom, SbisTownId=@SbisTownId, LastOnline=@LastOnline 
where Id = @Id";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);

            sqlCommand.Parameters.AddWithValue("Id", user.Id);
            AddAddOrUpdateSqlParameters(sqlCommand.Parameters, user);

            int result = 0;
            try
            {
                connect.Open();
                result = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return result > 0;
        }

        //public static bool DeleteUser(int id)
        //{
        //	const string query = "delete from dbo.Users where Id = @Id";

        //	var connect = new SqlConnection(connStr);
        //	var sqlCommand = new SqlCommand(query, connect);

        //	sqlCommand.Parameters.AddWithValue("Id", id);

        //	int result = 0;
        //	try
        //	{
        //		connect.Open();
        //		result = sqlCommand.ExecuteNonQuery();
        //	}
        //	catch (Exception ex)
        //	{
        //		string methodName = MethodBase.GetCurrentMethod().Name;
        //		throw new Exception("in UsersDAL." + methodName + "(): " + ex);
        //	}
        //	finally
        //	{
        //		connect.Close();
        //	}

        //	return result > 0;
        //}

        public static int AddRegularClient(int userId, int clientId)
        {
            int newRegularClientId = 0;
            const string query = @"insert into dbo.RegularClients (UserId, ClientId) 
values (@UserId, @ClientId);
DECLARE @newRegularClientID int;
   SELECT @newRegularClientID = SCOPE_IDENTITY();
   SELECT @newRegularClientID";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);
            var parameters = sqlCommand.Parameters;

            parameters.AddWithValue("UserId", userId);
            parameters.AddWithValue("ClientId", clientId);

            try
            {
                connect.Open();
                newRegularClientId = (int)sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return newRegularClientId;
        }

        public static List<User> GetRegularClients(int userId)
        {
            var result = new List<User>();
            string query = @"select rc.UserId, rc.ClientId, Users.*
from dbo.RegularClients as rc
join dbo.Users on rc.UserId=" + userId + " and ClientId=Users.Id";

            var connection = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                var reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    var client = ReadUserInfo(reader);
                    result.Add(client);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        public static bool DeleteRegularClient(int userId, int clientId)
        {
            const string query = "delete from dbo.RegularClients where UserId=@UserId and ClientId=@ClientId";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);

            sqlCommand.Parameters.AddWithValue("UserId", userId);
            sqlCommand.Parameters.AddWithValue("ClientId", clientId);

            int result = 0;
            try
            {
                connect.Open();
                result = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return result > 0;
        }

        public static bool UpdateUserChangeCurrentRespondentId(int userId, int respondentId)
        {
            string query = $"update dbo.Users set CurrentRespondentId={respondentId} where Id={userId}";

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
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return result > 0;
        }

        public static List<int> GetOpenDialogRespondentIds(int userId)
        {
            var result = new List<int>();
            var resultStr = "";
            string query = @"select * from dbo.Users where Id=" + userId;

            var connection = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                var reader = sqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    if (reader["OpenDialogRespondentIds"] != DBNull.Value)
                    {
                        resultStr = (string)reader["OpenDialogRespondentIds"];
                        if (resultStr.Length > 0)
                            result = resultStr.Split(',').Select(d => Convert.ToInt32(d)).ToList();
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        public static bool UpdateOpenDialogRespondentIds(int userId, List<int> respondentIds)
        {
            string query = $"update dbo.Users set OpenDialogRespondentIds=@OpenDialogRespondentIds where Id=@Id";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);
            sqlCommand.Parameters.AddWithValue("OpenDialogRespondentIds", string.Join(",", respondentIds));
            sqlCommand.Parameters.AddWithValue("Id", userId);

            int result = 0;
            try
            {
                connect.Open();
                result = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return result > 0;
        }

        public static int AddUserRating(int toUserId, int fromUserId, int rating, int adId = 0)
        {
            int newRatingId = 0;
            const string query = @"insert into dbo.UserRatings (ToUserId, FromUserId, Rating, AdId, Created) values (@ToUserId, @FromUserId, @Rating, @AdId, @Created)
DECLARE @newUserID int;
   SELECT @newUserID = SCOPE_IDENTITY();
   SELECT @newUserID";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);
            var parameters = sqlCommand.Parameters;

            parameters.AddWithValue("ToUserId", toUserId);
            parameters.AddWithValue("FromUserId", fromUserId);
            parameters.AddWithValue("Rating", rating);
            parameters.AddWithValue("AdId", adId);
            parameters.AddWithValue("Created", DateTime.Now.ToUniversalTime());

            try
            {
                connect.Open();
                newRatingId = (int)sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return newRatingId;
        }

        public static int GetUserRating(int toUserId, int fromUserId, int adId = 0)
        {
            var result = 0;
            string query = $"select Rating from dbo.UserRatings where ToUserId={toUserId} and FromUserId={fromUserId} and AdId={adId}";

            var connection = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                var reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    result = (int)reader["Rating"];
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        public static List<int> GetUserRatings(int toUserId)
        {
            var result = new List<int>();
            string query = $"select * from dbo.UserRatings where ToUserId={toUserId}";

            var connection = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                var reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    var rating = (int)reader["Rating"];
                    result.Add(rating);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Проставляет данные в поле Rating таблицы Users для нескольких пользователей за 1 раз
        /// </summary>
        /// <param name="newUserRatings">ключ - Id пользователя, значение - значение рейтинга</param>
        public static void UpdateUserRatings(Dictionary<int, float> newUserRatings)
        {
            if (newUserRatings == null || !newUserRatings.Keys.Any())
                return;

            var update = new StringBuilder();
            foreach (var userId in newUserRatings.Keys)
            {
                var rating = newUserRatings[userId];
                update.AppendLine($"update dbo.Users set Rating = {rating.ToString().Replace(',', '.')} where Id = {userId}");
            }

            var query = update.ToString();

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);

            try
            {
                connect.Open();
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }
        }

        public static bool UpdateUserModerateResult(int userId, int moderateResultId)
        {
            const string query = "update dbo.Users set ModerateResultId=@ModerateResultId where Id=@Id";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);

            sqlCommand.Parameters.AddWithValue("Id", userId);
            sqlCommand.Parameters.AddWithValue("ModerateResultId", moderateResultId);

            try
            {
                connect.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return true;
        }

        public static bool UpdateUserPassword(int userId, string password)
        {
            const string query = "update dbo.Users set Password=@Password where Id=@Id";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);

            sqlCommand.Parameters.AddWithValue("Id", userId);
            sqlCommand.Parameters.AddWithValue("Password", password);

            try
            {
                connect.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return true;
        }

        #endregion

        #region
        public static bool UpdateUserLastOnline(int userId, DateTime lastOnline)
        {
            const string query = "update dbo.Users set LastOnline=@LastOnline where Id=@Id";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);

            sqlCommand.Parameters.AddWithValue("Id", userId);
            sqlCommand.Parameters.AddWithValue("LastOnline", lastOnline);

            try
            {
                connect.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return true;
        }

        public static bool UpdateUserDeleted(int userId, bool deleted)
        {
            const string query = "update dbo.Users set Deleted=@Deleted where Id=@Id";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);

            sqlCommand.Parameters.AddWithValue("Id", userId);
            sqlCommand.Parameters.AddWithValue("Deleted", deleted);

            try
            {
                connect.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return true;
        }

        public static bool UpdateUserCheckedInSbis(User user)
        {
            string query = "update dbo.Users set CheckedInSbis = 1";

            if (user.SbisCompanyName != null)
                query += " and SbisCompanyName = @SbisCompanyName";

            if (user.SbisTypeOfOwnershipId.HasValue)
                query += " and SbisTypeOfOwnershipId = " + user.SbisTypeOfOwnershipId.Value;

            if (user.SbisOGRN != null)
                query += " and SbisOGRN = @SbisOGRN";

            if (user.SbisWorksFrom.HasValue)
                query += " and SbisWorkedFrom = @SbisWorkedFrom";

            if (user.SbisTownId.HasValue)
                query += $" and SbisTownId = {user.SbisTownId.Value}";

            if (user.SbisScope != null)
                query += " and SbisScope = @SbisScope";

            query += " where Id=@Id";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);

            sqlCommand.Parameters.AddWithValue("Id", user.Id);

            if (user.SbisCompanyName != null)
                sqlCommand.Parameters.AddWithValue("SbisCompanyName", user.SbisCompanyName);

            if (user.SbisOGRN != null)
                sqlCommand.Parameters.AddWithValue("SbisOGRN", user.SbisOGRN);

            if (user.SbisWorksFrom.HasValue)
                sqlCommand.Parameters.AddWithValue("SbisWorksFrom", user.SbisWorksFrom.Value);

            if (user.SbisScope != null)
                sqlCommand.Parameters.AddWithValue("SbisScope", user.SbisScope);

            try
            {
                connect.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return true;
        }
        #endregion

        //==========================================================//

        #region Повторная отправка запроса для подтверждения регистрации

        public static User GetUserByEmail(string email)
        {
            User result = null;
            const string query = @"select * from dbo.Users where Email=@Email";
            var connection = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connection);
            sqlCommand.Parameters.AddWithValue("Email", email);

            try
            {
                connection.Open();
                var reader = sqlCommand.ExecuteReader();
                if (reader.Read())
                    result = ReadUserInfo(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connection.Close();
            }

            return result;
        }
        #endregion

        #region для проверки нового пользователя на уникальный EMAIL
        public static bool UserEmailUnique(string email)
        {
            const string query = "select COUNT(*) from dbo.Users where Email=@Email";
            var connect = new SqlConnection(connStr);
            var command = new SqlCommand(query, connect);
            command.Parameters.AddWithValue("Email", email);

            try
            {
                connect.Open();
                int count = (int)command.ExecuteScalar();
                if (count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }
        }
        #endregion

        #region для проверки нового пользователя на уникальный ИНН

        public static bool UserINNUnique(string inn)
        {
            const string query = "select COUNT(*) from dbo.Users where INN=@INN";
            var connect = new SqlConnection(connStr);
            var command = new SqlCommand(query, connect);
            command.Parameters.AddWithValue("INN", inn);

            try
            {
                connect.Open();
                int count = (int)command.ExecuteScalar();
                if (count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }
        }
        #endregion

        #region для проверки нового пользователя == верификации почты

        public static User GetUserByToken(string token)
        {
            User result = null;
            const string query = @"select * from dbo.Users where VerificationCode=@VerificationCode";
            var connection = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connection);
            sqlCommand.Parameters.AddWithValue("VerificationCode", token);

            try
            {
                connection.Open();
                var reader = sqlCommand.ExecuteReader();
                if (reader.HasRows) // если есть данные
                {
                    if (reader.Read())
                        result = ReadUserInfo(reader);

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connection.Close();
            }

            return result;
        }
        #endregion

        #region Подтверждение ЕМАЙЛ 

        public static bool UpdateUserEmailConfirmed(int userId, string token)
        {
            const string query = "update dbo.Users set EmailConfirmed=@EmailConfirmed, VerificationCode=@VerificationCode where Id=@Id";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);

            sqlCommand.Parameters.AddWithValue("Id", userId);
            sqlCommand.Parameters.AddWithValue("EmailConfirmed", true);
            sqlCommand.Parameters.AddWithValue("VerificationCode", token);

            try
            {
                connect.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return true;
        }
        #endregion

        #region Удаление подтверждение ЕМАЙЛ ввиду смены ЕМАЙЛ в личном кабинете

        public static bool UpdateUserEmailNOConfirmed(int userId, string token)
        {
            const string query = "update dbo.Users set EmailConfirmed=@EmailConfirmed, VerificationCode=@VerificationCode where Id=@Id";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);

            sqlCommand.Parameters.AddWithValue("Id", userId);
            sqlCommand.Parameters.AddWithValue("EmailConfirmed", false);
            sqlCommand.Parameters.AddWithValue("VerificationCode", token);

            try
            {
                connect.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return true;
        }
        #endregion

        # region Задать новый пароль

        public static bool UpdateUserResetPassword(int userId, string token, string password)
        {
            const string query = "update dbo.Users set Password=@Password, VerificationCode=@VerificationCode where Id=@Id";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);

            sqlCommand.Parameters.AddWithValue("Id", userId);
            sqlCommand.Parameters.AddWithValue("Password", password);
            sqlCommand.Parameters.AddWithValue("VerificationCode", token);

            try
            {
                connect.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return true;
        }
        #endregion

        #region Обновление VerificationCode для ЮЗЕРА =>  Отправка запроса - напоминаем пароль
        public static bool UpdateVerificationCode(int userId, string token)
        {
           
            const string query = "update dbo.Users set VerificationCode=@VerificationCode where Id=@Id";

            var connect = new SqlConnection(connStr);
            var sqlCommand = new SqlCommand(query, connect);
            var parameters = sqlCommand.Parameters;

            parameters.AddWithValue("Id", userId);
            parameters.AddWithValue("VerificationCode", token);

            try
            {
                connect.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string methodName = MethodBase.GetCurrentMethod().Name;
                throw new Exception("in UsersDAL." + methodName + "(): " + ex);
            }
            finally
            {
                connect.Close();
            }

            return true;
        }
        #endregion

    }
}