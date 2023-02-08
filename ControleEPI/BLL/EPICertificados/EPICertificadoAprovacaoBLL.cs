using ControleEPI.DAL.EPICertificados;
using ControleEPI.DTO;
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

        public async Task<EPICertificadoAprovacaoDTO> ativaDesativaCertificado(string status, int id, string observacao)
        {
            try
            {
                var localizaCertificado = await _certificado.getCertificado(id);

                if (localizaCertificado != null)
                {
                    localizaCertificado.ativo = status;
                    localizaCertificado.observacao = observacao;

                    if (status == "S")
                    {
                        var ativaCertificado = await _certificado.Update(localizaCertificado);

                        if (ativaCertificado != null)
                        {
                            return ativaCertificado;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {                        
                        var desativaCertificado = await _certificado.Update(localizaCertificado);

                        if (desativaCertificado != null)
                        {
                            return desativaCertificado;
                        }
                        else
                        {
                            return null;
                        }
                    }
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
                if (certificado.validade >= DateTime.Now)
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
    }
}
