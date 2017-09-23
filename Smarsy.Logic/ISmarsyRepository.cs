using System.Collections.Generic;
using SmarsyEntities;

namespace Smarsy.Logic
{
    public interface ISmarsyRepository
    {
        void UpsertLessons(List<string> lessons);
        void UpsertAds(IList<Ad> ads);
        void UpsertHomeWorks(List<HomeWork> homeWorks);
        void UpserStudentAllLessonsMarks(IList<LessonMark> marks);
        int GetLessonIdByLessonShortName(string lessonName);
        int InsertTeacherIfNotExists(string teacherName);
        int GetLessonIdByName(string markLessonName);
        Student GetStudentBySmarsyLogin(string login);
        List<Student> GetStudentsWithBirthdayTomorrow();
        List<Ad> GetNewAds();
        List<Remark> GetNewRemarks();
        List<HomeWork> GetHomeWorkForFuture();
        List<LessonMark> GetStudentMarkSummary(int studentId);
        void UpsertStudents(IList<Student> students);
        void UpsertRemarks(IList<Remark> remarks);
    }
}