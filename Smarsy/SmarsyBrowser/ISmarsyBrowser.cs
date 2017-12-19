using System.Collections.Generic;
using SmarsyEntities;

namespace Smarsy.SmarsyBrowser
{
    public interface ISmarsyBrowser
    {
        IEnumerable<T> GetSmarsyElementFromPage<T>(SmarsyPage<T> page) where T : SmarsyElement;

        void GoToPage(Page page);
    }
}