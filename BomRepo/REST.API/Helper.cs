using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BomRepo.REST.API
{
    public static class Helper
    {
        public static Tuple<string, string> GetAuthorizationHeaderValuesFromString(string AuthorizationHeaderString)
        {
            if (AuthorizationHeaderString == string.Empty) return null;
            if (AuthorizationHeaderString.IndexOf(" ") == -1) return null;

            string[] parts = AuthorizationHeaderString.Split(" ");
            return new Tuple<string, string>(parts[0], parts[1]);
        }

        public static Tuple<string, string> GetUsernameAndPasswordFromEncodedString(string EncodedString)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(EncodedString);
            string decodedAuthHeader = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

            if (!decodedAuthHeader.Contains(":")) return new Tuple<string, string>(string.Empty, string.Empty);

            string[] userpassword = decodedAuthHeader.Split(":");

            return new Tuple<string, string>(userpassword[0], userpassword[1]);
        }
    }
}
