using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
    public class ProductOffer
    {
        public int Id { get; set; }
        public int OfferId { get; set; }
        public int ProductId { get; set; }
        public float PricePerWeight { get; set; }
    }
}