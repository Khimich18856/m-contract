using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class UserMessagesViewModel
	{
		public List<Message> Messages { get; set; } = new List<Message>();

		public string LastPageUrl { get; set; }

		public LeftMenuViewModel LeftMenuViewModel { get; set; }

		public User CurrentUser { get; set; }

		public User Respondent { get; set; }

		
	}
}