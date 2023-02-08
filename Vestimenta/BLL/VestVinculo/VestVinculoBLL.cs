using RH.DAL.RHUsuarios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilitarios.Utilitários;
using Vestimenta.DAL.VestEstoque;
using Vestimenta.DAL.VestLog;
using Vestimenta.DAL.VestPedidos;
using Vestimenta.DAL.VestVestimenta;
using Vestimenta.DAL.VestVinculo;
using Vestimenta.DTO;
using Vestimenta.DTO.FromBody;

namespace Vestimenta.BLL.VestVinculo
{
    public class VestVinculoBLL : IVestVinculoBLL
    {
        private readonly IVestPedidosDAL _pedidos;
        private readonly IVestVinculoDAL _vinculo;
        private readonly IRHConUserDAL _usuario;
        private readonly IVestVestimentaDAL _vestimenta;
        private readonly IVestEstoqueDAL _estoque;
        private readonly IVestLogDAL _log;

        public VestVinculoBLL(IVestPedidosDAL pedidos, IVestVinculoDAL vinculo, IRHConUserDAL usuario, IVestVestimentaDAL vestimenta, IVestEstoqueDAL estoque, IVestLogDAL log)
        {
            _pedidos = pedidos;
            _vinculo = vinculo;
            _usuario = usuario;
            _vestimenta = vestimenta;
            _estoque = estoque;
            _log = log;
        }

