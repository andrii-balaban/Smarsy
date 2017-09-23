using System.Linq;
using Smarsy.Email;
using Smarsy.SmarsyBrowser;

namespace Smarsy
{
    using System.Collections.Generic;
    using Logic;
    using NLog;
    using SmarsyEntities;

    public class SmarsyOperations
    {
        private const string SmarsyLink = "http://www.smarsy.ua";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISmarsyRepository _repository;
        private readonly ISmarsyBrowser _smarsyBrowser;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SmarsyOperations(ISmarsyRepository repository, ISmarsyBrowser smarsyBrowser, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _smarsyBrowser = smarsyBrowser;
            _dateTimeProvider = dateTimeProvider;
        }

        public Student Student { get; set; }

        public ISmarsyRepository Repository => _repository;

        public void LoginToSmarsy(string login)
        {
            LoadStudent(login);

            _smarsyBrowser.GoToLink(SmarsyLink);
            _smarsyBrowser.Login(Student);
        }

        private void LoadStudent(string login)
        {
            Logger.Info("Getting student info from database");
            Student = _repository.GetStudentBySmarsyLogin(login);
        }

        public void UpdateAds()
        {
            AdPage marksPage = new AdPage(Student.SmarsyChildId);
            List<Ad> result = _smarsyBrowser.GetSmarsyElementFromPage(marksPage).ToList();

            Logger.Info("Upserting Ads in database");
            _repository.UpsertAds(result);
        }

        public void UpdateMarks()
        {
            MarksPage marksPage = new MarksPage(Student.SmarsyChildId);
            List<LessonMark> result =_smarsyBrowser.GetSmarsyElementFromPage(marksPage).ToList();

            Logger.Info("Upserting LessonMark in database");
            _repository.UpserStudentAllLessonsMarks(result);
        }

        public void UpdateStudents()
        {
            StudentsPage marksPage = new StudentsPage(Student.SmarsyChildId);
            List<Student> students = _smarsyBrowser.GetSmarsyElementFromPage(marksPage).ToList();

            Logger.Info("Upserting Students in database");
            _repository.UpsertStudents(students);
        }

        public void UpdateRemarks()
        {
            RemarksPage marksPage = new RemarksPage(Student.SmarsyChildId);
            List<Remark> remarks = _smarsyBrowser.GetSmarsyElementFromPage(marksPage).ToList();

            Logger.Info("Upserting Remarks in database");
            _repository.UpsertRemarks(remarks);
        }

        public void UpdateHomeWork()
        {
            List<HomeWork> homeWorks = _smarsyBrowser.UpdateHomeWork(this, Student.SmarsyChildId).ToList();

            Logger.Info("Upserting Homework in database");
            Repository.UpsertHomeWorks(homeWorks);
        }

        public void SendEmail(IEnumerable<string> emails, string emailFrom, string password)
        {
            string[] emailsArray = emails.ToArray();

            Logger.Info($"Sending email to {string.Join(",", emailsArray)}");

            string subject = CreateEmailSubject();

            Email.Email email = new EmailBuilder()
                .WithHomeworks(Repository.GetHomeWorkForFuture())
                .WithTomorrowBirthDayStudents(Repository.GetStudentsWithBirthdayTomorrow())
                .WithRemarks(Repository.GetNewRemarks())
                .WithAds(Repository.GetNewAds())
                .WithMarks(Repository.GetStudentMarks(Student.StudentId))
                .Build();

            new EmailClient().SendEmail(email, emailsArray, emailFrom, password, subject);
        }

        private string CreateEmailSubject()
        {
            return "Лизины оценки (" + _dateTimeProvider.GetDateTime().ToShortDateString() + ")";
        }
    }
}
