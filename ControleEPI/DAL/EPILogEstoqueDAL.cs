using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using ControleEPI.BLL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL
{
    public class EPILogEstoqueDAL : IEPILogEstoqueBLL
    {
        public readonly AppDbContext _context;
        public EPILogEstoqueDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EPILogEstoqueDTO> Insert(EPILogEstoqueDTO logEstoque)
        {
            _context.EPILogEstoque.Add(logEstoque);
            await _context.SaveChangesAsync();

            return logEstoque;
        }

        public async Task<IEnumerable<EPILogEstoqueDTO>> GetLogsEstoque()
        {
            return await _context.EPILogEstoque.ToListAsync();
        }

        public async Task<EPILogEstoqueDTO> GetLogEstoque(int Id)
        {
            return await _context.EPILogEstoque.FindAsync(Id);
        }
    }
}
