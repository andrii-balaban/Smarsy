using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Smarsy.Logic;
using SmarsyEntities;

namespace Smarsy
{
    internal class Program
    {
        [STAThread]
        private static void Main()
        {
            var sl = new SqlServerLogic();
            var student = sl.GetStudentBySmarsyChildId(90018970);
            var webBrowser = new WebBrowser();
            GoToLink(webBrowser, "http://www.smarsy.ua");

            Login(student, webBrowser);
            GoToLink(webBrowser, "http://smarsy.ua/private/parent.php?jsid=Diary&child=" + student.SmarsyChildId + "&tab=Mark");

            if (webBrowser.Document == null) return;
            var tables = webBrowser.Document.GetElementsByTagName("table");
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
            sl.UpsertLessons(marks.Select(x => x.LessonName).Distinct().ToList());
            GoToLink(webBrowser, "http://smarsy.ua/private/parent.php?jsid=Homework&child=" + student.SmarsyChildId + "&tab=Lesson");
            var homeWorks = new List<HomeWork>();
            tables = webBrowser.Document.GetElementsByTagName("table");
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
                    teacherId = sl.InsertTeacherIfNotExists(teacherName);
                    lessonId = sl.GetLessonIdByLessonShortName(lessonName);
                }
                else
                {
                    foreach (HtmlElement rows in el.All)
                    {
                        isHeader = true;
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

            sl.UpsertHomeWorks(homeWorks);
            var emailTo = "keyboards4everyone@gmail.com";
            var subject = "Лизины оценки (" + DateTime.Now.ToShortDateString() + ")";
            var emailBody = "";

            var newMarks = sl.UpserStudentAllLessonsMarks(student.SmarsyChildId, marks);
            if (newMarks.Any())
            {
                emailBody = GenerateEmailBodyForMarks(newMarks);
                
            }

            emailBody += Environment.NewLine;
            emailBody += Environment.NewLine;
            emailBody += GenerateEmailBodyForHomeWork(sl.GetHomeWorkForFuture());

            new EmailLogic().SendEmail(emailTo, subject, emailBody);
        }

        private static string GenerateEmailBodyForHomeWork(List<HomeWork> hwList)
        {
            var result = "";
            var isFirst = true;
            foreach (var homeWork in hwList)
            {
                if (isFirst && ((TimeSpan) (homeWork.HomeWorkDate - DateTime.Now)).TotalDays > 1)
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

        private static string GetTeacherNameFromLessonWithTeacher(string lessonNameWithTeacher, string lessonName)
        {
            var result = lessonNameWithTeacher.Replace(lessonName, "").Replace("(", "").Replace(")", "").Trim();
            return result;
        }

        private static string GetLessonNameFromLessonWithTeacher(string lessonNameWithTeacher)
        {
            var result = lessonNameWithTeacher.Substring(0, lessonNameWithTeacher.IndexOf("(") - 1);
            return result;
        }

        private static string GenerateEmailBodyForMarks(List<MarksRowElement> marks)
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

        private static MarksRowElement ProcessMarksRow(HtmlElement row)
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

        private static HomeWork ProccessHomeWork(HtmlElement row)
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

        private static DateTime GetDateFromComment(string comment, bool isThisYear = true)
        {
            var year = isThisYear ? DateTime.Now.Year.ToString() : (DateTime.Now.Year - 1).ToString();
            var dateWithText = GetTextBetweenSubstrings(comment, "Дата оценки: ", ";");
            var date = dateWithText.Substring(3, dateWithText.Length - 3) + "." + year;

            var result = ChangeDateFormat(date);
            return DateTime.Parse(result);

        }

        private static string ChangeDateFormat(string date)
        {
            return date.Substring(6, 4) + "." + date.Substring(3, 2) + "." + date.Substring(0, 2);
        }

        private static string GetTextBetweenSubstrings(string text, string from, string to)
        {
            int pTo = 0;
            var pFrom = text.IndexOf(from, StringComparison.Ordinal) + from.Length;
            if (to.Length == 0) pTo = text.Length;
            else pTo = text.LastIndexOf(to, StringComparison.Ordinal);
            return text.Substring(pFrom, pTo - pFrom);
        }

        private static int GetStudentId(WebBrowser webBrowser)
        {
            var pattern = new Regex("private/parent.php\\?jsid=Child&child=(?<childId>[0-9]*)\"");
            var match = pattern.Match(webBrowser.DocumentText);
            var childId = int.Parse(match.Groups["childId"].Value);
            return childId;
        }

        private static void ClickOnLoginButton(WebBrowser webBrowser)
        {
            if (webBrowser.Document == null) return;
            var bclick = webBrowser.Document.GetElementsByTagName("input");
            foreach (HtmlElement btn in bclick)
            {
                var name = btn.Name;
                if (name == "submit")
                    btn.InvokeMember("click");
            }
        }

        private static void Login(Student student, WebBrowser webBrowser)
        {
            FillUserName(student, webBrowser.Document);
            FillPassword(student, webBrowser.Document);
            ClickOnLoginButton(webBrowser);

            WaitForPageToLoad(webBrowser);
        }

        private static void WaitForPageToLoad(WebBrowser webBrowser)
        {
            while (webBrowser.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
            System.Threading.Thread.Sleep(500);
        }

        private static void GoToLink(WebBrowser webBrowser, string url)
        {
            webBrowser.Navigate(url);
            WaitForPageToLoad(webBrowser);
        }

        private static void FillPassword(Student student, HtmlDocument doc)
        {
            var element = doc.GetElementById("password");
            if (element != null)
                element.InnerText = student.Password;
        }

        private static void FillUserName(Student student, HtmlDocument doc)
        {
            var element = doc.GetElementById("username");
            if (element != null)
                element.InnerText = student.Login;
        }


    }
}

