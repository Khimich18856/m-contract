using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class Town
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string RegionName { get; set; }
		public string FullNameAndRegionName
		{
			get
			{
				return Name + " (" + RegionName + ")";
			}
		}
		public string FullNameAndRegionNameWithComma
		{
			get
			{
				return Name + ", " + RegionName;
			}
		}
		public string NameAndRegionName
		{
			get
			{
				if (RegionName.Contains(Name))
					return Name;
				else
					return Name + " (" + RegionName + ")";
			}
		}

		public string NameAndRegionNameWithComma
		{
			get
			{
				if (RegionName.Contains(Name))
					return Name;
				else
					return Name + ", " + RegionName;
			}
		}
	}
}