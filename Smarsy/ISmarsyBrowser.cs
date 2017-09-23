using System.Collections.Generic;
using SmarsyEntities;

namespace Smarsy
{
    public interface ISmarsyBrowser
    {
        IEnumerable<T> GetSmarsyElementFromPage<T>(string url, int childId) where T : SmarsyElement;
        void GoToLink(string url);
        void WaitForPageToLoad();
        IEnumerable<HomeWork> UpdateHomeWork(SmarsyOperations smarsyOperations, int smarsyChildId);
        string GetLessonNameFromLessonWithTeacher(string lessonNameWithTeacher);
        string GetTeacherNameFromLessonWithTeacher(string lessonNameWithTeacher, string lessonName);
        void Login(Student student);
    }
}