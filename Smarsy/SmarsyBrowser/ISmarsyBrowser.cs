using System.Collections.Generic;
using SmarsyEntities;

namespace Smarsy.SmarsyBrowser
{
    public interface ISmarsyBrowser
    {
        IEnumerable<T> GetSmarsyElementFromPage<T>(SmarsyPage<T> page) where T : SmarsyElement;
        void GoToLink(string url);
        void WaitForPageToLoad();
        IEnumerable<HomeWork> UpdateHomeWork(SmarsyOperations smarsyOperations, int smarsyChildId);
        string GetLessonNameFromLessonWithTeacher(string lessonNameWithTeacher);
        string GetTeacherNameFromLessonWithTeacher(string lessonNameWithTeacher, string lessonName);
        void Login(Student student);
    }
}