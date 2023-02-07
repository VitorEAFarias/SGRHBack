using Vestimenta.DTO._DbContext;
using Vestimenta.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vestimenta.DAL.VestLog
{
    public class VestLogDAL : IVestLogDAL
    {
        public readonly AppDbContext _context;

        public VestLogDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int Id)
        {
            var logDelete = await _context.VestLog.FindAsync(Id);
            _context.VestLog.Remove(logDelete);

            await _context.SaveChangesAsync();
        }

        public async Task<VestLogDTO> getLog(int Id)
        {
            return await _context.VestLog.FindAsync(Id);
        }

        public async Task<IList<VestLogDTO>> getLogs()
        {
            return await _context.VestLog.ToListAsync();
        }

        public async Task<VestLogDTO> Insert(VestLogDTO log)
        {
            _context.ChangeTracker.Clear();

            _context.VestLog.Add(log);
            await _context.SaveChangesAsync();

            return log;
        }

        public async Task Update(VestLogDTO log)
        {
            _context.Entry(log).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
