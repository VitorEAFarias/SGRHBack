using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestEstoque
{
    public interface IVestEstoqueBLL
    {
        Task<VestEstoqueDTO> Insert(VestEstoqueDTO estoque);
        Task<VestEstoqueDTO> getItemEstoque(int Id);
        Task<VestEstoqueDTO> getItemExistente(int idItem, string tamanho);
        Task<VestEstoqueDTO> getDesativados(int idItem, string tamanho);
        Task<IList<EstoqueDTO>> getItensExistentes(int idItens);
        Task<IList<VestEstoqueDTO>> getEstoque();
        Task<IList<VestEstoqueDTO>> atualizaLogEstoque(int id, List<VestEstoqueDTO> estoque);
        Task Update(VestEstoqueDTO estoque);
        Task Delete(int id);
    }
}
