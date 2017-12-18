using System.Collections.Generic;
using SmarsyEntities;

namespace Smarsy.Logic
{
    public interface ISmarsyRepository
    {
        void UpsertLessons(List<string> lessons);
        void UpsertAds(IEnumerable<Ad> ads);
        void UpsertHomeWorks(List<HomeWork> homeWorks);
        void UpserStudentAllLessonsMarks(SmarsyStudent student, IEnumerable<LessonMark> marks);
        int GetLessonIdByLessonShortName(string lessonName);
        int InsertTeacherIfNotExists(string teacherName);
        int GetLessonIdByName(string markLessonName);
        SmarsyStudent GetStudentBySmarsyLogin(string login);
        List<StudentDto> GetStudentsWithBirthdayTomorrow();
        List<Ad> GetNewAds();
        List<Remark> GetNewRemarks();
        List<HomeWork> GetHomeWorkForFuture();
        List<LessonMark> GetStudentMarks(int studentId);
        void UpsertStudents(IList<SmarsyStudent> students);
        void UpsertRemarks(IList<Remark> remarks);
    }
}