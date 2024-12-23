using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models.ViewModels;
using eBookStore.Models;
using Microsoft.Data.SqlClient;
using eBookStore.Repository;
using System.Text;
namespace eBookStore.Controllers;


public class PersonalLibraryController : Controller
{
	private readonly ILogger<PersonalLibraryController> _logger;
	private readonly IConfiguration _configuration;
	private string? connectionString;
	private PersonalLibraryRepository personalLibraryRepository;

	public PersonalLibraryController(IConfiguration configuration, ILogger<PersonalLibraryController> logger)
	{
		_configuration = configuration;
		connectionString = _configuration.GetConnectionString("DefaultConnection");
		_logger = logger;
		personalLibraryRepository = new PersonalLibraryRepository(connectionString);
	}

	public IActionResult getPersonalLibrary()
	{
		int? userId = HttpContext.Session.GetInt32("userId");

		if (!userId.HasValue)
		{
			return RedirectToAction("Login", "Account"); 
		}

		List<PersonalLibraryModel> personalLibraryList = personalLibraryRepository.GetPersonalLibrary(userId.Value);
		return View(personalLibraryList);
	}
    
}