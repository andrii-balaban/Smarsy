using System;

namespace SmarsyEntities
{
    public class TextProcessor
    {
        public string GetTextBetweenSubstrings(string text, string from, string to)
        {
            var charFrom = text.IndexOf(@from, StringComparison.Ordinal) + @from.Length;
            var charTo = to.Length == 0 ? text.Length : text.LastIndexOf(to, StringComparison.Ordinal);
            return text.Substring(charFrom, charTo - charFrom);
        }

        public string ChangeDateFormat(string date)
        {
            return date.Substring(6, 4) + "." + date.Substring(3, 2) + "." + date.Substring(0, 2);
        }

        public DateTime GetDateFromText(string birthDate, int studentAge)
        {
            var year = DateTime.Now.Year - studentAge;
            var month = GetMonthFromRussianName(GetMonthNameFromStringWithDayNumber(birthDate));
            var day = GetDayFromStringWithDayNumber(birthDate);
            var tmpDate = new DateTime(DateTime.Now.Year, month, day);

            if (DateTime.Now < tmpDate)
            {
                year--;
            }

            return new DateTime(year, month, day);
        }

        private static int GetMonthFromRussianName(string name)
        {
            switch (name)
            {
                case "января":
                    return 1;
                case "февраля":
                    return 2;
                case "марта":
                    return 3;
                case "апреля":
                    return 4;
                case "мая":
                    return 5;
                case "июня":
                    return 6;
                case "июля":
                    return 7;
                case "августа":
                    return 8;
                case "сентября":
                    return 9;
                case "октября":
                    return 10;
                case "ноября":
                    return 11;
                case "декабря":
                    return 12;
            }

            return -1;
        }

        private static int GetDayFromStringWithDayNumber(string date)
        {
            return int.Parse(date.Substring(0, 2).Trim());
        }

        private static string GetMonthNameFromStringWithDayNumber(string date)
        {
            return date.Substring(2, date.Length - 2).Trim();
        }
    }
}