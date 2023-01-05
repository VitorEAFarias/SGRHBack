using Vestimenta.DTO._DbContext;
using Vestimenta.BLL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.DAL
{
    public class DinkPDFDAL : IDinkPDFBLL
    {
        public readonly VestAppDbContext _context;

        public DinkPDFDAL(VestAppDbContext context)
        {
            _context = context;
        }
        public async Task<IList<VestVinculoDTO>> dadosPDF(int idUsuario)
        {
            return await _context.VestVinculo.FromSqlRaw("SELECT * FROM VestVinculo WHERE idUsuario = '"+idUsuario+"' AND status = 6").ToListAsync();
        }
    }
}
