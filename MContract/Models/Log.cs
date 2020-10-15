using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class Log
	{
		public int ID { get; set; }
		public int LogTypeID { get; set; }
		public DateTime Time { get; set; }
		public string Message { get; set; }
	}
}