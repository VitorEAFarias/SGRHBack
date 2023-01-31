using ControleEPI.DAL.RHContratos;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.RHContratos
{
    public class RHEmpContratosBLL : IRHEmpContratosBLL
    {
        private readonly IRHEmpContratosDAL _contratos;

        public RHEmpContratosBLL(IRHEmpContratosDAL contratos)
        {
            _contratos = contratos;
        }

        public async Task<RHEmpContratosDTO> getContrato(int Id)
        {
            try
            {
                var localizaContrato = await _contratos.getContrato(Id);

                if (localizaContrato != null)
                {
                    return localizaContrato;
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

        public async Task<IEnumerable<RHEmpContratosDTO>> getContratos()
        {
            try
            {
                var localizaContratos = await _contratos.getContratos();

                if (localizaContratos != null)
                {
                    return localizaContratos;
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

        public async Task<RHEmpContratosDTO> getEmpContrato(int Id)
        {
            try
            {
                var localizaEmpContrato = await _contratos.getEmpContrato(Id);

                if (localizaEmpContrato != null)
                {
                    return localizaEmpContrato;
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
