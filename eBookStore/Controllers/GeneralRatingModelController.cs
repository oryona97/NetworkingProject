using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Repository;
using eBookStore.Models;
using eBookStore.Models.ViewModels;

namespace eBookStore.Controllers;

public class GeneralRatingModelController : Controller
{
    private readonly ILogger<GeneralRatingModelController> _logger;
    private readonly IConfiguration _configuration;
    private string? _connectionString;
    public GeneralRatingRepository _genraleRatingRepository;

    public GeneralRatingModelController(ILogger<GeneralRatingModelController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
        _genraleRatingRepository = new GeneralRatingRepository(_connectionString);
    }

    // Create instance of GeneralRatingModel and initialize all the models in it with values with the func in the model
    private GeneralRatingModel generalRatingModelTest = new GeneralRatingModel
    {
        starRating = 5,
        userId = 3,
        createdAt = DateTime.Now
    };

    // This function will add the GeneralRating to the database
    public IActionResult AddGeneralRating(GeneralRatingModel generalRatingModel)
    {
        //get user id from the session
    
        int? userId = HttpContext.Session.GetInt32("userId");
        if(userId == null)
        {
            return RedirectToAction("ShowLogIn", "Home");
        }
       
        if(!_genraleRatingRepository.CheckIfUserRated(userId.Value))
        {
            Console.WriteLine("Adding General Rating");
            _genraleRatingRepository.AddGeneralRating(generalRatingModel);
            return Ok("Rating Added");
        }
        else
        {
            return BadRequest("User already rated");
        }
        
    }

    //this func return average rating of the general rating of the website
    public IActionResult GetAverageRating()
    {
        var averageRating = _genraleRatingRepository.GetAverageRating();
        Console.WriteLine("Getting average rating {0}", averageRating);
        return Ok(averageRating);
    }

    //this func will get all the general ratings from the database and show them in web page
    public IActionResult GetAllGeneralRating()
    {
        Console.WriteLine("Getting all General Rating");
        List<GeneralRatingModel> generalRatingModels = _genraleRatingRepository.GetAllGeneralRating();
        return Ok(generalRatingModels);
    }

    


}