using ControleEPI.DAL.EPICategorias;
using ControleEPI.DAL.EPICertificados;
using ControleEPI.DAL.EPIPedidos;
using ControleEPI.DAL.EPIProdutos;
using ControleEPI.DAL.EPIProdutosEstoque;
using ControleEPI.DAL.EPIStatus;
using ControleEPI.DAL.EPITamanhos;
using ControleEPI.DAL.EPIVinculos;
using ControleEPI.DTO;
using RH.DAL.RHUsuarios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilitarios.Utilitários;

namespace ControleEPI.BLL.EPIVinculos
{
    public class EPIVinculoBLL : IEPIVinculoBLL
    {
        private readonly IEPIVinculoDAL _vinculo;
        private readonly IRHConUserDAL _usuario;
        private readonly IEPIProdutosDAL _produtos;
        private readonly IEPIStatusDAL _status;
        private readonly IEPICertificadoAprovacaoDAL _certificado;
        private readonly IEPICategoriasDAL _categoria;
        private readonly IEPITamanhosDAL _tamanho;
        private readonly IEPIProdutosEstoqueDAL _estoque;
        private readonly IEPIPedidosDAL _pedidos;

        public EPIVinculoBLL(IEPIVinculoDAL vinculo, IRHConUserDAL usuario, IEPIProdutosDAL produtos, IEPIStatusDAL status, IEPICertificadoAprovacaoDAL certificado,
            IEPICategoriasDAL categoria, IEPITamanhosDAL tamanho, IEPIProdutosEstoqueDAL estoque, IEPIPedidosDAL pedidos)
        {
            _vinculo = vinculo;
            _usuario = usuario;
            _produtos = produtos;
            _status = status;
            _certificado = certificado;
            _categoria = categoria;
            _tamanho = tamanho;
            _estoque = estoque;
            _pedidos = pedidos;
        }

