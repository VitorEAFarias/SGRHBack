using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ControleEPI.BLL;
using ControleEPI.DTO;
using System;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe de manipulação de pedidos de epi's
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerPedidos : ControllerBase
    {
        private readonly IEPIPedidosBLL _pedidos;
        private readonly IEPIStatusBLL _status;
        private readonly IEPIMotivosBLL _motivos;
        private readonly IRHConUserBLL _conUser;
        private readonly IEPIProdutosEstoqueBLL _produtosEstoque;
        private readonly IEPIPedidosAprovadosBLL _pedidosAprovados;
        private readonly IEPIProdutosBLL _produtos;

        /// <summary>
        /// Construtor PedidosController
        /// </summary>
        /// <param name="pedidos"></param>
        /// <param name="status"></param>
        /// <param name="motivos"></param>
        /// <param name="conUser"></param>
        /// <param name="produtosEstoque"></param>
        /// <param name="pedidosAprovados"></param>
        /// <param name="produtos"></param>
        public ControllerPedidos(IEPIPedidosBLL pedidos, IEPIStatusBLL status, IEPIMotivosBLL motivos, IRHConUserBLL conUser, IEPIProdutosEstoqueBLL produtosEstoque,
            IEPIProdutosBLL produtos, IEPIPedidosAprovadosBLL pedidosAprovados)
        {
            _pedidos = pedidos;
            _status = status;
            _motivos = motivos;
            _conUser = conUser;            
            _produtosEstoque = produtosEstoque;
            _produtos = produtos;
            _pedidosAprovados = pedidosAprovados;
        }

        /// <summary>
        /// insere um pedido a ser feito
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> inserePedido([FromBody] EPIPedidosDTO pedido)
        {
            try
            {
                if (pedido.produtos != null)
                {
                    pedido.dataPedido = DateTime.Now;
                    pedido.status = 1;

                    var novoPedido = await _pedidos.Insert(pedido);

                    return Ok(new { message = "Pedido realizado com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum produto selecionado para o pedido", result = false });   
                }                  
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }        
        }

        /// <summary>
        /// Aprovar pedido
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [HttpPut("aprovaPedido")]
        public async Task<IActionResult> aprovaPedido([FromBody] EPIPedidosDTO pedido)
        {
            try
            {
                var localizaPedido = await _pedidos.getPedido(pedido.id);

                if (localizaPedido != null)
                {
                    EPIPedidosAprovadosDTO aprovado = new EPIPedidosAprovadosDTO();

                    foreach (var item in localizaPedido.produtos)
                    {
                        if (item.status == 3)
                        {
                            return BadRequest(new { message = "Impossivel aprovar produtos do pedido pois, há produtos reprovados", result = false, 
                                data = item.id + item.idTamanho });
                        }
                        else
                        {
                            aprovado.idProduto = item.id;
                            aprovado.idPedido = localizaPedido.id;
                            aprovado.idTamanho = item.idTamanho;
                            aprovado.quantidade = item.quantidade;
                            aprovado.enviadoCompra = "N";

                            await _pedidosAprovados.Update(aprovado);
                        }                        
                    }

                    return Ok(new { message = "Pedido aprovado com sucesso!!!", result = true });
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

        /// <summary>
        /// Aprova um produto do pedido
        /// </summary>
        /// <param name="idProduto"></param>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [HttpPut("aprovaProdutoPedido/{idProduto}")]
        public async Task<IActionResult> aprovaProdutoPedido(int idProduto, [FromBody] EPIPedidosDTO pedido)
        {
            try
            {
                var localizaPedido = await _pedidos.getPedido(pedido.id);

                if (localizaPedido != null)
                {
                    List<Produtos> produtos = new List<Produtos>();

                    foreach (var item in localizaPedido.produtos)
                    {
                        if (item.id == idProduto)
                        {
                            produtos.Add(new Produtos
                            {
                                id = item.id,
                                nome = item.nome,
                                quantidade = item.quantidade,
                                status = 2,
                                idTamanho = item.idTamanho
                            });
                        }
                        else
                        {
                            produtos.Add(new Produtos
                            {
                                id = item.id,
                                nome = item.nome,
                                quantidade = item.quantidade,
                                status = item.status,
                                idTamanho = item.idTamanho
                            });
                        }
                    }

                    EPIPedidosAprovadosDTO aprovados = new EPIPedidosAprovadosDTO();

                    foreach (var produtosAprovados in produtos)
                    {
                        if (produtosAprovados.status == 2)
                        {
                            aprovados.idProduto = produtosAprovados.id;
                            aprovados.idPedido = localizaPedido.id;
                            aprovados.idTamanho = produtosAprovados.idTamanho;
                            aprovados.quantidade = produtosAprovados.quantidade;
                            aprovados.enviadoCompra = "N";

                            await _pedidosAprovados.Update(aprovados);
                        }
                    }

                    int contador = 0;

                    foreach (var item in produtos)
                    {
                        if (item.status == 3 || item.status == 13)
                        {
                            contador++;
                        }
                    }

                    localizaPedido.produtos = produtos;

                    if (contador == produtos.Count)
                    {
                        localizaPedido.status = 10;
                    }
                    else
                    {
                        localizaPedido.status = pedido.status;
                    }

                    await _pedidos.Update(localizaPedido);

                    return Ok(new { message = "Produto aprovado com sucesso", result = true });
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

        /// <summary>
        /// Reprovar produto do pedido
        /// </summary>
        /// <param name="idProduto"></param>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [HttpPut("reprovarProdutoPedido/{idProduto}")]
        public async Task<IActionResult> reprovaProdutoPedido(int idProduto, [FromBody] EPIPedidosDTO pedido)
        {
            try
            {
                var localizaPedido = await _pedidos.getPedido(pedido.id);

                if (localizaPedido != null)
                {
                    List<Produtos> produtos = new List<Produtos>();

                    foreach (var item in localizaPedido.produtos)
                    {
                        if (item.id == idProduto)
                        {
                            produtos.Add(new Produtos { 
                                id = item.id,
                                nome = item.nome,
                                quantidade = item.quantidade,
                                status = 3,
                                idTamanho = item.idTamanho
                            });
                        }
                        else
                        {
                            produtos.Add(new Produtos
                            {
                                id = item.id,
                                nome = item.nome,
                                quantidade = item.quantidade,
                                status = item.status,
                                idTamanho = item.idTamanho
                            });
                        }
                    }

                    int contador = 0;

                    foreach (var item in produtos)
                    {
                        if (item.status == 3 || item.status == 13)
                        {
                            contador++;
                        }
                    }

                    localizaPedido.produtos = produtos;

                    if (contador == produtos.Count)
                    {
                        localizaPedido.status = 10;
                    }
                    else
                    {
                        localizaPedido.status = pedido.status;
                    }

                    await _pedidos.Update(localizaPedido);

                    return Ok(new { message = "Produto reprovado com sucesso", result = true });
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

        /// <summary>
        /// Reprova pedido de EPI
        /// </summary>
        /// <param name="status"></param>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [HttpPut("reprovarPedido/{status}")]
        public async Task<IActionResult> reprovaPedido(int status, [FromBody] EPIPedidosDTO pedido)
        {
            try
            {
                var localizaPedido = await _pedidos.getPedido(pedido.id);

                if (localizaPedido != null)
                {
                    var idStatus = await _status.getStatus(status);

                    List<Produtos> produtos = new List<Produtos>();

                    localizaPedido.status = idStatus.id;

                    foreach (var item in localizaPedido.produtos)
                    {
                        produtos.Add(new Produtos { 
                            id = item.id,
                            nome = item.nome,
                            quantidade = item.quantidade,
                            status = 3,
                            idTamanho = item.idTamanho
                        });
                    }

                    localizaPedido.produtos = produtos;

                    await _pedidos.Update(localizaPedido);

                    return Ok(new { message = "Pedido reprovado com sucesso!!!", result = true });
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

        /// <summary>
        /// Seleciona um pedido
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EPIPedidosDTO>> GetPedido(int id)
        {
            try
            {
                var pedido = await _pedidos.getPedido(id);

                if (pedido != null)
                {
                    var motivo = await _motivos.getMotivo(pedido.motivo);
                    var usuario = await _conUser.GetEmp(pedido.idUsuario);
                    var status = await _status.getStatus(pedido.status);

                    var lista = new List<object>();

                    foreach (var value in pedido.produtos)
                    {
                        var query = await _produtosEstoque.getProdutoEstoque(value.id);

                        lista.Add(new
                        {
                            value.id,
                            value.quantidade,
                            value.nome,
                            estoque = query.quantidade
                        });
                    }

                    var item = new
                    {
                        pedido.id,
                        pedido.dataPedido,
                        pedido.descricao,
                        produtos = lista,
                        motivo = motivo.nome,
                        usuario = usuario.nome,
                        status = status.nome
                    };

                    return Ok(new { message = "Pedido encontrado", pedido = item, result = true });
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
        /// Seleciona os produto ao qual fazem parte de uma categoria especifica
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<EPIPedidosDTO>> getPedidosUsuario([FromRoute] int idUsuario)
        {
            try
            {
                List<object> listaPedidos = new List<object>();

                var pedidos = await _pedidos.getPedidosUsuario(idUsuario);

                foreach(var item in pedidos)
                {
                    var motivo = await _motivos.getMotivo(item.motivo);
                    var usuario = await _conUser.GetEmp(item.idUsuario);

                    listaPedidos.Add(new
                    {
                        item.id,
                        item.dataPedido,
                        item.descricao,
                        item.produtos,
                        motivo = motivo.nome,
                        usuario = usuario.nome
                    });
                }

                return Ok(new { message = "Lista de pedidos encontrado", lista = listaPedidos, result = true });

            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista todos os pedidos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<EPIPedidosDTO>> getTodosPedidos()
        {
            try
            {
                var localizaPedidos = await _pedidos.getPedidos();

                if (localizaPedidos != null)
                {
                    return Ok(new { message = "Pedidos encontrados!!!", result = true, lista = localizaPedidos });
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
        /// Lista todos os pedidos
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet("{status}")]
        public async Task<ActionResult<EPIPedidosDTO>> getPedidos(int status)
        {
            try
            {
                var pedidos = await _pedidos.getTodosPedidos(status);
                var localizaNomeStatus = await _status.getStatus(status);

                if (pedidos != null)
                {
                    return Ok(new { message = "lista encontrada", result = true, lista = pedidos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum pedido encontrado com statuso '" + localizaNomeStatus.nome + "'", result = false});
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
