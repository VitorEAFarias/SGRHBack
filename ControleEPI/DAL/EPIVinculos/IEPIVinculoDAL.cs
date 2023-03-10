using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPIVinculos
{
    public interface IEPIVinculoDAL
    {
        Task<EPIVinculoDTO> insereVinculo(EPIVinculoDTO vinculo);
        Task<IList<EPIVinculoDTO>> localizaVinculoStatus(int status);
        Task<IList<EPIVinculoDTO>> localizaVinculoUsuario(int usuario);
        Task<IList<EPIVinculoDTO>> vinculoUsuarioStatus(int idUSuario, int idStatus);
        Task<EPIVinculoDTO> localizaVinculo(int Id);
        Task<IList<EPIVinculoDTO>> localizaVinculos();
        Task<EPIVinculoDTO> Update(EPIVinculoDTO vinculo);
        Task<EPIVinculoDTO> localizaProdutoVinculo(int idProduto);
    }
}
