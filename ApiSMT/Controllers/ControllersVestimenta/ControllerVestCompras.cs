using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.BLL;
using Vestimenta.DTO;
using System;
using System.Collections.Generic;
using ControleEPI.BLL;
using Microsoft.AspNetCore.Authorization;
using ControleEPI.DTO.E_Mail;
using Vestimenta.DTO.FromBody;
using ApiSMT.Utilitários;
using ControleEPI.DTO;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe de Compras
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerVestCompras : ControllerBase
    {
        private readonly IComprasVestBLL _comprasVest;
        private readonly IEstoqueBLL _estoque;
        private readonly IVestimentaBLL _vestimenta;
        private readonly ILogBLL _log;
        private readonly IPedidosVestBLL _pedidos;
        private readonly IVestRepositorioBLL _repositorio;
        private readonly IRHConUserBLL _conuser;
        private readonly IStatusVestBLL _status;
        private readonly IMailService _mail;
        private readonly IRHEmpContratosBLL _contrato;
        private readonly IRHDepartamentosBLL _departamento;

        /// <summary>
        /// Construtor de Compras
        /// </summary>
        /// <param name="comprasVest"></param>
        /// <param name="estoque"></param>
        /// <param name="vestimenta"></param>
        /// <param name="log"></param>
        /// <param name="pedidos"></param>
        /// <param name="repositorio"></param>
        /// <param name="conuser"></param>
        /// <param name="status"></param>
        /// <param name="mail"></param>
        /// <param name="contrato"></param>
        /// <param name="departamento"></param>
        public ControllerVestCompras(IComprasVestBLL comprasVest, IEstoqueBLL estoque, IVestimentaBLL vestimenta, ILogBLL log, IPedidosVestBLL pedidos, 
            IVestRepositorioBLL repositorio, IRHConUserBLL conuser, IStatusVestBLL status, IMailService mail, IRHEmpContratosBLL contrato, IRHDepartamentosBLL departamento)
        {
            _comprasVest = comprasVest;
            _estoque = estoque;
            _vestimenta = vestimenta;
            _log = log;
            _pedidos = pedidos;
            _repositorio = repositorio;
            _conuser = conuser;
            _status = status;
            _mail = mail;
            _contrato = contrato;
            _departamento = departamento;
        }

        /// <summary>
        /// Cadastra uma nova compra
        /// </summary>
        /// <param name="compra"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<VestComprasDTO>> postCadastrarCompra([FromBody] VestComprasDTO compra)
        {
            try
            {
                if (compra != null)
                {
                    var novaCompra = await _comprasVest.Insert(compra);

                    if (novaCompra != null)
                    {
                        EmailRequestDTO email = new EmailRequestDTO();
                        List<VestConteudoEmailDTO> conteudoEmails = new List<VestConteudoEmailDTO>();

                        var getEmail = await _conuser.getEmail(novaCompra.idUsuario);
                        var getEmp = await _conuser.GetEmp(novaCompra.idUsuario);

                        foreach (var item in novaCompra.itensRepositorio)
                        {
                            var nomeItem = await _vestimenta.getVestimenta(item.idItem);
                            var statusItem = await _status.getStatus(novaCompra.status);

                            conteudoEmails.Add(new VestConteudoEmailDTO {
                                nome = nomeItem.nome,
                                tamanho = item.tamanho,
                                status = statusItem.nome,
                                quantidade = item.quantidade
                            });
                        }                        

                        email.EmailDe = getEmail.valor;
                        email.EmailPara = "rinaldo.bordim@reisoffice.com.br";
                        email.Conteudo = conteudoEmails;
                        email.Assunto = "Novo pedido de compra avulsa";

                        await _mail.SendEmailAsync(email);

                        return Ok(new { message = "Compra cadastrada com sucesso!!!", result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Erro ao cadastrar nova compra", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Erro ao cadastrar compra " + compra, result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualizar status da compra para aguardando compra
        /// </summary>
        /// <param name="aguardandoCompra"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("aguardandoCompra/{idCompra}")]
        public async Task<ActionResult> aguardandoCompra([FromBody] VestComprasDTO aguardandoCompra)
        {
            try
            {
                var checkCompra = await _comprasVest.getCompra(aguardandoCompra.id);

                if (checkCompra != null)
                {
                    checkCompra.status = 8;

                    await _comprasVest.Update(checkCompra);

                    EmailRequestDTO email = new EmailRequestDTO();
                    VestConteudoEmailColaboradorDTO conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO();
                    List<VestConteudoEmailDTO> conteudoEmails = new List<VestConteudoEmailDTO>();

                    var checkUsuario = await _conuser.GetEmp(aguardandoCompra.idUsuario);
                    var contrato = await _contrato.getEmpContrato(aguardandoCompra.idUsuario);
                    var departamento = await _departamento.getDepartamento(contrato.id_departamento);
                    var getEmail = await _conuser.getEmail(aguardandoCompra.idUsuario);
                    var getStatusItem = await _status.getStatus(aguardandoCompra.status);

                    foreach (var item in aguardandoCompra.itensRepositorio)
                    {
                        var nomeItem = await _vestimenta.getVestimenta(item.idItem);

                        conteudoEmails.Add(new VestConteudoEmailDTO
                        {
                            nome = nomeItem.nome,
                            tamanho = item.tamanho,
                            status = getStatusItem.nome,
                            quantidade = item.quantidade
                        });
                    }

                    conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO
                    {
                        idPedido = aguardandoCompra.id,
                        nomeColaborador = checkUsuario.nome,
                        departamento = departamento.titulo
                    };

                    email.EmailDe = getEmail.valor;
                    email.EmailPara = "rh@reisoffice.com.br";
                    email.ConteudoColaborador = conteudoEmailColaborador;
                    email.Conteudo = conteudoEmails;
                    email.Assunto = "Pedido de Compra";

                    await _mail.SendEmailAsync(email);

                    return Ok(new { message = "Aguardando compra", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Compra não encontrada", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Compra aprovada
        /// </summary>
        /// <param name="processoCompra"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("processoCompra/{idCompra}")]
        public async Task<ActionResult> processoDeCompra([FromBody] VestComprasDTO processoCompra)
        {
            try
            {
                RHEmpContatoDTO getEmail = new RHEmpContatoDTO();                
                EmailRequestDTO email = new EmailRequestDTO();
                RHEmpregadoDTO checkUsuario = new RHEmpregadoDTO();                
                List<VestConteudoEmailDTO> conteudoEmails = new List<VestConteudoEmailDTO>();
                VestConteudoEmailColaboradorDTO conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO();
                
                VestComprasDTO checkCompra = await _comprasVest.getCompra(processoCompra.id);
                RHEmpContratosDTO contrato = await _contrato.getEmpContrato(processoCompra.idUsuario);
                RHDepartamentosDTO departamento = await _departamento.getDepartamento(contrato.id_departamento);

                if (checkCompra != null)
                {
                    getEmail = await _conuser.getEmail(checkCompra.idUsuario);
                    checkUsuario = await _conuser.GetEmp(processoCompra.idUsuario);

                    checkCompra.status = processoCompra.status;

                    await _comprasVest.Update(checkCompra);

                    foreach (var itensComprar in processoCompra.itensRepositorio)
                    {
                        VestVestimentaDTO vestimenta = new VestVestimentaDTO();

                        var getNome = await _vestimenta.getVestimenta(itensComprar.idItem);
                        var getStatus = await _status.getStatus(processoCompra.status);

                        conteudoEmails.Add(new VestConteudoEmailDTO
                        {
                            nome = getNome.nome,
                            tamanho = itensComprar.tamanho,
                            status = getStatus.nome,
                            quantidade = itensComprar.quantidade
                        });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Compra não encontrada", result = false });
                }

                conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO
                {
                    idPedido = checkCompra.id,
                    nomeColaborador = checkUsuario.nome,
                    departamento = departamento.titulo
                };

                email.EmailDe = getEmail.valor;
                email.EmailPara = "rh@reisoffice.com.br";
                email.ConteudoColaborador = conteudoEmailColaborador;
                email.Conteudo = conteudoEmails;
                email.Assunto = "Pedido de compra";

                await _mail.SendEmailAsync(email);

                return Ok(new { message = "Itens aprovados com sucesso!!!", result = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Reprovar compra
        /// </summary>
        /// <param name="reprovarCompra"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("reprovarCompra/{idCompra}")]
        public async Task<ActionResult> reprovarCompra([FromBody] VestComprasDTO reprovarCompra)
        {
            try
            {
                VestComprasDTO checkCompra = await _comprasVest.getCompra(reprovarCompra.id);

                if (checkCompra != null)
                {
                    EmailRequestDTO email = new EmailRequestDTO();
                    List<VestConteudoEmailDTO> conteudoEmails = new List<VestConteudoEmailDTO>();
                    VestConteudoEmailColaboradorDTO conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO();

                    RHEmpContratosDTO contrato = await _contrato.getEmpContrato(reprovarCompra.idUsuario);
                    RHDepartamentosDTO departamento = await _departamento.getDepartamento(contrato.id_departamento);

                    var getEmail = await _conuser.getEmail(checkCompra.idUsuario);
                    var checkUsuario = await _conuser.GetEmp(reprovarCompra.idUsuario);
                    var nomeStatus = await _status.getStatus(reprovarCompra.status);

                    var nomeItem = string.Empty;
                    var tamanho = string.Empty;
                    int quantidade = 0;

                    if (checkCompra.status != 8)
                    {
                        checkCompra.status = 3;

                        await _comprasVest.Update(checkCompra);

                        foreach (var repositorio in checkCompra.itensRepositorio)
                        {
                            var itemNome = await _vestimenta.getVestimenta(repositorio.idItem);

                            foreach (var idRepositorio in repositorio.idRepositorio)
                            {
                                var getRepositorio = await _repositorio.getRepositorio(idRepositorio);

                                getRepositorio.enviadoCompra = "N";

                                await _repositorio.Update(getRepositorio);
                            }

                            tamanho = repositorio.tamanho;
                            nomeItem = itemNome.nome;
                            quantidade = repositorio.quantidade;                            
                        }

                        conteudoEmails.Add(new VestConteudoEmailDTO
                        {
                            nome = nomeItem,
                            tamanho = tamanho,
                            status = nomeStatus.nome,
                            quantidade = quantidade
                        });

                        conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO
                        {
                            idPedido = checkCompra.id,
                            nomeColaborador = checkUsuario.nome,
                            departamento = departamento.titulo
                        };

                        email.EmailDe = getEmail.valor;
                        email.EmailPara = "rh@reisoffice.com.br";
                        email.ConteudoColaborador = conteudoEmailColaborador;
                        email.Conteudo = conteudoEmails;
                        email.Assunto = "Pedido de compra";

                        await _mail.SendEmailAsync(email);

                        return Ok(new { message = "Compra reprovada com sucesso!!!", result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Essa compra ja foi aprovada", result = false});
                    }
                }
                else
                {
                    return BadRequest(new { message = "Compra não encontrada", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Item comprado e enviado para estoque
        /// </summary>
        /// <param name="comprarItens"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("comprarItens/{idCompra}")]
        public async Task<ActionResult> comprarItem([FromBody] VestComprasDTO comprarItens)
        {
            try
            {
                VestComprasDTO checkCompra = await _comprasVest.getCompra(comprarItens.id);

                if (checkCompra.status == 8)
                {
                    RHEmpContratosDTO contrato = await _contrato.getEmpContrato(comprarItens.idUsuario);
                    RHDepartamentosDTO departamento = await _departamento.getDepartamento(contrato.id_departamento);

                    EmailRequestDTO email = new EmailRequestDTO();                    
                    VestPedidosDTO getPedido = new VestPedidosDTO();
                    VestEstoqueDTO getEstoque = new VestEstoqueDTO();
                    VestStatusDTO getStatusItem = new VestStatusDTO();
                    VestRepositorioDTO getRepositorio = new VestRepositorioDTO();
                    VestConteudoEmailColaboradorDTO conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO();

                    List<VestConteudoEmailDTO> conteudoEmails = new List<VestConteudoEmailDTO>();

                    var getEmail = await _conuser.getEmail(checkCompra.idUsuario);
                    var checkUsuario = await _conuser.GetEmp(comprarItens.idUsuario);                   

                    foreach (var repositorio in checkCompra.itensRepositorio)
                    {
                        foreach (var idRepositorio in repositorio.idRepositorio)
                        {
                            if (idRepositorio != 0)
                            {
                                getRepositorio = await _repositorio.getRepositorio(idRepositorio);
                                getPedido = await _pedidos.getPedido(getRepositorio.idPedido);

                                List<ItemDTO> getItemPedido = new List<ItemDTO>();

                                foreach (var itens in getPedido.item)
                                {                                     
                                    if (itens.id == repositorio.idItem && itens.tamanho == repositorio.tamanho)
                                    {
                                        getItemPedido.Add(new ItemDTO
                                        {
                                            id = itens.id,
                                            nome = itens.nome,
                                            tamanho = itens.tamanho,
                                            quantidade = itens.quantidade,
                                            status = 7,
                                            dataAlteracao = DateTime.Now
                                        });                                        

                                        getStatusItem = await _status.getStatus(7);

                                        conteudoEmails.Add(new VestConteudoEmailDTO
                                        {
                                            nome = itens.nome,
                                            tamanho = itens.tamanho,
                                            status = getStatusItem.nome,
                                            quantidade = itens.quantidade
                                        });
                                    } 
                                    else
                                    {
                                        getItemPedido.Add(new ItemDTO
                                        {
                                            id = itens.id,
                                            nome = itens.nome,
                                            tamanho = itens.tamanho,
                                            quantidade = itens.quantidade,
                                            status = itens.status,
                                            dataAlteracao = itens.dataAlteracao
                                        });

                                        getStatusItem = await _status.getStatus(itens.status);
                                    }
                                }

                                int contador = 0;

                                foreach (var status in getItemPedido)
                                {
                                    if (status.status == 2 || status.status == 7 || status.status == 3 || status.status == 6)
                                        contador++;
                                }

                                getPedido.item = getItemPedido;

                                if (contador == getItemPedido.Count)
                                {
                                    getPedido.status = 2;
                                }
                                else
                                {
                                    getPedido.status = getPedido.status;
                                }

                                await _pedidos.Update(getPedido);
                                await _comprasVest.Update(comprarItens);
                            }
                            else
                            {
                                checkCompra.status = comprarItens.status;

                                await _comprasVest.Update(checkCompra);
                            }

                            getEstoque = await _estoque.getItemExistente(repositorio.idItem, repositorio.tamanho);

                            var quantidadeAnterior = getEstoque.quantidade;

                            if (getEstoque != null)
                            {
                                getEstoque.quantidade = getEstoque.quantidade + repositorio.quantidade;
                                getEstoque.dataAlteracao = DateTime.Now;

                                VestLogDTO log = new VestLogDTO();

                                log.data = DateTime.Now;
                                log.idUsuario = checkCompra.idUsuario;
                                log.idItem = repositorio.idItem;
                                log.quantidadeAnt = quantidadeAnterior;
                                log.quantidadeDep = getEstoque.quantidade;
                                log.tamanho = repositorio.tamanho;
                                log.usado = "N";

                                await _estoque.Update(getEstoque);
                                var insereLogEstoqueDisponivel = await _log.Insert(log);
                            }
                        }  
                    }

                    conteudoEmailColaborador = new VestConteudoEmailColaboradorDTO
                    {
                        idPedido = checkCompra.id,
                        nomeColaborador = checkUsuario.nome,
                        departamento = departamento.titulo
                    };

                    email.EmailDe = getEmail.valor;
                    email.EmailPara = "rh@reisoffice.com.br";
                    email.ConteudoColaborador = conteudoEmailColaborador;
                    email.Conteudo = conteudoEmails;
                    email.Assunto = "Pedido de compra";

                    await _mail.SendEmailAsync(email);

                    return Ok(new { message = "Itens comprados com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Compra '" + checkCompra.id + "' ainda não foi aprovada", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }        

        /// <summary>
        /// Lista todas as compras
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> getCompras()
        {
            try
            {                                
                var compras = await _comprasVest.getCompras();
                List<object> lista = new List<object>();

                foreach (var item in compras)
                {
                    var getUsuario = await _conuser.GetEmp(item.idUsuario);
                    var getStatus = await _status.getStatus(item.status);

                    lista.Add(new
                    {
                        id = item.id,
                        nome = getUsuario.nome,
                        status = getStatus.nome,
                        item.dataCompra,
                        item.itensRepositorio
                    });                                        
                }

                return Ok(new { message = "lista encontrada", result = true, lista = lista });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona uma compra
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<VestComprasDTO>> getCompra(int id)
        {
            try
            {
                var checkCompra = await _comprasVest.getCompra(id);

                if (checkCompra != null)
                {
                    var getUsuario = await _conuser.GetEmp(checkCompra.idUsuario);
                    var getStatus = await _status.getStatus(checkCompra.status);

                    List<object> lista = new List<object>();

                    foreach (var item in checkCompra.itensRepositorio)
                    {
                        var checkVestimenta = await _vestimenta.getVestimenta(item.idItem);
                        var total = item.quantidade * item.preco;

                        lista.Add(new {
                            item.idItem,
                            item.idRepositorio,
                            item.tamanho,
                            item.quantidade,
                            item.preco,
                            precoTotal = total,
                            nome = checkVestimenta.nome
                        });
                    }

                    var itens = new
                    {
                        idUsuario = getUsuario.id,
                        id = checkCompra.id,
                        nome = getUsuario.nome,
                        idStatus = getStatus.id,
                        status = getStatus.nome,
                        checkCompra.dataCompra,
                        itensRepositorio = lista
                    };

                    return Ok(new { message = "Compra encontrada", compra = itens, result = true });
                }
                else
                {
                    return BadRequest(new { message = "Compra não encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
