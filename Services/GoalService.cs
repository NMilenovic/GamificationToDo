using Models;
using StackExchange.Redis;

namespace Services
{
  public class GoalService
  {
    private UserService userService;
    private RedisService redisService;
    private User? currentUser;
    private PostService postService;
    public GoalService()
    {
        userService = new UserService();
        redisService = new RedisService();
        postService = new PostService();
    }

    private async Task<Goal> GetGoal(string username, string goalName)
    {
      if(!userService.UserExist(username).Result)
        throw new Exception("User dosent exist.");
      var exist = redisService.db.KeyExists($"{username}::goal::{goalName}");
      if(!exist)
        throw new Exception("Goal dosent exist.");

      var hUsername = await redisService.db.HashGetAsync($"{username}::goal::{goalName}","username");
      var hNaziv = await redisService.db.HashGetAsync($"{username}::goal::{goalName}","goalName");
      var hXP = await redisService.db.HashGetAsync($"{username}::goal::{goalName}","xp");
      var hTasksDone = await redisService.db.HashGetAsync($"{username}::goal::{goalName}","tasksDone");

      return new Goal{
        Username = hUsername.ToString(),
        Naziv = hNaziv.ToString(),
        XP = (double)hXP,
        TasksDone = (int)hTasksDone
      };
    }
    public async Task AddGoal(string username,string goalname)
    {
      
      HashEntry[] goal = {
        new HashEntry("username",username),
        new HashEntry("goalName",goalname),
        new HashEntry("xp",0),
        new HashEntry("taskDone",0)};
      
        await redisService.db.ListRightPushAsync($"{username}::goals",goalname);
        await redisService.db.HashSetAsync($"{username}::goal::{goalname}",goal); 

    }
    public async Task DeleteGoal(string username,string goalName)
    {
      var userExist  = userService.UserExist(username);
      if(userExist.Result == false)
        throw new Exception("User dosent exist."); 

      var exist  = await redisService.db.KeyExistsAsync($"{username}::goal::{goalName}");
      if(exist == false)
        throw new Exception("Goal dosent exist");
      await redisService.db.ListRemoveAsync($"{username}::goals",goalName);
      await redisService.db.KeyDeleteAsync($"{username}::goal::{goalName}");
    }

    public async Task<bool> UpdateGoal(User user,string goalName,string newGoalName)
    {
      currentUser = userService.FindAndReturnUser(user).Result;
      if(currentUser == null)
        return false;

      try
      {
        var exist  = await redisService.db.KeyExistsAsync($"{user.Username}::goal::{goalName}");
        if(exist == false)
          return false;
        await redisService.db.ListInsertAfterAsync($"{user.Username}::goals",goalName,newGoalName);
        await redisService.db.ListRemoveAsync($"{user.Username}::goals",goalName);
        await redisService.db.KeyRenameAsync($"{user.Username}::goal::{goalName}",$"{user.Username}::goal::{newGoalName}");
        return true;
      }
      catch(Exception e)
      {
        return false;
      }
    }

    public async Task CompleteGoal(string username,string goalName)
    {
      Goal g = await GetGoal(username,goalName);

      await redisService.db.ListRemoveAsync($"{username}::goals",goalName,0);
      await redisService.db.KeyDeleteAsync($"{username}::goal::{goalName}");

      await redisService.db.HashIncrementAsync(username,"goalsDone",1);
      Guid uuid = Guid.NewGuid();
      string mes = $"{g.Username} completed goal {g.Naziv}. For this goal he/she did {g.TasksDone} tasks, acquiring {g.XP} xps";
      Post post = new Post{
        Username = username,
        ID = uuid.ToString(),
        Message = mes
      };
      await postService.MakePost(post);
    }

    public async Task<List<Goal>> GetGoals(string username)
    {
      var goals = await redisService.db.ListRangeAsync($"{username}::goals",0,-1);
      List<Goal> retGoals = new List<Goal>();
      foreach (var g in goals)
      {
         var goal = await GetGoal(username,g.ToString());
         retGoals.Add(goal);
      }
      return retGoals;
    }

}

   
    
}