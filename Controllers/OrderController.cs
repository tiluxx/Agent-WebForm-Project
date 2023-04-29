using Agent_WebForm_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (Session["AgentID"] == null || Session["AgentID"].Equals(""))
            {
                return View("Index");
            }
            C_Order order = new C_Order();
            List<C_Order> orderList = order.SelectOrderQuery();
            ViewBag.OrderList = orderList;
            return View();
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

            ViewBag.Message = "Create warehouse receipt successfully";
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