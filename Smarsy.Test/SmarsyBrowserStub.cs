using System;
using System.Collections.Generic;
using Smarsy.SmarsyBrowser;
using SmarsyEntities;

namespace Smarsy.Test
{
    public class SmarsyBrowserStub : ISmarsyBrowser
    {
        public IEnumerable<T> GetSmarsyElementFromPage<T>(SmarsyPage<T> page) where T : SmarsyElement
        {
            throw new NotImplementedException();
        }

        public void GoToPage(Page page)
        {
            
        }
    }
}
