using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Smarsy.Logic;
using SmarsyEntities;

namespace Smarsy.SmarsyBrowser
{
    public class HomeworkPage : SmarsyPage<HomeWork>
    {
        private readonly ISmarsyRepository _smarsyRepository;

        public HomeworkPage(SmarsyStudent student,ISmarsyRepository repository) : base(student.SmarsyChildId)
        {
            _smarsyRepository = repository;
        }

        protected override string PageLink => "http://smarsy.ua/private/parent.php?jsid=Homework&tab=Lesson";

        public override IEnumerable<HomeWork> GetSmarsyElementsFromPage()
        {
            if (!IsPageLoaded())
                return Enumerable.Empty<HomeWork>();

            var homeWorks = new List<HomeWork>();
            var tables = Document.GetElementsByTagName("table");
            var separateLessonNameFromHomeWork = 0;
            var teacherId = 0;
            var lessonId = 0;

            foreach (HtmlElement el in tables)
            {
                if (separateLessonNameFromHomeWork++ % 2 == 0)
                {
                    var lessonNameWithTeacher = el.InnerText.Replace("\r\n", string.Empty);
                    var lessonName = GetLessonNameFromLessonWithTeacher(lessonNameWithTeacher);
                    var teacherName = GetTeacherNameFromLessonWithTeacher(lessonNameWithTeacher, lessonName);

                    teacherId = _smarsyRepository.InsertTeacherIfNotExists(teacherName);
                    lessonId = _smarsyRepository.GetLessonIdByLessonShortName(lessonName);
                }
                else
                {
                    foreach (HtmlElement rows in el.All)
                    {
                        var isHeader = true;
                        foreach (HtmlElement row in rows.GetElementsByTagName("tr"))
                        {
                            if (isHeader)
                            {
                                isHeader = false;
                                continue;
                            }

                            var homework = new HomeWork();
                            homework.ParseFromHtmlElement(row);

                            homework.LessonId = lessonId;
                            homework.TeacherId = teacherId;

                            if ((homework.HomeWorkDescr != null) && !homework.HomeWorkDescr.Trim().Equals(string.Empty))
                            {
                                homeWorks.Add(homework);
                            }
                        }
                    }
                }
            }

            return homeWorks;
        }

        private string GetLessonNameFromLessonWithTeacher(string lessonNameWithTeacher)
        {
            if (!lessonNameWithTeacher.Contains("("))
            {
                return lessonNameWithTeacher;
            }

            return lessonNameWithTeacher.Substring(0, lessonNameWithTeacher.IndexOf("(", StringComparison.Ordinal) - 1);
        }

        private string GetTeacherNameFromLessonWithTeacher(string lessonNameWithTeacher, string lessonName)
        {
            return lessonNameWithTeacher.Replace(lessonName, string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Trim();
        }
    }
}
