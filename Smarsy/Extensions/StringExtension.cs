namespace Smarsy.Extensions
{
    using System;

    public static class StringExtension
    {
        public static DateTime ConvertDateToRussianFormat(this string date)
        {
            var day = int.Parse(date.Substring(0, 2));
            var month = int.Parse(date.Substring(3, 2));
            var year = int.Parse(date.Substring(6, 4));

            return new DateTime(year, month, day);
        }
    }
}
