using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPITamanhos
{
    public interface IEPITamanhosDAL
    {
        Task<EPITamanhosDTO> insereTamanho(EPITamanhosDTO tamanho);
        Task<EPITamanhosDTO> localizaTamanho(int Id);
        Task<EPITamanhosDTO> verificaTamanho(string nome);
        Task<IList<EPITamanhosDTO>> localizaTamanhos();
        Task<IList<EPITamanhosDTO>> tamanhosCategoria(int idCategoria);
        Task<EPITamanhosDTO> Update(EPITamanhosDTO tamanho);
        Task<EPITamanhosDTO> Delete(int id);
    }
}
