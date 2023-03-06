using ControleEPI.DTO;
using ControleEPI.DTO._DbContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPIHistorico
{
    public class EPIHistoricoDAL : IEPIHistoricoDAL
    {
        public readonly AppDbContext _context;

        public EPIHistoricoDAL(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IList<EPIVinculoDTO>> dadosPDF(int idUsuario)
        {
            return await _context.EPIVinculo.FromSqlRaw("SELECT * FROM EPIVinculo WHERE idUsuario = '" + idUsuario + "'").ToListAsync();
        }
    }
}
