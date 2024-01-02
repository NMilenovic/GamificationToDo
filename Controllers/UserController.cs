using Microsoft.AspNetCore.Mvc;
using Services;
namespace GamificationToDo.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
  
    UserService service;
    public UserController()
    {
        service = new UserService();
    }

    [HttpPost("register/{username}/{password}")]
    public IActionResult Register(string username, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Username and password are required.");
            }
            if(!service.IsUsernameTaken(username))
            {
                  return BadRequest("Username is already taken");
            }
            if(service.RegisterUser(username,password).Result)
            {
                 return Ok("User is registered");
            }
            return BadRequest("Greska pilikom registrovanja!");
           
        }
        catch(Exception ex)
        {
            return StatusCode(500,"Server error + "+ex);
        }
    }

    
    [HttpGet("login/{username}/{password}")]
    public IActionResult Login(string username,string password)
    {
        User user = new User{Username = username,Password = password};
        try
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Username and password are required.");
            }
            var pronadjenUser = service.FindAndReturnUser(user).Result;
            if(pronadjenUser != null)
            {
                return Ok(pronadjenUser);
            }
            return BadRequest("Korisnik sa datim korisnickim imenom/lozikom ne postoji!");

        }
        catch(Exception ex)
        {
            return StatusCode(500,"Error on endpoint Login"+ex);
        }
    }
    [HttpGet("GetUser/{username}")]
    public IActionResult GetUser(string username)
    {
        try
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username is required.");
            }
            var pronadjenUser = service.FindAndReturnUser(username).Result;
            return Ok(pronadjenUser);
        }
        catch(Exception ex)
        {
            return StatusCode(500,ex.Message);
        }
    }
    
}
