using ControleEPI.DAL.EPIVinculos;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIVinculos
{
    public class EPIVinculoBLL : IEPIVinculoBLL
    {
        private readonly IEPIVinculoDAL _vinculo;

        public EPIVinculoBLL(IEPIVinculoDAL vinculo)
        {
            _vinculo = vinculo;       
        }

        public async Task<EPIVinculoDTO> insereVinculo(EPIVinculoDTO vinculo)
        {
            try
            {
                var insereVinculo = await _vinculo.insereVinculo(vinculo);

                if (insereVinculo != null)
                {
                    return insereVinculo;
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

        public async Task<EPIVinculoDTO> localizaVinculo(int Id)
        {
            try
            {
                var localizaVinculo = await _vinculo.localizaVinculo(Id);

                if (localizaVinculo != null)
                {
                    return localizaVinculo;
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

        public async Task<IList<EPIVinculoDTO>> localizaVinculos()
        {
            try
            {
                var localizaVinculos = await _vinculo.localizaVinculos();

                if (localizaVinculos != null)
                {
                    return localizaVinculos;
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

        public async Task<IList<EPIVinculoDTO>> localizaVinculoStatus(int status)
        {
            try
            {
                var localizaVinculoStatus = await _vinculo.localizaVinculoStatus(status);

                if (localizaVinculoStatus != null)
                {
                    return localizaVinculoStatus;
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

        public async Task<IList<EPIVinculoDTO>> localizaVinculoUsuario(int usuario)
        {
            try
            {
                var localizaVinculoUsuario = await _vinculo.localizaVinculoUsuario(usuario);

                if (localizaVinculoUsuario != null)
                {
                    return localizaVinculoUsuario;
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

        public async Task<EPIVinculoDTO> Update(EPIVinculoDTO vinculo)
        {
            try
            {
                var atualizaVinculo = await _vinculo.Update(vinculo);

                if (atualizaVinculo != null)
                {
                    return atualizaVinculo;
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
