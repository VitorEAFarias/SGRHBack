using RH.DAL.RHDepartamentos;
using RH.DAL.RHContratos;
using RH.DAL.RHUsuarios;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilitarios.Utilitários.email;
using ControleEPI.DAL.EPIStatus;
using ControleEPI.DAL.EPIPedidos;
using ControleEPI.DAL.EPITamanhos;
using ControleEPI.DAL.EPIPedidosAprovados;
using ControleEPI.DAL.EPIMotivos;
using ControleEPI.DAL.EPIProdutosEstoque;
using ControleEPI.DAL.EPIProdutos;
using ControleEPI.DAL.EPIVinculos;
using ControleEPI.BLL.EPICertificados;
using ControleEPI.DAL.EPICertificados;

namespace ControleEPI.BLL.EPIPedidos
{
    public class EPIPedidosBLL : IEPIPedidosBLL
    {
        private readonly IEPIPedidosDAL _pedidos;
        private readonly IRHConUserDAL _usuario;
        private readonly IRHEmpContratosDAL _contrato;
        private readonly IRHDepartamentosDAL _departamento;
        private readonly IEPIStatusDAL _status;
        private readonly IEPITamanhosDAL _tamanho;
        private readonly IMailService _mail;
        private readonly IEPIPedidosAprovadosDAL _pedidosAprovados;
        private readonly IEPIMotivosDAL _motivos;
        private readonly IEPIProdutosEstoqueDAL _estoque;
        private readonly IEPIProdutosDAL _produtos;
        private readonly IEPIVinculoDAL _vinculo;
        private readonly IEPICertificadoAprovacaoDAL _certificado;

        public EPIPedidosBLL(IEPIPedidosDAL pedidos, IRHConUserDAL usuario, IRHEmpContratosDAL contrato, IRHDepartamentosDAL departamento, IEPIStatusDAL status, IEPITamanhosDAL tamanho,
            IMailService mail, IEPIPedidosAprovadosDAL pedidosAprovados, IEPIMotivosDAL motivos, IEPIProdutosEstoqueDAL estoque, IEPIProdutosDAL produtos, IEPIVinculoDAL vinculo,
            IEPICertificadoAprovacaoDAL certificado)
        {
            _pedidos = pedidos;
            _usuario = usuario;
            _contrato = contrato;
            _departamento = departamento;
            _status = status;
            _tamanho = tamanho;
            _mail = mail;
            _pedidosAprovados = pedidosAprovados;
            _motivos = motivos;
            _estoque = estoque;
            _produtos = produtos;
            _vinculo = vinculo;
            _certificado = certificado;
        }

