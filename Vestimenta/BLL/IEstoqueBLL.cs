using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL
{
    public interface IEstoqueBLL
    {
        Task<VestEstoqueDTO> Insert(VestEstoqueDTO estoque);
        Task<VestEstoqueDTO> getItemEstoque(int Id);
        Task<VestEstoqueDTO> getItemExistente(int idItem, string tamanho);
        Task<VestEstoqueDTO> getDesativados(int idItem, string tamanho);
        Task<IList<VestEstoqueDTO>> getItensExistentes(int idItens);
        Task<IList<VestEstoqueDTO>> getEstoque();
        Task Update(VestEstoqueDTO estoque);
        Task Delete(int id);
    }
}
