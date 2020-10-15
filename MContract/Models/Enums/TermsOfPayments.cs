using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models.Enums
{
	/// <summary>
	/// Условия оплаты
	/// </summary>
	public enum TermsOfPayments
	{
		/// <summary>
		/// Не выбрано
		/// </summary>
		Any = 0,
		/// <summary>
		/// Отсрочка платежа
		/// </summary>
		DefermentOfPayment = 1,
		/// <summary>
		/// 100% предоплата
		/// </summary>
		FullPrePayment = 2,
		/// <summary>
		/// Частичная предоплата
		/// </summary>
		PartialPrePayment = 3,
		/// <summary>
		/// По факту поставки
		/// </summary>
		OnArrival = 4
	}
}