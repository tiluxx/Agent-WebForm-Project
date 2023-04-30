using Agent_WebForm_Prodject.Models;
using Agent_WebForm_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agent_WebForm_Prodject.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorize(UserAccount userAccountModel)
        {
            using (DistributorDBEntities db = new DistributorDBEntities())
            {
                var agentAccounts = db.UserAccounts.Where(staff => staff.UserName == userAccountModel.UserName && staff.UserPassword == userAccountModel.UserPassword).FirstOrDefault();
                if (agentAccounts == null)
                {
                    userAccountModel.LoginMessageError = "Invalid account";
                    return View("Index", userAccountModel);
                }
                else
                {
                    StaffAccount staffAccount = new agentAccounts();
                    Session["StaffID"] = staffAccount.GetStaffID(userAccountModel.UserName);
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}