using MContract.DAL;
using MContract.Models;
using MContract.Models.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml;

namespace MContract.AppCode
{
	public class P /*Parser*/
	{
		public static void FillInformationFromSbis(User user)
		{
			try
			{
				var html = GetContentByUrl("https://sbis.ru/contragents/" + user.INN.Trim());
				var notFoundMessage = "Возможно, документ был удален, или вы перешли по неправильной ссылке";
				if (html.Contains(notFoundMessage) || html == "Удаленный сервер возвратил ошибку: (404) Не найден.")
					return;
				var nameTag = "<div itemprop=\"name\" class=\"cCard__MainReq-Name\">";
				var companyNameWithTypeOfOwnership = html.Substring(html.IndexOf(nameTag) + nameTag.Length);
				nameTag = "<h1>";
				companyNameWithTypeOfOwnership = companyNameWithTypeOfOwnership.Substring(companyNameWithTypeOfOwnership.IndexOf(nameTag) + nameTag.Length);
				companyNameWithTypeOfOwnership = companyNameWithTypeOfOwnership.Substring(0, companyNameWithTypeOfOwnership.IndexOf("<"));
				//var whitespaceRegex = @"\s+";
				//companyNameWithTypeOfOwnership = Regex.Replace(companyNameWithTypeOfOwnership, whitespaceRegex, "");
				var companyName = companyNameWithTypeOfOwnership.Substring(0, companyNameWithTypeOfOwnership.IndexOf(","));
				user.SbisCompanyName = companyName;
				var typeOfOwnershipString = companyNameWithTypeOfOwnership.Substring(companyNameWithTypeOfOwnership.IndexOf(", ") + ", ".Length);
				switch(typeOfOwnershipString)
				{
					case "ООО": user.SbisTypeOfOwnershipId = (int)Models.Enums.TypesOfOwnership.OOO; break;
					case "ОАО": user.SbisTypeOfOwnershipId = (int)Models.Enums.TypesOfOwnership.OAO; break;
					case "ИП": user.SbisTypeOfOwnershipId = (int)Models.Enums.TypesOfOwnership.IP; break;
				}
				var ogrnLiteral = "\"ОГРН\":\"";
				int ind1 = html.IndexOf(ogrnLiteral);
				if (ind1 != -1)
				{
					var ogrn = html.Substring(ind1 + ogrnLiteral.Length);
					ogrn = ogrn.Remove(ogrn.IndexOf("\""));
					user.SbisOGRN = ogrn;
				}

				int ind2 = html.IndexOf("Действует с ");
				if (ind2 != -1)
				{
					var sinceStr = html.Substring(ind2 + "Действует с ".Length);
					sinceStr = sinceStr.Remove("dd.mm.yyyy".Length);
					DateTime workFrom;
					if (DateTime.TryParse(sinceStr, out workFrom))
						user.SbisWorksFrom = workFrom.ToUniversalTime();
				}

				user.CheckedInSbis = true;
			}
			catch (Exception ex)
			{
				
			}
		}

