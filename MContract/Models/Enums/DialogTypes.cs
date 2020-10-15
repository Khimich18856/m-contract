using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MContract.Models.Enums
{
    public enum DialogTypes
    {
        /// <summary>
        /// Прямой диалог с пользователем
        /// </summary>
        User = 0,
        /// <summary>
        /// Диалог, связанный с объявлением
        /// </summary>
        Ad = 1,
        /// <summary>
        /// Диалог, связанный с предложением
        /// </summary>
        Offer = 2
    }
}