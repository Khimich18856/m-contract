using MContract.AppCode;
using MContract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
    public class HomeIndexViewModel
    {
		public List<QuoteItemViewModel> InvestingComQuotes { get; set; }

		public List<QuoteItemViewModel> LmeQuotes { get; set; }

		public Quote TodayUsdQuote { get; set; }
		public Quote TomorrowUsdQuote { get; set; }
		public Quote TodayEuroQuote { get; set; }
		public Quote TomorrowEuroQuote { get; set; }

		public string TodayStr
		{
			get
			{
				if (TodayUsdQuote == null || TodayUsdQuote.CbrDate == null)
					return null;

				return TextHelper.GetDayAndMonthStr(TodayUsdQuote.CbrDate);
			}
		}

		public string TomorrowStr
		{
			get
			{
				if (TomorrowUsdQuote == null || TomorrowUsdQuote.CbrDate == null)
					return null;

				return TextHelper.GetDayAndMonthStr(TomorrowUsdQuote.CbrDate);
			}
		}
	}
}