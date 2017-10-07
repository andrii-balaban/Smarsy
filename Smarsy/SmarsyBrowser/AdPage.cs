
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SmarsyEntities;

namespace Smarsy.SmarsyBrowser
{
    public class AdPage : SmarsyPage<Ad>
    {
        public AdPage(SmarsyStudent student) : base(student.SmarsyChildId)
        {
        }

        protected override string PageLink => "http://smarsy.ua/private/parent.php?jsid=Announ&tab=List";

        public override IEnumerable<Ad> GetSmarsyElementsFromPage()
        {
            return GetPageTableRows().Select(CreateAd);
        }

        private static Ad CreateAd(HtmlElement r)
        {
            Ad ad = new Ad();
            ad.ParseFromHtmlElement(r);
            return ad;
        }
    }
}
