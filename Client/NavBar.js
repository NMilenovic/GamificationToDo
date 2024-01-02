import { FriendsPage } from "./FriendsPage.js";
import { ProfilePage } from "./ProfilePage.js ";
import { LeaderboardsPage } from "./LeaderboardsPage.js"
import { StatisticPage } from "./StatisticPage.js";
import { FeedPage } from "./FeedPage.js";

export class NavBar{

  constructor(kont,currentUser)
  {
    this.kont = kont;
    this.currentUser = currentUser;
    this.pages = ["PROFILE","LEADERBOARDS","FEED","FRIENDS","STATISTICS"];
  }


  Draw(host)
  {
    this.pages.forEach(page => {
      let button = document.createElement("button");
      button.className = "navButton";
      button.onclick = (ev) =>{this[page]();} 
      button.innerHTML = page;
      host.appendChild(button);
    });
  }

  PROFILE()
  {
    let p = new ProfilePage(this.currentUser,this.kont);
    p.Draw();
  }
  LEADERBOARDS()
  {
    let l = new LeaderboardsPage(this.currentUser,this.kont);
    l.Draw();
  }
  FEED()
  {
    let f = new FeedPage(this.currentUser,this.kont);
    f.Draw(); 
  }
  FRIENDS()
  {
    let f = new FriendsPage(this.currentUser,this.kont);
    f.Draw();
  }
  STATISTICS()
  {
    let s = new StatisticPage(this.currentUser,this.kont);
    s.Draw();
  }
}