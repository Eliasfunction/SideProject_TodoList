using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace ToDoListApi.TokenAuthentication
{
    public class TokenManager : ITokenManager
    {
        private JwtSecurityTokenHandler tokenHandler;
        private byte[] SecretKey;
        private readonly IConfiguration _configuration;
        SqlConnection ToDoListDB;
        public TokenManager(IConfiguration configuration)
        {
            tokenHandler = new JwtSecurityTokenHandler();
            SecretKey = Encoding.ASCII.GetBytes("abcdefghabcdefghabcdefghabcdefgh");//32byte
            _configuration = configuration;
            ToDoListDB = new SqlConnection(_configuration.GetConnectionString("ToDoListDBConnection"));
        }
        public bool Authenticate(string Username, string Password)
        {
            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) &&
                CheckUserInfoInDataBase(Username, Password))
                return true;
            else
                return false;
        }
        public bool CheckUserInfoInDataBase(string Username, string password)
        {
            //測試用
            if (Username.ToLower() == "admin" && password == "password") return true;

            string Select = @"SELECT passwordHash FROM UserInfo where UserName =@Username";
            SqlCommand SearchCommand = new SqlCommand(Select, ToDoListDB);
            SearchCommand.Parameters.Add("@Username", SqlDbType.VarChar).Value = Username;
            try
            {
                ToDoListDB.Open();
                SqlDataReader SqlData = SearchCommand.ExecuteReader();
                if (SqlData.HasRows && SqlData.Read())
                {
                    string  passwordHash = SqlData["passwordHash"].ToString();
                    SqlData.Close();
                    ToDoListDB.Close();
                    return (BCrypt.Net.BCrypt.Verify(password, passwordHash));
                }
                SqlData.Close();
                ToDoListDB.Close();
                return false;
            }
            //輸出SQL有關的錯誤訊息 &其他錯誤訊息
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }
            
            return false;
        }
        
        public string NewToken(string User)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name,"TOKENTESTING")}),
                Expires =DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(SecretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtString = tokenHandler.WriteToken(token);
            //生成新TOKEN
            //add new token in list
            //測試用 只存LIST 不存資料庫
            if(User.ToLower() == "admin")
            { 
                return jwtString;
            }/*
            //add Token Value in datebase
            string update = @"UPDATE UserInfo SET TokenValue=@TokenValue,RefreshTokenValue=@RefreshTokenValue";
            SqlCommand SearchCommand = new SqlCommand(update , ToDoListDB);
            SearchCommand.Parameters.Add("@TokenValue", SqlDbType.VarChar).Value = jwtString.GetHashCode();
            //SearchCommand.Parameters.Add("@RefreshTokenValue", SqlDbType.VarChar).Value = token.RefreshTokenValue;
            try
            {
                ToDoListDB.Open();
                SearchCommand.ExecuteNonQuery();
                ToDoListDB.Close();
            }
            //輸出SQL有關的錯誤訊息 &其他錯誤訊息
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }*/

            return jwtString;
        }

        public ClaimsPrincipal VerifyToken(string token)
        {
            var claims = tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(SecretKey),
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
            return claims;
                /*
            if (listTokens.Any(x => x.TokenValue == token && x.ExpiryDate > DateTime.Now))
                return true;
            else
                return false;*/
        }
        public bool RefreshTokenCheck(string User,string Refreshtoken)
        {
            return false;
            /*
            if (listTokens.Any(x => x.RefreshTokenValue == Refreshtoken && x.ExpiryDate > DateTime.Now))
            {
                ///刪除的部分之後要改寫成非同步
                listTokens.RemoveAll(x => x.RefreshTokenValue == Refreshtoken || x.ExpiryDate < DateTime.Now);
                return true;
            }
            else
            {   
                listTokens.RemoveAll(x => x.ExpiryDate < DateTime.Now);
                return false;
            }*/
        }
    }
}
