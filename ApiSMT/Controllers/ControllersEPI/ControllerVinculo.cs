using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ControleEPI.DTO;
using ControleEPI.DTO.FromBody;
using ControleEPI.BLL;
using System;
using System.Collections.Generic;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe ControllerVinculo
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerVinculo : ControllerBase
    {
        private readonly IEPIVinculoBLL _vinculo;
        private readonly IRHConUserBLL _usuario;
        private readonly IEPIProdutosEstoqueBLL _produtoEstoque;
        private readonly IEPIProdutosBLL _produtos;
        private readonly IEPIStatusBLL _status;

        /// <summary>
        /// Construtor ControllerVinculo
        /// </summary>
        /// <param name="vinculo"></param>
        /// <param name="usuario"></param>
        /// <param name="produtoEstoque"></param>
        /// <param name="produtos"></param>
        /// <param name="status"></param>
        public ControllerVinculo(IEPIVinculoBLL vinculo, IRHConUserBLL usuario, IEPIProdutosEstoqueBLL produtoEstoque, IEPIProdutosBLL produtos, IEPIStatusBLL status)
        {
            _vinculo = vinculo;
            _usuario = usuario;
            _produtoEstoque = produtoEstoque;
            _produtos = produtos;
            _status = status;
        }

        /// <summary>
        /// Modifica os status do item vinculado
        /// </summary>
        /// <param name="status"></param>
        /// <param name="idVinculo"></param>
        /// <returns></returns>
        [HttpPut("{idVinculo}/{status}")]
        public async Task<IActionResult> modificaStatus(int status, int idVinculo)
        {
            try
            {
                var localizaVinculo = await _vinculo.localizaVinculo(idVinculo);

                if (localizaVinculo != null)
                {
                    string message = string.Empty;

                    if (idVinculo == 13)
                    {
                        localizaVinculo.dataVinculo = DateTime.Now;
                        localizaVinculo.status = status;

                        message = "Item vinculado com sucesso!!!";
                    }
                    else if (idVinculo == 12)
                    {
                        localizaVinculo.dataDevolucao = DateTime.Now;
                        localizaVinculo.status = status;

                        message = "Item devolvido com sucesso";
                    }

                    await _vinculo.Update(localizaVinculo);

                    return Ok(new { message = message, result = true });
                }
                else
                {
                    return BadRequest(new { message = "Vinculo não encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista itens vinculados
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet("{status}")]
        public async Task<IActionResult> listaVinculosStatus(int status)
        {
            try
            {
                var localizaVinculos = await _vinculo.localizaVinculoStatus(status);

                if (localizaVinculos != null)
                {
                    List<VinculoDTO> listaVinculos = new List<VinculoDTO>();

                    foreach (var item in localizaVinculos)
                    {
                        var localizaEmp = await _usuario.GetEmp(item.idUsuario);
                        var localizaProduto = await _produtos.getProduto(item.idItem);
                        var localizaStatus = await _status.getStatus(item.status);

                        listaVinculos.Add(new VinculoDTO
                        {
                            nomeUsuario = localizaEmp.nome,
                            nomeItem = localizaProduto.nomeProduto,
                            dataVinculo = item.dataVinculo,
                            dataDevolucao = item.dataDevolucao,
                            status = localizaStatus.nome,
                            validade = DateTime.MinValue
                        });
                    }

                    return Ok(new { message = "Lista encontrada", result = true, lista = listaVinculos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum vinculo encontrado com esse status", result = false }); 
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista itens vinculados
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [HttpGet("{idUsuario}")]
        public async Task<IActionResult> listaVinculosUsuarios(int idUsuario)
        {
            try
            {
                var localizaVinculos = await _vinculo.localizaVinculoUsuario(idUsuario);

                if (localizaVinculos != null)
                {
                    List<VinculoDTO> listaVinculos = new List<VinculoDTO>();

                    foreach (var item in localizaVinculos)
                    {
                        var localizaEmp = await _usuario.GetEmp(item.idUsuario);
                        var localizaProduto = await _produtos.getProduto(item.idItem);
                        var localizaStatus = await _status.getStatus(item.status);

                        listaVinculos.Add(new VinculoDTO
                        {
                            nomeUsuario = localizaEmp.nome,
                            nomeItem = localizaProduto.nomeProduto,
                            dataVinculo = item.dataVinculo,
                            dataDevolucao = item.dataDevolucao,
                            status = localizaStatus.nome,
                            validade = DateTime.MinValue
                        });
                    }

                    return Ok(new { message = "Lista encontrada", result = true, lista = listaVinculos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum vinculo encontrado para esse usuário", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
