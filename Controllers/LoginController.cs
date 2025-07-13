using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication2022_Core8_WebApi_JWT.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    // 安全改進：添加適當的授權屬性
    // GET: api/<LoginController>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get()
    {
        // 安全改進：返回結構化響應而不是裸露的字符串數組
        return Ok(new { message = "Login controller is available", timestamp = DateTime.UtcNow });
    }

    // 這個操作方法僅允許特定來源
    [EnableCors("AllowSpecificOrigins")]
    // GET api/<LoginController>/5
    [HttpGet("{id}")]
    [Authorize] // 安全改進：需要授權訪問
    public IActionResult Get(int id)
    {
        // 安全改進：添加輸入驗證
        if (id <= 0)
        {
            return BadRequest(new { message = "Invalid ID provided" });
        }
        
        // 僅允許特定來源的操作
        return Ok(new { id = id, message = "Authorized access granted", user = User.Identity?.Name });
    }

    // POST api/<LoginController>
    [HttpPost]
    [AllowAnonymous]
    public IActionResult Post([FromBody] string value)
    {
        // 安全改進：添加輸入驗證
        if (string.IsNullOrWhiteSpace(value))
        {
            return BadRequest(new { message = "Value cannot be empty" });
        }
        
        return Ok(new { message = "Data received", receivedValue = value.Length > 100 ? value.Substring(0, 100) + "..." : value });
    }

    // PUT api/<LoginController>/5
    [HttpPut("{id}")]
    [Authorize] // 安全改進：需要授權訪問
    public IActionResult Put(int id, [FromBody] string value)
    {
        // 安全改進：添加輸入驗證
        if (id <= 0)
        {
            return BadRequest(new { message = "Invalid ID provided" });
        }
        
        if (string.IsNullOrWhiteSpace(value))
        {
            return BadRequest(new { message = "Value cannot be empty" });
        }
        
        return Ok(new { message = "Data updated", id = id, user = User.Identity?.Name });
    }

    // DELETE api/<LoginController>/5
    [HttpDelete("{id}")]
    [Authorize] // 安全改進：需要授權訪問
    public IActionResult Delete(int id)
    {
        // 安全改進：添加輸入驗證
        if (id <= 0)
        {
            return BadRequest(new { message = "Invalid ID provided" });
        }
        
        return Ok(new { message = "Delete operation completed", id = id, user = User.Identity?.Name });
    }
}