		public static User GetSbisInformationFromInn(string Inn)
		{
			var result = new User();
			try
			{
				var html = GetContentByUrl("https://sbis.ru/contragents/" + Inn.Trim());
				var notFoundMessage = "Возможно, документ был удален, или вы перешли по неправильной ссылке";
				if (html.Contains(notFoundMessage) || html == "Удаленный сервер возвратил ошибку: (404) Не найден.")
					return null;
				var nameTag = "<div itemprop=\"name\" class=\"cCard__MainReq-Name\">";
				var companyNameWithTypeOfOwnership = html.Substring(html.IndexOf(nameTag) + nameTag.Length);
				nameTag = "<h1>";
				companyNameWithTypeOfOwnership = companyNameWithTypeOfOwnership.Substring(companyNameWithTypeOfOwnership.IndexOf(nameTag) + nameTag.Length);
				companyNameWithTypeOfOwnership = companyNameWithTypeOfOwnership.Substring(0, companyNameWithTypeOfOwnership.IndexOf("<"));
				//var whitespaceRegex = @"\s+";
				//companyNameWithTypeOfOwnership = Regex.Replace(companyNameWithTypeOfOwnership, whitespaceRegex, "");
				var companyName = companyNameWithTypeOfOwnership.Substring(0, companyNameWithTypeOfOwnership.IndexOf(","));
				result.CompanyName = companyName;
				var typeOfOwnershipString = companyNameWithTypeOfOwnership.Substring(companyNameWithTypeOfOwnership.IndexOf(",") + 1);
				result.TypeOfOwnership = (typeOfOwnershipString == "ООО" ? Models.Enums.TypesOfOwnership.OOO :
					typeOfOwnershipString == "ОАО" ? Models.Enums.TypesOfOwnership.OAO :
					typeOfOwnershipString == "ИП" ? Models.Enums.TypesOfOwnership.IP :
					Models.Enums.TypesOfOwnership.IP);
				var ogrnLiteral = "\"ОГРН\":\"";
				int ind1 = html.IndexOf(ogrnLiteral);
				if (ind1 != -1)
				{
					var ogrn = html.Substring(ind1 + ogrnLiteral.Length);
					ogrn = ogrn.Remove(ogrn.IndexOf("\""));

					result.OGRN = ogrn;
				}
			}
			catch (Exception ex)
			{

			}
			return result;
		}
		public static void RefreshQuotesFromLme()
		{
			int i = 0;
			var whitespaceRegex = @"\s+";
			while (true)
			{
				i++;
				var tickers = TickersDAL.GetTickers().Where(t => t.LmeName != null).ToList();
				var updateLines = new StringBuilder();
				try
				{
					var html = GetContentByUrl("https://lme.com");
					var tableContent = html.Substring(html.IndexOf("<table class=\"ring-times\""));
					tableContent = tableContent.Substring(html.IndexOf(">") + 1);
					tableContent = tableContent.Substring(0, tableContent.IndexOf("</table>"));
					tableContent = Regex.Replace(tableContent, whitespaceRegex, "");
					foreach (var ticker in tickers)
					{
						var lmeName = Regex.Replace(ticker.LmeName, whitespaceRegex, "");
						var indexOfLmeName = tableContent.IndexOf(lmeName);
						var lastQuoteTdContent = tableContent.Substring(tableContent.IndexOf("<td>", indexOfLmeName));
						lastQuoteTdContent = lastQuoteTdContent.Substring(lastQuoteTdContent.IndexOf(">") + 1);
						lastQuoteTdContent = lastQuoteTdContent.Substring(0, lastQuoteTdContent.IndexOf("</td>"));
						lastQuoteTdContent = lastQuoteTdContent.Replace(",", "");
						lastQuoteTdContent = lastQuoteTdContent.Replace(".", ",");
						decimal lastQuote = 0;
						if (!Decimal.TryParse(lastQuoteTdContent, out lastQuote))
							continue;
						var lastQuoteStr = lastQuote.ToString().Replace(',', '.');
						var sqlDate = TextHelper.GetTSqlDate(DateTime.Now);
						var updateLine = $"update dbo.Tickers " +
							$"set LastQuote = {lastQuoteStr}, " +
							$"LastQuoteDate = '{sqlDate}' " +
							$"where Id = {ticker.Id}";
						updateLines.AppendLine(updateLine);
					}
					var sql = updateLines.ToString();
					TickersDAL.UpdateTickers(sql);
				}
				catch (Exception ex)
				{

				}

				Thread.Sleep(15000);
			}
		}
		public static void RefreshQuotesFromInvestingCom()
		{
			int i = 0;
			
			while (true)
			{
				i++;
				var tickers = TickersDAL.GetTickers().Where(t => t.InvestingComPairId.HasValue).ToList();
				var updateLines = new StringBuilder();
				try
				{
					var html = GetContentByUrl("https://ru.investing.com/commodities/metals");
					foreach (var ticker in tickers)
					{
						var pairId = ticker.InvestingComPairId.Value;

						//<td class="pid-8831-last">2,719</td>
						var lastQuoteTd = $"<td class=\"pid-{pairId}-last\">";
						var indexOfLastQuoteTd = html.IndexOf(lastQuoteTd);

						var lastQuoteTdContent = html.Substring(indexOfLastQuoteTd + lastQuoteTd.Length);
						lastQuoteTdContent = lastQuoteTdContent.Remove(lastQuoteTdContent.IndexOf("</td>")).Trim().Replace(".", "");
						decimal lastQuote = 0;
						if (!Decimal.TryParse(lastQuoteTdContent, out lastQuote))
							continue;

						//<td class="bold greenFont pid-8831-pcp" >+0,85%</td>
						var trContent = html.Substring(indexOfLastQuoteTd);
						trContent = trContent.Remove(trContent.IndexOf("</tr>"));

						#region Извлекаем значение из столбца "Изм."
						var changeTd = $"pid-{pairId}-pc\"";
						var indexOfChangeTd = trContent.IndexOf(changeTd);
						if (indexOfChangeTd == -1)
							continue;

						var changeTdContent = trContent.Substring(indexOfChangeTd + changeTd.Length);
						changeTdContent = changeTdContent.Substring(changeTdContent.IndexOf(">") + ">".Length);
						changeTdContent = changeTdContent.Remove(changeTdContent.IndexOf("</td>")).Trim().Replace(".", "");

						decimal change = 0;
						if (!Decimal.TryParse(changeTdContent, out change))
							continue;
						#endregion

						#region Извлекаем значение из столбца "Изм.%"
						var changePercentTd = $"pid-{pairId}-pcp\"";
						var indexOfChangePercentTd = trContent.IndexOf(changePercentTd);
						if (indexOfChangePercentTd == -1)
							continue;

						var changePercentTdContent = trContent.Substring(indexOfChangePercentTd + changePercentTd.Length);
						changePercentTdContent = changePercentTdContent.Substring(changePercentTdContent.IndexOf(">") + ">".Length);
						changePercentTdContent = changePercentTdContent.Remove(changePercentTdContent.IndexOf("</td>")).Trim().Replace(".", "").Replace("%", "");

						decimal changePercent = 0;
						if (!Decimal.TryParse(changePercentTdContent, out changePercent))
							continue;
						#endregion

						#region Извлекаем значение из столбца "Время"
						var timeTd = $"pid-{pairId}-time\"";
						var indexOfTimeTd = trContent.IndexOf(timeTd);
						if (indexOfTimeTd == -1)
							continue;

						var timeTdContent = trContent.Substring(indexOfTimeTd + timeTd.Length);
						timeTdContent = timeTdContent.Substring(timeTdContent.IndexOf(">") + ">".Length);
						timeTdContent = timeTdContent.Remove(timeTdContent.IndexOf("</td>")).Trim();
						#endregion

						var lastQuoteStr = lastQuote.ToString().Replace(',', '.');
						var sqlDate = TextHelper.GetTSqlDate(DateTime.Now);
						var changeStr = change.ToString().Replace(',', '.');
						var changePercentStr = changePercent.ToString().Replace(',', '.');

						var updateLine = $"update dbo.Tickers " +
							$"set LastQuote = {lastQuoteStr}, " +
							$"LastQuoteDate = '{sqlDate}', " +
							$"ChangeFromYesterdayClose = {changeStr}, " +
							$"ChangeFromYesterdayClosePercent = {changePercentStr}, " +
							$"TimeStr = '{timeTdContent}' " +
							$"where Id = {ticker.Id}";
						updateLines.AppendLine(updateLine);
					}

					var sql = updateLines.ToString();
					TickersDAL.UpdateTickers(sql);
				}
				catch (Exception ex)
				{

				}

				Thread.Sleep(15000);
			}
		}

