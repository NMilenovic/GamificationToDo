using System.Security.Cryptography;
using Models;

namespace Services
{
  public class FriendService 
  {
    private RedisService redisService;
    private UserService userService;

    public FriendService()
    {
      redisService = new RedisService();
      userService = new UserService();
    }

    public async Task<bool> AreFriends(string user1,string user2)
    {
      var check = await redisService.db.ListRemoveAsync($"{user1}::friends",user2,0);
      if(check == 0)
          return false;
      else
      {
        await redisService.db.ListRightPushAsync($"{user1}::friends",user2);
        return true;
      }
    }


    public async Task SendFriendRequest(string username,string friendUsername)
    {
      var check = await userService.UserExist(username);
      if(check == false)
        throw new Exception("Sender dosent exist.");
      
      check = await userService.UserExist(friendUsername);
      if(check == false)
        throw new Exception("Receiver dosent exist.");
      
      await redisService.db.ListRightPushAsync($"{friendUsername}::friendRequestsList",username);
    }
    public async Task AcceptFriendRequest(string username,string friendUsername)
     {
      var check = await userService.UserExist(friendUsername);
      if(check == false)
        throw new Exception("Sender dosent exist.");
      
      check = await userService.UserExist(username);
      if(check == false)
        throw new Exception("Receiver dosent exist.");

      await redisService.db.ListRightPushAsync($"{username}::friends",friendUsername);
      await redisService.db.ListRightPushAsync($"{friendUsername}::friends",username);

      await redisService.db.HashIncrementAsync(username,"numOfFriends",1);
      await redisService.db.HashIncrementAsync(friendUsername,"numOfFriends",1);
      var exist = await redisService.db.ListRemoveAsync($"{username}::friendRequestsList",friendUsername,0);
      if(exist == 0)
        throw new Exception("Friendship request dosent exist!");
    }
    public async Task DeclineFriendRequest(string username, string friendUsername)
    {
      var check = await userService.UserExist(friendUsername);
      if(check == false)
        throw new Exception("Sender dosent exist.");
      
      check = await userService.UserExist(username);
      if(check == false)
        throw new Exception("Receiver dosent exist.");
      
      var exist = await redisService.db.ListRemoveAsync($"{username}::friendRequestsList",friendUsername,0);
      if(exist == 0)
        throw new Exception("Friendship request dosent exist!");
    }

    public async Task RemoveFriend(string username,string friendUsername)
    {
      var check = await userService.UserExist(username);
      if(check == false)
        throw new Exception($"{username} dosent exist.");
      
      check = await userService.UserExist(friendUsername);
      if(check == false)
        throw new Exception($"{friendUsername} dosent exist.");
      
      if(!AreFriends(username,friendUsername).Result)
        throw new Exception($"{username} and {friendUsername} are not friends.");
      
      await redisService.db.ListRemoveAsync($"{username}::friends",friendUsername,0);
      await redisService.db.ListRemoveAsync($"{friendUsername}::friends",username,0);

      await redisService.db.HashDecrementAsync(username,"numOfFriends",1);
      await redisService.db.HashDecrementAsync(friendUsername,"numOfFriends",1);
    }

    public async Task<List<FriendDTO>> GetFriends(string username)
    {
      List<FriendDTO> listOfFriends = new List<FriendDTO>();
      var check = await userService.UserExist(username);
      if(check == false) 
        throw new Exception($"{username} dosent exist.");
      
      var numOfFriends = redisService.db.ListLength($"{username}::friends");
      if(numOfFriends == 0)
        return listOfFriends;    
      var friendsUsernames = await redisService.db.ListRangeAsync($"{username}::friends",0,-1);
      foreach(var friend in friendsUsernames)
      {
        FriendDTO fDTO = new FriendDTO{
          Username = redisService.db.HashGet(friend.ToString(),"username").ToString(),
          Level = (int)redisService.db.HashGet(friend.ToString(),"level")
        };
        listOfFriends.Add(fDTO);
      }
      return listOfFriends;
    }
    public async Task<List<FriendDTO>> GetFriendRequests(string username)
    {
      List<FriendDTO> listOfFriendsRequests = new List<FriendDTO>();
      var check = await userService.UserExist(username);
      if(check == false) 
        throw new Exception($"{username} dosent exist.");
      
      var numOfFriendsRequests = redisService.db.ListLength($"{username}::friendRequestsList");
      if(numOfFriendsRequests == 0)
        return listOfFriendsRequests;    
      var friendRequestsUsernames = await redisService.db.ListRangeAsync($"{username}::friendRequestsList",0,-1);
      foreach(var friendRequest in friendRequestsUsernames)
      {
        FriendDTO fDTO = new FriendDTO{
          Username = redisService.db.HashGet(friendRequest.ToString(),"username").ToString(),
          Level = (int)redisService.db.HashGet(friendRequest.ToString(),"level")
        };
        listOfFriendsRequests.Add(fDTO);
      }
      return listOfFriendsRequests;
    }
    public async Task<List<FriendDTO>> SearchNewFriends(string username,string pattern)
    {
      var users = await redisService.db.ListRangeAsync("users",0,-1);

      List<FriendDTO> usersFromQuery = new  List<FriendDTO>();
      foreach(var user in users)
      {
        
        if(!AreFriends(username,user.ToString()).Result && user.ToString() != username && !ExistInList($"{user.ToString()}::friendRequestsList",username))
        {
          if(user.ToString().Contains(pattern))
          {
            int level = (int) redisService.db.HashGet(user.ToString(),"level");
            FriendDTO userFromQuery = new FriendDTO{
            Username = user.ToString(),
            Level = level
          };
            usersFromQuery.Add(userFromQuery);
          }
        }
      }
      return usersFromQuery;
    }

    private bool ExistInList(string list, string value)
    {
      var exist = redisService.db.ListRemove(list,value,0);
      if(exist == 0)
        return false;
      else
      {
        redisService.db.ListRightPush(list,value);
        return true;
      }
    }
  }


 
}