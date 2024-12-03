using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using Microsoft.Data.SqlClient;

namespace eBookStore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    private string? connectionString;
    public HomeController(IConfiguration configuration ,ILogger<HomeController> logger )
    {
        _configuration = configuration;
        connectionString = _configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }
    
    
    [HttpGet]
    public IActionResult register()
    {
        return View();
    }
    [HttpPost]
    public IActionResult register(UserModel user)
    {
        if(ModelState.IsValid)
        {
           using(SqlConnection  connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string queries = "INSERT INTO Users (Username, Password, Email, FirstName, LastName, PhoneNumber)  VALUES (@Username, @Password,@Email,@FirstName,@LastName,@PhoneNumber);";
                using(SqlCommand command = new SqlCommand(queries , connection))
                {
                    command.Parameters.AddWithValue("@Username",user.username);
                    command.Parameters.AddWithValue("@Password",user.password);
                    command.Parameters.AddWithValue("@Email",user.email);
                    command.Parameters.AddWithValue("@FirstName",user.firstName);
                    command.Parameters.AddWithValue("@LastName",user.lastName);
                    command.Parameters.AddWithValue("@PhoneNumber",user.phoneNumber);

                    int rowAffect = command.ExecuteNonQuery();
                }
            }

            return View("showRegister" , user);
        }
        return View(user);
    }

    public IActionResult showLogin(UserModel user)
    {
        if(ModelState.IsValid)
            return View(user);
        return View("LogIn");
    }

    [HttpGet]
    public IActionResult LogIn(UserModel user)
    {
        
        if(ModelState.IsValid)
        {
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT username, password, email, firstName, lastName, phoneNumber  FROM Users Where username = @username ;";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                           
                            string? usernameTest = reader["username"].ToString();
                            string? passwordTest = reader["password"].ToString();
                            
                                
                            if(user.username == usernameTest && user.password == passwordTest)
                            {
                                
                                UserModel foundUser = new UserModel
                                {
                                    username = reader["username"].ToString(),
                                    password = reader["password"].ToString(),
                                    email = reader["email"].ToString(),
                                    firstName = reader["firstName"].ToString(),
                                    lastName = reader["lastName"].ToString(),
                                    phoneNumber = reader["phoneNumber"].ToString()
                                };

                                
                                return View("showLogIn", foundUser);
                            }
                            
                        }
                    }
                }
            }
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
