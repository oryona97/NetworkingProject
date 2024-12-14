using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using Microsoft.Data.SqlClient;

namespace eBookStore.Controllers;
//this calss represent user
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
    
    //register
    [HttpGet]
    public IActionResult register()
    {
        return View();
    }
    [HttpPost]
    public IActionResult register(UserModel user)
    {
        try
        {
            if(ModelState.IsValid)
            {
            using(SqlConnection  connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connection successful!");
                    //this query insert the detais of user
                    string queries = "INSERT INTO [User] (Username, Password, Email, FirstName, LastName, PhoneNumber)  VALUES (@Username, @Password,@Email,@FirstName,@LastName,@PhoneNumber);";
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
                    connection.Close();
                }
                // show the details to the user
                return View("showRegister" , user);
            }
        }catch(SqlException ex)
        {
            Console.WriteLine($"Connection failed: {ex.Message}");
            return View(user);
        }
            //if not valid get the details agian 
            return View(user);
    }
    //**********************************************************************//

    [HttpGet]
    //login
    public IActionResult showLogIn()
    {
        Console.WriteLine("showlogin");
        UserModel user =new UserModel();
        return View("LogIn",user);        
    }
    
    [HttpPost]
    public IActionResult LogIn()
    {
        UserModel user =new UserModel();
        //get the userName and password from the form
        string? userName = Request.Form["userName"].ToString();
        string? password = Request.Form["password"].ToString();
        Console.WriteLine("login");
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            Console.WriteLine("Connection successful!");
            //this quary 
            string query = "SELECT * FROM [User] WHERE Username = @username;";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                
                command.Parameters.AddWithValue("@username",userName);
                Console.WriteLine(userName);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        if(userName == reader["Username"].ToString() && password == reader["Password"].ToString())
                        {
                            //init the user details
                            user.createAt = DateTime.Parse(reader["createAd"].ToString());
                            user.id =int.Parse(reader["id"].ToString());
                            user.username = reader["Username"].ToString();
                            user.password = reader["password"].ToString();
                            user.email = reader["Email"].ToString();
                            user.firstName = reader["firstName"].ToString();
                            user.lastName = reader["lastName"].ToString();
                            user.phoneNumber = reader["phoneNumber"].ToString();
                            //show to user 
                            Console.WriteLine("login success");
                            return View("showLogIn",user);
                        }
                    }
                        
                }
            }
            connection.Close();
            
        }
        Console.WriteLine("login failed");
        //if not valid get again
        return View(user);
    }
    //**********************************************************************//



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
