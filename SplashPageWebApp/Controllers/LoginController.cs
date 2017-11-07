using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using SplashPageWebApp.Models;
using SplashPageWebApp.Services;

namespace SplashPageWebApp.Controllers
{
    public class LoginController : Controller
    {
        testwifiEntities entities = new testwifiEntities();

        // GET: Login
        public ActionResult Index()
        {
            if (RouteData.Values.ContainsKey("switch_url"))
            {
                if (RouteData.Values["switch_url"].ToString().Equals("https://1.1.1.1"))
                {
                    if (RouteData.Values.ContainsKey("redirect"))
                    {
                        return View();
                    }
                }
            }
            return View();
        }

        public ActionResult ConnectedError()
        {
            return View();
        }

        public ActionResult AdminPage()
        {
            return View();
        }

        public ActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckEmail(String email)
        {
            var success = false;
            var message = "";
            var id = -1;
            if (email.Contains("@gmail.com"))
            {
                //send code to email

                var newCode = entities.GeneratedCodes.Add(new GeneratedCode()
                {
                    code = GeneratePasswordWifi.Generate(6),
                    email = email,
                });
                entities.SaveChangesAsync().Wait();
                SendEmailWithTemplate.SendTo("freewifi.fis@gmail.com","FPT Wi-Fi Hotspot",email, newCode.code);
                success = true;
                id = Int32.Parse((newCode.datetime != null ? newCode.datetime.Value.ToString("MMddyyyyHHmmss") : "0") + newCode.id.ToString());
            }
            else
            {
                message = "Invalid email! Please enter another one";
            }
            return Json(new
            {
                success,
                message,
                id,
            }, JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
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

        //[ValidateAntiForgeryToken]
        //[HttpPost]
        //public ActionResult Login(LoginViewModel model, string switch_url)
        //{
        //    return Redirect
        //}
    }

    //public class LoginViewModel
    //{
    //    public int buttonClicked { get; set; }
    //    public string redirect_url { get; set; }
    //    public int err_flag { get; set; }
    //}
}