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
                    // Start session
                    AgentAccount agentAccount = new AgentAccount();
                    string agentId = agentAccount.GetAgentID(userAccountModel.UserName);
                    Session["AgentID"] = agentId;

                    // Get the current cart of agent
                    AgentCart cart = new AgentCart();
                    AgentCartDetail agentCartDetail = new AgentCartDetail();
                    AgentCart agentCart = cart.SelectAgentCartQuery(agentId);

                    if (agentCart.CartID != null)
                    {
                        List<Product> productList = agentCartDetail.SelectProductCartQuery(agentCart.CartID);
                        Session["CurrCartList"] = productList;
                        Session["CartQuan"] = productList.Count();
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        public ActionResult Logout()
        {
            Session["AgentID"] = null;
            Session["CurrCartList"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}