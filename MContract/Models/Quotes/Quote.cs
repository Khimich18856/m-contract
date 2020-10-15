using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class Quote
	{
		public int Id { get; set; }

		public int TickerId { get; set; }

		public DateTime CbrDate { get; set; }

		public float Value { get; set; }
	}
}