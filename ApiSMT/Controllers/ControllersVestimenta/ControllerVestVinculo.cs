using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.BLL;
using Vestimenta.DTO;
using System;
using ControleEPI.BLL;
using System.Collections.Generic;
using ApiSMT.Utilitários;
using Microsoft.AspNetCore.Authorization;
using Vestimenta.DTO.FromBody;

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
        private readonly IEstoqueBLL _estoque;
        private readonly IVestimentaBLL _vestimenta;
        private readonly ILogBLL _log;
        private readonly IPedidosVestBLL _pedidos;

        /// <summary>
        /// Construtor VestVinculoController
        /// </summary>
        /// <param name="vinculo"></param>
        /// <param name="usuario"></param>
        /// <param name="estoque"></param>
        /// <param name="vestimenta"></param>
        /// <param name="log"></param>
        /// <param name="pedidos"></param>
        public ControllerVestVinculo(IVestVinculoBLL vinculo, IRHConUserBLL usuario, IEstoqueBLL estoque, IVestimentaBLL vestimenta, ILogBLL log, IPedidosVestBLL pedidos)
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
        public async Task<ActionResult> vinculoHistorico([FromBody] VestHistoricoVinculadoDTO historico)
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
                        vinculo.idVestimenta = localizaVestimenta.id;
                        vinculo.dataVinculo = historico.dataVinculo;
                        vinculo.status = 6;
                        vinculo.tamanhoVestVinculo = historico.tamanho;
                        vinculo.usado = historico.usado;
                        vinculo.dataDesvinculo = DateTime.MinValue;
                        vinculo.statusAtual = "Y";                        
                        vinculo.idPedido = 0;
                        vinculo.quantidade = historico.quantidade;

                        var localizaEstoque = await _estoque.getItemExistente(localizaVestimenta.id, historico.tamanho);

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
        public async Task<ActionResult> postVinculo(int idUsuario, [FromBody] VestVinculoDTO itemVinculo)
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

                        foreach (var item in checkPedido.item)
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
                            log.idItem = nomeVest.id;
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
        public async Task<ActionResult> retiraItemVinculo(bool enviarEstoque, int idVinculo)
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
        public async Task<ActionResult> aceitaVinculo(int idUsuario, string senha, [FromBody] List<VestPedidoItensVinculoDTO> pedidosItens)
        {
            try
            {
                if (pedidosItens != null)
                {
                    var checkUsuario = await _usuario.GetSenha(idUsuario);

                    if (checkUsuario != null)
                    {
                        GerarMD5 md5 = new GerarMD5();

                        var senhaMD5 = md5.GeraMD5(senha);

                        if (checkUsuario.senha == senhaMD5)
                        {
                            foreach (var pedido in pedidosItens)
                            {
                                foreach (var itemTamanho in pedido.idItens)
                                {
                                    var checkVinculo = await _vinculo.getVinculoTamanho(pedido.idPedido, itemTamanho.tamanho);

                                    if (checkVinculo != null)
                                    {
                                        List<ItemDTO> getItemPedido = new List<ItemDTO>();

                                        checkVinculo.status = 6;
                                        checkVinculo.dataVinculo = DateTime.Now;
                                        checkVinculo.statusAtual = "Y";

                                        await _vinculo.Update(checkVinculo);

                                        var checkPedido = await _pedidos.getPedido(pedido.idPedido);

                                        foreach (var item in checkPedido.item)
                                        {
                                            if (checkVinculo.idVestimenta == item.id && checkVinculo.tamanhoVestVinculo == item.tamanho)
                                            {
                                                getItemPedido.Add(new ItemDTO
                                                {
                                                    id = item.id,
                                                    nome = item.nome,
                                                    tamanho = item.tamanho,
                                                    quantidade = item.quantidade,
                                                    status = 6,
                                                    dataAlteracao = DateTime.Now,
                                                    usado = item.usado
                                                });
                                            }
                                            else
                                            {
                                                getItemPedido.Add(new ItemDTO
                                                {
                                                    id = item.id,
                                                    nome = item.nome,
                                                    tamanho = item.tamanho,
                                                    quantidade = item.quantidade,
                                                    status = item.status,
                                                    dataAlteracao = item.dataAlteracao,
                                                    usado = item.usado
                                                });
                                            }
                                        }

                                        int contador = 0;

                                        foreach (var status in getItemPedido)
                                        {
                                            if (status.status == 2 || status.status == 7 || status.status == 3 || status.status == 6)
                                                contador++;
                                        }

                                        checkPedido.item = getItemPedido;

                                        if (contador == getItemPedido.Count)
                                        {
                                            checkPedido.status = 2;
                                        }
                                        else
                                        {
                                            checkPedido.status = checkPedido.status;
                                        }

                                        await _pedidos.Update(checkPedido);
                                        
                                    }
                                }                                
                            }

                            return Ok(new { message = "Itens vinculados com sucesso!!!", result = true });
                        }
                        else
                        {
                            return BadRequest(new { message = "Senha incorreta", result = false });
                        }                        
                    }
                    else
                    {
                        return BadRequest(new { message = "Nenhum usuario encontrado!!!", result = false });
                    }         
                }
                else
                {
                    return BadRequest(new { message = "Nenhum item enviado para vincular", result = false });
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
        /// <param name="idSatus"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("status/{idSatus}/{idUsuario}")]
        public async Task<ActionResult> getStatus(int idSatus, int idUsuario)
        {
            try
            {
                var itensPendentes = await _vinculo.getVinculoPendente(idSatus, idUsuario);

                if (itensPendentes != null)
                {
                    List<object> vinculoPendente = new List<object>();

                    foreach (var item in itensPendentes)
                    {
                        var checkVestimenta = await _vestimenta.getVestimenta(item.idVestimenta);
                        var checkUsuario = await _usuario.GetEmp(item.idUsuario);

                        vinculoPendente.Add(new {
                            id = item.id,
                            idPedido = item.idPedido,
                            idItem = item.idVestimenta,
                            nomeUsuario = checkUsuario.nome,
                            nomeVestimenta = checkVestimenta.nome,
                            tamanho = item.tamanhoVestVinculo,
                            data = item.dataVinculo
                        });
                    }

                    return Ok(new { message = "Itens pendentes encontrados!!!", result = true, lista = vinculoPendente });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum colaborador encontrado!!!", result = false });
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
        public async Task<ActionResult> getSituações(int idUsuario)
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
                    if (item.status == 2)
                    {
                        pedidosFinalizados.Add(new { 
                            item = item.id
                        });

                    }
                    else if (item.status == 1)
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
        public async Task<ActionResult<VestVinculoDTO>> getItensVinculados(int idUsuario)
        {
            try
            {
                var checkVinculados = await _vinculo.getItensVinculados(idUsuario);

                if (checkVinculados != null)
                {
                    List<object> lista = new List<object>();

                    foreach (var item in checkVinculados)
                    {
                        var vestimenta = await _vestimenta.getVestimenta(item.idVestimenta);

                        lista.Add(new { 
                            idItem = vestimenta.id,
                            idVinculado = item.id,
                            Vestimenta = vestimenta.nome,
                            Tamanho = item.tamanhoVestVinculo,
                            DataVinculo = item.dataVinculo,
                            DataDesvinculo = item.dataDesvinculo,
                            Usado = item.usado,
                            Status = item.status,
                            Quantidade = item.quantidade
                        });
                    }

                    if (lista != null)
                    {
                        return Ok(new { message = "Itens encontrados", lista = lista, result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Nenhum item pendente encontrado", result = false });
                    }
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
