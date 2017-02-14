namespace Smarsy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using NLog;
    using Extensions;
    using Logic;
    using SmarsyEntities;

    public class Operational
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly SqlServerLogic _sqlServerLogic;

        public Operational(string login)
        {
            Student = new Student
            {
                Login = login
            };
            SmarsyBrowser = new WebBrowser();
            _sqlServerLogic = new SqlServerLogic();

        }

        public Student Student { get; set; }

        public WebBrowser SmarsyBrowser { get; set; }

        public void InitStudentFromDb()
        {
            Logger.Info("Getting student info from database");
            Student = _sqlServerLogic.GetStudentBySmarsyLogin(Student.Login);
        }

        public void LoginToSmarsy()
        {
            GoToLink("http://www.smarsy.ua");
            Login();
        }

        public void UpdateAds()
        {
            GoToLink($"http://smarsy.ua/private/parent.php?jsid=Announ&child={Student.SmarsyChildId}&tab=List");

            if (SmarsyBrowser.Document == null)
            {
                return;
            }

            var tables = SmarsyBrowser.Document.GetElementsByTagName("table");
            var i = 0;
            var isHeader = true;
            var ads = new List<Ad>();
            foreach (HtmlElement el in tables)
            {
                if (i++ != 1)
                {
                    continue; // skip first table
                }

                foreach (HtmlElement rows in el.All)
                {
                    foreach (HtmlElement row in rows.GetElementsByTagName("tr"))
                    {
                        if (isHeader)
                        {
                            isHeader = false;
                            continue;
                        }

                        ads.Add(ProcessAdsRow(row));
                    }
                }
            }

            Logger.Info("Upserting ads in database");
            _sqlServerLogic.UpsertAds(ads);
        }

        public void UpdateMarks()
        {
            GoToLink($"http://smarsy.ua/private/parent.php?jsid=Diary&child={Student.SmarsyChildId}&tab=Mark");

            if (SmarsyBrowser.Document == null)
            {
                return;
            }

            var tables = SmarsyBrowser.Document.GetElementsByTagName("table");
            var i = 0;
            var isHeader = true;
            var marks = new List<LessonMark>();
            foreach (HtmlElement el in tables)
            {
                if (i++ != 1)
                {
                    continue; // skip first table
                }

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

            Logger.Info("Upserting lessons in database");
            _sqlServerLogic.UpsertLessons(marks.Select(x => x.LessonName).Distinct().ToList());
            _sqlServerLogic.UpserStudentAllLessonsMarks(Student.Login, marks);
        }

        private void GoToLinkWithChild(string url)
        {
            GoToLink($"{url}&child={Student.SmarsyChildId}");
        }

        public void GetTableObjectFromPage<T>(string url, Func<HtmlElement, T> methodName, string entityNameForLog, Action<IList<T>> databaseProcessingMethodName,  bool isSkipHeader = true)
        {
            GoToLinkWithChild(url);
            if (SmarsyBrowser.Document == null)
            {
                return;
            }

            var result = SmarsyBrowser.Document.GetElementsByTagName("table").OfType<HtmlElement>()
                .Skip(1) // skip the first table on the page
                .Take(1) // take the only second table on the page
                .SelectMany(row => row.GetElementsByTagName("tr").OfType<HtmlElement>())
                .Skip(isSkipHeader ? 1 : 0) // skip header row
                .Select(methodName).ToArray();

            Logger.Info($"Upserting {entityNameForLog} in database");
            databaseProcessingMethodName(result);
        }

        public void UpdateStudents()
        {
            GetTableObjectFromPage<Student>("http://smarsy.ua/private/parent.php?jsid=Grade&lesson=0&tab=List", ProcessStudentsRow, "Students", _sqlServerLogic.UpsertStudents);
        }

        public void UpdateHomeWork()
        {
            GoToLink($"http://smarsy.ua/private/parent.php?jsid=Homework&child={Student.SmarsyChildId}&tab=Lesson");
            if (SmarsyBrowser.Document == null)
            {
                return;
            }

            var homeWorks = new List<HomeWork>();
            var tables = SmarsyBrowser.Document.GetElementsByTagName("table");
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
                            if ((tmp.HomeWorkDescr != null) && !tmp.HomeWorkDescr.Trim().Equals(string.Empty))
                            {
                                homeWorks.Add(tmp);
                            }
                        }
                    }
                }
            }

            Logger.Info("Upserting homeworks in database");
            _sqlServerLogic.UpsertHomeWorks(homeWorks);
        }
        //public void UpdateRemarks()
        //{
        //    GoToLink($"http://smarsy.ua/private/parent.php?jsid=Remark&child={Student.SmarsyChildId}&tab=List");
        //    if (SmarsyBrowser.Document == null)
        //    {
        //        return;
        //    }

        //    var remarks = new List<Remark>();
        //    var tables = SmarsyBrowser.Document.GetElementsByTagName("table");

        //    foreach (HtmlElement el in tables)
        //    {
        //        if (separateLessonNameFromHomeWork++ % 2 == 0)
        //        {
        //            var lessonNameWithTeacher = el.InnerText.Replace("\r\n", string.Empty);
        //            var lessonName = GetLessonNameFromLessonWithTeacher(lessonNameWithTeacher);
        //            var teacherName = GetTeacherNameFromLessonWithTeacher(lessonNameWithTeacher, lessonName);
        //            teacherId = _sqlServerLogic.InsertTeacherIfNotExists(teacherName);
        //            lessonId = _sqlServerLogic.GetLessonIdByLessonShortName(lessonName);
        //        }
        //        else
        //        {
        //            foreach (HtmlElement rows in el.All)
        //            {
        //                var isHeader = true;
        //                foreach (HtmlElement row in rows.GetElementsByTagName("tr"))
        //                {
        //                    if (isHeader)
        //                    {
        //                        isHeader = false;
        //                        continue;
        //                    }

        //                    var tmp = ProccessHomeWork(row);
        //                    tmp.LessonId = lessonId;
        //                    tmp.TeacherId = teacherId;
        //                    if ((tmp.HomeWorkDescr != null) && !tmp.HomeWorkDescr.Trim().Equals(string.Empty))
        //                    {
        //                        remarks.Add(tmp);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    Logger.Info("Upserting homeworks in database");
        //    _sqlServerLogic.UpsertHomeWorks(remarks);
        //}

        public void SendEmail()
        {
            var emailTo = "keyboards4everyone@gmail.com";
            var subject = "Лизины оценки (" + DateTime.Now.ToShortDateString() + ")";
            var emailBody = new StringBuilder();

            emailBody.Append(GenerateEmailForTomorrowBirthdays());
            emailBody.AppendLine();
            emailBody.AppendLine();

            emailBody.Append(GenerateEmailForNewAds());
            emailBody.AppendLine();
            emailBody.AppendLine();

            emailBody.Append(GenerateEmailBodyForMarks());
            emailBody.AppendLine();
            emailBody.AppendLine();

            emailBody.Append(GenerateEmailBodyForHomeWork(_sqlServerLogic.GetHomeWorkForFuture()));

            Logger.Info($"Sending email to {emailTo}");
            new EmailLogic().SendEmail(emailTo, subject, emailBody.ToString());
        }

        private string GenerateEmailForNewAds()
        {
            var ads = _sqlServerLogic.GetNewAds();
            var result = new StringBuilder();

            if (!ads.Any()) return string.Empty;

            foreach (var ad in ads)
            {
                result.AppendWithDashes(ad.AdDate);
                result.AppendWithNewLine(ad.AdText);
            }

            return result.ToString();
        }

        private static string GenerateEmailBodyForHomeWork(IEnumerable<HomeWork> homeWorks)
        {
            var result = new StringBuilder();
            var isFirst = true;
            foreach (var homeWork in homeWorks)
            {
                if (isFirst && ((homeWork.HomeWorkDate - DateTime.Now).TotalDays > 1))
                {
                    result.AppendLine();
                    result.AppendLine();
                    isFirst = false;
                }

                result.AppendWithDashes(homeWork.HomeWorkDate.ToShortDateString());
                result.AppendWithDashes(homeWork.LessonName);
                result.AppendWithDashes(homeWork.TeacherName);
                result.AppendWithNewLine(homeWork.HomeWorkDescr);
            }

            return result.ToString();
        }

        private static string ChangeDateFormat(string date)
        {
            return date.Substring(6, 4) + "." + date.Substring(3, 2) + "." + date.Substring(0, 2);
        }

        internal string GetTextBetweenSubstrings(string text, string from, string to)
        {
            var charFrom = text.IndexOf(from, StringComparison.Ordinal) + from.Length;
            var charTo = to.Length == 0 ? text.Length : text.LastIndexOf(to, StringComparison.Ordinal);
            return text.Substring(charFrom, charTo - charFrom);
        }

        private void Login()
        {
            FillTextBoxByElementId("username", Student.Login);
            FillTextBoxByElementId("password", Student.Password);
            ClickOnLoginButton();

            WaitForPageToLoad();
        }

        private void GoToLink(string url)
        {
            Logger.Info($"Go to {url} page");
            SmarsyBrowser.Navigate(url);
            WaitForPageToLoad();
        }

        private void WaitForPageToLoad()
        {
            while (SmarsyBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            Thread.Sleep(500);
        }

        private void ClickOnLoginButton()
        {
            if (SmarsyBrowser.Document == null)
            {
                return;
            }

            var bclick = SmarsyBrowser.Document.GetElementsByTagName("input");
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
            if (SmarsyBrowser == null || SmarsyBrowser.Document == null || elementId == null)
            {
                return;
            }

            Logger.Info($"Entering text to the {elementId} element");

            var element = SmarsyBrowser.Document.GetElementById(elementId);
            if (element != null)
            {
                element.InnerText = value;
            }
        }

        private Student ProcessStudentsRow(HtmlElement row)
        {
            var student = new Student();
            var i = 0;
            var birthDate = string.Empty;

            foreach (HtmlElement studentRow in row.GetElementsByTagName("td"))
            {
                if (i == 0)
                {
                    i++;
                    continue; // skip student sequence number
                }

                if (i == 1)
                {
                    student.Name = studentRow.InnerHtml;
                }

                if (i == 2)
                {
                    birthDate = studentRow.InnerHtml;
                }

                if (i == 3)
                {
                    student.BirthDate = GetDateFromText(birthDate, int.Parse(studentRow.InnerHtml));
                }

                i++;
            }

            return student;
        }

        internal DateTime GetDateFromText(string birthDate, int studentAge)
        {
            var year = DateTime.Now.Year - studentAge;
            var month = GetMonthFromRussianName(GetMonthNameFromStringWithDayNumber(birthDate));
            var day = GetDayFromStringWithDayNumber(birthDate);
            var tmpDate = new DateTime(DateTime.Now.Year, month, day);

            if (DateTime.Now < tmpDate) year--;

            return new DateTime(year, month, day);
        }

        private int GetDayFromStringWithDayNumber(string date)
        {
            return int.Parse(date.Substring(0, 2).Trim());
        }
        private string GetMonthNameFromStringWithDayNumber(string date)
        {
            return date.Substring(2, date.Length - 2).Trim();
        }

        private int GetMonthFromRussianName(string name)
        {
            switch (name)
            {
                case "января":
                    return 1;
                case "февраля":
                    return 2;
                case "марта":
                    return 3;
                case "апреля":
                    return 4;
                case "мая":
                    return 5;
                case "июня":
                    return 6;
                case "июля":
                    return 7;
                case "августа":
                    return 8;
                case "сентября":
                    return 9;
                case "октября":
                    return 10;
                case "ноября":
                    return 11;
                case "декабря":
                    return 12;
            }
            return -1;
        }

        private LessonMark ProcessMarksRow(HtmlElement row)
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

            var marks = new LessonMark
            {
                LessonName = lessonName,
                Marks = new List<StudentMark>()
            };
            foreach (HtmlElement mark in tmpMarks.GetElementsByTagName("a"))
            {
                var studentMark = new StudentMark
                {
                    Mark = int.Parse(mark.InnerText),
                    Reason = GetTextBetweenSubstrings(mark.GetAttribute("title"), "За что получена:", string.Empty),
                    Date = GetDateFromComment(mark.GetAttribute("title"))
                };
                marks.Marks.Add(studentMark);
            }

            return marks;
        }

        internal string GetTeacherNameFromLessonWithTeacher(string lessonNameWithTeacher, string lessonName)
        {
            var result = lessonNameWithTeacher.Replace(lessonName, string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Trim();
            return result;
        }

        internal string GetLessonNameFromLessonWithTeacher(string lessonNameWithTeacher)
        {
            if (!lessonNameWithTeacher.Contains("(")) return lessonNameWithTeacher;
            var result = lessonNameWithTeacher.Substring(0, lessonNameWithTeacher.IndexOf("(", StringComparison.Ordinal) - 1);
            return result;
        }

        private string GenerateEmailBodyForMarks()
        {
            var marks = _sqlServerLogic.GetStudentMarkSummary(Student.StudentId);

            var sb = new StringBuilder();
            foreach (var lesson in marks.OrderBy(x => x.LessonName).ToList())
            {
                sb.Append(lesson.LessonName);
                sb.Append(":");
                sb.Append(Environment.NewLine);
                foreach (var mark in lesson.Marks.OrderByDescending(x => x.Date))
                {
                    sb.AppendWithDashes(mark.Date.ToShortDateString());
                    sb.AppendWithDashes(mark.Mark);
                    sb.AppendWithNewLine(mark.Reason);
                }

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        private string GenerateEmailForTomorrowBirthdays()
        {
            var birthdayStudents = _sqlServerLogic.GetStudentsWithBirthdayTomorrow();
            if (!birthdayStudents.Any()) return string.Empty;

            var sb = new StringBuilder("Завтра день рождения у:");
            sb.AppendLine();

            foreach (var birthday in birthdayStudents)
            {
                sb.AppendWithDashes(birthday.Name);    
                sb.AppendWithNewLine(DateTime.Now.Year - birthday.BirthDate.Year);
            }

            return sb.ToString();
        }

        private HomeWork ProccessHomeWork(HtmlElement row)
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

        private DateTime GetDateFromComment(string comment, bool isThisYear = true)
        {
            var year = isThisYear ? DateTime.Now.Year.ToString() : (DateTime.Now.Year - 1).ToString();
            var dateWithText = GetTextBetweenSubstrings(comment, "Дата оценки: ", ";");
            var date = dateWithText.Substring(3, dateWithText.Length - 3) + "." + year;

            var result = ChangeDateFormat(date);
            return DateTime.Parse(result);
        }

        private Ad ProcessAdsRow(HtmlElement row)
        {
            var ad = new Ad();
            var i = 1;

            foreach (HtmlElement adRow in row.GetElementsByTagName("td"))
            {

                if (i == 1)
                {
                    ad.AdDate = DateTime.ParseExact(adRow.InnerHtml, "dd.mm.yyyy", null);
                }

                if (i == 2)
                {
                    ad.AdText = adRow.InnerHtml;
                }

                i++;
            }

            return ad;
        }
    }
}