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

        public SmarsyOperations(ISmarsyRepository repository, ISmarsyBrowser smarsyBrowser)
        {
            _repository = repository;
            _smarsyBrowser = smarsyBrowser;
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
            var resutl = _smarsyBrowser.GetTableObjectFromPage<Ad>(AdsLink, "Ads", Student.SmarsyChildId).ToList();
            _repository.UpsertAds(resutl);
        }

        public void UpdateMarks()
        {
            var result =_smarsyBrowser.GetTableObjectFromPage<LessonMark>(MarksLink, "Marks", Student.SmarsyChildId).ToList();
            _repository.UpserStudentAllLessonsMarks(result);
        }

        public void UpdateStudents()
        {
            var students = _smarsyBrowser.GetTableObjectFromPage<Student>(StudentsLink, "Students", Student.SmarsyChildId).ToList();

            _repository.UpsertStudents(students);
        }

        public void UpdateRemarks()
        {
            var remarks = _smarsyBrowser.GetTableObjectFromPage<Remark>(RemarksLink, "Remarks", Student.SmarsyChildId).ToList();
            _repository.UpsertRemarks(remarks);
        }

        public void UpdateHomeWork()
        {
            var homeWorks = _smarsyBrowser.UpdateHomeWork(this).ToList();

            Repository.UpsertHomeWorks(homeWorks);
        }

        public void SendEmail(IEnumerable<string> emails, string emailFrom, string password)
        {
            string[] emailsArray = emails.ToArray();

            Logger.Info($"Sending email to {string.Join(",", emailsArray)}");

            List<LessonMark> marks = Repository.GetStudentMarks(Student.StudentId);

            new EmailClient().SendEmail(marks, emailsArray, emailFrom, password);
        }
    }
}
