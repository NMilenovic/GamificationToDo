using System.Formats.Tar;
using System.Xml.XPath;
using Models;

namespace Services{
  public class LeaderboardService{
     private RedisService redisService;
     private UserService userService;
     public LeaderboardService()
     {
      redisService = new RedisService();
      userService = new UserService();
     }

     public async Task IncrementXP(string username,string friend,double xp)
     {
        if(!userService.UserExist(username).Result)
          throw new Exception($"User {username} dosent exist");
        DailyInc(username,friend,xp);
        WeeklyInc(username,friend,xp);
        YearlyInc(username,friend,xp);
        AllTimeInc(username,friend,xp);
     }

     public List<LeaderboardsDTO> ReturnLeaderboard(string username,string leaderboard)
     {
        var RedisLeaderboard = redisService.db.SortedSetRangeByRankWithScores($"{username}::{leaderboard}",0,-1,StackExchange.Redis.Order.Descending);
        List<LeaderboardsDTO> leaderboardArray = new List<LeaderboardsDTO>();
        foreach(var user in RedisLeaderboard)
        {
          LeaderboardsDTO entry = new LeaderboardsDTO{
            Username = user.Element,
            XP = user.Score
          };
          leaderboardArray.Add(entry);
        }
        return leaderboardArray;
     }

     private async Task DailyInc(string username,string friend,double xp)
     {
      string daily = $"{friend}::daily";
      TimeSpan exparation = TimeSpan.FromHours(24);

      if(!redisService.db.KeyExists(daily))
      {
        await redisService.db.SortedSetAddAsync(daily,username,xp);
        redisService.db.KeyExpire(daily,exparation);
      }
      else 
      {
        await redisService.db.SortedSetIncrementAsync(daily,username,xp);
      }
     }

     private async Task WeeklyInc(string username,string friend,double xp)
     {
      string weekly = $"{friend}::weekly";
      TimeSpan exparation = TimeSpan.FromDays(7);

      if(!redisService.db.KeyExists(weekly))
      {
        await redisService.db.SortedSetAddAsync(weekly,username,xp);
        redisService.db.KeyExpire(weekly,exparation);
      }
      else 
      {
        await redisService.db.SortedSetIncrementAsync(weekly,username,xp);
      }
     }

    private async Task YearlyInc(string username,string friend,double xp)
     {
      string yearly = $"{friend}::yearly";
      TimeSpan exparation = TimeSpan.FromDays(365);
      if(!redisService.db.KeyExists(yearly))
      {
        await redisService.db.SortedSetAddAsync(yearly,username,xp);
        redisService.db.KeyExpire(yearly,exparation);
      }
      else 
      {
        await redisService.db.SortedSetIncrementAsync(yearly,username,xp);
      }
     }
    private async Task AllTimeInc(string username,string friend,double xp)
     {
      string allTime = $"{friend}::allTime";

      if(!redisService.db.KeyExists(allTime))
      {
        await redisService.db.SortedSetAddAsync(allTime,username,xp);
      }
      else 
      {
        await redisService.db.SortedSetIncrementAsync(allTime,username,xp);
      }
     }

  }
}