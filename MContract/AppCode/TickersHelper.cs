using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MContract.AppCode
{
    public class TickersHelper
    {
        public static float GetTodayUsdQuote()
        {
            var usdTickerId = 7;
            var ticker = MContract.DAL.TickersDAL.GetTicker(usdTickerId);
            var usdQuotes = MContract.DAL.QuotesDAL.GetQuotes(usdTickerId, DateTime.Now.AddDays(-14)).Where(q => q.TickerId == usdTickerId).OrderByDescending(q => q.CbrDate).Take(2).ToList();
            if (usdQuotes.Count > 1 && DateTime.Now > usdQuotes[0].CbrDate)
            {
                return usdQuotes[1].Value;
            } else if (usdQuotes.Count > 0)
            {
                return usdQuotes[0].Value;
            } else
            {
                return 0;
            }
        }
    }
}