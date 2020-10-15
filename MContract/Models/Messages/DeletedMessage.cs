using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class DeletedMessage
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int DialogWithUserId { get; set; }
		public int MessageId { get; set; }
	}
}