using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NLog;
using SmarsyEntities;

namespace Smarsy
{
    public class SmarsyBrowser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SmarsyBrowser()
        {
            Browser = new WebBrowser();
        }

        private WebBrowser Browser { get; set; }

        public void GetTableObjectFromPage<T>(
            string url,
            Func<HtmlElement, T> methodName,
            string entityNameForLog,
            Action<IList<T>> databaseProcessingMethodName,
            int childId,
            bool isSkipHeader = true)
        {
            GoToLinkWithChild(url, childId);
            if (Browser.Document == null)
            {
                return;
            }

            var result = Browser.Document.GetElementsByTagName("table").OfType<HtmlElement>()
                .Skip(1) // skip the first table on the page
                .Take(1) // take the only second table on the page
                .SelectMany(row => row.GetElementsByTagName("tr").OfType<HtmlElement>())
                .Skip(isSkipHeader ? 1 : 0) // skip header row
                .Select(methodName).ToArray();

            Logger.Info($"Upserting {entityNameForLog} in database");
            databaseProcessingMethodName(result);
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

        public void ClickOnLoginButton()
        {
            if (Browser.Document == null)
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

        public void FillTextBoxByElementId(string elementId, string value)
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

        public void UpdateHomeWork(Operational operational)
        {
            GoToLink($"http://smarsy.ua/private/parent.php?jsid=Homework&child={operational.Student.SmarsyChildId}&tab=Lesson");

            if (Browser.Document == null)
            {
                return;
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
                    var lessonName = operational.GetLessonNameFromLessonWithTeacher(lessonNameWithTeacher);
                    var teacherName = operational.GetTeacherNameFromLessonWithTeacher(lessonNameWithTeacher, lessonName);
                    teacherId = operational.Repository.InsertTeacherIfNotExists(teacherName);
                    lessonId = operational.Repository.GetLessonIdByLessonShortName(lessonName);
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

                            var tmp = operational.ProccessHomeWork(row);
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
            operational.Repository.UpsertHomeWorks(homeWorks);
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