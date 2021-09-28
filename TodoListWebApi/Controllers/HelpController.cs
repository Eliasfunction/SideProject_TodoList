using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListApi.Controllers
{
    [ApiController]
    
    public class HelpController : ControllerBase
    {
        [Route("api/[Controller]")]
        public IActionResult Get()
        {
            string helphelp =" https://github.com/Eliasfunction/SideProjectSelf_TodoListWebApiAndJWT ";
            return Ok(helphelp);
        }
    }
}
