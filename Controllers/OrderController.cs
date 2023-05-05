using Agent_WebForm_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Agent_WebForm_Project.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        // GET: Order/Result
        public ActionResult Result()
        {
            return View();
        }

        // GET: Order/ViewOrder
        public ActionResult ViewOrder()
        {
            if (Session["AgentID"] == null || Session["AgentID"].ToString().Equals(""))
            {
                return View("Index");
            }
            C_Order order = new C_Order();
            List<C_Order> orderList = order.SelectAgentOrderQuery(Session["AgentID"].ToString());
            ViewBag.OrderList = orderList;
            return View();
        }

        public ActionResult VnPaymentResponse(bool status, string message, long transactionNo = 0, long payDate = 0, string orderId = "")
        {
            if (status)
            {
                C_Order order = new C_Order();
                order.UpdateOrderQuery(orderId, transactionNo, payDate);
            }
            ViewBag.Message = message;
            return View("Result");
        }

        [HttpPost]
        public ActionResult PlaceOrder()
        {
            C_Order order = new C_Order();
            string newOrderID = order.GetNewOrderID();
            string agentID = Request.Form["AgentID"];
            string paymentMethod = Request.Form["PaymentMethod"];
            decimal totalBill = Convert.ToDecimal(Request.Form["TotalBill"]);
            order.AddOrderQuery(newOrderID, agentID, "Processing Stock", "Awaiting Payment", paymentMethod, false, totalBill, DateTime.Now);

            // Get the acutal number of groups of product's information
            int formCount = Request.Form.Count / 8;
            for (int i = 0; i < formCount; i++)
            {
                string productID = Request.Form["ProductID[" + i + "]"];
                int productQuantity = Convert.ToInt32(Request.Form["ProductQuantity[" + i + "]"]);

                OrderDetail orderDetail = new OrderDetail();
                orderDetail.AddOrderDetailQuery(newOrderID, productID, productQuantity);
            }

            Session["CurrCartList"] = new List<Product>();
            Session["CartQuan"] = 0;

            // Send confirm message to email
            // Get Agent's email
            C_User agent = new C_User();
            string agentEmail = agent.GetAgentInfo("UserEmail", agentID);
            string agentName = agent.GetAgentInfo("UserName", agentID);

            MailMessage mail = new MailMessage();
            mail.To.Add("work.lethanhtien@gmail.com");
            mail.From = new MailAddress("lethanhtienhqv@gmail.com");
            mail.Subject = "Order Confirm Letter";
            string Body = "Dear " + agentName + ",<br />Thank you for your consideration to choose our service. We are grateful to say that your order is placed successfully and in the way to process.<br />Sincerely, Distributor";
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("lethanhtienhqv@gmail.com", "nhqpbctsxsoqfdxi"),
                EnableSsl = true
            };
            smtp.Send(mail);

            if (paymentMethod.Equals("VNPay"))
            {
                return RedirectToAction("CreatePayment", "Payment", new { amount = totalBill, orderId = newOrderID });
            }

            ViewBag.Message = "Place order successfully";
            return View("Result");
        }

        [HttpPost]
        public ActionResult ViewOrderByOrderID()
        {
            C_Order order = new C_Order();
            List<C_Order> orderList = order.SelectOrderByIDQuery(Request.Form["OrderID"]);
            ViewBag.OrderList = orderList;
            return View("ViewOrder");
        }
    }
}