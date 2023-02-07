using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;
using Vestimenta.DTO.FromBody;

namespace Vestimenta.BLL.VestVinculo
{
    public interface IVestVinculoBLL
    {
        Task<VestVinculoDTO> Insert(VestVinculoDTO vinculo);
        Task<VestVinculoDTO> getVinculo(int Id);
        Task<VestVinculoDTO> getVinculoTamanho(int idPedidos, string tamanho);
        Task<VestVinculoDTO> getUsuarioVinculo(int id);
        Task<IList<VestVinculoDTO>> aceitaVinculo(int idUsuario, string senha, List<VestPedidoItensVinculoDTO> pedidosItens);
        Task<IList<VinculoDTO>> getVinculoPendente(int idStatus, int idUsuario);
        Task<IList<VestVinculoDTO>> getItensUsuarios(int idUsuario);
        Task<IList<VinculoUsuarioDTO>> getItensVinculados(int idUsuario);
        Task<IList<VestVinculoDTO>> getVinculos();
        Task Update(VestVinculoDTO vinculo);
        Task Delete(int id);
    }
}
