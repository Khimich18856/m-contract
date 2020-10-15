using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class AdsNewAdFinalStepViewModel
	{
		public Ad Ad { get; set; } 
		public User PersonalAreaUser { get; set; }
		public List<User> Users { get; set; }
		public List<string> AdProductCategoryNames { get; set; }
		public string CityName { get; set; }
		public List<string> Categories { get; set; } = new List<string>();
		public List<int> Currencies { get; set; } = new List<int>();
	}
}