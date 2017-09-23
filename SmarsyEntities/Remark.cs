using System.Windows.Forms;

namespace SmarsyEntities
{
    using System;

    public class Remark : SmarsyElement
    {
        public DateTime RemarkDate { get; set; }

        public string RemarkText { get; set; }

        public int LessonId { get; set; }

        public string LessonName { get; set; }
        public override void ParseElementFrom(HtmlElement row)
        {
            var i = 0;

            foreach (HtmlElement element in row.GetElementsByTagName("td"))
            {
                if (i == 0)
                {
                    RemarkDate = ConvertDateToRussianFormat(element.InnerHtml);
                }

                if (i == 1)
                {
                    LessonName = element.InnerText;
                }

                if (i == 2)
                {
                    RemarkText = element.InnerText;
                }

                i++;
            }
        }

        private DateTime ConvertDateToRussianFormat(string date)
        {
            var day = int.Parse(date.Substring(0, 2));
            var month = int.Parse(date.Substring(3, 2));
            var year = int.Parse(date.Substring(6, 4));

            return new DateTime(year, month, day);
        }
    }
}