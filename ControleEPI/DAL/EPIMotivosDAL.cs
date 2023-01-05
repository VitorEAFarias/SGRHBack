using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using ControleEPI.BLL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL
{
    public class EPIMotivosDAL : IEPIMotivosBLL
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
