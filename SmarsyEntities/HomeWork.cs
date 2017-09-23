using System.Windows.Forms;

namespace SmarsyEntities
{
    using System;

    public class HomeWork : SmarsyElement<HomeWork>
    {
        public int LessonId { get; set; }

        public string LessonName { get; set; }

        public string HomeWorkDescr { get; set; }

        public DateTime HomeWorkDate { get; set; }

        public int TeacherId { get; set; }

        public string TeacherName { get; set; }

        public override HomeWork GetElement(HtmlElement row)
        {
            var result = new HomeWork();
            var i = 0;
            foreach (HtmlElement cell in row.GetElementsByTagName("td"))
            {
                if (i == 1)
                {
                    result.HomeWorkDate = DateTime.Parse(ChangeDateFormat(cell.InnerText));
                }

                if (i++ == 2)
                {
                    result.HomeWorkDescr = cell.InnerText;
                }
            }

            return result;
        }
    }
}