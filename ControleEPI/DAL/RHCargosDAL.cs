using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using ControleEPI.BLL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL
{
    public class RHCargosDAL : IRHCargosBLL
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
