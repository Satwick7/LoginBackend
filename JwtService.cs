using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Register
{
    public class JwtService
    {
        public string SecretKey { get; set; }
        public int TokenDuration { get; set; }

        private readonly IConfiguration config;

        public JwtService(IConfiguration _config)
        {
            config = _config;
            SecretKey = config.GetSection("jwtConfig").GetSection("Key").Value;
            TokenDuration = int.Parse(config.GetSection("jwtConfig").GetSection("Duration").Value);
        }

        public string GenerateToken(string id, string firstname, string lastname, string email, string mobile, string gender)
        {
            var keyBytes = Encoding.UTF8.GetBytes(SecretKey);
            if (keyBytes.Length < 32)
            {
                using (var sha256 = SHA256.Create())
                {
                    keyBytes = sha256.ComputeHash(keyBytes);  // Hash the key to ensure it's 32 bytes long
                }
            }

            var key = new SymmetricSecurityKey(keyBytes);
            var signature = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var payload = new[]
            {
            new Claim("id", id),
            new Claim("firstname", firstname),
            new Claim("lastname", lastname),
            new Claim("email", email),
            new Claim("mobile", mobile),
            new Claim("gender", gender),
        };

            var jwtToken = new JwtSecurityToken(
                issuer: "localhost",
                audience: "localhost",
                claims: payload,
                expires: DateTime.Now.AddMinutes(TokenDuration),
                signingCredentials: signature
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
