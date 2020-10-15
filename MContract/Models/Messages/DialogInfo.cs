using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	/// <summary>
	/// Информация по конкретному диалогу для страницы User/Dialogs
	/// </summary>
	public class DialogInfo
	{
		public int Id { get; set; }

		/// <summary>
		/// Пользователь, который просматривает страницу
		/// </summary>
		public int UserId { get; set; }

		/// <summary>
		/// Id пользователя, с которым диалог
		/// </summary>
		public int RespondentId { get; set; }

		public int ShowMessagesFromId { get; set; }
	}
}