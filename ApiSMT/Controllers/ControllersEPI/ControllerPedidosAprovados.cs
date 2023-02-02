using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ControleEPI.DTO;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using ControleEPI.BLL.EPIProdutos;
using ControleEPI.BLL.EPICompras;
using ControleEPI.BLL.EPIPedidosAprovados;
using ControleEPI.BLL.EPIPedidos;
using ControleEPI.BLL.EPIProdutosEstoque;
using ControleEPI.BLL.EPIStatus;
using ControleEPI.BLL.EPITamanhos;
using ControleEPI.BLL.RHUsuarios;
using Utilitarios.Utilitários.email;
using ControleEPI.BLL.EPIVinculos;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe ProdutosAprovadosController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerPedidosAprovados : ControllerBase
    {
        private readonly IEPIPedidosAprovadosBLL _pedidosAprovados;
        private readonly IEPIPedidosBLL _pedidos;
        private readonly IEPIComprasBLL _compras;
        private readonly IEPIProdutosBLL _produtos;
        private readonly IRHConUserBLL _usuario;
        private readonly IEPIStatusBLL _status;
        private readonly IEPITamanhosBLL _tamanho;
        private readonly IEPIProdutosEstoqueBLL _estoque;
        private readonly IMailService _mail;
        private readonly IEPIVinculoBLL _vinculo;

        /// <summary>
        /// Construtor ProdutosAprovadosController
        /// </summary>
        /// <param name="pedidosAprovados"></param>
        /// <param name="pedidos"></param>
        /// <param name="compras"></param>
        /// <param name="produtos"></param>
        /// <param name="usuario"></param>
        /// <param name="status"></param>
        /// <param name="tamanho"></param>
        /// <param name="estoque"></param>
        /// <param name="mail"></param>
        /// <param name="vinculo"></param>
        public ControllerPedidosAprovados(IEPIPedidosAprovadosBLL pedidosAprovados, IEPIPedidosBLL pedidos, IEPIComprasBLL compras, IEPIProdutosBLL produtos,
             IRHConUserBLL usuario, IEPIStatusBLL status, IEPITamanhosBLL tamanho, IEPIProdutosEstoqueBLL estoque ,IMailService mail, IEPIVinculoBLL vinculo)
        {
            _pedidosAprovados = pedidosAprovados;
            _pedidos = pedidos;
            _compras = compras;
            _produtos = produtos;
            _usuario = usuario;
            _status = status;
            _tamanho = tamanho;
            _estoque = estoque;
            _mail = mail;
            _vinculo = vinculo;
        }

        /// <summary>
        /// Envia produtos avulsos para compras
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> produtosAvulsos([FromBody] List<EPIPedidosAprovadosDTO> itensAvulsos)
        {
            try
            {
                foreach (var item in itensAvulsos)
                {
                    item.idPedido = 0;

                    await _pedidosAprovados.Insert(item);
                }

                return Ok(new { message = "Itens avulsos inseridos com sucesso!!!", result = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Enviar produtos para compras
        /// </summary>
        /// <param name="enviaCompra"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("enviarCompra/{idUsuario}")]
        public async Task<IActionResult> enviaParaCompras([FromBody] List<EPIPedidosAprovadosDTO> enviaCompra, int idUsuario)
        {
            try
            {
                var localizaUsuario = await _usuario.GetEmp(idUsuario);

                if (localizaUsuario != null)
                {
                    if (enviaCompra != null)
                    {
                        EmailRequestDTO email = new EmailRequestDTO();
                        ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                        List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();
                        List<PedidosAprovados> pedidosAprovados = new List<PedidosAprovados>();
                        EPIComprasDTO compras = new EPIComprasDTO();

                        decimal valorTotalCompra = 0;
                        string tamanho = string.Empty;

                        foreach (var produto in enviaCompra)
                        {
                            pedidosAprovados.Add(new PedidosAprovados
                            {
                                idPedidosAprovados = produto.id
                            });

                            var localizaPedido = await _pedidos.getPedido(produto.idPedido);
                            var localizaProduto = await _produtos.localizaProduto(produto.idProduto);
                            var checkStatusItem = await _status.getStatus(4);

                            if (localizaPedido != null)
                            {
                                var getEmail = await _usuario.getEmail(localizaPedido.idUsuario);

                                EPITamanhosDTO localizaTamanho = new EPITamanhosDTO();

                                foreach (var item in localizaPedido.produtos)
                                {
                                    var localizaProdutoEstoque = await _estoque.getProdutoEstoqueTamanho(item.id, item.tamanho);

                                    if (localizaProdutoEstoque != null || !localizaProdutoEstoque.Equals(0))
                                    {
                                        localizaTamanho = await _tamanho.localizaTamanho(item.tamanho);
                                    }
                                }

                                if (localizaTamanho != null || !localizaTamanho.Equals(0))
                                {
                                    tamanho = "";
                                }
                                else
                                {
                                    tamanho = localizaTamanho.tamanho;
                                }

                                conteudoEmails.Add(new ConteudoEmailDTO
                                {
                                    nome = localizaProduto.nome,
                                    tamanho = tamanho,
                                    status = checkStatusItem.nome,
                                    quantidade = produto.quantidade
                                });

                                email.EmailDe = getEmail.valor;
                                email.EmailPara = "fabiana.lie@reisoffice.com.br";
                                email.ConteudoColaborador = conteudoEmailColaborador;
                                email.Conteudo = conteudoEmails;
                                email.Assunto = "EPI enviado para compras";

                                await _mail.SendEmailAsync(email);
                            }
                            else
                            {
                                produto.enviadoCompra = "S";
                            }

                            await _pedidosAprovados.Update(produto);
                        }

                        compras.idPedidosAprovados = pedidosAprovados;
                        compras.dataCadastroCompra = DateTime.Now;

                        foreach (var item in pedidosAprovados)
                        {
                            var localizaPedidoAprovado = await _pedidosAprovados.getProdutoAprovado(item.idPedidosAprovados, "S");
                            var localizaProduto = await _produtos.localizaProduto(localizaPedidoAprovado.idProduto);
                            decimal valorTotal = localizaPedidoAprovado.quantidade * localizaProduto.preco;
                            valorTotalCompra = valorTotalCompra + valorTotal;
                        }

                        compras.valorTotalCompra = valorTotalCompra;
                        compras.status = 1;
                        compras.idUsuario = idUsuario;
                        compras.dataFinalizacaoCompra = DateTime.MinValue;

                        var insereCompra = await _compras.Insert(compras);

                        if (insereCompra != null)
                        {
                            return Ok(new { message = "Produtos enviados para compra com sucesso!!!", result = true });
                        }
                        else
                        {
                            return BadRequest(new { message = "Erro ao enviar produtos para compra", result = false });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Nenhum item selecionado para envio", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Colaborador com varios contratos ativos ou nenhum, verifique no portal do RH", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Localiza um produto aprovado
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> localizaProdutoAprovado(int id, string status)
        {
            try
            {
                var localizaProduto = await _pedidosAprovados.getProdutoAprovado(id, status);

                if (localizaProduto != null)
                {
                    List<object> produto = new List<object>();

                    produto.Add(new { 
                        nome = _produtos.localizaProduto(localizaProduto.idProduto).Result.nome,
                        quantidade = localizaProduto.quantidade
                    });

                    return Ok(new { message = "Produto encontrado", result = true, dado = produto });
                }
                else
                {
                    return BadRequest(new { message = "Produto não encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Aprovar para vinculo
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut("aprovarVinculo")]
        public async Task<IActionResult> aprovarVinculo([FromBody] List<EPIPedidosAprovadosDTO> enviaCompra)
        {
            try
            {
                foreach (var item in enviaCompra)
                {
                    var localizaPedido = await _pedidos.getPedido(item.idPedido);
                    var localizaProduto = await _produtos.localizaProduto(item.idProduto);

                    EPIVinculoDTO novoVinculo = new EPIVinculoDTO();

                    novoVinculo.idUsuario = localizaPedido.idUsuario;
                    novoVinculo.idItem = item.idProduto;
                    novoVinculo.dataVinculo = DateTime.Now;
                    novoVinculo.status = 13;
                    novoVinculo.dataDevolucao = DateTime.MinValue;
                    novoVinculo.validade = DateTime.Now.AddYears(localizaProduto.validadeEmUso);

                    await _vinculo.insereVinculo(novoVinculo);

                    item.liberadoVinculo = "S";

                    await _pedidosAprovados.Update(item);                    
                }

                return Ok(new { message = "Itens aprovados para vinculo com sucesso!!!", result = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista todos os produtos aprovados
        /// </summary>
        /// <param name="statusCompra"></param>
        /// <param name="statusVinculo"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("produtosAprovados/{statusCompra}/{statusVinculo}")]
        public async Task<IActionResult> produtosAprovados(string statusCompra, string statusVinculo)
        {
            try
            {
                var produtosAprovados = await _pedidosAprovados.getProdutosAprovados(statusCompra, statusVinculo);
                List<object> produtos = new List<object>();

                foreach (var item in produtosAprovados)
                {
                    var localizaProduto = _produtos.localizaProduto(item.idProduto).Result;
                    var localizaTamanho = _tamanho.localizaTamanho(item.idTamanho).Result;
                    var localizaPedido = _pedidos.getPedido(item.idPedido).Result;

                    if (localizaPedido != null)
                    {
                        var localizaUsuario = _usuario.GetEmp(localizaPedido.idUsuario).Result;
                        var localizaEstoque = _estoque.getProdutoExistente(item.idProduto).Result;

                        produtos.Add(new
                        {
                            idProdutoAprovado = item.id,
                            enviadoCompra = item.enviadoCompra,
                            idPedido = item.idPedido,
                            idProduto = localizaProduto.id,
                            nome = localizaProduto.nome,
                            idTamanho = localizaTamanho.id,
                            tamanho = localizaTamanho.tamanho,
                            quantidade = item.quantidade,
                            idUsuario = localizaUsuario.id,
                            usuario = localizaUsuario.nome,
                            dataPedido = localizaPedido.dataPedido,
                            estoque = localizaEstoque.quantidade,
                            liberadoVinculo = item.liberadoVinculo
                        });
                    }
                    else
                    {
                        var localizaEstoque = _estoque.getProdutoExistente(item.idProduto).Result;

                        produtos.Add(new
                        {
                            idProdutoAprovado = item.id,
                            enviadoCompra = item.enviadoCompra,
                            idPedido = item.idPedido,
                            idProduto = localizaProduto.id,
                            nome = localizaProduto.nome,
                            idTamanho = localizaTamanho.id,
                            tamanho = localizaTamanho.tamanho,
                            quantidade = item.quantidade,
                            idUsuario = 0,
                            usuario = string.Empty,
                            dataPedido = string.Empty,
                            estoque = localizaEstoque.quantidade,
                            liberadoVinculo = item.liberadoVinculo
                        });
                    }
                }

                if (produtos != null)
                {
                    return Ok(new { message = "Produtos encontrados", result = true, lista = produtos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum produto aprovado encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
