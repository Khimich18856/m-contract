using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	/// <summary>
	/// Используется для отображения конкретной сделки на странице истории сделок
	/// </summary>
	public class UserDealsHistoryItem
	{
		/// <summary>
		/// Объявление, по которому прошла сделка
		/// </summary>
		public Ad Ad { get; set; }

		/// <summary>
		/// Предложение, по которому прошла сделка
		/// </summary>
		public Offer Offer { get; set; }

		/// <summary>
		/// Контрагент, с которым заключена сделка
		/// </summary>
		public User Counteragent { get; set; }

		/// <summary>
		/// Дата сделки
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Сделка по моему объявлению?
		/// true = мое объявление, false = мое предложение
		/// </summary>
		public bool IsDealForMyAd { get; set; }

		/// <summary>
		/// Список id категорий товаров в объявлении
		/// </summary>
		public List<int> ProductCategoryIds { get; set; }

		public float TotalWeight { get; set; }
	}
}