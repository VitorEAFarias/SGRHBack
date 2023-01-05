using Microsoft.EntityFrameworkCore;
using Vestimenta.DTO._DbContext;
using Vestimenta.DTO;
using Vestimenta.BLL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vestimenta.DAL
{
    public class VestItemVinculoDAL : IVestItemVinculoBLL
    {
        public readonly VestAppDbContext _context;

        public VestItemVinculoDAL(VestAppDbContext context)
        {
            _context = context;
        }        

        public async Task Delete(int Id)
        {
            var itemVinculo = await _context.VestItemVinculo.FindAsync(Id);
            _context.VestItemVinculo.Remove(itemVinculo);

            await _context.SaveChangesAsync();
        }

        public async Task<VestItemVinculoDTO> getItemVinculo(int Id)
        {
            return await _context.VestItemVinculo.FindAsync(Id);
        }

        public async Task<IList<VestItemVinculoDTO>> getItensVinculo()
        {
            return await _context.VestItemVinculo.ToListAsync();
        }

        public async Task<IList<VestItemVinculoDTO>> getItensPedido(int idPedido)
        {
            return await _context.VestItemVinculo.FromSqlRaw("SELECT * FROM VestItemVinculo WHERE idPedido = '" + idPedido + "'").ToListAsync();
        }

        public async Task<VestItemVinculoDTO> Insert(VestItemVinculoDTO itemVinculo)
        {
            _context.VestItemVinculo.Add(itemVinculo);
            await _context.SaveChangesAsync();

            return itemVinculo;
        }

        public async Task Update(VestItemVinculoDTO itemVinculo)
        {
            _context.Entry(itemVinculo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
