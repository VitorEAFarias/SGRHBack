using ControleEPI.BLL.Pedidos;
using ControleEPI.BLL.PedidosAprovados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.BLL;

namespace ApiSMT.Controllers
{
    /// <summary>
    /// Classe que manipula as informações de produtos e seu estoque
    /// </summary>
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IEPIPedidosBLL _pedidosEPI;
        private readonly IPedidosVestBLL _pedidosVest;
        private readonly IEPIPedidosAprovadosBLL _EPIAprovados;
        private readonly IVestRepositorioBLL _VestAprovados;

        /// <summary>
        /// Construtor DashboardController
        /// </summary>
        /// <param name="pedidosEPI"></param>
        /// <param name="pedidosVest"></param>
        /// <param name="EPIAprovados"></param>
        /// <param name="vestAprovados"></param>
        public DashboardController(IEPIPedidosBLL pedidosEPI, IPedidosVestBLL pedidosVest, IEPIPedidosAprovadosBLL EPIAprovados, IVestRepositorioBLL vestAprovados)
        {
            _pedidosEPI = pedidosEPI;
            _pedidosVest = pedidosVest;
            _EPIAprovados = EPIAprovados;
            _VestAprovados = vestAprovados;
        }

        /// <summary>
        /// Pega todos os pedidos
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> contagemPedidos(int idUsuario)
        {
            try
            {
                var todosPedidosEPI = await _pedidosEPI.getPedidosUsuario(idUsuario);
                var todosPedidosVest = await _pedidosVest.getPedidosUsuarios(idUsuario);
                var todosAprovadosEPI = await _EPIAprovados.getProdutosAprovados("S");
                var todosAprovadosVest = await _VestAprovados.getRepositorioStatus("S");

                List<object> pendentes = new List<object>();
                List<object> realizados = new List<object>();
                List<object> finalizados = new List<object>();
                var aprovados = todosAprovadosEPI.Count + todosAprovadosVest.Count;

                foreach (var pedidosEPI in todosPedidosEPI)
                {
                    if (pedidosEPI.status == 1)
                    {
                        pendentes.Add(pedidosEPI.id);
                    }
                    else if (pedidosEPI.status == 2)
                    {
                        finalizados.Add(pedidosEPI.id);
                    }

                    realizados.Add(pedidosEPI.id);
                }

                foreach (var pedidosVest in todosPedidosVest)
                {
                    if (pedidosVest.status == 1)
                    {
                        pendentes.Add(pedidosVest.id);
                    }
                    else if (pedidosVest.status == 2)
                    {
                        finalizados.Add(pedidosVest.id);
                    }

                    realizados.Add(pedidosVest.id);
                }

                return Ok(new { message = "Pedidos totais", result = true, pendentes = pendentes.Count, finalizados = finalizados.Count, 
                    realizados = realizados.Count, aprovados = aprovados });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
