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
    private readonly ShoppingCartRepository _shoppingCartRepo;
    private readonly string _connectionString;
    private readonly UserRepository _userRepo;
    private readonly string _storeName;
    private readonly string _supportEmail;
    private readonly ILogger<UserRepository> _userRepoLogger;

    public CheckoutController(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _connectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        _storeName = _configuration["Store:Name"] ?? "eBookStore";
        _supportEmail = _configuration["Store:SupportEmail"] ?? "support@ebookstore.com";

        var stripeKey = _configuration["Stripe:SecretKey"]
            ?? throw new InvalidOperationException("Stripe secret key not found in configuration.");
        StripeConfiguration.ApiKey = stripeKey;

        _userRepoLogger = loggerFactory.CreateLogger<UserRepository>();

        _shoppingCartRepo = new ShoppingCartRepository(_connectionString);
        _userRepo = new UserRepository(_connectionString, _userRepoLogger);
    }

    private EmailTemplateGenerator CreateEmailGenerator()
    {
        string storeUrl = $"{Request.Scheme}://{Request.Host}";
        return new EmailTemplateGenerator(_storeName, storeUrl, _supportEmail);
    }

    [HttpPost]
    [Route("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CartData cartData)
    {
        if (cartData?.Items == null || !cartData.Items.Any())
        {
            return BadRequest("Cart is empty");
        }

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
                        UnitAmount = item.Amount,
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
            return RedirectToAction("Login", "Auth");
        }

        var cartJson = HttpContext.Session.GetString("cart");
        if (string.IsNullOrEmpty(cartJson))
        {
            return RedirectToAction("landingPage", "Home");
        }

        var cart = JsonSerializer.Deserialize<CartData>(cartJson);
        if (cart?.Items == null || !cart.Items.Any())
        {
            return RedirectToAction("landingPage", "Home");
        }

        var bookRepo = new BookRepository(_connectionString);
        var receiptRepo = new ReceiptRepository(_connectionString);
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var repoLogger = loggerFactory.CreateLogger<PersonalLibraryRepository>();
        var libRepo = new PersonalLibraryRepository(_connectionString, repoLogger);

        var user = bookRepo.getUserModelById(currentUserId.Value);
        if (user == null)
        {
            return BadRequest("User not found");
        }
        if (string.IsNullOrEmpty(user.email))
        {
            return BadRequest("User email not found");
        }

        int? id = null;
        decimal total = 0;
        try
        {
            foreach (var item in cart.Items)
            {
                try
                {
                    if (item.PurchaseType.ToLower() == "borrow")
                    {
                        await libRepo.BorrowBookAsync(currentUserId.Value, item.id);
                    }
                    else
                    {
                        await libRepo.AddBookToLibrary(currentUserId.Value, item.id);
                    }

                    _shoppingCartRepo.RemoveOneFromShoppingCart(currentUserId.Value, item.id);
                    repoLogger.LogInformation($"Successfully processed book {item.id} for user {currentUserId.Value}");

                    var receiptModel = new RecieptModel
                    {
                        bookId = item.id,
                        userId = currentUserId.Value,
                        total = item.Amount / 100f,
                        createdAt = DateTime.UtcNow
                    };
                    id = receiptModel.id;

                    var addedReceipt = await receiptRepo.AddAsync(receiptModel);
                    if (addedReceipt == null || addedReceipt.id == 0)
                    {
                        throw new Exception($"Failed to create receipt for book {item.id}");
                    }

                    total += item.Amount / 100m;
                }
                catch (Exception ex)
                {
                    repoLogger.LogError($"Error processing item {item.id}: {ex.Message}");
                    throw;
                }
            }

            string orderId = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{id}";

            string customerName = user.firstName ?? user.email.Split('@')[0];

            var emailGenerator = CreateEmailGenerator();
            string emailContent = emailGenerator.GenerateOrderConfirmationEmail(
                customerName,
                cart.Items,
                orderId,
                cart.Items.First().Currency.ToUpper()
            );

            _userRepo.SendEmail(
                user.email,
                $"{_storeName} - Order Confirmation #{orderId}",
                emailContent
            );

            HttpContext.Session.Remove("cart");

            ViewBag.OrderId = orderId;
            return View();
        }
        catch (Exception ex)
        {
            repoLogger.LogError($"Error processing order: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your order");
        }
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
    public string PurchaseType { get; set; } = "buy";
}
