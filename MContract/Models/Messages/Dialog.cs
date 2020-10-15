using MContract.AppCode;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
    public class Dialog
    {
        public int Id { get; set; }
        public int TopicId { get; set; }
        public DialogTypes DialogType { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
        /// <summary>
        /// Вычисляемые поля
        /// </summary>
        public string Url {
            get
            {
                return C.SiteUrl + "User/Messages?respondentId=" + Respondent?.Id;
            }
        }

		public Ad Ad { get; set; }
        public Offer Offer { get; set; }
        public User Sender { get; set; }
		public User Recipient { get; set; }
        public User Respondent { get; set; }
        
        /// <summary>
        /// Для перехода со страницы Ads/NewOffer
        /// </summary>
        public bool IsFromNewOffer { get; set; }

        public string LastPageUrl { get; set; }

		/// <summary>
		/// Для передачи в контрол меню личного кабинета _PersonalArea
		/// </summary>
		public User PersonalAreaUser { get; set; }
        
        /// <summary>
        /// для поиска на странице User/Dialogs
        /// </summary>
        public string AllMessagesText { get; set; }

		public int NewMessagesCount { get; set; }
	}
}