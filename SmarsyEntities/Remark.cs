using System.Windows.Forms;

namespace SmarsyEntities
{
    using System;

    public class Remark : SmarsyElement<Remark>
    {
        public DateTime RemarkDate { get; set; }

        public string RemarkText { get; set; }

        public int LessonId { get; set; }

        public string LessonName { get; set; }
        public override Remark GetElement(HtmlElement row)
        {
            var remark = new Remark();
            var i = 0;

            foreach (HtmlElement element in row.GetElementsByTagName("td"))
            {
                if (i == 0)
                {
                    remark.RemarkDate = ConvertDateToRussianFormat(element.InnerHtml);
                }

                if (i == 1)
                {
                    remark.LessonName = element.InnerText;
                }

                if (i == 2)
                {
                    remark.RemarkText = element.InnerText;
                }

                i++;
            }

            return remark;
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