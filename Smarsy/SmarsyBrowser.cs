using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NLog;
using SmarsyEntities;

namespace Smarsy
{
    public class SmarsyBrowser : ISmarsyBrowser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly SmarsyEntitiesFactory _smarsyEntitiesFactory;

        public SmarsyBrowser(SmarsyEntitiesFactory entityFactory)
        {
            _smarsyEntitiesFactory = entityFactory;
            Browser = new WebBrowser();
        }

        private WebBrowser Browser { get; set; }

        public IEnumerable<T> GetTableObjectFromPage<T>(string url, string entityNameForLog, int childId, bool isSkipHeader = true) where T : SmarsyElement
        {
            GoToLinkWithChild(url, childId);

            if (!IsPageLoaded())
            {
                return Enumerable.Empty<T>();
            }

            IEnumerable<T> result = Browser.Document.GetElementsByTagName("table").OfType<HtmlElement>()
                .Skip(1) // skip the first table on the page
                .Take(1) // take the only second table on the page
                .SelectMany(row => row.GetElementsByTagName("tr").OfType<HtmlElement>())
                .Skip(isSkipHeader ? 1 : 0) // skip header row
                .Select(_smarsyEntitiesFactory.CreateElementOfType<T>)
                .ToArray();


            return result;
        }

        private bool IsPageLoaded()
        {
            return Browser.Document == null;
        }

        private void GoToLinkWithChild(string url, int childId)
        {
            GoToLink($"{url}&child={childId}");
        }

        public void GoToLink(string url)
        {
            Logger.Info($"Go to {url} page");
            Browser.Navigate(url);
            WaitForPageToLoad();
        }

        public void WaitForPageToLoad()
        {
            while (Browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            Thread.Sleep(500);
        }

        private void ClickOnLoginButton()
        {
            if (!IsPageLoaded())
            {
                return;
            }

            var bclick = Browser.Document.GetElementsByTagName("input");
            foreach (HtmlElement btn in bclick)
            {
                var name = btn.Name;
                if (name == "submit")
                {
                    btn.InvokeMember("click");
                }
            }
        }

        private void FillTextBoxByElementId(string elementId, string value)
        {
            if (Browser == null || Browser.Document == null || elementId == null)
            {
                return;
            }

            Logger.Info($"Entering text to the {elementId} element");

            var element = Browser.Document.GetElementById(elementId);
            if (element != null)
            {
                element.InnerText = value;
            }
        }

        public IEnumerable<HomeWork> UpdateHomeWork(SmarsyOperations smarsyOperations, int smarsyChildId)
        {
            GoToLink($"http://smarsy.ua/private/parent.php?jsid=Homework&child={smarsyChildId}&tab=Lesson");

            if (Browser.Document == null)
            {
                return Enumerable.Empty<HomeWork>();
            }

            var homeWorks = new List<HomeWork>();
            var tables = Browser.Document.GetElementsByTagName("table");
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

                    teacherId = smarsyOperations.Repository.InsertTeacherIfNotExists(teacherName);
                    lessonId = smarsyOperations.Repository.GetLessonIdByLessonShortName(lessonName);
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

                            var tmp = _smarsyEntitiesFactory.CreateElementOfType<HomeWork>(row);
                            tmp.LessonId = lessonId;
                            tmp.TeacherId = teacherId;

                            if ((tmp.HomeWorkDescr != null) && !tmp.HomeWorkDescr.Trim().Equals(string.Empty))
                            {
                                homeWorks.Add(tmp);
                            }
                        }
                    }
                }
            }

            Logger.Info("Upserting homeworks in database");

            return homeWorks;
        }

        public string GetLessonNameFromLessonWithTeacher(string lessonNameWithTeacher)
        {
            if (!lessonNameWithTeacher.Contains("("))
            {
                return lessonNameWithTeacher;
            }

            return lessonNameWithTeacher.Substring(0, lessonNameWithTeacher.IndexOf("(", StringComparison.Ordinal) - 1);
        }

        public string GetTeacherNameFromLessonWithTeacher(string lessonNameWithTeacher, string lessonName)
        {
            var result = lessonNameWithTeacher.Replace(lessonName, string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Trim();
            return result;
        }

        public void Login(Student student)
        {
            FillTextBoxByElementId("username", student.Login);
            FillTextBoxByElementId("password", student.Password);
            ClickOnLoginButton();

            WaitForPageToLoad();
        }
    }
}