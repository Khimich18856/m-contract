﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
	public class UserDealCardViewModel
	{
		public Ad Ad { get; set; }

		/// <summary>
		/// Предложение, по которому заключен контракт
		/// </summary>
		public Offer ContractOffer { get; set; }

		/// <summary>
		/// Направление сделки (строкой Продажа или Покупка)
		/// </summary>
		public string DealDirection { get; set; }

		/// <summary>
		/// Дата сделки
		/// </summary>
		public DateTime DealDate { get; set; }

		/// <summary>
		/// Покупатель
		/// </summary>
		public User Buyer { get; set; }


		/// <summary>
		/// Продавец
		/// </summary>
		public User Seller { get; set; }

		/// <summary>
		/// Условия поставки
		/// </summary>
		public string DeliveryType { get; set; }

		/// <summary>
		/// Погрузка
		/// </summary>
		public string DeliveryLoadType { get; set; }

		/// <summary>
		/// Способ доставки
		/// </summary>
		public string DeliveryWay { get; set; }

		/// <summary>
		/// Цена (С НДС/Без НДС)
		/// </summary>
		public string Nds { get; set; }

		///// <summary>
		///// ОТЛОЖЕННЫЙ ПЕРИОД
		///// </summary>
		//public int DeferMentPeriod { get; set; }

		/// <summary>
		/// Активен до даты
		/// </summary>
		public string TermsOfPayments { get; set; }

		/// <summary>
		/// Активен до даты
		/// </summary>
		public DateTime ActiveUntilDate { get; set; }

		public User PersonalAreaUser { get; set; }
	}
}