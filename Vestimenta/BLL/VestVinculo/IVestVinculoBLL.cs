using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DTO;
using Vestimenta.DTO.FromBody;

namespace Vestimenta.BLL.VestVinculo
{
    public interface IVestVinculoBLL
    {
        Task<VestEstoqueDTO> vinculoHistorico(VestHistoricoVinculadoDTO historico);
        Task<IList<VestVinculoDTO>> aceitaVinculo(int idUsuario, string senha, List<VestPedidoItensVinculoDTO> pedidosItens);
        Task<IList<VinculoDTO>> getVinculoPendente(int idStatus, int idUsuario);
        Task<VestVinculoDTO> atualizaVinculo(int idUsuario, VestVinculoDTO itemVinculo);
        Task<VestVinculoDTO> retiraItemVinculo(bool enviarEstoque, int idVinculo);
        Task<IList<VestVinculoDTO>> getItensUsuarios(int idUsuario);
        Task<IList<VinculoUsuarioDTO>> getItensVinculados(int idUsuario);
    }
}
