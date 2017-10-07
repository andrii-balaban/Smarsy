using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SmarsyEntities;

namespace Smarsy.SmarsyBrowser
{
    public class RemarksPage : SmarsyPage<Remark>
    {
        public RemarksPage(SmarsyStudent student) : base(student.SmarsyChildId)
        {
        }

        protected override string PageLink => "http://smarsy.ua/private/parent.php?jsid=Remark&tab=List";

        public override IEnumerable<Remark> GetSmarsyElementsFromPage()
        {
            return GetPageTableRows().Select(CreateRemark);
        }

        private static Remark CreateRemark(HtmlElement r)
        {
            Remark remark = new Remark();
            remark.ParseFromHtmlElement(r);
            return remark;
        }
    }
}
