using ControleEPI.DAL.Certificado;
using ControleEPI.DTO;
using ControleEPI.DTO.FromBody;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEPI.BLL.Certificado
{
    public class EPICertificadoAprovacaoBLL : IEPICertificadoAprovacaoBLL
    {
        private readonly IEPICertificadoAprovacaoDAL _certificado;

        public EPICertificadoAprovacaoBLL(IEPICertificadoAprovacaoDAL certificado)
        {
            _certificado = certificado;
        }

        public Task<EPICertificadoAprovacaoDTO> getCertificado(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CertificadoProdutoDTO> getCertificadoProduto(int idCertificadoAprovacao)
        {
            throw new NotImplementedException();
        }

        public Task<IList<CertificadoProdutoDTO>> getCertificados()
        {
            throw new NotImplementedException();
        }

        public Task<IList<EPICertificadoAprovacaoDTO>> getCertificadosNumero()
        {
            throw new NotImplementedException();
        }        

        public Task<EPICertificadoAprovacaoDTO> getValorCertificado(string valor)
        {
            throw new NotImplementedException();
        }

        public Task<EPICertificadoAprovacaoDTO> Insert(EPICertificadoAprovacaoDTO certificado)
        {
            throw new NotImplementedException();
        }

        public Task<IList<EPICertificadoAprovacaoDTO>> listaStatus(string status)
        {
            throw new NotImplementedException();
        }

        public Task Update(EPICertificadoAprovacaoDTO certificado)
        {
            throw new NotImplementedException();
        }

        public Task<CertificadoProdutoDTO> getProduto(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<CertificadoProdutoDTO>> getProdutos()
        {
            try
            {
                var localizaProdutos = await _certificado.getProdutos();

                if (localizaProdutos != null)
                {
                    return localizaProdutos;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
