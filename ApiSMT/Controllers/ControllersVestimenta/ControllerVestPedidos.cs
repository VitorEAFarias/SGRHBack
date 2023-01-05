using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.BLL;
using Vestimenta.DTO;
using ControleEPI.BLL;
using System;
using System.Collections.Generic;
using ControleEPI.DTO.E_Mail;
using Microsoft.AspNetCore.Authorization;
using Vestimenta.DTO.FromBody;
using ApiSMT.Utilitários;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe de pedidos de vestimenta
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerVestPedidos : ControllerBase
    {
        private readonly IPedidosVestBLL _pedidosVest;
        private readonly IRHConUserBLL _usuario;
        private readonly IStatusVestBLL _status;
        private readonly IEstoqueBLL _estoque;
        private readonly IVestRepositorioBLL _repositorio;
        private readonly IVestVinculoBLL _itemVinculo;
        private readonly ILogBLL _log;
        private readonly IMailService _mail;
        private readonly IVestimentaBLL _vestimenta;
        private readonly IRHDepartamentosBLL _departamento;
        private readonly IRHEmpContratosBLL _contrato;

        /// <summary>
        /// Construtor de Pedidos
        /// </summary>
        /// <param name="pedidosVest"></param>
        /// <param name="usuario"></param>
        /// <param name="status"></param>
        /// <param name="estoque"></param>
        /// <param name="repositorio"></param>
        /// <param name="itemVinculo"></param>
        /// <param name="log"></param>
        /// <param name="mail"></param>
        /// <param name="vestimenta"></param>
        /// <param name="departamento"></param>
        /// <param name="contrato"></param>
        public ControllerVestPedidos(IPedidosVestBLL pedidosVest, IRHConUserBLL usuario, IStatusVestBLL status, IEstoqueBLL estoque, IVestRepositorioBLL repositorio,
            IVestVinculoBLL itemVinculo, ILogBLL log, IMailService mail, IVestimentaBLL vestimenta, IRHDepartamentosBLL departamento, IRHEmpContratosBLL contrato)
        {
            _pedidosVest = pedidosVest;
            _usuario = usuario;
            _status = status;
            _estoque = estoque;
            _repositorio = repositorio;
            _itemVinculo = itemVinculo;
            _log = log;
            _mail = mail;
            _vestimenta = vestimenta;
            _departamento = departamento;
            _contrato = contrato;
        }

        /// <summary>
        /// Insere um novo pedido
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<VestPedidosDTO>> postPedido([FromBody] VestPedidosDTO pedido)
        {
            try
            {
                var checkUsuario = await _usuario.GetEmp(pedido.idUsuario);

                if (checkUsuario != null)
                {
                    if (pedido != null)
                    {
                        pedido.dataPedido = DateTime.Now;

                        //var novoPedido = await _pedidosVest.Insert(pedido);
                        var getEmail = await _usuario.getEmail(checkUsuario.id);
                        var getContrato = await _contrato.getEmpContrato(pedido.idUsuario);
                        var getDepartamento = await _departamento.getDepartamento(getContrato.id_departamento);

                        EmailRequestDTO email = new EmailRequestDTO();
                        VestConteudoEmailColaboradorDTO conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO();
                        List<VestConteudoEmailDTO> conteudoEmails = new List<VestConteudoEmailDTO>();

                        foreach (var item in pedido.item)
                        {
                            var checkStatusItem = await _status.getStatus(item.status);

                            conteudoEmails.Add(new VestConteudoEmailDTO
                            {
                                nome = item.nome,
                                tamanho = item.tamanho,
                                status = checkStatusItem.nome,
                                quantidade = item.quantidade
                            });
                        }

                        conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO {
                            idPedido = 1,//novoPedido.id,
                            nomeColaborador = checkUsuario.nome,
                            departamento = getDepartamento.titulo
                        };

                        email.EmailDe = getEmail.valor;
                        email.EmailPara = "rh@reisoffice.com.br";
                        email.ConteudoColaborador = conteudoEmailColaborador;
                        email.Conteudo = conteudoEmails;
                        email.Assunto = "Novo pedido de vestimenta";

                        await _mail.SendEmailAsync(email);
                        
                        return Ok(new { message = "Pedido feito com sucesso!!!", result = true, data = 1 });
                    }
                    else
                    {
                        return BadRequest(new { message = "Erro ao efetuar pedido " + pedido, result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Usuário não encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza status do pedido e do item
        /// </summary>
        /// <param name="pedidoItem"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> atualizaStatusPedidoItem([FromBody] VestPedidosDTO pedidoItem)
        {
            try
            {
                VestPedidosDTO checkPedido = await _pedidosVest.getPedido(pedidoItem.id);

                if (checkPedido != null)
                {
                    bool verificaCompra = false;
                    List<ItemDTO> listaItens = new List<ItemDTO>();
                    List<VestConteudoEmailDTO> conteudoEmails = new List<VestConteudoEmailDTO>();
                    VestConteudoEmailColaboradorDTO conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO();

                    int cont = 0;

                    foreach (var item in pedidoItem.item)
                    {
                        var statusPedido = await _status.getStatus(item.status);
                        var nomeColaborador = await _usuario.GetEmp(pedidoItem.idUsuario);
                        var contrato = await _contrato.getEmpContrato(nomeColaborador.id);
                        var departamento = await _departamento.getDepartamento(contrato.id_departamento);

                        if (item.status == 5 && checkPedido.item[cont].status != 5)
                        {
                            var getEstoque = await _estoque.getItemExistente(item.id, item.tamanho);
                            var nomeItem = await _vestimenta.getVestimenta(item.id);
                            var statusItem = await _status.getStatus(item.status);

                            VestRepositorioDTO repositorio = new VestRepositorioDTO();

                            repositorio.idItem = item.id;
                            repositorio.idPedido = pedidoItem.id;
                            repositorio.enviadoCompra = "N";
                            repositorio.dataAtualizacao = DateTime.Now;
                            repositorio.tamanho = item.tamanho;
                            repositorio.quantidade = item.quantidade;

                            listaItens.Add(new ItemDTO
                            {
                                id = item.id,
                                nome = item.nome,
                                tamanho = item.tamanho,
                                quantidade = item.quantidade,
                                status = 5,
                                dataAlteracao = DateTime.Now
                            });

                            conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO
                            {
                                idPedido = pedidoItem.id,
                                nomeColaborador = nomeColaborador.nome,
                                departamento = departamento.titulo
                            };

                            conteudoEmails.Add(new VestConteudoEmailDTO
                            {
                                nome = nomeItem.nome,
                                tamanho = item.tamanho,
                                status = statusItem.nome,
                                quantidade = item.quantidade
                            });

                            var insereItens = await _repositorio.Insert(repositorio);                                

                        }
                        else if (item.status == 4 && checkPedido.item[cont].status != 4)
                        {
                            VestEstoqueDTO getEstoque = await _estoque.getItemExistente(item.id, item.tamanho);
                            VestVinculoDTO liberadoVinculo = new VestVinculoDTO();

                            var nomeItem = await _vestimenta.getVestimenta(item.id);
                            var statusItem = await _status.getStatus(item.status);

                            if (item.usado != "N")
                            {
                                getEstoque.quantidadeUsado = getEstoque.quantidadeUsado - item.quantidade;
                                getEstoque.tamanho = item.tamanho;
                                getEstoque.dataAlteracao = DateTime.Now;
                                getEstoque.quantidadeVinculado = getEstoque.quantidadeVinculado + item.quantidade;

                                await _estoque.Update(getEstoque);

                                VestLogDTO log = new VestLogDTO();

                                log.data = DateTime.Now;
                                log.idUsuario = pedidoItem.idUsuarioAlteracao;
                                log.idItem = item.id;
                                log.quantidadeAnt = getEstoque.quantidade;
                                log.quantidadeDep = getEstoque.quantidade - item.quantidade;
                                log.tamanho = item.tamanho;
                                log.usado = "N";

                                var insereLog = await _log.Insert(log);

                                liberadoVinculo.idUsuario = pedidoItem.idUsuario;
                                liberadoVinculo.idVestimenta = item.id;
                                liberadoVinculo.dataVinculo = DateTime.MinValue;
                                liberadoVinculo.status = item.status;
                                liberadoVinculo.tamanhoVestVinculo = item.tamanho;
                                liberadoVinculo.dataDesvinculo = DateTime.MinValue;
                                liberadoVinculo.statusAtual = "N";
                                liberadoVinculo.idPedido = checkPedido.id;
                                liberadoVinculo.usado = "N";                                

                                conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO
                                {
                                    idPedido = pedidoItem.id,
                                    nomeColaborador = nomeColaborador.nome,
                                    departamento = departamento.titulo
                                };

                                conteudoEmails.Add(new VestConteudoEmailDTO
                                {
                                    nome = nomeItem.nome,
                                    tamanho = item.tamanho,
                                    status = statusItem.nome,
                                    quantidade = item.quantidade
                                });

                                item.dataAlteracao = DateTime.Now;
                                listaItens.Add(item);
                            }
                            else
                            {
                                getEstoque.quantidade = getEstoque.quantidade - item.quantidade;
                                getEstoque.tamanho = item.tamanho;
                                getEstoque.dataAlteracao = DateTime.Now;
                                getEstoque.quantidadeVinculado = getEstoque.quantidadeVinculado + item.quantidade;

                                await _estoque.Update(getEstoque);

                                VestLogDTO log = new VestLogDTO();

                                log.data = DateTime.Now;
                                log.idUsuario = pedidoItem.idUsuarioAlteracao;
                                log.idItem = item.id;
                                log.quantidadeAnt = getEstoque.quantidadeUsado;
                                log.quantidadeDep = item.quantidade;
                                log.tamanho = item.tamanho;
                                log.usado = "S";

                                var insereLog = await _log.Insert(log);

                                liberadoVinculo.idUsuario = pedidoItem.idUsuario;
                                liberadoVinculo.idVestimenta = item.id;
                                liberadoVinculo.dataVinculo = DateTime.MinValue;
                                liberadoVinculo.status = item.status;
                                liberadoVinculo.tamanhoVestVinculo = item.tamanho;
                                liberadoVinculo.dataDesvinculo = DateTime.MinValue;
                                liberadoVinculo.statusAtual = "N";
                                liberadoVinculo.idPedido = checkPedido.id;
                                liberadoVinculo.usado = "N";

                                var insereItemVinculo = await _itemVinculo.Insert(liberadoVinculo);

                                if (insereItemVinculo != null)
                                {
                                    VestRepositorioDTO repositorio = new VestRepositorioDTO();

                                    repositorio.idItem = item.id;
                                    repositorio.idPedido = checkPedido.id;
                                    repositorio.enviadoCompra = "N";
                                    repositorio.dataAtualizacao = DateTime.Now;
                                    repositorio.quantidade = item.quantidade;
                                    repositorio.tamanho = item.tamanho;

                                    var insereCompraItemUsado = await _repositorio.Insert(repositorio);

                                    if (insereCompraItemUsado != null)
                                    {
                                        verificaCompra = true;
                                    }
                                }

                                conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO
                                {
                                    idPedido = pedidoItem.id,
                                    nomeColaborador = nomeColaborador.nome,
                                    departamento = departamento.titulo
                                };

                                conteudoEmails.Add(new VestConteudoEmailDTO
                                {
                                    nome = nomeItem.nome,
                                    tamanho = item.tamanho,
                                    status = statusItem.nome,
                                    quantidade = item.quantidade
                                });

                                item.dataAlteracao = DateTime.Now;
                                listaItens.Add(item);
                            }
                        }
                        else if (item.status == 3 && checkPedido.item[cont].status != 3)
                        {
                            var checkRepositorio = await _repositorio.getRepositorioItensPedidos(item.id, checkPedido.id);

                            if (checkRepositorio != null)
                            {
                                checkRepositorio.enviadoCompra = "N";
                                checkRepositorio.ativo = "N";

                                await _repositorio.Update(checkRepositorio);
                            } 

                            var nomeItem = await _vestimenta.getVestimenta(item.id);
                            var statusItem = await _status.getStatus(item.status);

                            conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO
                            {
                                idPedido = pedidoItem.id,
                                nomeColaborador = nomeColaborador.nome,
                                departamento = departamento.titulo
                            };

                            conteudoEmails.Add(new VestConteudoEmailDTO
                            {
                                nome = nomeItem.nome,
                                tamanho = item.tamanho,
                                status = statusItem.nome,
                                quantidade = item.quantidade
                            });

                            item.dataAlteracao = DateTime.Now;
                            listaItens.Add(item);
                        }
                        else
                        {
                            listaItens.Add(item);
                        }

                        cont ++;
                    }                    

                    pedidoItem.item = listaItens;

                    await _pedidosVest.Update(pedidoItem);

                    var verificaPedidoInserido = await _pedidosVest.getPedido(pedidoItem.id);

                    int contador = 0;

                    foreach (var status in verificaPedidoInserido.item)
                    {
                        if (status.status == 2 || status.status == 7 || status.status == 3 || status.status == 6)
                            contador++;
                    }

                    if (contador == listaItens.Count)
                    {
                        verificaPedidoInserido.status = 2;
                    }
                    else
                    {
                        verificaPedidoInserido.status = verificaPedidoInserido.status;
                    }

                    await _pedidosVest.Update(verificaPedidoInserido);

                    if (conteudoEmails != null)
                    {
                        EmailRequestDTO email = new EmailRequestDTO();

                        var getEmail = await _usuario.getEmail(pedidoItem.idUsuario);
                        var statusDePedido = await _status.getStatus(verificaPedidoInserido.status);

                        email.EmailDe = getEmail.valor;
                        email.EmailPara = "rh@reisoffice.com.br";
                        email.ConteudoColaborador = conteudoEmailColaborador;
                        email.Conteudo = conteudoEmails;
                        email.Assunto = "Atualização de Pedido";

                        await _mail.SendEmailAsync(email);
                    }                    

                    return Ok(new { message = "Status Atualizado com sucesso!!!", result = true, compraUsado = verificaCompra });
                }
                else
                {
                    return BadRequest(new { message = "Pedido não encontrado!!!", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista pedidos por usuarios
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult> getPedidosUsuario(int idUsuario)
        {
            try
            {
                var emp = await _usuario.GetEmp(idUsuario);

                if (emp != null)
                {
                    var pedidosUsuario = await _pedidosVest.getPedidosUsuarios(idUsuario);

                    List<object> lista = new List<object>();

                    foreach (var item in pedidosUsuario)
                    {
                        var status = await _status.getStatus(item.status);

                        lista.Add(new
                        {
                            Id = item.id,
                            Nome = emp.nome,
                            Pedido = item.item,
                            status = status.nome,
                            DataPedido = item.dataPedido
                        });
                    }

                    return Ok(new { message = "lista encontrada", result = true, lista = lista });
                }
                else
                {
                    var pedidos = await _pedidosVest.getPedidos();

                    List<object> lista = new List<object>();

                    foreach (var item in pedidos)
                    {
                        var status = await _status.getStatus(item.status);

                        lista.Add(new
                        {
                            Id = item.id,
                            Nome = emp.nome,
                            Pedido = item.item,
                            status = status.nome,
                            DataPedido = item.dataPedido
                        });
                    }

                    return Ok(new { message = "lista encontrada", result = true, lista = lista });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista pedidos por status
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("status/{idStatus}")]
        public async Task<ActionResult> getPedidosStatus(int idStatus)
        {
            try
            {
                var pedidos = await _pedidosVest.getPedidosStatus(idStatus);

                if (pedidos != null)
                {
                    List<object> lista = new List<object>();

                    foreach (var item in pedidos)
                    {
                        var emp = await _usuario.GetEmp(item.idUsuario);

                        var status = await _status.getStatus(idStatus);

                        if (emp != null)
                        {
                            lista.Add(new
                            {
                                Id = item.id,
                                Nome = emp.nome,
                                Pedido = item.item,
                                status = status.nome,
                                DataPedido = item.dataPedido
                            });
                        }
                        else
                        {
                            return BadRequest(new { message = "Supervisor não encontrado!!!", result = false });
                        }
                    }

                    return Ok(new { message = "lista encontrada", result = true, lista = lista });
                }
                else
                {
                    var todosPedidos = await _pedidosVest.getPedidos();

                    List<object> lista = new List<object>();

                    foreach (var item in todosPedidos)
                    {
                        var emp = await _usuario.GetEmp(item.idUsuario);
                        var status = await _status.getStatus(item.status);

                        if (emp != null)
                        {
                            lista.Add(new
                            {
                                Id = item.id,
                                Nome = emp.nome,
                                Pedido = item.item,
                                status = status.nome,
                                DataPedido = item.dataPedido
                            });
                        }
                        else
                        {
                            return BadRequest(new { message = "Supervisor não encontrado!!!", result = false });
                        }
                    }

                    return Ok(new { message = "lista encontrada", result = true, lista = lista });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Pedidos liberados para vinculo 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("vinculo/{idPedido}")]
        public async Task<ActionResult<VestPedidosDTO>> getLiberadoVinculo(int idPedido)
        {
            try
            {
                var getPedidos = await _pedidosVest.getPedido(idPedido);
                var pedidosVinculo = string.Empty;
                List<ItemDTO> itensLiberadosVinculo = new List<ItemDTO>();

                if (getPedidos != null)
                {
                    foreach (var item in getPedidos.item)
                    {
                        if (item.status == 4)
                        {
                            itensLiberadosVinculo.Add(new ItemDTO {
                                id = item.id,
                                nome = item.nome,
                                tamanho = item.tamanho,
                                quantidade = item.quantidade,
                                status = item.status,
                                dataAlteracao = item.dataAlteracao
                            });
                        }
                    }

                    if (itensLiberadosVinculo != null)
                    {
                        return Ok(new { message = "Itens encontrados!!!", itens = itensLiberadosVinculo, result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Nenhum item liberado para vinculo foi encontrado", result = false});
                    }                    
                }
                else
                {
                    return BadRequest(new { message = "Nenhum pedido encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza status dos itens de todos os pedidos
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut("itens/{ids}")]
        public async Task<ActionResult> atualizaStatusTodosPedidos([FromBody] List<VestPedidosDTO> pedidosItens)
        {
            try
            {
                bool verificaCompra = false;

                foreach (var pedido in pedidosItens)
                {
                    VestPedidosDTO checkPedido = await _pedidosVest.getPedido(pedido.id);
                    List<ItemDTO> listaItens = new List<ItemDTO>();

                    if (checkPedido != null)
                    {
                        int cont = 0;

                        foreach (var item in pedido.item)
                        {
                            if (item.status == 5 && checkPedido.item[cont].status != 5)
                            {
                                var getEstoque = await _estoque.getItemExistente(item.id, item.tamanho);

                                VestRepositorioDTO repositorio = new VestRepositorioDTO();

                                repositorio.idItem = item.id;
                                repositorio.idPedido = checkPedido.id;
                                repositorio.enviadoCompra = "N";
                                repositorio.dataAtualizacao = DateTime.Now;
                                repositorio.tamanho = item.tamanho;
                                repositorio.quantidade = item.quantidade;

                                listaItens.Add(new ItemDTO
                                {
                                    id = item.id,
                                    nome = item.nome,
                                    tamanho = item.tamanho,
                                    quantidade = item.quantidade,
                                    status = 5,
                                    dataAlteracao = DateTime.Now
                                });

                                var insereItens = await _repositorio.Insert(repositorio);

                            }
                            else if (item.status == 4 && checkPedido.item[cont].status != 4)
                            {
                                VestEstoqueDTO getEstoque = await _estoque.getItemExistente(item.id, item.tamanho);
                                VestVinculoDTO liberadoVinculo = new VestVinculoDTO();

                                if (item.usado == "N")
                                {
                                    getEstoque.quantidade = getEstoque.quantidade - item.quantidade;
                                    getEstoque.tamanho = item.tamanho;
                                    getEstoque.dataAlteracao = DateTime.Now;
                                    getEstoque.quantidadeVinculado = getEstoque.quantidadeVinculado + item.quantidade;

                                    await _estoque.Update(getEstoque);

                                    VestLogDTO log = new VestLogDTO();

                                    log.data = DateTime.Now;
                                    log.idUsuario = checkPedido.idUsuarioAlteracao;
                                    log.idItem = item.id;
                                    log.quantidadeAnt = getEstoque.quantidade;
                                    log.quantidadeDep = getEstoque.quantidade - item.quantidade;
                                    log.tamanho = item.tamanho;
                                    log.usado = "N";

                                    var insereLog = await _log.Insert(log);

                                    liberadoVinculo.idUsuario = pedido.idUsuario;
                                    liberadoVinculo.idVestimenta = item.id;
                                    liberadoVinculo.dataVinculo = DateTime.MinValue;
                                    liberadoVinculo.status = item.status;
                                    liberadoVinculo.tamanhoVestVinculo = item.tamanho;
                                    liberadoVinculo.dataDesvinculo = DateTime.MinValue;
                                    liberadoVinculo.statusAtual = "N";
                                    liberadoVinculo.idPedido = checkPedido.id;
                                    liberadoVinculo.usado = "N";

                                    var insereItemVinculo = await _itemVinculo.Insert(liberadoVinculo);

                                    item.dataAlteracao = DateTime.Now;
                                    listaItens.Add(item);
                                }
                                else
                                {
                                    getEstoque.quantidadeUsado = getEstoque.quantidadeUsado - item.quantidade;
                                    getEstoque.tamanho = item.tamanho;
                                    getEstoque.dataAlteracao = DateTime.Now;
                                    getEstoque.quantidadeVinculado = getEstoque.quantidadeVinculado + item.quantidade;

                                    await _estoque.Update(getEstoque);

                                    VestLogDTO log = new VestLogDTO();

                                    log.data = DateTime.Now;
                                    log.idUsuario = checkPedido.idUsuarioAlteracao;
                                    log.idItem = item.id;
                                    log.quantidadeAnt = getEstoque.quantidadeUsado;
                                    log.quantidadeDep = item.quantidade;
                                    log.tamanho = item.tamanho;
                                    log.usado = "S";

                                    var insereLog = await _log.Insert(log);

                                    liberadoVinculo.idUsuario = pedido.idUsuario;
                                    liberadoVinculo.idVestimenta = item.id;
                                    liberadoVinculo.dataVinculo = DateTime.MinValue;
                                    liberadoVinculo.status = item.status;
                                    liberadoVinculo.tamanhoVestVinculo = item.tamanho;
                                    liberadoVinculo.dataDesvinculo = DateTime.MinValue;
                                    liberadoVinculo.statusAtual = "N";
                                    liberadoVinculo.idPedido = checkPedido.id;
                                    liberadoVinculo.usado = "S";

                                    var insereItemVinculo = await _itemVinculo.Insert(liberadoVinculo);

                                    if (insereItemVinculo != null)
                                    {
                                        VestRepositorioDTO repositorio = new VestRepositorioDTO();

                                        repositorio.idItem = item.id;
                                        repositorio.idPedido = checkPedido.id;
                                        repositorio.enviadoCompra = "N";
                                        repositorio.dataAtualizacao = DateTime.Now;
                                        repositorio.quantidade = item.quantidade;

                                        var insereCompraItemUsado = await _repositorio.Insert(repositorio);

                                        if (insereCompraItemUsado != null)
                                        {
                                            verificaCompra = true;
                                        }
                                    }

                                    item.dataAlteracao = DateTime.Now;
                                    listaItens.Add(item);
                                }
                            }
                            else if (item.status == 3 && checkPedido.item[cont].status != 3)
                            {
                                var checkRepositorio = await _repositorio.getRepositorioItensPedidos(item.id, checkPedido.id);

                                checkRepositorio.enviadoCompra = "N";
                                checkRepositorio.ativo = "N";

                                await _repositorio.Update(checkRepositorio);

                                item.dataAlteracao = DateTime.Now;
                                listaItens.Add(item);
                            }
                            else
                            {
                                listaItens.Add(item);
                            }

                            cont++;
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Nenhum status poderá ser alterado!!!", result = false });
                    }                                        

                    checkPedido.item = listaItens;

                    await _pedidosVest.Update(checkPedido);

                    var verificaPedidoInserido = await _pedidosVest.getPedido(pedido.id);

                    int contador = 0;

                    foreach (var status in verificaPedidoInserido.item)
                    {
                        if (status.status == 2 || status.status == 7 || status.status == 3 || status.status == 6)
                            contador++;
                    }

                    if (contador == listaItens.Count)
                    {
                        verificaPedidoInserido.status = 2;
                    }
                    else
                    {
                        verificaPedidoInserido.status = verificaPedidoInserido.status;
                    }

                    await _pedidosVest.Update(verificaPedidoInserido);
                }

                return Ok(new { message = "Status Atualizado com sucesso!!!", result = true, compraUsado = verificaCompra });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);                
            }
        }

        /// <summary>
        /// Get todos os itens com seus respectivos pedidos
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<VestPedidosDTO>> getItensPedidos()
        {
            try
            {
                var pedidos = await _pedidosVest.getPedidosPendentes();
                List<object> pedidoItens = new List<object>();

                foreach (var pedido in pedidos)
                {
                    var getEmp = await _usuario.GetEmp(pedido.idUsuario);
                    var getPedido = await _pedidosVest.getPedido(pedido.id);
                    
                    foreach (var item in pedido.item)
                    {
                        var checkEstoque = await _estoque.getItemExistente(item.id, item.tamanho);

                        if (checkEstoque != null)
                        {
                            if (item.status == 1)
                            {
                                pedidoItens.Add(new
                                {
                                    pedido = getPedido,
                                    idItem = item.id,
                                    emitente = getEmp.nome,
                                    nomeItem = item.nome,
                                    tamanhoItem = item.tamanho,
                                    quantidade = item.quantidade,
                                    quantidadeEstoque = checkEstoque.quantidade,
                                    quantidadeEstoqueUsado = checkEstoque.quantidadeUsado,
                                    status = item.status
                                });
                            }
                        }
                        else
                        {
                            if (item.status == 1)
                            {
                                pedidoItens.Add(new
                                {
                                    pedido = getPedido,
                                    idItem = item.id,
                                    emitente = getEmp.nome,
                                    nomeItem = item.nome,
                                    tamanhoItem = item.tamanho,
                                    quantidade = item.quantidade,
                                    quantidadeEstoque = "",
                                    quantidadeEstoqueUsado = "",
                                    status = item.status
                                });
                            }
                        }
                    }
                }

                if (pedidoItens != null)
                {
                    return Ok(new { message = "Lista encontrada", result = true, lista = pedidoItens });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum pedido pendente encontrado", result = false });
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona um pedido
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<VestPedidosDTO>> getPedido(int id)
        {
            try
            {
                if (id != 0)
                {
                    var compra = await _pedidosVest.getPedido(id);

                    List<object> listaItens = new List<object>();

                    foreach (var item in compra.item)
                    {
                        string enviadoCompra = string.Empty;
                        var checkEstoque = await _estoque.getItemExistente(item.id, item.tamanho);
                        var checkStatus = await _status.getStatus(item.status);
                        var checkRepositorio = await _repositorio.getRepositorioItensPedidos(item.id, compra.id);

                        if (checkRepositorio != null)
                        {
                            enviadoCompra = checkRepositorio.enviadoCompra;
                        }
                        else
                        {
                            enviadoCompra = "S";
                        }

                        listaItens.Add(new { 
                            item.id,
                            item.dataAlteracao,
                            item.nome,
                            item.quantidade,
                            item.status,
                            item.tamanho,
                            item.usado,
                            enviadoCompra,
                            statusNome = checkStatus.nome,
                            estoque = checkEstoque.quantidade,
                            estoqueUsado = checkEstoque.quantidadeUsado
                        });
                    }

                    var emp = await _usuario.GetEmp(compra.idUsuario);
                    var status = await _status.getStatus(compra.status);

                    List<object> list = new List<object>();

                    list.Add(new {
                        Id = compra.id,
                        Nome = emp.nome,
                        Pedido = listaItens,
                        idStatus = compra.status,
                        status = status.nome,
                        idUsuario = compra.idUsuario,
                        idUsuarioAlteracao = compra.idUsuarioAlteracao,
                        dataAlteracao = compra.dataAlteracao,
                        observacoes = compra.observacoes,
                        DataPedido = compra.dataPedido
                    });

                    return Ok(new { message = "Pedido encontrado", pedido = list, result = true });
                }
                else
                {
                    return BadRequest(new { message = "Pedido não encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
