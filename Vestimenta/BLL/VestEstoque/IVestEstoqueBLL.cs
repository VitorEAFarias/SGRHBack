using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestEstoque
{
    public interface IVestEstoqueBLL
    {
        Task<VestEstoqueDTO> getItemEstoque(int Id);
        Task<IList<EstoqueDTO>> getItensExistentes(int idItens);
        Task<IList<VestEstoqueDTO>> atualizaLogEstoque(int id, List<VestEstoqueDTO> estoque);
    }
}
