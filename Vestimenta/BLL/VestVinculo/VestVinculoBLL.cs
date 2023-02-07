using RH.DAL.RHUsuarios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilitarios.Utilitários;
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

        public VestVinculoBLL(IVestPedidosDAL pedidos, IVestVinculoDAL vinculo, IRHConUserDAL usuario, IVestVestimentaDAL vestimenta)
        {
            _pedidos = pedidos;
            _vinculo = vinculo;
            _usuario = usuario;
            _vestimenta = vestimenta;
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
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

        public Task<IList<VestVinculoDTO>> getItensUsuarios(int idUsuario)
        {
            throw new NotImplementedException();
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

        public Task<VestVinculoDTO> getUsuarioVinculo(int id)
        {
            throw new NotImplementedException();
        }

        public Task<VestVinculoDTO> getVinculo(int Id)
        {
            throw new NotImplementedException();
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

        public Task<IList<VestVinculoDTO>> getVinculos()
        {
            throw new NotImplementedException();
        }

        public Task<VestVinculoDTO> getVinculoTamanho(int idPedidos, string tamanho)
        {
            throw new NotImplementedException();
        }

        public Task<VestVinculoDTO> Insert(VestVinculoDTO vinculo)
        {
            throw new NotImplementedException();
        }

        public Task Update(VestVinculoDTO vinculo)
        {
            throw new NotImplementedException();
        }
    }
}
