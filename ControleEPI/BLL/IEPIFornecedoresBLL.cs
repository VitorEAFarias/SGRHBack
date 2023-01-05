using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL
{
    public interface IEPIFornecedoresBLL
    {
        Task<EPIFornecedoresDTO> Insert(EPIFornecedoresDTO fornecedor);
        Task<EPIFornecedoresDTO> getFornecedor(int Id);
        Task<EPIFornecedoresDTO> verificaFornecedor(string nome, string cnpj);
        Task<IEnumerable<EPIFornecedoresDTO>> getFornecedores();
        Task Update(EPIFornecedoresDTO fornecedor);
    }
}
