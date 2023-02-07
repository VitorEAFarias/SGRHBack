using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.DTO;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Vestimenta.DTO.FromBody;
using Vestimenta.BLL.VestVinculo;
using Vestimenta.BLL.VestEstoque;
using Vestimenta.BLL.VestVestimenta;
using Vestimenta.BLL.VestLog;
using Vestimenta.BLL.VestPedidos;
using RH.BLL.RHUsuarios;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe VestVinculoController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerVestVinculo : ControllerBase
    {
        private readonly IVestVinculoBLL _vinculo;
        private readonly IRHConUserBLL _usuario;
        private readonly IVestEstoqueBLL _estoque;
        private readonly IVestVestimentaBLL _vestimenta;
        private readonly IVestLogBLL _log;
        private readonly IVestPedidosBLL _pedidos;

        /// <summary>
        /// Construtor VestVinculoController
        /// </summary>
        /// <param name="vinculo"></param>
        /// <param name="usuario"></param>
        /// <param name="estoque"></param>
        /// <param name="vestimenta"></param>
        /// <param name="log"></param>
        /// <param name="pedidos"></param>
        public ControllerVestVinculo(IVestVinculoBLL vinculo, IRHConUserBLL usuario, IVestEstoqueBLL estoque, IVestVestimentaBLL vestimenta, IVestLogBLL log, IVestPedidosBLL pedidos)
        {
            _vinculo = vinculo;
            _usuario = usuario;
            _estoque = estoque;
            _vestimenta = vestimenta;
            _log = log;
            _pedidos = pedidos;
        }

        /// <summary>
        /// Cadastra vestimenta vinculada
        /// </summary>
        /// <param name="historico"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> vinculoHistorico([FromBody] VestHistoricoVinculadoDTO historico)
        {
            try
            {
                var localizaUsuario = await _usuario.GetEmp(historico.idUsuario);

                if (localizaUsuario != null)
                {
                    var localizaVestimenta = await _vestimenta.getVestimenta(historico.idVestimenta);

                    if (localizaVestimenta != null)
                    {
                        VestVinculoDTO vinculo = new VestVinculoDTO();

                        vinculo.idUsuario = localizaUsuario.id;
                        vinculo.idVestimenta = localizaVestimenta.idVestimenta;
                        vinculo.dataVinculo = historico.dataVinculo;
                        vinculo.status = 6;
                        vinculo.tamanhoVestVinculo = historico.tamanho;
                        vinculo.usado = historico.usado;
                        vinculo.dataDesvinculo = DateTime.MinValue;
                        vinculo.statusAtual = "Y";                        
                        vinculo.idPedido = 0;
                        vinculo.quantidade = historico.quantidade;

                        var localizaEstoque = await _estoque.getItemExistente(localizaVestimenta.idVestimenta, historico.tamanho);

                        if (localizaEstoque != null)
                        {                            
                            localizaEstoque.quantidadeVinculado = localizaEstoque.quantidadeVinculado + historico.quantidade;

                            var insereVinculo = await _vinculo.Insert(vinculo);

                            if (insereVinculo != null)
                            {
                                await _estoque.Update(localizaEstoque);

                                return Ok(new { message = "Vestimenta de colaborador cadastrado com sucesso!!!", result = true });
                            }
                            else
                            {
                                return BadRequest(new { message = "Erro ao inserir vinculo", result = false });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "Vestimenta não cadastrada", result = false });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Vestimenta não encontrada", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Usuário não encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Insere um novo status
        /// </summary>
        /// <param name="itemVinculo"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{idUsuario}")]
        public async Task<IActionResult> postVinculo(int idUsuario, [FromBody] VestVinculoDTO itemVinculo)
        {
            try
            {
                var checkItemVinculo = await _vinculo.getVinculoPendente(itemVinculo.id, idUsuario);

                if (checkItemVinculo != null)
                {
                    var usuario = await _usuario.GetEmp(idUsuario);

                    if (usuario != null)
                    {                        
                        var checkEstoque = await _estoque.getItemExistente(itemVinculo.idVestimenta, itemVinculo.tamanhoVestVinculo);
                        var nomeVest = await _vestimenta.getVestimenta(itemVinculo.idVestimenta);
                        var checkPedido = await _pedidos.getPedido(itemVinculo.idPedido);

                        foreach (var item in checkPedido.pedido)
                        {
                            for (int i = 0; i < item.quantidade; i++)
                            {
                                VestVinculoDTO vincular = new VestVinculoDTO();

                                vincular.idUsuario = usuario.id;
                                vincular.idVestimenta = itemVinculo.idVestimenta;
                                vincular.dataVinculo = DateTime.Now;
                                vincular.status = 6;
                                vincular.tamanhoVestVinculo = itemVinculo.tamanhoVestVinculo;
                                vincular.usado = itemVinculo.usado;
                                vincular.dataDesvinculo = DateTime.MinValue;
                                vincular.statusAtual = "Y";
                                vincular.idPedido = itemVinculo.idPedido;

                                var insereVinculo = await _vinculo.Insert(vincular);
                            }

                            VestLogDTO log = new VestLogDTO();

                            log.data = DateTime.Now;
                            log.idUsuario = usuario.id;
                            log.idItem = nomeVest.idVestimenta;
                            log.quantidadeAnt = checkEstoque.quantidade;
                            log.quantidadeDep = checkEstoque.quantidade - item.quantidade;

                            await _log.Insert(log);
                        }
                        return Ok(new { message = "Vinculo inserido com sucesso!!!", result = true });  
                    }
                    else
                    {
                        return BadRequest(new { message = "Colaborador não encontrado", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { messsage = "Nenhum vinculo selecionado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Desvincula vestimenta
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut("desvincular/{enviarEstoque}/{idVinculo}")]
        public async Task<IActionResult> retiraItemVinculo(bool enviarEstoque, int idVinculo)
        {
            try
            {
                VestVinculoDTO checkDesvinculo = await _vinculo.getVinculo(idVinculo);
                string mensagem = string.Empty;

                if (checkDesvinculo.dataDesvinculo == DateTime.MinValue)
                {
                    if (enviarEstoque == true)
                    {
                        VestEstoqueDTO checkEstoque = await _estoque.getItemExistente(checkDesvinculo.idVestimenta, checkDesvinculo.tamanhoVestVinculo);

                        if (checkEstoque != null)
                        {
                            checkEstoque.quantidadeUsado = checkEstoque.quantidadeUsado + 1;

                            await _estoque.Update(checkEstoque);
                        }
                        else
                        {
                            mensagem = "Item não encontrado em estoque";
                        }                        
                    }

                    checkDesvinculo.dataDesvinculo = DateTime.Now;

                    await _vinculo.Update(checkDesvinculo);

                    return Ok(new { message = "Item devolvido com sucesso!!!", result = true, desvinculo = mensagem });
                }
                else
                {
                    return BadRequest(new { message = "Esse item ja foi devolvido", result = false});
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Função que vincula itens com colaborador
        /// </summary>
        /// <param name="pedidosItens"></param>
        /// <param name="idUsuario"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("verificacao/{idUsuario}/{senha}")]
        public async Task<IActionResult> aceitaVinculo(int idUsuario, string senha, [FromBody] List<VestPedidoItensVinculoDTO> pedidosItens)
        {
            try
            {
                var aceitarVinculo = await _vinculo.aceitaVinculo(idUsuario, senha, pedidosItens);

                if (aceitarVinculo != null)
                {
                    return Ok(new { message = "Itens adquiridos com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Erro ao vincular itens com colaborador", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get de vinculos pendentes
        /// </summary>
        /// <param name="idStatus"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("status/{idStatus}/{idUsuario}")]
        public async Task<IActionResult> getStatus(int idStatus, int idUsuario)
        {
            try
            {
                var itensPendentes = await _vinculo.getVinculoPendente(idStatus, idUsuario);

                if (itensPendentes != null)
                {
                    return Ok(new { message = "Itens pendentes encontrados!!!", result = true, data = itensPendentes });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum item pendente encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }        
        
        /// <summary>
        /// get todas as situações do colaborador
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("situacoes/{idUsuario}")]
        public async Task<IActionResult> getSituações(int idUsuario)
        {
            try
            {
                var getPendente = await _vinculo.getItensUsuarios(idUsuario);
                var getPedidos = await _pedidos.getPedidosUsuarios(idUsuario);

                List<object> pendentes = new List<object>();
                List<object> vinculados = new List<object>();
                List<object> pedidosFinalizados = new List<object>();
                List<object> pedidosPendentes = new List<object>();                
                List<object> pedidosReprovados = new List<object>();

                foreach (var item in getPendente)
                {
                    if (item.status == 6)
                    {
                        vinculados.Add(new { 
                            item = item.id
                        });
                    }
                    else
                    {
                        pendentes.Add(new {
                            item = item.id
                        });
                    }
                }

                foreach (var item in getPedidos)
                {
                    if (item.status.Equals(2))
                    {
                        pedidosFinalizados.Add(new { 
                            item = item.id
                        });

                    }
                    else if (item.status.Equals(1))
                    {
                        pedidosPendentes.Add(new {
                            item = item.id
                        });
                    }
                    else
                    {
                        pedidosReprovados.Add(new {
                            item = item.id
                        });
                    }
                }

                return Ok(new { message = "Numeros encontrados!!!", result = true, vinculado = vinculados.Count, pendente = pendentes.Count, pedidosFinalizados = pedidosFinalizados.Count,
                    pedidosPendentes = pedidosPendentes.Count, pedidosReprovados = pedidosReprovados.Count });
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Pega todos os itens vinculados com um usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("vinculados/{idUsuario}")]
        public async Task<IActionResult> getItensVinculados(int idUsuario)
        {
            try
            {
                var checkVinculados = await _vinculo.getItensVinculados(idUsuario);

                if (checkVinculados != null)
                {                    
                    return Ok(new { message = "Itens encontrados", data = checkVinculados, result = true });                    
                }
                else
                {
                    return BadRequest(new { message = "Nenhum item vinculado com esse colaborador", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
