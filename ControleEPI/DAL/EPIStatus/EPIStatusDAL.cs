using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPIStatus
{
    public class EPIStatusDAL : IEPIStatusDAL
    {
        public readonly AppDbContext _context;
        public EPIStatusDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EPIStatusDTO> getStatus(int Id)
        {
            return await _context.EPIStatus.FindAsync(Id);
        }
    }
}
