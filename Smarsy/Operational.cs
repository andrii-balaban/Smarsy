using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Smarsy.Logic;
using SmarsyEntities;

namespace Smarsy
{

    public class Operational
    {
        private readonly SqlServerLogic _sqlServerLogic;
        private List<MarksRowElement> _newMarks;
        public Student Student { get; set; }
        public WebBrowser SmarsyBrowser { get; set; }

        public Operational(string login)
        {
            Student = new Student()
            {
                Login = login
            };
            SmarsyBrowser = new WebBrowser();
            _sqlServerLogic = new SqlServerLogic();

        }


        public void InitStudentFromDb()
        {
            Student = _sqlServerLogic.GetStudentBySmarsyLogin(Student.Login);
        }

        public void GoToLink(string url)
        {
            SmarsyBrowser.Navigate(url);
            WaitForPageToLoad();
        }

        private void WaitForPageToLoad()
        {
            while (SmarsyBrowser.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
            Thread.Sleep(500);
        }


        private void FillUserName()
        {
            var element = SmarsyBrowser.Document.GetElementById("username");
            if (element != null)
                element.InnerText = Student.Login;
        }

        private void ClickOnLoginButton()
        {
            if (SmarsyBrowser.Document == null) return;
            var bclick = SmarsyBrowser.Document.GetElementsByTagName("input");
            foreach (HtmlElement btn in bclick)
            {
                var name = btn.Name;
                if (name == "submit")
                    btn.InvokeMember("click");
            }
        }

        private void FillPassword()
        {
            var element = SmarsyBrowser.Document.GetElementById("password");
            if (element != null)
                element.InnerText = Student.Password;
        }

        public void Login()
        {
            FillUserName();
            FillPassword();
            ClickOnLoginButton();

            WaitForPageToLoad();
        }

        public void LoginToSmarsy()
        {
            GoToLink("http://www.smarsy.ua");
            Login();
        }

        private MarksRowElement ProcessMarksRow(HtmlElement row)
        {
            var i = 0;
            var tmpList = new List<string>();
            var tmpMarks = row;
            var lessonName = "";

            foreach (HtmlElement cell in row.GetElementsByTagName("td"))
            {
                tmpList.Add(cell.InnerHtml);
                if (i == 1) lessonName = cell.InnerHtml;
                if (i == 3) tmpMarks = cell;
                i++;
            }

            var marks = new MarksRowElement()
            {
                LessonName = lessonName,
                Marks = new List<StudentMark>()
            };
            foreach (HtmlElement mark in tmpMarks.GetElementsByTagName("a"))
            {
                var studentMark = new StudentMark()
                {
                    Mark = int.Parse(mark.InnerText),
                    Reason = GetTextBetweenSubstrings(mark.GetAttribute("title"), "За что получена:", ""),
                    Date = GetDateFromComment(mark.GetAttribute("title"))
                };
                marks.Marks.Add(studentMark);
            }
            return marks;
        }

        public void UpdateMarks()
        {
            GoToLink("http://smarsy.ua/private/parent.php?jsid=Diary&child=" + Student.SmarsyChildId + "&tab=Mark");

            var tables = SmarsyBrowser.Document.GetElementsByTagName("table");
            var i = 0;
            var isHeader = true;
            // https://social.msdn.microsoft.com/Forums/en-US/62e0fcd1-3d44-4b34-aa38-0749678aa0b6/extract-a-value-of-cell-in-table-with-webbrowser?forum=vbgeneral

            var marks = new List<MarksRowElement>();
            foreach (HtmlElement el in tables)
            {
                if (i++ != 1) continue; // skip first table
                foreach (HtmlElement rows in el.All)
                {
                    foreach (HtmlElement row in rows.GetElementsByTagName("tr"))
                    {
                        if (isHeader)
                        {
                            isHeader = false;
                            continue;
                        }
                        marks.Add(ProcessMarksRow(row));
                    }
                }
            }
            _sqlServerLogic.UpsertLessons(marks.Select(x => x.LessonName).Distinct().ToList());
            _newMarks = _sqlServerLogic.UpserStudentAllLessonsMarks(Student.Login, marks);

        }


        private  string GenerateEmailBodyForHomeWork(List<HomeWork> hwList)
        {
            var result = "";
            var isFirst = true;
            foreach (var homeWork in hwList)
            {
                if (isFirst && ((TimeSpan)(homeWork.HomeWorkDate - DateTime.Now)).TotalDays > 1)
                {
                    result += Environment.NewLine;
                    result += Environment.NewLine;
                    isFirst = false;
                }
                result += homeWork.HomeWorkDate.ToShortDateString();
                result += " - ";
                result += homeWork.LessonName;
                result += " - ";

                result += homeWork.TeacherName;
                result += " - ";
                result += homeWork.HomeWorkDescr;
                result += Environment.NewLine;
            }
            return result;
        }

        private  string GetTeacherNameFromLessonWithTeacher(string lessonNameWithTeacher, string lessonName)
        {
            var result = lessonNameWithTeacher.Replace(lessonName, "").Replace("(", "").Replace(")", "").Trim();
            return result;
        }

        private  string GetLessonNameFromLessonWithTeacher(string lessonNameWithTeacher)
        {
            var result = lessonNameWithTeacher.Substring(0, lessonNameWithTeacher.IndexOf("(") - 1);
            return result;
        }

        private  string GenerateEmailBodyForMarks(List<MarksRowElement> marks)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var lesson in marks.OrderBy(x => x.LessonName).ToList())
            {
                sb.Append(lesson.LessonName);
                sb.Append(":");
                sb.Append(Environment.NewLine);
                foreach (var mark in lesson.Marks.OrderByDescending(x => x.Date))
                {
                    sb.Append(mark.Date.ToShortDateString());
                    sb.Append(" - ");
                    sb.Append(mark.Mark);
                    sb.Append(" - ");
                    sb.Append(mark.Reason);
                    sb.Append(Environment.NewLine);
                }
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }


