using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//***** 請自己加入這些 NameSpace *****
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;   // 請自己安裝 NuGet套件
using System.Text;

//**************************************
using WebApplication2022_Core8_WebApi_JWT.Models_MVC_UserLogin;     // HomeWork（回家作業）會用到這一段
//**************************************

namespace WebApplication2022_Core8_WebApi_JWT.JwtServices
{
    /// <summary>
    /// 搭配「回家作業」做的修改，僅供參考。
    /// 資料來源：https://medium.com/the-innovation/asp-net-core-3-authorization-and-authentication-with-bearer-and-jwt-3041c47c8b1d
    /// </summary>

    public static class TokenService2   // 注意 關鍵字 static
    {
        public static string CreateToken(DbUser user)
        {   //                                                    ********* HomeWork（回家作業）需自己修改
            var key = Encoding.UTF8.GetBytes(LegacySettings.Secret);  
            var descriptor = new SecurityTokenDescriptor
            {
                // 在token裡面搭配 Claims來存放「帳號」、「角色、群組」等個人資料。
                Subject = new ClaimsIdentity(new Claim[]     {
                    new Claim(ClaimTypes.Name, user.UserName?.ToString() ?? string.Empty),
                    new Claim(ClaimTypes.Role, user.UserRank?.ToString() ?? string.Empty)   // 這名使用者的角色、群組
                    //                                                     ***************** HomeWork（回家作業）需自己修改
                }),

                Expires = DateTime.UtcNow.AddHours(1.0),   // 使用期限，設定一個小時就過期。
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                // 簽章。 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);  // 最後，傳回 token字串。
        }





    }
}
