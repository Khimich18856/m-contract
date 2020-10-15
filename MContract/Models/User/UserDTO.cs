//using MContract.Models.Enums;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace MContract.Models
//{
//	public interface IUserForMessage
//	{
//		int Id { get; set; }
//		string CompanyName { get; set; }
//		string CompanyNameWithTypeOfOwnership { get; set; }
//	}

//	/// <summary>
//	/// Для передачи с сервера на клиент в поле Sender у Message для сериализации без ошибок
//	/// </summary>
//	public class UserDTO : IUserForMessage
//	{
//		public int Id { get; set; }
//		public string CompanyName { get; set; }
//		public string CompanyNameWithTypeOfOwnership { get; set; }

//		public UserDTO(User user)
//		{
//			Id = user.Id;
//			CompanyName = user.CompanyName;
//			CompanyNameWithTypeOfOwnership = user.CompanyNameWithTypeOfOwnership;
//		}
//	}
//}