using System.Windows.Forms;

namespace Smarsy.SmarsyBrowser
{
    public enum PageType
    {
        Login,
        Ads
    }

    public abstract class Page
    {
        protected abstract string PageLink { get; }
        protected HtmlDocument Document { get; set; }

        public virtual string GetPageAddress()
        {
            return PageLink;
        }

        public void SetPageDocument(HtmlDocument document)
        {
            Document = document;
        }

        public bool IsPageLoaded()
        {
            return Document != null;
        }
    }
}