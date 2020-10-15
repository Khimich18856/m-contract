using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models.Enums
{
	/// <summary>
	/// Условия поставки
	/// </summary>
	public enum DeliveryTypes
	{
		/// <summary>
		/// Не выбрано
		/// </summary>
		Any = 0,

		/// <summary>
		/// Доставка продавцом
		/// </summary>
		DeliveryBySeller = 1,
		
		/// <summary>
		/// Самовывоз покупателем
		/// </summary>
		PickupByBuyer = 2
	}
}