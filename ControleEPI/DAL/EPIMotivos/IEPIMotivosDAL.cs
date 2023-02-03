using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPIMotivos
{
    public interface IEPIMotivosDAL
    {
        Task<EPIMotivoDTO> insereMotivo(EPIMotivoDTO motivo);
        Task<EPIMotivoDTO> verificaNome(string nomeMotivo);
        Task<IEnumerable<EPIMotivoDTO>> getMotivos();
        Task<EPIMotivoDTO> getMotivo(int Id);
        Task<EPIMotivoDTO> atualizaMotivo(EPIMotivoDTO motivo);
    }
}
