using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPIMotivos
{
    public class EPIMotivosDAL : IEPIMotivosDAL
    {
        public readonly AppDbContext _context;
        public EPIMotivosDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EPIMotivoDTO>> getMotivos()
        {
            return await _context.EPIMotivos.ToListAsync();
        }

        public async Task<EPIMotivoDTO> getMotivo(int Id)
        {
            return await _context.EPIMotivos.FindAsync(Id);
        }
    }
}
