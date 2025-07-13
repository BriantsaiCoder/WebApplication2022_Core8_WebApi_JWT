using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2022_Core8_WebApi_JWT.JwtServices
{
    public class Settings   // 注意 關鍵字 static 已移除，現在使用注入的配置
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationHours { get; set; } = 1;
    }
    
    // 保留靜態訪問以便向後兼容（臨時）
    public static class LegacySettings
    {
        public static string Secret => "YourVerySecureSecretKeyThatShouldBeAtLeast32CharactersLongForSecurity2024!";
        // 注意：這個仍然是硬編碼，但比原來的更安全。建議使用配置注入。
    }
}
