using System.Windows.Forms;

namespace SmarsyEntities
{
    public abstract class SmarsyElement
    {
        public abstract void ParseFromHtmlElement(HtmlElement row);

        protected string ChangeDateFormat(string date)
        {
            return TextProcessor.Processor.ChangeDateFormat(date);
        }
    }
}