		public static void RefreshQuotesFromCbr()
		{
			Thread.Sleep(15000);

			int i = 0;
			var usdTickerId = 7/*USD ЦБ*/;
			var eurTickerId = 8/*EUR ЦБ*/;

			while (true)
			{
				i++;
				var tickers = TickersDAL.GetTickers().Where(t => t.Id == usdTickerId || t.Id == eurTickerId).ToList();
				var quotes = QuotesDAL.GetQuotes(fromDate: DateTime.Now.AddDays(-14));
				var updateLines = new StringBuilder();
				try
				{
					var xml = GetContentByUrl("http://www.cbr.ru/scripts/XML_daily.asp", "windows1251");

					int startCbrDateInd = xml.IndexOf("<ValCurs Date=\"");

					var cbrDateStr = xml.Substring(startCbrDateInd + "<ValCurs Date=\"".Length);
					cbrDateStr = cbrDateStr.Remove(cbrDateStr.IndexOf('"'));

					var dateParts = cbrDateStr.Split('.');
					var year = Convert.ToInt32(dateParts[2]);
					var month = Convert.ToInt32(dateParts[1]);
					var day = Convert.ToInt32(dateParts[0]);
					var cbrDate = new DateTime(year, month, day);
					var cbrSqlDate = TextHelper.GetTSqlDate(cbrDate);
					if (day != DateTime.Now.Day)
						continue;
					var lastQuoteSqlDate = TextHelper.GetTSqlDate(DateTime.Now);

					int ind1 = xml.IndexOf("<CharCode>USD</CharCode>");
					if (ind1 != -1)
					{
						string afterInd1 = xml.Substring(ind1);
						int ind2 = afterInd1.IndexOf("<Value>");
						string afterInd2 = afterInd1.Substring(ind2 + "<Value>".Length);
						string usdValue = afterInd2.Remove(afterInd2.IndexOf("</Value>")).Replace(',', '.');
						var updateLine = $"update dbo.Tickers " +
											$"set LastQuote = {usdValue}, " +
											$"LastQuoteDate = '{lastQuoteSqlDate}', " +
											$"CbrDate = '{cbrDate}' " +
											$"where Id = 7";
						updateLines.AppendLine(updateLine);

						var lastUsdQuoteInDb = quotes.Where(_ => _.TickerId == usdTickerId).OrderByDescending(_ => _.CbrDate).FirstOrDefault();
						if (lastUsdQuoteInDb == null || cbrDate > lastUsdQuoteInDb.CbrDate)
						{
							var newUsdQuote = new Quote()
							{
								TickerId = usdTickerId,
								CbrDate = cbrDate,
								Value = Convert.ToSingle(usdValue.Replace('.', ','))
							};

							QuotesDAL.AddQuote(newUsdQuote);

							quotes.Add(newUsdQuote);
						}
					}

					ind1 = xml.IndexOf("<CharCode>EUR</CharCode>");
					if (ind1 != -1)
					{
						string afterInd1 = xml.Substring(ind1);
						int ind2 = afterInd1.IndexOf("<Value>");
						string afterInd2 = afterInd1.Substring(ind2 + "<Value>".Length);
						string eurValue = afterInd2.Remove(afterInd2.IndexOf("</Value>")).Replace(',', '.');
						var updateLine = $"update dbo.Tickers " +
											$"set LastQuote = {eurValue}, " +
											$"LastQuoteDate = '{lastQuoteSqlDate}', " +
											$"CbrDate = '{cbrDate}' " +
											$"where Id = 8";
						updateLines.AppendLine(updateLine);

						var lastEurQuoteInDb = quotes.Where(_ => _.TickerId == eurTickerId).OrderByDescending(_ => _.CbrDate).FirstOrDefault();
						if (lastEurQuoteInDb == null || cbrDate > lastEurQuoteInDb.CbrDate)
						{
							var newEurQuote = new Quote()
							{
								TickerId = eurTickerId,
								CbrDate = cbrDate,
								Value = Convert.ToSingle(eurValue.Replace('.', ','))
							};

							QuotesDAL.AddQuote(newEurQuote);

							quotes.Add(newEurQuote);
						}
					}

					var sql = updateLines.ToString();
					TickersDAL.UpdateTickers(sql);
				}
				catch (Exception ex)
				{

				}

				Thread.Sleep(10 * 60 * 1000);//10 минут
			}
		}

