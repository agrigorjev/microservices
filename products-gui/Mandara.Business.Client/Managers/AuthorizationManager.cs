using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mandara.Business.Client.Managers
{
    public class AuthorizationManager
    {
        public static string ComputeHash(string input)
        {
            if (input == null)
                return null;

            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = new SHA256CryptoServiceProvider().ComputeHash(inputBytes);

            return Convert.ToBase64String(hashedBytes);
        }
    }
}
