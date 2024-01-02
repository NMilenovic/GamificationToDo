
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Services;
using StackExchange.Redis;

namespace Services
{
public class UserService
{
    private RedisService redis = new RedisService();
    
    public UserService()
    {
        
    }
    private string ShaEncoding(string pass)
    {
        var sha = SHA256.Create();
        var strByte = Encoding.UTF8.GetBytes(pass);
        byte[] passHash = sha.ComputeHash(strByte); 
        var str = Encoding.UTF8.GetString(passHash);

        return str;

    }
    //Potencijalni bottleneck
    public async Task<List<User>> GetAllUsersAsync() //treba radi
    {
        var users = new List<User>();

        var usernames = await redis.db.ListRangeAsync("users",0,-1);

        foreach(var u in usernames)
        {
            var k = u.ToString();
            var username = await redis.db.HashGetAsync(k,"username");
            var password = await redis.db.HashGetAsync(k,"password");

            var xp = await redis.db.HashGetAsync(k,"xp");
            double.TryParse(xp,out double  xpd);
            
            var level = await redis.db.HashGetAsync(k,"level");
            var tasksDone = await redis.db.HashGetAsync(k,"tasksDone");
            var goalsDone = await redis.db.HashGetAsync(k,"goalsDone");
            var numOfFriends = await redis.db.HashGetAsync(k,"numOfFriends");

            users.Add(new User{
                Username = username.ToString(),
                Password = password.ToString(),
                XP = xpd,
                Level =(int)level,
                TasksDone = (int) tasksDone,
                GoalsDone = (int) goalsDone,
                NumOfFriends = (int) numOfFriends
                });
        }
       return users;

     
    }
    public bool IsUsernameTaken(string username)
    {
        var usersTasks = GetAllUsersAsync();
        var users = usersTasks.Result;
        
        if(users.Any(u => u.Username == username))
        {
            return false;
        }
        return true;
    }
    public async Task<bool> RegisterUser(string username, string password) //radi
    {
        var str = ShaEncoding(password);
        var hash = new HashEntry[]{
            new HashEntry("username",username),
            new HashEntry("password",str),
            new HashEntry("xp",0),
            new HashEntry("level",1),
            new HashEntry("tasksDone",0),
            new HashEntry("goalsDone",0),
            new HashEntry("numOfFriends",0)
        };
        var u =await redis.db.ListRightPushAsync("users",username);
        await redis.db.HashSetAsync(username,hash);

        if(u > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Ovo mola rewritujem za novi model
    public async Task<User> FindAndReturnUser(User user)
    {
         var usersTasks = GetAllUsersAsync();
         var users = usersTasks.Result;

         var str = ShaEncoding(user.Password);
         var pronadjenUser = users.FirstOrDefault(u => u.Username == user.Username && u.Password == str);
         
         if(pronadjenUser != null)
            return new User{
                Username = pronadjenUser.Username, 
                Password = pronadjenUser.Password,
                XP = pronadjenUser.XP,
                Level = pronadjenUser.Level,
                TasksDone = pronadjenUser.TasksDone,
                GoalsDone = pronadjenUser.GoalsDone,
                NumOfFriends = pronadjenUser.NumOfFriends
                };
         else
            return null;
    }
    public async Task<User> FindAndReturnUser(string user)
    {
         var usersTasks = GetAllUsersAsync();
         var users = usersTasks.Result;

         var pronadjenUser = users.FirstOrDefault(u => u.Username == user);
         
         if(pronadjenUser != null)
            return new User{
                Username = pronadjenUser.Username, 
                Password = pronadjenUser.Password,
                XP = pronadjenUser.XP,
                Level = pronadjenUser.Level,
                TasksDone = pronadjenUser.TasksDone,
                GoalsDone = pronadjenUser.GoalsDone,
                NumOfFriends = pronadjenUser.NumOfFriends
                };
         else 
            throw new Exception($"{user} dosent exist.");
    }

    public async Task<bool> UserExist(string username)
    {
        var exist =await redis.db.ListRemoveAsync("users",username,0);
        if(exist == 1)
        {
            await redis.db.ListRightPushAsync("users",username);
            return true;
        }
        return false;
    }
}
}