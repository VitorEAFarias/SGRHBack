using Vestimenta.DTO._DbContext;
using Vestimenta.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Vestimenta.DAL.VestStatus
{
    public class VestStatusDAL : IVestStatusDAL
    {
        public readonly AppDbContext _context;

        public VestStatusDAL(AppDbContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<VestStatusDTO> Delete(int Id)
        {
            var statusDelete = await _context.VestStatus.FindAsync(Id);
            _context.VestStatus.Remove(statusDelete);

            await _context.SaveChangesAsync();

            return statusDelete;
        }

        public async Task<VestStatusDTO> getStatus(int Id)
        {
            return await _context.VestStatus.FindAsync(Id);
        }

        public async Task<VestStatusDTO> getNomeStatus(string nome)
        {
            return await _context.VestStatus.FromSqlRaw("SELECT * FROM VestStatus WHERE nome = '" + nome + "'").OrderBy(c => c.id).FirstOrDefaultAsync();
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

        public async Task<VestStatusDTO> Update(VestStatusDTO status)
        {
            _context.Entry(status).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return status;
        }
    }
}
