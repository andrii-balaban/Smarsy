using System;
using System.Collections.Generic;
using SmarsyEntities;

namespace Smarsy.Test
{
    public class SmarsyBrowserStub : ISmarsyBrowser
    {
        public IEnumerable<T> GetTableObjectFromPage<T>(string url, string entityNameForLog, int childId, bool isSkipHeader = true) where T : SmarsyElement<T>
        {
            throw new NotImplementedException();
        }

        public void GoToLink(string url)
        {
            throw new NotImplementedException();
        }

        public void WaitForPageToLoad()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HomeWork> UpdateHomeWork(SmarsyOperations smarsyOperations)
        {
            throw new NotImplementedException();
        }

        public string GetLessonNameFromLessonWithTeacher(string lessonNameWithTeacher)
        {
            throw new NotImplementedException();
        }

        public string GetTeacherNameFromLessonWithTeacher(string lessonNameWithTeacher, string lessonName)
        {
            throw new NotImplementedException();
        }

        public void Login(Student student)
        {
            throw new NotImplementedException();
        }
    }
}
