using System.Linq;

namespace Smarsy
{
    using System.Collections.Generic;
    using Logic;
    using NLog;
    using SmarsyEntities;

    public class SmarsyOperations
    {
        private const string AdsLink = "http://smarsy.ua/private/parent.php?jsid=Announ&tab=List";
        private const string MarksLink = "http://smarsy.ua/private/parent.php?jsid=Diary&tab=Mark";
        private const string StudentsLink = "http://smarsy.ua/private/parent.php?jsid=Grade&lesson=0&tab=List";
        private const string RemarksLink = "http://smarsy.ua/private/parent.php?jsid=Remark&tab=List";
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
            List<Ad> resutl = _smarsyBrowser.GetTableObjectFromPage<Ad>(AdsLink, "Ads", Student.SmarsyChildId).ToList();

            Logger.Info("Upserting Ads in database");
            _repository.UpsertAds(resutl);
        }

        public void UpdateMarks()
        {
            List<LessonMark> result =_smarsyBrowser.GetTableObjectFromPage<LessonMark>(MarksLink, "Marks", Student.SmarsyChildId).ToList();

            Logger.Info("Upserting LessonMark in database");
            _repository.UpserStudentAllLessonsMarks(result);
        }

        public void UpdateStudents()
        {
            List<Student> students = _smarsyBrowser.GetTableObjectFromPage<Student>(StudentsLink, "Students", Student.SmarsyChildId).ToList();

            Logger.Info("Upserting Students in database");
            _repository.UpsertStudents(students);
        }

        public void UpdateRemarks()
        {
            List<Remark> remarks = _smarsyBrowser.GetTableObjectFromPage<Remark>(RemarksLink, "Remarks", Student.SmarsyChildId).ToList();

            Logger.Info("Upserting Remarks in database");
            _repository.UpsertRemarks(remarks);
        }

        public void UpdateHomeWork()
        {
            List<HomeWork> homeWorks = _smarsyBrowser.UpdateHomeWork(this).ToList();

            Logger.Info("Upserting Homework in database");
            Repository.UpsertHomeWorks(homeWorks);
        }

        public void SendEmail(IEnumerable<string> emails, string emailFrom, string password)
        {
            string[] emailsArray = emails.ToArray();

            Logger.Info($"Sending email to {string.Join(",", emailsArray)}");

            string subject = CreateEmailSubject();

            Email email = new EmailBuilder()
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
