using Vestimenta.DTO._DbContext;
using Vestimenta.DTO;
using Vestimenta.BLL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Vestimenta.DAL
{
    public class VestVinculoDAL : IVestVinculoBLL
    {
        public readonly VestAppDbContext _context;

        public VestVinculoDAL(VestAppDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int Id)
        {
            var statusDelete = await _context.VestVinculo.FindAsync(Id);
            _context.VestVinculo.Remove(statusDelete);

            await _context.SaveChangesAsync();
        }

        public async Task<VestVinculoDTO> getVinculo(int Id)
        {
            return await _context.VestVinculo.FindAsync(Id);
        }

        public async Task<IList<VestVinculoDTO>> getVinculoPendente(int idStatus, int idUsuario)
        {
            return await _context.VestVinculo.FromSqlRaw("SELECT * FROM VestVinculo WHERE status = '"+idStatus+"' AND idUsuario = '"+idUsuario+"'").ToListAsync();
        }

        public async Task<VestVinculoDTO> getVinculoTamanho(int idPedido, string tamanho)
        {
            return await _context.VestVinculo.FromSqlRaw("SELECT * FROM VestVinculo WHERE status = 4 AND idPedido = '" + idPedido + "' AND tamanhoVestVinculo = '"+tamanho+"'").OrderBy(c => c.id).FirstOrDefaultAsync();
        }

        public async Task<VestVinculoDTO> getUsuarioVinculo(int id)
        {
            return await _context.VestVinculo.FromSqlRaw("SELECT * FROM VestVinculo WHERE idUsuario = '"+id+"'").OrderBy(c => c.id).FirstOrDefaultAsync();
        }

        public async Task<IList<VestVinculoDTO>> getItensUsuarios(int idUsuario)
        {
            return await _context.VestVinculo.FromSqlRaw("SELECT * FROM VestVinculo WHERE idUsuario = '" + idUsuario + "'").ToListAsync();
        }        

        public async Task<IList<VestVinculoDTO>> getItensVinculados(int idUsuario)
        {
            return await _context.VestVinculo.FromSqlRaw("SELECT * FROM VestVinculo WHERE idUsuario = '"+idUsuario+"' AND status = 6").ToListAsync();
        }

        public async Task<IList<VestVinculoDTO>> getVinculos()
        {
            return await _context.VestVinculo.ToListAsync();
        }

        public async Task<VestVinculoDTO> Insert(VestVinculoDTO vinculo)
        {
            _context.VestVinculo.Add(vinculo);
            await _context.SaveChangesAsync();

            return vinculo;
        }

        public async Task Update(VestVinculoDTO vinculo)
        {
            _context.Entry(vinculo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
