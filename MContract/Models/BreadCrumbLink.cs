﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class BreadCrumbLink
	{
		public string Url { get; set; }
		public string Text { get; set; }
		public string Title { get; set; }
		public bool EndPoint { get; set; }
	}
}