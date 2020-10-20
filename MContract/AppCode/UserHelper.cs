using MContract.DAL;
using MContract.Models;
using MContract.Models.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace MContract.AppCode
{
	public class UserHelper
	{
		public static string GetSmallPhotoUrl(int userId)
		{
			var logos = PhotosDAL.GetCompanyLogoGroup(userId);
			if (logos.Any())
			{
				int requiredDimension = 200;
				var smallestDimensionDifference = logos.Min(p => Math.Abs(p.HigherDimension.Value - requiredDimension));
				var logo = logos.FirstOrDefault(p => Math.Abs(p.HigherDimension.Value - requiredDimension) == smallestDimensionDifference);
				if (logo != null)
					return logo.Url;
			}
			
			return PhotoHelper.NoLogoImageUrl;
		}


		public static string GetTypeOfOwnershipString(TypesOfOwnership typeOfOwnership)
		{
			switch (typeOfOwnership)
			{
				case TypesOfOwnership.IP: return "ИП";
				case TypesOfOwnership.OAO: return "ОАО";
				case TypesOfOwnership.OOO: return "ООО";
				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Фильтрует сообщения с учетом нажатий кнопок "Удалить чат", "Удалить все чаты"
		/// </summary>
		/// <param name="currentUserId">Id пользователя, который просматривает страницу</param>
		/// <param name="allMessages">все сообщения, отправленные или полученные текущим пользователем</param>
		/// <returns></returns>
		public static List<Message> FilterMessagesFromDialogInfo(int currentUserId, List<Message> allMessages)
		{
			var messagesFromMe = allMessages.Where(m => m.SenderId == currentUserId).ToList();
			var respondentIdsFromMe = messagesFromMe.Select(m => m.RecipientId).Distinct().ToList();
			var messagesToMe = allMessages.Where(m => m.RecipientId == currentUserId).ToList();
			var respondentIdsToMe = messagesToMe.Select(m => m.SenderId).Distinct().ToList();
			var respondentIds = new List<int>();
			respondentIds.AddRange(respondentIdsFromMe);
			foreach (var respondentId in respondentIdsToMe)
			{
				if (!respondentIds.Contains(respondentId))
					respondentIds.Add(respondentId);
			}

			var dialogInfos = DialogInfosDAL.GetDialogInfos(currentUserId);

			var filteredMessages = new List<Message>();
			foreach (var respondentId in respondentIds)
			{
				var messagesWithRespondent = allMessages.Where(m => m.SenderId == currentUserId || m.SenderId == respondentId).ToList();
				var dialogInfo = dialogInfos.FirstOrDefault(d => d.RespondentId == respondentId);
				if (dialogInfo != null)
					messagesWithRespondent = messagesWithRespondent.Where(m => m.Id >= dialogInfo.ShowMessagesFromId).ToList();

				filteredMessages.AddRange(messagesWithRespondent);
			}

			return filteredMessages;
		}

        public static int AddMessage(Message message)
        {
			#region Проверка заполненности обязательных полей
			if (message.SenderId == 0)
				throw new Exception("Не заполнено необходимое поле SenderId");

			if (message.RecipientId == 0)
				throw new Exception("Не заполнено необходимое поле RecipientId");
			#endregion

			if (message.Date == DateTime.MinValue)
                message.Date = DateTime.Now.ToUniversalTime();

            return MessagesDAL.AddMessage(message);
        }

        public static bool SendContract(int offerId)
        {

            var offer = OffersDAL.GetOffer(offerId);
            if (offer == null)
                return false;
            var ad = AdsDAL.GetAd(offer.AdId);
            if (ad == null)
                return false;
            var sender = SM.GetPersonalAreaUser();
            if (sender.Id == 0 || ad.SenderId != sender.Id)
                return false;
            //var notificationsUserId = User.SystemNotificationsUserId;
            var messageText = "Подтвердите контракт с " + sender.CompanyNameWithTypeOfOwnership + (sender.Town != null ? ", " + sender.TownName : "") + " по объявлению " + ad.Name + ".";
            var message = new Message
            {
                SenderId = ad.SenderId,
                RecipientId = offer.SenderId,
                Text = messageText,
                IsReviewContractNotification = true,
                IsContractReviewed = false,
                OfferId = offerId,
                AdId = ad.Id
            };
            if (AddMessage(message) > 0)
            {
                OffersDAL.ChangeOfferContractStatus(offer.Id, (int)ContractStatuses.Sent);
                return true;
            }
            else
                return false;
        }
        public static bool ReviewContract(int offerId, bool isAccept, bool isByAdCreator)
        {
            if (!MessagesDAL.MarkContractAsReviewed(offerId))
                return false;
            var offer = OffersDAL.GetOffer(offerId);
            if (offer == null)
                return false;
            var ad = AdsDAL.GetAd(offer.AdId);
            if (ad == null)
                return false;
            var sender = SM.GetPersonalAreaUser();
            if (offer.SenderId != sender.Id && ad.SenderId != sender.Id)
                return false;
            if (isAccept)
            {
                AdsDAL.ChangeAdStatusToFinishedAndActiveUntilDateToNow(offer.AdId);
                OffersDAL.ChangeOfferContractStatus(offer.Id, (int)ContractStatuses.Accepted);
				OffersDAL.UpdateOfferContractSendDate(offer.Id, DateTime.Now.ToUniversalTime());
				//var notificationsUserId = User.SystemNotificationsUserId;
				var messageText = "Контракт c " + sender.CompanyNameWithTypeOfOwnership + " по объявлению " + ad.Name + " подтвержден.";
                var message = new Message
                {
                    SenderId = offer.SenderId,
                    RecipientId = ad.SenderId,
                    Text = messageText,
                    AdId = ad.Id,
                    OfferId = offer.Id
                };
                AddMessage(message);
                var offers = OffersDAL.GetOffers(ad.Id).Where(o => o.ContractStatus != ContractStatuses.Accepted).ToList();
                if (offers.Any())
                {
                    var adSender = UsersDAL.GetUser(ad.SenderId);
                    if (adSender != null)
                    {
                        adSender.Town = TownsDAL.GetTown(adSender.CityId);
                        var notificationsUserId = MContract.Models.User.SystemNotificationsUserId;
                        foreach (var o in offers)
                        {
                            messageText = "По объявлению " + ad.Name + " организатора " + adSender.CompanyNameWithTypeOfOwnership + (adSender.Town != null ? ", " + adSender.TownName : "") +
                                          " заключен контракт с другим участником.";
                            message = new Message
                            {
                                SenderId = notificationsUserId,
                                RecipientId = o.SenderId,
                                Text = messageText,
                                AdId = ad.Id
                            };
                            AddMessage(message);
                        }
                    }
                }
            } else
            {
				
				//удаляем предложение, только если отменяет контракт участник, подавший это предложение, а не организатор
				//если отменяет контракт организатор
				if (isByAdCreator)
					OffersDAL.ChangeOfferContractStatus(offer.Id, (int)ContractStatuses.Declined);
				else//а если контракт отменяет участник, подавший предложение
					OffersDAL.DeleteOffer(offerId);
					
                //var notificationsUserId = User.SystemNotificationsUserId;
                var messageText = "Контракт c " + sender.CompanyNameWithTypeOfOwnership + " по объявлению " + ad.Name + " отменен.";
                var message = new Message
                {
                    SenderId = offer.SenderId,
                    RecipientId = ad.SenderId,
                    Text = messageText,
                    AdId = ad.Id,
                    OfferId = offer.Id
                };
                AddMessage(message);
            }
            return true;
        }

		public static void RecalculateUserRatings()
		{
			Thread.Sleep(10 * 60 * 1000);//10 минут

			while(true)
			{
				try
				{
					var newUserRatings = new Dictionary<int, float>();
					var users = UsersDAL.GetUsers();
					foreach (var user in users)
					{
						double rating = 0;
						//1. Аккаунт подтвержден. Максимальный балл по этому критерию - 1
						if (user.ModerateResult == ModerateResults.Accepted)
							rating += 1;

						//2. За время существования юридического лица или индивидуального предпринимателя.
						//(Начисляется по 0.1 балла за каждые 110 дней с момента регистрации по данным ЕГРЮЛ или ЕГРИП).
						//Максимальный балл по этому критерию - 1
						if (user.SbisWorksFrom.HasValue)
						{
							int companyExistDaysCount = (DateTime.Now.ToUniversalTime() - user.SbisWorksFrom.Value).Days;
							double addition = 0.1 * (companyExistDaysCount / 110);
							if (addition >= 1)
								addition = 1;

							rating += addition;
						}

						//3. За давность регистрации в M-Contract
						//(Начисляется по 0.1 балла за каждые 150 дней с момента регистрации в M - Contract).
						//Максимальный балл по этому критерию - 0,5
						var daysFromRegistration = (DateTime.Now.ToUniversalTime() - user.Created).Days;
						var multiplier = daysFromRegistration / 150;
						if (multiplier > 5)
							multiplier = 5;
						rating += 0.1 * (float)multiplier;

						//TODO: 4. За активную деятельность в M-Contract
						//(Правила расчета: 0.5 балла начисляется, если давность последнего платежа составляет не более 120 дней)
						//Максимальный балл по этому критерию – 0.5

						//5. Оценка пользователей
						//(Правила расчета: после каждой сделки(контракта), участники - и покупатель и продавец могут оценить друг друга,
						//что в итоге повлияет на итоговый рейтинг пользователя.Где
						//1 – контракт не состоялся, по явной вине второй стороны
						//2 – сильные нарушения условий
						//3 – незначительные нарушения
						//4 – условия не нарушены
						//5 – надежный партнер
						//За полученную оценку 1 или 2 с пользователя будет сниматься 0.1 балла с итогового рейтинга. За 3 – рейтинг остается без
						//изменений, а за 4 или 5 прибавляется 0.1 балла к рейтингу
						var rates = UsersDAL.GetUserRatings(user.Id);
						var badRatesCount = rates.Where(r => r < 3).Count();
						var goodRatesCount = rates.Where(r => r > 3).Count();
						rating -= 0.1 * badRatesCount;
						rating += 0.1 * goodRatesCount;

						if (rating > 5)
							rating = 5;

						if (Math.Abs(user.Rating - rating) >= 0.1)
							newUserRatings[user.Id] = (float)rating;
					}

					if (newUserRatings.Any())
						UsersDAL.UpdateUserRatings(newUserRatings);

					Thread.Sleep(24 * 60 * 60 * 1000);//сутки
				}
				catch
				{
					Thread.Sleep(10 * 60 * 1000);//10 минут
				}
			}
		}
    }
}