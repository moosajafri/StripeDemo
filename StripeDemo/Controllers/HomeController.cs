using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StripeDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Charge()
        {

            try
            {

                /*When form is submitted, we can either get the customer email along with other details 
                 * or get the email from logged in user in case a logins are maintaned*/

                StripeConfiguration.SetApiKey("sk_test_22k4YCpi8ZXIPNrbXknO9FBJ");


                //check if customer already exists
                var customerServiceObj = new CustomerService();
                var _options = new CustomerListOptions()
                {
                    Limit = 100
                };

                StripeList<Customer> allCustomers = customerServiceObj.List(_options);

                var exisitngCustomer = allCustomers.Where(x => x.Email == "moosa@example.com").FirstOrDefault();

                Customer customer = null;
                if (exisitngCustomer != null)
                {
                    customer = exisitngCustomer;
                }
                else
                {
                    var createCustomerOptions = new CustomerCreateOptions
                    {
                        Description = "Customer for moosa@example.com",
                        SourceToken = "tok_amex",
                        Email = "moosa@example.com"
                    };
                    customer = customerServiceObj.Create(createCustomerOptions);
                }

                // Set your secret key: remember to change this to your live secret key in production
                // See your keys here: https://dashboard.stripe.com/account/apikeys
                var apiKey = "sk_test_22k4YCpi8ZXIPNrbXknO9FBJ";
                var planId = "plan_ECiANlC1f0LmJ4";
                StripeConfiguration.SetApiKey(apiKey);

                // Token is created using Checkout or Elements!
                // Get the payment token submitted by the form:
                //and charge a one time fee, then add the customer for monthly subscription
                var token = Request["stripeToken"]; // Using ASP.NET MVC

                var options = new ChargeCreateOptions
                {
                    //this is equivalent to 75.00
                    Amount = 7500,
                    Currency = "usd",
                    Description = "Example charge",
                    SourceId = token,
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);


                //add customer to subscription here
                var subscription = MonthlySubscription(apiKey, planId, customer.Id);
            }
            catch (Exception)
            {
                //do something with the exception here
                throw;
            }


            return View("Success");
        }

        [HttpPost]
        public ActionResult MonthlySubscription()
        {
            try
            {
                // Set your secret key: remember to change this to your live secret key in production
                // See your keys here: https://dashboard.stripe.com/account/apikeys
                StripeConfiguration.SetApiKey("sk_test_22k4YCpi8ZXIPNrbXknO9FBJ");

                var items = new List<SubscriptionItemOption> {
                new SubscriptionItemOption {PlanId = "plan_ECiANlC1f0LmJ4"}
            };
                var options = new SubscriptionCreateOptions
                {
                    CustomerId = "cus_ECiBuW7SSWWOOD",
                    Items = items,
                };
                var service = new SubscriptionService();
                Subscription subscription = service.Create(options);

            }
            catch (Exception e)
            {
                //do something with the exception here
                throw;
            }

            return View("Success");
        }

        private Subscription MonthlySubscription(string apiKey, string planId, string customerId)
        {
            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.SetApiKey(apiKey);

            var items = new List<SubscriptionItemOption> {
                new SubscriptionItemOption {PlanId = planId}
            };
            var options = new SubscriptionCreateOptions
            {
                CustomerId = customerId,
                Items = items,
            };
            var service = new SubscriptionService();
            return service.Create(options);

        }
    }

}