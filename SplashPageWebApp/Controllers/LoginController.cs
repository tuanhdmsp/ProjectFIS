using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using ServicesFIS;
using SplashPageWebApp.Models;

namespace SplashPageWebApp.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly testwifiEntities entities = new testwifiEntities();

        // GET: Login
        public ActionResult Index()
        {
            var paramCollection = HttpContext.Request.Params;
            var keys = paramCollection.AllKeys;

            var path = HttpContext.Server.MapPath("~/log.txt");
            using (StreamWriter sr = System.IO.File.AppendText(path ?? "log.txt"))
            {
                sr.WriteLine("-----------------------------------------------------------------------------------------------------------------");
                foreach (var key in keys)
                {
                    sr.Write(key + ": ");
                    var values = paramCollection.GetValues(key);
                    if (values != null)
                    {
                        foreach (var value in values)
                        {
                            sr.Write(value + " ");
                        }
                    }
                    sr.WriteLine();
                }
                sr.WriteLine();
            }

            if (keys.Contains("switch_url"))
            {
                var switchUrlValues = paramCollection.GetValues("switch_url");
                if (switchUrlValues != null && switchUrlValues.Length == 1)
                    if (switchUrlValues[0].Equals("https://1.1.1.1/login.html"))
                        if (keys.Contains("redirect"))
                        {
                            var redirectValues = paramCollection.GetValues("redirect");
                            if (redirectValues != null && redirectValues.Length == 1)
                                return View();
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
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckEmail(string sponsorEmail, string guestName, string userEmail)
        {
            var success = false;
            var generatedCode = "";
            var message = "";
            var userId = -1;
            var id = -1;
            if (sponsorEmail.Contains("@gmail.com"))
            {
                //send code to email

                var newCode = entities.Codes.Add(new Code
                {
                    code1 = GeneratePasswordWifi.Generate(6),
                    sponsorEmail = sponsorEmail,
                    startTime = DateTime.Now,
                    expiredTime = DateTime.Now.AddHours(4),
                    isUsed = false
                });
                var newUser = entities.Users.Add(new User
                {
                    fullName = !string.IsNullOrEmpty(guestName) ? guestName : sponsorEmail.Substring(0, sponsorEmail.IndexOf("@")),
                    userEmail = userEmail,
                });
                try
                {
                    entities.SaveChangesAsync().Wait();
                    SendEmailWithTemplate.SendTo("freewifi.fis@gmail.com", "FPT Wi-Fi Hotspot", sponsorEmail, newCode.code1);
                    success = true;
                    generatedCode = newCode.code1;
                    userId = newUser.id;
                    //id = HashingHandler.SHA256Hashing((newCode.datetime?.ToString("MMddyyyyHHmmss") ?? "0") + newCode.id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
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
                generatedCode,
                userId,
            }, JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CheckCode(string inpCode, int userId)
        {
            var success = false;
            var message = "The code is unavailable";

            var codes = entities.Codes;

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

            var code = codes.SingleOrDefault(c =>
                c.code1.Equals(inpCode) && !(c.isUsed) && DateTime.Compare(c.expiredTime, DateTime.Now) > 0);
            if (code != null)
            {
                var user = entities.Users.SingleOrDefault(u => u.id == userId);
                if (user != null)
                {
                    user.usedCode = inpCode;
                }
                success = true;
                code.isUsed = true;
                entities.SaveChangesAsync();
            }
            return Json(new
            {
                success,
                message
            }, JsonRequestBehavior.AllowGet);
        }

        //[ValidateAntiForgeryToken]
        //[HttpPost]
        //public ActionResult Login(LoginViewModel model, string switch_url)
        //{
        //    return Redirect
        //}

        public ActionResult Download()
        {
            var path = HttpContext.Server.MapPath("~/log.txt");
            byte[] file = System.IO.File.ReadAllBytes(path);
            string filename = "log.txt";
            return File(file, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }
    }

    //public class LoginViewModel
    //{
    //    public int buttonClicked { get; set; }
    //    public string redirect_url { get; set; }
    //    public int err_flag { get; set; }
    //}
}