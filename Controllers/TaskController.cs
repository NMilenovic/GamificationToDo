using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace GamificationToDo.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskController : ControllerBase
{
  private TaskService taskService;

  public TaskController()
  {
    taskService = new TaskService();
  }

  [HttpPost("AddTask/{username}")]
  public IActionResult AddTask([FromBody]Taskk newTask,string username)
  {
    if (string.IsNullOrEmpty(username))
    {
      return BadRequest("Username is required.");
    }
    if (string.IsNullOrEmpty(newTask.GoalName))
    {
      return BadRequest("Goal name is required.");
    }
    if (string.IsNullOrEmpty(newTask.TaskName))
    {
      return BadRequest("Task name is required.");
    }
    if (newTask.XP < 0 || newTask.XP >= 1000)
    {
      return BadRequest("XP must be between 0 and 1000");
    }
    if(taskService.AddTask(newTask,username).Result)
    {
       return Ok($"User {username} added task {newTask.TaskName} in goal {newTask.GoalName}");
    }
    else
    {
      return StatusCode(500,"Internal Server Error.");
    }
  }

  [HttpDelete("DeleteTask/{username}")]
  public IActionResult DeleteTask([FromBody]Taskk delTask,string username)
  {
    if (string.IsNullOrEmpty(username))
    {
      return BadRequest("Username is required.");
    }
    if (string.IsNullOrEmpty(delTask.GoalName))
    {
      return BadRequest("Goal name is required.");
    }
    if (string.IsNullOrEmpty(delTask.TaskName))
    {
      return BadRequest("Task name is required.");
    }
    if(taskService.DeleteTask(delTask,username).Result)
    {
      return Ok($"User {username} deleted task {delTask.TaskName} from goal {delTask.GoalName}");
    }
    return StatusCode(500,"Internal Server Error.");
  }

  [HttpDelete("CompleteTask/{username}")]
  public IActionResult CompleteTask([FromBody]Taskk compTask,string username)
  {
    if (string.IsNullOrEmpty(username))
    {
      return BadRequest("Username is required.");
    }
    if (string.IsNullOrEmpty(compTask.GoalName))
    {
      return BadRequest("Goal name is required.");
    }
    if (string.IsNullOrEmpty(compTask.TaskName))
    {
      return BadRequest("Task name is required.");
    }
    if(taskService.CompleteTask(compTask,username).Result)
    {
      return Ok($"{username} completed a task.");
    }
    return StatusCode(500,"Internal Server Error!");
  }

  [HttpGet("GetTasksOfGoal/{username}/{goalName}")]
  public IActionResult GetTasksOfGoal(string username,string goalName)
  {
    if (string.IsNullOrEmpty(username))
    {
      return BadRequest("Username is required.");
    }
    if (string.IsNullOrEmpty(goalName))
    {
      return BadRequest("Goal name is required.");
    }
    try
    {
      var ret =  taskService.GetTasksOfGoal(username,goalName);
      return Ok(ret.Result);
    }
    catch(Exception e)
    {
      return StatusCode(500,e.Message);
    }
  }
}