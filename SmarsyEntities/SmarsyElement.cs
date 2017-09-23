﻿using System;
using System.Windows.Forms;

namespace SmarsyEntities
{
    public abstract class SmarsyElement<T>
    {
        public static T GetElement<T>(HtmlElement row) where T : SmarsyElement<T>
        {
            return (Activator.CreateInstance(typeof(T)) as T).GetElement(row);
        }

        public abstract T GetElement(HtmlElement row);

        protected string GetTextBetweenSubstrings(string text, string from, string to)
        {
            return TextProcessor.Processor.GetTextBetweenSubstrings(text, @from, to);
        }

        protected string ChangeDateFormat(string date)
        {
            return TextProcessor.Processor.ChangeDateFormat(date);
        }
    }
}