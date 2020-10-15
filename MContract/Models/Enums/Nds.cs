using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models.Enums
{
	/// <summary>
	/// НДС
	/// </summary>
	public enum Nds
	{
		/// <summary>
		/// Не выбрано
		/// </summary>
		Any = 0,

		/// <summary>
		/// С НДС
		/// </summary>
		IsIncluded = 1,

		/// <summary>
		/// Без НДС
		/// </summary>
		IsNotIncluded = 2
	}
}