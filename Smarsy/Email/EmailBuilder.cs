using System;
using System.Collections.Generic;
using System.Linq;
using SmarsyEntities;

namespace Smarsy.Email
{
    public class EmailBuilder
    {
        private IEnumerable<HomeWork> _homeworks;

        private IEnumerable<LessonMark> _marks;

        private IEnumerable<StudentDto> _tomorrowBirsdayStudents;

        private IEnumerable<Remark> _remark;

        private IEnumerable<Ad> _ads;

        private string _subject;

        private string _fromAddress;

        private string[] _toAddresses;

        public EmailBuilder WithFromAddress(string fromAddress)
        {
            _fromAddress = fromAddress;
            return this;
        }

        public EmailBuilder WithToAddresses(IEnumerable<string> toAddresses)
        {
            _toAddresses = toAddresses.ToArray();
            return this;
        }

        public EmailBuilder WithSubject(string subject)
        {
            _subject = subject;
            return this;
        }

        public EmailBuilder WithHomeworks(IEnumerable<HomeWork> homeWork)
        {
            _homeworks = homeWork;
            return this;
        }

        public EmailBuilder WithMarks(IEnumerable<LessonMark> marks)
        {
            _marks = marks;
            return this;
        }

        public EmailBuilder WithTomorrowBirthDayStudents(IEnumerable<StudentDto> tomorrowBirsdays)
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
            if (string.IsNullOrEmpty(_fromAddress))
                throw new ApplicationException("From address should be provided");

            if (_toAddresses == null || !_toAddresses.Any())
                throw  new ApplicationException("At least one TO address should be provided");

            Email email =  new Email(_fromAddress, _toAddresses, _subject);

            email.AddAds(_ads);
            email.AddHomeWork(_homeworks);
            email.AddMarks(_marks);
            email.AddNextDayBirthDayStudents(_tomorrowBirsdayStudents);
            email.AddRemrks(_remark);

            return email;
        }
    }
}