using EducationalServices.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace EducationalServices.Controllers
{
    public class PayPalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult CreatePayment(double CartTotal, int? modID)
        {
            if(modID!= null)
            {
                Session["ModID"] = modID.ToString();
            }
            else
            {
                Session["ModID"] = "0";
            }
                
                var CurrentUser = User.Identity.Name;
                double convertedTot = Math.Round(CartTotal / 14.357);
                int Rem = (int)(CartTotal % 14.357);
                string Cost = convertedTot.ToString() + "." + Rem;

                // Set up the PayPal API context
                var apiContext = PayPalConfig.GetAPIContext();

                // Retrieve the API credentials from configuration
                var clientId = ConfigurationManager.AppSettings["PayPalClientId"];
                var clientSecret = ConfigurationManager.AppSettings["PayPalClientSecret"];
                apiContext.Config = new Dictionary<string, string> { { "mode", "sandbox" } };
                var accessToken = new OAuthTokenCredential(clientId, clientSecret, apiContext.Config).GetAccessToken();
                apiContext.AccessToken = accessToken;

                // Create a new payment object
                var payment = new Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                    transactions = new List<Transaction>
                {
            new Transaction
            {
                amount = new Amount
                {

                    total = Cost,
                    currency = "USD"
                },

                description = "Medicine Payment"
            }
        },
                    redirect_urls = new RedirectUrls
                    {
                        return_url = Url.Action("CompletePayment", "PayPal", null, Request.Url.Scheme),
                        cancel_url = Url.Action("CancelPayment", "PayPal", null, Request.Url.Scheme)
                    }
                };

                // Create the payment and get the approval URL
                var createdPayment = payment.Create(apiContext);
                var approvalUrl = createdPayment.links.FirstOrDefault(l => l.rel == "approval_url")?.href;

                // Redirect the user to the PayPal approval URL
                return Redirect(approvalUrl);
            
        }


        public ActionResult CompletePayment(string paymentId, string token, string PayerID)
        {
            // Set up the PayPal API context
            var apiContext = PayPalConfig.GetAPIContext();

            // Execute the payment
            var paymentExecution = new PaymentExecution { payer_id = PayerID };
            var executedPayment = new Payment { id = paymentId }.Execute(apiContext, paymentExecution);

            // Process the payment completion
            // You can save the transaction details or perform other necessary actions

            // Redirect the user to a success page
            return RedirectToAction("PaymentSuccess", new { PayID  = paymentId});
        }

        public ActionResult CancelPayment()
        {
            // Handle the payment cancellation
            // You can redirect the user to a cancellation page or perform other necessary actions

            // Redirect the user to a cancellation page
            return RedirectToAction("PaymentCancelled");
        }

        public ActionResult PaymentSuccess(string PayID)
        {
            string modID;
            if (Session["ModID"] != null)
            {
                if (Session["ModID"] != "0")
                {
                    modID = Session["ModID"] as string;
                    int ModuleId = int.Parse(modID);
                    var Modules = db.Modules.Find(ModuleId);

                    StudentModule studentModule = new StudentModule();
                    studentModule.ModuleId = ModuleId;
                    studentModule.StudentId = User.Identity.GetUserId();
                    studentModule.Status = "Paid";
                    studentModule.TotalPaid = Modules.Price;
                    db.StudentModules.Add(studentModule);
                    db.SaveChanges();

                    TempData["Success"] = "Your Order Has Place. Thank You For Your Time!";
                    return RedirectToAction("RegisteredStudent", "Modules");
                }
                else
                {
                    var cart = Session["cart"] as List<Cart> ?? new List<Cart>();
                    var userdata = db.CustInfos.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                    
                    if (cart != null)
                    {

                        Meths meth = new Meths();
                        int UniqueCode = Meths.GenerateRandomCode();
                        bool conflict = db.Orders.Where(x => x.Code == UniqueCode).Any();
                        while (conflict)
                        {
                            UniqueCode = Meths.GenerateRandomCode();
                            conflict = db.Orders.Where(x => x.Code == UniqueCode).Any();
                        }


                        OrderDetails orderDetails = new OrderDetails();
                        Orders orders = new Orders();
                        decimal totalAmount = 0;
                        orders.OrderDate = DateTime.Now;
                        orders.UserId = User.Identity.GetUserId();
                        orders.DueDate = userdata.deliveryDate;
                        orders.Status = "Placed";
                        orders.Address = userdata.Address;
                        orders.Code = UniqueCode;
                        orders.Email = User.Identity.Name;
                        orders.PaymentID = PayID;
                        orders.DeliveryFee = (decimal)userdata.deliveryFee;
                        foreach (var item in cart)
                        {
                            totalAmount += item.Price * item.StockQuantity;
                            
                        }
                        totalAmount += (totalAmount * 15 / 100);
                        orders.ProductCost = totalAmount;
                        orders.TotalAmount = totalAmount + (decimal)userdata.deliveryFee;
                        

                        db.Orders.Add(orders);
                        //db.SaveChanges(); Delete if email fails 

                        foreach (var item in cart)
                        {
                            var Product = db.Products.Find(item.ProductId);

                            orderDetails.OrderId = orders.OrderId;
                            orderDetails.ProductId = item.ProductId;
                            orderDetails.Name = item.Name;
                            orderDetails.Description = item.Description;
                            orderDetails.Price = item.Price;
                            orderDetails.StockQuantity = item.StockQuantity;
                            orderDetails.Weight = item.Weight;
                            orderDetails.Picture = item.Picture;
                            
                            Product.StockQuantity -= item.StockQuantity;

                            db.OrderDetails.Add(orderDetails);

                        }

                        try
                        {

                            var email2 = new MailMessage();
                            email2.From = new MailAddress("Poultry.dbn@outlook.com");
                            email2.To.Add(User.Identity.Name);
                            email2.Subject = "Payment Confirmation |  " + orders.OrderId;
                            string emailBody = $"Order Number: " + orders.OrderId + "\t\t Estimated Delivery Date: " + userdata.deliveryDate.ToShortDateString() + " \n\n" +
                           $"Hi {userdata.Name}, \n\n" +
                           $"Thank you, we’ve received your payment for order {orders.OrderId}\n\n" +
                           $"Your order is not out for delivery yet.> We'll now begin to process your order for delivery and it should be ready from {userdata.deliveryDate.ToLongDateString()}.\n\n" +
                           $"We'll email you the moment it's ready." +
                           $"Regards,\r\nEducational Services";
                            email2.Body = emailBody;


                            var smtpClient = new SmtpClient();

                            smtpClient.Send(email2);

                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = "Failed to send email due to, " + ex.Message;
                            return RedirectToAction("Index", "ShoppingCart");
                        }

                        db.SaveChanges();
                        Session["cart"] = null;
                        Session["ModID"] = null;
                        TempData["Success"] = "Your Order Has Been Placed. Thank You For Your Time!";
                        return RedirectToAction("Orders", "Products");
                    }

                    return RedirectToAction("ViewCart", "Products");
                }
                
            }
            else
            {
                TempData["Error"] = "Your Session Has Ended Please Try Again!!!";
                return RedirectToAction("Index", "Home");
            }




            
        }

    }
}