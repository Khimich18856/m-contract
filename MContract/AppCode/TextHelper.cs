using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace MContract.AppCode
{
    public class TextHelper
	{
		//private object content;

		private static Dictionary<string, char> __unicodeChars;

		static TextHelper()
		{
			FillUnicodeChars();
		}

		private static void FillUnicodeChars()
		{
            //http://www.fileformat.info/info/unicode/char/430/index.htm
            __unicodeChars = new Dictionary<string, char>
            {
                { @"u0410", 'А' },
                { @"u0411", 'Б' },
                { @"u0412", 'В' },
                { @"u0413", 'Г' },
                { @"u0414", 'Д' },
                { @"u0415", 'Е' },
                { @"u0416", 'Ж' },
                { @"u0417", 'З' },
                { @"u0418", 'И' },
                { @"u0419", 'Й' },
                { @"u041a", 'К' },
                { @"u041b", 'Л' },
                { @"u041c", 'М' },
                { @"u041d", 'Н' },
                { @"u041e", 'О' },
                { @"u041f", 'П' },
                { @"u0420", 'Р' },
                { @"u0421", 'С' },
                { @"u0422", 'Т' },
                { @"u0423", 'У' },
                { @"u0424", 'Ф' },
                { @"u0425", 'Х' },
                { @"u0426", 'Ц' },
                { @"u0427", 'Ч' },
                { @"u0428", 'Ш' },
                { @"u0429", 'Щ' },
                { @"u042a", 'Ъ' },
                { @"u042b", 'Ы' },
                { @"u042c", 'Ь' },
                { @"u042d", 'Э' },
                { @"u042e", 'Ю' },
                { @"u042f", 'Я' },
                { @"u0430", 'а' },
                { @"u0431", 'б' },
                { @"u0432", 'в' },
                { @"u0433", 'г' },
                { @"u0434", 'д' },
                { @"u0435", 'е' },
                { @"u0436", 'ж' },
                { @"u0437", 'з' },
                { @"u0438", 'и' },
                { @"u0439", 'й' },
                { @"u043a", 'к' },
                { @"u043b", 'л' },
                { @"u043c", 'м' },
                { @"u043d", 'н' },
                { @"u043e", 'о' },
                { @"u043f", 'п' },
                { @"u0440", 'р' },
                { @"u0441", 'с' },
                { @"u0442", 'т' },
                { @"u0443", 'у' },
                { @"u0444", 'ф' },
                { @"u0445", 'х' },
                { @"u0446", 'ц' },
                { @"u0447", 'ч' },
                { @"u0448", 'ш' },
                { @"u0449", 'щ' },
                { @"u044a", 'ъ' },
                { @"u044b", 'ы' },
                { @"u044c", 'ь' },
                { @"u044d", 'э' },
                { @"u044e", 'ю' },
                { @"u044f", 'я' },
                { @"u0451", 'ё' }
            };
        }

		//private static char GetCharByUnicodeCode(string code)
		//{
		//	if (__unicodeChars.ContainsKey(code))
		//		return __unicodeChars[code];
		//	else
		//	{
		//		//LogsDAL.AddError("in App_Code/TextHelper.GetCharByUnicodeCode() code=" + code);
		//		return '?';
		//	}
		//}

		public static string GetStringFromUnicodeString(string text)
		{
			string result = text;

			foreach (KeyValuePair<string, char> kvp in __unicodeChars)
			{
				result = result.Replace(kvp.Key, kvp.Value.ToString());
			}

			return result;
		}

		public static Dictionary<char, string> GetTranslitRussianCharsRules()
		{
            Dictionary<char, string> translitRules = new Dictionary<char, string>
            {
                { 'а', "a" },
                { 'А', "A" },
                { 'б', "b" },
                { 'Б', "B" },
                { 'в', "v" },
                { 'В', "V" },
                { 'г', "g" },
                { 'Г', "G" },
                { 'д', "d" },
                { 'Д', "D" },
                { 'е', "e" },
                { 'Е', "E" },
                { 'ё', "yo" },
                { 'Ё', "YO" },
                { 'ж', "zh" },
                { 'Ж', "ZH" },
                { 'з', "z" },
                { 'З', "Z" },
                { 'и', "i" },
                { 'И', "I" },
                { 'й', "y" },
                { 'Й', "Y" },
                { 'к', "k" },
                { 'К', "K" },
                { 'л', "l" },
                { 'Л', "L" },
                { 'м', "m" },
                { 'М', "M" },
                { 'н', "n" },
                { 'Н', "N" },
                { 'о', "o" },
                { 'О', "O" },
                { 'п', "p" },
                { 'П', "P" },
                { 'р', "r" },
                { 'Р', "R" },
                { 'с', "s" },
                { 'С', "S" },
                { 'т', "t" },
                { 'Т', "T" },
                { 'у', "u" },
                { 'У', "U" },
                { 'ф', "f" },
                { 'Ф', "F" },
                { 'х', "h" },
                { 'Х', "H" },
                { 'ц', "ts" },
                { 'Ц', "TS" },
                { 'ч', "ch" },
                { 'Ч', "CH" },
                { 'ш', "sh" },
                { 'Ш', "SH" },
                { 'щ', "sch" },
                { 'Щ', "SCH" },
                { 'ъ', "" },
                { 'Ъ', "" },
                { 'ы', "y" },
                { 'Ы', "Y" },
                { 'ь', "" },
                { 'Ь', "" },
                { 'э', "e" },
                { 'Э', "E" },
                { 'ю', "yu" },
                { 'Ю', "YU" },
                { 'я', "ya" },
                { 'Я', "YA" }
            };

            return translitRules;
		}

		public static bool IsEnglishLetter(char ch)
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
			Dictionary<char, string> translitRussianCharsRules = GetTranslitRussianCharsRules();

			string englishString = string.Empty;
			foreach (char ch in russianString)
			{
				if (IsEnglishLetter(ch) || Char.IsDigit(ch))
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