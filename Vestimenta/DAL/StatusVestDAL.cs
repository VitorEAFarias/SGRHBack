using Vestimenta.DTO._DbContext;
using Vestimenta.DTO;
using Vestimenta.BLL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Vestimenta.DAL
{
    public class StatusVestDAL : IStatusVestBLL
    {
        public readonly VestAppDbContext _context;

        public StatusVestDAL(VestAppDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int Id)
        {
            var statusDelete = await _context.VestStatus.FindAsync(Id);
            _context.VestStatus.Remove(statusDelete);

            await _context.SaveChangesAsync();
        }

        public async Task<VestStatusDTO> getStatus(int Id)
        {
            return await _context.VestStatus.FindAsync(Id);
        }

        public async Task<VestStatusDTO> getNomeStatus(string nome)
        {
            return await _context.VestStatus.FromSqlRaw("SELECT * FROM VestStatus WHERE nome = '" +nome+ "'").OrderBy(c => c.id).FirstOrDefaultAsync();
        }

        public async Task<IList<VestStatusDTO>> getTodosStatus()
        {
            return await _context.VestStatus.ToListAsync();
        }

        public async Task<VestStatusDTO> Insert(VestStatusDTO status)
        {
            _context.VestStatus.Add(status);
            await _context.SaveChangesAsync();

            return status;
        }

        public async Task Update(VestStatusDTO status)
        {
            _context.Entry(status).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
