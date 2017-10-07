using System.Net;
using System.Security;

namespace SmarsyEntities
{
    public class SmarsyCredentials
    {
        private readonly string _login;
        private readonly SecureString _secureString;

        public SmarsyCredentials(string login, SecureString password)
        {
            _login = login;
            _secureString = password;
        }

        public NetworkCredential GetNetworkCredentials()
        {
            return new NetworkCredential(_login, _secureString);
        }
    }
}