using RH.DAL.RHContratos;
using RH.DAL.RHDepartamentos;
using RH.DAL.RHUsuarios;
using RH.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utilitarios.Utilitários.email;
using Vestimenta.DAL.VestCompras;
using Vestimenta.DAL.VestEstoque;
using Vestimenta.DAL.VestLog;
using Vestimenta.DAL.VestPedidos;
using Vestimenta.DAL.VestRepositorio;
using Vestimenta.DAL.VestStatus;
using Vestimenta.DAL.VestVestimenta;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestCompras
{
    public class VestComprasBLL : IVestComprasBLL
    {
        private readonly IVestComprasDAL _compras;
        private readonly IRHConUserDAL _usuario;
        private readonly IVestVestimentaDAL _vestimenta;
        private readonly IVestStatusDAL _status;
        private readonly IMailService _mail;
        private readonly IRHEmpContratosDAL _contrato;
        private readonly IRHDepartamentosDAL _departamento;
        private readonly IVestRepositorioDAL _repositorio;
        private readonly IVestPedidosDAL _pedidos;
        private readonly IVestEstoqueDAL _estoque;
        private readonly IVestLogDAL _log;

        public VestComprasBLL(IVestComprasDAL compras, IRHConUserDAL usuario, IVestVestimentaDAL vestimenta, IVestStatusDAL status, IMailService mail, IRHEmpContratosDAL contrato,
            IRHDepartamentosDAL departamento, IVestRepositorioDAL repositorio, IVestPedidosDAL pedidos, IVestEstoqueDAL estoque, IVestLogDAL log)
        {
            _compras = compras;
            _usuario = usuario;
            _vestimenta = vestimenta;
            _status = status;
            _mail = mail;
            _contrato = contrato;
            _departamento = departamento;
            _repositorio = repositorio;
            _pedidos = pedidos;
            _estoque = estoque;
            _log = log;
        }

        public async Task<RetornoCompraDTO> getCompra(int Id)
        {
            try
            {
                var checkCompra = await _compras.getCompra(Id);

                if (checkCompra != null)
                {
                    var getUsuario = await _usuario.GetEmp(checkCompra.idUsuario);
                    var getStatus = await _status.getStatus(checkCompra.status);

                    List<RepositorioDTO> lista = new List<RepositorioDTO>();

                    foreach (var item in checkCompra.itensRepositorio)
                    {
                        var checkVestimenta = await _vestimenta.getVestimenta(item.idItem);
                        var total = item.quantidade * item.preco;

                        lista.Add(new RepositorioDTO
                        {
                            idItem = item.idItem,
                            idRepositorio = item.idRepositorio,
                            tamanho = item.tamanho,
                            quantidade = item.quantidade,
                            preco = item.preco,
                            precoTotal = total,
                            nome = checkVestimenta.nome
                        });
                    }

                    var itens = new RetornoCompraDTO
                    {
                        idUsuario = getUsuario.id,
                        idCompra = checkCompra.id,
                        nome = getUsuario.nome,
                        idStatus = getStatus.id,
                        status = getStatus.nome,
                        dataCompra = checkCompra.dataCompra,
                        itensRepositorio = lista
                    };

                    if (itens != null)
                    {
                        return itens;
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

        public async Task<IList<ComprasDTO>> getCompras()
        {
            try
            {
                var compras = await _compras.getCompras();

                List<ComprasDTO> lista = new List<ComprasDTO>();

                foreach (var item in compras)
                {
                    var getUsuario = await _usuario.GetEmp(item.idUsuario);
                    var getStatus = await _status.getStatus(item.status);

                    lista.Add(new ComprasDTO
                    {
                        id = item.id,
                        nome = getUsuario.nome,
                        status = getStatus.nome,
                        dataCompra = item.dataCompra,
                        repositorio = item.itensRepositorio
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<VestComprasDTO> Insert(VestComprasDTO compra)
        {
            try
            {                
                var novaCompra = await _compras.Insert(compra);

                if (novaCompra != null)
                {
                    EmailRequestDTO email = new EmailRequestDTO();
                    List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();

                    var getEmail = await _usuario.getEmail(novaCompra.idUsuario);
                    var getEmp = await _usuario.GetEmp(novaCompra.idUsuario);

                    foreach (var item in novaCompra.itensRepositorio)
                    {
                        var nomeItem = await _vestimenta.getVestimenta(item.idItem);
                        var statusItem = await _status.getStatus(novaCompra.status);

                        conteudoEmails.Add(new ConteudoEmailDTO
                        {
                            nome = nomeItem.nome,
                            tamanho = item.tamanho,
                            status = statusItem.nome,
                            quantidade = item.quantidade
                        });
                    }

                    email.EmailDe = getEmail.valor;
                    email.EmailPara = "simone.maciviero@reisoffice.com.br";
                    email.Conteudo = conteudoEmails;
                    email.Assunto = "Novo pedido de compra avulsa";

                    await _mail.SendEmailAsync(email);

                    return novaCompra;
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

        public async Task<VestComprasDTO> processoDeCompra(VestComprasDTO processoCompra)
        {
            try
            {
                var checkCompra = await _compras.getCompra(processoCompra.id);

                if (checkCompra != null)
                {
                    EmailRequestDTO email = new EmailRequestDTO();
                    ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                    List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();

                    var checkUsuario = await _usuario.GetEmp(processoCompra.idUsuario);

                    if (checkUsuario != null)
                    {
                        var getEmail = await _usuario.getEmail(checkCompra.idUsuario);

                        if (getEmail != null)
                        {
                            var contrato = await _contrato.getEmpContrato(processoCompra.idUsuario);

                            if (contrato != null)
                            {
                                var departamento = await _departamento.getDepartamento(contrato.id_departamento);

                                checkCompra.status = processoCompra.status;

                                await _compras.Update(checkCompra);

                                foreach (var itensComprar in processoCompra.itensRepositorio)
                                {
                                    var getNome = await _vestimenta.getVestimenta(itensComprar.idItem);
                                    var getStatus = await _status.getStatus(processoCompra.status);

                                    conteudoEmails.Add(new ConteudoEmailDTO
                                    {
                                        nome = getNome.nome,
                                        tamanho = itensComprar.tamanho,
                                        status = getStatus.nome,
                                        quantidade = itensComprar.quantidade
                                    });
                                }

                                conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                                {
                                    idPedido = checkCompra.id.ToString(),
                                    nomeColaborador = checkUsuario.nome,
                                    departamento = departamento.titulo
                                };

                                email.EmailDe = getEmail.valor;
                                email.EmailPara = "rh@reisoffice.com.br";
                                email.ConteudoColaborador = conteudoEmailColaborador;
                                email.Conteudo = conteudoEmails;
                                email.Assunto = "Pedido de compra";

                                await _mail.SendEmailAsync(email);

                                return processoCompra;
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

        public async Task<VestComprasDTO> reprovarCompra(VestComprasDTO reprovarCompra)
        {
            try
            {
                var checkCompra = await _compras.getCompra(reprovarCompra.id);

                if (checkCompra != null)
                {
                    EmailRequestDTO email = new EmailRequestDTO();
                    List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();
                    ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();

                    var contrato = await _contrato.getEmpContrato(reprovarCompra.idUsuario);
                    var departamento = await _departamento.getDepartamento(contrato.id_departamento);

                    var getEmail = await _usuario.getEmail(checkCompra.idUsuario);
                    var checkUsuario = await _usuario.GetEmp(reprovarCompra.idUsuario);
                    var nomeStatus = await _status.getStatus(reprovarCompra.status);

                    var nomeItem = string.Empty;
                    var tamanho = string.Empty;
                    int quantidade = 0;

                    if (checkCompra.status != 8)
                    {
                        checkCompra.status = 3;

                        await _compras.Update(checkCompra);

                        foreach (var repositorio in checkCompra.itensRepositorio)
                        {
                            var itemNome = await _vestimenta.getVestimenta(repositorio.idItem);

                            foreach (var idRepositorio in repositorio.idRepositorio)
                            {
                                var getRepositorio = await _repositorio.getRepositorio(idRepositorio);

                                getRepositorio.enviadoCompra = "N";

                                await _repositorio.Update(getRepositorio);
                            }

                            tamanho = repositorio.tamanho;
                            nomeItem = itemNome.nome;
                            quantidade = repositorio.quantidade;
                        }

                        conteudoEmails.Add(new ConteudoEmailDTO
                        {
                            nome = nomeItem,
                            tamanho = tamanho,
                            status = nomeStatus.nome,
                            quantidade = quantidade
                        });

                        conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                        {
                            idPedido = checkCompra.id.ToString(),
                            nomeColaborador = checkUsuario.nome,
                            departamento = departamento.titulo
                        };

                        email.EmailDe = getEmail.valor;
                        email.EmailPara = "rh@reisoffice.com.br";
                        email.ConteudoColaborador = conteudoEmailColaborador;
                        email.Conteudo = conteudoEmails;
                        email.Assunto = "Pedido de compra";

                        await _mail.SendEmailAsync(email);

                        return reprovarCompra;
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

        public async Task<VestComprasDTO> comprarItem(VestComprasDTO comprarItens)
        {
            try
            {
                var checkCompra = await _compras.getCompra(comprarItens.id);

                if (checkCompra.status == 8)
                {
                    var contrato = await _contrato.getEmpContrato(comprarItens.idUsuario);
                    var departamento = await _departamento.getDepartamento(contrato.id_departamento);

                    EmailRequestDTO email = new EmailRequestDTO();
                    ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                    List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();

                    VestPedidosDTO getPedido = new VestPedidosDTO();
                    VestEstoqueDTO getEstoque = new VestEstoqueDTO();
                    VestStatusDTO getStatusItem = new VestStatusDTO();
                    VestRepositorioDTO getRepositorio = new VestRepositorioDTO();                    

                    var getEmail = await _usuario.getEmail(checkCompra.idUsuario);
                    var checkUsuario = await _usuario.GetEmp(comprarItens.idUsuario);

                    foreach (var repositorio in checkCompra.itensRepositorio)
                    {
                        foreach (var idRepositorio in repositorio.idRepositorio)
                        {
                            if (idRepositorio != 0)
                            {
                                getRepositorio = await _repositorio.getRepositorio(idRepositorio);
                                List<ItemDTO> getItemPedido = new List<ItemDTO>();

                                if (!getRepositorio.idPedido.Equals(0))
                                {
                                    getPedido = await _pedidos.getPedido(getRepositorio.idPedido);

                                    foreach (var itens in getPedido.item)
                                    {
                                        if (itens.id == repositorio.idItem && itens.tamanho == repositorio.tamanho)
                                        {
                                            getItemPedido.Add(new ItemDTO
                                            {
                                                id = itens.id,
                                                nome = itens.nome,
                                                tamanho = itens.tamanho,
                                                quantidade = itens.quantidade,
                                                status = 7,
                                                dataAlteracao = DateTime.Now
                                            });

                                            getStatusItem = await _status.getStatus(7);

                                            conteudoEmails.Add(new ConteudoEmailDTO
                                            {
                                                nome = itens.nome,
                                                tamanho = itens.tamanho,
                                                status = getStatusItem.nome,
                                                quantidade = itens.quantidade
                                            });
                                        }
                                        else
                                        {
                                            getItemPedido.Add(new ItemDTO
                                            {
                                                id = itens.id,
                                                nome = itens.nome,
                                                tamanho = itens.tamanho,
                                                quantidade = itens.quantidade,
                                                status = itens.status,
                                                dataAlteracao = itens.dataAlteracao
                                            });

                                            getStatusItem = await _status.getStatus(itens.status);
                                        }
                                    }
                                }

                                int contador = 0;

                                foreach (var status in getItemPedido)
                                {
                                    if (status.status == 2 || status.status == 7 || status.status == 3 || status.status == 6)
                                        contador++;
                                }

                                getPedido.item = getItemPedido;

                                if (contador == getItemPedido.Count)
                                {
                                    getPedido.status = 2;
                                }
                                else
                                {
                                    getPedido.status = getPedido.status;
                                }

                                await _pedidos.Update(getPedido);
                                await _compras.Update(comprarItens);
                            }
                            else
                            {
                                checkCompra.status = comprarItens.status;

                                await _compras.Update(checkCompra);
                            }

                            getEstoque = await _estoque.getItemExistente(repositorio.idItem, repositorio.tamanho);

                            var quantidadeAnterior = getEstoque.quantidade;

                            if (getEstoque != null)
                            {
                                getEstoque.quantidade = getEstoque.quantidade + repositorio.quantidade;
                                getEstoque.dataAlteracao = DateTime.Now;

                                VestLogDTO log = new VestLogDTO();

                                log.data = DateTime.Now;
                                log.idUsuario = checkCompra.idUsuario;
                                log.idItem = repositorio.idItem;
                                log.quantidadeAnt = quantidadeAnterior;
                                log.quantidadeDep = getEstoque.quantidade;
                                log.tamanho = repositorio.tamanho;
                                log.usado = "N";

                                await _estoque.Update(getEstoque);
                                var insereLogEstoqueDisponivel = await _log.Insert(log);
                            }
                        }
                    }

                    conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                    {
                        idPedido = checkCompra.id.ToString(),
                        nomeColaborador = checkUsuario.nome,
                        departamento = departamento.titulo
                    };

                    email.EmailDe = getEmail.valor;
                    email.EmailPara = "rh@reisoffice.com.br";
                    email.ConteudoColaborador = conteudoEmailColaborador;
                    email.Conteudo = conteudoEmails;
                    email.Assunto = "Pedido de compra";

                    await _mail.SendEmailAsync(email);

                    return comprarItens;
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

        public async Task<VestComprasDTO> enviarParaCompra(VestComprasDTO compras)
        {
            try
            {
                compras.dataCompra = DateTime.Now;

                var insereCompra = await _compras.Insert(compras);

                if (insereCompra != null)
                {
                    List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();
                    ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                    EmailRequestDTO email = new EmailRequestDTO();
                    RHEmpContatoDTO empContato = new RHEmpContatoDTO();
                    var checkUsuario = await _usuario.GetEmp(compras.idUsuario);

                    foreach (var item in compras.itensRepositorio)
                    {
                        foreach (var idRepositorio in item.idRepositorio)
                        {
                            VestRepositorioDTO repositorio = await _repositorio.getRepositorio(idRepositorio);

                            if (repositorio != null)
                            {
                                repositorio.enviadoCompra = "S";
                                repositorio.dataAtualizacao = DateTime.Now;

                                await _repositorio.Update(repositorio);
                            }
                        }

                        var getNomeItem = await _vestimenta.getVestimenta(item.idItem);
                        var getStatusItem = await _status.getStatus(compras.status);
                        var nomeEmp = await _usuario.GetEmp(insereCompra.idUsuario);
                        var contrato = await _contrato.getEmpContrato(insereCompra.idUsuario);
                        var departamento = await _departamento.getDepartamento(contrato.id_departamento);

                        empContato = await _usuario.getEmail(compras.idUsuario);

                        conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                        {
                            idPedido = insereCompra.id.ToString(),
                            nomeColaborador = nomeEmp.nome,
                            departamento = departamento.titulo
                        };

                        conteudoEmails.Add(new ConteudoEmailDTO
                        {
                            nome = getNomeItem.nome,
                            tamanho = item.tamanho,
                            status = getStatusItem.nome,
                            quantidade = item.quantidade
                        });
                    }

                    email.EmailDe = empContato.valor;
                    email.EmailPara = "simone.maciviero@reisoffice.com.br";
                    email.ConteudoColaborador = conteudoEmailColaborador;
                    email.Conteudo = conteudoEmails;
                    email.Assunto = "Enviar itens para compras";

                    await _mail.SendEmailAsync(email);

                    return insereCompra;
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
