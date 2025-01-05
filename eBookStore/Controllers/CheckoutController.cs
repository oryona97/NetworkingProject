using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

[Route("[controller]")]
[ApiController]
public class CheckoutController : Controller
{
    private readonly IConfiguration _configuration;

    public CheckoutController(IConfiguration configuration)
    {
        _configuration = configuration;
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
    }

    [HttpPost]
    [Route("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CartData cartData)
    {
        try
        {
            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in cartData.Items)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = item.Amount, // Amount in cents
                        Currency = item.Currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName,
                            Description = item.ProductDescription,
                        },
                    },
                    Quantity = 1,
                });
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return Json(new { sessionId = session.Id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet]
    [Route("success")]
    public IActionResult Success()
    {
        return View();
    }

    [HttpGet]
    [Route("cancel")]
    public IActionResult Cancel()
    {
        return View();
    }
}

public class CartData
{
    public List<CartItem> Items { get; set; } = new List<CartItem>();
}

public class CartItem
{
    public string ProductName { get; set; } = String.Empty;
    public string ProductDescription { get; set; } = String.Empty;
    public int Amount { get; set; } = 0;
    public string Currency { get; set; } = String.Empty;
}
