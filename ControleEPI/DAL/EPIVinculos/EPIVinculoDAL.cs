using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ControleEPI.DAL.EPIVinculos
{
    public class EPIVinculoDAL : IEPIVinculoDAL
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

        public async Task<IList<EPIVinculoDTO>> vinculoUsuarioStatus(int idUsuario, int idStatus)
        {
            return await _context.EPIVinculo.FromSqlRaw("SELECT * FROM EPIVinculo where idUsuario = '" + idUsuario + "' AND status = '" + idStatus + "'").OrderBy(v => v.id).ToListAsync();
        }

        public async Task<EPIVinculoDTO> localizaVinculo(int Id)
        {
            return await _context.EPIVinculo.FindAsync(Id);
        }

        public async Task<IList<EPIVinculoDTO>> localizaVinculos()
        {
            return await _context.EPIVinculo.ToListAsync();
        }

        public async Task<EPIVinculoDTO> Update(EPIVinculoDTO vinculo)
        {
            _context.ChangeTracker.Clear();

            _context.Entry(vinculo).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return vinculo;
        }

        public async Task<EPIVinculoDTO> localizaProdutoVinculo(int idProduto)
        {
            return await _context.EPIVinculo.FromSqlRaw("SELECT * FROM EPIVinculo WHERE idItem = '" + idProduto + "' AND status = 7").OrderBy(p => p.id).FirstOrDefaultAsync();
        }
    }
}