        private  HomeWork ProccessHomeWork(HtmlElement row)
        {
            var result = new HomeWork();
            var i = 0;
            foreach (HtmlElement cell in row.GetElementsByTagName("td"))
            {
                if (i == 0)
                {
                    i++;
                    continue;
                }
                if (i == 1) result.HomeWorkDate = DateTime.Parse(ChangeDateFormat(cell.InnerText));
                if (i++ == 2)
                {
                    result.HomeWorkDescr = cell.InnerText;
                }
            }
            return result;
        }

        private  DateTime GetDateFromComment(string comment, bool isThisYear = true)
        {
            var year = isThisYear ? DateTime.Now.Year.ToString() : (DateTime.Now.Year - 1).ToString();
            var dateWithText = GetTextBetweenSubstrings(comment, "Дата оценки: ", ";");
            var date = dateWithText.Substring(3, dateWithText.Length - 3) + "." + year;

            var result = ChangeDateFormat(date);
            return DateTime.Parse(result);

        }

        private string ChangeDateFormat(string date)
        {
            return date.Substring(6, 4) + "." + date.Substring(3, 2) + "." + date.Substring(0, 2);
        }

        private string GetTextBetweenSubstrings(string text, string from, string to)
        {
            int pTo = 0;
            var pFrom = text.IndexOf(from, StringComparison.Ordinal) + from.Length;
            if (to.Length == 0) pTo = text.Length;
            else pTo = text.LastIndexOf(to, StringComparison.Ordinal);
            return text.Substring(pFrom, pTo - pFrom);
        }

        private int GetStudentId(WebBrowser webBrowser)
        {
            var pattern = new Regex("private/parent.php\\?jsid=Child&child=(?<login>[0-9]*)\"");
            var match = pattern.Match(webBrowser.DocumentText);
            var childId = int.Parse(match.Groups["login"].Value);
            return childId;
        }

        public void UpdateHomeWork()
        {
            GoToLink("http://smarsy.ua/private/parent.php?jsid=Homework&child=" + Student.SmarsyChildId + "&tab=Lesson");
            var homeWorks = new List<HomeWork>();
            var tables = SmarsyBrowser.Document.GetElementsByTagName("table");
            var separateLessonNameFromHomeWork = 0;
            var teacherId = 0;
            var lessonId = 0;

            foreach (HtmlElement el in tables)
            {

                if (separateLessonNameFromHomeWork++ % 2 == 0)
                {
                    var lessonNameWithTeacher = el.InnerText.Replace("\r\n", "");
                    var lessonName = GetLessonNameFromLessonWithTeacher(lessonNameWithTeacher);
                    var teacherName = GetTeacherNameFromLessonWithTeacher(lessonNameWithTeacher, lessonName);
                    teacherId = _sqlServerLogic.InsertTeacherIfNotExists(teacherName);
                    lessonId = _sqlServerLogic.GetLessonIdByLessonShortName(lessonName);
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
                            var tmp = ProccessHomeWork(row);
                            tmp.LessonId = lessonId;
                            tmp.TeacherId = teacherId;
                            if (tmp.HomeWorkDescr != null && !tmp.HomeWorkDescr.Trim().Equals(""))
                                homeWorks.Add(tmp);
                        }
                    }
                }
            }

            _sqlServerLogic.UpsertHomeWorks(homeWorks);
        }

        public void SendEmail()
        {
            var emailTo = "keyboards4everyone@gmail.com";
            var subject = "Лизины оценки (" + DateTime.Now.ToShortDateString() + ")";
            var emailBody = "";

            if (_newMarks.Any())
            {
                emailBody = GenerateEmailBodyForMarks(_newMarks);

            }

            emailBody += Environment.NewLine;
            emailBody += Environment.NewLine;
            emailBody += GenerateEmailBodyForHomeWork(_sqlServerLogic.GetHomeWorkForFuture());

            new EmailLogic().SendEmail(emailTo, subject, emailBody);
        }
    }
}
