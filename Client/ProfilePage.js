import { Goal } from "./Goal.js";
import { NavBar } from "./NavBar.js";
import { Task } from "./Task.js";

export class ProfilePage{
  constructor(currentUser,host)
  {
    this.currentUser = currentUser;
    this.goals = [];
    this.kont = host;
  }

  Draw()
  {
    while(this.kont.firstChild)
      this.kont.removeChild(this.kont.firstChild);

    let profileDiv = document.createElement("div");
    profileDiv.className = "profileDiv";
    this.kont.appendChild(profileDiv);
  
    let navDiv = document.createElement("div");
    navDiv.className = "navDiv";
    profileDiv.appendChild(navDiv);  
    let navBar = new NavBar(this.kont,this.currentUser);
    navBar.Draw(navDiv);

   
    this.FetchGoals();

    let feedDiv = document.createElement("div");
    profileDiv.appendChild(feedDiv);
    feedDiv.className = "rightDiv";
    this.DrawFeed(feedDiv);

  }


  DrawGoals() 
  {
    let pd = document.querySelector(".profileDiv");
    let leftDiv = document.createElement("div");
    leftDiv.className = "leftDiv";
    pd.appendChild(leftDiv);
      
    let row = document.createElement("div");
    row.className = "title"
    leftDiv.appendChild(row);
    
    let l = document.createElement("label");
    l.innerHTML = "Your goals";
    row.appendChild(l);

    let goalFeild = document.createElement("div");
    goalFeild.className = "goalFeild"
    leftDiv.appendChild(goalFeild);
    

    this.goals.forEach(goal => {
      let b = document.createElement("button");
      b.type = "button";
      b.className = "collapsible";
      b.innerHTML = goal.naziv;
      goalFeild.appendChild(b);
    
      let contentDiv = document.createElement("div");
      contentDiv.className = "content";
      goalFeild.appendChild(contentDiv);
    
        
      this.FetchTasks(goal.naziv).then(p => {
        p.json().then(tasks => {
          tasks.forEach(task => {
            let rowForTask = document.createElement("div");
            contentDiv.appendChild(rowForTask);
            rowForTask.className = "rowForTask";
            let lab = document.createElement("label");
            lab.className = "taskNameLab";
            lab.innerHTML = task.taskName;
            rowForTask.appendChild(lab);

            let complBtn = document.createElement("button");
            complBtn.innerHTML = "Complete";
            complBtn.className = "completeTaskBtn";
            complBtn.onclick = (ev) => {
              this.CompleteTask(task.taskName,goal.naziv);
              rowForTask.remove()
            };
            rowForTask.append(complBtn);

            let deleteBtn = document.createElement("button");
            deleteBtn.className = "deleteTaskBtn";
            deleteBtn.innerHTML = "Delete";
            deleteBtn.onclick = (ev) => {
              this.DeleteTask(task.taskName,goal.naziv);
              rowForTask.remove()
            };
            rowForTask.append(deleteBtn);

          });
        });
      });

      row = document.createElement("div");
      row.className = "row";
      contentDiv.appendChild(row);
      this.CompleteGoal(row,goal.naziv);

      row = document.createElement("div");
      row.className = "row";
      contentDiv.appendChild(row);
      this.DeleteGoal(row,goal.naziv);

      this.AddTask(contentDiv,goal.naziv);
      b.addEventListener("click", function () {
        this.classList.toggle("active");
        var content = this.nextElementSibling;
        if (content.style.display === "block") {
          content.style.display = "none";
        } else {
          content.style.display = "block";
        }
      });
      });

      let newGoalSubDiv = document.createElement("div");
      newGoalSubDiv.className = "title";
      leftDiv.appendChild(newGoalSubDiv)
      let newGoalLab = document.createElement("lab");
      newGoalLab.innerHTML = "Add new goal";
      newGoalSubDiv.appendChild(newGoalLab);

      row = document.createElement("div");
      row.className = "goalSubmitRow";
      leftDiv.appendChild(row);

      let goalName = document.createElement("label");
      goalName.innerHTML = "New goal name: "
      row.appendChild(goalName);

      let goalInput = document.createElement("input");
      goalInput.className = "goalInput";
      row.appendChild(goalInput);

      row = document.createElement("div");
      row.className = "row";
      leftDiv.appendChild(row);
      let addNewGoalButton = document.createElement("button");
      addNewGoalButton.innerHTML="Add new goal"; 
      addNewGoalButton.className = "submitButton";
      addNewGoalButton.onclick = (ev) =>{
        this.AddNewGoal();
      }
      row.appendChild(addNewGoalButton);
}

  FetchGoals()
  {
    fetch("http://localhost:5191/Goal/GetGoals/"+this.currentUser.username).then(
      p=>{
        p.json().then(goals =>{
          goals.forEach(goal =>{
            var g = new Goal(goal.username,goal.naziv,goal.xp,goal.tasksDone);
            this.goals.push(g);
          });
          let gDiv = document.querySelector(".goalsDiv");
          this.DrawGoals(gDiv);
        });
      });
  }

