using RH.DAL.RHContratos;
using RH.DAL.RHDepartamentos;
using RH.DAL.RHUsuarios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilitarios.Utilitários.email;
using Vestimenta.DAL.VestEstoque;
using Vestimenta.DAL.VestLog;
using Vestimenta.DAL.VestPedidos;
using Vestimenta.DAL.VestRepositorio;
using Vestimenta.DAL.VestStatus;
using Vestimenta.DAL.VestVestimenta;
using Vestimenta.DAL.VestVinculo;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestPedidos
{
    public class VestPedidosBLL : IVestPedidosBLL
    {
        private readonly IRHConUserDAL _usuario;
        private readonly IRHEmpContratosDAL _contrato;
        private readonly IVestPedidosDAL _pedidos;
        private readonly IRHDepartamentosDAL _departamento;
        private readonly IVestStatusDAL _status;
        private readonly IMailService _mail;
        private readonly IVestEstoqueDAL _estoque;
        private readonly IVestVestimentaDAL _vestimenta;
        private readonly IVestRepositorioDAL _repositorio;
        private readonly IVestLogDAL _log;
        private readonly IVestVinculoDAL _itemVinculo;

        public VestPedidosBLL(IVestPedidosDAL pedidos, IRHConUserDAL usuario, IRHEmpContratosDAL contrato, IRHDepartamentosDAL departamento, IVestStatusDAL status, IMailService mail,
            IVestEstoqueDAL estoque, IVestVestimentaDAL vestimenta, IVestRepositorioDAL repositorio, IVestLogDAL log, IVestVinculoDAL itemVinculo)
        {
            _pedidos = pedidos;
            _usuario = usuario;
            _contrato = contrato;
            _departamento = departamento;
            _status = status;
            _mail = mail;
            _estoque = estoque;
            _vestimenta = vestimenta;
            _repositorio = repositorio;
            _log = log;
            _itemVinculo = itemVinculo;
        }

        public async Task<IList<ItemDTO>> getPedidoItens(int idPedido)
        {
            try
            {
                var getPedidos = await _pedidos.getPedido(idPedido);
                var pedidosVinculo = string.Empty;
                List<ItemDTO> itensLiberadosVinculo = new List<ItemDTO>();

                if (getPedidos != null)
                {
                    foreach (var item in getPedidos.item)
                    {
                        if (item.status == 4)
                        {
                            itensLiberadosVinculo.Add(new ItemDTO
                            {
                                id = item.id,
                                nome = item.nome,
                                tamanho = item.tamanho,
                                quantidade = item.quantidade,
                                status = item.status,
                                dataAlteracao = item.dataAlteracao
                            });
                        }
                    }

                    if (itensLiberadosVinculo != null)
                    {
                        return itensLiberadosVinculo;
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

        public async Task<CompraDTO> getPedido(int idPedido)
        {
            try
            {
                var compra = await _pedidos.getPedido(idPedido);

                if (compra != null)
                {
                    List<ItensCompraDTO> listaItens = new List<ItensCompraDTO>();

                    foreach (var item in compra.item)
                    {
                        string enviadoCompra = string.Empty;
                        var checkEstoque = await _estoque.getItemExistente(item.id, item.tamanho);
                        var checkStatus = await _status.getStatus(item.status);
                        var checkRepositorio = await _repositorio.getRepositorioItensPedidos(item.id, compra.id);

                        if (checkRepositorio != null)
                        {
                            enviadoCompra = checkRepositorio.enviadoCompra;
                        }
                        else
                        {
                            enviadoCompra = "S";
                        }

                        listaItens.Add(new ItensCompraDTO
                        {
                            id = item.id,
                            dataAlteracao = item.dataAlteracao,
                            nome = item.nome,
                            quantidade = item.quantidade,
                            status = item.status,
                            tamanho = item.tamanho,
                            usado = item.usado,
                            enviadoCompra = enviadoCompra,
                            statusNome = checkStatus.nome,
                            estoque = checkEstoque.quantidade,
                            estoqueUsado = checkEstoque.quantidadeUsado
                        });
                    }

                    var emp = await _usuario.GetEmp(compra.idUsuario);
                    var status = await _status.getStatus(compra.status);

                    CompraDTO list = new CompraDTO();

                    list = new CompraDTO
                    {
                        id = compra.id,
                        nome = emp.nome,
                        pedido = listaItens,
                        idStatus = compra.status,
                        status = status.nome,
                        idUsuario = compra.idUsuario,
                        idUsuarioAlteracao = compra.idUsuarioAlteracao,
                        dataAlteracao = compra.dataAlteracao,
                        observacoes = compra.observacoes,
                        dataPedido = compra.dataPedido
                    };

                    if (list != null)
                    {
                        return list;
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

        public async Task<IList<VestPedidosDTO>> atualizaStatusTodosPedidos(List<VestPedidosDTO> pedidosItens)
        {
            try
            {
                bool verificaCompra = false;

                foreach (var pedido in pedidosItens)
                {
                    VestPedidosDTO checkPedido = await _pedidos.getPedido(pedido.id);
                    List<ItemDTO> listaItens = new List<ItemDTO>();

                    if (checkPedido != null)
                    {
                        int cont = 0;

                        foreach (var item in pedido.item)
                        {
                            if (item.status == 5 && checkPedido.item[cont].status != 5)
                            {
                                var getEstoque = await _estoque.getItemExistente(item.id, item.tamanho);

                                VestRepositorioDTO repositorio = new VestRepositorioDTO();

                                repositorio.idItem = item.id;
                                repositorio.idPedido = checkPedido.id;
                                repositorio.enviadoCompra = "N";
                                repositorio.dataAtualizacao = DateTime.Now;
                                repositorio.tamanho = item.tamanho;
                                repositorio.quantidade = item.quantidade;

                                listaItens.Add(new ItemDTO
                                {
                                    id = item.id,
                                    nome = item.nome,
                                    tamanho = item.tamanho,
                                    quantidade = item.quantidade,
                                    status = 5,
                                    dataAlteracao = DateTime.Now
                                });

                                var insereItens = await _repositorio.Insert(repositorio);

                            }
                            else if (item.status == 4 && checkPedido.item[cont].status != 4)
                            {
                                VestEstoqueDTO getEstoque = await _estoque.getItemExistente(item.id, item.tamanho);
                                VestVinculoDTO liberadoVinculo = new VestVinculoDTO();

                                if (item.usado == "N")
                                {
                                    getEstoque.quantidade = getEstoque.quantidade - item.quantidade;
                                    getEstoque.tamanho = item.tamanho;
                                    getEstoque.dataAlteracao = DateTime.Now;
                                    getEstoque.quantidadeVinculado = getEstoque.quantidadeVinculado + item.quantidade;

                                    await _estoque.Update(getEstoque);

                                    VestLogDTO log = new VestLogDTO();

                                    log.data = DateTime.Now;
                                    log.idUsuario = checkPedido.idUsuarioAlteracao;
                                    log.idItem = item.id;
                                    log.quantidadeAnt = getEstoque.quantidade;
                                    log.quantidadeDep = getEstoque.quantidade - item.quantidade;
                                    log.tamanho = item.tamanho;
                                    log.usado = "N";

                                    var insereLog = await _log.Insert(log);

                                    liberadoVinculo.idUsuario = pedido.idUsuario;
                                    liberadoVinculo.idVestimenta = item.id;
                                    liberadoVinculo.dataVinculo = DateTime.MinValue;
                                    liberadoVinculo.status = item.status;
                                    liberadoVinculo.tamanhoVestVinculo = item.tamanho;
                                    liberadoVinculo.dataDesvinculo = DateTime.MinValue;
                                    liberadoVinculo.statusAtual = "N";
                                    liberadoVinculo.idPedido = checkPedido.id;
                                    liberadoVinculo.usado = "N";

                                    var insereItemVinculo = await _itemVinculo.Insert(liberadoVinculo);

                                    item.dataAlteracao = DateTime.Now;
                                    listaItens.Add(item);
                                }
                                else
                                {
                                    getEstoque.quantidadeUsado = getEstoque.quantidadeUsado - item.quantidade;
                                    getEstoque.tamanho = item.tamanho;
                                    getEstoque.dataAlteracao = DateTime.Now;
                                    getEstoque.quantidadeVinculado = getEstoque.quantidadeVinculado + item.quantidade;

                                    await _estoque.Update(getEstoque);

                                    VestLogDTO log = new VestLogDTO();

                                    log.data = DateTime.Now;
                                    log.idUsuario = checkPedido.idUsuarioAlteracao;
                                    log.idItem = item.id;
                                    log.quantidadeAnt = getEstoque.quantidadeUsado;
                                    log.quantidadeDep = item.quantidade;
                                    log.tamanho = item.tamanho;
                                    log.usado = "S";

                                    var insereLog = await _log.Insert(log);

                                    liberadoVinculo.idUsuario = pedido.idUsuario;
                                    liberadoVinculo.idVestimenta = item.id;
                                    liberadoVinculo.dataVinculo = DateTime.MinValue;
                                    liberadoVinculo.status = item.status;
                                    liberadoVinculo.tamanhoVestVinculo = item.tamanho;
                                    liberadoVinculo.dataDesvinculo = DateTime.MinValue;
                                    liberadoVinculo.statusAtual = "N";
                                    liberadoVinculo.idPedido = checkPedido.id;
                                    liberadoVinculo.usado = "S";

                                    var insereItemVinculo = await _itemVinculo.Insert(liberadoVinculo);

                                    if (insereItemVinculo != null)
                                    {
                                        VestRepositorioDTO repositorio = new VestRepositorioDTO();

                                        repositorio.idItem = item.id;
                                        repositorio.idPedido = checkPedido.id;
                                        repositorio.enviadoCompra = "N";
                                        repositorio.dataAtualizacao = DateTime.Now;
                                        repositorio.quantidade = item.quantidade;

                                        var insereCompraItemUsado = await _repositorio.Insert(repositorio);

                                        if (insereCompraItemUsado != null)
                                        {
                                            verificaCompra = true;
                                        }
                                    }

                                    item.dataAlteracao = DateTime.Now;
                                    listaItens.Add(item);
                                }
                            }
                            else if (item.status == 3 && checkPedido.item[cont].status != 3)
                            {
                                var checkRepositorio = await _repositorio.getRepositorioItensPedidos(item.id, checkPedido.id);

                                checkRepositorio.enviadoCompra = "N";
                                checkRepositorio.ativo = "N";

                                await _repositorio.Update(checkRepositorio);

                                item.dataAlteracao = DateTime.Now;
                                listaItens.Add(item);
                            }
                            else
                            {
                                listaItens.Add(item);
                            }

                            cont++;
                        }
                    }
                    else
                    {
                        return null;
                    }

                    checkPedido.item = listaItens;

                    await _pedidos.Update(checkPedido);

                    var verificaPedidoInserido = await _pedidos.getPedido(pedido.id);

                    int contador = 0;

                    foreach (var status in verificaPedidoInserido.item)
                    {
                        if (status.status == 2 || status.status == 7 || status.status == 3 || status.status == 6)
                            contador++;
                    }

                    if (contador == listaItens.Count)
                    {
                        verificaPedidoInserido.status = 2;
                    }
                    else
                    {
                        verificaPedidoInserido.status = verificaPedidoInserido.status;
                    }

                    await _pedidos.Update(verificaPedidoInserido);
                }

                if (verificaCompra == true)
                {
                    return pedidosItens;
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

        public async Task<IList<PedidosPendentesDTO>> getPedidosPendentes()
        {
            try
            {
                var pedidos = await _pedidos.getPedidosPendentes();

                if (pedidos != null)
                {
                    List<PedidosPendentesDTO> pedidoItens = new List<PedidosPendentesDTO>();

                    foreach (var pedido in pedidos)
                    {
                        var getEmp = await _usuario.GetEmp(pedido.idUsuario);
                        var getPedido = await _pedidos.getPedido(pedido.id);

                        foreach (var item in pedido.item)
                        {
                            var checkEstoque = await _estoque.getItemExistente(item.id, item.tamanho);

                            if (checkEstoque != null)
                            {
                                if (item.status == 1)
                                {
                                    pedidoItens.Add(new PedidosPendentesDTO
                                    {
                                        pedido = getPedido,
                                        idItem = item.id,
                                        emitente = getEmp.nome,
                                        nomeItem = item.nome,
                                        tamanhoItem = item.tamanho,
                                        quantidade = item.quantidade,
                                        quantidadeEstoque = checkEstoque.quantidade,
                                        quantidadeEstoqueUsado = checkEstoque.quantidadeUsado,
                                        status = item.status
                                    });
                                }
                            }
                            else
                            {
                                if (item.status == 1)
                                {
                                    pedidoItens.Add(new PedidosPendentesDTO
                                    {
                                        pedido = getPedido,
                                        idItem = item.id,
                                        emitente = getEmp.nome,
                                        nomeItem = item.nome,
                                        tamanhoItem = item.tamanho,
                                        quantidade = item.quantidade,
                                        quantidadeEstoque = 0,
                                        quantidadeEstoqueUsado = 0,
                                        status = item.status
                                    });
                                }
                            }
                        }
                    }

                    if (pedidoItens != null)
                    {
                        return pedidoItens;
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

        public async Task<IList<ItemUsuarioDTO>> getPedidosStatus(int idStatus)
        {
            try
            {
                var pedidos = await _pedidos.getPedidosStatus(idStatus);

                if (pedidos != null)
                {
                    List<ItemUsuarioDTO> lista = new List<ItemUsuarioDTO>();

                    foreach (var item in pedidos)
                    {
                        var emp = await _usuario.GetEmp(item.idUsuario);

                        var status = await _status.getStatus(idStatus);

                        if (emp != null)
                        {
                            lista.Add(new ItemUsuarioDTO
                            {
                                id = item.id,
                                nome = emp.nome,
                                pedido = item.item,
                                status = status.nome,
                                dataPedido = item.dataPedido
                            });
                        }
                        else
                        {
                            return null;
                        }
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
                    var todosPedidos = await _pedidos.getPedidos();

                    List<ItemUsuarioDTO> lista = new List<ItemUsuarioDTO>();

                    foreach (var item in todosPedidos)
                    {
                        var emp = await _usuario.GetEmp(item.idUsuario);
                        var status = await _status.getStatus(item.status);

                        if (emp != null)
                        {
                            lista.Add(new ItemUsuarioDTO
                            {
                                id = item.id,
                                nome = emp.nome,
                                pedido = item.item,
                                status = status.nome,
                                dataPedido = item.dataPedido
                            });
                        }
                        else
                        {
                            return null;
                        }
                    }

                    return lista;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<ItemUsuarioDTO>> getPedidosUsuarios(int idUsuario)
        {
            try
            {
                var emp = await _usuario.GetEmp(idUsuario);

                if (emp != null)
                {
                    var pedidosUsuario = await _pedidos.getPedidosUsuarios(idUsuario);

                    List<ItemUsuarioDTO> lista = new List<ItemUsuarioDTO>();

                    foreach (var item in pedidosUsuario)
                    {
                        var status = await _status.getStatus(item.status);

                        lista.Add(new ItemUsuarioDTO
                        {
                            id = item.id,
                            nome = emp.nome,
                            pedido = item.item,
                            status = status.nome,
                            dataPedido = item.dataPedido
                        });
                    }

                    return lista;
                }
                else
                {
                    var pedidos = await _pedidos.getPedidos();

                    List<ItemUsuarioDTO> lista = new List<ItemUsuarioDTO>();

                    foreach (var item in pedidos)
                    {
                        var status = await _status.getStatus(item.status);

                        lista.Add(new ItemUsuarioDTO
                        {
                            id = item.id,
                            nome = string.Empty,
                            pedido = item.item,
                            status = status.nome,
                            dataPedido = item.dataPedido
                        });
                    }

                    return lista;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<VestPedidosDTO> Insert(VestPedidosDTO pedido)
        {
            try
            {
                var checkUsuario = await _usuario.GetEmp(pedido.idUsuario);

                if (checkUsuario != null)
                {
                    if (pedido != null)
                    {
                        pedido.dataPedido = DateTime.Now;

                        var novoPedido = await _pedidos.Insert(pedido);

                        if (novoPedido != null)
                        {
                            var getEmail = await _usuario.getEmail(checkUsuario.id);
                            var getContrato = await _contrato.getEmpContrato(pedido.idUsuario);
                            var getDepartamento = await _departamento.getDepartamento(getContrato.id_departamento);

                            EmailRequestDTO email = new EmailRequestDTO();
                            ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                            List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();

                            foreach (var item in pedido.item)
                            {
                                var checkStatusItem = await _status.getStatus(item.status);

                                conteudoEmails.Add(new ConteudoEmailDTO
                                {
                                    nome = item.nome,
                                    tamanho = item.tamanho,
                                    status = checkStatusItem.nome,
                                    quantidade = item.quantidade
                                });
                            }

                            conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                            {
                                idPedido = 1.ToString(),
                                nomeColaborador = checkUsuario.nome,
                                departamento = getDepartamento.titulo
                            };

                            email.EmailDe = getEmail.valor;
                            email.EmailPara = "rh@reisoffice.com.br";
                            email.ConteudoColaborador = conteudoEmailColaborador;
                            email.Conteudo = conteudoEmails;
                            email.Assunto = "Novo pedido de vestimenta";

                            await _mail.SendEmailAsync(email);

                            return novoPedido;
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

        public async Task<VestPedidosDTO> atualizaStatusPedidoItem(VestPedidosDTO pedidoItem)
        {
            try
            {
                var checkPedido = await _pedidos.getPedido(pedidoItem.id);

                if (checkPedido != null)
                {
                    bool verificaCompra = false;

                    List<ItemDTO> listaItens = new List<ItemDTO>();
                    List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();
                    ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();

                    int cont = 0;

                    foreach (var item in pedidoItem.item)
                    {
                        var statusPedido = await _status.getStatus(item.status);
                        var nomeColaborador = await _usuario.GetEmp(pedidoItem.idUsuario);
                        var contrato = await _contrato.getEmpContrato(nomeColaborador.id);
                        var departamento = await _departamento.getDepartamento(contrato.id_departamento);

                        if (item.status == 5 && checkPedido.item[cont].status != 5)
                        {
                            var getEstoque = await _estoque.getItemExistente(item.id, item.tamanho);
                            var nomeItem = await _vestimenta.getVestimenta(item.id);
                            var statusItem = await _status.getStatus(item.status);

                            VestRepositorioDTO repositorio = new VestRepositorioDTO();

                            repositorio.idItem = item.id;
                            repositorio.idPedido = pedidoItem.id;
                            repositorio.enviadoCompra = "N";
                            repositorio.dataAtualizacao = DateTime.Now;
                            repositorio.tamanho = item.tamanho;
                            repositorio.quantidade = item.quantidade;

                            listaItens.Add(new ItemDTO
                            {
                                id = item.id,
                                nome = item.nome,
                                tamanho = item.tamanho,
                                quantidade = item.quantidade,
                                status = 5,
                                dataAlteracao = DateTime.Now
                            });

                            conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                            {
                                idPedido = pedidoItem.id.ToString(),
                                nomeColaborador = nomeColaborador.nome,
                                departamento = departamento.titulo
                            };

                            conteudoEmails.Add(new ConteudoEmailDTO
                            {
                                nome = nomeItem.nome,
                                tamanho = item.tamanho,
                                status = statusItem.nome,
                                quantidade = item.quantidade
                            });

                            var insereItens = await _repositorio.Insert(repositorio);

                        }
                        else if (item.status == 4 && checkPedido.item[cont].status != 4)
                        {
                            VestEstoqueDTO getEstoque = await _estoque.getItemExistente(item.id, item.tamanho);
                            VestVinculoDTO liberadoVinculo = new VestVinculoDTO();

                            var nomeItem = await _vestimenta.getVestimenta(item.id);
                            var statusItem = await _status.getStatus(item.status);

                            if (item.usado != "N")
                            {
                                getEstoque.quantidadeUsado = getEstoque.quantidadeUsado - item.quantidade;
                                getEstoque.tamanho = item.tamanho;
                                getEstoque.dataAlteracao = DateTime.Now;
                                getEstoque.quantidadeVinculado = getEstoque.quantidadeVinculado + item.quantidade;

                                await _estoque.Update(getEstoque);

                                VestLogDTO log = new VestLogDTO();

                                log.data = DateTime.Now;
                                log.idUsuario = pedidoItem.idUsuarioAlteracao;
                                log.idItem = item.id;
                                log.quantidadeAnt = getEstoque.quantidade;
                                log.quantidadeDep = getEstoque.quantidade - item.quantidade;
                                log.tamanho = item.tamanho;
                                log.usado = "N";

                                var insereLog = await _log.Insert(log);

                                liberadoVinculo.idUsuario = pedidoItem.idUsuario;
                                liberadoVinculo.idVestimenta = item.id;
                                liberadoVinculo.dataVinculo = DateTime.MinValue;
                                liberadoVinculo.status = item.status;
                                liberadoVinculo.tamanhoVestVinculo = item.tamanho;
                                liberadoVinculo.dataDesvinculo = DateTime.MinValue;
                                liberadoVinculo.statusAtual = "N";
                                liberadoVinculo.idPedido = checkPedido.id;
                                liberadoVinculo.usado = "N";

                                conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                                {
                                    idPedido = pedidoItem.id.ToString(),
                                    nomeColaborador = nomeColaborador.nome,
                                    departamento = departamento.titulo
                                };

                                conteudoEmails.Add(new ConteudoEmailDTO
                                {
                                    nome = nomeItem.nome,
                                    tamanho = item.tamanho,
                                    status = statusItem.nome,
                                    quantidade = item.quantidade
                                });

                                item.dataAlteracao = DateTime.Now;
                                listaItens.Add(item);
                            }
                            else
                            {
                                getEstoque.quantidade = getEstoque.quantidade - item.quantidade;
                                getEstoque.tamanho = item.tamanho;
                                getEstoque.dataAlteracao = DateTime.Now;
                                getEstoque.quantidadeVinculado = getEstoque.quantidadeVinculado + item.quantidade;

                                await _estoque.Update(getEstoque);

                                VestLogDTO log = new VestLogDTO();

                                log.data = DateTime.Now;
                                log.idUsuario = pedidoItem.idUsuarioAlteracao;
                                log.idItem = item.id;
                                log.quantidadeAnt = getEstoque.quantidadeUsado;
                                log.quantidadeDep = item.quantidade;
                                log.tamanho = item.tamanho;
                                log.usado = "S";

                                var insereLog = await _log.Insert(log);

                                liberadoVinculo.idUsuario = pedidoItem.idUsuario;
                                liberadoVinculo.idVestimenta = item.id;
                                liberadoVinculo.dataVinculo = DateTime.MinValue;
                                liberadoVinculo.status = item.status;
                                liberadoVinculo.tamanhoVestVinculo = item.tamanho;
                                liberadoVinculo.dataDesvinculo = DateTime.MinValue;
                                liberadoVinculo.statusAtual = "N";
                                liberadoVinculo.idPedido = checkPedido.id;
                                liberadoVinculo.usado = "N";

                                var insereItemVinculo = await _itemVinculo.Insert(liberadoVinculo);

                                if (insereItemVinculo != null)
                                {
                                    VestRepositorioDTO repositorio = new VestRepositorioDTO();

                                    repositorio.idItem = item.id;
                                    repositorio.idPedido = checkPedido.id;
                                    repositorio.enviadoCompra = "N";
                                    repositorio.dataAtualizacao = DateTime.Now;
                                    repositorio.quantidade = item.quantidade;
                                    repositorio.tamanho = item.tamanho;

                                    var insereCompraItemUsado = await _repositorio.Insert(repositorio);

                                    if (insereCompraItemUsado != null)
                                    {
                                        verificaCompra = true;
                                    }
                                }

                                conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                                {
                                    idPedido = pedidoItem.id.ToString(),
                                    nomeColaborador = nomeColaborador.nome,
                                    departamento = departamento.titulo
                                };

                                conteudoEmails.Add(new ConteudoEmailDTO
                                {
                                    nome = nomeItem.nome,
                                    tamanho = item.tamanho,
                                    status = statusItem.nome,
                                    quantidade = item.quantidade
                                });

                                item.dataAlteracao = DateTime.Now;
                                listaItens.Add(item);
                            }
                        }
                        else if (item.status == 3 && checkPedido.item[cont].status != 3)
                        {
                            var checkRepositorio = await _repositorio.getRepositorioItensPedidos(item.id, checkPedido.id);

                            if (checkRepositorio != null)
                            {
                                checkRepositorio.enviadoCompra = "N";
                                checkRepositorio.ativo = "N";

                                await _repositorio.Update(checkRepositorio);
                            }

                            var nomeItem = await _vestimenta.getVestimenta(item.id);
                            var statusItem = await _status.getStatus(item.status);

                            conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                            {
                                idPedido = pedidoItem.id.ToString(),
                                nomeColaborador = nomeColaborador.nome,
                                departamento = departamento.titulo
                            };

                            conteudoEmails.Add(new ConteudoEmailDTO
                            {
                                nome = nomeItem.nome,
                                tamanho = item.tamanho,
                                status = statusItem.nome,
                                quantidade = item.quantidade
                            });

                            item.dataAlteracao = DateTime.Now;
                            listaItens.Add(item);
                        }
                        else
                        {
                            listaItens.Add(item);
                        }

                        cont++;
                    }

                    pedidoItem.item = listaItens;

                    await _pedidos.Update(pedidoItem);

                    var verificaPedidoInserido = await _pedidos.getPedido(pedidoItem.id);

                    int contador = 0;

                    foreach (var status in verificaPedidoInserido.item)
                    {
                        if (status.status == 2 || status.status == 7 || status.status == 3 || status.status == 6)
                            contador++;
                    }

                    if (contador == listaItens.Count)
                    {
                        verificaPedidoInserido.status = 2;
                    }
                    else
                    {
                        verificaPedidoInserido.status = verificaPedidoInserido.status;
                    }

                    await _pedidos.Update(verificaPedidoInserido);

                    if (conteudoEmails != null)
                    {
                        EmailRequestDTO email = new EmailRequestDTO();

                        var getEmail = await _usuario.getEmail(pedidoItem.idUsuario);
                        var statusDePedido = await _status.getStatus(verificaPedidoInserido.status);

                        email.EmailDe = getEmail.valor;
                        email.EmailPara = "rh@reisoffice.com.br";
                        email.ConteudoColaborador = conteudoEmailColaborador;
                        email.Conteudo = conteudoEmails;
                        email.Assunto = "Atualização de Pedido";

                        await _mail.SendEmailAsync(email);
                    }

                    return pedidoItem;
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

        public async Task<IList<VestPedidosDTO>> getPedidosPendentesUsuario(int idUsuario)
        {
            try
            {
                var pedidos = await _pedidos.getPedidosPendentesUsuario(idUsuario);

                if (pedidos != null)
                {                    
                    return pedidos;                    
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
