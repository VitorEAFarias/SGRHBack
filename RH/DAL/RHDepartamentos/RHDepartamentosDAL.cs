using RH.DTO._DbContext;
using RH.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RH.DAL.RHDepartamentos
{
    public class RHDepartamentosDAL : IRHDepartamentosDAL
    {
        public readonly AppDbContext _context;
        public RHDepartamentosDAL(AppDbContext context)
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
