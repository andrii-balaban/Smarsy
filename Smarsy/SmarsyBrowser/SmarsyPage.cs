using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SmarsyEntities;

namespace Smarsy.SmarsyBrowser
{
    public abstract class SmarsyPage<T> where T : SmarsyElement
    {
        private readonly int _childId;

        protected SmarsyPage(int childId)
        {
            _childId = childId;
        }

        protected abstract string PageLink { get; }

        protected HtmlDocument Document { get; set; }

        public string GetPageAddress()
        {
            return $"{PageLink}&child={_childId}";
        }

        public void SetPageDocument(HtmlDocument document)
        {
            Document = document;
        }

        public bool IsPageLoaded()
        {
            return Document != null;
        }

        protected IEnumerable<HtmlElement> GetPageTableRows()
        {
            if (!IsPageLoaded())
                return Enumerable.Empty<HtmlElement>();

            return Document.GetElementsByTagName("table").OfType<HtmlElement>()
                .Skip(1) // skip the first table on the page
                .Take(1) // take the only second table on the page
                .SelectMany(row => row.GetElementsByTagName("tr").OfType<HtmlElement>())
                .Skip(1) // skip header row
                .ToArray();
        }

        public abstract IEnumerable<T> GetSmarsyElementsFromPage();
    }
}
