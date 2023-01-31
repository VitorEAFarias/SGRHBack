using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPIPedidos
{
    public class EPIPedidosDAL : IEPIPedidosDAL
    {
        public readonly AppDbContext _context;
        public EPIPedidosDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EPIPedidosDTO> Insert(EPIPedidosDTO pedidos)
        {
            _context.EPIPedidos.Add(pedidos);
            await _context.SaveChangesAsync();

            return pedidos;
        }

        public async Task<IList<EPIPedidosDTO>> getPedidos()
        {
            return await _context.EPIPedidos.ToListAsync();
        }

        public async Task<IList<EPIPedidosDTO>> getTodosPedidos(int status)
        {
            return await _context.EPIPedidos.FromSqlRaw("SELECT * FROM EPIPedidos WHERE status = '" + status + "'").ToListAsync();
        }

        public async Task<IList<EPIPedidosDTO>> getPedidosUsuario(int id)
        {
            return await _context.EPIPedidos.FromSqlRaw("SELECT * FROM EPIPedidos where idUsuario = '" + id + "'").ToListAsync();
        }

        public async Task<EPIPedidosDTO> getPedido(int Id)
        {
            return await _context.EPIPedidos.FindAsync(Id);
        }

        public async Task<EPIPedidosDTO> Update(EPIPedidosDTO pedido)
        {
            _context.ChangeTracker.Clear();

            _context.Entry(pedido).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return pedido
        }
    }
}