using System;
using System.Windows.Forms;

namespace SmarsyEntities
{
    using System.Collections.Generic;

    public class LessonMark : SmarsyElement
    {
        public List<StudentMark> Marks { get; set; }

        public string LessonName { get; set; }

        public int LessonId { get; set; }

        public override void ParseFromHtmlElement(HtmlElement row)
        {
            var i = 0;
            var tmpMarks = row;
            var lessonName = string.Empty;

            foreach (HtmlElement cell in row.GetElementsByTagName("td"))
            {
                if (i == 1)
                {
                    lessonName = cell.InnerHtml;
                }

                if (i == 3)
                {
                    tmpMarks = cell;
                }

                i++;
            }


            LessonName = lessonName;
            Marks = new List<StudentMark>();

            foreach (HtmlElement mark in tmpMarks.GetElementsByTagName("a"))
            {
                var studentMark = new StudentMark
                {
                    Mark = int.Parse(mark.InnerText),
                    Reason = TextProcessor.Processor.GetTextBetweenSubstrings(mark.GetAttribute("title"), "За что получена:", string.Empty),
                    Date = GetDateFromComment(mark.GetAttribute("title"))
                };

                Marks.Add(studentMark);
            }

        }

        private DateTime GetDateFromComment(string comment, bool isThisYear = true)
        {
            var year = isThisYear ? DateTime.Now.Year.ToString() : (DateTime.Now.Year - 1).ToString();
            var dateWithText = TextProcessor.Processor.GetTextBetweenSubstrings(comment, "Дата оценки: ", ";");
            var date = dateWithText.Substring(3, dateWithText.Length - 3) + "." + year;

            var result = ChangeDateFormat(date);
            return DateTime.Parse(result);
        }
    }
}