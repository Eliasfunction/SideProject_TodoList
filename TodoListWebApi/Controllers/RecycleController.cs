using Core.Models;
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
    public class RecycleController : ControllerBase
    {
        private readonly IToDoDBmanager toDoDBmanager;
        public RecycleController(IToDoDBmanager toDoDBmanager)
        {
            this.toDoDBmanager = toDoDBmanager;
        }
        [HttpGet]
        public IActionResult Get()
        {
            bool Invisible = true;
            List<Thing> things = toDoDBmanager.GetThing(Request.Headers["Authorization"], Invisible);
            return Ok(new { things });
        }
        [HttpPut]
        public IActionResult RcoverThing([FromBody] RecycleThing todoId)
        {
            bool Invisible = false;
            if (toDoDBmanager.Recycle(todoId, Request.Headers["Authorization"], Invisible))
            {
                return Ok("Delete Success");
            }
            return Ok("Delete failed");
        }
        [HttpDelete]
        public IActionResult Delete([FromBody] RecycleThing todoId)
        {
            if (toDoDBmanager.Delete(todoId, Request.Headers["Authorization"]))
            {
                return Ok("Delete(Erase) Success");
            }
            return Ok("Delete(Erase) failed");
        }
    }
}
