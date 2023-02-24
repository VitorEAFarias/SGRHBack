using DinkToPdf.Contracts;
using DinkToPdf;
using RH.DAL.RHCargos;
using RH.DAL.RHContratos;
using RH.DAL.RHDepartamentos;
using RH.DAL.RHUsuarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Utilitarios.Utilitários.PDF;
using ControleEPI.DAL.EPICompras;
using ControleEPI.DAL.EPIHistorico;
using ControleEPI.DAL.EPIPedidos;
using ControleEPI.DAL.EPIPedidosAprovados;
using ControleEPI.DAL.EPIProdutos;
using ControleEPI.DTO;
using ControleEPI.DAL.EPITamanhos;
using ControleEPI.DAL.EPIStatus;

namespace ControleEPI.BLL.EPIHistorico
{
    public class EPIHistoricoBLL : IEPIHistoricoBLL
    {
        private IConverter _converter;
        private readonly IEPIHistoricoDAL _dadosPDF;
        private readonly IRHConUserDAL _usuario;
        private readonly IRHEmpContratosDAL _contrato;
        private readonly IRHDepartamentosDAL _departamento;
        private readonly IRHCargosDAL _cargos;
        private readonly IEPIProdutosDAL _produto;
        private readonly IEPIComprasDAL _compras;
        private readonly IEPIPedidosAprovadosDAL _repositorio;
        private readonly IEPIPedidosDAL _pedidos;
        private readonly IEPITamanhosDAL _tamanho;
        private readonly IEPIStatusDAL _status;

        public EPIHistoricoBLL(IConverter converter, IEPIHistoricoDAL dadosPDF, IRHConUserDAL usuario, IRHEmpContratosDAL contrato, IRHDepartamentosDAL departamento,
            IRHCargosDAL cargos, IEPIProdutosDAL produto, IEPIComprasDAL compras, IEPIPedidosAprovadosDAL repositorio, IEPIPedidosDAL pedidos, IEPITamanhosDAL tamanho,
            IEPIStatusDAL status)
        {
            _converter = converter;
            _dadosPDF = dadosPDF;
            _departamento = departamento;
            _cargos = cargos;
            _contrato = contrato;
            _usuario = usuario;
            _produto = produto;
            _compras = compras;
            _repositorio = repositorio;
            _pedidos = pedidos;
            _tamanho = tamanho;
            _status = status;
        }

        public async Task<string> dadosPDF(int idUsuario)
        {
            try
            {
                var historicoItens = await _dadosPDF.dadosPDF(idUsuario);

                if (historicoItens != null)
                {
                    var globalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Margins = new MarginSettings { Top = 10 },
                        DocumentTitle = "PDF Report",
                        Out = ""//@"C:\\Users\\vitor.alves\\Desktop\\Projects\\TestePDF\\teste.pdf"
                    };

                    var nomeEmp = await _usuario.GetEmp(idUsuario);
                    var contrato = await _contrato.getEmpContrato(idUsuario);
                    var nomeDep = await _departamento.getDepartamento(contrato.id_departamento);
                    var nomeCargo = await _cargos.getCargo(contrato.id_cargo);

                    EPIDadosPDFDTO dadosPDF = new EPIDadosPDFDTO();
                    List<HistoricoEPI> dadosProdutoPDF = new List<HistoricoEPI>();

                    foreach (var item in historicoItens)
                    {
                        var prodNome = await _produto.localizaProduto(item.idItem);

                        var localizaTamanho = await _tamanho.localizaTamanho(item.idTamanho);
                        var localizaStatus = await _status.getStatus(item.status);

                        if (localizaTamanho != null)
                        {
                            dadosProdutoPDF.Add(new HistoricoEPI
                            {
                                nomeProduto = prodNome.nome,
                                tamanho = localizaTamanho.tamanho,
                                dataVinculo = item.dataVinculo,
                                dataDesvinculo = item.dataDevolucao,
                                statusAtual = localizaStatus.nome,
                                validade = item.validade,
                                quantidade = 1
                            });
                        }
                        else
                        {
                            dadosProdutoPDF.Add(new HistoricoEPI
                            {
                                nomeProduto = prodNome.nome,
                                tamanho = "Tamanho Único",
                                dataVinculo = item.dataVinculo,
                                dataDesvinculo = item.dataDevolucao,
                                statusAtual = localizaStatus.nome,
                                validade = item.validade,
                                quantidade = 1
                            });
                        }                        
                    }

                    dadosPDF = new EPIDadosPDFDTO
                    {
                        nome = nomeEmp.nome,
                        departamento = nomeDep.titulo,
                        cargo = nomeCargo.titulo,
                        produtos = dadosProdutoPDF
                    };

                    var objectSettings = new ObjectSettings
                    {
                        PagesCount = true,
                        HtmlContent = EPITemplatePDF.GetHTMLString(dadosPDF),
                        WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "Utilitários", "PDF", "assets", "styles.css") },
                        HeaderSettings = { FontName = "Times New Roman", FontSize = 14, Line = false },
                        FooterSettings = { FontName = "Times New Roman", FontSize = 12, Right = "Page [page] of [toPage]", Line = false, Center = DateTime.Now.ToString() }
                    };

