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

        public void SendEmail(int studentId)
        {
            var emailTo = "keyboards4everyone@gmail.com";
            var subject = "Лизины оценки (" + DateTime.Now.ToShortDateString() + ")";
            var emailBody = new StringBuilder();

            emailBody.Append(GenerateEmailForRemarks());
            emailBody.AppendLine();
            emailBody.AppendLine();

            emailBody.Append(GenerateEmailForTomorrowBirthdays());
            emailBody.AppendLine();
            emailBody.AppendLine();

            emailBody.Append(GenerateEmailForNewAds());
            emailBody.AppendLine();
            emailBody.AppendLine();

            emailBody.Append(GenerateEmailBodyForMarks(studentId));
            emailBody.AppendLine();
            emailBody.AppendLine();

            emailBody.Append(GenerateEmailBodyForHomeWork(_sqlServerLogic.GetHomeWorkForFuture()));

            SendEmail(emailTo, subject, emailBody.ToString());
        }

        public void SendEmail(string emailTo, string subject, string body)
        {
            var fromAddress = new MailAddress("olxsender@gmail.com", "Smarsy наблюдатель");
            var toAddress = new MailAddress(emailTo, "Оценки");
            var fromPassword = "1mCr3at1nG$3cur3P4$$";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
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

        private string GenerateEmailForTomorrowBirthdays()
        {
            var birthdayStudents = _sqlServerLogic.GetStudentsWithBirthdayTomorrow();
            if (!birthdayStudents.Any())
            {
                return string.Empty;
            }

            var sb = new StringBuilder("Завтра день рождения у:");
            sb.AppendLine();

            foreach (var birthday in birthdayStudents)
            {
                sb.AppendWithDashes(birthday.Name);
                sb.AppendWithNewLine(DateTime.Now.Year - birthday.BirthDate.Year);
            }

            return sb.ToString();
        }

        private string GenerateEmailForRemarks()
        {
            var remarks = _sqlServerLogic.GetNewRemarks();
            var result = new StringBuilder();

            if (!remarks.Any())
            {
                return string.Empty;
            }

            foreach (var rem in remarks)
            {
                result.AppendWithDashes(rem.RemarkDate.ToShortDateString());
                result.AppendWithDashes(rem.LessonName);
                result.AppendWithDashes(rem.RemarkText);
            }

            return result.ToString();
        }

        private string GenerateEmailForNewAds()
        {
            var ads = _sqlServerLogic.GetNewAds();
            var result = new StringBuilder();

            if (!ads.Any())
            {
                return string.Empty;
            }

            foreach (var ad in ads)
            {
                result.AppendWithDashes(ad.AdDate.ToShortDateString());
                result.AppendWithNewLine(ad.AdText);
            }

            return result.ToString();
        }

        private string GenerateEmailBodyForMarks(int studentId)
        {
            var marks = _sqlServerLogic.GetStudentMarkSummary(studentId);

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
    }
}
