using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace GamificationToDo.Controllers;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{

  private PostService postService;

  public PostController()
  {
    postService = new PostService();
  }
  [HttpGet("GetPosts/{username}")]
  public IActionResult GetPosts(string username)
  {
    if (string.IsNullOrEmpty(username))
    {
      return BadRequest("Username is required.");
    }
    try
    {
      var posts = postService.GetPosts(username);
      return Ok(posts.Result);
    }
    catch(Exception e)
    {
      return StatusCode(500,e.Message);
      
    }
  }

  [HttpGet("GetFeed/{username}")]
  public IActionResult GetFeed(string username)
  {
    if (string.IsNullOrEmpty(username))
    {
      return BadRequest("Username is required.");
    }
    try
    {
      return Ok(postService.GetFeed(username));
    }
    catch(Exception e)
    {
      return StatusCode(500,e.Message);
    }
  }
}