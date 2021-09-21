# SideProject_TodoListWebApi
Authenticate Token &amp;ToDoList CRUD
DB is name TodoListWebApiDB-Backup in root directory.

A   Authenticate Pass   
Get Token => api/Authenticate?User=[]&Pwd=[] , then you Get Token & RefreshToken    
Refresh   => api/Authenticate/Refresh?User=[]&Refresh=[]    
Now must be add Token Headers ,any request.

B   used Todolist    
url = ( api/Todo/[User] ) => Example ( api/Todo/Test1 )
Get    ( )                              ,GetTodolist    
Post   (need Body=> Title & description), =>Create Success OR Create failed    
Put    (need Body=> Title & description), =>Update Success OR Update failed    
Delete (need Body=> Title)              , =>Delete Success OR Delete failed


Wearning <Any Title cannot be repeated in ToDoList & Must be add Token Headers ,any Request.>
        
