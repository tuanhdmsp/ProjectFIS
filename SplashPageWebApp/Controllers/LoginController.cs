using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
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
                if (RouteData.Values["switch_url"].ToString().Equals("https://1.1.1.1/login.html"))
                {
                    if (RouteData.Values.ContainsKey("redirect"))
                    {
                        if ((RouteData.Values["redirect"]) != null)
                        {
                            return View();
                        }
                    }
                }
            }
            return RedirectToAction("ConnectedError");
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
        public ActionResult CheckEmail(String email, String guestName)
        {
            var success = false;
            var message = "";
            var id = "";
            if (email.Contains("@gmail.com"))
            {
                //send code to email

                var newCode = entities.GeneratedCodes.Add(new GeneratedCode()
                {
                    code = GeneratePasswordWifi.Generate(6),
                    email = email,
                    fullname = guestName,
                    isUsed = false,
                });
                entities.SaveChangesAsync().Wait();
                SendEmailWithTemplate.SendTo("freewifi.fis@gmail.com","FPT Wi-Fi Hotspot",email, newCode.code);
                success = true;
                id = HashingHandler.SHA256Hashing((newCode.datetime?.ToString("MMddyyyyHHmmss") ?? "0") + newCode.id);
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
        public ActionResult CheckCode(String inpCode, string email, string codeId)
        {
            var success = false;
            var message = "The code is unavailable";
            
            var codes = entities.GeneratedCodes;
            var filterCodes = codes.Where(c => c.email == email && (c.isUsed ?? false) && DateTime.Compare(c.expiredTime.Value, DateTime.Now) <= 0)
                .OrderByDescending(c => c.datetime).AsEnumerable();

            if (filterCodes.Any())
            {
                var uniCode = filterCodes.SingleOrDefault(c =>
                {
                    if (HashingHandler.SHA256Hashing(codeId)
                        .Equals(HashingHandler.SHA256Hashing((c.datetime?.ToString("MMddyyyyHHmmss") ?? "0") +
                                                             c.id.ToString()))) return true;
                    return false;
                });

                if (uniCode != null)
                {
                    uniCode.isUsed = true;
                    entities.SaveChangesAsync();
                    success = true;
                }
            }
            return Json(new
            {
                success,
                message,
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