using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.BLL;
using System.Linq;

namespace ControleEPI.DAL
{
    public class EPICategoriasDAL : IEPICategoriasBLL
    {
        public readonly AppDbContext _context;
        public EPICategoriasDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EPICategoriasDTO> Insert(EPICategoriasDTO categoria)
        {
            _context.EPICategoria.Add(categoria);
            await _context.SaveChangesAsync();

            return categoria;
        }

        public async Task<IList<EPICategoriasDTO>> getCategorias()
        {
            return await _context.EPICategoria.ToListAsync();
        }

        public async Task<EPICategoriasDTO> getCategoria(int Id)
        {
            return await _context.EPICategoria.FindAsync(Id);
        }

        public async Task<EPICategoriasDTO> verificaCategoria(string nome)
        {
            return await _context.EPICategoria.FromSqlRaw("SELECT * FROM EPICategoria WHERE nome = '" + nome + "'").OrderBy(x => x.id).FirstOrDefaultAsync();
        }

        public async Task Update(EPICategoriasDTO categoria)
        {
            _context.ChangeTracker.Clear();

            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
