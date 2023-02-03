using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL.EPIMotivos
{
    public interface IEPIMotivosBLL
    {
        Task<EPIMotivoDTO> insereMotivo(EPIMotivoDTO motivo);
        Task<IEnumerable<EPIMotivoDTO>> getMotivos();
        Task<EPIMotivoDTO> getMotivo(int Id);
        Task<EPIMotivoDTO> atualizaMotivo(EPIMotivoDTO motivo);
    }
}
