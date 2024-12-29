using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Repository;
using eBookStore.Models;
using eBookStore.Models.ViewModels;

namespace eBookStore.Controllers;

public class GeneralFeedbackModelController : Controller
{
    private readonly ILogger<GeneralFeedbackModelController> _logger;
    private readonly IConfiguration _configuration;
    private string? _connectionString;
    public GeneralFeedbackModelRepository _generalFeedbackRepository;

    public GeneralFeedbackModelController(ILogger<GeneralFeedbackModelController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
        _generalFeedbackRepository = new GeneralFeedbackModelRepository(_connectionString);
    }

    // Create instance of GeneralFeedbackModel and initialize all the models in it with values with the func in the model
    private GeneralFeedbackModel generalFeedbackModelTest = new GeneralFeedbackModel
    {
        userId = 3,
        comment = "I love this web youre the best",
        createdAt = DateTime.Now
    };

    // This function will add the GeneralFeedback to the database
   
    public IActionResult AddGeneralFeedback(GeneralFeedbackModel generalFeedbackModel)
    {
        if (ModelState.IsValid)
        {   
            Console.WriteLine("Adding General Feedback");
            _generalFeedbackRepository.AddGeneralFeedback(generalFeedbackModel);
            return Ok("Feedback Added");
        }
        return BadRequest("Invalid Feedback Data");
    }

    // This function will get all the general feedbacks from the database and show them in web page
   
    public IActionResult GetAllGeneralFeedback()
    {
        Console.WriteLine("Getting all General Feedback");
        List<GeneralFeedbackModel> generalFeedbackModels = _generalFeedbackRepository.GetAllGeneralFeedback();
        return Ok(generalFeedbackModels);
    }
}