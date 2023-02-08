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

        public EPIPedidosAprovadosBLL(IEPIPedidosAprovadosDAL pedidosAprovados, IRHConUserDAL usuario, IEPITamanhosDAL tamanho, IEPIPedidosDAL pedidos, IEPIProdutosDAL produtos,
            IEPIStatusDAL status, IEPIProdutosEstoqueDAL estoque, IMailService mail, IEPIComprasDAL compras, IEPIVinculoDAL vinculo)
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
        }

        public async Task<IList<PedidosAprovadosDTO>> getProdutosAprovados(string statusCompra, string statusVinculo)
        {
            try
            {
                var localizaProdutosAprovados = await _pedidosAprovados.getProdutosAprovados(statusCompra, statusVinculo);

                List<PedidosAprovadosDTO> produtos = new List<PedidosAprovadosDTO>();

                foreach (var item in localizaProdutosAprovados)
                {
                    var localizaProduto = _produtos.localizaProduto(item.idProduto).Result;
                    var localizaTamanho = _tamanho.localizaTamanho(item.idTamanho).Result;
                    var localizaPedido = _pedidos.getPedido(item.idPedido).Result;

                    if (localizaPedido != null)
                    {
                        var localizaUsuario = _usuario.GetEmp(localizaPedido.idUsuario).Result;
                        var localizaEstoque = _estoque.getProdutoExistente(item.idProduto).Result;

                        produtos.Add(new PedidosAprovadosDTO
                        {
                            idProdutoAprovado = item.id,
                            enviadoCompra = item.enviadoCompra,
                            idPedido = item.idPedido,
                            idProduto = localizaProduto.id,
                            nome = localizaProduto.nome,
                            idTamanho = localizaTamanho.id,
                            tamanho = localizaTamanho.tamanho,
                            quantidade = item.quantidade,
                            idUsuario = localizaUsuario.id,
                            usuario = localizaUsuario.nome,
                            dataPedido = localizaPedido.dataPedido,
                            estoque = localizaEstoque.quantidade,
                            liberadoVinculo = item.liberadoVinculo
                        });
                    }
                    else
                    {
                        var localizaEstoque = _estoque.getProdutoExistente(item.idProduto).Result;

                        produtos.Add(new PedidosAprovadosDTO
                        {
                            idProdutoAprovado = item.id,
                            enviadoCompra = item.enviadoCompra,
                            idPedido = item.idPedido,
                            idProduto = localizaProduto.id,
                            nome = localizaProduto.nome,
                            idTamanho = localizaTamanho.id,
                            tamanho = localizaTamanho.tamanho,
                            quantidade = item.quantidade,
                            idUsuario = 0,
                            usuario = string.Empty,
                            dataPedido = DateTime.MinValue,
                            estoque = localizaEstoque.quantidade,
                            liberadoVinculo = item.liberadoVinculo
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

                if (localizaUsuario != null)
                {
                    if (enviaCompras != null)
                    {
                        EmailRequestDTO email = new EmailRequestDTO();
                        ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                        List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();
                        List<PedidosAprovados> pedidosAprovados = new List<PedidosAprovados>();
                        EPIComprasDTO compras = new EPIComprasDTO();

                        decimal valorTotalCompra = 0;
                        string tamanho = string.Empty;

                        foreach (var produto in enviaCompras)
                        {
                            pedidosAprovados.Add(new PedidosAprovados
                            {
                                idPedidosAprovados = produto.id
                            });

                            var localizaPedido = await _pedidos.getPedido(produto.idPedido);
                            var localizaProduto = await _produtos.localizaProduto(produto.idProduto);
                            var checkStatusItem = await _status.getStatus(4);

                            if (localizaPedido != null)
                            {
                                var getEmail = await _usuario.getEmail(localizaPedido.idUsuario);

                                EPITamanhosDTO localizaTamanho = new EPITamanhosDTO();

                                foreach (var item in localizaPedido.produtos)
                                {
                                    var localizaProdutoEstoque = await _estoque.getProdutoEstoqueTamanho(item.id, item.tamanho);

                                    if (localizaProdutoEstoque != null || !localizaProdutoEstoque.Equals(0))
                                    {
                                        localizaTamanho = await _tamanho.localizaTamanho(item.tamanho);
                                    }
                                }

                                if (localizaTamanho != null || !localizaTamanho.Equals(0))
                                {
                                    tamanho = "";
                                }
                                else
                                {
                                    tamanho = localizaTamanho.tamanho;
                                }

                                conteudoEmails.Add(new ConteudoEmailDTO
                                {
                                    nome = localizaProduto.nome,
                                    tamanho = tamanho,
                                    status = checkStatusItem.nome,
                                    quantidade = produto.quantidade
                                });

                                email.EmailDe = getEmail.valor;
                                email.EmailPara = "fabiana.lie@reisoffice.com.br";
                                email.ConteudoColaborador = conteudoEmailColaborador;
                                email.Conteudo = conteudoEmails;
                                email.Assunto = "EPI enviado para compras";

                                await _mail.SendEmailAsync(email);

                                produto.enviadoCompra = "S";
                            }
                            else
                            {
                                produto.enviadoCompra = "S";
                            }

                            await _pedidosAprovados.Update(produto);
                        }

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
                    foreach (var item in produtosAprovados)
                    {
                        var localizaPedido = await _pedidos.getPedido(item.idPedido);
                        var localizaProduto = await _produtos.localizaProduto(item.idProduto);

                        EPIVinculoDTO novoVinculo = new EPIVinculoDTO();

                        novoVinculo.idUsuario = localizaPedido.idUsuario;
                        novoVinculo.idItem = item.idProduto;
                        novoVinculo.dataVinculo = DateTime.Now;
                        novoVinculo.status = 13;
                        novoVinculo.dataDevolucao = DateTime.MinValue;
                        novoVinculo.validade = DateTime.Now.AddYears(localizaProduto.validadeEmUso);

                        await _vinculo.insereVinculo(novoVinculo);

                        item.liberadoVinculo = "S";

                        await _pedidosAprovados.Update(item);
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
