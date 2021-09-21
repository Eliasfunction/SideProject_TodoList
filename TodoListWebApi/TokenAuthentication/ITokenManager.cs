using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ToDoListApi.TokenAuthentication
{
    public interface ITokenManager
    {
        (bool, int) Authenticate(string Username, string Password);
        (List<Token>, bool) GetToken(int UserID);
        (List<Token>, bool) RefreshToken( string Refreshtoken);
        ClaimsPrincipal VerifyToken(string token);
    }
}