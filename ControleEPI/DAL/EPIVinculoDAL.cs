using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.BLL;

namespace ControleEPI.DAL
{
    public class EPIVinculoDAL : IEPIVinculoBLL
    {
        private readonly AppDbContext _context;
        public EPIVinculoDAL(AppDbContext context)
        {
            _context = context;
        }
        public async Task<EPIVinculoDTO> insereVinculo(EPIVinculoDTO vinculo)
        {
            _context.EPIVinculo.Add(vinculo);
            await _context.SaveChangesAsync();

            return vinculo;
        }

        public async Task<IList<EPIVinculoDTO>> localizaVinculoStatus(int status)
        {
            return await _context.EPIVinculo.FromSqlRaw("SELECT * FROM EPIVinculo WHERE status = '" + status + "'").ToListAsync();
        }

        public async Task<IList<EPIVinculoDTO>> localizaVinculoUsuario(int idUsuario)
        {
            return await _context.EPIVinculo.FromSqlRaw("SELECT * FROM EPIVinculo WHERE idUsuario = '" + idUsuario + "'").ToListAsync();
        }

        public async Task<EPIVinculoDTO> localizaVinculo(int Id)
        {
            return await _context.EPIVinculo.FindAsync(Id);
        }

        public async Task<IList<EPIVinculoDTO>> localizaVinculos()
        {
            return await _context.EPIVinculo.ToListAsync();
        }

        public async Task Update(EPIVinculoDTO vinculo)
        {
            _context.ChangeTracker.Clear();

            _context.Entry(vinculo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
