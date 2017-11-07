﻿using System;
using System.Collections.Generic;
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
            if (email.Contains("@gmail.com"))
            {
                //send code to email

                var newCode = entities.GeneratedCodes.Add(new GeneratedCode()
                {
                    code = GeneratePasswordWifi.Generate(6),
                    email = email,
                });
                entities.SaveChanges();
                SendEmailWithTemplate.SendTo("freewifi.fis@gmail.com","FPT Wi-Fi Hotspot",email, newCode.code);
                success = true;
            }
            return Json(new
            {
                success,
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