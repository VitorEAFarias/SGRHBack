using ControleEPI.DAL.EPICompras;
using ControleEPI.DAL.EPIPedidos;
using ControleEPI.DAL.EPIPedidosAprovados;
using ControleEPI.DAL.EPIProdutos;
using ControleEPI.DAL.EPIProdutosEstoque;
using ControleEPI.DAL.EPIStatus;
using ControleEPI.DAL.EPITamanhos;
using ControleEPI.DAL.EPIVinculos;
using RH.DAL.RHUsuarios;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilitarios.Utilitários.email;
using RH.DAL.RHDepartamentos;
using RH.DAL.RHContratos;
using RH.DTO;
using ControleEPI.DAL.EPICertificados;

namespace ControleEPI.BLL.EPIPedidosAprovados
{
    public class EPIPedidosAprovadosBLL : IEPIPedidosAprovadosBLL
    {
        private readonly IEPIPedidosAprovadosDAL _pedidosAprovados;
        private readonly IRHConUserDAL _usuario;
        private readonly IEPITamanhosDAL _tamanho;
        private readonly IEPIPedidosDAL _pedidos;
        private readonly IEPIProdutosDAL _produtos;
        private readonly IEPIStatusDAL _status;
        private readonly IEPIProdutosEstoqueDAL _estoque;
        private readonly IMailService _mail;
        private readonly IEPIComprasDAL _compras;
        private readonly IEPIVinculoDAL _vinculo;
        private readonly IRHDepartamentosDAL _departamento;
        private readonly IRHEmpContratosDAL _contratos;
        private readonly IEPICertificadoAprovacaoDAL _certificado;

        public EPIPedidosAprovadosBLL(IEPIPedidosAprovadosDAL pedidosAprovados, IRHConUserDAL usuario, IEPITamanhosDAL tamanho, IEPIPedidosDAL pedidos, IEPIProdutosDAL produtos,
            IEPIStatusDAL status, IEPIProdutosEstoqueDAL estoque, IMailService mail, IEPIComprasDAL compras, IEPIVinculoDAL vinculo, IRHDepartamentosDAL departamento, 
            IRHEmpContratosDAL contratos, IEPICertificadoAprovacaoDAL certificado)
        {
            _pedidosAprovados = pedidosAprovados;
            _usuario = usuario;
            _tamanho = tamanho;
            _pedidos = pedidos;
            _produtos = produtos;
            _status = status;
            _estoque = estoque;
            _mail = mail;
            _compras = compras;
            _vinculo = vinculo;
            _departamento = departamento;
            _contratos = contratos;
            _certificado = certificado;
        }

