using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SmarsyEntities;

namespace Smarsy.SmarsyBrowser
{
    public class StudentsPage : SmarsyPage<SmarsyStudent>
    {
        public StudentsPage(SmarsyStudent student) : base(student.SmarsyChildId)
        {
        }

        protected override string PageLink => "http://smarsy.ua/private/parent.php?jsid=Grade&lesson=0&tab=List";

        public override IEnumerable<SmarsyStudent> GetSmarsyElementsFromPage()
        {
            return GetPageTableRows().Select(Parse);
        }

        private static SmarsyStudent Parse(HtmlElement r)
        {
            SmarsyStudent student = new SmarsyStudent();
            student.ParseFromHtmlElement(r);

            return student;
        }
    }
}
