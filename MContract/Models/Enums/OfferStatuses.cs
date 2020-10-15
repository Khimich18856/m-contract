using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models.Enums
{
    public enum OfferStatuses
    {
        /// <summary>
        /// Черновик
        /// </summary>
        Draft = 1,
        
		/// <summary>
        /// Опубликовано
        /// </summary>
        Published = 2,
        
		/// <summary>
        /// Завершено
        /// </summary>
        Finished = 3,
        
		/// <summary>
        /// Истек срок действия
        /// </summary>
        Expired = 4,

		/// <summary>
		/// Удалено
		/// </summary>
		Deleted = 5
    }
}