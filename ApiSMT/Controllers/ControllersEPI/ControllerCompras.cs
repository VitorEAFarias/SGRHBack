using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ControleEPI.DTO;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using ControleEPI.BLL.EPIProdutos;
using ControleEPI.BLL.EPICompras;
using ControleEPI.BLL.EPILogCompras;
using ControleEPI.BLL.EPILogEstoque;
using ControleEPI.BLL.EPIPedidosAprovados;
using ControleEPI.BLL.EPIPedidos;
using ControleEPI.BLL.EPIProdutosEstoque;
using ControleEPI.BLL.EPIStatus;
using ControleEPI.BLL.EPITamanhos;
using ControleEPI.BLL.RHUsuarios;
using ControleEPI.BLL.RHDepartamentos;
using ControleEPI.BLL.RHContratos;
using Utilitarios.Utilitários.email;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe que manipula as informações relacionadas a compras de produtos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerCompras : ControllerBase
    {
        private readonly IEPIComprasBLL _compras;
        private readonly IEPIPedidosBLL _pedidos;
        private readonly IEPIProdutosBLL _produtos;
        private readonly IEPILogEstoqueBLL _log;
        private readonly IEPILogComprasBLL _logCompras;
        private readonly IEPIProdutosEstoqueBLL _produtoEstoque;
        private readonly IEPIStatusBLL _status;
        private readonly IRHConUserBLL _usuario;
        private readonly IEPIPedidosAprovadosBLL _pedidoAprovado;
        private readonly IRHEmpContratosBLL _contrato;
        private readonly IMailService _mail;
        private readonly IEPITamanhosBLL _tamanhos;
        private readonly IRHDepartamentosBLL _departamento;

        /// <summary>
        /// Construtor ComprasController
        /// </summary>
        /// <param name="compras"></param>
        /// <param name="pedidos"></param>
        /// <param name="produtos"></param>
        /// <param name="log"></param>
        /// <param name="status"></param>
        /// <param name="usuario"></param>
        /// <param name="pedidoAprovado"></param>
        /// <param name="produtoEstoque"></param>
        /// <param name="logCompras"></param>
        /// <param name="contrato"></param>
        /// <param name="mail"></param>
        /// <param name="tamanhos"></param>
        /// <param name="departamento"></param>
        public ControllerCompras(IEPIComprasBLL compras, IEPIPedidosBLL pedidos, IEPIProdutosBLL produtos, IEPILogEstoqueBLL log, IEPIStatusBLL status,
            IRHConUserBLL usuario, IEPIPedidosAprovadosBLL pedidoAprovado, IEPIProdutosEstoqueBLL produtoEstoque, IEPILogComprasBLL logCompras,
            IRHEmpContratosBLL contrato, IMailService mail, IEPITamanhosBLL tamanhos, IRHDepartamentosBLL departamento)
        {
            _compras = compras;
            _pedidos = pedidos;
            _produtos = produtos;
            _log = log;
            _logCompras = logCompras;
            _status = status;
            _usuario = usuario;
            _pedidoAprovado = pedidoAprovado;
            _produtoEstoque = produtoEstoque;
            _contrato = contrato;
            _mail = mail;
            _tamanhos = tamanhos;
            _departamento = departamento;
        }

        /// <summary>
        /// Lista todas as compras
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{status}")]
        public async Task<IActionResult> getcompras(string status)
        {
            try
            {
                var compras = await _compras.getCompras(status);
                EPIStatusDTO nomeStatus = new EPIStatusDTO();

                List<object> compra = new List<object>();
                List<object> compraProdutos = new List<object>();

                if (compras != null)
                {
                    foreach (var item in compras)
                    {
                        foreach (var pedidosAprovados in item.idPedidosAprovados)
                        {
                            var localizaProdutoAprovado = await _pedidoAprovado.getProdutoAprovado(pedidosAprovados.idPedidosAprovados, "S");
                            var localizaProduto = await _produtos.localizaProduto(localizaProdutoAprovado.idProduto);

                            compraProdutos.Add(new
                            {
                                localizaProdutoAprovado.idPedido,
                                localizaProdutoAprovado.idProduto,
                                localizaProduto.nome
                            });
                        }

                        nomeStatus = await _status.getStatus(item.status);
                        var nomeEmp = await _usuario.GetEmp(item.idUsuario);

                        compra.Add(new
                        {
                            idCompra = item.id,
                            ProdutosAprovados = compraProdutos,
                            DataCadastroCompra = item.dataCadastroCompra,
                            DataFinalizacaoCompra = item.dataFinalizacaoCompra,
                            ValorTotalCompra = item.valorTotalCompra,
                            Status = nomeStatus.nome,
                            Usuario = nomeEmp.nome
                        });
                    }

                    return Ok(new { message = "Compras encontradas", result = true, lista = compra });
                }
                else
                {
                    return BadRequest(new { message = "Nenhuma compra encontrada com o status de: '" + status + "'" });
                }
            }
            catch (System.Exception ex)
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
        public async Task<ActionResult<EPIComprasDTO>> getCompra(int id)
        {
            try
            {
                var localizaCompra = await _compras.getCompra(id);

                List<object> compra = new List<object>();
                List<object> compraProdutos = new List<object>();

                if (localizaCompra != null)
                {
                    foreach (var item in localizaCompra.idPedidosAprovados)
                    {
                        var localizaProdutoAprovado = await _pedidoAprovado.getProdutoAprovado(item.idPedidosAprovados, "S");
                        var localizaProduto = await _produtos.localizaProduto(localizaProdutoAprovado.idProduto);

                        compraProdutos.Add(new
                        {
                            localizaProdutoAprovado.idPedido,
                            localizaProdutoAprovado.idProduto,
                            localizaProduto.nome
                        });
                    }

                    var nomeStatus = await _status.getStatus(localizaCompra.status);
                    var nomeEmp = await _usuario.GetEmp(localizaCompra.idUsuario);

                    compra.Add(new
                    {
                        idCompra = localizaCompra.id,
                        ProdutosAprovados = compraProdutos,
                        DataCadastraCompra = localizaCompra.dataCadastroCompra,
                        DataFinalizacaoCompra = localizaCompra.dataFinalizacaoCompra,
                        ValorTotalCompra = localizaCompra.valorTotalCompra,
                        Status = nomeStatus.nome,
                        Usuario = nomeEmp.nome
                    });

                    return Ok(new { message = "Compra encontrada: '" + compra, result = true });
                }
                else
                {
                    return BadRequest(new { message = "Compra não encontrada", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cadastra uma nova compra
        /// </summary>
        /// <param name="produtosAprovados"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> cadastraCompra(int idUsuario, [FromBody] List<EPIPedidosAprovadosDTO> produtosAprovados)
        {
            try
            {
                if (produtosAprovados != null)
                {
                    EPIComprasDTO cadastraCompra = new EPIComprasDTO();
                    List<PedidosAprovados> pedidosAprovados = new List<PedidosAprovados>();

                    foreach (var item in produtosAprovados)
                    {
                        pedidosAprovados.Add(new PedidosAprovados
                        {
                            idPedidosAprovados = item.id
                        });
                    }

                    cadastraCompra.idPedidosAprovados = pedidosAprovados;
                    cadastraCompra.dataCadastroCompra = DateTime.Now;

                    decimal valorTotalCompra = 0;

                    foreach (var item in pedidosAprovados)
                    {
                        var localizaProdutoAprovado = await _pedidoAprovado.getProdutoAprovado(item.idPedidosAprovados, "S");
                        var localizaProduto = await _produtos.localizaProduto(localizaProdutoAprovado.idProduto);

                        var valorTotalProduto = localizaProdutoAprovado.quantidade * localizaProduto.preco;

                        valorTotalCompra = valorTotalCompra + valorTotalProduto;
                    }

                    cadastraCompra.valorTotalCompra = valorTotalCompra;
                    cadastraCompra.status = 1;
                    cadastraCompra.idUsuario = idUsuario;
                    cadastraCompra.dataFinalizacaoCompra = DateTime.MinValue;

                    var insereCompra = await _compras.Insert(cadastraCompra);

                    if (insereCompra != null)
                    {
                        return Ok(new { message = "Compra cadastrada com sucesso!!!", result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Erro ao cadastrar compra", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Nenhum produto selecionado", result = false });
                }

            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Efetuar compra e atualizar estoque
        /// </summary>
        /// <param name="compras"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("compras")]
        public async Task<ActionResult> aprovarCompra([FromBody] EPIComprasDTO compras)
        {
            try
            {
                var localizaCompra = await _compras.getCompra(compras.id);

                if (localizaCompra != null)
                {
                    EmailRequestDTO email = new EmailRequestDTO();
                    ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                    List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();

                    var getEmp = await _usuario.GetEmp(localizaCompra.idUsuario);
                    var getEmail = await _usuario.getEmail(localizaCompra.id);

                    if (getEmail != null || !getEmail.Equals(0))
                    {
                        var localizaContrato = await _contrato.getContrato(localizaCompra.id);

                        if (localizaContrato != null || !localizaContrato.Equals(0))
                        {
                            var localizaDepartamento = await _departamento.getDepartamento(localizaContrato.id_departamento);
                            string numeroPedidos = string.Empty;

                            foreach (var compra in localizaCompra.idPedidosAprovados)
                            {
                                var localizaPedidoAprovado = await _pedidoAprovado.getProdutoAprovado(compra.idPedidosAprovados, "S");
                                var localizaProduto = await _produtos.localizaProduto(localizaPedidoAprovado.id);
                                var verificaTamanho = await _tamanhos.localizaTamanho(localizaPedidoAprovado.idTamanho);
                                var nomeStatus = await _status.getStatus(14);

                                conteudoEmails.Add(new ConteudoEmailDTO
                                {
                                    nome = localizaProduto.nome,
                                    tamanho = verificaTamanho.tamanho,
                                    status = nomeStatus.nome,
                                    quantidade = localizaPedidoAprovado.quantidade
                                });

                                numeroPedidos += localizaPedidoAprovado.id;
                            }

                            conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                            {
                                idPedido = numeroPedidos,
                                nomeColaborador = getEmp.nome,
                                departamento = localizaDepartamento.titulo
                            };

                            email.EmailDe = getEmail.valor;
                            email.EmailPara = "fabiana.lie@reisoffice.com.br";
                            email.ConteudoColaborador = conteudoEmailColaborador;
                            email.Conteudo = conteudoEmails;
                            email.Assunto = "Atualização de Pedido";

                            await _mail.SendEmailAsync(email);

                            return Ok(new { message = "Compras aprovadas com sucesso!!!", result = true });
                        }
                        else
                        {
                            return BadRequest(new { message = "Colaborador com mais de um contrato ativo ou nenhum, verifique no Portal do RH", result = false });
                        }                        
                    }
                    else
                    {
                        return BadRequest(new { message = "É necessário ter email funcional vinculado ao perfil, verifique no RH", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Nenhuma compra selecionada para aprovação", result = false });
                }

            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Reprovar compra
        /// </summary>
        /// <param name="reprovarCompra"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("reprovar/idUsuario")]
        public async Task<IActionResult> reprovaCompra([FromBody] List<EPIComprasDTO> reprovarCompra, int idUsuario)
        {
            try
            {
                var localizaUsuarioReprova = await _usuario.GetEmp(idUsuario);
                EmailRequestDTO email = new EmailRequestDTO();
                ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();

                if (localizaUsuarioReprova != null || !localizaUsuarioReprova.Equals(0))
                {
                    var getEmail = await _usuario.getEmail(localizaUsuarioReprova.id);

                    if (getEmail != null || !getEmail.Equals(0))
                    {
                        var localizaContrato = await _contrato.getContrato(localizaUsuarioReprova.id);

                        if (localizaContrato != null || !localizaContrato.Equals(0))
                        {
                            if (reprovarCompra != null)
                            {
                                foreach (var item in reprovarCompra)
                                {
                                    var localizaCompra = await _compras.getCompra(item.id);
                                    RHEmpContatoDTO usuarioPedidoEmail = new RHEmpContatoDTO();

                                    foreach (var produto in item.idPedidosAprovados)
                                    {
                                        List<Produtos> produtos = new List<Produtos>();

                                        var localizaProdutoAprovado = await _pedidoAprovado.getProdutoAprovado(produto.idPedidosAprovados, "S");
                                        var localizaPedido = await _pedidos.getPedido(localizaProdutoAprovado.idPedido);
                                        var usuarioPedido = await _usuario.GetEmp(localizaPedido.idUsuario);
                                        usuarioPedidoEmail = await _usuario.getEmail(localizaPedido.idUsuario);
                                        var usuarioPedidoContrato = await _contrato.getEmpContrato(localizaPedido.idUsuario);
                                        var usuarioPedidoDepartamento = await _departamento.getDepartamento(usuarioPedidoContrato.id_departamento);

                                        foreach (var pedidoProduto in localizaPedido.produtos)
                                        {
                                            if (pedidoProduto.id == localizaProdutoAprovado.idProduto && pedidoProduto.tamanho == localizaProdutoAprovado.idTamanho && 
                                                pedidoProduto.quantidade == localizaProdutoAprovado.quantidade)
                                            {
                                                var nomeTamanho = await _tamanhos.localizaTamanho(pedidoProduto.tamanho);
                                                var nomeStatus = await _status.getStatus(pedidoProduto.status);

                                                conteudoEmails.Add(new ConteudoEmailDTO
                                                {
                                                    nome = pedidoProduto.nome,
                                                    tamanho = nomeTamanho.tamanho,
                                                    status = nomeStatus.nome,
                                                    quantidade = pedidoProduto.quantidade
                                                });

                                                produtos.Add(new Produtos {
                                                    id = pedidoProduto.id,
                                                    nome = pedidoProduto.nome,
                                                    quantidade = pedidoProduto.quantidade,
                                                    status = 3,
                                                    tamanho = pedidoProduto.tamanho
                                                });
                                            }
                                            else
                                            {
                                                produtos.Add(new Produtos {
                                                    id = pedidoProduto.id,
                                                    nome = pedidoProduto.nome,
                                                    quantidade = pedidoProduto.quantidade,
                                                    status = pedidoProduto.status,
                                                    tamanho = pedidoProduto.tamanho
                                                });
                                            }
                                        }

                                        var verificaPedidoInserido = await _pedidos.getPedido(localizaPedido.id);
                                        int contador = 0;

                                        foreach (var verifica in verificaPedidoInserido.produtos)
                                        {
                                            if (verifica.status == 2 && verifica.status == 3 && verifica.status == 7)
                                            {
                                                contador++;
                                            }
                                        }

                                        if (contador == verificaPedidoInserido.produtos.Count)
                                        {
                                            localizaPedido.status = 10;
                                        }
                                        else
                                        {
                                            localizaPedido.status = localizaPedido.status;
                                        }

                                        localizaPedido.produtos = produtos;

                                        await _pedidos.Update(localizaPedido);

                                        conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                                        {
                                            idPedido = localizaPedido.id.ToString(),
                                            nomeColaborador = usuarioPedido.nome,
                                            departamento = usuarioPedidoDepartamento.titulo
                                        };
                                    }

                                    localizaCompra.status = 3;
                                    localizaCompra.idUsuario = idUsuario;
                                    localizaCompra.dataFinalizacaoCompra = DateTime.Now;

                                    await _compras.Update(localizaCompra);

                                    email.EmailDe = getEmail.valor;
                                    email.EmailPara = $"fabiana.lie@reisoffice.com.br, {usuarioPedidoEmail.valor}";
                                    email.ConteudoColaborador = conteudoEmailColaborador;
                                    email.Conteudo = conteudoEmails;
                                    email.Assunto = "Produto de pedido de EPI reprovado";

                                    await _mail.SendEmailAsync(email);
                                }

                                return Ok(new { message = "Compras reprovadas com sucesso!!!", result = true });
                            }
                            else
                            {
                                return BadRequest(new { message = "Nenhuma compra enviada para reprovação", result = false });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "Colaborador sem contrato ativo ou com mais de um, verifique no Portal do RH", result = false });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "É necessário que o colaborador tenha um email funcional vinculado, verifique no Portal do RH", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Colaborador não encontrado, verifique no portal do RH", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Realizar compra e atualizar status do pedido
        /// </summary>
        /// <param name="idCompra"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("comprar/{idCompra}")]
        public async Task<IActionResult> efetuarCompra(int idCompra, int idUsuario)
        {
            try
            {
                EPIComprasDTO localizaCompra = await _compras.getCompra(idCompra);
                EPIPedidosDTO localizaPedido = new EPIPedidosDTO();
                EPIProdutosEstoqueDTO localizaEstoque = new EPIProdutosEstoqueDTO();
                EPILogComprasDTO logCompras = new EPILogComprasDTO();
                EPILogEstoqueDTO logEstoque = new EPILogEstoqueDTO();
                int quantidadeAtual = 0;

                if (localizaCompra != null)
                {
                    List<Produtos> pedidoProduto = new List<Produtos>();

                    foreach (var item in localizaCompra.idPedidosAprovados)
                    {
                        var localizaProdutoAprovado = await _pedidoAprovado.getProdutoAprovado(item.idPedidosAprovados, "S");
                        localizaPedido = await _pedidos.getPedido(localizaProdutoAprovado.idPedido);

                        foreach (var produto in localizaPedido.produtos)
                        {
                            var localizaProduto = await _produtos.localizaProduto(produto.id);

                            if (produto.id == localizaProdutoAprovado.idProduto && produto.tamanho.ToString().IsNullOrEmpty())
                            {
                                pedidoProduto.Add(new Produtos
                                {
                                    id = produto.id,
                                    nome = localizaProduto.nome,
                                    quantidade = localizaProdutoAprovado.quantidade,
                                    status = 7,
                                    tamanho = localizaProdutoAprovado.idTamanho
                                });

                                localizaEstoque = await _produtoEstoque.getProdutoEstoqueTamanho(produto.id, produto.tamanho);

                                quantidadeAtual = localizaEstoque.quantidade;

                                localizaEstoque.quantidade = localizaEstoque.quantidade + produto.quantidade;

                                await _produtoEstoque.Update(localizaEstoque);

                                logEstoque.idProduto = produto.id;
                                logEstoque.idUsuario = idUsuario;
                                logEstoque.de = quantidadeAtual;
                                logEstoque.para = localizaEstoque.quantidade;
                                logEstoque.quantidadeMovimentada = quantidadeAtual - produto.quantidade;
                                logEstoque.dataAlteracao = DateTime.Now;
                                logEstoque.retirada = false;
                                logEstoque.automatico = true;

                                await _log.Insert(logEstoque);
                            }
                            else if (produto.id == localizaProdutoAprovado.idProduto && produto.tamanho == localizaProdutoAprovado.idTamanho)
                            {
                                pedidoProduto.Add(new Produtos
                                {
                                    id = produto.id,
                                    nome = localizaProduto.nome,
                                    quantidade = localizaProdutoAprovado.quantidade,
                                    status = 7,
                                    tamanho = localizaProdutoAprovado.idTamanho
                                });

                                localizaEstoque = await _produtoEstoque.getProdutoEstoqueTamanho(produto.id, produto.tamanho);

                                quantidadeAtual = localizaEstoque.quantidade;

                                localizaEstoque.quantidade = localizaEstoque.quantidade + produto.quantidade;

                                await _produtoEstoque.Update(localizaEstoque);

                                logEstoque.idProduto = produto.id;
                                logEstoque.idUsuario = idUsuario;
                                logEstoque.de = quantidadeAtual;
                                logEstoque.para = localizaEstoque.quantidade;
                                logEstoque.quantidadeMovimentada = quantidadeAtual - produto.quantidade;
                                logEstoque.dataAlteracao = DateTime.Now;
                                logEstoque.retirada = false;
                                logEstoque.automatico = true;

                                await _log.Insert(logEstoque);
                            }
                            else
                            {
                                pedidoProduto.Add(new Produtos
                                {
                                    id = produto.id,
                                    nome = localizaProduto.nome,
                                    quantidade = localizaProdutoAprovado.quantidade,
                                    status = produto.status,
                                    tamanho = localizaProdutoAprovado.idTamanho
                                });
                            }
                        }
                    }

                    int itensCompra = 0;

                    foreach (var item in pedidoProduto)
                    {
                        if (item.status == 7)
                        {
                            itensCompra++;
                        }
                    }

                    if (itensCompra == localizaCompra.idPedidosAprovados.Count)
                    {
                        logCompras.valor = localizaCompra.valorTotalCompra;
                        logCompras.idCompra = localizaCompra.id;
                        logCompras.idUsuario = _usuario.GetEmp(idUsuario).Result.id;
                        logCompras.dataCompra = DateTime.Now;

                        localizaCompra.dataFinalizacaoCompra = DateTime.Now;

                        await _logCompras.insereLogCompra(logCompras);
                    }

                    int contador = 0;

                    foreach (var status in pedidoProduto)
                    {
                        if (status.status == 3 || status.status == 7 || status.status == 11)
                            contador++;
                    }

                    localizaPedido.produtos = pedidoProduto;

                    if (contador == pedidoProduto.Count)
                    {
                        localizaPedido.status = 3;
                    }
                    else
                    {
                        localizaPedido.status = localizaPedido.status;
                    }

                    localizaCompra.status = 7;

                    await _pedidos.Update(localizaPedido);
                    await _compras.Update(localizaCompra);

                    return Ok(new { message = "Compra realizada com sucesso!!!", result = true });
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
    }
}
