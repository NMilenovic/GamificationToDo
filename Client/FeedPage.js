import { NavBar } from "./NavBar.js";

export class FeedPage{
  constructor(currentUser,host)
  {
    this.currentUser = currentUser;
    this.kont = host;
  }

  Draw()
  {
    while(this.kont.firstChild)
      this.kont.removeChild(this.kont.firstChild);

    let feedPage = document.createElement("div");
    this.kont.appendChild(feedPage);
    
    let navDiv = document.createElement("div");
    navDiv.className = "navDiv";
    feedPage.appendChild(navDiv);  
    let navBar = new NavBar(this.kont,this.currentUser);
    navBar.Draw(navDiv);

    let messagesDiv = document.createElement("div");
    feedPage.appendChild(messagesDiv);
    let divForMessage;
    fetch("http://localhost:5191/Post/GetFeed/"+this.currentUser.username)
    .then(p =>{
      p.json().then(messages =>{
        messages.forEach(message =>{
          divForMessage = document.createElement("div");
          divForMessage.className = "divForMessage";
          messagesDiv.appendChild(divForMessage);

          let lMessage = document.createElement("label");
          lMessage.innerHTML = message;
          divForMessage.appendChild(lMessage);
        });
      });
    });
    
  }
}