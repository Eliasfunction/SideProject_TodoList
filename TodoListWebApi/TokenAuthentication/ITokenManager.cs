using System.Security.Claims;

namespace ToDoListApi.TokenAuthentication
{
    public interface ITokenManager
    {
        bool Authenticate(string Username, string Password);
        bool CheckUserInfoInDataBase(string Username, string password);
        public string NewToken(string User);
        public ClaimsPrincipal VerifyToken(string token);
        public bool RefreshTokenCheck(string User,string Refreshtoken);
    }
}