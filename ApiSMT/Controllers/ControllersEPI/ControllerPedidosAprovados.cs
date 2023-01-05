using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ControleEPI.BLL;
using ControleEPI.DTO;
using System.Collections.Generic;
using System;

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

        /// <summary>
        /// Construtor ProdutosAprovadosController
        /// </summary>
        /// <param name="pedidosAprovados"></param>
        /// <param name="pedidos"></param>
        /// <param name="compras"></param>
        /// <param name="produtos"></param>
        public ControllerPedidosAprovados(IEPIPedidosAprovadosBLL pedidosAprovados, IEPIPedidosBLL pedidos, IEPIComprasBLL compras, IEPIProdutosBLL produtos)
        {
            _pedidosAprovados = pedidosAprovados;
            _pedidos = pedidos;
            _compras = compras;
            _produtos = produtos;
        }

        /// <summary>
        /// Enviar produtos para compras
        /// </summary>
        /// <param name="enviaCompra"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [HttpPut("enviarCompra/{idUsuario}")]
        public async Task<ActionResult> enviaParaCompras([FromBody] List<EPIPedidosAprovadosDTO> enviaCompra, int idUsuario)
        {
            try
            {
                if (enviaCompra != null)
                {
                    List<PedidosAprovados> pedidosAprovados = new List<PedidosAprovados>();
                    EPIComprasDTO compras = new EPIComprasDTO();
                    decimal valorTotalCompra = 0;

                    foreach (var item in enviaCompra)
                    {
                        pedidosAprovados.Add(new PedidosAprovados
                        {
                            idPedidosAprovados = item.id
                        });

                        item.enviadoCompra = "S";

                        await _pedidosAprovados.Update(item);
                    }

                    compras.pedidosAprovados = pedidosAprovados;
                    compras.dataCadastroCompra = DateTime.Now;

                    foreach (var item in pedidosAprovados)
                    {
                        var localizaPedidoAprovado = await _pedidosAprovados.getProdutoAprovado(item.idPedidosAprovados, "S");
                        var localizaProduto = await _produtos.getProduto(localizaPedidoAprovado.idProduto);
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
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Aprovar pedidos
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> aprovaPedido(int id)
        {
            try
            {
                var localizaPedido = await _pedidos.getPedido(id);
                List<Produtos> produtosStatus = new List<Produtos>();

                if (localizaPedido != null)
                {
                    EPIPedidosAprovadosDTO pedidosAprovados = new EPIPedidosAprovadosDTO();

                    foreach (var produtos in localizaPedido.produtos)
                    {
                        pedidosAprovados.idProduto = produtos.id;
                        pedidosAprovados.idPedido = localizaPedido.id;
                        pedidosAprovados.quantidade = produtos.quantidade;
                        pedidosAprovados.enviadoCompra = "N";

                        var aprovaPedido = await _pedidosAprovados.Insert(pedidosAprovados);

                        if (aprovaPedido != null)
                        {
                            produtosStatus.Add(new Produtos
                            {
                                id = produtos.id,
                                nome = produtos.nome,
                                quantidade = produtos.quantidade,
                                status = 2,
                                idTamanho = produtos.idTamanho
                            });
                        }
                    }

                    localizaPedido.produtos = produtosStatus;
                    localizaPedido.status = 2;

                    await _pedidos.Update(localizaPedido);

                    return Ok(new { message = "Pedidos aprovados com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Pedido não encontrado", result = false });
                }
            }
            catch (System.Exception ex)
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
                        nome = _produtos.getProduto(localizaProduto.idProduto).Result.nomeProduto,
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
        /// Lista todos os produtos aprovados
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet("produtosAprovados")]
        public async Task<ActionResult> produtosAprovados(string status)
        {
            try
            {
                var produtosAprovados = await _pedidosAprovados.getProdutosAprovados(status);
                List<object> produtos = new List<object>();

                foreach (var item in produtosAprovados)
                {
                    produtos.Add(new {
                        nome = _produtos.getProduto(item.id).Result.nomeProduto,
                        quantidade = item.quantidade
                    });
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
