using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.BLL
{
    public interface IVestRepositorioBLL
    {
        Task<VestRepositorioDTO> Insert(VestRepositorioDTO repo);
        Task<VestRepositorioDTO> getRepositorio(int Id);
        Task<VestRepositorioDTO> getRepositorioItensPedidos(int idPedido, int idItem);
        Task<IList<VestRepositorioDTO>> getRepositorioStatus(string status);
        Task<IList<VestRepositorioDTO>> getRepositorios();
        Task Update(VestRepositorioDTO repo);
        Task Delete(int id);
    }
}
