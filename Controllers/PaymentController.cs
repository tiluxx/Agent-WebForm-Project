using Agent_WebForm_Project.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agent_WebForm_Project.Controllers
{
    public class PaymentController : Controller
    {
        public ActionResult CreatePayment(decimal amount, string orderId)
        {
            string vnpayUrl = ConfigurationManager.AppSettings["vnp_Url"];
            string vnpayReturnUrl = ConfigurationManager.AppSettings["vnp_Returnurl"];
            string vnpayTmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnpayHashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

            VnPayLibrary pay = new VnPayLibrary();

            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", vnpayTmnCode);
            pay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());
            pay.AddRequestData("vnp_BankCode", "");
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); 
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            pay.AddRequestData("vnp_Locale", "en"); 
            pay.AddRequestData("vnp_OrderInfo", orderId);
            pay.AddRequestData("vnp_OrderType", "other"); 
            pay.AddRequestData("vnp_ReturnUrl", vnpayReturnUrl); 
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); 

            string paymentUrl = pay.CreateRequestUrl(vnpayUrl, vnpayHashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult ConfirmPayment()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; 
                var vnpayData = Request.QueryString;
                VnPayLibrary pay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                long payDate = Convert.ToInt64(pay.GetResponseData("vnp_PayDate"));
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string orderInfo = pay.GetResponseData("vnp_OrderInfo"); 
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        // Payment successfully
                        string message = "Payment successfully for bill " + orderId + " | Transaction No.: " + vnpayTranId;
                        return RedirectToAction("VnPaymentResponse", "Order", new { status = true, message, orderId = orderInfo, transactionNo = vnpayTranId, payDate });
                    }
                    else
                    {
                        // Fail payment. Error code: vnp_ResponseCode
                        string message = "There is error in the payment process for bill " + orderId + " | Transaction No.: " + vnpayTranId + " | Error code: " + vnp_ResponseCode;
                        return RedirectToAction("VnPaymentResponse", "Order", new { status = false, message, transactionNo = vnpayTranId });
                    }
                }
                else
                {
                    string message = "There is error in the payment process";
                    return RedirectToAction("VnPaymentResponse", "Order", new { status = false, message });
                }
            }

            return View();
        }
    }
}