using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ControleEPI.DAL.EPIMotivos
{
    public class EPIMotivosDAL : IEPIMotivosDAL
    {
        public readonly AppDbContext _context;
        public EPIMotivosDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EPIMotivoDTO> insereMotivo(EPIMotivoDTO motivo)
        {
            _context.EPIMotivos.Add(motivo);
            await _context.SaveChangesAsync();

            return motivo;
        }

        public async Task<EPIMotivoDTO> verificaNome(string nome)
        {
            return await _context.EPIMotivos.FromSqlRaw("SELECT * FROM EPIMotivos WHERE nome = '" + nome + "'").OrderBy(m => m.id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EPIMotivoDTO>> getMotivos()
        {
            return await _context.EPIMotivos.ToListAsync();
        }

        public async Task<EPIMotivoDTO> getMotivo(int Id)
        {
            return await _context.EPIMotivos.FindAsync(Id);
        }

        public async Task<EPIMotivoDTO> atualizaMotivo(EPIMotivoDTO motivo)
        {
            _context.ChangeTracker.Clear();

            _context.Entry(motivo).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return motivo;
        }
    }
}
