using ControleEPI.BLL.EPIPedidos;
using ControleEPI.BLL.EPIPedidosAprovados;
using ControleEPI.BLL.EPIProdutos;
using ControleEPI.BLL.EPITamanhos;
using ControleEPI.BLL.EPIVinculos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Vestimenta.BLL.VestPedidos;
using Vestimenta.BLL.VestRepositorio;
using Vestimenta.BLL.VestVestimenta;
using Vestimenta.BLL.VestVinculo;

namespace ApiSMT.Controllers
{
    /// <summary>
    /// Classe que manipula as informações de produtos e seu estoque
    /// </summary>
    [Route("api/[controller]")]
    public class ControllerDashboard : ControllerBase
    {
        private readonly IEPIPedidosBLL _pedidosEPI;
        private readonly IVestPedidosBLL _pedidosVest;
        private readonly IEPIPedidosAprovadosBLL _EPIAprovados;
        private readonly IVestRepositorioBLL _VestAprovados;
        private readonly IEPIVinculoBLL _vinculoEPI;
        private readonly IVestVinculoBLL _vinculoVest;
        private readonly IVestVestimentaBLL _vestimenta;
        private readonly IEPIProdutosBLL _produto;
        private readonly IEPITamanhosBLL _tamanho;

        /// <summary>
        /// Construtor DashboardController
        /// </summary>
        /// <param name="pedidosEPI"></param>
        /// <param name="pedidosVest"></param>
        /// <param name="EPIAprovados"></param>
        /// <param name="vestAprovados"></param>
        /// <param name="vinculoEPI"></param>
        /// <param name="vinculoVest"></param>
        /// <param name="vestimenta"></param>
        /// <param name="produto"></param>
        /// <param name="tamanho"></param>
        public ControllerDashboard(IEPIPedidosBLL pedidosEPI, IVestPedidosBLL pedidosVest, IEPIPedidosAprovadosBLL EPIAprovados, IVestRepositorioBLL vestAprovados, IEPIVinculoBLL vinculoEPI,
            IVestVinculoBLL vinculoVest, IVestVestimentaBLL vestimenta, IEPIProdutosBLL produto, IEPITamanhosBLL tamanho)
        {
            _pedidosEPI = pedidosEPI;
            _pedidosVest = pedidosVest;
            _EPIAprovados = EPIAprovados;
            _VestAprovados = vestAprovados;
            _vinculoEPI = vinculoEPI;
            _vinculoVest = vinculoVest;
            _vestimenta = vestimenta;
            _produto = produto;
            _tamanho = tamanho;
        }

        /// <summary>
        /// Pega todos os pedidos
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{idUsuario}")]
        public async Task<IActionResult> contagemPedidos(int idUsuario)
        {
            try
            {
                var todosPedidosEPI = await _pedidosEPI.getPedidosUsuario(idUsuario);
                var todosPedidosVest = await _pedidosVest.getPedidosUsuarios(idUsuario);
                var todosAprovadosEPI = await _EPIAprovados.getProdutosAprovados("S", "S");
                var todosAprovadosVest = await _VestAprovados.getRepositorioStatus("S");
                var todosVinculadosEPI = await _vinculoEPI.localizaVinculoUsuario(idUsuario);
                var todosVinculadosVest = await _vinculoVest.getItensUsuarios(idUsuario);

                List<object> pendentes = new List<object>();
                List<object> realizados = new List<object>();
                int aprovados = 0;
                var vinculados = todosVinculadosEPI.Count + todosVinculadosVest.Count;

                foreach (var aprovadosEPI in todosAprovadosEPI)
                {
                    var localizaPedido = await _pedidosEPI.getPedidoProduto(aprovadosEPI.idPedido);

                    if (localizaPedido != null)
                    {
                        if (localizaPedido.idUsuario == idUsuario)
                        {
                            aprovados++;
                        }
                    }                    
                }

                foreach (var aprovadosVest in todosAprovadosVest)
                {
                    var localizaPedido = await _pedidosVest.getPedido(aprovadosVest.idPedido);

                    if (localizaPedido != null)
                    {
                        if (localizaPedido.idUsuario == idUsuario)
                        {
                            aprovados++;
                        }
                    }                    
                }

                foreach (var pedidosEPI in todosPedidosEPI)
                {
                    if (pedidosEPI.status.Equals(1))
                    {
                        pendentes.Add(pedidosEPI.idPedido);
                    }

                    realizados.Add(pedidosEPI.idPedido);
                }

                foreach (var pedidosVest in todosPedidosVest)
                {
                    if (pedidosVest.status.Equals(1))
                    {
                        pendentes.Add(pedidosVest.id);
                    }

                    realizados.Add(pedidosVest.id);
                }

                return Ok(new
                {
                    message = "Pedidos totais",
                    result = true,
                    pendentes = pendentes.Count,
                    vinculados = vinculados,
                    realizados = realizados.Count,
                    aprovados = aprovados
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// teste dashboard
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("tabelas/{idUsuario}")]
        public async Task<object> preencherTabelasDashboard(int idUsuario)
        {
            try
            {
                var localizaVestVinculados = await _vinculoVest.getItensVinculados(idUsuario);
                var localizaEPIVinculados = await _vinculoEPI.vinculoUsuarioStatus(idUsuario, 13);

                Random random = new Random();

                List<object> listVest = new List<object>();
                List<object> listEPI = new List<object>();

                foreach (var item in localizaEPIVinculados)
                {
                    var localizaProduto = await _produto.localizaProduto(item.idItem);
                    var localizaTamanho = await _tamanho.localizaTamanho(item.idTamanho);

                    listEPI.Add(new
                    {
                        localizaProduto.produto,
                        item.dataVinculo,
                        localizaTamanho.tamanho
                    });
                }

                foreach (var item in localizaVestVinculados)
                {
                    var localizaItem = await _vestimenta.getVestimenta(item.idItem);

                    listVest.Add(new
                    {
                        localizaItem.nome,
                        item.dataVinculo,
                        item.tamanho
                    });
                }

                var agregado = listVest.Concat(listEPI);
                var embaralhar = agregado.OrderBy(_ => random.Next()).ToList();

                return Ok(new { message = "Lista encontrada", result = true, data = embaralhar });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
