using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ControleEPI.DTO;
using ControleEPI.BLL;
using System.Collections.Generic;
using System;

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
        private readonly IEPIPedidosAprovadosBLL _produtoAprovado;

        /// <summary>
        /// Construtor ComprasController
        /// </summary>
        /// <param name="compras"></param>
        /// <param name="pedidos"></param>
        /// <param name="produtos"></param>
        /// <param name="log"></param>
        /// <param name="status"></param>
        /// <param name="usuario"></param>
        /// <param name="produtoAprovado"></param>
        /// <param name="produtoEstoque"></param>
        /// <param name="logCompras"></param>
        public ControllerCompras(IEPIComprasBLL compras, IEPIPedidosBLL pedidos, IEPIProdutosBLL produtos, IEPILogEstoqueBLL log, IEPIStatusBLL status,
            IRHConUserBLL usuario, IEPIPedidosAprovadosBLL produtoAprovado, IEPIProdutosEstoqueBLL produtoEstoque, IEPILogComprasBLL logCompras)
        {
            _compras = compras;
            _pedidos = pedidos;
            _produtos = produtos;
            _log = log;
            _logCompras = logCompras;
            _status = status;
            _usuario = usuario;
            _produtoAprovado = produtoAprovado;
            _produtoEstoque = produtoEstoque;
        }

        /// <summary>
        /// Lista todas as compras
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
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
                        foreach (var pedidosAprovados in item.pedidosAprovados)
                        {
                            var localizaProdutoAprovado = await _produtoAprovado.getProdutoAprovado(pedidosAprovados.idPedidosAprovados, "S");
                            var localizaProduto = await _produtos.getProduto(localizaProdutoAprovado.idProduto);

                            compraProdutos.Add(new
                            {
                                idPedido = localizaProdutoAprovado.idPedido,
                                idProduto = localizaProdutoAprovado.idProduto,
                                nomeProduto = localizaProduto.nomeProduto
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
                    foreach (var item in localizaCompra.pedidosAprovados)
                    {
                        var localizaProdutoAprovado = await _produtoAprovado.getProdutoAprovado(item.idPedidosAprovados, "S");
                        var localizaProduto = await _produtos.getProduto(localizaProdutoAprovado.idProduto);

                        compraProdutos.Add(new {
                            idPedido = localizaProdutoAprovado.idPedido,
                            idProduto = localizaProdutoAprovado.idProduto,
                            nomeProduto = localizaProduto.nomeProduto
                        });
                    }

                    var nomeStatus = await _status.getStatus(localizaCompra.status);
                    var nomeEmp = await _usuario.GetEmp(localizaCompra.idUsuario);

                    compra.Add(new {
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

                    cadastraCompra.pedidosAprovados = pedidosAprovados;
                    cadastraCompra.dataCadastroCompra = DateTime.Now;

                    decimal valorTotalCompra = 0;

                    foreach (var item in pedidosAprovados)
                    {
                        var localizaProdutoAprovado = await _produtoAprovado.getProdutoAprovado(item.idPedidosAprovados, "S");
                        var localizaProduto = await _produtos.getProduto(localizaProdutoAprovado.idProduto);

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
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [HttpPut("{idUsuario}")]
        public async Task<ActionResult> aprovarCompra(List<EPIComprasDTO> compras, int idUsuario)
        {
            try
            {
                if (compras != null)
                {
                    foreach (var compra in compras)
                    {
                        var localizaCompra = await _compras.getCompra(compra.id);

                        localizaCompra.status = 9;
                        localizaCompra.idUsuario = idUsuario;
                        localizaCompra.idFornecedor = compra.idFornecedor;

                        await _compras.Update(localizaCompra);
                    }

                    return Ok(new { message = "Compras aprovadas com sucesso!!!", result = true });
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
        [HttpPut("reprovar/idUsuario")]
        public async Task<IActionResult> reprovaCompra([FromBody] List<EPIComprasDTO> reprovarCompra, int idUsuario)
        {
            try
            {
                if (reprovarCompra != null)
                {
                    foreach (var item in reprovarCompra)
                    {
                        var localizaCompra = await _compras.getCompra(item.id);

                        localizaCompra.status = 3;
                        localizaCompra.idUsuario = idUsuario;
                        localizaCompra.dataFinalizacaoCompra = DateTime.Now;

                        await _compras.Update(localizaCompra);                            
                    }

                    return Ok(new { message = "Compras reprovadas com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Nenhuma compra enviada para reprovação", result = false});
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

                    foreach (var item in localizaCompra.pedidosAprovados)
                    {
                        var localizaProdutoAprovado = await _produtoAprovado.getProdutoAprovado(item.idPedidosAprovados, "S");
                        localizaPedido = await _pedidos.getPedido(localizaProdutoAprovado.idPedido);

                        foreach (var produto in localizaPedido.produtos)
                        {
                            var localizaProduto = await _produtos.getProduto(produto.id);

                            if (produto.id == localizaProdutoAprovado.idProduto && produto.idTamanho == 0)
                            {
                                pedidoProduto.Add(new Produtos
                                {
                                    id = produto.id,
                                    nome = localizaProduto.nomeProduto,
                                    quantidade = localizaProdutoAprovado.quantidade,
                                    status = 7,
                                    idTamanho = localizaProdutoAprovado.idTamanho
                                });

                                localizaEstoque = await _produtoEstoque.getProdutoEstoqueTamanho(produto.id, produto.idTamanho);

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
                            else if (produto.id == localizaProdutoAprovado.idProduto && produto.idTamanho == localizaProdutoAprovado.idTamanho)
                            {
                                pedidoProduto.Add(new Produtos
                                {
                                    id = produto.id,
                                    nome = localizaProduto.nomeProduto,
                                    quantidade = localizaProdutoAprovado.quantidade,
                                    status = 7,
                                    idTamanho = localizaProdutoAprovado.idTamanho
                                });

                                localizaEstoque = await _produtoEstoque.getProdutoEstoqueTamanho(produto.id, produto.idTamanho);

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
                                    nome = localizaProduto.nomeProduto,
                                    quantidade = localizaProdutoAprovado.quantidade,
                                    status = produto.status,
                                    idTamanho = localizaProdutoAprovado.idTamanho
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

                    if (itensCompra == localizaCompra.pedidosAprovados.Count)
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
