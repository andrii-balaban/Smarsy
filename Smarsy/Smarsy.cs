using System.Linq;
using Smarsy.Email;
using Smarsy.SmarsyBrowser;

namespace Smarsy
{
    using System.Collections.Generic;
    using Logic;
    using NLog;
    using SmarsyEntities;

    public class Smarsy
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISmarsyRepository _repository;
        private readonly ISmarsyBrowser _smarsyBrowser;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Smarsy(ISmarsyRepository repository, ISmarsyBrowser smarsyBrowser, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _smarsyBrowser = smarsyBrowser;
            _dateTimeProvider = dateTimeProvider;
        }

        public SmarsyStudent Student { get; set; }

        public ISmarsyRepository Repository => _repository;

        public void Login(string login)
        {
            LoadStudent(login);

            _smarsyBrowser.Login(new LoginPage(Student));
        }

        private void LoadStudent(string login)
        {
            Logger.Info("Getting student info from database");
            Student = _repository.GetStudentBySmarsyLogin(login);
        }

        public void UpdateAds()
        {
            AdPage marksPage = new AdPage(Student);
            List<Ad> result = _smarsyBrowser.GetSmarsyElementFromPage(marksPage).ToList();

            Logger.Info("Upserting Ads in database");
            _repository.UpsertAds(result);
        }

        public void UpdateMarks()
        {
            MarksPage marksPage = new MarksPage(Student.SmarsyChildId);
            List<LessonMark> result =_smarsyBrowser.GetSmarsyElementFromPage(marksPage).ToList();

            Logger.Info("Upserting LessonMark in database");
            _repository.UpserStudentAllLessonsMarks(Student, result);
        }

        public void UpdateStudents()
        {
            StudentsPage marksPage = new StudentsPage(Student);
            List<SmarsyStudent> students = _smarsyBrowser.GetSmarsyElementFromPage(marksPage).ToList();

            Logger.Info("Upserting Students in database");
            _repository.UpsertStudents(students);
        }

        public void UpdateRemarks()
        {
            RemarksPage marksPage = new RemarksPage(Student);
            List<Remark> remarks = _smarsyBrowser.GetSmarsyElementFromPage(marksPage).ToList();

            Logger.Info("Upserting Remarks in database");
            _repository.UpsertRemarks(remarks);
        }

        public void UpdateHomeWork()
        {
            HomeworkPage homeworkPage = new HomeworkPage(Student, this);
            List<HomeWork> homeWorks = _smarsyBrowser.GetSmarsyElementFromPage(homeworkPage).ToList();

            Logger.Info("Upserting Homework in database");
            Repository.UpsertHomeWorks(homeWorks);
        }

        public void SendEmail(string emailFrom, IEnumerable<string> emailsTo)
        {
            string[] emailsArray = emailsTo.ToArray();

            Logger.Info($"Sending email to {string.Join(",", emailsArray)}");
            
            Email.Email email = CreatEmail(emailFrom, emailsArray);

            new EmailClient().SendEmail(email, Student.Credentials);
        }

        private Email.Email CreatEmail(string emailFrom, IEnumerable<string> emailsTo)
        {
            string subject = CreateEmailSubject();

            Email.Email email = new EmailBuilder()
                .WithFromAddress(emailFrom)
                .WithToAddresses(emailsTo)
                .WithSubject(subject)
                .WithHomeworks(Repository.GetHomeWorkForFuture())
                .WithTomorrowBirthDayStudents(Repository.GetStudentsWithBirthdayTomorrow())
                .WithRemarks(Repository.GetNewRemarks())
                .WithAds(Repository.GetNewAds())
                .WithMarks(Repository.GetStudentMarks(Student.StudentId))
                .Build();

            return email;
        }

        private string CreateEmailSubject()
        {
            return "Лизины оценки (" + _dateTimeProvider.GetDateTime().ToShortDateString() + ")";
        }
    }
}
