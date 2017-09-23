using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SmarsyEntities;

namespace Smarsy.SmarsyBrowser
{
    public abstract class SmarsyPage<T> : Page where T : SmarsyElement
    {
        private readonly int _childId;

        protected SmarsyPage(int childId)
        {
            _childId = childId;
        }

        public override string GetPageAddress()
        {
            return $"{PageLink}&child={_childId}";
        }

        protected virtual IEnumerable<HtmlElement> GetPageTableRows()
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
