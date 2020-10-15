using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
    public class AdsIndexViewModel
    {
		public List<Ad> Ads { get; set; }
        public bool IsSearch { get; set; }
        public bool IsBuy { get; set; }
        public List<int> CategoriesId { get; set; }
        public List<int> CitiesId { get; set; }
		public string Heading { get; set; }
		public User PersonalAreaUser { get; set; }

		//для совместимости с IndexOld (потом удалить):
		public bool IsDrafts { get; set; }
		public bool IsMy { get; set; }
	}
}