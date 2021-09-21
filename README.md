# SideProject_TodoListWebApi
Authenticate Token &amp;ToDoList CRUD
DB is name TodoListWebApiDB-Backup in root directory.

A   Authenticate 
Get Token (Get Token & RefreshToken)
        url=> api/Authenticate
        Headers =>  "Username" & "Password"

RefreshToken (Get New Token & RefreshToken)
        url=> api/Authenticate/Refresh 
        Headers =>  "Refreshtoken"
        
Now must be add Token Headers ,any request.

B   Used Todolist    
        url => api/Todo (need Headers => "Authorization")
                Get    ()
                        ==> GetTodolist    
                Post   (need Body=> Title & description) 
                        ==> Create Success OR Create failed    
                Put    (need Body=> Title & description , Finish ,toDoId)
                        ==>Update Success OR Update failed    
                Delete (need Body=> todoId)
                        ==>Delete Success OR Delete failed
C Database Design
        ER_MAP IS
![image](https://github.com/Eliasfunction/SideProjectSelf_TodoListWebApiAndJWT/blob/master/ReadmeIMG/ER_MAP.png)
        Database_Tools IS
![image](https://github.com/Eliasfunction/SideProjectSelf_TodoListWebApiAndJWT/blob/master/ReadmeIMG/Database%20Tools.png)
