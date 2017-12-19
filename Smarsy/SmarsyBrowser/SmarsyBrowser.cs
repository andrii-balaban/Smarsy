using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NLog;
using SmarsyEntities;

namespace Smarsy.SmarsyBrowser
{
    public class SmarsyBrowser : ISmarsyBrowser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SmarsyBrowser()
        {
            Browser = new WebBrowser();
        }

        private WebBrowser Browser { get; }

        public IEnumerable<T> GetSmarsyElementFromPage<T>(SmarsyPage<T> page) where T : SmarsyElement
        {
            GoToPage(page);

            if (!page.IsPageLoaded())
            {
                return Enumerable.Empty<T>();
            }

            return page.GetSmarsyElementsFromPage();
        }

        public void GoToPage(Page page)
        {
            GoToLink(page.GetPageAddress());
            page.SetPageDocument(Browser.Document);
            page.AfterLoaded();
        }

        public void GoToLink(string url)
        {
            Logger.Info($"Go to {url} page");
            Browser.Navigate(url);
            WaitForPageToLoad();
        }

        private void WaitForPageToLoad()
        {
            while (Browser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            Thread.Sleep(500);
        }
    }
}