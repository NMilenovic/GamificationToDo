using Models;
using StackExchange.Redis;

namespace Services
{
  public class PostService
  {
      RedisService redisService;

      public PostService()
      {
        redisService = new RedisService();
      }

      public async Task<List<string>> GetPosts(string username)
      {
        var namesOfPosts = redisService.db.ListRange($"{username}::posts",0,-1);
        List<string> posts = new List<string>();
        foreach(var name in namesOfPosts)
        {
          var message  =await redisService.db.StringGetAsync($"{username}::post::{name}");
          posts.Add(message);
        }
        return posts;
      }

      public async Task MakePost(Post post)
      {

        await redisService.db.ListLeftPushAsync($"{post.Username}::posts",post.ID);
        var check = await redisService.db.StringSetAsync($"{post.Username}::post::{post.ID}",post.Message);
        if(check == false)
        {
          await redisService.db.ListRemoveAsync($"{post.Username}::posts",post.ID);
          throw new Exception("Error ocurred while trying to make a post. Please try again.");
        }

        var listOfFriends = redisService.db.ListRange($"{post.Username}::friends",0,-1);
        if(listOfFriends.Length == 0)
          return;
        
        foreach(var friend in listOfFriends)
        {
          SendMessage($"{friend}::feed",post.Message);
        }
      }

      private async Task SendMessage(string list,string message)
      {
        var len = redisService.db.ListLength(list);
        if(len < 20)
        {
          redisService.db.ListLeftPushAsync(list,message);
        }
        else
        {
          redisService.db.ListRightPop(list);
          redisService.db.ListLeftPushAsync(list,message);
        }
      }

    public List<string> GetFeed(string username)
    {
      var listaPoruka =  redisService.db.ListRange($"{username}::feed",0,-1);
      List<string> feed = new List<string>();
      foreach(var poruka in listaPoruka)
      {
        feed.Add(poruka.ToString());
      }
      return feed;
    }
  }
}