        public async Task<EPIVinculoDTO> adicionarHistorico(EPIVinculoDTO historico)
        {
            try
            {
                historico.dataDevolucao = DateTime.MinValue;

                var insereHistorico = await _vinculo.insereVinculo(historico);

                if (insereHistorico != null)
                {
                    return insereHistorico;
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

        public async Task<IList<EPIVinculoDTO>> vincularItem(List<EPIVinculoDTO> vinculos, int idUsuario, string senha)
        {
            try
            {
                var localizaUsuario = await _usuario.GetSenha(idUsuario);

                if (localizaUsuario != null)
                {
                    GerarMD5 md5 = new GerarMD5();

                    var senhaMD5 = md5.GeraMD5(senha);

                    if (localizaUsuario.senha == senhaMD5)
                    {
                        foreach (var item in vinculos)
                        {
                            var localizaUsuarioVinculo = await _vinculo.localizaVinculo(item.id);
                            var localizaProduto = await _produtos.localizaProduto(item.idItem);
                            var localizaProdutoEstoque = await _estoque.getProdutoEstoqueTamanho(localizaProduto.id, item.idTamanho);
                            var localizaPedido = await _pedidos.getPedido(item.idPedido);

                            foreach(var item2 in localizaPedido.produtos)
                            {
                                if(item2.id == item.idItem && item2.tamanho == item.idTamanho)
                                {
                                    localizaProdutoEstoque.quantidade = localizaProdutoEstoque.quantidade - item2.quantidade;
                                }
                            }

                            await _estoque.Update(localizaProdutoEstoque);

                            localizaUsuarioVinculo.dataVinculo = DateTime.Now;
                            localizaUsuarioVinculo.status = 13;
                            localizaUsuarioVinculo.validade = DateTime.Now.AddYears(localizaProduto.validadeEmUso);

                            await _vinculo.Update(localizaUsuarioVinculo);
                        }

                        return vinculos;
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

        public async Task<EPIVinculoDTO> devolverItem(int idVinculo)
        {
            try
            {
                var localizaVinculo = await _vinculo.localizaVinculo(idVinculo);

                if (localizaVinculo != null)
                {
                    localizaVinculo.status = 12;
                    localizaVinculo.dataDevolucao = DateTime.Now;

                    var devolverItem = await _vinculo.Update(localizaVinculo);

                    if (devolverItem != null)
                    {
                        return devolverItem;
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

        public async Task<IList<VinculoDTO>> vinculoUsuarioStatus(int idUsuario, int idStatus)
        {
            try
            {
                var listarVinculos = await _vinculo.vinculoUsuarioStatus(idUsuario, idStatus);

                if (listarVinculos != null)
                {
                    List<VinculoDTO> listaVinculos = new List<VinculoDTO>();

                    foreach (var item in listarVinculos)
                    {
                        var localizaEmp = await _usuario.GetEmp(item.idUsuario);
                        var localizaProduto = await _produtos.localizaProduto(item.idItem);
                        var localizaCertificado = await _certificado.getCertificado(localizaProduto.idCertificadoAprovacao);
                        var localizaCategoria = await _categoria.getCategoria(localizaProduto.idCategoria);
                        var localizaStatus = await _status.getStatus(item.status);
                        var localizaTamanho = await _tamanho.localizaTamanho(item.idTamanho);

                        if (localizaTamanho != null)
                        {
                            listaVinculos.Add(new VinculoDTO
                            {
                                idVinculo = item.id,
                                certificado = localizaCertificado.numero,
                                categoria = localizaCategoria.nome,
                                idUsuario = localizaEmp.id,
                                idPedido = item.idPedido,
                                nomeUsuario = localizaEmp.nome,
                                idItem = localizaProduto.id,
                                nomeItem = localizaProduto.nome,
                                idTamanho = localizaTamanho.id,
                                tamanho = localizaTamanho.tamanho,
                                dataVinculo = item.dataVinculo,
                                dataDevolucao = item.dataDevolucao,
                                idStatus = localizaStatus.id,
                                status = localizaStatus.nome,
                                validade = item.validade
                            });
                        }
                        else
                        {
                            listaVinculos.Add(new VinculoDTO
                            {
                                idVinculo = item.id,
                                certificado = localizaCertificado.numero,
                                categoria = localizaCategoria.nome,
                                idUsuario = localizaEmp.id,
                                idPedido = item.idPedido,
                                nomeUsuario = localizaEmp.nome,
                                idItem = localizaProduto.id,
                                nomeItem = localizaProduto.nome,
                                idTamanho = 0,
                                tamanho = "Tamanho Único",
                                dataVinculo = item.dataVinculo,
                                dataDevolucao = item.dataDevolucao,
                                idStatus = localizaStatus.id,
                                status = localizaStatus.nome,
                                validade = item.validade
                            });
                        }
                    }

                    if (listaVinculos != null)
                    {
                        return listaVinculos;
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

        public async Task<IList<VinculoDTO>> localizaVinculoStatus(int status)
        {
            try
            {
                var localizaVinculoStatus = await _vinculo.localizaVinculoStatus(status);

                if (localizaVinculoStatus != null)
                {
                    List<VinculoDTO> listaVinculos = new List<VinculoDTO>();

                    foreach (var item in localizaVinculoStatus)
                    {
                        var localizaEmp = await _usuario.GetEmp(item.idUsuario);
                        var localizaProduto = await _produtos.localizaProduto(item.idItem);
                        var localizaCertificado = await _certificado.getCertificado(localizaProduto.idCertificadoAprovacao);
                        var localizaCategoria = await _categoria.getCategoria(localizaProduto.idCategoria);
                        var localizaStatus = await _status.getStatus(item.status);
                        var localizaTamanho = await _tamanho.localizaTamanho(item.idTamanho);

                        listaVinculos.Add(new VinculoDTO
                        {
                            idVinculo = item.id,
                            certificado = localizaCertificado.numero,
                            categoria = localizaCategoria.nome,
                            idUsuario = localizaEmp.id,
                            idPedido = item.idPedido,
                            nomeUsuario = localizaEmp.nome,
                            idItem = localizaProduto.id,
                            nomeItem = localizaProduto.nome,
                            idTamanho = localizaTamanho.id,
                            tamanho = localizaTamanho.tamanho,
                            dataVinculo = item.dataVinculo,
                            dataDevolucao = item.dataDevolucao,
                            idStatus = localizaStatus.id,
                            status = localizaStatus.nome,
                            validade = DateTime.MinValue
                        });
                    }

                    if (listaVinculos != null)
                    {
                        return listaVinculos;
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

        public async Task<IList<VinculoDTO>> localizaVinculoUsuario(int usuario)
        {
            try
            {
                var localizaVinculoUsuario = await _vinculo.localizaVinculoUsuario(usuario);

                if (localizaVinculoUsuario != null)
                {
                    List<VinculoDTO> listaVinculos = new List<VinculoDTO>();

                    foreach (var item in localizaVinculoUsuario)
                    {
                        var localizaEmp = await _usuario.GetEmp(item.idUsuario);
                        var localizaProduto = await _produtos.localizaProduto(item.idItem);
                        var localizaCertificado = await _certificado.getCertificado(localizaProduto.idCertificadoAprovacao);
                        var localizaCategoria = await _categoria.getCategoria(localizaProduto.idCategoria);
                        var localizaStatus = await _status.getStatus(item.status);
                        var localizaTamanho = await _tamanho.localizaTamanho(item.idTamanho);

                        if (localizaTamanho != null)
                        {
                            listaVinculos.Add(new VinculoDTO
                            {
                                idVinculo = item.id,
                                certificado = localizaCertificado.numero,
                                categoria = localizaCategoria.nome,
                                idUsuario = localizaEmp.id,
                                idPedido = item.idPedido,
                                nomeUsuario = localizaEmp.nome,
                                idItem = localizaProduto.id,
                                nomeItem = localizaProduto.nome,
                                idTamanho = localizaTamanho.id,
                                tamanho = localizaTamanho.tamanho,
                                dataVinculo = item.dataVinculo,
                                dataDevolucao = item.dataDevolucao,
                                idStatus = localizaStatus.id,
                                status = localizaStatus.nome,
                                validade = item.validade
                            });
                        }
                        else
                        {
                            listaVinculos.Add(new VinculoDTO
                            {
                                idVinculo = item.id,
                                certificado = localizaCertificado.numero,
                                categoria = localizaCategoria.nome,
                                idUsuario = localizaEmp.id,
                                idPedido = item.idPedido,
                                nomeUsuario = localizaEmp.nome,
                                idItem = localizaProduto.id,
                                nomeItem = localizaProduto.nome,
                                idTamanho = 0,
                                tamanho = "Tamanho Único",
                                dataVinculo = item.dataVinculo,
                                dataDevolucao = item.dataDevolucao,
                                idStatus = localizaStatus.id,
                                status = localizaStatus.nome,
                                validade = item.validade
                            });
                        }
                    }

                    if (listaVinculos != null)
                    {
                        return listaVinculos;
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
    }
}
