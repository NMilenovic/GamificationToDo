import { ProfilePage } from "./ProfilePage.js";
import { User } from "./User.js";

export class LoginPage{

  constructor(host)
  {
    this.kont = host;
  }

  draw()
  {

    while(this.kont.firstChild)
      this.kont.removeChild(this.kont.firstChild);

    let loginDiv = document.createElement("div");
    loginDiv.className = "loginDiv";
    this.kont.appendChild(loginDiv);

    let row = document.createElement("div");
    row.className = "title";
    loginDiv.appendChild(row);

    let l = document.createElement("label");
    l.innerHTML = "Welcome back!";
    row.appendChild(l);

    row = document.createElement("div");
    row.className = "subTitle";
    loginDiv.appendChild(row);
    l = document.createElement("label");
    l.innerHTML = "Login";
    row.appendChild(l);

    let inputBox = document.createElement("div");
    inputBox.className = "inputBox";
    loginDiv.appendChild(inputBox);

    let rowDiv = document.createElement("div");
    rowDiv.className = "rowDiv";
    inputBox.appendChild(rowDiv);

    l = document.createElement("label");
    l.innerHTML = "Username";
    rowDiv.appendChild(l);
    let input = document.createElement("input");
    input.placeholder = "Enter your username";
    input.className ="loginInput";
    input.id = "username";
    rowDiv.appendChild(input);

    rowDiv = document.createElement("div");
    rowDiv.className = "rowDiv";
    inputBox.appendChild(rowDiv);

    l = document.createElement("label");
    l.innerHTML = "Password";
    rowDiv.appendChild(l);
    input = document.createElement("input");
    input.className ="loginInput";
    input.id = "password";
    input.type = "password";
    input.placeholder = "Enter your password";
    rowDiv.appendChild(input);

    rowDiv = document.createElement("div");
    rowDiv.className = "rowDiv";
    inputBox.appendChild(rowDiv);

    let b = document.createElement("button");
    b.innerHTML = "Login";
    b.className = "loginButton";
    b.onclick = (ev) => {this.LoginFetch(this.kont)}
    rowDiv.appendChild(b);
  }

  LoginFetch(host)
  {
    let username = host.querySelector("#username");
    let password = host.querySelector("#password");

    fetch("http://localhost:5191/user/login/"+username.value+"/"+password.value).then(
      u =>{
        u.json().then(user =>{
         let currentUser= new User(user.username,user.xp,user.level,user.tasksDone,user.goalsDone,user.numOfFriends);
         let profilePage = new ProfilePage(currentUser,host);
         profilePage.Draw();
        })
      }
    )
  }

  
  
}