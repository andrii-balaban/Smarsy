using System.Collections.Generic;
using SmarsyEntities;

namespace Smarsy.Email
{
    public class EmailBuilder
    {
        private IEnumerable<HomeWork> _homeworks;

        private IEnumerable<LessonMark> _marks;

        private IEnumerable<Student> _tomorrowBirsdayStudents;

        private IEnumerable<Remark> _remark;

        private IEnumerable<Ad> _ads;

        public EmailBuilder WithHomeworks(List<HomeWork> homeWork)
        {
            _homeworks = homeWork;
            return this;
        }

        public EmailBuilder WithMarks(IEnumerable<LessonMark> marks)
        {
            _marks = marks;
            return this;
        }

        public EmailBuilder WithTomorrowBirthDayStudents(IEnumerable<Student> tomorrowBirsdays)
        {
            _tomorrowBirsdayStudents = tomorrowBirsdays;
            return this;
        }

        public EmailBuilder WithRemarks(IEnumerable<Remark> newRemarks)
        {
            _remark = newRemarks;
            return this;
        }

        public EmailBuilder WithAds(IEnumerable<Ad> newAds)
        {
            _ads = newAds;
            return this;
        }

        public Email Build()
        {
            Email email =  new Email();

            email.AddAds(_ads);
            email.AddHomeWork(_homeworks);
            email.AddMarks(_marks);
            email.AddNextDayBirthDayStudents(_tomorrowBirsdayStudents);
            email.AddRemrks(_remark);

            return email;
        }
    }
}