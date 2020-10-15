using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class QuoteItemViewModel
	{
		public int TickerId { get; set; }
		public string TickerName { get; set; }
		public decimal Change { get; set; }
		public decimal ChangePercent { get; set; }
		public decimal Quote { get; set; }
		public string TimeStr { get; set; }
		public string ChartUrl { get; set; }
		public string ATitle { get; set; }
		public DateTime? CbrDate { get; set; }

		public string ChangeStr
		{
			get
			{
				return (Change > 0 ? "+" : "") + Change;
			}
		}

		public string ChangePercentStr
		{
			get
			{
				return (ChangePercent > 0 ? "+" : "") + ChangePercent;
			}
		}

		public string CssClass
		{
			get
			{
				if (ChangePercent >= 0)
					return "table-text-green";

				if (ChangePercent < 0)
					return "table-text-red";

				return "";
			}
		}

	}
}