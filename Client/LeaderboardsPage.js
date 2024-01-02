import { NavBar } from "./NavBar.js";

export class LeaderboardsPage{

  constructor(currentUser,host)
  {
    this.currentUser = currentUser;
    this.kont = host;
  }

  Draw()
  {
    while(this.kont.firstChild)
      this.kont.removeChild(this.kont.firstChild);

      let LeaderboardsPage = document.createElement("div");
      this.kont.appendChild(LeaderboardsPage);

      let navDiv = document.createElement("div");
      navDiv.className = "navDiv";
      LeaderboardsPage.appendChild(navDiv);  
      let navBar = new NavBar(this.kont,this.currentUser);
      navBar.Draw(navDiv);

      let tabDiv = document.createElement("div");
      tabDiv.className = "tabs"
      LeaderboardsPage.appendChild(tabDiv);
     
      let surfaceDiv = document.createElement("div");
      LeaderboardsPage.appendChild(surfaceDiv);
  
      this.DrawButtons(tabDiv,surfaceDiv);
  }

  DrawButtons(host,div)
  {
    let button = document.createElement("button");
    button.innerHTML = "Daily";
    button.className = "selectiveBtn";
    button.onclick = (ev) =>{this.DrawLeaderboard(div,"Daily")}
    host.appendChild(button);

    button = document.createElement("button");
    button.innerHTML = "Weekly";
    button.className = "selectiveBtn";
    button.onclick = (ev) =>{this.DrawLeaderboard(div,"Weekly")}
    host.appendChild(button);

    button = document.createElement("button");
    button.innerHTML = "Yearly";
    button.className = "selectiveBtn";
    button.onclick = (ev) =>{this.DrawLeaderboard(div,"Yearly")}
    host.appendChild(button);

    button = document.createElement("button");
    button.innerHTML = "All time";
    button.className = "selectiveBtn";
    button.onclick = (ev) =>{this.DrawLeaderboard(div,"AllTime")}
    host.appendChild(button);
  }

  DrawLeaderboard(host,param)
  {
    while(host.firstChild)
      host.removeChild(host.firstChild);

    let leadebordDiv = document.createElement("div");
    leadebordDiv.className = "leaderBoardsDiv ";
    host.appendChild(leadebordDiv);

    let entryDiv;
    let i =1;
    fetch("http://localhost:5191/Leaderboard/"+param+"Leaderboard/"+this.currentUser.username)
    .then(p =>{
      p.json().then(entries =>{
        entries.forEach(entry =>{
          entryDiv = document.createElement("div");
          entryDiv.classList = "entryDiv";
          leadebordDiv.appendChild(entryDiv);

          let name = document.createElement("label");
          let xp = document.createElement("label");
          let rank = document.createElement("label");
          rank.className = "rank";

          rank.innerHTML = i+".";
          i = i+1;
          name.innerHTML = entry.username;
          xp.innerHTML = "XP: "+entry.xp;

          entryDiv.appendChild(rank);
          entryDiv.appendChild(name);
          entryDiv.appendChild(xp);
        });
      });
    })
  }
}