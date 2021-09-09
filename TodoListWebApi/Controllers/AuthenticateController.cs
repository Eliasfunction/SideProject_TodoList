using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoListApi.Filters;
using ToDoListApi.TokenAuthentication;

namespace ToDoListApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly ITokenManager tokenManager;
        public AuthenticateController(ITokenManager tokenManager)
        {
            this.tokenManager = tokenManager;
        }

        [HttpGet]
        public IActionResult Authenticate(string User,string Pwd)
        {
            if(tokenManager.Authenticate( User, Pwd))
                return Ok(new { Token = tokenManager.NewToken(User) });
            else
            { 
                ModelState.AddModelError("Unauthorized", "You Are Not Unauthorized.");
                return Unauthorized(ModelState);
            }
        }
        /*
        [HttpPost]
        [Route("/api/[Controller]/Refresh")]
        [TokenAuthenticationFilter]
        public IActionResult Refresh(string User,string Refresh)
        {
            if (tokenManager.RefreshTokenCheck(User,Refresh))
                return Ok(new { Token = tokenManager.NewToken(User) });
            else
            {
                ModelState.AddModelError("Errors", "Service is offline");
                return Unauthorized(ModelState);
            }
            
        }*/
    }
}
