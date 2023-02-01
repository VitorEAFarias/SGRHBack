using System.Collections.Generic;
using System.Threading.Tasks;
using ControleEPI.DTO;
using ControleEPI.DTO.FromBody;

namespace ControleEPI.BLL.RHUsuarios
{
    public interface IRHConUserBLL
    {
        Task<RHDocumentoDTO> GetDoc(string numero);
        Task<LoginDTO> login(LoginDTO login);
        Task<RHEmpContatoDTO> getEmail(int idEmpregado);
        Task<IList<EmpregadoDTO>> GetColaboradores();
        Task<IList<EmpregadoDTO>> getColaboradores(int idSuperior);        
        Task<EmpregadoDTO> GetEmp(int Id);
        Task<RHSenhaDTO> Get(int id);
        Task<RHSenhaDTO> GetSenha(int id);
        Task<RHEmpContatoDTO> GetEmpCont(int id);
    }
}