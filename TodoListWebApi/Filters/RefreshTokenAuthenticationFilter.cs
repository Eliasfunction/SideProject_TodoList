﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoListApi.TokenAuthentication;

namespace ToDoListApi.Filters
{
    public class RefreshTokenAuthenticationFilter : Attribute , IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var tokenManager = (ITokenManager)context.HttpContext.RequestServices.GetService(typeof(ITokenManager));

            var result = true;
            //沒有Headers值Authorization =>false
            if (!context.HttpContext.Request.Headers.ContainsKey("Refreshtoken"))
                result = false;
            string token = string.Empty;
            if (result)
            {   //抓Headers值Authorization 為token
                token = context.HttpContext.Request.Headers["Refreshtoken"].FirstOrDefault();
                //
                try
                {
                    var ClaimPrinciple = tokenManager.VerifyToken(token);
                }
                catch(Exception ex)
                {
                    result =false;
                    context.ModelState.AddModelError("Unauthorized", ex.ToString());
                }
            }
            if (!result) context.Result = new UnauthorizedObjectResult(context.ModelState);
        }
    }
}