                    var pdf = new HtmlToPdfDocument()
                    {
                        GlobalSettings = globalSettings,
                        Objects = { objectSettings }
                    };

                    var rst = _converter.Convert(pdf);

                    if (rst != null)
                    {
                        string pdf64 = Convert.ToBase64String(rst);

                        return pdf64;
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

        public async Task<string> relatorioCompra(int idCompra)
        {
            try
            {
                var localizaCompra = await _compras.getCompra(idCompra);

                EPIProdutosDTO localizaProduto = new EPIProdutosDTO();
                EPITamanhosDTO localizaTamanho = new EPITamanhosDTO();
                List<EPIRelatorioProdutosDTO> relatorio = new List<EPIRelatorioProdutosDTO>();

                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = "PDF Report",
                    Out = ""//@"C:\\Users\\vitor.alves\\Desktop\\Projects\\TestePDF\\teste.pdf"
                };

                var localizaColaborador = await _usuario.GetEmp(localizaCompra.idUsuario);
                var localizaContrato = await _contrato.getEmpContrato(localizaCompra.idUsuario);
                var localizaDepartamento = await _departamento.getDepartamento(localizaContrato.id_departamento);
                var localizaCargo = await _cargos.getCargo(localizaContrato.id_cargo);

                var tamanho = string.Empty;

                foreach (var repositorio in localizaCompra.idPedidosAprovados)
                {
                    var localizaRepositorio = await _repositorio.getProdutoAprovado(repositorio.idPedidosAprovados, "S");

                    if (!localizaRepositorio.idPedido.Equals(0))
                    {
                        var localizaPedido = await _pedidos.getPedido(localizaRepositorio.idPedido);

                        foreach (var item in localizaPedido.produtos)
                        {
                            localizaProduto = await _produto.localizaProduto(item.id);

                            if (item.id == localizaRepositorio.idProduto && item.tamanho == localizaRepositorio.idTamanho)
                            {
                                localizaTamanho = await _tamanho.localizaTamanho(localizaRepositorio.idTamanho);

                                if (localizaTamanho != null)
                                {
                                    tamanho = localizaTamanho.tamanho;
                                }
                                else
                                {
                                    tamanho = "Tamanho Único";
                                }

                                relatorio.Add(new EPIRelatorioProdutosDTO
                                {
                                    numeroPedido = localizaPedido.id,
                                    dataPedido = localizaPedido.dataPedido,
                                    colaborador = localizaColaborador.nome,
                                    departamento = localizaDepartamento.titulo,
                                    produto = localizaProduto.nome,
                                    tamanho = tamanho,
                                    quantidade = localizaRepositorio.quantidade
                                });
                            }
                        }
                    }
                    else
                    {
                        localizaProduto = await _produto.localizaProduto(localizaRepositorio.idProduto);
                        localizaTamanho = await _tamanho.localizaTamanho(localizaRepositorio.idTamanho);

                        if (localizaTamanho != null)
                        {
                            tamanho = localizaTamanho.tamanho;
                        }
                        else
                        {
                            tamanho = "Tamanho Único";
                        }

                        relatorio.Add(new EPIRelatorioProdutosDTO
                        {
                            numeroPedido = 0,
                            dataPedido = DateTime.MinValue,
                            colaborador = localizaColaborador.nome,
                            departamento = localizaDepartamento.titulo,
                            produto = localizaProduto.nome,
                            tamanho = tamanho,
                            quantidade = localizaRepositorio.quantidade
                        });
                        
                    }                    
                }

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = EPIRelatorioPDF.GetHTMLString(relatorio),
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "Utilitários", "PDF", "assets", "styles.css") },
                    HeaderSettings = { FontName = "Times New Roman", FontSize = 14, Line = false },
                    FooterSettings = { FontName = "Times New Roman", FontSize = 12, Right = "Page [page] of [toPage]", Line = false, Center = DateTime.Now.ToString() }
                };

                var pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                var rst = _converter.Convert(pdf);

                if (rst != null)
                {
                    string pdf64 = Convert.ToBase64String(rst);

                    if (pdf64 != null)
                    {
                        return pdf64;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null; ;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
