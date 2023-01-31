using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.RHDepartamentos
{
    public class RHDepartamentosDAL : IRHDepartamentosDAL
    {
        public readonly AppDbContextRH _context;
        public RHDepartamentosDAL(AppDbContextRH context)
        {
            _context = context;
        }
        public async Task<RHDepartamentosDTO> getDepartamento(int Id)
        {
            return await _context.rh_departamentos.FindAsync(Id);
        }

        public async Task<IEnumerable<RHDepartamentosDTO>> getDepartamentos()
        {
            return await _context.rh_departamentos.ToListAsync();
        }
    }
}
