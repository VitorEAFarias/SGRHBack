using ControleEPI.DAL.RHUsuarios;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.RHUsuarios
{
    public class RHConUserBLL : IRHConUserBLL
    {
        private readonly IRHConUserDAL _usuario;

        public RHConUserBLL(IRHConUserDAL usuario)
        {
            _usuario = usuario;
        }

        public async Task<RHSenhaDTO> Get(int id)
        {
            try
            {
                var verificaSenha = await _usuario.Get(id);

                if (verificaSenha != null)
                {
                    return verificaSenha;
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

        public async Task<IEnumerable<RHEmpregadoDTO>> GetColaboradores()
        {
            try
            {
                var localizaColaboradores = await _usuario.GetColaboradores();

                if (localizaColaboradores != null)
                {
                    return localizaColaboradores;
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

        public async Task<List<RHEmpregadoDTO>> getColaboradores(int idSuperior)
        {
            try
            {
                var localizaSuperior = await _usuario.getColaboradores(idSuperior);

                if (localizaSuperior != null)
                {
                    return localizaSuperior;
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

        public async Task<IEnumerable<RHDocumentoDTO>> GetDoc()
        {
            try
            {
                var localizaDocumentos = await _usuario.GetDoc();

                if (localizaDocumentos != null)
                {
                    return localizaDocumentos;
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

        public async Task<RHEmpContatoDTO> getEmail(int idEmpregado)
        {
            try
            {
                var localizaEmail = await _usuario.getEmail(idEmpregado);

                if (localizaEmail != null)
                {
                    return localizaEmail;
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

        public async Task<RHEmpregadoDTO> GetEmp(int Id)
        {
            try
            {
                var localizaColaborador = await _usuario.GetEmp(Id);

                if (localizaColaborador != null)
                {
                    return localizaColaborador;
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

        public async Task<RHEmpContatoDTO> GetEmpCont(int id)
        {
            try
            {
                var localizaContratoColaborador = await _usuario.GetEmpCont(id);

                if (localizaContratoColaborador != null)
                {
                    return localizaContratoColaborador;
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

        public async Task<RHSenhaDTO> GetSenha(int id)
        {
            try
            {
                var verificaSenha = await _usuario.GetSenha(id);

                if (verificaSenha != null)
                {
                    return verificaSenha;
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
