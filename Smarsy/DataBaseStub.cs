using System;
using System.Collections.Generic;
using Smarsy.Logic;
using SmarsyEntities;

namespace Smarsy
{
    public class DataBaseStub : ISmarsyRepository
    {
        public void UpsertLessons(List<string> lessons)
        {
            throw new NotImplementedException();
        }

        public void UpsertAds(IList<Ad> ads)
        {
            throw new NotImplementedException();
        }

        public void UpsertHomeWorks(List<HomeWork> homeWorks)
        {
            throw new NotImplementedException();
        }

        public void UpserStudentAllLessonsMarks(IList<LessonMark> marks)
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

        public Student GetStudentBySmarsyLogin(string login)
        {
            throw new NotImplementedException();
        }

        public List<Student> GetStudentsWithBirthdayTomorrow()
        {
            throw new NotImplementedException();
        }

        public List<Ad> GetNewAds()
        {
            throw new NotImplementedException();
        }

        public List<Remark> GetNewRemarks()
        {
            throw new NotImplementedException();
        }

        public List<HomeWork> GetHomeWorkForFuture()
        {
            throw new NotImplementedException();
        }

        public List<LessonMark> GetStudentMarks(int studentId)
        {
            throw new NotImplementedException();
        }

        public void UpsertStudents(IList<Student> students)
        {
            throw new NotImplementedException();
        }

        public void UpsertRemarks(IList<Remark> remarks)
        {
            throw new NotImplementedException();
        }
    }
}