        public async Task<IList<PedidosAprovadosDTO>> getProdutosAprovados(string statusCompra, string statusVinculo)
        {
            try
            {
                var localizaProdutosAprovados = await _pedidosAprovados.getProdutosAprovados(statusCompra, statusVinculo);

                List<PedidosAprovadosDTO> produtos = new List<PedidosAprovadosDTO>();

                foreach (var item in localizaProdutosAprovados)
                {
                    var localizaProduto = await _produtos.localizaProduto(item.idProduto);
                    var localizaPedido = await _pedidos.getPedido(item.idPedido);
                    var localizaProdutoEstoque = await _estoque.getProdutoEstoqueTamanho(item.idProduto, item.idTamanho);
                    var localizaTamanho = await _tamanho.localizaTamanho(localizaProdutoEstoque.idTamanho);
                    var localizaCertificado = await _certificado.getCertificado(localizaProduto.idCertificadoAprovacao);

                    if (localizaPedido != null)
                    {
                        var localizaUsuario = await _usuario.GetEmp(localizaPedido.idUsuario);

                        if (localizaTamanho != null)
                        {
                            if (localizaPedido != null)
                            {
                                produtos.Add(new PedidosAprovadosDTO
                                {
                                    idProdutoAprovado = item.id,
                                    enviadoCompra = item.enviadoCompra,
                                    idPedido = item.idPedido,
                                    idProduto = localizaProdutoEstoque.idProduto,
                                    nome = localizaProduto.nome,
                                    idTamanho = localizaProdutoEstoque.idTamanho,
                                    tamanho = localizaTamanho.tamanho,
                                    quantidade = item.quantidade,
                                    idUsuario = localizaUsuario.id,
                                    usuario = localizaUsuario.nome,
                                    dataPedido = localizaPedido.dataPedido,
                                    estoque = localizaProdutoEstoque.quantidade,
                                    liberadoVinculo = item.liberadoVinculo,
                                    idCertificado = localizaCertificado.id,
                                    numeroCertificado = localizaCertificado.numero
                                });
                            }
                            else
                            {
                                produtos.Add(new PedidosAprovadosDTO
                                {
                                    idProdutoAprovado = item.id,
                                    enviadoCompra = item.enviadoCompra,
                                    idPedido = item.idPedido,
                                    idProduto = localizaProdutoEstoque.idProduto,
                                    nome = localizaProduto.nome,
                                    idTamanho = localizaProdutoEstoque.idTamanho,
                                    tamanho = localizaTamanho.tamanho,
                                    quantidade = item.quantidade,
                                    idUsuario = 0,
                                    usuario = string.Empty,
                                    dataPedido = DateTime.MinValue,
                                    estoque = localizaProdutoEstoque.quantidade,
                                    liberadoVinculo = item.liberadoVinculo,
                                    idCertificado = localizaCertificado.id,
                                    numeroCertificado = localizaCertificado.numero
                                });
                            }
                        }
                        else
                        {
                            if (localizaPedido != null)
                            {
                                produtos.Add(new PedidosAprovadosDTO
                                {
                                    idProdutoAprovado = item.id,
                                    enviadoCompra = item.enviadoCompra,
                                    idPedido = item.idPedido,
                                    idProduto = localizaProdutoEstoque.idProduto,
                                    nome = localizaProduto.nome,
                                    idTamanho = 0,
                                    tamanho = "Tamanho Único",
                                    quantidade = item.quantidade,
                                    idUsuario = localizaUsuario.id,
                                    usuario = localizaUsuario.nome,
                                    dataPedido = localizaPedido.dataPedido,
                                    estoque = localizaProdutoEstoque.quantidade,
                                    liberadoVinculo = item.liberadoVinculo,
                                    idCertificado = localizaCertificado.id,
                                    numeroCertificado = localizaCertificado.numero
                                });
                            }
                            else
                            {
                                produtos.Add(new PedidosAprovadosDTO
                                {
                                    idProdutoAprovado = item.id,
                                    enviadoCompra = item.enviadoCompra,
                                    idPedido = item.idPedido,
                                    idProduto = localizaProdutoEstoque.idProduto,
                                    nome = localizaProduto.nome,
                                    idTamanho = 0,
                                    tamanho = "Tamanho Único",
                                    quantidade = item.quantidade,
                                    idUsuario = 0,
                                    usuario = string.Empty,
                                    dataPedido = DateTime.MinValue,
                                    estoque = localizaProdutoEstoque.quantidade,
                                    liberadoVinculo = item.liberadoVinculo,
                                    idCertificado = localizaCertificado.id,
                                    numeroCertificado = localizaCertificado.numero
                                });
                            }
                        }
                    }
                    else
                    {
                        produtos.Add(new PedidosAprovadosDTO
                        {
                            idProdutoAprovado = item.id,
                            enviadoCompra = item.enviadoCompra,
                            idPedido = 0,
                            idProduto = localizaProdutoEstoque.idProduto,
                            nome = localizaProduto.nome,
                            idTamanho = localizaTamanho.id,
                            tamanho = localizaTamanho.tamanho,
                            quantidade = item.quantidade,
                            idUsuario = 0,
                            usuario = string.Empty,
                            dataPedido = DateTime.MinValue,
                            estoque = localizaProdutoEstoque.quantidade,
                            liberadoVinculo = item.liberadoVinculo,
                            idCertificado = localizaCertificado.id,
                            numeroCertificado = localizaCertificado.numero
                        });
                    }                                       
                }

                if (produtos != null)
                {
                    return produtos;
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

        public async Task<IList<EPIPedidosAprovadosDTO>> insereProdutosAprovados(List<EPIPedidosAprovadosDTO> produtosAprovados)
        {
            try
            {
                foreach (var item in produtosAprovados)
                {
                    item.idPedido = 0;

                    await _pedidosAprovados.Insert(item);
                }

                return produtosAprovados;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IList<EPIPedidosAprovadosDTO>> enviaParaCompras(List<EPIPedidosAprovadosDTO> enviaCompras, int idUsuario)
        {
            try
            {
                var localizaUsuario = await _usuario.GetEmp(idUsuario);
                var localizaContrato = await _contratos.getEmpContrato(localizaUsuario.id);
                var localizaDepartamento = await _departamento.getDepartamento(localizaContrato.id_departamento);

                if (localizaUsuario != null)
                {
                    if (enviaCompras != null)
                    {
                        EmailRequestDTO email = new EmailRequestDTO();
                        ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();                        
                        EPIComprasDTO compras = new EPIComprasDTO();
                        RHEmpContatoDTO getEmail = new RHEmpContatoDTO();

                        List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();
                        List<PedidosAprovados> pedidosAprovados = new List<PedidosAprovados>();

                        string str = string.Empty;
                        decimal valorTotalCompra = 0;
                        string tamanho = string.Empty;

                        foreach (var produto in enviaCompras)
                        {
                            List<Produtos> produtosCompras = new List<Produtos>();

                            str = str + "," +produto.id.ToString();

                            pedidosAprovados.Add(new PedidosAprovados {
                                idPedidosAprovados = produto.id
                            });

                            var localizaPedido = await _pedidos.getPedido(produto.idPedido);                            
                            var checkStatusItem = await _status.getStatus(4);                            

                            EPITamanhosDTO localizaTamanho = new EPITamanhosDTO();
                            EPIProdutosDTO localizaProduto = new EPIProdutosDTO();

                            if (localizaPedido != null)
                            {
                                getEmail = await _usuario.getEmail(localizaPedido.idUsuario);

                                foreach (var item in localizaPedido.produtos)
                                {
                                    localizaProduto = await _produtos.localizaProduto(item.id);

                                    if (item.id == produto.idProduto && item.tamanho == produto.idTamanho)
                                    {
                                        localizaTamanho = await _tamanho.localizaTamanho(item.tamanho);
                                    }

                                    if (item.id == produto.idProduto && produto.enviadoCompra == "A")
                                    {
                                        produtosCompras.Add(new Produtos
                                        {
                                            id = localizaProduto.id,
                                            nome = localizaProduto.nome,
                                            quantidade = item.quantidade,
                                            status = 4,
                                            tamanho = item.tamanho
                                        });
                                    }
                                    else
                                    {
                                        produtosCompras.Add(new Produtos
                                        {
                                            id = localizaProduto.id,
                                            nome = localizaProduto.nome,
                                            quantidade = item.quantidade,
                                            status = item.status,
                                            tamanho = item.tamanho
                                        });
                                    }
                                }

                                int cont = 0;

                                foreach (var produtos in produtosCompras)
                                {
                                    if (produtos.status == 2 || produtos.status == 15 || produtos.status == 3 || produtos.status == 7)
                                    {
                                        cont++;
                                    }
                                }

                                if (cont == produtosCompras.Count)
                                {
                                    localizaPedido.status = 10;
                                }
                                else
                                {
                                    localizaPedido.status = localizaPedido.status;
                                }

                                localizaPedido.produtos = produtosCompras;

                                await _pedidos.Update(localizaPedido);

                                if (localizaTamanho != null)
                                {
                                    conteudoEmails.Add(new ConteudoEmailDTO
                                    {
                                        nome = localizaProduto.nome,
                                        tamanho = localizaTamanho.tamanho,
                                        status = checkStatusItem.nome,
                                        quantidade = produto.quantidade
                                    });
                                }
                                else
                                {
                                    conteudoEmails.Add(new ConteudoEmailDTO
                                    {
                                        nome = localizaProduto.nome,
                                        tamanho = "Tamanho Único",
                                        status = checkStatusItem.nome,
                                        quantidade = produto.quantidade
                                    });
                                }

                                produto.enviadoCompra = "S";
                            }
                            else
                            {
                                getEmail = await _usuario.getEmail(localizaUsuario.id);
                                localizaTamanho = await _tamanho.localizaTamanho(produto.idTamanho);

                                if (localizaTamanho != null)
                                {
                                    conteudoEmails.Add(new ConteudoEmailDTO
                                    {
                                        nome = localizaProduto.nome,
                                        tamanho = localizaTamanho.tamanho,
                                        status = checkStatusItem.nome,
                                        quantidade = produto.quantidade
                                    });
                                }
                                else
                                {
                                    conteudoEmails.Add(new ConteudoEmailDTO
                                    {
                                        nome = localizaProduto.nome,
                                        tamanho = "Tamanho Único",
                                        status = checkStatusItem.nome,
                                        quantidade = produto.quantidade
                                    });
                                }

                                produto.enviadoCompra = "S";
                            }

                            await _pedidosAprovados.Update(produto);
                        }                        

                        conteudoEmailColaborador = new ConteudoEmailColaboradorDTO {
                            idPedido = str,
                            nomeColaborador = localizaUsuario.nome,
                            departamento = localizaDepartamento.titulo
                        };

                        email.EmailDe = getEmail.valor;
                        email.EmailPara = "fabiana.lie@reisoffice.com.br";
                        email.ConteudoColaborador = conteudoEmailColaborador;
                        email.Conteudo = conteudoEmails;
                        email.Assunto = "EPI enviado para compras";

                        await _mail.SendEmailAsync(email);

                        compras.idPedidosAprovados = pedidosAprovados;
                        compras.dataCadastroCompra = DateTime.Now;

                        foreach (var item in pedidosAprovados)
                        {
                            var localizaPedidoAprovado = await _pedidosAprovados.getProdutoAprovado(item.idPedidosAprovados, "S");
                            var localizaProduto = await _produtos.localizaProduto(localizaPedidoAprovado.idProduto);
                            decimal valorTotal = localizaPedidoAprovado.quantidade * localizaProduto.preco;
                            valorTotalCompra = valorTotalCompra + valorTotal;
                        }

                        compras.valorTotalCompra = valorTotalCompra;
                        compras.status = 1;
                        compras.idUsuario = idUsuario;
                        compras.dataFinalizacaoCompra = DateTime.MinValue;

                        var insereCompra = await _compras.Insert(compras);

                        if (insereCompra != null)
                        {
                            return enviaCompras;
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

        public async Task<IList<EPIPedidosAprovadosDTO>> atualizaVinculos(List<EPIPedidosAprovadosDTO> produtosAprovados)
        {
            try
            {
                if (produtosAprovados != null)
                {
                    List<ConteudoEmailDTO> conteudoEmail = new List<ConteudoEmailDTO>();
                    ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                    EmailRequestDTO email = new EmailRequestDTO();                    

                    foreach (var item in produtosAprovados)
                    {
                        List<Produtos> produtosPedido = new List<Produtos>();

                        var localizaPedido = await _pedidos.getPedido(item.idPedido);

                        if (localizaPedido == null)
                        {
                            continue;
                        }

                        foreach (var item2 in localizaPedido.produtos)
                        {
                            if (item2.status == 2)
                            {
                                produtosPedido.Add(new Produtos
                                {
                                    id = item2.id,
                                    nome = item2.nome,
                                    quantidade = item2.quantidade,
                                    status = 15,
                                    tamanho = item2.tamanho
                                });
                            }
                            else
                            {
                                produtosPedido.Add(new Produtos
                                {
                                    id = item2.id,
                                    nome = item2.nome,
                                    quantidade = item2.quantidade,
                                    status = item2.status,
                                    tamanho = item2.tamanho
                                });
                            }
                        }

                        int cont = 0;

                        foreach (var produtos in produtosPedido)
                        {
                            if (produtos.status == 2 || produtos.status == 15 || produtos.status == 3 || produtos.status == 7)
                            {
                                cont++;
                            }
                        }

                        if (cont == produtosPedido.Count)
                        {
                            localizaPedido.status = 10;
                        }

                        localizaPedido.produtos = produtosPedido;

                        await _pedidos.Update(localizaPedido);

                        var localizaProduto = await _produtos.localizaProduto(item.idProduto);
                        var localizaTamanho = await _tamanho.localizaTamanho(item.idTamanho);
                        var localizaStatus = await _status.getStatus(15);

                        EPIVinculoDTO novoVinculo = new EPIVinculoDTO();

                        novoVinculo.idUsuario = localizaPedido.idUsuario;
                        novoVinculo.idItem = item.idProduto;
                        novoVinculo.idTamanho = item.idTamanho;
                        novoVinculo.dataVinculo = DateTime.Now;
                        novoVinculo.idPedido = localizaPedido.id;
                        novoVinculo.status = 15;
                        novoVinculo.dataDevolucao = DateTime.MinValue;
                        novoVinculo.validade = DateTime.Now.AddYears(localizaProduto.validadeEmUso);                        

                        var insereVinculo = await _vinculo.insereVinculo(novoVinculo);

                        if (localizaTamanho != null)
                        {
                            conteudoEmail.Add(new ConteudoEmailDTO
                            {
                                nome = localizaProduto.nome,
                                tamanho = localizaTamanho.tamanho,
                                status = localizaStatus.nome,
                                quantidade = item.quantidade
                            });
                        }
                        else
                        {
                            conteudoEmail.Add(new ConteudoEmailDTO
                            {
                                nome = localizaProduto.nome,
                                tamanho = "Tamanho Único",
                                status = localizaStatus.nome,
                                quantidade = item.quantidade
                            });
                        }                        

                        var localizaUsuario = await _usuario.GetEmp(localizaPedido.idUsuario);
                        var localizaContrato = await _contratos.getEmpContrato(localizaUsuario.id);
                        var localizaDepartamento = await _departamento.getDepartamento(localizaContrato.id_departamento);
                        var localizaEmail = await _usuario.getEmail(localizaUsuario.id);

                        conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                        {
                            idPedido = localizaPedido.id.ToString(),
                            nomeColaborador = localizaUsuario.nome,
                            departamento = localizaDepartamento.titulo
                        };

                        item.liberadoVinculo = "S";

                        await _pedidosAprovados.Update(item);

                        email.EmailDe = localizaEmail.valor;
                        email.EmailPara = "fabiana.lie@reisoffice.com.br";
                        email.ConteudoColaborador = conteudoEmailColaborador;
                        email.Conteudo = conteudoEmail;
                        email.Assunto = "Pedido de EPI liberado para retirada";

                        await _mail.SendEmailAsync(email);
                    }

                    return produtosAprovados;
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
