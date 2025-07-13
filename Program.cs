//****************************************
// 連結資料庫才會用到這一段（DB連結字串）

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebApplication2022_Core8_WebApi_JWT.Models_MVC_UserDB; // （第二個範例才會用到）  連結資料庫
using WebApplication2022_Core8_WebApi_JWT.Models_MVC_UserLogin; // （[回家作業 HomeWork] 才會用到）  連結資料庫
//****************************************
// JWT (json web token) 才會用到這一段
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApplication2022_Core8_WebApi_JWT.JwtServices; // 位於 /JwtServices目錄下，Token源自於此。
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApplication2022_Core8_WebApi_JWT.Filter;
using WebApplication2022_Core8_WebApi_JWT.Models;

//****************************************
// 資料來源：https://medium.com/the-innovation/asp-net-core-3-authorization-and-authentication-with-bearer-and-jwt-3041c47c8b1d
//         （跟上面這一篇相同）https://levelup.gitconnected.com/asp-net-5-authorization-and-authentication-with-bearer-and-jwt-2d0cef85dc5d
//                    https://www.cnblogs.com/nsky/p/10312101.html
//                    https://medium.com/@szaloki/jwt-authentication-between-asp-net-core-and-angular-part-1-asp-net-core-315af889fdce
//                    https://www.c-sharpcorner.com/article/authentication-and-authorization-in-asp-net-5-with-jwt-and-swagger/
//                    https://www.c-sharpcorner.com/article/how-to-use-jwt-authentication-with-web-api/

// 請先裝這些 Nuget套件 --
//(1) Microsoft.AspNetCore.Authentication
//(2) Microsoft.AspNetCore.Authentication.JwtBearer  // JWT會用到
//(3) Microsoft.EntityFrameworkCore               // 資料庫會用到
//(4) Microsoft.EntityFrameworkCore.Tools    // 資料庫會用到
//(5) Microsoft.EntityFrameworkCore.SqlServer  // SQL Server會用到
//(6) System.Security.Claims

// 本範例搭配 HomeController（注意！這是 API控制器！）


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 添加 CORS 服務 - 安全配置
//避免Client端寫JS時，遇見問題「跨原始來源要求 (CORS)  / Access - Control - Allow - Origin」
builder.Services.AddCors(options =>
{
    // 配置更安全的CORS策略 - 限制特定來源
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7000", "http://localhost:5000") // 限制特定來源
            .AllowAnyMethod() // 允許所有HTTP方法（GET、POST、PUT、DELETE等）
            .AllowAnyHeader() // 允許所有標頭
            .AllowCredentials(); // 允許憑證
    });

    // 配置僅允許特定來源的CORS策略
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins("https://example.com", "https://anotherexample.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
    
    // 開發環境的寬鬆策略（僅在開發時使用）
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddControllers();

#region // JWT (json web token) 才會用到這一段 - 安全配置

//********************************************************************
// 這裡需要新增 很多的命名空間，請使用「顯示可能的修正」讓系統自己加上。

// 配置JWT設定
builder.Services.Configure<Settings>(builder.Configuration.GetSection("JwtSettings"));

// 讀取JWT設定以設置認證
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<Settings>();
var key = Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? LegacySettings.Secret);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true; // 在生產環境中要求HTTPS
        options.SaveToken = true;

        // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
        options.IncludeErrorDetails = true; // 預設值為 true

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            
            // 啟用 Issuer 和 Audience 驗證以提高安全性
            ValidateIssuer = true,
            ValidIssuer = jwtSettings?.Issuer ?? "WebApplication2022_Core8_WebApi_JWT",
            
            ValidateAudience = true,
            ValidAudience = jwtSettings?.Audience ?? "WebApplication2022_Core8_WebApi_JWT_Users",
            
            // 驗證過期時間
            ValidateLifetime = true,
            RequireExpirationTime = true,
            
            // 允許的伺服器時間偏移量
            ClockSkew = TimeSpan.FromMinutes(5), // 減少時間偏移量以提高安全性
            
            // 要求簽名的Token
            RequireSignedTokens = true
        };
    });

#endregion

#region // 連結資料庫才會用到這一段（DB連結字串）- 安全配置

//********************************************************************
// 這裡的關鍵字<MVC_UserDBContext>，
// 請跟 /Models_MVC_UserDB目錄下「MVC_UserDBContext.cs」類別名稱一模一樣。

//**** 讀取 appsettings.json 設定檔裡面的資料（資料庫連結字串）****
// 使用配置的連接字串
builder.Services.AddDbContext<MVC_UserDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

#endregion

#region // 連結資料庫才會用到這一段（DB連結字串）- 安全配置 - HomeLogin

//********************************************************************
// 這裡的關鍵字<MVC_UserLoginContext>，
// 請跟 /Models_MVC_UserLogin目錄下「MVC_UserLoginContext.cs」類別名稱一模一樣。

//**** 讀取 appsettings.json 設定檔裡面的資料（資料庫連結字串）****
// 使用配置的連接字串
builder.Services.AddDbContext<MVC_UserLoginContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HomeLoginConnection")));

#endregion


// [補充範例]-- 微軟範例 WebAPI的 ToDoList（Api控制器，名為 TodoItemsController）
// 需要手動安裝  NuGet 套件 -- Microsoft.EntityFrameworkCore.InMemory
builder.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // API 服務簡介
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Version = "v1",
            Title = "ASP.NET Core Web API",
            Description = "ASP.NET Core Web API Sample" // 描述
            // TermsOfService = new Uri("https://igouist.github.io/"),
            // Contact = new OpenApiContact
            // {
            //     Name = "Brian",
            //     Email = string.Empty,
            //     Url = new Uri("https://igouist.github.io/about/"),
            // }
        }
    );
    //說明api如何受到保護
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            //選擇類型，type選擇http時，透過swagger畫面做認證時可以省略Bearer前綴詞
            //Type使用SecuritySchemeType.ApiKey，需要打Bearer與空白以及文字描述會包含Name、In、Description
            Type = SecuritySchemeType.Http,
            //採用Bearer token方式
            Scheme = "Bearer",
            //bearer格式使用jwt
            BearerFormat = "JWT",
            //認證放在http request的header上
            In = ParameterLocation.Header,
            //描述
            Description = "JWT驗證描述"
        }
    );
    options.OperationFilter<AuthorizeCheckOperationFilter>();
    // 設定 Swagger 產生的 XML 檔案路徑 目的是為了可以讀取我們所寫的註解
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//=== 分 隔 線 ===============================================================


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // ASP.NET Core 中的靜態檔案
// https://learn.microsoft.com/zh-tw/aspnet/core/fundamentals/static-files?view=aspnetcore-7.0

// 使用安全的 CORS middleware - 在開發環境使用寬鬆策略，生產環境使用嚴格策略
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevelopmentPolicy"); // 開發環境使用寬鬆策略
}
else
{
    app.UseCors(); // 生產環境使用預設的安全策略
}
//**************************************************************
// JWT (json web token) 才會用到這一段。必須放在 app.UseAuthorization();之前，順序不能錯！
app.UseAuthentication(); // ** JWT 請自己動手加上這一段 **
//***************************************************************
app.UseAuthorization(); // 順序不能錯！

//app.MapControllers();  // WebAPI專案預設的值
//為了執行 "JSON控制器"，我才從MVC專案複製了下面這一段設定
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");


app.Run();