using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;

namespace ControleEPI.BLL
{
    public interface IRHConUserBLL
    {
        Task<IEnumerable<RHDocumentoDTO>> GetDoc();
        Task<RHEmpContatoDTO> getEmail(int idEmpregado);
        Task<IEnumerable<RHEmpregadoDTO>> GetColaboradores();
        Task<List<RHEmpregadoDTO>> getColaboradores(int idSuperior);
        Task<RHEmpregadoDTO> GetEmp(int Id);
        Task<RHSenhaDTO> Get(int id);
        Task<RHSenhaDTO> GetSenha(int id);
        Task<RHEmpContatoDTO> GetEmpCont(int id);
    }
}