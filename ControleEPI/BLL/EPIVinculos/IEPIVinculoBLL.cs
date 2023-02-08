using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPIVinculos
{
    public interface IEPIVinculoBLL
    {
        Task<EPIVinculoDTO> adicionarHistorico(EPIVinculoDTO historico);
        Task<EPIVinculoDTO> devolverItem(int idVinculo);
        Task<IList<EPIVinculoDTO>> vincularItem(List<EPIVinculoDTO> vinculos, int idUsuario, string senha);
        Task<IList<VinculoDTO>> localizaVinculoStatus(int status);
        Task<IList<VinculoDTO>> localizaVinculoUsuario(int usuario);
        Task<IList<VinculoDTO>> vinculoUsuarioStatus(int idUSuario, int idStatus);
    }
}
