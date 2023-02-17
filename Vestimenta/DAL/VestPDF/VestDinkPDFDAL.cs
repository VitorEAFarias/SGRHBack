using Vestimenta.DTO._DbContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.DAL.VestPDF
{
    public class VestDinkPDFDAL : IVestDinkPDFDAL
    {
        public readonly AppDbContext _context;

        public VestDinkPDFDAL(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IList<VestVinculoDTO>> dadosPDF(int idUsuario)
        {
            return await _context.VestVinculo.FromSqlRaw("SELECT * FROM VestVinculo WHERE idUsuario = '" + idUsuario + "' AND status = 6").ToListAsync();
        }
    }
}
