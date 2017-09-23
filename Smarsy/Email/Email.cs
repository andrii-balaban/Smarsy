using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarsy.Extensions;
using SmarsyEntities;

namespace Smarsy.Email
{
    public class Email
    {
        private HomeWork[] _homework;

        private LessonMark[] _marks;

        private Student[] _tomorrowBirsdays;

        private Remark[] _remarks;

        private Ad[] _ads;

        public void AddHomeWork(IEnumerable<HomeWork> homeWork)
        {
            _homework = homeWork.ToArray();
        }

        public void AddMarks(IEnumerable<LessonMark> marks)
        {
            _marks = marks.ToArray();
        }

        public void AddRemrks(IEnumerable<Remark> remarks)
        {
            _remarks = remarks.ToArray();
        }

        public void AddAds(IEnumerable<Ad> ads)
        {
            _ads = ads.ToArray();
        }

        public void AddNextDayBirthDayStudents(IEnumerable<Student> students)
        {
            _tomorrowBirsdays = students.ToArray();
        }

        public StringBuilder GenerateEmailBody()
        {
            var emailBody = new StringBuilder();

            emailBody.AppendWithDoubleBrTag(GenerateEmailForRemarks());
            emailBody.AppendWithDoubleBrTag(GenerateEmailForTomorrowBirthdays());
            emailBody.AppendWithDoubleBrTag(GenerateEmailForNewAds());
            emailBody.AppendWithDoubleBrTag(GenerateEmailBodyForMarks());
            emailBody.AppendWithDoubleBrTag(GenerateEmailBodyForHomeWork());

            return emailBody;
        }

        private string GenerateEmailBodyForHomeWork()
        {
            if (!HasHomework())
                return string.Empty;

            var result = new StringBuilder("Домашняя работа");
            var isFirst = true;
            result.Append("<table border=\"1\">");

            foreach (var homeWork in _homework)
            {
                if (isFirst && ((homeWork.HomeWorkDate - DateTime.Now).TotalDays > 1))
                {
                    result.AppendWithDoubleBrTag(string.Empty);
                    isFirst = false;
                    result.AppendWithDoubleBrTag("</table>");
                    result.Append("Задания на другие дни");
                    result.Append("<table border=\"1\">");
                }

                result.Append("<tr>");
                result.AppendSurroundTd(homeWork.HomeWorkDate.ToShortDateString());
                result.AppendSurroundTd(homeWork.LessonName);
                result.AppendSurroundTd(homeWork.TeacherName);
                result.AppendSurroundTd(homeWork.HomeWorkDescr);
                result.Append("</tr>");
            }

            return result.ToString();
        }

        private bool HasHomework()
        {
            return _homework != null && _homework.Any();
        }

        private string GenerateEmailForTomorrowBirthdays()
        {
            if (!HasTomorrowBirthdays())
                return string.Empty;

            var result = new StringBuilder("Завтра день рождения у:");
            result.Append("<table border=\"1\">");

            foreach (var birthday in _tomorrowBirsdays)
            {
                result.Append("<tr>");
                result.AppendSurroundTd(birthday.Name);
                result.AppendSurroundTd(DateTime.Now.Year - birthday.BirthDate.Year);
                result.Append("</tr>");
            }

            result.Append("</table>");

            return result.ToString();
        }

        private bool HasTomorrowBirthdays()
        {
            return _tomorrowBirsdays != null && _tomorrowBirsdays.Any();
        }

        private string GenerateEmailForRemarks()
        {
            if (!HasRemarks())
                return string.Empty;

            var result = new StringBuilder("Замечания");
            result.Append("<table border=\"1\">");

            foreach (var rem in _remarks)
            {
                result.Append("<tr>");
                result.AppendSurroundTd(rem.RemarkDate.ToShortDateString());
                result.AppendSurroundTd(rem.LessonName);
                result.AppendSurroundTd(rem.RemarkText);
                result.Append("</tr>");
            }

            result.Append("</table>");

            return result.ToString();
        }

        private bool HasRemarks()
        {
            return _remarks != null && _remarks.Any();
        }

        private string GenerateEmailForNewAds()
        {
            if (!HasAds())
                return string.Empty;

            var result = new StringBuilder("Объявления");
            result.Append("<table border=\"1\">");

            foreach (var ad in _ads)
            {
                result.Append("<tr>");
                result.AppendSurroundTd(ad.AdDate.ToShortDateString());
                result.AppendSurroundTd(ad.AdText);
                result.Append("</tr>");
            }
            result.Append("</table>");

            return result.ToString();
        }

        private bool HasAds()
        {
            return _ads != null && _ads.Any();
        }

        private string GenerateEmailBodyForMarks()
        {
            if (!HasMarks())
                return string.Empty;

            var result = new StringBuilder("Оценки");
            result.Append("<table border=\"1\">");

            foreach (var lesson in _marks.OrderBy(x => x.LessonName).ToList())
            {
                foreach (var mark in lesson.Marks.OrderByDescending(x => x.Date))
                {
                    result.Append("<tr>");
                    result.AppendSurroundTd(lesson.LessonName);
                    result.AppendSurroundTd(mark.Date.ToShortDateString());
                    result.AppendSurroundTd(mark.Mark);
                    result.AppendSurroundTd(mark.Reason);
                    result.Append("</tr>");
                }
            }
            result.Append("</table>");

            return result.ToString();
        }

        private bool HasMarks()
        {
            return _marks != null && _marks.Any();
        }
    }
}