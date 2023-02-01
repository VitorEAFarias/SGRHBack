using ControleEPI.DAL.EPICertificados;
using ControleEPI.DTO;
using ControleEPI.DTO.FromBody;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPICertificados
{
    public class EPICertificadoAprovacaoBLL : IEPICertificadoAprovacaoBLL
    {
        private readonly IEPICertificadoAprovacaoDAL _certificado;

        public EPICertificadoAprovacaoBLL(IEPICertificadoAprovacaoDAL certificado)
        {
            _certificado = certificado;
        }

        public async Task<EPICertificadoAprovacaoDTO> getCertificado(int id)
        {
            try
            {
                var localizaCertificado = await _certificado.getCertificado(id);

                if (localizaCertificado != null)
                {
                    return localizaCertificado;
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

        public async Task<CertificadoProdutoDTO> getCertificadoProduto(int idCertificadoAprovacao)
        {
            try
            {
                var localizaCertificadoProduto = await _certificado.getCertificadoProduto(idCertificadoAprovacao);

                if (localizaCertificadoProduto != null)
                {
                    return localizaCertificadoProduto;
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

        public async Task<IList<CertificadoProdutoDTO>> getCertificados()
        {
            try
            {
                var localizaCertificados = await _certificado.getCertificados();

                if (localizaCertificados != null)
                {
                    return localizaCertificados;
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

        public async Task<IList<EPICertificadoAprovacaoDTO>> getCertificadosNumero()
        {
            try
            {
                var localizaNumerosCertificados = await _certificado.getCertificadosNumero();

                if (localizaNumerosCertificados != null)
                {
                    return localizaNumerosCertificados;
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

        public async Task<EPICertificadoAprovacaoDTO> getValorCertificado(string valor)
        {
            try
            {
                var verificaValorCertificado = await _certificado.getValorCertificado(valor);

                if (verificaValorCertificado != null)
                {
                    return verificaValorCertificado;
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

        public async Task<EPICertificadoAprovacaoDTO> Insert(EPICertificadoAprovacaoDTO certificado)
        {
            try
            {
                certificado.ativo = "S";
                certificado.observacao = "";

                var insereCertificado = await _certificado.Insert(certificado);

                if (insereCertificado != null)
                {
                    return insereCertificado;
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

        public async Task<IList<EPICertificadoAprovacaoDTO>> listaStatus(string status)
        {
            try
            {
                var listaCertificadoStatus = await _certificado.listaStatus(status);

                if (listaCertificadoStatus != null)
                {
                    return listaCertificadoStatus;
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

        public async Task<EPICertificadoAprovacaoDTO> Update(EPICertificadoAprovacaoDTO certificado)
        {
            try
            {
                var atualizaCertificado = await _certificado.Update(certificado);

                if (atualizaCertificado != null)
                {
                    return atualizaCertificado;
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

        public async Task<CertificadoProdutoDTO> getProduto(int id)
        {
            try
            {
                var localizaProdutoCertificado = await _certificado.getProduto(id);

                if (localizaProdutoCertificado != null)
                {
                    return localizaProdutoCertificado;
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
