using ControleEPI.DAL.RHCargos;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.RHCargos
{
    public class RHCargosBLL : IRHCargosBLL
    {
        private IRHCargosDAL _cargos;

        public RHCargosBLL(IRHCargosDAL cargos)
        {
            _cargos = cargos;       
        }

        public async Task<RHCargosDTO> getCargo(int Id)
        {
            try
            {
                var localizaCargo = await _cargos.getCargo(Id);

                if (localizaCargo != null)
                {
                    return localizaCargo;
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

        public async Task<IEnumerable<RHCargosDTO>> getCargos()
        {
            try
            {
                var localizaCargos = await _cargos.getCargos();

                if (localizaCargos != null)
                {
                    return localizaCargos;
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
