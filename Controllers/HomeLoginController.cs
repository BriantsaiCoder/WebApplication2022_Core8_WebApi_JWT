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
//***************************************
using WebApplication2022_Core8_WebApi_JWT.Models_MVC_UserDB;          //  使用UserTable
using WebApplication2022_Core8_WebApi_JWT.Models_MVC_UserLogin;     // 連結資料庫  // *** 記得自己修改一下
using static WebApplication2022_Core8_WebApi_JWT.JwtServices.TokenService;
//**************************************


namespace WebApplication2022_Core8_WebApi_JWT.Controllers
{
    // 資料來源：https://medium.com/the-innovation/asp-net-core-3-authorization-and-authentication-with-bearer-and-jwt-3041c47c8b1d

    // 請先裝這些 Nuget套件 -- 
    //(1) Microsoft.AspNetCore.Authentication
    //(2) Microsoft.AspNetCore.Authentication.JwtBearer  // JWT會用到
    //(3) Microsoft.EntityFrameworkCore               // 資料庫會用到
    //(4) Microsoft.EntityFrameworkCore.Tools    // 資料庫會用到
    //(5) Microsoft.EntityFrameworkCore.SqlServer  // SQL Server會用到
    //(6) System.Security.Claims

    [Route("api/[controller]")]    // （注意！這是 API控制器！）
    [ApiController]                        // （注意！這是 API控制器！）
    public class HomeLoginController : ControllerBase
    {
        #region    //*****  連結 MVC_UserLogin 資料庫 ***** ( //*** (1) 請自己修改   )
        private readonly MVC_UserLoginContext _db;   // *** 記得自己修改一下

        public HomeLoginController(MVC_UserLoginContext context)   // *** 記得自己修改一下，修改「控制器名稱」、「資料庫名稱」
        {
            _db = context;
        }
        #endregion


        [HttpPost]
        //[Route("login")]
        [AllowAnonymous]
        public IActionResult Post([FromBody] DbUser model)
        {
            // 安全改進：添加輸入驗證
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data", errors = ModelState });
            }

            // 安全改進：防止空值攻擊
            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.UserPassword))
            {
                return BadRequest(new { message = "UserName and Password cannot be empty" });
            }

            try
            {
                // 連結DB的 UserTable資料表，請參考 （MVC 入門 第三天課程）的 Details動作，共有四種寫法。
                // 請輸入UserName = 111  與 UserPassword = 111
                //*******************************************************************************************************
                //*** (2) 請自己修改
                var ListOne = from m in _db.DbUsers
                              where m.UserName == model.UserName && m.UserPassword == model.UserPassword
                              select m;
                            //select new { UserName = m.UserName, UserId = m.UserRank };    
                            // 修改欄位名稱，https://stackoverflow.com/questions/128277/linq-custom-column-names

                var user = ListOne.FirstOrDefault();  // 執行上面的查詢句，得到 "第一筆" 結果。
                //*** (3) 改成 var。因為你把上面的select修改後，只輸出兩個欄位，無法跟DBUser匹配，只能使用"匿名類型 (anonymous type)"。
                //*******************************************************************************************************

                if (user == null)
                    return NotFound(new { message = "查無此人，可能帳號或密碼有錯！" });
                else
                {
                    //**********************************************************
                    //*** (4) 因為產生的那支程式 TokenService 寫死了，只能輸入 CreateToken(UserTable user)，所以只能配合他。
                    UserTable ut = new UserTable
                    {
                        UserName = user.UserName,
                        UserId = (int)(user.UserRank ?? 0)    // 強制轉成 int，並處理 null 值
                    };

                    string token = CreateToken(ut);   //*** (5) 請自己修改   // 程式放在 /JwtServices 目錄底下
                    //**********************************************************
                    return Ok(new
                    {
                        //message,
                        user = new { user.UserName, UserRank = user.UserRank }, // 安全改進：只返回必要信息，不包含密碼
                        token     // OK代表成功，Http Status = 200
                    });
                }
            }
            catch (Exception ex)
            {
                // 安全改進：不暴露詳細錯誤信息
                return StatusCode(500, new { message = "An error occurred during authentication" });
            }
            // (1) Postman - 請自己動手輸入 Body => raw => JSON的內容（下面範例，帳號 / 密碼有錯）
            //            {
            //            "UserName":"123X",
            //            "UserPassword": "123X"
            //            }

            //            // Postman - 傳回成果  ( 請參閱圖片  HomeWork-1.jpg )
            //            {
            //                "message": "查無此人，可能帳號或密碼有錯！"
            //            }


            // (2) Postman - 請自己動手輸入 Body => raw => JSON的內容 （下面範例，帳號 / 密碼正確）
            //必須是真正在 db_user資料表裡面的紀錄
            //        {
            //            "UserName":"123",
            //            "UserPassword": "123"
            //        }
            //        //            // Postman - 傳回成果  ( 請參閱圖片  HomeWork-2.jpg )
        }

        [HttpGet]
        //[Route("anonymous")]    // 資料來源 (REST Api 的屬性路由)  https://docs.microsoft.com/zh-tw/aspnet/core/mvc/controllers/routing?view=aspnetcore-5.0
        [AllowAnonymous]
        public string Anonymous()
        {
            return "*****回家作業*****  [HomeLogin] API控制器 -- You are Anonymous（匿名登入）.  -- 透過[HttpGet]";
        }


        //==========================================================
        [HttpPost]
        [Route("auth")]
        [Authorize]       // *** 取得 token 的人才能使用這裡的方法 ***     
        public string Authenticated() => String.Format("*****回家作業*****  [HomeLogin] API控制器 -- Authenticated - 歡迎光臨，尊貴的 {0} 閣下", User.Identity.Name);
        //   ( 請參閱圖片  HomeWork-3.jpg )

        [HttpPost]
        [Route("test")]
        [Authorize(Roles = "1")]
        public string Tester()
        {
            return "*****回家作業*****  [HomeLogin] API控制器 -- You are a Tester -- Roles = 1";
            //   ( 請參閱圖片  HomeWork-4.jpg )
        }


    }
}
