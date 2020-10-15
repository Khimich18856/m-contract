using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace MContract.AppCode
{
	public class TextHelper
	{
		private object content;
		private static Dictionary<string, char> __unicodeChars;

		static TextHelper()
		{
			FillUnicodeChars();
		}

		private static void FillUnicodeChars()
		{
			//http://www.fileformat.info/info/unicode/char/430/index.htm
			__unicodeChars = new Dictionary<string, char>();
			__unicodeChars.Add(@"u0410", 'А');
			__unicodeChars.Add(@"u0411", 'Б');
			__unicodeChars.Add(@"u0412", 'В');
			__unicodeChars.Add(@"u0413", 'Г');
			__unicodeChars.Add(@"u0414", 'Д');
			__unicodeChars.Add(@"u0415", 'Е');
			__unicodeChars.Add(@"u0416", 'Ж');
			__unicodeChars.Add(@"u0417", 'З');
			__unicodeChars.Add(@"u0418", 'И');
			__unicodeChars.Add(@"u0419", 'Й');
			__unicodeChars.Add(@"u041a", 'К');
			__unicodeChars.Add(@"u041b", 'Л');
			__unicodeChars.Add(@"u041c", 'М');
			__unicodeChars.Add(@"u041d", 'Н');
			__unicodeChars.Add(@"u041e", 'О');
			__unicodeChars.Add(@"u041f", 'П');
			__unicodeChars.Add(@"u0420", 'Р');
			__unicodeChars.Add(@"u0421", 'С');
			__unicodeChars.Add(@"u0422", 'Т');
			__unicodeChars.Add(@"u0423", 'У');
			__unicodeChars.Add(@"u0424", 'Ф');
			__unicodeChars.Add(@"u0425", 'Х');
			__unicodeChars.Add(@"u0426", 'Ц');
			__unicodeChars.Add(@"u0427", 'Ч');
			__unicodeChars.Add(@"u0428", 'Ш');
			__unicodeChars.Add(@"u0429", 'Щ');
			__unicodeChars.Add(@"u042a", 'Ъ');
			__unicodeChars.Add(@"u042b", 'Ы');
			__unicodeChars.Add(@"u042c", 'Ь');
			__unicodeChars.Add(@"u042d", 'Э');
			__unicodeChars.Add(@"u042e", 'Ю');
			__unicodeChars.Add(@"u042f", 'Я');
			__unicodeChars.Add(@"u0430", 'а');
			__unicodeChars.Add(@"u0431", 'б');
			__unicodeChars.Add(@"u0432", 'в');
			__unicodeChars.Add(@"u0433", 'г');
			__unicodeChars.Add(@"u0434", 'д');
			__unicodeChars.Add(@"u0435", 'е');
			__unicodeChars.Add(@"u0436", 'ж');
			__unicodeChars.Add(@"u0437", 'з');
			__unicodeChars.Add(@"u0438", 'и');
			__unicodeChars.Add(@"u0439", 'й');
			__unicodeChars.Add(@"u043a", 'к');
			__unicodeChars.Add(@"u043b", 'л');
			__unicodeChars.Add(@"u043c", 'м');
			__unicodeChars.Add(@"u043d", 'н');
			__unicodeChars.Add(@"u043e", 'о');
			__unicodeChars.Add(@"u043f", 'п');
			__unicodeChars.Add(@"u0440", 'р');
			__unicodeChars.Add(@"u0441", 'с');
			__unicodeChars.Add(@"u0442", 'т');
			__unicodeChars.Add(@"u0443", 'у');
			__unicodeChars.Add(@"u0444", 'ф');
			__unicodeChars.Add(@"u0445", 'х');
			__unicodeChars.Add(@"u0446", 'ц');
			__unicodeChars.Add(@"u0447", 'ч');
			__unicodeChars.Add(@"u0448", 'ш');
			__unicodeChars.Add(@"u0449", 'щ');
			__unicodeChars.Add(@"u044a", 'ъ');
			__unicodeChars.Add(@"u044b", 'ы');
			__unicodeChars.Add(@"u044c", 'ь');
			__unicodeChars.Add(@"u044d", 'э');
			__unicodeChars.Add(@"u044e", 'ю');
			__unicodeChars.Add(@"u044f", 'я');
			__unicodeChars.Add(@"u0451", 'ё');
		}

		private static char GetCharByUnicodeCode(string code)
		{
			if (__unicodeChars.ContainsKey(code))
				return __unicodeChars[code];
			else
			{
				//LogsDAL.AddError("in App_Code/TextHelper.GetCharByUnicodeCode() code=" + code);
				return '?';
			}
		}

		public static string GetStringFromUnicodeString(string text)
		{
			string result = text;

			foreach (KeyValuePair<string, char> kvp in __unicodeChars)
			{
				result = result.Replace(kvp.Key, kvp.Value.ToString());
			}

			return result;
		}

		public static Dictionary<char, string> getTranslitRussianCharsRules()
		{
			Dictionary<char, string> translitRules = new Dictionary<char, string>();
			translitRules.Add('а', "a");
			translitRules.Add('А', "A");
			translitRules.Add('б', "b");
			translitRules.Add('Б', "B");
			translitRules.Add('в', "v");
			translitRules.Add('В', "V");
			translitRules.Add('г', "g");
			translitRules.Add('Г', "G");
			translitRules.Add('д', "d");
			translitRules.Add('Д', "D");
			translitRules.Add('е', "e");
			translitRules.Add('Е', "E");
			translitRules.Add('ё', "yo");
			translitRules.Add('Ё', "YO");
			translitRules.Add('ж', "zh");
			translitRules.Add('Ж', "ZH");
			translitRules.Add('з', "z");
			translitRules.Add('З', "Z");
			translitRules.Add('и', "i");
			translitRules.Add('И', "I");
			translitRules.Add('й', "y");
			translitRules.Add('Й', "Y");
			translitRules.Add('к', "k");
			translitRules.Add('К', "K");
			translitRules.Add('л', "l");
			translitRules.Add('Л', "L");
			translitRules.Add('м', "m");
			translitRules.Add('М', "M");
			translitRules.Add('н', "n");
			translitRules.Add('Н', "N");
			translitRules.Add('о', "o");
			translitRules.Add('О', "O");
			translitRules.Add('п', "p");
			translitRules.Add('П', "P");
			translitRules.Add('р', "r");
			translitRules.Add('Р', "R");
			translitRules.Add('с', "s");
			translitRules.Add('С', "S");
			translitRules.Add('т', "t");
			translitRules.Add('Т', "T");
			translitRules.Add('у', "u");
			translitRules.Add('У', "U");
			translitRules.Add('ф', "f");
			translitRules.Add('Ф', "F");
			translitRules.Add('х', "h");
			translitRules.Add('Х', "H");
			translitRules.Add('ц', "ts");
			translitRules.Add('Ц', "TS");
			translitRules.Add('ч', "ch");
			translitRules.Add('Ч', "CH");
			translitRules.Add('ш', "sh");
			translitRules.Add('Ш', "SH");
			translitRules.Add('щ', "sch");
			translitRules.Add('Щ', "SCH");
			translitRules.Add('ъ', "");
			translitRules.Add('Ъ', "");
			translitRules.Add('ы', "y");
			translitRules.Add('Ы', "Y");
			translitRules.Add('ь', "");
			translitRules.Add('Ь', "");
			translitRules.Add('э', "e");
			translitRules.Add('Э', "E");
			translitRules.Add('ю', "yu");
			translitRules.Add('Ю', "YU");
			translitRules.Add('я', "ya");
			translitRules.Add('Я', "YA");

			return translitRules;
		}

		public static bool isEnglishLetter(char ch)
		{
			if (ch >= 'A' && ch <= 'z')
				return true;
			else
				return false;
		}

		public static string GetNameForUrl(string name)
		{
			name = name.Replace(" ", "-").ToLower();
			return TranslitRussianStringToEnglish(name);
		}

		public static string TranslitRussianStringToEnglish(string russianString)
		{
			Dictionary<char, string> translitRussianCharsRules = getTranslitRussianCharsRules();

			string englishString = string.Empty;
			foreach (char ch in russianString)
			{
				if (isEnglishLetter(ch) || Char.IsDigit(ch))
				{
					englishString += ch;
					continue;
				}
				if (translitRussianCharsRules.ContainsKey(ch))
					englishString += translitRussianCharsRules[ch];
				else if (ch == '-')
					englishString += ch;
					 //else
					//englishString += '-';
			}

			return englishString;
		}

		/// <summary>
		/// Возвращает строку вида: 10 января
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static string GetDayAndMonthStr(DateTime date)
		{
			var result = date.Day.ToString() + " ";
			switch (date.Month)
			{
				case 1: result += "января"; break;
				case 2: result += "февраля"; break;
				case 3: result += "марта"; break;
				case 4: result += "апреля"; break;
				case 5: result += "мая"; break;
				case 6: result += "июня"; break;
				case 7: result += "июля"; break;
				case 8: result += "августа"; break;
				case 9: result += "сентября"; break;
				case 10: result += "октября"; break;
				case 11: result += "ноября"; break;
				case 12: result += "декабря"; break;
			}

			return result;
		}

		/// <summary>
		/// Возвращает строку вида: 3 часа 20 минут назад
		/// </summary>
		public static string GetDateTimeForScreen(DateTime dateTime, int deltaHours)
		{
			string result = String.Empty;

			DateTime now = DateTime.Now.AddHours(deltaHours);

			TimeSpan elapsed = now - dateTime;

			if (elapsed.Days > 30)
			{
				int months = elapsed.Days / 30;
				result = (months > 1 ? months + " " : "") + Ending(months, "месяц", "меcяца", "месяцев") + " назад";
			}
			else if (elapsed.Days >= 7)
			{
				int weeks = elapsed.Days / 7;
				if (weeks == 1)
					result = "неделю назад";
				else
					result = (weeks > 1 ? weeks + " " : "") + Ending(weeks, "неделю", "недели", "недель") + " назад";
			}
			else if (elapsed.Days > 1)
				result = elapsed.Days + " " + Ending(elapsed.Days, "день", "дня", "дней") + " назад";
			else if (elapsed.Days == 1)
			{
				if (dateTime.Date.AddDays(1) == now.Date)
					result = "вчера в " + dateTime.Hour + ":" + (dateTime.Minute < 10 ? "0" : "") + dateTime.Minute;
				else if (dateTime.Date.AddDays(2) == now.Date)
					result = "позавчера в " + dateTime.Hour + ":" + (dateTime.Minute < 10 ? "0" : "") + dateTime.Minute;
			}
			else if (elapsed.Hours > 0)
			{
				if (elapsed.Hours == 1)
					result = "час назад";
				else
					result = elapsed.Hours + " " + Ending(elapsed.Hours, "час", "часа", "часов") + " назад";
			}
			else if (elapsed.Minutes > 30)
				result = "полчаса назад";
			else if (elapsed.Minutes > 5)
				result = "несколько минут назад";
			else if (elapsed.Minutes > 0)
			{
				if (elapsed.Minutes == 1)
					result = "минуту назад";
				else
					result = elapsed.Minutes + " " + Ending(elapsed.Minutes, "минуту", "минуты", "минут") + " назад";
			}
			else
				result = "только что";//elapsed.Seconds + " " + Endings(elapsed.Seconds, "секунду", "секунды", "секунд") + " назад";

			return result;
		}

		public static string Ending(int number, string edinstv, string rodEdinstv, string rodMnoz)
		{
			int lastNum = number % 10;
			int lastTwoNum = number % 100;
			if (lastNum == 1 && lastTwoNum != 11)
				return edinstv;
			if (lastNum == 2 && lastTwoNum != 12 || lastNum == 3 && lastTwoNum != 13 || lastNum == 4 && lastTwoNum != 14)
				return rodEdinstv;
			return rodMnoz;
		}

		public static string GetString(decimal f, int? precision = null)
		{
			return GetString((float)f, precision);
		}

		public static string GetString(double f, int? precision = null)
		{
			return GetString((float)f, precision);
		}

		public static string GetString(float f, int? precision = null)
		{
			if (precision.HasValue)
			{
				switch (precision.Value)
				{
					case 0: return String.Format("{0:0}", f);
					case 1: return String.Format("{0:0.#}", f);
					case 2: return String.Format("{0:0.##}", f);
					case 3: return String.Format("{0:0.###}", f);
				}
			}

			if (f > 100)
				return String.Format("{0:0}", f);
			if (f > 10)
				return String.Format("{0:0.#}", f);
			if (f > 1)
				return String.Format("{0:0.##}", f);

			return String.Format("{0:0.###}", f);
		}

		/// <summary>
		/// Возвращает строку вида 01.10.1984 13:25
		/// </summary>
		public static string GetDateTimeStr(DateTime d)
		{
			return d.ToString("dd.MM.yyyy H:mm");
			//var day = d.Day < 10 ? "0" + d.Day : d.Day.ToString();
			//var month = d.Month < 10 ? "0" + d.Month : d.Month.ToString();
			//var year = d.Year.ToString();
			//var hour = d.Hour < 10 ? "0" + d.Hour : d.Hour.ToString();
			//var minute = d.Minute < 10 ? "0" + d.Minute : d.Minute.ToString();

			//return day + "." + month + "." + year + " " + hour + ":" + minute;
		}

		/// <summary>
		/// Возвращает строку вида 01.10.1984
		/// </summary>
		public static string GetDateStr(DateTime d)
		{
			var day = d.Day < 10 ? "0" + d.Day : d.Day.ToString();
			var month = d.Month < 10 ? "0" + d.Month : d.Month.ToString();
			var year = d.Year.ToString();

			return day + "." + month + "." + year;
		}

		/// <summary>
		/// Возвращает строку вида 07.05
		/// </summary>
		public static string GetDateShortStr(DateTime d)
		{
			var day = d.Day < 10 ? "0" + d.Day : d.Day.ToString();
			var month = d.Month < 10 ? "0" + d.Month : d.Month.ToString();

			return day + "." + month;
		}

		public static string GetOnlyTime(DateTime d)
		{
			var hour = d.Hour < 10 ? "0" + d.Hour : d.Hour.ToString();
			var minute = d.Minute < 10 ? "0" + d.Minute : d.Minute.ToString();

			return hour + ":" + minute;
		}

		public static string GetSafeIdsStr(string idsStr)
		{
			var ids = idsStr.Split(',').ToList();

			var idsSafe = new List<int>();
			foreach (var id in ids)
			{
				idsSafe.Add(Convert.ToInt32(id));
			}

			return String.Join(",", idsSafe);
		}

		public static Dictionary<string, string> GetObjectFromJsonString(string jsonString)
		{
			var ser = new JavaScriptSerializer();
			return ser.Deserialize<Dictionary<string, string>>(jsonString); 
		}

		public static string GetTSqlDate(DateTime d)
		{
			//Формат даты T-SQL: 2018-05-01T00:00:00.000  //1 мая 2018
			return d.Year.ToString() + "-" + GetWith0(d.Month) + "-" + GetWith0(d.Day) + "T" + GetWith0(d.Hour) + ":" + GetWith0(d.Minute) + ":" + GetWith0(d.Second) + ".000";
		}

		public static string GetWith0(int figure)
		{
			return figure < 10 ? "0" + figure : figure.ToString();
		}
	}
}