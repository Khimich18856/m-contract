using MContract.DAL;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
    public class AdProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AdId { get; set; }
        public int ProductCategoryId { get; set; }
        public float Weight { get; set; }
        public float PricePerWeight { get; set; }
        public Currencies Currency { get; set; }
        public string ProductCategoryName { get; set; }

		public ProductOffer OfferProduct { get; set; }
    }
}