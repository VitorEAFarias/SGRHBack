using Vestimenta.DTO._DbContext;
using Vestimenta.DTO;
using Vestimenta.BLL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Vestimenta.DAL
{
    public class VestimentaDAL : IVestimentaBLL
    {
        public readonly VestAppDbContext _context;

        public VestimentaDAL(VestAppDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int Id)
        {
            var statusDelete = await _context.VestVestimenta.FindAsync(Id);
            _context.VestVestimenta.Remove(statusDelete);

            await _context.SaveChangesAsync();
        }

        public async Task<VestVestimentaDTO> getVestimenta(int Id)
        {
            return await _context.VestVestimenta.FindAsync(Id);
        }

        public async Task<VestVestimentaDTO> getNomeVestimenta(string Nome)
        {
            return await _context.VestVestimenta.FromSqlRaw("SELECT * FROM VestVestimenta WHERE nome = '" + Nome + "'").OrderBy(c => c.id).FirstOrDefaultAsync();
        }

        public async Task<IList<VestVestimentaDTO>> getItens(int idVestimenta)
        {
            return await _context.VestVestimenta.FromSqlRaw("SELECT * from VestVestimenta WHERE id = '" +idVestimenta+ "'").ToListAsync();
        }

        public async Task<IList<VestVestimentaDTO>> getVestimentas()
        {
            return await _context.VestVestimenta.FromSqlRaw("SELECT * FROM VestVestimenta WHERE ativo != 2").ToListAsync();
        }

        public async Task<VestVestimentaDTO> Insert(VestVestimentaDTO vestimenta)
        {
            _context.VestVestimenta.Add(vestimenta);
            await _context.SaveChangesAsync();

            return vestimenta;
        }

        public async Task<VestVestimentaDTO> Update(VestVestimentaDTO vestimenta)
        {
            _context.ChangeTracker.Clear();

            _context.Entry(vestimenta).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return vestimenta;
        }
    }
}
