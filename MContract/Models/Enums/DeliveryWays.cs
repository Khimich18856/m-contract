using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models.Enums
{
	/// <summary>
	/// Способ доставки
	/// </summary>
	public enum DeliveryWays
	{
		/// <summary>
		/// Не выбрано
		/// </summary>
		Any = 0,

		/// <summary>
		/// Авто
		/// </summary>
		Auto = 1,

		/// <summary>
		/// Ж/Д
		/// </summary>
		Railroad = 2
	}
}