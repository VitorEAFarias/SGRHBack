using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.BLL;
using System;
using Microsoft.AspNetCore.Authorization;
using ControleEPI.BLL;
using System.Collections.Generic;
using Vestimenta.DTO.DinkPDF;
using DinkToPdf.Contracts;
using DinkToPdf;
using System.IO;
using ApiSMT.Utilitários.PDF;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe de PDF
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerPDFVinculo : ControllerBase
    {
        private IConverter _converter;
        private readonly IDinkPDFBLL _dadosPDF;
        private readonly IRHConUserBLL _usuario;
        private readonly IRHEmpContratosBLL _contrato;
        private readonly IRHDepartamentosBLL _departamento;
        private readonly IRHCargosBLL _cargos;
        private readonly IVestimentaBLL _vestimenta;

        /// <summary>
        /// Construtor VestimentaController
        /// </summary>
        /// <param name="dadosPDF"></param>
        /// <param name="usuario"></param>
        /// <param name="contrato"></param>
        /// <param name="departamento"></param>
        /// <param name="cargos"></param>
        /// <param name="vestimenta"></param>
        /// <param name="converter"></param>
        public ControllerPDFVinculo(IConverter converter, IDinkPDFBLL dadosPDF, IRHConUserBLL usuario, IRHEmpContratosBLL contrato, 
            IRHDepartamentosBLL departamento, IRHCargosBLL cargos, IVestimentaBLL vestimenta)
        {
            _converter = converter;
            _dadosPDF = dadosPDF;
            _departamento = departamento;
            _cargos = cargos;
            _contrato = contrato;
            _usuario = usuario;
            _vestimenta = vestimenta;
        }

        /// <summary>
        /// Lista historico de itens com colaborador e monta PDF
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{idUsuario}")]
        public async Task<ActionResult> getTodosStatus(int idUsuario)
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

                    VestDadosPDFDTO dadosPDF = new VestDadosPDFDTO();
                    List<Historico> dadosVestimentaPDF = new List<Historico>();

                    foreach (var item in historicoItens)
                    {
                        var vestNome = await _vestimenta.getVestimenta(item.idVestimenta);

                        var status = string.Empty;

                        if (item.status.Equals("Y"))
                        {
                            status = "Sim";
                        }
                        else
                        {
                            status = "Não";
                        }                        

                        dadosVestimentaPDF.Add(new Historico { 
                            nomeVestimenta = vestNome.nome,
                            tamanho = item.tamanhoVestVinculo,
                            dataVinculo = item.dataVinculo,
                            dataDesvinculo = item.dataDesvinculo,
                            statusAtual = item.statusAtual,
                            usado = status,
                            quantidade = item.quantidade
                        });
                    }

                    dadosPDF = new VestDadosPDFDTO
                    {
                        nome = nomeEmp.nome,
                        departamento = nomeDep.titulo,
                        cargo = nomeCargo.titulo,
                        vestimentas = dadosVestimentaPDF
                    };

                    var objectSettings = new ObjectSettings
                    {
                        PagesCount = true,
                        HtmlContent = TemplatePDF.GetHTMLString(dadosPDF),
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

                        return Ok(new { message = "PDF Criado com sucesso!!!", result = true, pdf = pdf64 });
                    }
                    else
                    {
                        return BadRequest(new { message = "Erro ao gerar PDF!!!", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Nenhum item vinculado encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
