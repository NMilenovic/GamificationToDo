using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace GamificationToDo.Controllers;

[ApiController]
[Route("[controller]")]
public class GoalController : ControllerBase
{
  
    private GoalService goalService;
    
    public GoalController()
    {
        goalService = new GoalService();
    }

    [HttpPost("AddGoal/{username}/{goalName}")]
    public async Task<IActionResult> AddGoal(string username, string goalName)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Username is required.");
        }
        if(string.IsNullOrEmpty(goalName))
        {
            return BadRequest("Goal name is required.");
        }
        try
        {
            await goalService.AddGoal(username,goalName);
            return Ok("Goal added");
        }
        catch(Exception e)
        {
             return StatusCode(500,e.Message);
        }
    }
   [HttpDelete("DeleteGoal/{username}/{goalName}")]
   public async Task<IActionResult> DeleteGoal(string username,string goalName)
   {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Username is required.");
        }
        if(string.IsNullOrEmpty(goalName))
        {
            return BadRequest("Goal name is required.");
        }
        try
        {
            await goalService.DeleteGoal(username,goalName);
            return Ok("Goal deleted");
        }
        catch(Exception e)
        {
            return StatusCode(500,e.Message);
        }
   }
   [HttpPut("UpdateGoal/{goalName}/{newGoalName}")]
   public IActionResult ChangeGoalName([FromBody]User user,string goalName,string newGoalName)
   {
        if (string.IsNullOrEmpty(user.Username))
        {
            return BadRequest("Username is required.");
        }
        if(string.IsNullOrEmpty(goalName))
        {
            return BadRequest("Goal name is required.");
        }
        if(string.IsNullOrEmpty(newGoalName))
        {
            return BadRequest("New goal name is required.");
        }
        if(goalName == newGoalName)
        {
            return BadRequest("New name must be different than old one.");
        }
        if(goalService.UpdateGoal(user,goalName,newGoalName).Result)
        {
            return Ok($"User {user.Username} updated goal {goalName} to {newGoalName}");
        }
        else
            return StatusCode(500,"Internal Server Error!");
   }

   [HttpDelete("CompleteGoal/{username}/{goalName}")]
   public async Task<IActionResult> CompleteGoal(string username,string goalName)
   {
    if (string.IsNullOrEmpty(username))
    {
      return BadRequest("Username is required.");
    }
    if(string.IsNullOrEmpty(goalName))
    {
      return BadRequest("Goal name is required.");
    }
    try
    {
        await goalService.CompleteGoal(username,goalName);
        return Ok($"{username} completed goal {goalName}");
    }
    catch(Exception e)
    {
        return StatusCode(500,e.Message);
    }
   }

   [HttpGet("GetGoals/{username}")]
   public IActionResult GetGoals(string username)
   {
    if (string.IsNullOrEmpty(username))
    {
      return BadRequest("Username is required.");
    }
    try
    {
        var goals = goalService.GetGoals(username);
        return Ok(goals.Result);
    }
    catch(Exception e)
    {
        return StatusCode(500,e.Message);
    }

   }
}
