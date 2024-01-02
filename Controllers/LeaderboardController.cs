using Microsoft.AspNetCore.Mvc;
using Services;

namespace GamificationToDo.Controllers;
[ApiController]
[Route("[controller]")]
public class LeaderboardController : ControllerBase
{
  LeaderboardService leaderboardService;

  public LeaderboardController()
  {
    leaderboardService = new LeaderboardService();
  }
  
  [HttpGet("DailyLeaderboard/{user}")]
  public IActionResult DailyLeaderboard(string user)
  {
    if(String.IsNullOrWhiteSpace(user))
      return BadRequest("Username is required");
    var leaderboard = leaderboardService.ReturnLeaderboard(user,"daily");
    return Ok(leaderboard);

  }
   [HttpGet("WeeklyLeaderboard/{user}")]
  public IActionResult WeeklyLeaderboard(string user)
  {
    if(String.IsNullOrWhiteSpace(user))
      return BadRequest("Username is required");
    var leaderboard = leaderboardService.ReturnLeaderboard(user,"weekly");
    return Ok(leaderboard);
  }
   [HttpGet("YearlyLeaderboard/{user}")]
  public IActionResult YearlyLeaderboard(string user)
  {
    if(String.IsNullOrWhiteSpace(user))
      return BadRequest("Username is required");
    var leaderboard = leaderboardService.ReturnLeaderboard(user,"yearly");
    return Ok(leaderboard);
  }
   [HttpGet("AllTimeLeaderboard/{user}")]
  public IActionResult AllTimeLeaderboard(string user)
  {
    if(String.IsNullOrWhiteSpace(user))
      return BadRequest("Username is required");
    var leaderboard = leaderboardService.ReturnLeaderboard(user,"allTime");
    return Ok(leaderboard);
  }
}