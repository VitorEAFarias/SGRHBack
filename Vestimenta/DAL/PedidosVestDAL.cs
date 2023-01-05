using Vestimenta.DTO._DbContext;
using Vestimenta.DTO;
using Vestimenta.BLL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vestimenta.DAL
{
    public class PedidosVestDAL : IPedidosVestBLL
    {
        public readonly VestAppDbContext _context;

        public PedidosVestDAL(VestAppDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int Id)
        {
            var pedidoDelete = await _context.VestPedidos.FindAsync(Id);
            _context.VestPedidos.Remove(pedidoDelete);

            await _context.SaveChangesAsync();
        }

        public async Task<IList<VestPedidosDTO>> getPedidosStatus(int idStatus)
        {
            return await _context.VestPedidos.FromSqlRaw("SELECT * FROM VestPedidos WHERE status = '" +idStatus+ "'").ToListAsync();
        }

        public async Task<IList<VestPedidosDTO>> getPedidosUsuarios(int idUsuario)
        {
            return await _context.VestPedidos.FromSqlRaw("SELECT * FROM VestPedidos WHERE idUsuario = '"+idUsuario+"'").ToListAsync();
        }

        public async Task<IList<VestPedidosDTO>> getLiberadoVinculo()
        {
            return await _context.VestPedidos.FromSqlRaw("SELECT * FROM VestPedidos WHERE status != 2 AND status != 3").ToListAsync();
        }

        public async Task<IList<VestPedidosDTO>> getPedidosPendentes()
        {
            return await _context.VestPedidos.FromSqlRaw("SELECT * FROM VestPedidos WHERE status = 1").ToListAsync();
        }

        public async Task<VestPedidosDTO> getPedido(int Id)
        {
            return await _context.VestPedidos.FindAsync(Id);
        }

        public async Task<IList<VestPedidosDTO>> getPedidos()
        {
            return await _context.VestPedidos.ToListAsync();
        }

        public async Task<VestPedidosDTO> Insert(VestPedidosDTO pedido)
        {
            _context.VestPedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return pedido;
        }

        public async Task Update(VestPedidosDTO pedido)
        {
            _context.ChangeTracker.Clear();

            _context.Entry(pedido).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
