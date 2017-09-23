using System.Windows.Forms;

namespace SmarsyEntities
{
    using System;

    public class Ad : SmarsyElement<Ad>
    {
        public DateTime AdDate { get; set; }

        public string AdText { get; set; }

        public override Ad GetElement(HtmlElement row)
        {
            var ad = new Ad();
            var i = 1;

            foreach (HtmlElement oneROw in row.GetElementsByTagName("td"))
            {
                if (i == 1)
                {
                    ad.AdDate = DateTime.ParseExact(oneROw.InnerHtml, "dd.mm.yyyy", null);
                }

                if (i == 2)
                {
                    ad.AdText = oneROw.InnerHtml;
                }

                i++;
            }

            return ad;
        }
    }
}