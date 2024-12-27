using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMG.TokenManager
{
    public static class TokenManager
    {
        public static string Token { get; set; }
        public static DateTime TokenExpiry { get; set; }

        
        public static bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(Token) && TokenExpiry > DateTime.Now;
        }
        public static string GetUsername()
        {
            if (IsLoggedIn())
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(Token) as JwtSecurityToken;
                if (jsonToken != null)
                {
                    // Giải mã các thông tin trong JWT, ví dụ như lấy 'username' từ claim
                    var usernameClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
                    return usernameClaim ?? string.Empty;
                }
            }
            return string.Empty;
        }
    }
}
