using System.Linq;
using System.Net;
using System.Security;

namespace SmarsyEntities
{
    public class SmarsyCredentials
    {
        private readonly string _login;
        private readonly SecureString _secureString;

        public SmarsyCredentials(string login, string password)
        {
            _secureString = new SecureString();
            password.ToList().ForEach(c => _secureString.AppendChar(c));

            _login = login;
        }

        public NetworkCredential GetNetworkCredentials()
        {
            return new NetworkCredential(_login, _secureString);
        }
    }
}