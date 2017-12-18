using System.Collections.Generic;
using SmarsyEntities;

namespace Smarsy.Logic
{
    public interface ISmarsyRepository
    {
        void UpsertLessons(IEnumerable<string> lessons);
        void UpsertAds(IEnumerable<Ad> ads);
        void UpsertHomeWorks(IEnumerable<HomeWork> homeWorks);
        void UpserStudentAllLessonsMarks(SmarsyStudent student, IEnumerable<LessonMark> marks);
        int GetLessonIdByLessonShortName(string lessonName);
        int InsertTeacherIfNotExists(string teacherName);
        int GetLessonIdByName(string markLessonName);
        SmarsyStudent GetStudentBySmarsyLogin(string login);
        IEnumerable<StudentDto> GetStudentsWithBirthdayTomorrow();
        IEnumerable<Ad> GetNewAds();
        IEnumerable<Remark> GetNewRemarks();
        IEnumerable<HomeWork> GetHomeWorkForFuture();
        IEnumerable<LessonMark> GetStudentMarks(int studentId);
        void UpsertStudents(IEnumerable<SmarsyStudent> students);
        void UpsertRemarks(IEnumerable<Remark> remarks);
    }
}