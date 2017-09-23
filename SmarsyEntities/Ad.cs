using System.Windows.Forms;

namespace SmarsyEntities
{
    using System;

    public class Ad : SmarsyElement
    {
        public DateTime AdDate { get; set; }

        public string AdText { get; set; }

        public override void ParseElementFrom(HtmlElement row)
        {
            var i = 1;

            foreach (HtmlElement oneROw in row.GetElementsByTagName("td"))
            {
                if (i == 1)
                {
                    AdDate = DateTime.ParseExact(oneROw.InnerHtml, "dd.mm.yyyy", null);
                }

                if (i == 2)
                {
                    AdText = oneROw.InnerHtml;
                }

                i++;
            }
        }
    }
}