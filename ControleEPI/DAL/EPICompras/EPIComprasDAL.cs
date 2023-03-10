using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPICompras
{
    public class EPIComprasDAL : IEPIComprasDAL
    {
        public readonly AppDbContext _context;

        public EPIComprasDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EPIComprasDTO> Delete(int Id)
        {
            var compraDelete = await _context.EPICompras.FindAsync(Id);
            _context.EPICompras.Remove(compraDelete);

            await _context.SaveChangesAsync();

            return compraDelete;
        }

        public async Task<IList<EPIComprasDTO>> getTodasCompras()
        {
            return await _context.EPICompras.ToListAsync();
        }

        public async Task<EPIComprasDTO> getCompra(int Id)
        {
            return await _context.EPICompras.FindAsync(Id);
        }

        public async Task<IList<EPIComprasDTO>> getCompras(string status)
        {
            return await _context.EPICompras.FromSqlRaw("SELECT * FROM EPICompras WHERE status = '" + status + "'").ToListAsync();
        }

        public async Task<EPIComprasDTO> Insert(EPIComprasDTO compra)
        {
            _context.EPICompras.Add(compra);
            await _context.SaveChangesAsync();

            return compra;
        }

        public async Task<EPIComprasDTO> Update(EPIComprasDTO compra)
        {
            _context.ChangeTracker.Clear();
            _context.Entry(compra).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return compra;
        }

        public async Task<IList<EPIComprasDTO>> getStatusCompras(int status)
        {
            return await _context.EPICompras.FromSqlRaw("SELECT * FROM compras where status = '" + status + "'").ToListAsync();
        }
    }
}
