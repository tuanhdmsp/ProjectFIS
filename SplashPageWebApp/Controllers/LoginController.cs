using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConsoleApp1;
using SplashPageWebApp.Models;

namespace SplashPageWebApp.Controllers
{
    public class LoginController : Controller
    {
        testwifiEntities entities = new testwifiEntities();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CheckEmail(String email)
        {
            var success = false;
            if (email.Contains("@gmail.com"))
            {
                //send code to email

                var newCode = entities.GeneratedCodes.Add(new GeneratedCode()
                {
                    code = GenerateCode.GeneratedCode(),
                    email = email,
                });
                entities.SaveChanges();
                SendEmail.SendEmailTo("freewifi.fis@gmail.com","Test Wifi",email, newCode.code);
                success = true;
            }
            return Json(new
            {
                success,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CheckCode(String inpCode)
        {
            var success = false;

            var codes = entities.GeneratedCodes;
            var code = codes.AsEnumerable().Last().code;

            if (inpCode.Equals(code))
            {
                success = true;
            }

            return Json(new
            {
                success
            }, JsonRequestBehavior.AllowGet);
        }
    }
}