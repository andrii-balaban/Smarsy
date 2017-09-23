using System.Windows.Forms;

namespace Smarsy.SmarsyBrowser
{
    public class LoginPage : Page
    {
        private readonly string _login;
        private readonly string _password;

        public LoginPage(string login, string password)
        {
            _password = password;
            _login = login;
        }

        protected override string PageLink => "http://www.smarsy.ua";

        public void EnterLogin()
        {
            FillTextBoxByElementId("username", _login);
            FillTextBoxByElementId("password", _password);

            ClickOnLoginButton();
        }

        private void FillTextBoxByElementId(string elementId, string value)
        {
            if (!IsPageLoaded() || elementId == null)
            {
                return;
            }

            var element = Document.GetElementById(elementId);
            if (element != null)
            {
                element.InnerText = value;
            }
        }


        private void ClickOnLoginButton()
        {
            if (!IsPageLoaded())
            {
                return;
            }

            var bclick = Document.GetElementsByTagName("input");
            foreach (HtmlElement btn in bclick)
            {
                var name = btn.Name;
                if (name == "submit")
                {
                    btn.InvokeMember("click");
                }
            }
        }
    }
}
