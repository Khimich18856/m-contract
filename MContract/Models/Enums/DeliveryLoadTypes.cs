using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models.Enums
{
	/// <summary>
	/// Условия поставки
	/// </summary>
	public enum DeliveryLoadTypes
	{
		/// <summary>
		/// Не выбрано
		/// </summary>
		Any = 0,

		/// <summary>
		/// Погрузка продавцом
		/// </summary>
		LoadBySeller = 1,

		/// <summary>
		/// Погрузка покупателем
		/// </summary>
		LoadByBuyer = 2
	}
}