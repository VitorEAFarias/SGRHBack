using ControleEPI.BLL;
using ControleEPI.DTO;
using ControleEPI.DTO._DbContext;
using System.Threading.Tasks;

namespace ControleEPI.DAL
{
    public class EPILogComprasDAL : IEPILogComprasBLL
    {
        private readonly AppDbContext _context;

        public EPILogComprasDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EPILogComprasDTO> insereLogCompra(EPILogComprasDTO logCompras)
        {
            _context.EPILogCompras.Add(logCompras);
            await _context.SaveChangesAsync();

            return logCompras;
        }
    }
}