  DrawFeed(host)
  {
    while(host.firstChild)
      host.remove(host.firstChild);

    fetch("http://localhost:5191/Post/GetPosts/"+this.currentUser.username)
    .then(p =>{
      p.json().then(messages =>{
        messages.forEach(message =>{
          let divForMessage = document.createElement("div");
          divForMessage.className = "divForMessage";
          host.appendChild(divForMessage);

          let lMessage = document.createElement("label");
          lMessage.innerHTML = message;
          divForMessage.appendChild(lMessage);
        });
      });
    });
  }

  async FetchTasks(goalname)
  {
    return fetch("http://localhost:5191/Task/GetTasksOfGoal/"+this.currentUser.username+"/"+goalname);
  }

  CompleteTask(taskName,goalname)
  {
    let completedTask = new Task(this.currentUser.username,goalname,taskName,0);

    fetch("http://localhost:5191/Task/CompleteTask/"+this.currentUser.username,{
      method: "delete",
      headers: {'Content-Type' : 'application/json'},
      body: JSON.stringify(completedTask)
    }).then(p=>{
      console.log(p);
     
    });
  }
  AddTask(host,goalName)
  {
    let addTaskDiv = document.createElement("div");
    host.appendChild(addTaskDiv);

    let titleRow = document.createElement("div");
    titleRow.className = "subTitle";
    addTaskDiv.appendChild(titleRow);

    let AddTaskLabel = document.createElement("label");
    AddTaskLabel.innerHTML = "Add new task";
    titleRow.appendChild(AddTaskLabel);

    let submitRow = document.createElement("div");
    submitRow.className = "submitTaskDiv";
    addTaskDiv.appendChild(submitRow);

    let taskNameLab = document.createElement("label");
    taskNameLab.innerHTML = "Name:";
    submitRow.appendChild(taskNameLab);

    let inputName = document.createElement("input");
    submitRow.appendChild(inputName);
  
    taskNameLab = document.createElement("label");
    taskNameLab.innerHTML = "XP:";
    submitRow.appendChild(taskNameLab);

    let inputXP = document.createElement("input");
    submitRow.appendChild(inputXP);

    let addButton = document.createElement("button");

    addButton.innerHTML = "Add task";
    addButton.classList = "addTaskBtn"
    addButton.onclick = (ev) =>{
      if(inputXP.value < 0 || inputXP.value >999)
      {
        alert("XP must be between 1 and 999!");
        return;
      }
      if(isNaN(inputXP.value))
      {
        alert("XP must be number!");
        return;
      }
      let task = new Task(this.currentUser.username,goalName,inputName.value,inputXP.value);
      fetch("http://localhost:5191/Task/AddTask/"+this.currentUser.username,{
        method: "post",
        headers: {'Content-Type' : 'application/json'},
        body: JSON.stringify(task)
      }).then(
        p=>{
          let profilePage = new ProfilePage(this.currentUser,this.kont);
          profilePage.Draw();
        }
      )
    }
    submitRow.appendChild(addButton);
  }

  CompleteGoal(host,goalName)
  {
    let completeGoalBtn = document.createElement("button");
    completeGoalBtn.innerHTML = "COMPLETE GOAL";
    completeGoalBtn.className = "completeGoalBtn";
    host.appendChild(completeGoalBtn);
    completeGoalBtn.onclick = (ev) =>{
      fetch("http://localhost:5191/Goal/CompleteGoal/"+this.currentUser.username+"/"+goalName,{
      method: "delete",
      headers: {'Content-Type' : 'application/json'}
    }).then(p=>{
      let profilePage = new ProfilePage(this.currentUser,this.kont);
      profilePage.Draw();
    });
    }
  }

  DeleteGoal(host,goalName)
  {
    let completeGoalBtn = document.createElement("button");
    completeGoalBtn.innerHTML = "DELETE GOAL";
    completeGoalBtn.className = "deleteGoalBtn";
    host.appendChild(completeGoalBtn);
    completeGoalBtn.onclick = (ev) =>{
      fetch("http://localhost:5191/Goal/DeleteGoal/"+this.currentUser.username+"/"+goalName,{
      method: "delete",
      headers: {'Content-Type' : 'application/json'}
    }).then(p=>{
      let profilePage = new ProfilePage(this.currentUser,this.kont);
      profilePage.Draw();
    });
    }
  }

  //Mozda da popravim
  AddNewGoal()
  {
    let goalIn = document.querySelector(".goalInput");
    fetch("http://localhost:5191/Goal/AddGoal/"+this.currentUser.username+"/"+goalIn.value,{
      method:"post",
      headers: {'Content-Type' : 'application/json'}
    }).then
    (p=>{
      let profilePage = new ProfilePage(this.currentUser,this.kont);
      profilePage.Draw();
    })
  }

  DeleteTask(taskName,goalname)
  {
    let completedTask = new Task(this.currentUser.username,goalname,taskName,0);

    fetch("http://localhost:5191/Task/DeleteTask/"+this.currentUser.username,{
      method: "delete",
      headers: {'Content-Type' : 'application/json'},
      body: JSON.stringify(completedTask)
    }).then(p=>{
      let profilePage = new ProfilePage(this.currentUser,this.kont);
      profilePage.Draw();
    });
  }
}