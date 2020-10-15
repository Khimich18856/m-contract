using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class UserFavoriteAd
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int AdId { get; set; }
	}
}