        public async Task<IList<VestVinculoDTO>> aceitaVinculo(int idUsuario, string senha, List<VestPedidoItensVinculoDTO> pedidosItens)
        {
            try
            {
                var checkUsuario = await _usuario.GetSenha(idUsuario);

                if (checkUsuario != null)
                {
                    GerarMD5 md5 = new GerarMD5();

                    var senhaMD5 = md5.GeraMD5(senha);

                    if (checkUsuario.senha == senhaMD5)
                    {
                        foreach (var pedido in pedidosItens)
                        {
                            foreach (var itemTamanho in pedido.idItens)
                            {
                                var checkVinculo = await _vinculo.getVinculoTamanho(pedido.idPedido, itemTamanho.tamanho);

                                if (checkVinculo != null)
                                {
                                    List<ItemDTO> getItemPedido = new List<ItemDTO>();

                                    checkVinculo.status = 6;
                                    checkVinculo.dataVinculo = DateTime.Now;
                                    checkVinculo.statusAtual = "Y";

                                    await _vinculo.Update(checkVinculo);

                                    var checkPedido = await _pedidos.getPedido(pedido.idPedido);

                                    foreach (var item in checkPedido.item)
                                    {
                                        if (checkVinculo.idVestimenta == item.id && checkVinculo.tamanhoVestVinculo == item.tamanho)
                                        {
                                            getItemPedido.Add(new ItemDTO
                                            {
                                                id = item.id,
                                                nome = item.nome,
                                                tamanho = item.tamanho,
                                                quantidade = item.quantidade,
                                                status = 6,
                                                dataAlteracao = DateTime.Now,
                                                usado = item.usado
                                            });
                                        }
                                        else
                                        {
                                            getItemPedido.Add(new ItemDTO
                                            {
                                                id = item.id,
                                                nome = item.nome,
                                                tamanho = item.tamanho,
                                                quantidade = item.quantidade,
                                                status = item.status,
                                                dataAlteracao = item.dataAlteracao,
                                                usado = item.usado
                                            });
                                        }
                                    }

                                    int contador = 0;

                                    foreach (var status in getItemPedido)
                                    {
                                        if (status.status == 2 || status.status == 7 || status.status == 3 || status.status == 6)
                                            contador++;
                                    }

                                    checkPedido.item = getItemPedido;

                                    if (contador == getItemPedido.Count)
                                    {
                                        checkPedido.status = 2;
                                    }
                                    else
                                    {
                                        checkPedido.status = checkPedido.status;
                                    }

                                    await _pedidos.Update(checkPedido);

                                }
                            }
                        }

                        return (IList<VestVinculoDTO>)pedidosItens;
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

        public async Task<IList<VestVinculoDTO>> getItensUsuarios(int idUsuario)
        {
            try
            {
                var localizaItens = await _vinculo.getItensUsuarios(idUsuario);

                if (localizaItens != null)
                {
                    return localizaItens;
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

        public async Task<IList<VinculoUsuarioDTO>> getItensVinculados(int idUsuario)
        {
            try
            {
                var checkVinculados = await _vinculo.getItensVinculados(idUsuario);

                if (checkVinculados != null)
                {
                    List<VinculoUsuarioDTO> lista = new List<VinculoUsuarioDTO>();

                    foreach (var item in checkVinculados)
                    {
                        var vestimenta = await _vestimenta.getVestimenta(item.idVestimenta);

                        lista.Add(new VinculoUsuarioDTO
                        {
                            idItem = vestimenta.id,
                            idVinculado = item.id,
                            vestimenta = vestimenta.nome,
                            tamanho = item.tamanhoVestVinculo,
                            dataVinculo = item.dataVinculo,
                            dataDesvinculo = item.dataDesvinculo,
                            usado = item.usado,
                            status = item.status,
                            quantidade = item.quantidade
                        });
                    }

                    if (lista != null)
                    {
                        return lista;
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

        public async Task<VestVinculoDTO> retiraItemVinculo(bool enviarEstoque, int idVinculo)
        {
            try
            {
                var checkDesvinculo = await _vinculo.getVinculo(idVinculo);

                if (checkDesvinculo.dataDesvinculo == DateTime.MinValue)
                {
                    if (enviarEstoque == true)
                    {
                        VestEstoqueDTO checkEstoque = await _estoque.getItemExistente(checkDesvinculo.idVestimenta, checkDesvinculo.tamanhoVestVinculo);

                        if (checkEstoque != null)
                        {
                            checkEstoque.quantidadeUsado = checkEstoque.quantidadeUsado + 1;

                            await _estoque.Update(checkEstoque);
                        }
                        else
                        {
                            return null;
                        }
                    }

                    checkDesvinculo.dataDesvinculo = DateTime.Now;

                    var atualizaVinculo = await _vinculo.Update(checkDesvinculo);

                    if (atualizaVinculo != null)
                    {
                        return atualizaVinculo;
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

        public async Task<VestVinculoDTO> atualizaVinculo(int idUsuario, VestVinculoDTO itemVinculo)
        {
            try
            {
                var checkItemVinculo = await _vinculo.getVinculoPendente(itemVinculo.id, idUsuario);

                if (checkItemVinculo != null)
                {
                    var usuario = await _usuario.GetEmp(idUsuario);

                    if (usuario != null)
                    {
                        var checkEstoque = await _estoque.getItemExistente(itemVinculo.idVestimenta, itemVinculo.tamanhoVestVinculo);
                        var nomeVest = await _vestimenta.getVestimenta(itemVinculo.idVestimenta);
                        var checkPedido = await _pedidos.getPedido(itemVinculo.idPedido);

                        foreach (var item in checkPedido.item)
                        {
                            for (int i = 0; i < item.quantidade; i++)
                            {
                                VestVinculoDTO vincular = new VestVinculoDTO();

                                vincular.idUsuario = usuario.id;
                                vincular.idVestimenta = itemVinculo.idVestimenta;
                                vincular.dataVinculo = DateTime.Now;
                                vincular.status = 6;
                                vincular.tamanhoVestVinculo = itemVinculo.tamanhoVestVinculo;
                                vincular.usado = itemVinculo.usado;
                                vincular.dataDesvinculo = DateTime.MinValue;
                                vincular.statusAtual = "Y";
                                vincular.idPedido = itemVinculo.idPedido;

                                var insereVinculo = await _vinculo.Insert(vincular);
                            }

                            VestLogDTO log = new VestLogDTO();

                            log.data = DateTime.Now;
                            log.idUsuario = usuario.id;
                            log.idItem = nomeVest.id;
                            log.quantidadeAnt = checkEstoque.quantidade;
                            log.quantidadeDep = checkEstoque.quantidade - item.quantidade;

                            await _log.Insert(log);
                        }

                        return itemVinculo;
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

        public async Task<IList<VinculoDTO>> getVinculoPendente(int idStatus, int idUsuario)
        {
            try
            {
                var itensPendentes = await _vinculo.getVinculoPendente(idStatus, idUsuario);

                if (itensPendentes != null)
                {
                    List<VinculoDTO> vinculoPendente = new List<VinculoDTO>();

                    foreach (var item in itensPendentes)
                    {
                        var checkVestimenta = await _vestimenta.getVestimenta(item.idVestimenta);
                        var checkUsuario = await _usuario.GetEmp(item.idUsuario);

                        vinculoPendente.Add(new VinculoDTO
                        {
                            id = item.id,
                            idPedido = item.idPedido,
                            idItem = item.idVestimenta,
                            nomeUsuario = checkUsuario.nome,
                            nomeVestimenta = checkVestimenta.nome,
                            tamanho = item.tamanhoVestVinculo,
                            data = item.dataVinculo
                        });
                    }

                    if (vinculoPendente != null)
                    {
                        return vinculoPendente;
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

        public async Task<VestEstoqueDTO> vinculoHistorico(VestHistoricoVinculadoDTO historico)
        {
            try
            {
                var localizaUsuario = await _usuario.GetEmp(historico.idUsuario);

                if (localizaUsuario != null)
                {
                    var localizaVestimenta = await _vestimenta.getVestimenta(historico.idVestimenta);

                    if (localizaVestimenta != null)
                    {
                        VestVinculoDTO vinculo = new VestVinculoDTO();

                        vinculo.idUsuario = localizaUsuario.id;
                        vinculo.idVestimenta = localizaVestimenta.id;
                        vinculo.dataVinculo = historico.dataVinculo;
                        vinculo.status = 6;
                        vinculo.tamanhoVestVinculo = historico.tamanho;
                        vinculo.usado = historico.usado;
                        vinculo.dataDesvinculo = DateTime.MinValue;
                        vinculo.statusAtual = "Y";
                        vinculo.idPedido = 0;
                        vinculo.quantidade = historico.quantidade;

                        var localizaEstoque = await _estoque.getItemExistente(localizaVestimenta.id, historico.tamanho);

                        if (localizaEstoque != null)
                        {
                            localizaEstoque.quantidadeVinculado = localizaEstoque.quantidadeVinculado + historico.quantidade;

                            var insereVinculo = await _vinculo.Insert(vinculo);

                            if (insereVinculo != null)
                            {
                                var atualizaEstoque = await _estoque.Update(localizaEstoque);

                                if (atualizaEstoque != null)
                                {
                                    return atualizaEstoque;
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
