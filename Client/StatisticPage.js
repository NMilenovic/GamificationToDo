import { NavBar } from "./NavBar.js";
import { User } from "./User.js";

export class StatisticPage{
  constructor(currentUser,host)
  {
    this.currentUser = currentUser;
    this.kont = host;
  }

  Draw()
  {
    while(this.kont.firstChild)
      this.kont.removeChild(this.kont.firstChild);

    let statisticPage = document.createElement("div");
    this.kont.appendChild(statisticPage);
    
    let navDiv = document.createElement("div");
    navDiv.className = "navDiv";
    statisticPage.appendChild(navDiv);  
    let navBar = new NavBar(this.kont,this.currentUser);
    navBar.Draw(navDiv);

    let statsDiv = document.createElement("div");
    statsDiv.className = "statsDiv";
    statisticPage.appendChild(statsDiv);
    let row;
    fetch("http://localhost:5191/User/GetUser/"+this.currentUser.username)
    .then(p =>{
      p.json().then(u =>{
        let statsOfUser = new User(u.username,u.xp,u.level,u.tasksDone,u.goalsDone,u.numOfFriends);
        
        row = document.createElement("div");
        row.className = "statsTitle";
        statsDiv.appendChild(row);

        let l = document.createElement("label");
        l.innerHTML = statsOfUser.username;
        row.appendChild(l);

        row = document.createElement("div");
        row.className = "stat";
        statsDiv.appendChild(row);
        l = document.createElement("label");
        l.innerHTML = "LEVEL: ";
        row.appendChild(l);
        l = document.createElement("label");
        l.innerHTML = statsOfUser.level;
        row.appendChild(l);


        row = document.createElement("div");
        row.className = "stat";
        statsDiv.appendChild(row);
        l = document.createElement("label");
        l.innerHTML = "XP: ";
        row.appendChild(l);
        l = document.createElement("label");
        l.innerHTML = statsOfUser.xp;
        row.appendChild(l);

        row = document.createElement("div");
        row.className = "stat";
        statsDiv.appendChild(row);
        l = document.createElement("label");
        l.innerHTML = "Tasks done: ";
        row.appendChild(l); 
        l = document.createElement("label");
        l.innerHTML = statsOfUser.tasksDone;
        row.appendChild(l)
        

        row = document.createElement("div");
        row.className = "stat";
        statsDiv.appendChild(row);
        l = document.createElement("label");
        l.innerHTML = "Goals done : ";
        row.appendChild(l); 
        l = document.createElement("label");
        l.innerHTML = statsOfUser.goalsDone;
        row.appendChild(l)

        row = document.createElement("div");
        row.className = "stat";
        statsDiv.appendChild(row);
        l = document.createElement("label");
        l.innerHTML = "Number of friends: ";
        row.appendChild(l); 
        l = document.createElement("label");
        l.innerHTML = statsOfUser.numOfFriends;
        row.appendChild(l)

      });
    });
    
  }
}