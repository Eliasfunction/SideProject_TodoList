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
        public IActionResult Authenticate() //string User, string Pwd
        {   //check userinfo & return USERID

            string User = Request.Headers["Username"];
            string Pwd = Request.Headers["Password"];

            if (string.IsNullOrWhiteSpace(User) || string.IsNullOrWhiteSpace(Pwd))
            {
                ModelState.AddModelError("Parameters ERROR", "Required Parameters are not met.");
                return Unauthorized(ModelState);
            }
            //檢查登入 返回UserID
            (bool AuthPass, int UserID) = tokenManager.Authenticate(User, Pwd);
            if (AuthPass)
            {
                //返回TOKEN值 & 資料庫寫入狀態
                (List<Token> tokens, bool StoredSUCC) = tokenManager.GetToken(UserID);
                //資料庫寫入成功
                if (StoredSUCC)
                    return Ok(new { tokens });
                //資料庫寫入失敗
                ModelState.AddModelError("Server Error", "TOKEN can be distributed but cannot be verified");
                return Unauthorized(ModelState);
            }
            else
            {
                ModelState.AddModelError("Unauthorized", "You Are Not Unauthorized.");
                return Unauthorized(ModelState);
            }
        }

        [HttpPost]
        [RefreshTokenAuthenticationFilter]
        [Route("/api/[Controller]/Refresh")]
        public IActionResult Refresh()
        {
            string Refresh = Request.Headers["RefreshToken"];

            //用Refresh獲取新TOKEN
            (List<Token> tokens, bool StoredSUCC) = tokenManager.RefreshToken(Refresh);
            //資料庫寫入成功
            if (StoredSUCC)
                return Ok(new { tokens });
            //資料庫寫入失敗
            ModelState.AddModelError("Error", "TOKEN can be distributed but cannot be verified,Please log in again");
            return Unauthorized(ModelState);
        }
    }
}
