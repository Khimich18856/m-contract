using MContract.AppCode;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
    public class Offer
    {
		//поля из БД:
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int AdId { get; set; }
        public List<ProductOffer> ProductOffers { get; set; }
        public DateTime DateOfPosting { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime ActiveUntilDate { get; set; }
        public int? CityId { get; set; }
        public DeliveryTypes DeliveryType { get; set; }
        public string DeliveryAddress { get; set; }
        public DeliveryLoadTypes DeliveryLoadType { get; set; }
        public DeliveryWays DeliveryWay { get; set; }
        public TermsOfPayments TermsOfPayments { get; set; }
        public int? DefermentPeriod { get; set; }
        public FormOfPayment? FormOfPayment { get; set; }
        public Nds Nds { get; set; }
        public string Comment { get; set; }
        public ModerateResults ModerateResult { get; set; }
        public bool ShowInDealsHistory { get; set; }

        /// <summary>
        /// Дата заключения контракта (дата перехода предложения в статус Accepted)
        /// </summary>
        public DateTime? ContractSendDate { get; set; }

		private OfferStatuses _offerStatus;
        public OfferStatuses OfferStatus
        {
            get
            {
                if (_offerStatus == OfferStatuses.Published && ActiveUntilDate < DateTime.Now.ToUniversalTime() && ContractStatus == ContractStatuses.NotSent)
                    return OfferStatuses.Expired;
                else
                    return this._offerStatus;
            }
            set
            {
                _offerStatus = value;
            }
        }
        public ContractStatuses ContractStatus { get; set; }

        //вычисляемые и заполняемые поля:
        public string Url
        {
            get
            {
                return Urls.Ads + "/" + (this.OfferStatus == OfferStatuses.Draft ? "EditOffer" : "Offer") + "/" + this.Id; 
            }
        }
        public string Name
        {
            get
            {
                return "Предложение №" + this.Id;
            }
        }
        public User Sender { get; set; }
        public Ad Ad { get; set; }
        public Town City { get; set; }
        public float SumOffer { 
            get
            {
                if (SumProduct != null && SumProduct.Any())
                    return SumProduct.Sum();
                else
                    return 0;
            }
        }
        public List<float> SumProduct { get; set; }
        public string ProductOffersDescription { get; set; }
        public User PersonalAreaUser { get; set; }

        /// <summary>
        /// Является ли объявление моим (при просмотре на странице Ads/Show например)
        /// </summary>
        public bool IsMy
        {
            get
            {
				return SenderId == SM.CurrentUserId;
            }
        }

        // передается в ads/offer
        public bool IsFromAd { get; set; }

		/// <summary>
		/// Условия поставки строкой
		/// </summary>
		public string DeliveryTypeStr
		{
			get
			{
				return AdHelper.GetDeliveryTypeString(DeliveryType);
			}
		}

		/// <summary>
		/// Погрузка строкой
		/// </summary>
		public string DeliveryLoadTypeStr
		{
			get
			{
				return AdHelper.GetDeliveryLoadTypeString(DeliveryLoadType);
			}
		}

		/// <summary>
		/// Способ доставки строкой
		/// </summary>
		public string DeliveryWayStr
		{
			get
			{
				return AdHelper.GetDeliveryWayString(DeliveryWay);
			}
		}

		/// <summary>
		/// Цена (с НДС/без НДС) строкой
		/// </summary>
		public string NdsStr
		{
			get
			{
				return AdHelper.GetNdsString(Nds);
			}
		}

		/// <summary>
		/// Условия оплаты строкой
		/// </summary>
		public string TermsOfPaymentsStr
		{
			get
			{
				return AdHelper.GetTermsOfPaymentsString(TermsOfPayments);
			}
		}

		#region Для страницы Ads/Show
		public string CanEditCssClassForDeliveryType
		{
			get
			{
				return IsMy && DeliveryType == DeliveryTypes.Any || !IsMy && DeliveryType != DeliveryTypes.Any ? " can-edit" : "";
			}
		}

		public string CanEditCssClassForDeliveryLoadType
		{
			get
			{
				return IsMy && DeliveryLoadType == DeliveryLoadTypes.Any || !IsMy && DeliveryLoadType != DeliveryLoadTypes.Any ? " can-edit" : "";
			}
		}

		public string CanEditCssClassForDeliveryWay
		{
			get
			{
				return IsMy && DeliveryWay == DeliveryWays.Any || !IsMy && DeliveryWay != DeliveryWays.Any ? " can-edit" : "";
			}
		}

		public string CanEditCssClassForNds
		{
			get
			{
				return IsMy && Nds == Nds.Any || !IsMy && Nds != Nds.Any ? " can-edit" : "";
			}
		}

		public string CanEditCssClassForTermsOfPayments
		{
			get
			{
				return IsMy && TermsOfPayments == TermsOfPayments.Any || !IsMy && TermsOfPayments != TermsOfPayments.Any ? " can-edit" : "";
			}
		}
		#endregion

	}
}