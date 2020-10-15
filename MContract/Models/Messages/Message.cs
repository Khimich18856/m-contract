using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public bool IsReviewContractNotification { get; set; }
        public bool IsContractReviewed { get; set; }
        public int OfferId { get; set; }
        public int AdId { get; set; }
        public int RequestJoinAdFromUserId { get; set; }
        
		public User Sender { get; set; }
        public List<UserFile> Files { get; set; }
        public string Direction { get; set; }
		public bool IsLoadedToClient { get; set; }

		public string SenderLogoUrl { get; set; }
    }
}