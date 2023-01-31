using ControleEPI.DTO.FromBody;
using ControleEPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.DAL.EPICertificados
{
    public interface IEPICertificadoAprovacaoDAL
    {
        Task<EPICertificadoAprovacaoDTO> Insert(EPICertificadoAprovacaoDTO certificado);
        Task<EPICertificadoAprovacaoDTO> getCertificado(int id);
        Task<CertificadoProdutoDTO> getCertificadoProduto(int idCertificadoAprovacao);
        Task<IList<EPICertificadoAprovacaoDTO>> listaStatus(string status);
        Task<EPICertificadoAprovacaoDTO> getValorCertificado(string valor);
        Task<IList<EPICertificadoAprovacaoDTO>> getCertificadosNumero();
        Task<IList<CertificadoProdutoDTO>> getCertificados();
        Task<EPICertificadoAprovacaoDTO> Update(EPICertificadoAprovacaoDTO certificado);
        Task<IList<CertificadoProdutoDTO>> getProdutos();
        Task<CertificadoProdutoDTO> getProduto(int id);
    }
}
