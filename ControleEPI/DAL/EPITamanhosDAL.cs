using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.BLL;
using System.Linq;

namespace ControleEPI.DAL
{
    public class EPITamanhosDAL : IEPITamanhosBLL
    {
        private readonly AppDbContext _context;
        public EPITamanhosDAL(AppDbContext context)
        {
            _context = context;
        }
        public async Task<EPITamanhosDTO> insereTamanho(EPITamanhosDTO tamanho)
        {
            _context.EPITamanhos.Add(tamanho);
            await _context.SaveChangesAsync();

            return tamanho;
        }

        public async Task<EPITamanhosDTO> localizaTamanho(int Id)
        {
            return await _context.EPITamanhos.FindAsync(Id);
        }

        public async Task<IList<EPITamanhosDTO>> localizaTamanhos()
        {
            return await _context.EPITamanhos.ToListAsync();
        }

        public async Task Update(EPITamanhosDTO tamanho)
        {
            _context.Entry(tamanho).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<EPITamanhosDTO> verificaTamanho(string nome)
        {
            return await _context.EPITamanhos.FromSqlRaw("SELECT * FROM EPITamanhos WHERE tamanho = '" + nome + "'").OrderBy(x => x.id).FirstOrDefaultAsync();
        }

        public async Task Delete(int Id)
        {
            var tamanhoDelete = await _context.EPITamanhos.FindAsync(Id);
            _context.EPITamanhos.Remove(tamanhoDelete);

            await _context.SaveChangesAsync();
        }
    }
}
