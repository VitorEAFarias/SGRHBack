using Vestimenta.DTO._DbContext;
using Vestimenta.DTO;
using Vestimenta.BLL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vestimenta.DAL
{
    public class ComprasVestDAL : IComprasVestBLL
    {
        public readonly VestAppDbContext _context;

        public ComprasVestDAL(VestAppDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int Id)
        {
            var compraDelete = await _context.VestCompra.FindAsync(Id);
            _context.VestCompra.Remove(compraDelete);

            await _context.SaveChangesAsync();
        }

        public async Task<VestComprasDTO> getCompra(int Id)
        {
            return await _context.VestCompra.FindAsync(Id);
        }

        public async Task<IList<VestComprasDTO>> getCompras()
        {
            return await _context.VestCompra.ToListAsync();
        }

        public async Task<VestComprasDTO> Insert(VestComprasDTO compra)
        {
            _context.ChangeTracker.Clear();

            _context.VestCompra.Add(compra);
            await _context.SaveChangesAsync();

            return compra;
        }

        public async Task Update(VestComprasDTO compra)
        {
            _context.ChangeTracker.Clear();

            _context.Entry(compra).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
