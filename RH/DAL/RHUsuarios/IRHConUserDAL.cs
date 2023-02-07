using RH.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RH.DAL.RHUsuarios
{
    public interface IRHConUserDAL
    {
        Task<RHDocumentoDTO> GetDoc(string numero);
        Task<RHEmpContatoDTO> getEmail(int idEmpregado);
        Task<IList<RHEmpregadoDTO>> GetColaboradores();
        Task<IList<RHEmpregadoDTO>> getColaboradores(int idSuperior);
        Task<RHEmpregadoDTO> GetEmp(int Id);
        Task<RHSenhaDTO> Get(int id);
        Task<RHSenhaDTO> GetSenha(int id);
        Task<RHEmpContatoDTO> GetEmpCont(int id);
    }
}
