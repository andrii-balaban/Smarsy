using System.Windows.Forms;

namespace SmarsyEntities
{
    using System;

    public class HomeWork : SmarsyElement
    {
        public int LessonId { get; set; }

        public string LessonName { get; set; }

        public string HomeWorkDescr { get; set; }

        public DateTime HomeWorkDate { get; set; }

        public int TeacherId { get; set; }

        public string TeacherName { get; set; }

        public override void ParseElementFrom(HtmlElement row)
        {
            var i = 0;
            foreach (HtmlElement cell in row.GetElementsByTagName("td"))
            {
                if (i == 1)
                {
                    HomeWorkDate = DateTime.Parse(ChangeDateFormat(cell.InnerText));
                }

                if (i++ == 2)
                {
                    HomeWorkDescr = cell.InnerText;
                }
            }
        }
    }
}