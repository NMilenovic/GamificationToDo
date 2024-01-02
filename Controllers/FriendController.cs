using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Services;
using StackExchange.Redis;

namespace GamificationToDo.Controllers;

[ApiController]
[Route("[controller]")]
public class FriendController : ControllerBase
{

  private FriendService friendService;
  public FriendController()
  {
    friendService = new FriendService();
  }
  [HttpPost("SendFriendRequest/{username}/{friendUsername}")]
  public async Task<IActionResult> SendFriendRequest(string username, string friendUsername)
  {
    if(string.IsNullOrWhiteSpace(username))
      return BadRequest("Username is required.");
    if(string.IsNullOrWhiteSpace(friendUsername))
      return BadRequest("Friend username is required.");
    if(friendService.AreFriends(username,friendUsername).Result)
    {//Ovo rewrite kao exception u SendFriendRequest funkciji
      return BadRequest($"{username} and {friendUsername} are already friends");
    }
    try
    {
      await friendService.SendFriendRequest(username,friendUsername);
      return Ok($"{username} send friend request to {friendUsername}");
    }
    catch(Exception e)
    {
      return StatusCode(500,e.Message);
    }
  }
  [HttpDelete("AcceptFriendRequest/{username}/{friendUsername}")]
  public async Task<IActionResult> AcceptFriendRequest(string username,string friendUsername)
  {//Povecaj numOfFriends
    if(string.IsNullOrWhiteSpace(username))
      return BadRequest("Username is required.");
    if(string.IsNullOrWhiteSpace(friendUsername))
      return BadRequest("Friend username is required.");
    try
    {
      await friendService.AcceptFriendRequest(username,friendUsername);
      return Ok($"{username} accepted request of {friendUsername}");
    }
    catch(Exception e)
    {
      return StatusCode(500,e.Message);
    }
  }

  [HttpDelete("DeclineFriendRequest/{username}/{friendUsername}")]
  public async Task<IActionResult> DeclineFriendRequest(string username,string friendUsername)
  {
    if(string.IsNullOrWhiteSpace(username))
      return BadRequest("Username is required.");
    if(string.IsNullOrWhiteSpace(friendUsername))
      return BadRequest("Friend username is required.");
    
    try
    {
      await friendService.DeclineFriendRequest(username,friendUsername);
      return Ok($"{username} decliend friend request from {friendUsername}");
    }
    catch(Exception e)
    {
      return StatusCode(500,e.Message);
    }
  }

  [HttpDelete("RemoveFriend/{username}/{friendUsername}")]
  public async Task<IActionResult> RemoveFriend(string username,string friendUsername)
  {
    if(string.IsNullOrWhiteSpace(username))
      return BadRequest("Username is required.");
    if(string.IsNullOrWhiteSpace(friendUsername))
      return BadRequest("Friend username is required.");
    try 
    {
      await friendService.RemoveFriend(username,friendUsername);
      return Ok($"{username} removed {friendUsername} from friends.");
    }
    catch(Exception e)
    {
      return StatusCode(500,e.Message);
    }
  }

  [HttpGet("GetFriends/{username}")]
  public async Task<IActionResult> GetFriends(string username)
  {
    if(string.IsNullOrWhiteSpace(username))
      return BadRequest("Username is required.");
    try
    {
      var dtos = await friendService.GetFriends(username);
      return Ok(dtos);
    }
    catch(Exception e)
    {
      return StatusCode(500,e.Message);
    }
  }

  [HttpGet("GetFriendRequests/{username}")]
  public async Task<IActionResult> GetFriendRequests(string username)
  {
    if(string.IsNullOrWhiteSpace(username))
      return BadRequest("Username is required.");
    try
    {
      var dtos = await friendService.GetFriendRequests(username);
      return Ok(dtos);
    }
    catch(Exception e)
    {
      return StatusCode(500,e.Message);
    }
  }

  [HttpGet("SearchNewFriends/{username}/{pattern}")]
  public async Task<IActionResult> SearchNewFriends(string username,string pattern)
  {
     if(string.IsNullOrWhiteSpace(pattern))
      return BadRequest("Patern is required.");
    try
    {
        var users = friendService.SearchNewFriends(username,pattern);
        return Ok(users.Result);
    }
    catch(Exception e)
    {
      return StatusCode(500,e.Message);
    }
  }
  
}