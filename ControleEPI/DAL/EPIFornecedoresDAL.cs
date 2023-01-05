using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.BLL;
using System.Linq;

namespace ControleEPI.DAL
{
    public class EPIFornecedoresDAL : IEPIFornecedoresBLL
    {
        public readonly AppDbContext _context;
        public EPIFornecedoresDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EPIFornecedoresDTO> Insert(EPIFornecedoresDTO fornecedor)
        {
            _context.EPIFornecedores.Add(fornecedor);
            await _context.SaveChangesAsync();

            return fornecedor;
        }

        public async Task<IEnumerable<EPIFornecedoresDTO>> getFornecedores()
        {
            return await _context.EPIFornecedores.ToListAsync();
        }

        public async Task<EPIFornecedoresDTO> getFornecedor(int Id)
        {
            return await _context.EPIFornecedores.FindAsync(Id);
        }

        public async Task<EPIFornecedoresDTO> verificaFornecedor(string nome, string cnpj)
        {
            return await _context.EPIFornecedores.FromSqlRaw("SELECT * FROM EPIFornecedores WHERE nome = '"+nome+"' OR cnpj = '"+cnpj+"'").OrderBy(x => x.id).FirstOrDefaultAsync();
        }

        public async Task Update(EPIFornecedoresDTO fornecedor)
        {
            _context.ChangeTracker.Clear();

            _context.Entry(fornecedor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
