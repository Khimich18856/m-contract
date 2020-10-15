using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models.Enums
{
    public enum ContractStatuses
    {
        /// <summary>
        /// Контракт не отправлен
        /// </summary>
        NotSent = 0,
        /// <summary>
        /// Контракт отправлен
        /// </summary>
        Sent = 1,
        /// <summary>
        /// Контракт принят
        /// </summary>
        Accepted = 2,
        /// <summary>
        /// Контракт отменен
        /// </summary>
        Declined = 3
    }
}