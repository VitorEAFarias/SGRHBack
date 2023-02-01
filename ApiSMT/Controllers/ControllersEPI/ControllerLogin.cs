using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ControleEPI.DTO.FromBody;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using ApiSMT.Utilitários.JWT;
using ControleEPI.BLL.RHUsuarios;
using ControleEPI.BLL.RHDepartamentos;
using ControleEPI.BLL.RHContratos;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe de login do sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerLogin : ControllerBase
    {
        private readonly IRHConUserBLL _conuser;
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;

        private string generatedToken = null;        

        /// <summary>
        /// Construtor LoginController
        /// </summary>
        /// <param name="conuser"></param>
        /// <param name="config"></param>
        /// <param name="tokenService"></param>
        public ControllerLogin(IRHConUserBLL conuser, IConfiguration config, ITokenService tokenService)
        {
            _conuser = conuser;
            _config = config;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Renova o token de acesso
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>       
        [Authorize]
        [HttpPut("token")]
        public IActionResult MainWindow([FromBody]TokenObject token)
        {
            if (token.token == null)
            {
                return BadRequest(new { message = "Token não encontrado", result = false });
            }

            string newToken = _tokenService.IsTokenValid(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), token.token);

            if (newToken == null)
            {
                return BadRequest(new { message = "Token inválido", result = false });
            }

            return Ok(new { token = newToken });            
        }

        /// <summary>
        /// Verifica credenciais de usuario
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> login([FromBody] LoginDTO login)
        {
            var logar = await _conuser.login(login);            

            if (logar.cpf != "")
            {
                if (logar.senha != "")
                {
                    generatedToken = _tokenService.BuildToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), logar.id.ToString());

                    if (generatedToken != null)
                    {
                        return Ok(new { data = logar, message = " Logado com sucesso!!!", Token = generatedToken, result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Token Inválido", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Senha incorreta", result = false });
                }
            }
            else
            {
                return BadRequest(new { message = "Usuario não encontrado", result = false });
            }            
        }
    }
}
