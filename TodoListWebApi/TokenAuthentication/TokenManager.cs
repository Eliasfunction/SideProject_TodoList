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
        private readonly IConfiguration _configuration;
        private SqlConnection ToDoListDB;
        // JWTparameter
        private JwtSecurityTokenHandler tokenHandler;
        private byte[] SecretKey;
        private DateTime TokenExpires;//Token 存活時間
        private DateTime RefreshTokenExpires;//Refresh存活時間
        private string ClaimTypes_Name;
        //DataBaseRegulations
        private int MaxTokenQuantity;
        public TokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
            tokenHandler = new JwtSecurityTokenHandler();
            SecretKey = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTparameter:SecretKey"));
            TokenExpires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JWTparameter:TokenExpires_UtcMin"));
            RefreshTokenExpires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JWTparameter:RefreshTokenExpires_UtcMin"));
            ClaimTypes_Name = _configuration.GetValue<string>("JWTparameter:ClaimTypes_Name");
            ToDoListDB = new SqlConnection(_configuration.GetConnectionString("ToDoListDBConnection"));
            MaxTokenQuantity = _configuration.GetValue<int>("DataBaseRegulations:MaxTokenQuantity");
        }
        public (bool, int) Authenticate(string Username, string Password)
        {        //check User&password  then return  AuthValue &Userid
            string UserInfoSelect = @"SELECT TOP (1) UserID,passwordHash FROM UserInfo where UserName =@Username";
            SqlCommand SearchCommand = new SqlCommand(UserInfoSelect, ToDoListDB);
            SearchCommand.Parameters.Add("@Username", SqlDbType.VarChar).Value = Username;

            try
            {
                ToDoListDB.Open();
                SqlDataReader SqlData = SearchCommand.ExecuteReader();
                if (SqlData.HasRows && SqlData.Read())
                {
                    int UserID = Convert.ToInt32(SqlData["UserID"]);
                    string passwordHash = SqlData["passwordHash"].ToString();
                    SqlData.Close();
                    ToDoListDB.Close();
                    return ((BCrypt.Net.BCrypt.Verify(Password, passwordHash), UserID));
                }
                SqlData.Close();
                ToDoListDB.Close();
            }
            //輸出SQL有關的錯誤訊息 &其他錯誤訊息
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }

            return (false, 0);

        }
        public (List<Token>, bool) GetToken(int UserID)
        {
            List<Token> tokens = new List<Token>();

            string TokenValue = NewToken(TokenExpires);
            string RefreshTokenValue = NewToken(RefreshTokenExpires);

            Token value = new Token
            {
                TokenValue = TokenValue,
                RefreshTokenValue = RefreshTokenValue,
                ExpiryDate = TokenExpires
            };
            tokens.Add(value);
            //add token value in database
            bool StoredSUCC = New_Token_Stored(TokenValue, RefreshTokenValue,TokenExpires, UserID);
            return (tokens, StoredSUCC);
        }
        private string NewToken(DateTime Expires)
        {   //生成TOKEN
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, ClaimTypes_Name) }),
                Expires = Expires, 
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(SecretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtString = tokenHandler.WriteToken(token);
            return jwtString;
        }
        private bool New_Token_Stored(string TokenValue, string RefreshTokenValue, DateTime Expires, int UserID)
        {
            string TokenInsert = //check token max quantity //Max = 2
              @"IF (SELECT COUNT(TokenID) FROM Token WHERE UserID=@UserID) >= @MaxTokenQuantity
                    UPDATE TOP (1) Token SET TokenValue=@TokenValue , RefreshTokenValue=@RefreshTokenValue , ExpiresTime=@ExpiresTime  WHERE 
                        TokenID = (SELECT TOP (1) TokenID FROM Token WHERE UserID=@UserID ORDER BY ExpiresTime ASC)
                ELSE
                INSERT INTO Token (TokenValue , RefreshTokenValue , ExpiresTime , UserID) VALUES(@TokenValue , @RefreshTokenValue, @ExpiresTime, @UserID)";
            SqlCommand Command = new SqlCommand(TokenInsert, ToDoListDB);
            Command.Parameters.Add("@MaxTokenQuantity", SqlDbType.Int).Value = MaxTokenQuantity;
            Command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
            Command.Parameters.Add("@TokenValue", SqlDbType.VarChar).Value = TokenValue;
            Command.Parameters.Add("@RefreshTokenValue", SqlDbType.VarChar).Value = RefreshTokenValue;
            Command.Parameters.Add("@ExpiresTime", SqlDbType.DateTime).Value = Expires;
            try
            {
                ToDoListDB.Open();
                int RowsAffected =Command.ExecuteNonQuery();
                ToDoListDB.Close();
                if (RowsAffected != 0)
                    return true;
            }
            //輸出SQL有關的錯誤訊息 &其他錯誤訊息
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }
            return false;
        }
        public (List<Token>, bool) RefreshToken(string Refreshtoken)
        {
            List<Token> tokens = new List<Token>();
            string TokenValue = NewToken(TokenExpires);
            string RefreshTokenValue = NewToken(RefreshTokenExpires);

            Token value = new Token
            {
                TokenValue = TokenValue,
                RefreshTokenValue = RefreshTokenValue,
                ExpiryDate = TokenExpires
            };
            tokens.Add(value);
            bool StoredSUCC = Refresh_Token_Stored(TokenValue, RefreshTokenValue, TokenExpires, Refreshtoken);
            return (tokens, StoredSUCC);
        }
        private bool Refresh_Token_Stored(string TokenValue, string RefreshTokenValue, DateTime Expires, string Refreshtoken)
        {        //update tokens value in token table 
            string RefreshUpdate = @"UPDATE TOP (1) Token 
                                SET TokenValue=@TokenValue , RefreshTokenValue=@RefreshTokenValue , ExpiresTime=@ExpiresTime  
                                WHERE TokenID = (SELECT TokenID FROM Token WHERE RefreshTokenValue =@Refreshtoken)";
            SqlCommand Command = new SqlCommand(RefreshUpdate, ToDoListDB);
            Command.Parameters.Add("@TokenValue", SqlDbType.VarChar).Value = TokenValue;
            Command.Parameters.Add("@RefreshTokenValue", SqlDbType.VarChar).Value = RefreshTokenValue;
            Command.Parameters.Add("@ExpiresTime", SqlDbType.DateTime).Value = Expires;
            Command.Parameters.Add("@Refreshtoken", SqlDbType.VarChar).Value = Refreshtoken;
            try
            {
                ToDoListDB.Open();
                ///避免發生TokenID不存在的狀況 檢視受影響行數 
                ///受影響表示成功 無影響表示用戶HEADERS Refreshtoken值不存在於資料庫
                int RowsAffected = Command.ExecuteNonQuery();
                ToDoListDB.Close();
                if (RowsAffected != 0)
                    return true;
            }
            //輸出SQL有關的錯誤訊息 &其他錯誤訊息
            catch (SqlException ex) { System.Diagnostics.Debug.WriteLine(ex); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex); }
            return false;
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
        }
    }
}
