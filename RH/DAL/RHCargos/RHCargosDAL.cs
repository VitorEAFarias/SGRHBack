using RH.DTO._DbContext;
using RH.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RH.DAL.RHCargos
{
    public class RHCargosDAL : IRHCargosDAL
    {
        public readonly AppDbContext _context;
        public RHCargosDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RHCargosDTO> getCargo(int Id)
        {
            return await _context.rh_cargos.FindAsync(Id);
        }

        public async Task<IEnumerable<RHCargosDTO>> getCargos()
        {
            return await _context.rh_cargos.ToListAsync();
        }
    }
}