		public static string GetContentByUrl(String url, string encoding = "UTF8")
		{
			string resHtml;
			try
			{
				StreamReader readStream;
				var request = (HttpWebRequest)WebRequest.Create(url);
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				request.UserAgent = "Google Chrome 75.0.3770.100";
				request.Accept = "*/*";
				var responce = (HttpWebResponse)request.GetResponse();
				var receiveStream = responce.GetResponseStream();
				switch (encoding.Replace("-", "").ToLower())
				{
					case "utf8":
						readStream = new StreamReader(receiveStream, Encoding.UTF8);
						break;

					case "windows1251":
						readStream = new StreamReader(receiveStream, Encoding.GetEncoding(1251));
						break;

					default:
						throw new Exception("Для кодировки " + encoding + " нужно дописать код");
				}
				resHtml = readStream.ReadToEnd();
				responce.Close();
				readStream.Close();

			}
			catch (Exception exc)
			{
				resHtml = exc.Message;
			}

			return resHtml;
		}

		public static List<Tag> GetTags(string tag, string html, string cssClass = null)
		{
			var result = new List<Tag>();

			html = RemoveScripts(html);

			int indexOfTag = html.IndexOf("<" + tag);
			while (indexOfTag != -1)
			{
				var tagObj = new Tag();
				var tmpHtml = html.Substring(indexOfTag);
				var ind1 = tmpHtml.IndexOf(">");
				var startTag = tmpHtml.Remove(ind1 + 1);

				bool includeObj = true;
				int indexOfClass = startTag.IndexOf(" class=\"");
				if (indexOfClass != -1)
				{
					var class0 = startTag.Substring(indexOfClass + " class=\"".Length);
					tagObj.Class = class0.Remove(class0.IndexOf("\""));

					if (cssClass != null && tagObj.Class != cssClass)
						includeObj = false;
				}
				else if (cssClass != null)
					includeObj = false;

				if (includeObj)
				{
					if (tag != "img")
					{
						var content = tmpHtml.Substring(startTag.Length);
						int indexOfEndTag0 = content.IndexOf("</" + tag + ">");
						if (indexOfEndTag0 != -1)
						{
							content = content.Remove(indexOfEndTag0).Trim();
							tagObj.Content = RemoveLinks(content);
						}
					}

					int indexOfId = startTag.IndexOf(" id=\"");
					if (indexOfId != -1)
					{
						var id = startTag.Substring(indexOfId + " id=\"".Length);
						tagObj.Id = id.Remove(id.IndexOf("\""));
					}

					int indexOfHref = startTag.IndexOf(" href=\"");
					if (indexOfHref != -1)
					{
						var href = startTag.Substring(indexOfHref + " href=\"".Length);
						tagObj.Href = href.Remove(href.IndexOf("\""));
					}

					int indexOfHref2 = startTag.IndexOf(" href='");
					if (indexOfHref2 != -1)
					{
						var href = startTag.Substring(indexOfHref2 + " href='".Length);
						tagObj.Href = href.Remove(href.IndexOf("'"));
					}

					var indexOfSrc = startTag.IndexOf(" src=\"");
					if (indexOfSrc != -1)
					{
						var src = startTag.Substring(indexOfSrc + " src=\"".Length);
						tagObj.Src = src.Remove(src.IndexOf("\""));
					}

					var indexOfDataSrc = startTag.IndexOf(" data-src=\"");
					if (indexOfDataSrc != -1)
					{
						var dataSrc = startTag.Substring(indexOfDataSrc + " data-src=\"".Length);
						tagObj.DataSrc = dataSrc.Remove(dataSrc.IndexOf("\""));
					}

					result.Add(tagObj);
				}

				int indexOfEndTag;
				int endTagLength;
				switch (tag)
				{
					case "img":
						indexOfEndTag = tmpHtml.IndexOf(">");
						endTagLength = ">".Length;
						break;

					default:
						indexOfEndTag = tmpHtml.IndexOf("</" + tag + ">");
						endTagLength = ("</" + tag + ">").Length;
						break;
				}

				if (indexOfEndTag == -1)
					break;

				html = tmpHtml.Substring(indexOfEndTag + endTagLength);
				indexOfTag = html.IndexOf("<" + tag);
			}

			return result;
		}

