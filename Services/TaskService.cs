using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Models;
using StackExchange.Redis;

namespace Services
{
  public class TaskService
  {
    private UserService userService;
    private User? currentUser;
    private RedisService redisService;
    private LeaderboardService leaderboardService;
    
    public TaskService()
    {
      redisService = new RedisService();
      leaderboardService = new LeaderboardService();
    }

    public async Task<bool> AddTask(Taskk newTask,string username)
    {
      HashEntry[] task = {
        new HashEntry("username",username),
        new HashEntry("goalName",newTask.GoalName),
        new HashEntry("taskName",newTask.TaskName),
        new HashEntry("xp",(double) newTask.XP)
      };
      try
      {
        var broj = await redisService.db.ListPositionsAsync("users",username,1);
        if(broj.Length != 1)
          return false;
        if(!redisService.db.KeyExists($"{username}::goal::{newTask.GoalName}"))
          return false;
        await redisService.db.ListRightPushAsync($"{username}::{newTask.GoalName}::tasks",newTask.TaskName);
        await redisService.db.HashSetAsync($"{username}::{newTask.GoalName}::{newTask.TaskName}",task);//goalName ne sme biti "goal"
        return true;
      }
      catch(Exception e)
      {
        return false;
      }
  }
  public async Task<bool> DeleteTask(Taskk delTask,string username)
  {
    try
      {
        var broj = await redisService.db.ListPositionsAsync("users",username,1);
        if(broj.Length != 1)
          return false;
        var exist = redisService.db.KeyExists($"{username}::{delTask.GoalName}::{delTask.TaskName}");
        if(!exist)
          return false;
        
        await redisService.db.ListRemoveAsync($"{username}::{delTask.GoalName}::tasks",delTask.TaskName);
        await redisService.db.KeyDeleteAsync($"{username}::{delTask.GoalName}::{delTask.TaskName}");
        return true;
      }
      catch(Exception e)
      {
        return false;
      }
  }

  public async Task<bool> CompleteTask(Taskk compTask,string username)
  {
    try
      {
        var broj = await redisService.db.ListPositionsAsync("users",username,1);
        if(broj.Length != 1)
          return false;
        var exist = redisService.db.KeyExists($"{username}::{compTask.GoalName}::{compTask.TaskName}");
        if(!exist)
          return false;
          
        await Progression(compTask,username);

        
        var taskXP = (double)await redisService.db.HashGetAsync($"{username}::{compTask.GoalName}::{compTask.TaskName}","xp");
        await redisService.db.HashIncrementAsync($"{username}::goal::{compTask.GoalName}","xp",taskXP);
        await redisService.db.HashIncrementAsync($"{username}::goal::{compTask.GoalName}","tasksDone",1);
        var friends = redisService.db.ListRange($"{username}::friends",0,-1);
        await leaderboardService.IncrementXP(username,username,taskXP);
        foreach(var friend in friends)
        {
          await leaderboardService.IncrementXP(username,friend.ToString(),taskXP);
        }
        
        await redisService.db.ListRemoveAsync($"{username}::{compTask.GoalName}::tasks",compTask.TaskName);
        await redisService.db.KeyDeleteAsync($"{username}::{compTask.GoalName}::{compTask.TaskName}");

        return true;
      }
      catch(Exception e)
      {
        return false;
      }
  }

  public async Task<List<Taskk>> GetTasksOfGoal(string username, string goalName)
  {
    var broj = await redisService.db.ListPositionsAsync("users",username,1);
    if(broj.Length != 1)
      throw new Exception("User dosent exist!");
    var exist = redisService.db.KeyExists($"{username}::goal::{goalName}");
    if(!exist)
      throw new Exception("Goal dosent exist.");
    
    List<Taskk> retList = new List<Taskk>();
    var listOfTasksNames = redisService.db.ListRange($"{username}::{goalName}::tasks",0,-1);

    foreach (var taskName in listOfTasksNames)
    {
      var userN = redisService.db.HashGet($"{username}::{goalName}::{taskName.ToString()}","username").ToString();
      var goalN = redisService.db.HashGet($"{username}::{goalName}::{taskName.ToString()}","goalName").ToString();
      var taskN = redisService.db.HashGet($"{username}::{goalName}::{taskName.ToString()}","taskName").ToString();
      var xp = (double)redisService.db.HashGet($"{username}::{goalName}::{taskName.ToString()}","xp");

      var task = new Taskk{
        Username = userN,
        GoalName = goalN,
        TaskName = taskN,
        XP = xp
      };

      retList.Add(task);
    }

    return retList;
    
  }

  private async Task Progression(Taskk task,string username)
  {
    var userLevel = (int)await redisService.db.HashGetAsync(username,"level");
    var userXp = (double)await redisService.db.HashGetAsync(username,"xp");
    var usersTasks = (int)await redisService.db.HashGetAsync(username,"tasksDone");
    var taskXP = (double)await redisService.db.HashGetAsync($"{username}::{task.GoalName}::{task.TaskName}","xp");

    double baseValue =100;
    double exponent = 1.5;
    double xpNeeded = baseValue * Math.Pow(userLevel+1,exponent);

    if(userXp+taskXP >= xpNeeded)
    {
      HashEntry newXp =new HashEntry("xp",xpNeeded);
      HashEntry newLevel = new HashEntry("level",userLevel+1);
      HashEntry newTasks = new HashEntry("tasksDone",usersTasks+1);
      HashEntry[] updatePodaci = [newXp,newLevel,newTasks];
      await redisService.db.HashSetAsync(username,updatePodaci);
    }
    else
    {
      HashEntry newXp =new HashEntry("xp",userXp+taskXP);
      HashEntry newTasks = new HashEntry("tasksDone",usersTasks+1);
      HashEntry[] updatePodaci = [newXp,newTasks];
      await redisService.db.HashSetAsync(username,updatePodaci);
    }
  }


}
}