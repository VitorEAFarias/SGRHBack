using Vestimenta.DTO._DbContext;
using Vestimenta.DTO;
using Vestimenta.BLL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Vestimenta.DAL
{
    public class EstoqueDAL : IEstoqueBLL
    {
        public readonly VestAppDbContext _context;

        public EstoqueDAL(VestAppDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int Id)
        {
            var estoqueDelete = await _context.VestEstoque.FindAsync(Id);
            _context.VestEstoque.Remove(estoqueDelete);

            await _context.SaveChangesAsync();
        }

        public async Task<VestEstoqueDTO> getItemEstoque(int Id)
        {
            return await _context.VestEstoque.FindAsync(Id);
        }

        public async Task<VestEstoqueDTO> getItemExistente(int idItem, string tamanho)
        {
            return await _context.VestEstoque.FromSqlRaw("SELECT * FROM VestEstoque WHERE idItem = '" + idItem + "' AND tamanho = '" +tamanho+ "' AND ativado = 'Y'").OrderBy(c => c.id).FirstOrDefaultAsync();
        }

        public async Task<VestEstoqueDTO> getDesativados(int idItem, string tamanho)
        {
            return await _context.VestEstoque.FromSqlRaw("SELECT * FROM VestEstoque WHERE idItem = '" + idItem + "' AND tamanho = '" + tamanho + "' AND ativado = 'N'").OrderBy(c => c.id).FirstOrDefaultAsync();
        }

        public async Task<IList<VestEstoqueDTO>> getItensExistentes(int idItens)
        {
            return await _context.VestEstoque.FromSqlRaw("SELECT * FROM VestEstoque WHERE idItem = '" + idItens + "' AND ativado = 'Y'").ToListAsync();
        }

        public async Task<IList<VestEstoqueDTO>> getEstoque()
        {
            return await _context.VestEstoque.ToListAsync();
        }

        public async Task<VestEstoqueDTO> Insert(VestEstoqueDTO estoque)
        {
            _context.VestEstoque.Add(estoque);
            await _context.SaveChangesAsync();

            return estoque;
        }

        public async Task Update(VestEstoqueDTO estoque)
        {
            _context.ChangeTracker.Clear();

            _context.Entry(estoque).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
