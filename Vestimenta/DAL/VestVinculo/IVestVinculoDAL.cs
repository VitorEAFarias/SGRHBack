using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;

namespace Vestimenta.DAL.VestVinculo
{
    public interface IVestVinculoDAL
    {
        Task<VestVinculoDTO> Insert(VestVinculoDTO vinculo);
        Task<VestVinculoDTO> getVinculo(int Id);
        Task<VestVinculoDTO> getVinculoTamanho(int idPedidos, string tamanho);
        Task<VestVinculoDTO> getUsuarioVinculo(int id);
        Task<IList<VestVinculoDTO>> getVinculoPendente(int idStatus, int idUsuario);
        Task<IList<VestVinculoDTO>> getItensUsuarios(int idUsuario);
        Task<IList<VestVinculoDTO>> getItensVinculados(int idUsuario);
        Task<IList<VestVinculoDTO>> getVinculos();
        Task<VestVinculoDTO> Update(VestVinculoDTO vinculo);
        Task Delete(int id);
    }
}
