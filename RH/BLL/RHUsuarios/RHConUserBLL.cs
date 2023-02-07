using RH.DAL.RHCargos;
using RH.DAL.RHContratos;
using RH.DAL.RHDepartamentos;
using RH.DAL.RHUsuarios;
using RH.DTO;
using RH.DTO.DynamicObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilitarios.Utilitários;

namespace RH.BLL.RHUsuarios
{
    public static class Extensions
    {
        /// <summary>
        /// Função array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int findIndex<T>(this T[] array, T item)
        {
            return Array.IndexOf(array, item);
        }
    }

    public class RHConUserBLL : IRHConUserBLL
    {
        private readonly IRHConUserDAL _usuario;
        private readonly IRHEmpContratosDAL _contrato;
        private readonly IRHDepartamentosDAL _departamento;
        private readonly IRHCargosDAL _cargo;

        int[] array = { 34, 29, 47, 35, 13 };

        public RHConUserBLL(IRHConUserDAL usuario, IRHEmpContratosDAL contrato, IRHDepartamentosDAL departamento, IRHCargosDAL cargo)
        {
            _usuario = usuario;
            _contrato = contrato;
            _departamento = departamento;
            _cargo = cargo;
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

        public async Task<IList<EmpregadoDTO>> GetColaboradores()
        {
            try
            {
                var localizaColaboradores = await _usuario.GetColaboradores();

                List<EmpregadoDTO> listaColaboradores = new List<EmpregadoDTO>();

                if (localizaColaboradores != null)
                {
                    foreach (var item in localizaColaboradores)
                    {
                        var contratoColaborador = await _contrato.getEmpContrato(item.id);

                        if (contratoColaborador != null)
                        {
                            var departamentoColaborador = await _departamento.getDepartamento(contratoColaborador.id_departamento);
                            var cargoColaborador = await _cargo.getCargo(contratoColaborador.id_cargo);

                            listaColaboradores.Add(new EmpregadoDTO
                            {
                                id = item.id,
                                nome = item.nome,
                                departamento = departamentoColaborador.titulo,
                                cargo = cargoColaborador.titulo
                            });
                        }
                    }

                    return listaColaboradores;
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

        public async Task<IList<EmpregadoDTO>> getColaboradores(int idSuperior)
        {
            try
            {
                var localizaSuperior = await _usuario.getColaboradores(idSuperior);

                if (localizaSuperior != null)
                {
                    List<EmpregadoDTO> lista = new List<EmpregadoDTO>();

                    foreach (var item in localizaSuperior)
                    {
                        var contratoColaborador = await _contrato.getEmpContrato(item.id);

                        if (contratoColaborador != null)
                        {
                            var departamentoColaborador = await _departamento.getDepartamento(contratoColaborador.id_departamento);
                            var cargoColaborador = await _cargo.getCargo(contratoColaborador.id_cargo);

                            lista.Add(new EmpregadoDTO
                            {
                                id = item.id,
                                nome = item.nome,
                                departamento = departamentoColaborador.titulo,
                                cargo = cargoColaborador.titulo
                            });
                        }
                    }

                    return lista;
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

        public async Task<RHDocumentoDTO> GetDoc(string numero)
        {
            try
            {
                var localizaDocumentos = await _usuario.GetDoc(numero);

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

        public async Task<EmpregadoDTO> GetEmp(int Id)
        {
            try
            {
                var localizaColaborador = await _usuario.GetEmp(Id);

                if (localizaColaborador != null)
                {
                    EmpregadoDTO colaborador = new EmpregadoDTO();

                    var contratoColaborador = await _contrato.getEmpContrato(localizaColaborador.id);
                    var departamentoColaborador = await _departamento.getDepartamento(contratoColaborador.id_departamento);
                    var cargoColaborador = await _cargo.getCargo(contratoColaborador.id_cargo);

                    colaborador = new EmpregadoDTO
                    { 
                        id = localizaColaborador.id,
                        nome = localizaColaborador.nome,
                        departamento = departamentoColaborador.titulo,
                        cargo = cargoColaborador.titulo
                    };

                    return colaborador;
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

        public async Task<LoginDTO> login(LoginDTO login)
        {
            var doc = await GetDoc(login.cpf);

            if (doc != null)
            {
                login.cpf = doc.numero;

                if (login.cpf != "")
                {
                    var empregado = await GetEmp(doc.id_empregado);

                    if (empregado != null)
                    {
                        var senhas = await GetSenha(doc.id_empregado);
                        var emailCorp = await GetEmpCont(doc.id_empregado);
                        var contrato = await _contrato.getEmpContrato(doc.id_empregado);

                        if (contrato != null)
                        {
                            var departamento = await _departamento.getDepartamento(contrato.id_departamento);

                            login.usuario = empregado.nome;
                            login.id = empregado.id;
                            login.email = emailCorp.valor;

                            GerarMD5 md5 = new GerarMD5();

                            var senhaMD5 = md5.GeraMD5(login.senha);

                            if (senhas.senha == senhaMD5)
                            {
                                login.senha = senhas.senha;

                                int index = array.findIndex(departamento.id);

                                if (index != -1 || doc.id_empregado == 122)
                                {
                                    login.adm = true;
                                }

                                if (departamento.titulo == "COMPRAS")
                                {
                                    login.comprador = true;
                                }
                            }
                        }
                    }
                }

                return login;
            }
            else
            {
                return null;
            }
        }
    }
}
