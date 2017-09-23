using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SmarsyEntities;

namespace Smarsy.SmarsyBrowser
{
    public class StudentsPage : SmarsyPage<Student>
    {
        public StudentsPage(int childId) : base(childId)
        {
        }

        protected override string PageLink => "http://smarsy.ua/private/parent.php?jsid=Grade&lesson=0&tab=List";

        public override IEnumerable<Student> GetSmarsyElementsFromPage()
        {
            return GetPageTableRows().Select(CreateStudent);
        }

        private static Student CreateStudent(HtmlElement r)
        {
            Student student = new Student();
            student.ParseFromHtmlElement(r);
            return student;
        }
    }
}
