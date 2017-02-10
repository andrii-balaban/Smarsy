using System;
using System.Text;

namespace Smarsy.Extensions
{
    public static class StringBuilderExtension
    {
        public static void AppendWithDashes(this StringBuilder sb, object text)
        {
            sb.Append(text);
            sb.Append(" - ");
        }

        public static void AppendWithNewLine(this StringBuilder sb, object text)
        {
            sb.Append(text);
            sb.Append(Environment.NewLine);
        }
    }
}