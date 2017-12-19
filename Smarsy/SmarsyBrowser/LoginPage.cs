using System;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using SmarsyEntities;

namespace Smarsy.SmarsyBrowser
{
    public class LoginPage : Page
    {
        protected override string PageLink => "http://www.smarsy.ua";

        private readonly SmarsyStudent _smarsyStudent;

        public LoginPage(SmarsyStudent student)
        {
            _smarsyStudent = student;
        }

        public override void AfterLoaded()
        {
            EnterCredentials();

            base.AfterLoaded();
        }

        private void EnterCredentials()
        {
            NetworkCredential credentials = _smarsyStudent.Credentials.GetNetworkCredentials();
            FillElementWithValue("username", credentials.UserName);
            FillElementWithValue("password", credentials.Password);

            ClickOnLoginButton();
        }

        private void FillElementWithValue(string elementId, string value)
        {
            if (!IsPageLoaded() || elementId == null)
            {
                return;
            }

            var element = Document.GetElementById(elementId);
            if (element == null)
            {
                throw new ApplicationException($"{elementId} was not found.");
            }

            element.InnerText = value;
        }
        
        private void ClickOnLoginButton()
        {
            if (!IsPageLoaded())
            {
                return;
            }

            HtmlElement button = GetSubmitButton();

            if (button == null)
            {
                throw new ApplicationException("Submit button was not found");
            }

            button.InvokeMember("click");
        }

        private HtmlElement GetSubmitButton()
        {
            return Document.GetElementsByTagName("input").OfType<HtmlElement>().FirstOrDefault(e => e.Name == "submit");
        }
    }
}
