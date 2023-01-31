using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.RHCargos
{
    public class RHCargosDAL : IRHCargosDAL
    {
        public readonly AppDbContextRH _context;
        public RHCargosDAL(AppDbContextRH context)
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
