using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SmarsyEntities;

namespace Smarsy.SmarsyBrowser
{
    public class MarksPage : SmarsyPage<LessonMark>
    {
        protected override string PageLink => "http://smarsy.ua/private/parent.php?jsid=Diary&tab=Mark";

        public override IEnumerable<LessonMark> GetSmarsyElementsFromPage()
        {
            return GetPageTableRows().Select(CreateLessonMark);
        }

        private static LessonMark CreateLessonMark(HtmlElement r)
        {
            LessonMark lessonMark = new LessonMark();
            lessonMark.ParseFromHtmlElement(r);
            return lessonMark;
        }

        public MarksPage(int childId) : base(childId)
        {
        }
    }
}
