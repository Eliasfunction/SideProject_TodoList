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
    [Route("/api/[Controller]")]
    //[Authorize]
    public class TodoController : ControllerBase
    {
        private readonly IToDoDBmanager toDoDBmanager;
        public TodoController(IToDoDBmanager toDoDBmanager)
        {
            this.toDoDBmanager = toDoDBmanager;
        }
        [HttpGet]
        public IActionResult GetTning()
        {
            bool Invisible = false;
            List<Thing> things = toDoDBmanager.GetThing(Request.Headers["Authorization"], Invisible);
            return Ok(new { things });
            //return Ok(toDoDBmanager.GetThing(Token));
        }
        [HttpPost]
        public IActionResult CreateTning([FromBody] Thing thing)
        {
            if (toDoDBmanager.NewThing(thing, Request.Headers["Authorization"]))
            {
                return Ok("Create Success");
            }
            return Ok("Create failed");
        }
        [HttpPut]
        public IActionResult UpdateThing([FromBody] Thing thing)
        {
            if (toDoDBmanager.ChangeThing(thing, Request.Headers["Authorization"]))
            {
                return Ok("Update Success");
            }
            return Ok("Update failed");
        }
        [HttpDelete]
        public IActionResult Delete([FromBody] RecycleThing todoId)
        {
            bool Invisible = true;
            if (toDoDBmanager.Recycle(todoId, Request.Headers["Authorization"], Invisible))
            {
                return Ok("Delete Success");
            }
            return Ok("Delete failed");
        }
    }
}
