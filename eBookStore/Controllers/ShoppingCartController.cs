using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Models.ViewModels;
using Microsoft.Data.SqlClient;
using eBookStore.Repository;
using System.Text;
namespace eBookStore.Controllers;


public class ShoppingCartController : Controller
{
	private readonly ILogger<ShoppingCartController> _logger;
	private readonly IConfiguration _configuration;
	private string? connectionString;
	private ShoppingCartRepository shoppingCartRepo;

	public ShoppingCartController(IConfiguration configuration, ILogger<ShoppingCartController> logger)
	{
		_configuration = configuration;
		connectionString = _configuration.GetConnectionString("DefaultConnection");
		_logger = logger;
		shoppingCartRepo = new ShoppingCartRepository(connectionString);
	}

	//this method is used to show the shopping cart
    public IActionResult ShowShoppingCart()
	{
		try
		{
			int? nullableUserId = HttpContext.Session.GetInt32("userId");
			if (nullableUserId.HasValue)
			{
				var cart = shoppingCartRepo.GetShoppingCart(nullableUserId.Value);
				if (cart == null || cart.shoppingCart == null)
				{
					cart = new ShoppingCartViewModel { shoppingCart = new ShoppingCartModel() };
				}
				return View("ShowShoppingCart", cart);
			}
			else
			{
				return RedirectToAction("Showlogin","Home");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to load shopping cart");
			return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}

	//this method is used to add a book to the shopping cart
	public IActionResult AddToShoppingCart(int? userId , int? bookId, string? format)
	{
		try
		{
			int? nullableUserId = HttpContext.Session.GetInt32("userId");
			if (nullableUserId.HasValue)
			{
				//shoppingCartRepo.AddToShoppingCart(nullableUserId.Value, bookId, format);
				shoppingCartRepo.AddToShoppingCart(nullableUserId.Value, 2, "PDF");
				return View("ShowShoppingCart", shoppingCartRepo.GetShoppingCart(nullableUserId.Value));
			}
			else
			{
				return RedirectToAction("Home/showLogIn");
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
				shoppingCartRepo.RemoveOneFromShoppingCart(nullableUserId.Value, 4);
				return View("ShowShoppingCart", shoppingCartRepo.GetShoppingCart(nullableUserId.Value));
			}
			else
			{
				return RedirectToAction("Home/showLogIn");
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
				shoppingCartRepo.RemoveAllFromShoppingCart(nullableUserId.Value);
				return View("ShowShoppingCart", shoppingCartRepo.GetShoppingCart(nullableUserId.Value));
			}else
			{
				return RedirectToAction("Home/showLogIn");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to clear shopping cart");
			return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}