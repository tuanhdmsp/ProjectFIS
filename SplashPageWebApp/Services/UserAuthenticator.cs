using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SplashPageWebApp.Models;

namespace SplashPageWebApp.Services
{
    public class UserAuthenticator
    {
        public static Administrator AuthenticateAdmin(string email, string password)
        {
            testwifiEntities entities = new testwifiEntities();
            Administrator admin = entities.Administrators.AsQueryable().SingleOrDefault(a => a.email == email);
            if (admin != null)
            {
                if (password.Equals(admin.password))
                {
                    return admin;
                }
            }
            return null;
        }
    }
}