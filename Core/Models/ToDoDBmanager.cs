using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Core.Models
{
    public class ToDoDBmanager : IToDoDBmanager
    {
        private readonly IConfiguration _configuration;
        private SqlConnection ToDoListDB;
        public ToDoDBmanager(IConfiguration configuration)
        {
            _configuration = configuration;
            ToDoListDB = new SqlConnection(configuration.GetConnectionString("ToDoListDBConnection"));
        }

        public List<Thing> GetThing(string Token)
        {
            List<Thing> things = new List<Thing>();
            
            string select = @"SELECT * FROM ToDo 
                                WHERE UserId=(SELECT UserID FROM Token WHERE TokenValue =@TokenValue) 
                                AND Recycle=@Recycle";
            SqlCommand Search = new SqlCommand(select, ToDoListDB);
            Search.Parameters.Add("@Recycle", SqlDbType.Bit).Value = false;
            Search.Parameters.Add("@TokenValue", SqlDbType.VarChar).Value = Token;
            try
            {
                ToDoListDB.Open();
                SqlDataReader SqlData = Search.ExecuteReader();
                if (SqlData.HasRows)
                {
                    while (SqlData.Read())
                    {
                        Thing thing = new Thing
                        {
                            TodoId= SqlData.GetInt32(SqlData.GetOrdinal("ToDoId")),
                            Title = SqlData.GetString(SqlData.GetOrdinal("Title")),
                            Description = SqlData.GetString(SqlData.GetOrdinal("Description")),
                            AddDate = SqlData.GetDateTime(SqlData.GetOrdinal("AddDate")),
                            Finish = SqlData.GetBoolean(SqlData.GetOrdinal("Finish"))
                        };
                        things.Add(thing);
                    }
                }
                SqlData.Close();
                ToDoListDB.Close();
            }
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }

            return things;
        }
        public bool NewThing(Thing thing,string Token)
        {
            string insertinto = @"INSERT INTO ToDo ( Title , Description , UserId)
                        VALUES (@Title , @Description, (SELECT UserID FROM Token WHERE TokenValue =@TokenValue))";
            SqlCommand insert = new SqlCommand(insertinto, ToDoListDB);
            insert.Parameters.Add("@Title", SqlDbType.NVarChar).Value = thing.Title;
            insert.Parameters.Add("@Description", SqlDbType.NVarChar).Value = thing.Description;
            insert.Parameters.Add("@TokenValue", SqlDbType.VarChar).Value = Token;
            try
            {
                ToDoListDB.Open();
                int RowsAffected = insert.ExecuteNonQuery();
                ToDoListDB.Close();
                if (RowsAffected != 0)
                    return true;
            }
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }

            return false;
        }
        public bool ChangeThing(Thing thing, string Token)
        {
            string UPDATE = @"UPDATE TOP (1) ToDo SET Title=@Title , Description=@Description , Finish=@Finish
                                Where ToDoId=@ToDoId
                                AND UserId = (SELECT UserID FROM Token WHERE TokenValue =@TokenValue)";
            SqlCommand update = new SqlCommand(UPDATE, ToDoListDB);
            update.Parameters.Add("@Title", SqlDbType.NVarChar).Value = thing.Title;
            update.Parameters.Add("@Description", SqlDbType.NVarChar).Value = thing.Description;
            update.Parameters.Add("@Finish", SqlDbType.Bit).Value = thing.Finish;
            update.Parameters.Add("@ToDoId", SqlDbType.Int).Value = thing.TodoId;
            update.Parameters.Add("@TokenValue", SqlDbType.VarChar).Value = Token;

            try
            {
                ToDoListDB.Open();
                int RowsAffected = update.ExecuteNonQuery();
                ToDoListDB.Close();
                if (RowsAffected != 0)
                    return true;

            }
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }

            return false;
        }
        public bool Recycle(RecycleThing todoId, string Token)
        {
            string Recycle = @"UPDATE TOP (1) Todo SET Recycle=@Recycle
                                Where ToDoId=@ToDoId
                                AND UserId = (SELECT UserID FROM Token WHERE TokenValue =@TokenValue)";
            SqlCommand recycle = new SqlCommand(Recycle, ToDoListDB);
            recycle.Parameters.Add("@Recycle", SqlDbType.Bit).Value = true;
            recycle.Parameters.Add("@ToDoId", SqlDbType.Int).Value = todoId.TodoId;
            recycle.Parameters.Add("@TokenValue", SqlDbType.VarChar).Value = Token;
            try
            {
                ToDoListDB.Open();
                int RowsAffected = recycle.ExecuteNonQuery();
                ToDoListDB.Close();
                if (RowsAffected != 0)
                    return true;
            }
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }

            return false;
        }
    }
}
