import { LoginPage } from "./LoginPage.js";
import { RegisterPage } from "./RegisterPage.js";

export class IndexPage{
  constructor()
  {
    this.kont = null;
  }

  draw(host)
  {
    this.kont = document.createElement("div");
    this.kont.className = "MainKont";
    host.appendChild(this.kont);

    let indexDiv = document.createElement("div");
    indexDiv.className = "indexDiv";
    this.kont.appendChild(indexDiv);

    let row = document.createElement("div");
    row.className = "indexRow";
    indexDiv.appendChild(row);

    let b = document.createElement("button");
    b.innerHTML = "Login";
    b.onclick = (ev) =>{this.LogIn(this.kont)}
    b.className = "indexBtn";
    row.appendChild(b);

    row = document.createElement("div");
    row.className = "indexRow";
    indexDiv.appendChild(row);

    b = document.createElement("button");
    b.innerHTML = "Register";
    b.onclick = (ev) =>{this.Register(this.kont)}
    b.className = "indexBtn";
    row.appendChild(b);

  }

  LogIn(host){
    let loginPage = new LoginPage(this.kont);
    loginPage.draw();
  }
  Register(host){
    let registerPage = new RegisterPage(host);
    registerPage.draw();
  }

}