using System.Text;
using System.Security.Claims;
using MAF.Adapters.Input.WebServer.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace MAF.Adapters.Input.WebServer.Services{

    public class Key
    {
        public static string SecretKey = "irmaosgoblinF5tFb8Cwf7pk9H16Wd6i";

    }


    public class Token{

        public static object GenerateToken(string userID){

            var key = Encoding.ASCII.GetBytes(Key.SecretKey);

            var tokenConfig = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(
                    
                    new Claim[]{

                        new Claim("userID",userID),
                    }
                ),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenConfig);
            var tokenString = tokenHandler.WriteToken(token);

            return new
            {
                token = tokenString
            };
        }
    }
}