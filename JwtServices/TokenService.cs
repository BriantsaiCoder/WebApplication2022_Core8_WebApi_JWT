using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//***** 請自己加入這些 NameSpace *****
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;   // 請自己安裝 NuGet套件
using System.Text;

using WebApplication2022_Core8_WebApi_JWT.Models_MVC_UserDB;  // 連結資料庫，使用UserTable
//**************************************

namespace WebApplication2022_Core8_WebApi_JWT.JwtServices
{
    // 資料來源：https://medium.com/the-innovation/asp-net-core-3-authorization-and-authentication-with-bearer-and-jwt-3041c47c8b1d
    public static class TokenService   // 注意 關鍵字 static
    {
        //private const double EXPIRE_HOURS = 1.0;       // token一個小時就會過期        

        public static string CreateToken(UserTable user)
        {
            var key = Encoding.UTF8.GetBytes(LegacySettings.Secret);  
            // 使用更安全的密鑰並改用 LegacySettings

            var descriptor = new SecurityTokenDescriptor
            {
                // 如果您沒有學過之前的「 .NET Core會員登入 (ClaimsIdentity)」課程，這裡就卡住了。
                // 在token裡面搭配 Claims來存放「帳號」、「角色、群組」等個人資料。
                Subject = new ClaimsIdentity(new Claim[]     {
                    new Claim(ClaimTypes.Name, user.UserName?.ToString() ?? string.Empty),
                    new Claim(ClaimTypes.Role, user.UserId.ToString())   // 將來可用於存放  這名使用者的角色、群組 (Role)

                    // 如果您要用 JwtRegisteredClaim，請看 https://blog.poychang.net/authenticating-jwt-tokens-in-asp-net-core-webapi/
                    //new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub,  user.UserName),
                    //new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Jti,     user.UserId.ToString())
                }),

                Expires = DateTime.UtcNow.AddHours(1.0),   // 使用期限，多久會過期？
                // token 一個小時就過期。也可寫成 DateTime.UtcNow.AddHours(EXPIRE_HOURS)
                // token不像Session可以強制註銷、清除（Session.Abandon()）。只能等待它過期失效。

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                // 簽章。  透過  Hash雜湊運算（單向打亂，無法復原）
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);  // 最後，傳回 token字串。內容如下（以英文句號（.）區隔，分成三個段落）：
            // "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
            //                  eyJ1bmlxdWVfbmFtZSI6Im1pczIwMDBsYWIiLCJyb2xlIjoiMTAwIiwibmJmIjoxNjIyOTA2NDk0LCJleHAiOjE2MjI5MTAwOTQsImlhdCI6MTYyMjkwNjQ5NH0.
            //                  T5uatcvIgEMQjK3vJmh7YIs8V49rvSjNFzt_Wnf8btM"
        }





    }
}
