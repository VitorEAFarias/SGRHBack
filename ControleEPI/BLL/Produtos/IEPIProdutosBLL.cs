using ControleEPI.DTO;
using ControleEPI.DTO.FromBody;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.Produtos
{
    public interface IEPIProdutosBLL
    {
        Task<EPIProdutosDTO> Insert(EPIProdutosDTO produto);        
        Task<EPIProdutosDTO> localizaProduto(int id);
        Task<EPIProdutosDTO> ativaDesativaProduto(int id);
        Task<EPIProdutosDTO> getCertificadoProduto(int idCertificado);
        Task<EPIProdutosDTO> verificaCategoria(int idCategoria);
        Task<EPIProdutosDTO> getNomeProduto(string nome);
        Task<IList<EPIProdutosDTO>> getProdutosSolicitacao();        
        Task<IList<EPIProdutosDTO>> produtosStatus(string status);
        Task<EPIProdutosDTO> Update(EPIProdutosDTO produto);
    }
}
