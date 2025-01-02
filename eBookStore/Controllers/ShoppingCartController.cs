using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Models.ViewModels;
using eBookStore.Repository;
namespace eBookStore.Controllers;


public class ShoppingCartController : Controller
{
	private readonly ILogger<ShoppingCartController> _logger;
	private readonly IConfiguration _configuration;
	private string? connectionString;
	private ShoppingCartRepository _shoppingCartRepo;

	public ShoppingCartController(IConfiguration configuration, ILogger<ShoppingCartController> logger)
	{
		_configuration = configuration;
		connectionString = _configuration.GetConnectionString("DefaultConnection");
		_logger = logger;
		_shoppingCartRepo = new ShoppingCartRepository(connectionString);
	}

	[Route("cart")]
	[Route("ShoppingCart")]
	//this method is used to show the shopping cart
	public IActionResult Index()
	{
		try
		{
			int? nullableUserId = HttpContext.Session.GetInt32("userId");
			if (nullableUserId.HasValue)
			{
				var cart = _shoppingCartRepo.GetShoppingCart(nullableUserId.Value);
				if (cart == null || cart.shoppingCart == null)
				{
					cart = new ShoppingCartViewModel { shoppingCart = new ShoppingCartModel() };
				}
				return View("Index", cart);
			}
			else
			{
				return RedirectToAction("Login", "Auth");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to load shopping cart");
			return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}

	//this method is used to add a book to the shopping cart
	public IActionResult AddToShoppingCart(int? userId, int? bookId, string? format)
	{
		try
		{
			int? nullableUserId = HttpContext.Session.GetInt32("userId");
			if (nullableUserId.HasValue)
			{
				//shoppingCartRepo.AddToShoppingCart(nullableUserId.Value, bookId, format);
				_shoppingCartRepo.AddToShoppingCart(nullableUserId.Value, 2, "PDF");
				return View("ShowShoppingCart", _shoppingCartRepo.GetShoppingCart(nullableUserId.Value));
			}
			else
			{
				return RedirectToAction("auth/login");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to add to shopping cart");
			return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}

	//this method is used to remove a book from the shopping cart
	public IActionResult RemoveFromShoppingCart(int? userId, int? bookId)
	{
		try
		{
			int? nullableUserId = HttpContext.Session.GetInt32("userId");
			if (nullableUserId.HasValue)
			{

				//this line is for production
				//shoppingCartRepo.RemoveOneFromShoppingCart(nullableUserId.Value, bookId);

				//this line is for testing
				_shoppingCartRepo.RemoveOneFromShoppingCart(nullableUserId.Value, 4);
				return View("ShowShoppingCart", _shoppingCartRepo.GetShoppingCart(nullableUserId.Value));
			}
			else
			{
				return RedirectToAction("auth/login");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to remove from shopping cart");
			return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}

	//this method is used to clear the shopping cart
	public IActionResult ClearShoppingCart(int? userId)
	{
		try
		{
			int? nullableUserId = HttpContext.Session.GetInt32("userId");
			if (nullableUserId.HasValue)
			{
				_shoppingCartRepo.RemoveAllFromShoppingCart(nullableUserId.Value);
				return View("ShowShoppingCart", _shoppingCartRepo.GetShoppingCart(nullableUserId.Value));
			}
			else
			{
				return RedirectToAction("auth/login");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to clear shopping cart");
			return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}

