using System.Windows.Forms;

namespace SmarsyEntities
{
    public abstract class SmarsyElement
    {
        public abstract void ParseElementFrom(HtmlElement row);

        protected string GetTextBetweenSubstrings(string text, string from, string to)
        {
            return TextProcessor.Processor.GetTextBetweenSubstrings(text, from, to);
        }

        protected string ChangeDateFormat(string date)
        {
            return TextProcessor.Processor.ChangeDateFormat(date);
        }
    }
}