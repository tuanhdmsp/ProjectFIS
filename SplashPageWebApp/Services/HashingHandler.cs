using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SplashPageWebApp.Services
{
    public class HashingHandler
    {
        public static string SHA512Hashing(string inp)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(inp);
            byte[] hashBytes = (new SHA512Managed()).ComputeHash(bytes);
            return Encoding.UTF8.GetString(hashBytes);
        }

        public static string SHA256Hashing(string inp)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(inp);
            byte[] hashBytes = (new SHA256Managed()).ComputeHash(bytes);
            return Encoding.UTF8.GetString(hashBytes);
        }

        public static string SHA384Hashing(string inp)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(inp);
            byte[] hashBytes = (new SHA384Managed()).ComputeHash(bytes);
            return Encoding.UTF8.GetString(hashBytes);
        }

        public static string SHA1Hashing(string inp)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(inp);
            byte[] hashBytes = (new SHA1Managed()).ComputeHash(bytes);
            return Encoding.UTF8.GetString(hashBytes);
        }
    }
}