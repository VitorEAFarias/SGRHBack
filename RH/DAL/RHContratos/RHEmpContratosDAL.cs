using RH.DTO._DbContext;
using RH.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RH.DAL.RHContratos
{
    public class RHEmpContratosDAL : IRHEmpContratosDAL
    {
        public readonly AppDbContext _context;
        public RHEmpContratosDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RHEmpContratosDTO> getContrato(int Id)
        {
            return await _context.rh_empregados_contratos.FindAsync(Id);
        }

        public async Task<IEnumerable<RHEmpContratosDTO>> getContratos()
        {
            return await _context.rh_empregados_contratos.ToListAsync();
        }

        public async Task<RHEmpContratosDTO> getEmpContrato(int IdEmpregado)
        {
            return await _context.rh_empregados_contratos.FromSqlRaw("SELECT * FROM rh_empregados_contratos WHERE" +
                " id_empregado = '" + IdEmpregado + "' AND contrato_atual = '1' AND contrato_principal = '1'").OrderBy(x => x.id).FirstOrDefaultAsync();
        }
    }
}
