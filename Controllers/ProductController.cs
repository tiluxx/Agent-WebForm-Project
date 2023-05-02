﻿using Agent_WebForm_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agent_WebForm_Project.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            Product product = new Product();
            List<Product> res = product.SelectProductQuery();
            ViewBag.ProductList = res;
            return View();
        }

        // GET: Product/Cart
        public ActionResult Cart()
        {
            if (Session["CurrCartList"] != null)
            {
                List<Product> productList = (List<Product>)Session["CurrCartList"];
                ViewBag.ProductList = productList;
            }

            if (Session["AgentID"] != null)
            {
                AgentCart cart = new AgentCart();
                AgentCartDetail agentCartDetail = new AgentCartDetail();
                AgentCart agentCart = cart.SelectAgentCartQuery(Session["AgentID"].ToString());
                if (agentCart.CartID != null)
                {
                    List<Product> productList = agentCartDetail.SelectProductCartQuery(agentCart.CartID);
                    ViewBag.ProductList = productList;
                }
            }
            return View();
        }

        // GET: Product/OrderPreview
        public ActionResult OrderPreview()
        {
            List<Product> productList = new List<Product>();
            int formCount = Request.Form.Count / 8;
            decimal totalBill = 0;
            for (int i = 0; i < formCount; i++)
            {
                int productQuantity = Convert.ToInt32(Request.Form["ProductQuantity[" + i + "]"]);
                decimal productPrice = Convert.ToDecimal(Request.Form["ProductPrice[" + i + "]"]);
                totalBill += productPrice * productQuantity;

                Product product = new Product
                {
                    ProductID = Request.Form["ProductID[" + i + "]"],
                    ProductName = Request.Form["ProductName[" + i + "]"],
                    ProductSize = Request.Form["ProductSize[" + i + "]"],
                    ProductUnitSize = Request.Form["ProductUnitSize[" + i + "]"],
                    ProductBrand = Request.Form["ProductBrand[" + i + "]"],
                    ProductOrigin = Request.Form["ProductOrigin[" + i + "]"],
                    ProductQuantity = productQuantity,
                    ProductPrice = productPrice
                };
                productList.Add(product);
            }

            ViewBag.ProductList = productList;
            ViewBag.TotalBill = totalBill;
            return View();
        }


    }
}