        public async Task<PedidosDTO> getPedidoProduto(int Id)
        {
            try
            {
                var pedido = await _pedidos.getPedido(Id);

                if (pedido != null)
                {
                    var motivo = await _motivos.getMotivo(pedido.motivo);
                    var usuario = await _usuario.GetEmp(pedido.idUsuario);
                    var status = await _status.getStatus(pedido.status);

                    List<ProdutosEstoqueDTO> lista = new List<ProdutosEstoqueDTO>();
                    PedidosDTO item = new PedidosDTO();

                    foreach (var produto in pedido.produtos)
                    {
                        var query = await _estoque.getProdutoEstoqueTamanho(produto.id, produto.tamanho);

                        if (query != null)
                        {
                            var localizaProduto = await _produtos.localizaProduto(query.idProduto);                            
                            var nomeStatus = await _status.getStatus(produto.status);
                            var localizaTamanho = await _tamanho.localizaTamanho(produto.tamanho);
                            var localizaCertificado = await _certificado.getCertificado(localizaProduto.idCertificadoAprovacao);

                            if (localizaTamanho != null)
                            {
                                lista.Add(new ProdutosEstoqueDTO
                                {
                                    id = localizaProduto.id,
                                    quantidade = produto.quantidade,
                                    nome = produto.nome,
                                    idTamanho = localizaTamanho.id,
                                    tamanho = localizaTamanho.tamanho,
                                    idStatus = produto.status,
                                    nomeStatus = nomeStatus.nome,
                                    estoque = query.quantidade,
                                    idCertificado = localizaCertificado.id,
                                    numeroCertificado = localizaCertificado.numero
                                });
                            }
                            else
                            {
                                lista.Add(new ProdutosEstoqueDTO
                                {
                                    id = localizaProduto.id,
                                    quantidade = produto.quantidade,
                                    nome = produto.nome,
                                    idTamanho = 0,
                                    tamanho = "Tamanho Único",
                                    idStatus = produto.status,
                                    nomeStatus = nomeStatus.nome,
                                    estoque = query.quantidade,
                                    idCertificado = localizaCertificado.id,
                                    numeroCertificado = localizaCertificado.numero
                                });
                            }                            
                        }
                    }

                    item = new PedidosDTO
                    {
                        idPedido = pedido.id,
                        dataPedido = pedido.dataPedido,
                        descricao = pedido.descricao,
                        idMotivo = motivo.id,
                        produtos = lista,
                        motivo = motivo.nome,
                        usuario = usuario.nome,
                        idUsuario = usuario.id,
                        idStatus = status.id,
                        status = status.nome
                    };

                    return item;
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

        public async Task<IList<PedidosUsuarioDTO>> getPedidos()
        {
            try
            {
                var localizaPedidos = await _pedidos.getPedidos();

                List<PedidosUsuarioDTO> pedidosEncontrados = new List<PedidosUsuarioDTO>();

                if (localizaPedidos != null)
                {
                    foreach (var pedido in localizaPedidos)
                    {
                        var nomeColaborador = await _usuario.GetEmp(pedido.idUsuario);
                        var motivo = await _motivos.getMotivo(pedido.motivo);
                        var status = await _status.getStatus(pedido.status);

                        pedidosEncontrados.Add(new PedidosUsuarioDTO
                        {
                            idPedido = pedido.id,
                            dataPedido = pedido.dataPedido,
                            descricao = pedido.descricao,
                            produtos = pedido.produtos,
                            motivo = motivo.nome,
                            idUsuario = nomeColaborador.id,
                            nomeUsuario = nomeColaborador.nome,
                            idStatus = status.id,
                            status = status.nome
                        });
                    }

                    if (pedidosEncontrados != null)
                    {
                        return pedidosEncontrados;
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

        public async Task<IList<PedidosUsuarioDTO>> localizaPedidosUsuarioStatus(int idUsuario, int idStatus)
        {
            try
            {
                var localizaPedidos = await _pedidos.localizaPedidosUsuarioStatus(idUsuario, idStatus);

                if (localizaPedidos != null)
                {
                    List<PedidosUsuarioDTO> listaPedidos = new List<PedidosUsuarioDTO>();

                    foreach (var item in localizaPedidos)
                    {
                        var motivo = await _motivos.getMotivo(item.motivo);
                        var usuario = await _usuario.GetEmp(item.idUsuario);
                        var status = await _status.getStatus(item.status);

                        listaPedidos.Add(new PedidosUsuarioDTO
                        {
                            idPedido = item.id,
                            dataPedido = item.dataPedido,
                            descricao = item.descricao,
                            produtos = item.produtos,
                            motivo = motivo.nome,
                            idUsuario = usuario.id,
                            nomeUsuario = usuario.nome,
                            idStatus = status.id,
                            status = status.nome
                        });
                    }

                    if (listaPedidos != null)
                    {
                        return listaPedidos;
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

        public async Task<IList<PedidosUsuarioDTO>> getPedidosUsuario(int Id)
        {
            try
            {
                var localizaPedidoUsuario = await _pedidos.getPedidosUsuario(Id);

                List<PedidosUsuarioDTO> listaPedidos = new List<PedidosUsuarioDTO>();

                foreach (var item in localizaPedidoUsuario)
                {
                    var motivo = await _motivos.getMotivo(item.motivo);
                    var usuario = await _usuario.GetEmp(item.idUsuario);
                    var status = await _status.getStatus(item.status);

                    listaPedidos.Add(new PedidosUsuarioDTO
                    {
                        idPedido = item.id,
                        dataPedido = item.dataPedido,
                        descricao = item.descricao,
                        produtos = item.produtos,
                        motivo = motivo.nome,
                        idUsuario = usuario.id,
                        nomeUsuario = usuario.nome,
                        idStatus = status.id,
                        status = status.nome
                    });
                }

                if (listaPedidos != null)
                {
                    return listaPedidos;
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

        public async Task<IList<EPIPedidosDTO>> getTodosPedidos(int status)
        {
            try
            {
                var localizaPedidosStatus = await _pedidos.getTodosPedidos(status);

                if (localizaPedidosStatus != null)
                {
                    return localizaPedidosStatus;
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

        public async Task<EPIPedidosDTO> Insert(EPIPedidosDTO pedido)
        {
            try
            {
                var checkUsuario = await _usuario.GetEmp(pedido.idUsuario);

                if (checkUsuario != null)
                {
                    pedido.dataPedido = DateTime.Now;

                    var novoPedido = await _pedidos.Insert(pedido);

                    if (novoPedido != null)
                    {
                        string tamanho = string.Empty;

                        EmailRequestDTO email = new EmailRequestDTO();
                        ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                        List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();

                        var getEmail = await _usuario.getEmail(checkUsuario.id);

                        if (getEmail != null)
                        {
                            var getContrato = await _contrato.getEmpContrato(pedido.idUsuario);

                            if (getContrato != null)
                            {
                                var getDepartamento = await _departamento.getDepartamento(getContrato.id_departamento);

                                foreach (var produto in pedido.produtos)
                                {
                                    var checkStatusItem = await _status.getStatus(produto.status);
                                    var localizaTamanho = await _tamanho.localizaTamanho(produto.tamanho);

                                    if (localizaTamanho == null)
                                    {
                                        tamanho = "Tamanho Único";
                                    }
                                    else
                                    {
                                        tamanho = localizaTamanho.tamanho;
                                    }

                                    conteudoEmails.Add(new ConteudoEmailDTO
                                    {
                                        nome = produto.nome,
                                        tamanho = tamanho,
                                        status = checkStatusItem.nome,
                                        quantidade = produto.quantidade
                                    });
                                }

                                conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                                {
                                    idPedido = novoPedido.id.ToString(),
                                    nomeColaborador = checkUsuario.nome,
                                    departamento = getDepartamento.titulo
                                };

                                email.EmailDe = getEmail.valor;
                                email.EmailPara = "fabiana.lie@reisoffice.com.br";
                                email.ConteudoColaborador = conteudoEmailColaborador;
                                email.Conteudo = conteudoEmails;
                                email.Assunto = "Novo pedido de EPI";

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

        public async Task<EPIPedidosDTO> reprovaProdutoPedido(EPIPedidosDTO pedido, int idProduto, int idTamanho)
        {
            try
            {
                var checkUsuario = await _usuario.GetEmp(pedido.idUsuario);

                if (checkUsuario != null || !checkUsuario.Equals(0))
                {
                    string tamanho = string.Empty;

                    List<Produtos> produtos = new List<Produtos>();
                    EmailRequestDTO email = new EmailRequestDTO();
                    ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                    List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();

                    var getEmail = await _usuario.getEmail(checkUsuario.id);

                    if (getEmail != null || !getEmail.Equals(0))
                    {
                        var getContrato = await _contrato.getEmpContrato(pedido.idUsuario);

                        if (getContrato != null || !getContrato.Equals(0))
                        {
                            var getDepartamento = await _departamento.getDepartamento(getContrato.id_departamento);

                            foreach (var produto in pedido.produtos)
                            {
                                if (produto.id == idProduto && produto.tamanho == idTamanho)
                                {
                                    var checkStatusItem = await _status.getStatus(produto.status);
                                    var localizaTamanho = await _tamanho.localizaTamanho(produto.tamanho);

                                    if (localizaTamanho == null || localizaTamanho.Equals(0))
                                    {
                                        tamanho = "Tamanho Único";
                                    }
                                    else
                                    {
                                        tamanho = localizaTamanho.tamanho;
                                    }

                                    conteudoEmails.Add(new ConteudoEmailDTO
                                    {
                                        nome = produto.nome,
                                        tamanho = tamanho,
                                        status = checkStatusItem.nome,
                                        quantidade = produto.quantidade
                                    });

                                    produtos.Add(new Produtos
                                    {
                                        id = produto.id,
                                        nome = produto.nome,
                                        quantidade = produto.quantidade,
                                        status = 3,
                                        tamanho = produto.tamanho
                                    });
                                }
                                else
                                {
                                    produtos.Add(new Produtos
                                    {
                                        id = produto.id,
                                        nome = produto.nome,
                                        quantidade = produto.quantidade,
                                        status = produto.status,
                                        tamanho = produto.tamanho
                                    });
                                }
                            }

                            conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                            {
                                idPedido = pedido.id.ToString(),
                                nomeColaborador = checkUsuario.nome,
                                departamento = getDepartamento.titulo
                            };

                            int contador = 0;

                            foreach (var item in produtos)
                            {
                                if (item.status == 3 || item.status == 13 || item.status == 15)
                                {
                                    contador++;
                                }
                            }

                            pedido.produtos = produtos;

                            if (contador == produtos.Count)
                            {
                                pedido.status = 10;
                            }
                            else
                            {
                                pedido.status = pedido.status;
                            }

                            await _pedidos.Update(pedido);

                            email.EmailDe = getEmail.valor;
                            email.EmailPara = "fabiana.lie@reisoffice.com.br";
                            email.ConteudoColaborador = conteudoEmailColaborador;
                            email.Conteudo = conteudoEmails;
                            email.Assunto = "Produto de pedido de EPI reprovado";

                            await _mail.SendEmailAsync(email);

                            return pedido;
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

        public async Task<EPIPedidosDTO> aprovaProdutoPedido(EPIPedidosDTO pedido, int idProduto, int idTamanho)
        {
            try
            {
                var checkUsuario = await _usuario.GetEmp(pedido.idUsuario);

                if (checkUsuario != null || !checkUsuario.Equals(0))
                {
                    string tamanho = string.Empty;
                                                
                    List<Produtos> produtos = new List<Produtos>();
                    EmailRequestDTO email = new EmailRequestDTO();
                    ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                    List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();

                    var getEmail = await _usuario.getEmail(checkUsuario.id);

                    if (getEmail != null || !getEmail.Equals(0))
                    {
                        var getContrato = await _contrato.getEmpContrato(pedido.idUsuario);

                        if (getContrato != null || !getContrato.Equals(0))
                        {
                            var getDepartamento = await _departamento.getDepartamento(getContrato.id_departamento);

                            foreach (var produto in pedido.produtos)
                            {
                                if (produto.id == idProduto && produto.tamanho == idTamanho && produto.status == 2)
                                {
                                    EPIPedidosAprovadosDTO aprovados = new EPIPedidosAprovadosDTO();

                                    var checkStatusItem = await _status.getStatus(produto.status);
                                    var localizaTamanho = await _tamanho.localizaTamanho(produto.tamanho);

                                    if (localizaTamanho != null)
                                    {
                                        conteudoEmails.Add(new ConteudoEmailDTO
                                        {
                                            nome = produto.nome,
                                            tamanho = localizaTamanho.tamanho,
                                            status = checkStatusItem.nome,
                                            quantidade = produto.quantidade
                                        });

                                        produtos.Add(new Produtos
                                        {
                                            id = produto.id,
                                            nome = produto.nome,
                                            quantidade = produto.quantidade,
                                            status = 2,
                                            tamanho = localizaTamanho.id
                                        });

                                        aprovados.idProduto = produto.id;
                                        aprovados.idPedido = pedido.id;
                                        aprovados.idTamanho = produto.tamanho;
                                        aprovados.quantidade = produto.quantidade;
                                        aprovados.enviadoCompra = "A";
                                        aprovados.liberadoVinculo = "N";

                                        await _pedidosAprovados.Insert(aprovados);
                                    }
                                    else
                                    {
                                        conteudoEmails.Add(new ConteudoEmailDTO
                                        {
                                            nome = produto.nome,
                                            tamanho = "Tamanho Único",
                                            status = checkStatusItem.nome,
                                            quantidade = produto.quantidade
                                        });

                                        produtos.Add(new Produtos
                                        {
                                            id = produto.id,
                                            nome = produto.nome,
                                            quantidade = produto.quantidade,
                                            status = 2,
                                            tamanho = 0
                                        });

                                        aprovados.idProduto = produto.id;
                                        aprovados.idPedido = pedido.id;
                                        aprovados.idTamanho = 0;
                                        aprovados.quantidade = produto.quantidade;
                                        aprovados.enviadoCompra = "A";
                                        aprovados.liberadoVinculo = "N";

                                        await _pedidosAprovados.Insert(aprovados);
                                    }                                        
                                }
                                else
                                {
                                    produtos.Add(new Produtos
                                    {
                                        id = produto.id,
                                        nome = produto.nome,
                                        quantidade = produto.quantidade,
                                        status = produto.status,
                                        tamanho = produto.tamanho
                                    });
                                }
                            }

                            conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                            {
                                idPedido = pedido.id.ToString(),
                                nomeColaborador = checkUsuario.nome,
                                departamento = getDepartamento.titulo
                            };

                            int contador = 0;

                            foreach (var item in produtos)
                            {
                                if (item.status == 3 || item.status == 13 || item.status == 15)
                                {
                                    contador++;
                                }
                            }

                            pedido.produtos = produtos;

                            if (contador == produtos.Count)
                            {
                                pedido.status = 10;
                            }
                            else
                            {
                                pedido.status = pedido.status;
                            }

                            email.EmailDe = getEmail.valor;
                            email.EmailPara = "fabiana.lie@reisoffice.com.br";
                            email.ConteudoColaborador = conteudoEmailColaborador;
                            email.Conteudo = conteudoEmails;
                            email.Assunto = "Produto de pedido de EPI aprovado";

                            await _mail.SendEmailAsync(email);

                            var atualizaPedido = await _pedidos.Update(pedido);

                            if (atualizaPedido != null)
                            {
                                return atualizaPedido;
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

        public async Task<EPIPedidosDTO> reprovaPedido(int status, EPIPedidosDTO pedido)
        {
            try
            {
                var checkUsuario = await _usuario.GetEmp(pedido.idUsuario);

                if (checkUsuario != null || !checkUsuario.Equals(0))
                {
                    var localizaPedido = await _pedidos.getPedido(pedido.id);

                    if (localizaPedido != null)
                    {
                        string tamanho = string.Empty;

                        var getEmail = await _usuario.getEmail(checkUsuario.id);

                        if (getEmail != null || !getEmail.Equals(0))
                        {
                            var getContrato = await _contrato.getEmpContrato(pedido.idUsuario);

                            if (getContrato != null || !getContrato.Equals(0))
                            {
                                var getDepartamento = await _departamento.getDepartamento(getContrato.id_departamento);

                                List<Produtos> produtos = new List<Produtos>();
                                EmailRequestDTO email = new EmailRequestDTO();
                                ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                                List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();

                                var idStatus = await _status.getStatus(status);

                                localizaPedido.status = idStatus.id;

                                foreach (var produto in localizaPedido.produtos)
                                {
                                    var checkStatusItem = await _status.getStatus(produto.status);
                                    var localizaTamanho = await _tamanho.localizaTamanho(produto.tamanho);

                                    if (localizaTamanho != null || !localizaTamanho.Equals(0))
                                    {
                                        tamanho = "Tamanho Único";
                                    }
                                    else
                                    {
                                        tamanho = localizaTamanho.tamanho;
                                    }

                                    conteudoEmails.Add(new ConteudoEmailDTO
                                    {
                                        nome = produto.nome,
                                        tamanho = tamanho,
                                        status = checkStatusItem.nome,
                                        quantidade = produto.quantidade
                                    });

                                    produtos.Add(new Produtos
                                    {
                                        id = produto.id,
                                        nome = produto.nome,
                                        quantidade = produto.quantidade,
                                        status = 3,
                                        tamanho = produto.tamanho
                                    });
                                }

                                conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                                {
                                    idPedido = localizaPedido.id.ToString(),
                                    nomeColaborador = checkUsuario.nome,
                                    departamento = getDepartamento.titulo
                                };

                                localizaPedido.produtos = produtos;

                                email.EmailDe = getEmail.valor;
                                email.EmailPara = "fabiana.lie@reisoffice.com.br";
                                email.ConteudoColaborador = conteudoEmailColaborador;
                                email.Conteudo = conteudoEmails;
                                email.Assunto = "Pedido de EPI reprovado";

                                await _mail.SendEmailAsync(email);

                                await _pedidos.Update(localizaPedido);

                                return pedido;
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

        public async Task<EPIPedidosDTO> aprovaPedido(EPIPedidosDTO pedido)
        {
            try
            {
                var checkUsuario = await _usuario.GetEmp(pedido.idUsuario);

                if (checkUsuario != null)
                {
                    var localizaPedido = await _pedidos.getPedido(pedido.id);

                    if (localizaPedido != null)
                    {
                        string tamanho = string.Empty;

                        EPIPedidosAprovadosDTO aprovado = new EPIPedidosAprovadosDTO();
                        EmailRequestDTO email = new EmailRequestDTO();
                        ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                        List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();

                        var getEmail = await _usuario.getEmail(checkUsuario.id);

                        if (getEmail != null || !getEmail.Equals(0))
                        {
                            var getContrato = await _contrato.getEmpContrato(pedido.idUsuario);

                            if (getContrato != null || !getContrato.Equals(0))
                            {
                                var getDepartamento = await _departamento.getDepartamento(getContrato.id_departamento);

                                foreach (var produto in localizaPedido.produtos)
                                {
                                    if (produto.status != 3)
                                    {
                                        var checkStatusItem = await _status.getStatus(produto.status);
                                        var localizaTamanho = await _tamanho.localizaTamanho(produto.tamanho);

                                        if (localizaTamanho != null || !localizaTamanho.Equals(0))
                                        {
                                            tamanho = "Tamanho Único";
                                        }
                                        else
                                        {
                                            tamanho = localizaTamanho.tamanho;
                                        }

                                        conteudoEmails.Add(new ConteudoEmailDTO
                                        {
                                            nome = produto.nome,
                                            tamanho = tamanho,
                                            status = checkStatusItem.nome,
                                            quantidade = produto.quantidade
                                        });

                                        aprovado.idProduto = produto.id;
                                        aprovado.idPedido = localizaPedido.id;
                                        aprovado.idTamanho = produto.tamanho;
                                        aprovado.quantidade = produto.quantidade;
                                        aprovado.enviadoCompra = "A";
                                        aprovado.liberadoVinculo = "N";

                                        await _pedidosAprovados.Insert(aprovado);
                                    }
                                }

                                conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                                {
                                    idPedido = localizaPedido.id.ToString(),
                                    nomeColaborador = checkUsuario.nome,
                                    departamento = getDepartamento.titulo
                                };

                                email.EmailDe = getEmail.valor;
                                email.EmailPara = "fabiana.lie@reisoffice.com.br";
                                email.ConteudoColaborador = conteudoEmailColaborador;
                                email.Conteudo = conteudoEmails;
                                email.Assunto = "Pedido de EPI aprovado";

                                await _mail.SendEmailAsync(email);

                                return pedido;
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

        public async Task<IList<Produtos>> liberarParaVinculo(int idPedido)
        {
            try
            {
                var localizaPedido = await _pedidos.getPedido(idPedido);

                if (localizaPedido != null)
                {
                    List<Produtos> produtos = new List<Produtos>();
                    List<ConteudoEmailDTO> conteudoEmail = new List<ConteudoEmailDTO>();
                    ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                    EmailRequestDTO email = new EmailRequestDTO();

                    var localizaColaborador = await _usuario.GetEmp(localizaPedido.idUsuario);

                    if (localizaColaborador != null)
                    {
                        var localizaEmail = await _usuario.getEmail(localizaColaborador.id);

                        if (localizaEmail != null)
                        {
                            var localizaContrato = await _contrato.getEmpContrato(localizaColaborador.id);

                            if (localizaContrato != null)
                            {
                                foreach (var item in localizaPedido.produtos)
                                {
                                    if (item.status == 7)
                                    {
                                        produtos.Add(new Produtos
                                        {
                                            id = item.id,
                                            nome = item.nome,
                                            quantidade = item.quantidade,
                                            status = 15,
                                            tamanho = item.tamanho
                                        });

                                        var localizaVinculo = await _vinculo.localizaProdutoVinculo(item.id);

                                        localizaVinculo.status = 15;

                                        await _vinculo.Update(localizaVinculo);
                                    }
                                    else
                                    {
                                        produtos.Add(new Produtos
                                        {
                                            id = item.id,
                                            nome = item.nome,
                                            quantidade = item.quantidade,
                                            status = item.status,
                                            tamanho = item.tamanho
                                        });
                                    }
                                }

                                foreach (var produto in produtos)
                                {
                                    if (produto.status == 15)
                                    {
                                        var localizaTamanho = await _tamanho.localizaTamanho(produto.tamanho);
                                        var nomeStatus = await _status.getStatus(13);

                                        if (localizaTamanho != null)
                                        {
                                            conteudoEmail.Add(new ConteudoEmailDTO
                                            {
                                                nome = produto.nome,
                                                tamanho = localizaTamanho.tamanho,
                                                status = nomeStatus.nome,
                                                quantidade = produto.quantidade
                                            });
                                        }
                                        else
                                        {
                                            conteudoEmail.Add(new ConteudoEmailDTO
                                            {
                                                nome = produto.nome,
                                                tamanho = "Tamanho Único",
                                                status = nomeStatus.nome,
                                                quantidade = produto.quantidade
                                            });
                                        }                                        
                                    }
                                }

                                var localizaDepartamento = await _departamento.getDepartamento(localizaContrato.id_departamento);

                                conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                                {
                                    idPedido = localizaPedido.id.ToString(),
                                    nomeColaborador = localizaColaborador.nome,
                                    departamento = localizaDepartamento.titulo
                                };

                                localizaPedido.produtos = produtos;

                                var atualizaPedido = await _pedidos.Update(localizaPedido);

                                if (atualizaPedido != null)
                                {
                                    email.EmailDe = localizaEmail.valor;
                                    email.EmailPara = "fabiana.lie@reisoffice.com.br";
                                    email.ConteudoColaborador = conteudoEmailColaborador;
                                    email.Conteudo = conteudoEmail;
                                    email.Assunto = "Pedido de EPI liberado para retirada";

                                    await _mail.SendEmailAsync(email);

                                    return produtos;
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