		public static string RemoveScripts(string html)
		{
			//удаление скриптов
			int indexOfScript = html.IndexOf("<script");
			while (indexOfScript != -1)
			{
				var beforeScript = html.Remove(indexOfScript);
				var tmpHtml = html.Substring(indexOfScript + "<script".Length);
				var afterScript = tmpHtml.Substring(tmpHtml.IndexOf("</script>") + "</script>".Length);
				html = beforeScript + afterScript;

				indexOfScript = html.IndexOf("<script");
			}

			return html;
		}

		public static string RemoveLinks(string html)
		{
			var tmpHtml = html;
			int indexOfA = tmpHtml.IndexOf("<a ");
			while (indexOfA != -1)
			{
				var regex = new Regex("Сбербанк - хватит копировать");
				if (regex.Matches(tmpHtml).Count > 1)
				{

				}

				var beforeA = tmpHtml.Remove(indexOfA);
				var textA = tmpHtml.Substring(indexOfA);
				var ind1 = textA.IndexOf(">");
				if (ind1 == -1)
					return tmpHtml;

				textA = textA.Substring(ind1 + 1);
				var ind2 = textA.IndexOf("</a>");
				if (ind2 == -1)
					return tmpHtml;

				textA = textA.Remove(ind2);

				var ind3 = tmpHtml.IndexOf("</a>");
				if (ind3 == -1)
					return tmpHtml;

				var afterA = tmpHtml.Substring(ind3 + "</a>".Length);
				tmpHtml = beforeA + textA + afterA;

				indexOfA = tmpHtml.IndexOf("<a ");

				if (indexOfA > 40000)
				{

				}
			}

			return tmpHtml;
		}
	}
}