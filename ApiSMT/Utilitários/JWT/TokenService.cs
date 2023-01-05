using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ApiSMT.Utilitários.JWT
{
    /// <summary>
    /// Classe TokenService
    /// </summary>
    public class TokenService : ITokenService
    {
        private const double EXPIRY_DURATION_MINUTES = 15;

        /// <summary>
        /// Função cria token
        /// </summary>
        /// <param name="key"></param>
        /// <param name="issuer"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public string BuildToken(string key, string issuer, string usuario)
        {
            var claims = new[] 
            {
                new Claim("id", usuario),
                //new Claim(ClaimTypes.Name, usuario),                
                //new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims, expires: DateTime.UtcNow.AddMinutes(EXPIRY_DURATION_MINUTES), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        /// <summary>
        /// Função verifica validade do token
        /// </summary>
        /// <param name="key"></param>
        /// <param name="issuer"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string IsTokenValid(string key, string issuer, string token)
        {
            var mySecret = Encoding.UTF8.GetBytes(key);
            var mySecurityKey = new SymmetricSecurityKey(mySecret);
            var tokenHandler = new JwtSecurityTokenHandler();
            string newToken = null;

            try
            {
                tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = issuer,
                    IssuerSigningKey = mySecurityKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                newToken = BuildToken(key, issuer, userId.ToString());
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

            return newToken;
        }

        /// <summary>
        /// Constrói a mensagem que mostra o token
        /// </summary>
        /// <param name="stringToSplit"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public string BuildMessage(string stringToSplit, int chunkSize)
        {
            var data = Enumerable.Range(0, stringToSplit.Length / chunkSize).Select(i => stringToSplit.Substring(i * chunkSize, chunkSize));

            string result = "O Token gerado é: ";

            foreach (string str in data)
            {
                result += Environment.NewLine + str;
            }

            return result;
        }
    }
}
