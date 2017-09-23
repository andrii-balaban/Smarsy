using System;
using System.Windows.Forms;

namespace SmarsyEntities
{
    public class SmarsyEntitiesFactory
    {
        public T CreateElementOfType<T>(HtmlElement row) where T : SmarsyElement
        {
            var element = Activator.CreateInstance(typeof(T)) as SmarsyElement;

            element?.ParseFromHtmlElement(row);

            return element as T;
        }
    }
}
