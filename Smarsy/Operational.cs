namespace Smarsy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using Extensions;
    using Logic;
    using NLog;
    using SmarsyEntities;

    public class Operational
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISmarsyRepository _repository;
        private readonly SmarsyBrowser _smarsyBrowser;

        public Operational(ISmarsyRepository repository, string login)
        {
            Student = new Student
            {
                Login = login
            };


            _repository = repository;
            _smarsyBrowser = new SmarsyBrowser();
        }

        public Student Student { get; set; }

        public ISmarsyRepository Repository => _repository;


        public void InitStudentFromDb()
        {
            Logger.Info("Getting student info from database");
            Student = _repository.GetStudentBySmarsyLogin(Student.Login);
        }

        public void LoginToSmarsy()
        {
            _smarsyBrowser.GoToLink("http://www.smarsy.ua");
            _smarsyBrowser.Login(Student);
        }

        public void UpdateAds()
        {
            _smarsyBrowser.GetTableObjectFromPage("http://smarsy.ua/private/parent.php?jsid=Announ&tab=List", ProcessAdsRow, "Ads", _repository.UpsertAds, Student.SmarsyChildId);
        }

        public void UpdateMarks()
        {
            _smarsyBrowser.GetTableObjectFromPage("http://smarsy.ua/private/parent.php?jsid=Diary&tab=Mark", ProcessMarksRow, "Marks", _repository.UpserStudentAllLessonsMarks, Student.SmarsyChildId);
        }

        public void UpdateStudents()
        {
            _smarsyBrowser.GetTableObjectFromPage("http://smarsy.ua/private/parent.php?jsid=Grade&lesson=0&tab=List", ProcessStudentsRow, "Students", _repository.UpsertStudents, Student.SmarsyChildId);
        }

        public void UpdateRemarks()
        {
            _smarsyBrowser.GetTableObjectFromPage("http://smarsy.ua/private/parent.php?jsid=Remark&tab=List", ProcessRemarksRow, "Remarks", _repository.UpsertRemarks, Student.SmarsyChildId);
        }

        public void SendEmail(IEnumerable<string> emailToList, string emailFrom, string password)
        {
            Logger.Info($"Sending email to {string.Join(",", emailToList)}");
            var ec = new EmailClient();
            ec.SendEmail(Student.StudentId, emailToList, emailFrom, password);
        }

        internal static DateTime GetDateFromText(string birthDate, int studentAge)
        {
            var year = DateTime.Now.Year - studentAge;
            var month = GetMonthFromRussianName(GetMonthNameFromStringWithDayNumber(birthDate));
            var day = GetDayFromStringWithDayNumber(birthDate);
            var tmpDate = new DateTime(DateTime.Now.Year, month, day);

            if (DateTime.Now < tmpDate)
            {
                year--;
            }

            return new DateTime(year, month, day);
        }

        internal string GetTextBetweenSubstrings(string text, string from, string to)
        {
            var charFrom = text.IndexOf(from, StringComparison.Ordinal) + from.Length;
            var charTo = to.Length == 0 ? text.Length : text.LastIndexOf(to, StringComparison.Ordinal);
            return text.Substring(charFrom, charTo - charFrom);
        }

        public string GetTeacherNameFromLessonWithTeacher(string lessonNameWithTeacher, string lessonName)
        {
            var result = lessonNameWithTeacher.Replace(lessonName, string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Trim();
            return result;
        }

        public string GetLessonNameFromLessonWithTeacher(string lessonNameWithTeacher)
        {
            if (!lessonNameWithTeacher.Contains("("))
            {
                return lessonNameWithTeacher;
            }

            var result = lessonNameWithTeacher.Substring(0, lessonNameWithTeacher.IndexOf("(", StringComparison.Ordinal) - 1);
            return result;
        }

        private static string ChangeDateFormat(string date)
        {
            return date.Substring(6, 4) + "." + date.Substring(3, 2) + "." + date.Substring(0, 2);
        }

        private static Ad ProcessAdsRow(HtmlElement row)
        {
            var ad = new Ad();
            var i = 1;

            foreach (HtmlElement oneROw in row.GetElementsByTagName("td"))
            {
                if (i == 1)
                {
                    ad.AdDate = DateTime.ParseExact(oneROw.InnerHtml, "dd.mm.yyyy", null);
                }

                if (i == 2)
                {
                    ad.AdText = oneROw.InnerHtml;
                }

                i++;
            }

            return ad;
        }

        public HomeWork ProccessHomeWork(HtmlElement row)
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

        private static Remark ProcessRemarksRow(HtmlElement row)
        {
            var remark = new Remark();
            var i = 0;

            foreach (HtmlElement element in row.GetElementsByTagName("td"))
            {
                if (i == 0)
                {
                    remark.RemarkDate = element.InnerText.ConvertDateToRussianFormat();
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

        private static int GetMonthFromRussianName(string name)
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

        private static Student ProcessStudentsRow(HtmlElement row)
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

        private static int GetDayFromStringWithDayNumber(string date)
        {
            return int.Parse(date.Substring(0, 2).Trim());
        }

        private static string GetMonthNameFromStringWithDayNumber(string date)
        {
            return date.Substring(2, date.Length - 2).Trim();
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

        private DateTime GetDateFromComment(string comment, bool isThisYear = true)
        {
            var year = isThisYear ? DateTime.Now.Year.ToString() : (DateTime.Now.Year - 1).ToString();
            var dateWithText = GetTextBetweenSubstrings(comment, "Дата оценки: ", ";");
            var date = dateWithText.Substring(3, dateWithText.Length - 3) + "." + year;

            var result = ChangeDateFormat(date);
            return DateTime.Parse(result);
        }
    }
}
