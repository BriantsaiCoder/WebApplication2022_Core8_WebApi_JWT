using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//***** 請自己加入這些 NameSpace *****
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using WebApplication2022_Core8_WebApi_JWT.JwtServices;     // 放在 /JwtServices 目錄底下
using WebApplication2022_Core8_WebApi_JWT.Models_MVC_UserDB;     // 連結資料庫，使用UserTable（第一個範例沒用到資料庫）
using Microsoft.AspNetCore.Cors;
using static WebApplication2022_Core8_WebApi_JWT.JwtServices.TokenService;
//**************************************


namespace WebApplication2022_Core8_WebApi_JWT.Controllers
{
    // 資料來源：https://medium.com/the-innovation/asp-net-core-3-authorization-and-authentication-with-bearer-and-jwt-3041c47c8b1d
    //         （跟上面這一篇相同）https://levelup.gitconnected.com/asp-net-5-authorization-and-authentication-with-bearer-and-jwt-2d0cef85dc5d


    // 請先裝這些 Nuget套件 --  （第一個範例沒用到資料庫）
    //(1) Microsoft.AspNetCore.Authentication
    //(2) Microsoft.AspNetCore.Authentication.JwtBearer  // JWT會用到
    //(3) System.Security.Claims

    [Route("api/[controller]")]    // （注意！這是 API控制器！）
    [ApiController]                        // （注意！這是 API控制器！）
    public class HomeController : ControllerBase
    {

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] UserTable user)
        {
            // 安全改進：添加輸入驗證
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data", errors = ModelState });
            }

            // 安全改進：防止空值攻擊
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                return BadRequest(new { message = "UserName cannot be empty" });
            }

            try
            {
                string token = CreateToken(user);   // 放在 /JwtServices 目錄底下
                return Ok(new
                {
                    //message,    // OK代表成功，Http Status = 200
                    user = new { user.UserId, user.UserName }, // 安全改進：只返回必要的用戶信息
                    token
                });
            }
            catch (Exception ex)
            {
                // 安全改進：不暴露詳細錯誤信息
                return StatusCode(500, new { message = "An error occurred during token generation" });
            }

            // Postman - 請輸入 Body => raw => JSON的內容
            //{
            //    "UserName":"mis2000lab",
            //    "UserId": "100"
            //}

            // Postman - 傳回成果（分成 user與 token兩個區塊）  ( 請參閱圖片  JWT_Postman.jpg 與 JWT_Debug.jpg )
            //{
            //   "user": {
            //        "userId": 100,
            //        "userName": "mis2000lab",
            //        "userSex": null,
            //        "userBirthDay": null,
            //        "userMobilePhone": null
            //    },
            //    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6Im1pczIwMDBsYWIiLCJyb2xlIjoiMTAwIiwibmJmIjoxNjIyOTA2NDk0LCJleHAiOjE2MjI5MTAwOTQsImlhdCI6MTYyMjkwNjQ5NH0.T5uatcvIgEMQjK3vJmh7YIs8V49rvSjNFzt_Wnf8btM"
            //}

        }

        [HttpGet]
        [AllowAnonymous]
        public string Get()
        {
            return "You are Anonymous（匿名登入）.  -- 透過[HttpGet]";
        }

    }
}
