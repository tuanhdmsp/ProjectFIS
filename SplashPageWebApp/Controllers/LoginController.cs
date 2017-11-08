using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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
            var paramCollection = HttpContext.Request.Params;
            var keys = paramCollection.AllKeys;
            if (keys.Contains("switch_url"))
            {
                var switchUrlValues = paramCollection.GetValues("switch_url");
                if (switchUrlValues != null && switchUrlValues.Length == 1)
                {
                    if (switchUrlValues[0].Equals("https://1.1.1.1/login.html"))
                    {
                        if (keys.Contains("redirect"))
                        {
                            var redirectValues = paramCollection.GetValues("redirect");
                            if (redirectValues != null && redirectValues.Length == 1)
                            {
                                return View();
                            }
                        }
                    }
                }
            }
            
            //var tokens = HttpContext.Request.Params.AllKeys;
            //Console.WriteLine(tokens);
            //foreach (var token in tokens.AllKeys)
            //{
            //    var values = tokens.GetValues(token);
            //    Console.WriteLine(token.ToString());
            //    foreach (var value in values)
            //    {
            //        Console.WriteLine(value);
            //    }
            //    Console.WriteLine();
            //}
            //return View();
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
            var id = -1;
            if (email.Contains("@gmail.com"))
            {
                //send code to email

                var newCode = entities.GeneratedCodes.Add(new GeneratedCode()
                {
                    code = GeneratePasswordWifi.Generate(6),
                    email = email,
                    fullname = !String.IsNullOrEmpty(guestName) ? guestName : email.Substring(0, email.IndexOf("@")),
                    isUsed = false,
                    datetime = null,
                    expiredTime = null
                });
                try
                {
                    entities.SaveChangesAsync().Wait();
                    SendEmailWithTemplate.SendTo("freewifi.fis@gmail.com", "FPT Wi-Fi Hotspot", email, newCode.code);
                    success = true;
                    id = newCode.id;
                    //id = HashingHandler.SHA256Hashing((newCode.datetime?.ToString("MMddyyyyHHmmss") ?? "0") + newCode.id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    message = "Something went wrong!";
                }
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
            
            //var filterCodes = codes.Where(c => c.email == email && (c.isUsed ?? false) && DateTime.Compare(c.expiredTime.Value, DateTime.Now) <= 0)
            //    .OrderByDescending(c => c.datetime).AsEnumerable();

            //if (filterCodes.Any())
            //{
            //    var uniCode = filterCodes.SingleOrDefault(c =>
            //    {
            //        if (codeId.Equals(HashingHandler.SHA256Hashing((c.datetime?.ToString("MMddyyyyHHmmss") ?? "0") + c.id.ToString()))) return true;
            //        return false;
            //    });

            //    var uniCode = filterCodes.SingleOrDefault();

            //    if (uniCode != null)
            //    {
            //        uniCode.isUsed = true;
            //        entities.SaveChangesAsync();
            //        success = true;
            //    }
            //}

            var code = codes.SingleOrDefault(c => c.code.Equals(inpCode) && !(c.isUsed??true));
            if (code != null)
            {
                success = true;
                code.isUsed = true;
                entities.SaveChangesAsync();
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