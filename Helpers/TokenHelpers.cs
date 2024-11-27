using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;
using System;

namespace WebQuanLyNhanSu.Helpers
{
    public class TokenHelper
    {
        private static TokenHelper instance;

        public static TokenHelper Instance
        {
            get { if (instance == null) instance = new TokenHelper(); return instance; }
            private set { instance = value; }
        }

        public string CreateToken(string email, string chucVu, IConfiguration config)
        {
            // Tạo danh sách claims cho token
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim("ChucVu", chucVu) // Sử dụng "ChucVu" thay vì "Role"
            };

            // Tạo khóa bảo mật từ cấu hình
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Tạo token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Thời gian duy trì phiên đăng nhập
                signingCredentials: creds);

            // Trả về token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Token").Value);

            // Tham số để xác thực token
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // Cho phép xác thực token đã hết hạn
                ValidateLifetime = false
            };

            try
            {
                // Trả về claims principal
                return tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            }
            catch
            {
                return null;
            }
        }
    }
}
