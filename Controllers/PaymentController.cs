using DiscreetKindnessApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiscreetKindnessApp.Controllers
{
    public class PaymentController : Controller
    {
        //[HttpPost]
        //public IActionResult Donate()
        //{
        //    var options = new SessionCreateOptions
        //    {
        //        PaymentMethodTypes = new List<string> { "card" },
        //        LineItems = new List<SessionLineItemOptions>
        //        {
        //            new SessionLineItemOptions
        //            {
        //                PriceData = new SessionLineItemPriceDataOptions
        //                {
        //                    UnitAmount = 200, // Amount in cents ($2.00)
        //                    Currency = "usd",
        //                    ProductData = new SessionLineItemPriceDataProductDataOptions
        //                    {
        //                        Name = "Support Donation",
        //                    },
        //                },
        //                Quantity = 1,
        //            },
        //        },
        //        Mode = "payment",
        //        SuccessUrl = Url.Action("Success", "Payment", null, Request.Scheme),
        //        CancelUrl = Url.Action("Cancel", "Payment", null, Request.Scheme),
        //    };

        //    var service = new SessionService();
        //    Session session = service.Create(options);

        //    Response.Headers.Add("Location", session.Url);
        //    return new StatusCodeResult(303);
        //}

        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public IActionResult Donate()
        {
            string successUrl = Url.Action("Success", "Payment", null, Request.Scheme);
            string cancelUrl = Url.Action("Cancel", "Payment", null, Request.Scheme);

            string sessionUrl = _paymentService.CreateCheckoutSession(2.00m, "usd", successUrl, cancelUrl);

            if (string.IsNullOrEmpty(sessionUrl))
            {
                return BadRequest("Failed to create Stripe session.");
            }

            Response.Headers.Add("Location", sessionUrl);
            return new StatusCodeResult(303);
        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}
