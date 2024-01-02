import { NavBar } from "./NavBar.js";

export class FriendsPage{
  constructor(currentUser,host)
  {
    this.currentUser = currentUser;
    this.kont = host;
  }

  Draw()
  {
    while(this.kont.firstChild)
      this.kont.removeChild(this.kont.firstChild);

    let friendsPage = document.createElement("div");
    this.kont.appendChild(friendsPage);
    
    //Nav bar
    let navDiv = document.createElement("div");
    navDiv.className = "navDiv";
    friendsPage.appendChild(navDiv);  
    let navBar = new NavBar(this.kont,this.currentUser);
    navBar.Draw(navDiv);

    //Tabs row
    let tabDiv = document.createElement("div");
    tabDiv.className = "tabs"
    friendsPage.appendChild(tabDiv);
   
    let surfaceDiv = document.createElement("div");
    friendsPage.appendChild(surfaceDiv);

    this.DrawButtons(tabDiv,surfaceDiv);
  }

    DrawButtons(host,div)
    {
    let button = document.createElement("button");
    button.innerHTML = "Friendslist";
    button.className = "selectiveBtn";
    button.onclick = (ev) =>{this.DrawFriendlist(div)}
    host.appendChild(button);

    button = document.createElement("button");
    button.innerHTML = "Friend requests";
    button.className = "selectiveBtn";
    button.onclick = (ev) =>{this.DrawRequests(div)}
    host.appendChild(button);

    button = document.createElement("button");
    button.innerHTML = "Search users";
    button.className = "selectiveBtn";
    button.onclick = (ev) => {this.DrawUsers(div)}
    host.appendChild(button);
  }

  DrawFriendlist(host)
  {
    while(host.firstChild)
      host.removeChild(host.firstChild);

    let friendslist = document.createElement("div");
    host.appendChild(friendslist);

    let friendDiv;
    fetch("http://localhost:5191/Friend/GetFriends/"+this.currentUser.username)
    .then(
      p=>{
        p.json().then(friends =>{
          friends.forEach(friend =>{
          friendDiv = document.createElement("div");
          friendDiv.className = "entryDiv";
          friendslist.appendChild(friendDiv);

          let name = document.createElement("label");
          let level = document.createElement("label");
          let removeFriend = document.createElement("button");
          removeFriend.onclick = (ev) =>{
            this.RemoveFriend(friendDiv,friend.username);
          }

          name.innerHTML = friend.username;
          level.innerHTML = "LEVEL: "+friend.level;
          removeFriend.innerHTML = "REMOVE";

          friendDiv.appendChild(level);
          friendDiv.appendChild(name);
          friendDiv.appendChild(removeFriend);
          })
        });
      }
    );
  }

  DrawRequests(host)
  {
    while(host.firstChild)
      host.removeChild(host.firstChild);

    let requests = document.createElement("div");
    host.appendChild(requests);

    let reqDiv;
    //Ovo mora bude on click
    fetch("http://localhost:5191/Friend/GetFriendRequests/"+this.currentUser.username)
    .then(p =>{
      p.json().then(reqs =>{
        reqs.forEach(req =>{
          reqDiv = document.createElement("div");
          reqDiv.className = "entryDiv";
          requests.appendChild(reqDiv);

          let name = document.createElement("label");
          let level = document.createElement("label");
          name.innerHTML = req.username;
          level.innerHTML = "LEVEL: "+req.level;

          reqDiv.appendChild(name);
          reqDiv.appendChild(level);

          let acpBtn = document.createElement("button");
          acpBtn.innerHTML = "Accept";
          acpBtn.onclick = (ev) =>{this.AcceptFriendRequest(reqDiv,req.username)}
          let decBtn = document.createElement("button");
          decBtn.innerHTML = "Decline";
          decBtn.onclick = (ev) =>{this.DeclineFriendRequest(reqDiv,req.username)}

          reqDiv.appendChild(acpBtn);
          reqDiv.appendChild(decBtn);
        })
      })
    })
  }

  DrawUsers(host)
  {
    while(host.firstChild)
    host.removeChild(host.firstChild);

    let searchRow = document.createElement("div");
    host.appendChild(searchRow);
    searchRow.className = "searchRow";
    let searchbar = document.createElement("input");
    searchbar.className = "searchbar";
    searchRow.appendChild(searchbar);

    searchRow = document.createElement("div");
    host.appendChild(searchRow);
    searchRow.className = "searchRow";
    let searchBtn = document.createElement("button");
    searchBtn.className = "searchBtn";
    searchBtn.innerHTML = "SEARCH";
    searchRow.appendChild(searchBtn);

    let userslist = document.createElement("div");
    host.appendChild(userslist);

    let userDiv;
    searchBtn.onclick = (ev) =>{

      while(userslist.firstChild)
        userslist.removeChild(userslist.firstChild);
        

      fetch("http://localhost:5191/Friend/SearchNewFriends/"+this.currentUser.username+"/"+searchbar.value)
      .then(p =>{
        p.json().then(friends =>{
          friends.forEach(friend =>{
            userDiv = document.createElement("div");
            userDiv.className = "entryDiv";
            userslist.appendChild(userDiv);

            let name = document.createElement("label");
            let level = document.createElement("label");
            let add = document.createElement("button");

            name.innerHTML = friend.username;
            level.innerHTML = friend.level;
            add.innerHTML = "ADD";

            add.onclick = (ev) =>{
              this.AddFriend(userDiv,friend.username);
            }

            userDiv.appendChild(name);
            userDiv.appendChild(level);
            userDiv.appendChild(add);
          });
        });
      });
    }
  }

  AddFriend(host,name)
  {
    fetch("http://localhost:5191/Friend/SendFriendRequest/"+this.currentUser.username+"/"+name,{
      method: "post",
      headers: {'Content-Type' : 'application/json'}
    })
    .then(p =>{
      host.remove();  
    });
  }
  AcceptFriendRequest(host,username)
  {
    fetch("http://localhost:5191/Friend/AcceptFriendRequest/"+this.currentUser.username+"/"+username,{
      method:"delete",
      headers: {'Content-Type' : 'application/json'}
    })
    .then(p =>{
      host.remove();
    });
  }

  DeclineFriendRequest(host,username)
  {
    fetch("http://localhost:5191/Friend/DeclineFriendRequest/"+this.currentUser.username+"/"+username,{
      method:"delete",
      headers: {'Content-Type' : 'application/json'}
    })
    .then(p =>{
      host.remove();
    });
  }

  RemoveFriend(host,name)
  {
    fetch("http://localhost:5191/Friend/RemoveFriend/"+this.currentUser.username+"/"+name,
    {
      method: "delete",
      headers: {'Content-Type' : 'application/json'}
    })
    .then(p =>{
      host.remove();
    })
  }
}