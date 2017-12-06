using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SplashPageWebApp.Models;
using SplashPageWebApp.Services;

namespace SplashPageWebApp.Controllers
{
    public class AdminController : Controller
    {
        private testwifiEntities entities = new testwifiEntities();

        public ActionResult AdminPage()
        {
            if (String.IsNullOrEmpty((String)Session["user"]))
            {
                return RedirectToAction("AdminLogin");
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult AdminLogin()
        {
            if (String.IsNullOrEmpty((String)Session["user"]))
                return View();
            else
            {
                return RedirectToAction("AdminPage");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult AdminLogin(LoginViewModel loginInfo)
        {
            if (String.IsNullOrEmpty((String)Session["user"]))
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                Administrator admin = UserAuthenticator.AuthenticateAdmin(loginInfo.email, loginInfo.password);
                if (admin != null)
                {
                    Session["user"] = admin.email.Substring(0, admin.email.IndexOf("@"));
                    return RedirectToAction("AdminPage");
                }
                else
                {
                    ViewBag.failMessage = "Wrong email or password!";
                    return View();
                }
            }
            return RedirectToAction("AdminPage");
        }

        public ActionResult AdminLogout()
        {
            Session.Abandon();
            return RedirectToAction("AdminLogin");
        }

        [HttpPost]
        public ActionResult GetLogData(JQueryDataTableParamModel param)
        {
            if (String.IsNullOrEmpty((String) Session["user"]))
            {
                return RedirectToAction("AdminLogin");
            }
            var list = entities.Codes.AsEnumerable();

            var totalResult = list.Count();

            if (!String.IsNullOrEmpty(param.sSearch))
            {
                list = list.Skip(param.iDisplayStart).Take(param.iDisplayLength).Where(c => c.sponsorEmail.ToLower().Contains(param.sSearch.ToLower()));
            }

            var totalDisplay = list.Count();
            
            var result = list.ToList().Select(a => new IConvertible[]
            {
                a.id,
                a.sponsorEmail,
                a.code1,
                a.startTime.ToString("dd/MM/yyyy HH:mm:ss"),
                a.isUsed && !a.isActive,
                null
            });
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalResult,
                iTotalDisplayRecords = totalDisplay,
                aaData = result,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteCode(int[] id)
        {
            var success = false;
            var message = "";
            var error = "";

            try
            {
                foreach (var i in id)
                {
                    var code = entities.Codes.SingleOrDefault(c => c.id == i);
                    if (code != null)
                    {
                        entities.Codes.Remove(code);
                        entities.SaveChanges();
                        success = true;
                    }
                    else
                    {
                        if (id.Length == 1)
                        {
                            message = "No such code";
                        }
                        else
                        {
                            message = "Unable to delete some codes";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(400, e.Message);
            }
            return Json( new
            {
                success,
                message
            }, JsonRequestBehavior.AllowGet);
        }
        
    }

    public class LoginViewModel
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class JQueryDataTableParamModel
    {
        /// <summary>
        /// Request sequence number sent by DataTable,
        /// same value must be returned in response
        /// </summary>       
        public string sEcho { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }

        /// <summary>
        /// Sort column
        /// </summary>
        public int iSortCol_0 { get; set; }

        /// <summary>
        /// Asc or Desc
        /// </summary>
        public string sSortDir_0 { get; set; }
    }
}