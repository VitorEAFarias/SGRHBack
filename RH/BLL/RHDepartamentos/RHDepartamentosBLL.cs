using RH.DAL.RHDepartamentos;
using RH.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RH.BLL.RHDepartamentos
{
    public class RHDepartamentosBLL : IRHDepartamentosBLL
    {
        private readonly IRHDepartamentosDAL _departamento;

        public RHDepartamentosBLL(IRHDepartamentosDAL departamento)
        {
            _departamento = departamento;
        }

        public async Task<RHDepartamentosDTO> getDepartamento(int Id)
        {
            try
            {
                var localizaDepartamento = await _departamento.getDepartamento(Id);

                if (localizaDepartamento != null)
                {
                    return localizaDepartamento;
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

        public async Task<IEnumerable<RHDepartamentosDTO>> getDepartamentos()
        {
            try
            {
                var localizaDepartamentos = await _departamento.getDepartamentos();

                if (localizaDepartamentos != null)
                {
                    return localizaDepartamentos;
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
