using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models.Enums
{
	/// <summary>
	/// Форма оплаты
	/// </summary>
	public enum FormOfPayment
	{
		/// <summary>
		/// Частичные платежи
		/// </summary>
		PartialPayments = 1,

		/// <summary>
		/// Разовый платеж по окончанию отсрочки
		/// </summary>
		OneTimePaymentAtTheEndOfTheDeferment = 2
	}
}