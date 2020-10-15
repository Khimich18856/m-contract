using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class Ticker
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public string Description { get; set; }

		public bool InDollars { get; set; }

		public bool Old { get; set; }

		public float ChangeFromYesterdayClose { get; set; }

		public float ChangeFromYesterdayClosePercent { get; set; }

		public float LastQuote { get; set; }
		public DateTime LastQuoteDate { get; set; }

		public float YesterdayCloseQuote { get; set; }
		public DateTime YesterdayCloseDate { get; set; }

		public int? TickerType { get; set; }
		public int? InvestingComPairId { get; set; }
		public string LmeName { get; set; }
		
		public int DigitsAfterComma { get; set; }

		public string TimeStr { get; set; }

		public DateTime CbrDate { get; set; }
	}
}