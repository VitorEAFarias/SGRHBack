using ControleEPI.DTO;
using ControleEPI.DTO.FromBody;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL
{
    public interface IEPICertificadoAprovacaoBLL
    {
        Task<EPICertificadoAprovacaoDTO> Insert(EPICertificadoAprovacaoDTO certificado);
        Task<EPICertificadoAprovacaoDTO> getCertificado(int id);
        Task<CertificadoProdutoDTO> getCertificadoProduto(int idCertificadoAprovacao);
        Task<EPICertificadoAprovacaoDTO> getValorCertificado(string valor);
        Task<IList<EPICertificadoAprovacaoDTO>> getCertificadosNumero();
        Task<IList<CertificadoProdutoDTO>> getCertificados();
        Task Update(EPICertificadoAprovacaoDTO certificado);
    }
}
