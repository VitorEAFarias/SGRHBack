using ControleEPI.BLL.EPIPedidos;
using ControleEPI.BLL.EPIPedidosAprovados;
using ControleEPI.BLL.EPIVinculos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.BLL.VestPedidos;
using Vestimenta.BLL.VestRepositorio;
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

        /// <summary>
        /// Construtor DashboardController
        /// </summary>
        /// <param name="pedidosEPI"></param>
        /// <param name="pedidosVest"></param>
        /// <param name="EPIAprovados"></param>
        /// <param name="vestAprovados"></param>
        /// <param name="vinculoEPI"></param>
        /// <param name="vinculoVest"></param>
        public ControllerDashboard(IEPIPedidosBLL pedidosEPI, IVestPedidosBLL pedidosVest, IEPIPedidosAprovadosBLL EPIAprovados, IVestRepositorioBLL vestAprovados, IEPIVinculoBLL vinculoEPI,
            IVestVinculoBLL vinculoVest)
        {
            _pedidosEPI = pedidosEPI;
            _pedidosVest = pedidosVest;
            _EPIAprovados = EPIAprovados;
            _VestAprovados = vestAprovados;
            _vinculoEPI = vinculoEPI;
            _vinculoVest = vinculoVest;
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
                var aprovados = todosAprovadosEPI.Count + todosAprovadosVest.Count;
                var vinculados = todosVinculadosEPI.Count + todosVinculadosVest.Count;

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
    }
}
