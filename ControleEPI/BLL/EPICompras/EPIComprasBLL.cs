using ControleEPI.DAL.EPICompras;
using ControleEPI.DAL.EPIFornecedores;
using ControleEPI.DAL.EPIPedidos;
using ControleEPI.DAL.EPIPedidosAprovados;
using ControleEPI.DAL.EPIProdutos;
using ControleEPI.DAL.EPIProdutosEstoque;
using ControleEPI.DAL.EPIStatus;
using ControleEPI.DAL.EPITamanhos;
using ControleEPI.DAL.EPIVinculos;
using RH.DAL.RHContratos;
using RH.DAL.RHDepartamentos;
using RH.DAL.RHUsuarios;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilitarios.Utilitários.email;
using RH.DTO;

namespace ControleEPI.BLL.EPICompras
{
    public class EPIComprasBLL : IEPIComprasBLL
    {
        private readonly IEPIComprasDAL _compras;
        private readonly IEPIPedidosAprovadosDAL _pedidoAprovado;
        private readonly IEPIProdutosDAL _produtos;
        private readonly IEPITamanhosDAL _tamanhos;
        private readonly IEPIStatusDAL _status;
        private readonly IRHConUserDAL _usuario;
        private readonly IEPIFornecedoresDAL _fornecedor;
        private readonly IRHEmpContratosDAL _contrato;
        private readonly IRHDepartamentosDAL _departamento;
        private readonly IEPIProdutosEstoqueDAL _produtosEstoque;
        private readonly IEPIPedidosDAL _pedidos;
        private readonly IEPIVinculoDAL _vinculo;
        private readonly IMailService _mail;

        public EPIComprasBLL(IEPIComprasDAL compras, IEPIPedidosAprovadosDAL pedidoAprovado, IEPIProdutosDAL produtos, IEPITamanhosDAL tamanhos, IEPIStatusDAL status, IRHConUserDAL usuario,
            IEPIFornecedoresDAL fornecedor, IRHEmpContratosDAL contrato, IRHDepartamentosDAL departamento, IEPIProdutosEstoqueDAL produtosEstoque, IEPIPedidosDAL pedidos,
            IEPIVinculoDAL vinculo, IMailService mail)
        {
            _compras = compras;
            _pedidoAprovado = pedidoAprovado;
            _produtos = produtos;
            _tamanhos = tamanhos;
            _status = status;
            _usuario = usuario;
            _fornecedor = fornecedor;
            _contrato = contrato;
            _departamento = departamento;
            _produtosEstoque = produtosEstoque;
            _pedidos = pedidos;
            _vinculo = vinculo;
            _mail = mail;
        }

