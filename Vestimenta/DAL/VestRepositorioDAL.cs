using Vestimenta.DTO._DbContext;
using Vestimenta.DTO;
using Vestimenta.BLL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Vestimenta.DAL
{
    public class VestRepositorioDAL : IVestRepositorioBLL
    {
        public readonly VestAppDbContext _context;

        public VestRepositorioDAL(VestAppDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int Id)
        {
            var repoDelete = await _context.VestRepositorio.FindAsync(Id);
            _context.VestRepositorio.Remove(repoDelete);

            await _context.SaveChangesAsync();
        }

        public async Task<VestRepositorioDTO> getRepositorio(int Id)
        {
            return await _context.VestRepositorio.FindAsync(Id);
        }

        public async Task<IList<VestRepositorioDTO>> getRepositorios()
        {
            return await _context.VestRepositorio.ToListAsync();
        }

        public async Task<VestRepositorioDTO> getRepositorioItensPedidos(int idItem, int idPedido)
        {
            return await _context.VestRepositorio.FromSqlRaw("SELECT * FROM VestRepositorio WHERE idItem = '"+idItem+"' AND idPedido = '"+idPedido+"' " +
                "AND enviadoCompra = 'N' AND ativo = 'Y'").OrderBy(c => c.id).FirstOrDefaultAsync();
        }

        public async Task<IList<VestRepositorioDTO>> getRepositorioStatus(string status)
        {
            return await _context.VestRepositorio.FromSqlRaw("SELECT * FROM VestRepositorio WHERE enviadoCompra = '"+status+"' AND ativo = 'Y'").ToListAsync();
        }

        public async Task<VestRepositorioDTO> Insert(VestRepositorioDTO repo)
        {
            _context.VestRepositorio.Add(repo);
            await _context.SaveChangesAsync();

            return repo;
        }        

        public async Task Update(VestRepositorioDTO repo)
        {
            _context.Entry(repo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
