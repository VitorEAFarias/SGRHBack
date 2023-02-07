﻿using ApiSMT.Utilitários.PDF;
using DinkToPdf;
using DinkToPdf.Contracts;
using RH.DAL.RHCargos;
using RH.DAL.RHContratos;
using RH.DAL.RHDepartamentos;
using RH.DAL.RHUsuarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Utilitarios.Utilitários.PDF;
using Vestimenta.DAL.VestPDF;
using Vestimenta.DAL.VestVestimenta;

namespace Vestimenta.BLL.VestPDF
{
    public class VestDinkPDFBLL : IVestDinkPDFBLL
    {
        private IConverter _converter;
        private readonly IVestDinkPDFDAL _dadosPDF;
        private readonly IRHConUserDAL _usuario;
        private readonly IRHEmpContratosDAL _contrato;
        private readonly IRHDepartamentosDAL _departamento;
        private readonly IRHCargosDAL _cargos;
        private readonly IVestVestimentaDAL _vestimenta;

        public VestDinkPDFBLL(IConverter converter, IVestDinkPDFDAL dadosPDF, IRHConUserDAL usuario, IRHEmpContratosDAL contrato, IRHDepartamentosDAL departamento,
            IRHCargosDAL cargos, IVestVestimentaDAL vestimenta)
        {
            _converter = converter;
            _dadosPDF = dadosPDF;
            _departamento = departamento;
            _cargos = cargos;
            _contrato = contrato;
            _usuario = usuario;
            _vestimenta = vestimenta;
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

                        dadosVestimentaPDF.Add(new Historico
                        {
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
    }
}
