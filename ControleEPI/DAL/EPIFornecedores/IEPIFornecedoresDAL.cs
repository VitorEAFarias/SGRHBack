using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPIFornecedores
{
    public interface IEPIFornecedoresDAL
    {
        Task<EPIFornecedoresDTO> Insert(EPIFornecedoresDTO fornecedor);
        Task<EPIFornecedoresDTO> getFornecedor(int Id);
        Task<EPIFornecedoresDTO> verificaFornecedor(string nome, string cnpj);
        Task<IEnumerable<EPIFornecedoresDTO>> getFornecedores();
        Task<EPIFornecedoresDTO> Update(EPIFornecedoresDTO fornecedor);
    }
}
