using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ControleEPI.BLL;
using ControleEPI.DTO.FromBody;
using ApiSMT.Utilitários;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using ApiSMT.Utilitários.JWT;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe para identificar usuarios administradores
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Função array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int findIndex<T>(this T[] array, T item)
        {
            return Array.IndexOf(array, item);
        }
    }

    /// <summary>
    /// Classe de login do sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerLogin : ControllerBase
    {
        private readonly IRHConUserBLL _conuser;
        private readonly IConfiguration _config;
        private readonly IRHDepartamentosBLL _departamento;
        private readonly IRHEmpContratosBLL _contrato;
        private readonly ITokenService _tokenService;

        private string generatedToken = null;

        int[] array = { 34, 29, 47, 35, 13  }; 

        /// <summary>
        /// Construtor LoginController
        /// </summary>
        /// <param name="conuser"></param>
        /// <param name="config"></param>
        /// <param name="departamento"></param>
        /// <param name="contrato"></param>
        /// <param name="tokenService"></param>
        public ControllerLogin(IRHConUserBLL conuser, IConfiguration config, IRHDepartamentosBLL departamento, IRHEmpContratosBLL contrato, ITokenService tokenService)
        {
            _conuser = conuser;
            _config = config;
            _departamento = departamento;
            _contrato = contrato;
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
            var doc = await _conuser.GetDoc();

            string cpf = "";
            string senha = "";
            string usuario = "";
            string email = "";
            int id = 0;
            bool adm = false;
            bool comprador = false;

            foreach (var item in doc)
            {
                if (item.numero == login.CPF)
                {
                    cpf = item.numero;

                    if (cpf != "")
                    {
                        var empregado = await _conuser.GetEmp(item.id_empregado);

                        if (empregado != null)
                        {
                            var senhas = await _conuser.GetSenha(item.id_empregado);
                            var emailCorp = await _conuser.GetEmpCont(item.id_empregado);
                            var contrato = await _contrato.getEmpContrato(item.id_empregado);

                            if (contrato != null)
                            {
                                var departamento = await _departamento.getDepartamento(contrato.id_departamento);

                                usuario = empregado.nome;
                                id = empregado.id;
                                email = emailCorp.valor;

                                GerarMD5 md5 = new GerarMD5();

                                var senhaMD5 = md5.GeraMD5(login.Senha);

                                if (senhas.senha == senhaMD5)
                                {
                                    senha = senhas.senha;

                                    int index = array.findIndex(departamento.id);

                                    if (index != -1)
                                    {
                                        adm = true;
                                    }

                                    if (departamento.titulo == "COMPRAS")
                                    {
                                        comprador = true;
                                    }

                                    break;
                                }
                            }
                        }                     
                    }
                }
            }

            if (cpf != "")
            {
                if (senha != "")
                {
                    generatedToken = _tokenService.BuildToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), id.ToString());

                    if (generatedToken != null)
                    {
                        return Ok(new { id = id, nome = usuario, email = email, data = true, message = usuario + " Logado com sucesso!!!", Token = generatedToken, adm = adm, compras = comprador });
                    }
                    else
                    {
                        return BadRequest(new { data = false, message = "Token Inválido" });
                    }
                }
                else
                {
                    return BadRequest(new { data = false, message = "Senha incorreta" });
                }
            }
            else
            {
                return BadRequest(new { data = false, message = "Usuario não encontrado" });
            }            
        }
    }
}
