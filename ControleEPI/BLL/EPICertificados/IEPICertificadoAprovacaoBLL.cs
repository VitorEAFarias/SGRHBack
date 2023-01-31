using ControleEPI.DTO;
using ControleEPI.DTO.FromBody;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPICertificados
{
    public interface IEPICertificadoAprovacaoBLL
    {
        Task<EPICertificadoAprovacaoDTO> Insert(EPICertificadoAprovacaoDTO certificado);
        Task<EPICertificadoAprovacaoDTO> getCertificado(int id);
        Task<CertificadoProdutoDTO> getCertificadoProduto(int idCertificadoAprovacao);
        Task<IList<EPICertificadoAprovacaoDTO>> listaStatus(string status);
        Task<EPICertificadoAprovacaoDTO> getValorCertificado(string valor);
        Task<IList<EPICertificadoAprovacaoDTO>> getCertificadosNumero();
        Task<IList<CertificadoProdutoDTO>> getCertificados();
        Task<EPICertificadoAprovacaoDTO> Update(EPICertificadoAprovacaoDTO certificado);
        Task<CertificadoProdutoDTO> getProduto(int id);
        Task<IList<CertificadoProdutoDTO>> getProdutos();
    }
}
