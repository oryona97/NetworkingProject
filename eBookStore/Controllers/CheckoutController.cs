using Microsoft.AspNetCore.Mvc;
using eBookStore.Repository;
using eBookStore.Models;
using Stripe;
using System.Text;
using Stripe.Checkout;
using System.Text.Json;

[Route("[controller]")]
[ApiController]
public class CheckoutController : Controller
{
    private readonly IConfiguration _configuration;
    private CartData? _books;
    private ShoppingCartRepository _shoppingCartRepo;
    private string? _connectionString;
    private UserRepository _userRepo;

    public CheckoutController(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
        _shoppingCartRepo = new ShoppingCartRepository(_connectionString!);
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

        // Create a logger for UserRepository using ILoggerFactory
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var userRepoLogger = loggerFactory.CreateLogger<UserRepository>();

        _userRepo = new UserRepository(_connectionString, userRepoLogger);
    }

    [HttpPost]
    [Route("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CartData cartData)
    {
        HttpContext.Session.SetString("cart", JsonSerializer.Serialize(cartData));
        try
        {
            var lineItems = new List<SessionLineItemOptions>();
            _books = cartData;
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
    public async Task<IActionResult> Success()
    {
        var currentUserId = HttpContext.Session.GetInt32("userId");
        if (currentUserId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var cartJson = HttpContext.Session.GetString("cart");
        var cart = JsonSerializer.Deserialize<CartData>(cartJson);
        if (cart.Items == null) return RedirectToAction("landingPage", "Home");

        var bookRepo = new BookRepository(_connectionString);
        var receipt = new List<RecieptModel>();
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var repoLogger = loggerFactory.CreateLogger<PersonalLibraryRepository>();
        var libRepo = new PersonalLibraryRepository(_connectionString, repoLogger);

        // Get user details including email
        var user = bookRepo.getUserModelById(currentUserId.Value);
        if (user == null)
        {
            return BadRequest("User not found");
        }

        // Build email body with purchase details
        var emailBody = new StringBuilder();
        emailBody.AppendLine("Thank you for your purchase at eBookStore!");
        emailBody.AppendLine("\nOrder Details:");
        emailBody.AppendLine("---------------");

        decimal total = 0;
        foreach (var item in cart.Items)
        {
            // Add to personal library and remove from cart
            libRepo.AddBookToLibrary(currentUserId.Value, item.id);
            _shoppingCartRepo.RemoveOneFromShoppingCart(currentUserId.Value, item.id);

            // Add to receipt list
            receipt.Add(new RecieptModel
            {
                bookId = item.id,
                userId = currentUserId.Value,
            });

            // Add item details to email
            emailBody.AppendLine($"\nBook: {item.ProductName}");
            emailBody.AppendLine($"Description: {item.ProductDescription}");
            emailBody.AppendLine($"Price: {(item.Amount / 100m).ToString("C")} {item.Currency}");

            total += item.Amount / 100m;
        }

        // Save receipt in the data base
        emailBody.AppendLine($"\nTotal Amount: {total.ToString("C")}");
        emailBody.AppendLine("\nYour books are now available in your personal library.");
        emailBody.AppendLine("\nThank you for shopping with us!");

        // Send email receipt
        try
        {
            _userRepo.SendEmail(
                  user.email,
                "Your eBookStore Purchase Receipt",
                  emailBody.ToString()
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send receipt email: {ex.Message}");
        }

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
    public int id { get; set; } = 0;
    public string ProductName { get; set; } = String.Empty;
    public string ProductDescription { get; set; } = String.Empty;
    public int Amount { get; set; } = 0;
    public string Currency { get; set; } = String.Empty;
}
