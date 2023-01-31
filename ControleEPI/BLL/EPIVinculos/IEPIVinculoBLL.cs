using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPIVinculos
{
    public interface IEPIVinculoBLL
    {
        Task<EPIVinculoDTO> insereVinculo(EPIVinculoDTO vinculo);
        Task<IList<EPIVinculoDTO>> localizaVinculoStatus(int status);
        Task<IList<EPIVinculoDTO>> localizaVinculoUsuario(int usuario);
        Task<EPIVinculoDTO> localizaVinculo(int Id);
        Task<IList<EPIVinculoDTO>> localizaVinculos();
        Task<EPIVinculoDTO> Update(EPIVinculoDTO vinculo);
    }
}