        public async Task<ComprasDTO> getCompra(int Id)
        {
            try
            {
                var localizaCompra = await _compras.getCompra(Id);

                if (localizaCompra != null)
                {
                    ComprasDTO compra = new ComprasDTO();
                    List<ComprasProdutosDTO> compraProdutos = new List<ComprasProdutosDTO>();

                    foreach (var item in localizaCompra.idPedidosAprovados)
                    {
                        var localizaProdutoAprovado = await _pedidoAprovado.getProdutoAprovado(item.idPedidosAprovados, "S");
                        var localizaProduto = await _produtos.localizaProduto(localizaProdutoAprovado.idProduto);
                        var tamanho = await _tamanhos.localizaTamanho(localizaProdutoAprovado.idTamanho);

                        if (tamanho != null)
                        {
                            compraProdutos.Add(new ComprasProdutosDTO
                            {
                                idPedido = localizaProdutoAprovado.idPedido,
                                idProduto = localizaProdutoAprovado.idProduto,
                                nomeProduto = localizaProduto.nome,
                                idProdutoAprovado = localizaProdutoAprovado.id,
                                quantidade = localizaProdutoAprovado.quantidade,
                                preco = localizaProduto.preco,
                                idTamanho = tamanho.id,
                                tamanho = tamanho.tamanho
                            });
                        }
                        else 
                        {
                            compraProdutos.Add(new ComprasProdutosDTO
                            {
                                idPedido = localizaProdutoAprovado.idPedido,
                                idProduto = localizaProdutoAprovado.idProduto,
                                nomeProduto = localizaProduto.nome,
                                idProdutoAprovado = localizaProdutoAprovado.id,
                                quantidade = localizaProdutoAprovado.quantidade,
                                preco = localizaProduto.preco,
                                idTamanho = 0,
                                tamanho = "Tamanho Único"
                            });
                        }                        
                    }

                    var nomeStatus = await _status.getStatus(localizaCompra.status);
                    var nomeEmp = await _usuario.GetEmp(localizaCompra.idUsuario);
                    var localizaFornecedor = await _fornecedor.getFornecedor(localizaCompra.idFornecedor);

                    compra = new ComprasDTO
                    {
                        idCompra = localizaCompra.id,
                        produtosAprovados = compraProdutos,
                        dataCadastraCompra = localizaCompra.dataCadastroCompra,
                        dataFinalizacaoCompra = localizaCompra.dataFinalizacaoCompra,
                        valorTotalCompra = localizaCompra.valorTotalCompra,
                        idStatus = nomeStatus.id,
                        status = nomeStatus.nome,
                        idUsuario = nomeEmp.id,
                        usuario = nomeEmp.nome,
                        idFornecedor = localizaFornecedor.id,
                        fornecedor = localizaFornecedor.razaoSocial
                    };

                    return compra;
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

        public async Task<IList<ComprasDTO>> getTodasCompras()
        {
            try
            {
                var localizaCompras = await _compras.getTodasCompras();

                if (localizaCompras != null)
                {
                    List<ComprasDTO> compra = new List<ComprasDTO>();
                    EPIStatusDTO nomeStatus = new EPIStatusDTO();

                    foreach (var item in localizaCompras)
                    {
                        List<ComprasProdutosDTO> compraProdutos = new List<ComprasProdutosDTO>();

                        foreach (var pedidosAprovados in item.idPedidosAprovados)
                        {
                            var localizaProdutoAprovado = await _pedidoAprovado.getProdutoAprovado(pedidosAprovados.idPedidosAprovados, "S");
                            var localizaProduto = await _produtos.localizaProduto(localizaProdutoAprovado.idProduto);
                            var tamanho = await _tamanhos.localizaTamanho(localizaProdutoAprovado.idTamanho);

                            if (tamanho != null)
                            {
                                compraProdutos.Add(new ComprasProdutosDTO
                                {
                                    idPedido = localizaProdutoAprovado.idPedido,
                                    idProduto = localizaProdutoAprovado.idProduto,
                                    nomeProduto = localizaProduto.nome,
                                    idProdutoAprovado = localizaProdutoAprovado.id,
                                    quantidade = localizaProdutoAprovado.quantidade,
                                    preco = localizaProduto.preco,
                                    idTamanho = tamanho.id,
                                    tamanho = tamanho.tamanho
                                });
                            }
                            else
                            {
                                compraProdutos.Add(new ComprasProdutosDTO
                                {
                                    idPedido = localizaProdutoAprovado.idPedido,
                                    idProduto = localizaProdutoAprovado.idProduto,
                                    nomeProduto = localizaProduto.nome,
                                    idProdutoAprovado = localizaProdutoAprovado.id,
                                    quantidade = localizaProdutoAprovado.quantidade,
                                    preco = localizaProduto.preco,
                                    idTamanho = 0,
                                    tamanho = "Tamanho Único"
                                });
                            }
                        }

                        nomeStatus = await _status.getStatus(item.status);
                        var nomeEmp = await _usuario.GetEmp(item.idUsuario);
                        var localizaFornecedor = await _fornecedor.getFornecedor(item.idFornecedor);

                        compra.Add(new ComprasDTO
                        {
                            idCompra = item.id,
                            produtosAprovados = compraProdutos,
                            dataCadastraCompra = item.dataCadastroCompra,
                            dataFinalizacaoCompra = item.dataFinalizacaoCompra,
                            valorTotalCompra = item.valorTotalCompra,
                            idStatus = nomeStatus.id,
                            status = nomeStatus.nome,
                            idUsuario = nomeEmp.id,
                            usuario = nomeEmp.nome,
                            idFornecedor = localizaFornecedor.id,
                            fornecedor = localizaFornecedor.razaoSocial
                        });
                    }

                    return compra;
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

        public async Task<IList<ComprasDTO>> getCompras(string status)
        {
            try
            {
                var localizaCompras = await _compras.getCompras(status);

                if (localizaCompras != null)
                {
                    List<ComprasDTO> compra = new List<ComprasDTO>();
                    EPIStatusDTO nomeStatus = new EPIStatusDTO();

                    foreach (var item in localizaCompras)
                    {                        
                        List<ComprasProdutosDTO> compraProdutos = new List<ComprasProdutosDTO>();                        

                        foreach (var pedidosAprovados in item.idPedidosAprovados)
                        {
                            var localizaProdutoAprovado = await _pedidoAprovado.getProdutoAprovado(pedidosAprovados.idPedidosAprovados, "S");
                            var localizaProduto = await _produtos.localizaProduto(localizaProdutoAprovado.idProduto);
                            var tamanho = await _tamanhos.localizaTamanho(localizaProdutoAprovado.idTamanho);

                            if (tamanho != null)
                            {
                                compraProdutos.Add(new ComprasProdutosDTO
                                {
                                    idPedido = localizaProdutoAprovado.idPedido,
                                    idProduto = localizaProdutoAprovado.idProduto,
                                    nomeProduto = localizaProduto.nome,
                                    idProdutoAprovado = localizaProdutoAprovado.id,
                                    quantidade = localizaProdutoAprovado.quantidade,
                                    preco = localizaProduto.preco,
                                    idTamanho = tamanho.id,
                                    tamanho = tamanho.tamanho
                                });
                            }
                            else
                            {
                                compraProdutos.Add(new ComprasProdutosDTO
                                {
                                    idPedido = localizaProdutoAprovado.idPedido,
                                    idProduto = localizaProdutoAprovado.idProduto,
                                    nomeProduto = localizaProduto.nome,
                                    idProdutoAprovado = localizaProdutoAprovado.id,
                                    quantidade = localizaProdutoAprovado.quantidade,
                                    preco = localizaProduto.preco,
                                    idTamanho = 0,
                                    tamanho = "Tamanho Único"
                                });
                            }                            
                        }

                        nomeStatus = await _status.getStatus(item.status);
                        var nomeEmp = await _usuario.GetEmp(item.idUsuario);
                        var localizaFornecedor = await _fornecedor.getFornecedor(item.idFornecedor);

                        compra.Add(new ComprasDTO
                        {
                            idCompra = item.id,
                            produtosAprovados = compraProdutos,
                            dataCadastraCompra = item.dataCadastroCompra,
                            dataFinalizacaoCompra = item.dataFinalizacaoCompra,
                            valorTotalCompra = item.valorTotalCompra,
                            idStatus = nomeStatus.id,
                            status = nomeStatus.nome,
                            idUsuario = nomeEmp.id,
                            usuario = nomeEmp.nome,
                            idFornecedor = localizaFornecedor.id,
                            fornecedor = localizaFornecedor.razaoSocial
                        });
                    }

                    return compra;
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

        public async Task<EPIComprasDTO> efetuarCompra(EPIComprasDTO compra)
        {
            try
            {
                var localizaCompra = await _compras.getCompra(compra.id);

                if (localizaCompra != null)
                {
                    EmailRequestDTO email = new EmailRequestDTO();
                    ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                    List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();

                    var idPedido = string.Empty;

                    var getEmp = await _usuario.GetEmp(localizaCompra.idUsuario);
                    var getEmail = await _usuario.getEmail(getEmp.id);

                    if (getEmail != null || !getEmail.Equals(0))
                    {
                        var localizaContrato = await _contrato.getEmpContrato(localizaCompra.idUsuario);

                        if (localizaContrato != null || !localizaContrato.Equals(0))
                        {
                            var localizaDepartamento = await _departamento.getDepartamento(localizaContrato.id_departamento);
                            string numeroPedidos = string.Empty;

                            foreach (var compras in localizaCompra.idPedidosAprovados)
                            {
                                var localizaPedidoAprovado = await _pedidoAprovado.getProdutoAprovado(compras.idPedidosAprovados, "S");
                                var localizaProduto = await _produtos.localizaProduto(localizaPedidoAprovado.idProduto);
                                var verificaTamanho = await _tamanhos.localizaTamanho(localizaPedidoAprovado.idTamanho);
                                var nomeStatus = await _status.getStatus(14);

                                string tamanho = "0";

                                if (verificaTamanho != null)
                                {
                                    tamanho = localizaPedidoAprovado.idTamanho.ToString();
                                }

                                var localizaEstoque = await _produtosEstoque.getProdutoEstoqueTamanho(localizaProduto.id, Int32.Parse(tamanho));

                                localizaEstoque.quantidade += localizaPedidoAprovado.quantidade;

                                var localizaPedido = await _pedidos.getPedido(localizaPedidoAprovado.idPedido);

                                List<Produtos> atualizarStatusProdutos = new List<Produtos>();

                                if (localizaPedido == null)
                                {
                                    idPedido = "0";
                                }
                                else 
                                {
                                    idPedido = localizaPedido.id.ToString();

                                    foreach (var item in localizaPedido.produtos)
                                    {
                                        if (localizaEstoque.idProduto == item.id && localizaEstoque.idTamanho == item.tamanho)
                                        {
                                            atualizarStatusProdutos.Add(new Produtos
                                            {
                                                id = item.id,
                                                nome = item.nome,
                                                quantidade = item.quantidade,
                                                status = 7,
                                                tamanho = item.tamanho,
                                            });

                                            EPIVinculoDTO liberarVinculo = new EPIVinculoDTO();

                                            liberarVinculo.idUsuario = localizaPedido.idUsuario;
                                            liberarVinculo.idItem = item.id;
                                            liberarVinculo.idTamanho = item.tamanho;
                                            liberarVinculo.idPedido = localizaPedido.id;
                                            liberarVinculo.dataVinculo = DateTime.MinValue;
                                            liberarVinculo.status = 7;
                                            liberarVinculo.dataDevolucao = DateTime.MinValue;
                                            liberarVinculo.validade = DateTime.MinValue;

                                            await _vinculo.insereVinculo(liberarVinculo);
                                        }
                                        else
                                        {
                                            atualizarStatusProdutos.Add(new Produtos
                                            {
                                                id = item.id,
                                                nome = item.nome,
                                                quantidade = item.quantidade,
                                                status = item.status,
                                                tamanho = item.tamanho,
                                            });
                                        }
                                    }

                                    int contador = 0;

                                    foreach (var item in atualizarStatusProdutos)
                                    {
                                        if (item.status == 3 || item.status == 10)
                                        {
                                            contador++;
                                        }
                                    }

                                    if (contador == atualizarStatusProdutos.Count)
                                    {
                                        if (localizaPedido != null)
                                        {
                                            localizaPedido.status = 10;
                                        }
                                    }

                                    localizaPedido.produtos = atualizarStatusProdutos;

                                    await _pedidos.Update(localizaPedido);
                                }                                

                                localizaPedidoAprovado.liberadoVinculo = "S";

                                await _pedidoAprovado.Update(localizaPedidoAprovado);

                                var insereEstoque = await _produtosEstoque.Update(localizaEstoque);

                                if (tamanho.Equals(0))
                                {
                                    conteudoEmails.Add(new ConteudoEmailDTO
                                    {
                                        nome = localizaProduto.nome,
                                        tamanho = "Tamanho Único",
                                        status = nomeStatus.nome,
                                        quantidade = localizaPedidoAprovado.quantidade
                                    });
                                }
                                else
                                {
                                    conteudoEmails.Add(new ConteudoEmailDTO
                                    {
                                        nome = localizaProduto.nome,
                                        tamanho = tamanho.ToString(),
                                        status = nomeStatus.nome,
                                        quantidade = localizaPedidoAprovado.quantidade
                                    });
                                }

                                numeroPedidos += numeroPedidos + "," +localizaPedidoAprovado.id;
                            }

                            localizaCompra.status = 7;

                            var atualizaCompra = await _compras.Update(localizaCompra);

                            if (atualizaCompra != null)
                            {
                                conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                                {
                                    idPedido = idPedido,
                                    nomeColaborador = getEmp.nome,
                                    departamento = localizaDepartamento.titulo
                                };

                                email.EmailDe = getEmail.valor;
                                email.EmailPara = "fabiana.lie@reisoffice.com.br";
                                email.ConteudoColaborador = conteudoEmailColaborador;
                                email.Conteudo = conteudoEmails;
                                email.Assunto = "Atualização de Pedido";

                                await _mail.SendEmailAsync(email);

                                return localizaCompra;
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

        public async Task<EPIComprasDTO> reprovaCompra(EPIComprasDTO compra)
        {
            try
            {
                var localizaUsuarioReprova = await _usuario.GetEmp(compra.idUsuario);                

                if (localizaUsuarioReprova != null || !localizaUsuarioReprova.Equals(0))
                {
                    EmailRequestDTO email = new EmailRequestDTO();
                    ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                    List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();

                    var getEmail = await _usuario.getEmail(localizaUsuarioReprova.id);

                    if (getEmail != null || !getEmail.Equals(0))
                    {
                        var localizaContrato = await _contrato.getContrato(localizaUsuarioReprova.id);

                        if (localizaContrato != null || !localizaContrato.Equals(0))
                        {
                            if (compra != null)
                            {
                                var localizaCompra = await _compras.getCompra(compra.id);
                                RHEmpContatoDTO usuarioPedidoEmail = new RHEmpContatoDTO();

                                foreach (var produto in localizaCompra.idPedidosAprovados)
                                {
                                    List<Produtos> produtos = new List<Produtos>();

                                    var localizaProdutoAprovado = await _pedidoAprovado.getProdutoAprovado(produto.idPedidosAprovados, "S");
                                    var localizaPedido = await _pedidos.getPedido(localizaProdutoAprovado.idPedido);

                                    localizaProdutoAprovado.enviadoCompra = "R";

                                    await _pedidoAprovado.Update(localizaProdutoAprovado);

                                    if (localizaPedido == null)
                                    {
                                        continue;
                                    }

                                    var usuarioPedido = await _usuario.GetEmp(localizaPedido.idUsuario);
                                    usuarioPedidoEmail = await _usuario.getEmail(localizaPedido.idUsuario);
                                    var usuarioPedidoContrato = await _contrato.getEmpContrato(localizaPedido.idUsuario);
                                    var usuarioPedidoDepartamento = await _departamento.getDepartamento(usuarioPedidoContrato.id_departamento);

                                    foreach (var pedidoProduto in localizaPedido.produtos)
                                    {
                                        if (pedidoProduto.id == localizaProdutoAprovado.idProduto && pedidoProduto.tamanho == localizaProdutoAprovado.idTamanho &&
                                            pedidoProduto.quantidade == localizaProdutoAprovado.quantidade)
                                        {
                                            var nomeTamanho = await _tamanhos.localizaTamanho(pedidoProduto.tamanho);
                                            var nomeStatus = await _status.getStatus(3);

                                            conteudoEmails.Add(new ConteudoEmailDTO
                                            {
                                                nome = pedidoProduto.nome,
                                                tamanho = nomeTamanho.tamanho,
                                                status = nomeStatus.nome,
                                                quantidade = pedidoProduto.quantidade
                                            });

                                            produtos.Add(new Produtos
                                            {
                                                id = pedidoProduto.id,
                                                nome = pedidoProduto.nome,
                                                quantidade = pedidoProduto.quantidade,
                                                status = 3,
                                                tamanho = pedidoProduto.tamanho
                                            });
                                        }
                                        else
                                        {
                                            produtos.Add(new Produtos
                                            {
                                                id = pedidoProduto.id,
                                                nome = pedidoProduto.nome,
                                                quantidade = pedidoProduto.quantidade,
                                                status = pedidoProduto.status,
                                                tamanho = pedidoProduto.tamanho
                                            });
                                        }
                                    }

                                    int contador = 0;

                                    foreach (var verifica in produtos)
                                    {
                                        if (verifica.status == 2 || verifica.status == 3 || verifica.status == 7)
                                        {
                                            contador++;
                                        }
                                    }

                                    if (contador == produtos.Count)
                                    {
                                        localizaPedido.status = 10;
                                    }
                                    else
                                    {
                                        localizaPedido.status = localizaPedido.status;
                                    }

                                    localizaPedido.produtos = produtos;

                                    await _pedidos.Update(localizaPedido);

                                    conteudoEmailColaborador = new ConteudoEmailColaboradorDTO
                                    {
                                        idPedido = localizaPedido.id.ToString(),
                                        nomeColaborador = usuarioPedido.nome,
                                        departamento = usuarioPedidoDepartamento.titulo
                                    };
                                }

                                localizaCompra.status = 3;
                                localizaCompra.idUsuario = localizaCompra.idUsuario;
                                localizaCompra.dataFinalizacaoCompra = DateTime.Now;

                                var atualizaCompra = await _compras.Update(localizaCompra);

                                if (atualizaCompra != null)
                                {
                                    email.EmailDe = getEmail.valor;
                                    email.EmailPara = $"fabiana.lie@reisoffice.com.br, {usuarioPedidoEmail.valor}";
                                    email.ConteudoColaborador = conteudoEmailColaborador;
                                    email.Conteudo = conteudoEmails;
                                    email.Assunto = "Produto de pedido de EPI reprovado";

                                    await _mail.SendEmailAsync(email);

                                    return atualizaCompra;
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
