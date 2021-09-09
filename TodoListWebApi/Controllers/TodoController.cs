using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoListApi.Filters;

namespace ToDoListApi.Controllers
{
    [TokenAuthenticationFilter]
    [ApiController]
    [Route("/api/[Controller]/{UserName}")]
    //[Authorize]
    public class TodoController : ControllerBase
    {
        private readonly IToDoDBmanager toDoDBmanager;
        public TodoController(IToDoDBmanager toDoDBmanager)
        {
            this.toDoDBmanager = toDoDBmanager;
        }
        [HttpGet]
        public IActionResult GetTning(string UserName)
        {
            return Ok("succ");
            /*
            List<Thing> things = toDoDBmanager.GetThing(UserName);
            return Ok(new { things });*/
        }
        [HttpPost]
        public IActionResult CreateTning([FromBody] Thing thing,string UserName)
        {
            if(toDoDBmanager.NewThing(thing, UserName))
            {
                return Ok("Create Success");
            }
            return Ok("Create failed");
        }
        [HttpPut]
        public IActionResult UpdateThing([FromBody] Thing thing, string UserName)
        {
            if(toDoDBmanager.ChangeThingByTitle(thing, UserName))
            {
                return Ok("Update Success");
            }
            return Ok("Update failed");
        }
        [HttpDelete]
        public IActionResult Delete([FromBody] RecycleThing title, string UserName)
        {
            if (toDoDBmanager.RecycleByTitle(title, UserName))
            {
                return Ok("Delete Success");
            }
            return Ok("Delete failed");
        }
    }
}
