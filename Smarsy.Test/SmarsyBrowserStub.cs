using System;
using System.Collections.Generic;
using Smarsy.SmarsyBrowser;
using SmarsyEntities;

namespace Smarsy.Test
{
    public class SmarsyBrowserStub : ISmarsyBrowser
    {
        public IEnumerable<T> GetSmarsyElementFromPage<T>(string url, int childId) where T : SmarsyElement
        {
            throw new NotImplementedException();
        }

        public void GoToLink(string url)
        {
            
        }

        public void WaitForPageToLoad()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HomeWork> UpdateHomeWork(SmarsyOperations smarsyOperations, int smarsyChildId)
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
            
        }
    }
}
