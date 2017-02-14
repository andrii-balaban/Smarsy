namespace Smarsy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using Extensions;
    using Logic;
    using SmarsyEntities;

    public class EmailClient
    {
        private readonly SqlServerLogic _sqlServerLogic = new SqlServerLogic();

        public void SendEmail(int studentId, List<string> emailToList,  string emailFrom, string fromPassword)
        {
            var subject = "Лизины оценки (" + DateTime.Now.ToShortDateString() + ")";
            var emailBody = new StringBuilder();

            emailBody.AppendWithDoubleBrTag(GenerateEmailForRemarks());
            emailBody.AppendWithDoubleBrTag(GenerateEmailForTomorrowBirthdays());
            emailBody.AppendWithDoubleBrTag(GenerateEmailForNewAds());
            emailBody.AppendWithDoubleBrTag(GenerateEmailBodyForMarks(studentId));
            emailBody.AppendWithDoubleBrTag(GenerateEmailBodyForHomeWork());

            SendEmail(emailToList, subject, emailBody.ToString(), emailFrom, fromPassword);
        }

        public void SendEmail(List<string> emailTo, string subject, string body, string emailFrom, string fromPassword)
        {
            var fromAddress = new MailAddress(emailFrom, "Smarsy наблюдатель");

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage()
            {
                From = fromAddress,
                Subject = subject,
                Body = body
            })
            {
                foreach (var mail in emailTo)
                {
                    message.To.Add(mail);
                }

                message.IsBodyHtml = true;
                smtp.Send(message);
            }
        }

        private string GenerateEmailBodyForHomeWork()
        {
            var homeWorks = _sqlServerLogic.GetHomeWorkForFuture();

            if (!homeWorks.Any())
            {
                return string.Empty;
            }

            var result = new StringBuilder("Домашняя работа");
            var isFirst = true;
            result.Append("<table border=\"1\">");

            foreach (var homeWork in homeWorks)
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

        private string GenerateEmailForTomorrowBirthdays()
        {
            var birthdayStudents = _sqlServerLogic.GetStudentsWithBirthdayTomorrow();
            if (!birthdayStudents.Any())
            {
                return string.Empty;
            }

            var result = new StringBuilder("Завтра день рождения у:");
            result.Append("<table border=\"1\">");

            foreach (var birthday in birthdayStudents)
            {
                result.Append("<tr>");
                result.AppendSurroundTd(birthday.Name);
                result.AppendSurroundTd(DateTime.Now.Year - birthday.BirthDate.Year);
                result.Append("</tr>");
            }

            result.Append("</table>");

            return result.ToString();
        }

        private string GenerateEmailForRemarks()
        {
            var remarks = _sqlServerLogic.GetNewRemarks();

            if (!remarks.Any())
            {
                return string.Empty;
            }

            var result = new StringBuilder("Замечания");
            result.Append("<table border=\"1\">");

            foreach (var rem in remarks)
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

        private string GenerateEmailForNewAds()
        {
            var ads = _sqlServerLogic.GetNewAds();

            if (!ads.Any())
            {
                return string.Empty;
            }

            var result = new StringBuilder("Объявления");
            result.Append("<table border=\"1\">");

            foreach (var ad in ads)
            {
                result.Append("<tr>");
                result.AppendSurroundTd(ad.AdDate.ToShortDateString());
                result.AppendSurroundTd(ad.AdText);
                result.Append("</tr>");
            }
            result.Append("</table>");

            return result.ToString();
        }

        private string GenerateEmailBodyForMarks(int studentId)
        {
            List<LessonMark> marks = _sqlServerLogic.GetStudentMarkSummary(studentId);
            if (!marks.Any())
            {
                return string.Empty;
            }

            var result = new StringBuilder("Оценки");
            result.Append("<table border=\"1\">");

            foreach (var lesson in marks.OrderBy(x => x.LessonName).ToList())
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
    }
}
