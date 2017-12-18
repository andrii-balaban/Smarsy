using System;
using System.Collections.Generic;
using System.Security;
using Smarsy.Logic;
using SmarsyEntities;

namespace Smarsy.Test
{
    public class DataBaseStub : ISmarsyRepository
    {
        public void UpsertLessons(IEnumerable<string> lessons)
        {
            throw new NotImplementedException();
        }

        public void UpsertAds(IEnumerable<Ad> ads)
        {
            throw new NotImplementedException();
        }

        public void UpsertHomeWorks(IEnumerable<HomeWork> homeWorks)
        {
            throw new NotImplementedException();
        }

        public void UpserStudentAllLessonsMarks(SmarsyStudent student, IEnumerable<LessonMark> marks)
        {
            throw new NotImplementedException();
        }

        public int GetLessonIdByLessonShortName(string lessonName)
        {
            throw new NotImplementedException();
        }

        public int InsertTeacherIfNotExists(string teacherName)
        {
            throw new NotImplementedException();
        }

        public int GetLessonIdByName(string markLessonName)
        {
            throw new NotImplementedException();
        }

        public SmarsyStudent GetStudentBySmarsyLogin(string login)
        {
            return new SmarsyStudent(new StudentDto { Login = login, Password = new SecureString()});
        }

        public IEnumerable<StudentDto> GetStudentsWithBirthdayTomorrow()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Ad> GetNewAds()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Remark> GetNewRemarks()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HomeWork> GetHomeWorkForFuture()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LessonMark> GetStudentMarks(int studentId)
        {
            throw new NotImplementedException();
        }

        public void UpsertStudents(IEnumerable<SmarsyStudent> students)
        {
            throw new NotImplementedException();
        }

        public void UpsertRemarks(IEnumerable<Remark> remarks)
        {
            throw new NotImplementedException();
        }
    }
}
