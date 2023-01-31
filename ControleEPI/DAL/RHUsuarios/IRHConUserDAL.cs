using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.RHUsuarios
{
    public interface IRHConUserDAL
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
