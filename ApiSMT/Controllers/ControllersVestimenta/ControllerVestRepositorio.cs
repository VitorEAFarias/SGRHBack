using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.DTO;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Vestimenta.DTO.FromBody;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;
using Utilitarios.Utilitários.PDF;
using Utilitarios.Utilitários.email;
using Vestimenta.BLL.VestRepositorio;
using Vestimenta.BLL.VestVestimenta;
using Vestimenta.BLL.VestCompras;
using Vestimenta.BLL.VestStatus;
using RH.BLL.RHCargos;
using RH.BLL.RHUsuarios;
using Vestimenta.BLL.VestPedidos;
using RH.BLL.RHContratos;
using RH.BLL.RHDepartamentos;
using RH.DTO;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Controller VestRepositorioController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerVestRepositorio : ControllerBase
    {
        private IConverter _converter;
        private readonly IVestRepositorioBLL _repositorio;
        private readonly IVestVestimentaBLL _vestimenta;
        private readonly IVestComprasBLL _compras;
        private readonly IVestStatusBLL _status;
        private readonly IMailService _mail;
        private readonly IRHCargosBLL _cargo;
        private readonly IRHConUserBLL _usuario;
        private readonly IVestPedidosBLL _pedidos;
        private readonly IRHEmpContratosBLL _contrato;
        private readonly IRHDepartamentosBLL _departamento;

        /// <summary>
        /// Construtor VestRepositorioController
        /// </summary>
        /// <param name="repositorio"></param>
        /// <param name="vestimenta"></param>
        /// <param name="compras"></param>
        /// <param name="status"></param>
        /// <param name="mail"></param>
        /// <param name="usuario"></param>
        /// <param name="contrato"></param>
        /// <param name="departamento"></param>
        /// <param name="pedidos"></param>
        /// <param name="converter"></param>
        /// <param name="cargo"></param>
        public ControllerVestRepositorio(IConverter converter, IVestRepositorioBLL repositorio, IVestVestimentaBLL vestimenta, IVestComprasBLL compras, IVestStatusBLL status,
            IMailService mail, IRHConUserBLL usuario, IRHEmpContratosBLL contrato, IRHDepartamentosBLL departamento, IVestPedidosBLL pedidos, IRHCargosBLL cargo)
        {
            _converter = converter;
            _repositorio = repositorio;
            _vestimenta = vestimenta;
            _compras = compras;
            _pedidos = pedidos;
            _status = status;
            _mail = mail;
            _cargo = cargo;
            _usuario = usuario;
            _contrato = contrato;
            _departamento = departamento;
        }

        /// <summary>
        /// Relatório de itens a serem entregues
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("relatorio/{idCompra}")]
        public async Task<IActionResult> relatorioPDF(int idCompra)
        {
            try
            {
                var localizaCompra = await _compras.getCompra(idCompra);
                VestRepositorioDTO localizaRepositorio = new VestRepositorioDTO();
                TamanhosRam localizaVestimenta = new TamanhosRam();
                CompraDTO localizaPedido = new CompraDTO();
                List<VestRelatorioVestimentasDTO> relatorio = new List<VestRelatorioVestimentasDTO>();

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
                var localizaCargo = await _cargo.getCargo(localizaContrato.id_cargo);

                foreach (var repositorio in localizaCompra.itensRepositorio)
                {
                    foreach (var idRepositorio in repositorio.idRepositorio)
                    {
                        if (repositorio.idRepositorio.Count() > 1)
                        {
                            localizaRepositorio = await _repositorio.getRepositorio(idRepositorio);                           

                            if (!localizaRepositorio.idPedido.Equals(0))
                            {
                                localizaPedido = await _pedidos.getPedido(localizaRepositorio.idPedido);

                                foreach (var item in localizaPedido.pedido)
                                {
                                    localizaVestimenta = await _vestimenta.getVestimenta(item.id);

                                    if (item.id == localizaRepositorio.idItem && item.tamanho == localizaRepositorio.tamanho)
                                    {
                                        relatorio.Add(new VestRelatorioVestimentasDTO
                                        {
                                            numeroPedido = localizaPedido.id,
                                            dataPedido = localizaPedido.dataPedido,
                                            colaborador = localizaColaborador.nome,
                                            departamento = localizaDepartamento.titulo,
                                            vestimenta = localizaVestimenta.nome,
                                            tamanho = localizaRepositorio.tamanho,
                                            quantidade = item.quantidade - localizaRepositorio.quantidade
                                        });
                                    }
                                }
                            }                            
                        }
                        else
                        {
                            localizaRepositorio = await _repositorio.getRepositorio(idRepositorio);                            

                            if (!localizaRepositorio.idPedido.Equals(0))
                            {
                                localizaPedido = await _pedidos.getPedido(localizaRepositorio.idPedido);
                                localizaVestimenta = await _vestimenta.getVestimenta(repositorio.idItem);

                                relatorio.Add(new VestRelatorioVestimentasDTO
                                {
                                    numeroPedido = localizaPedido.id,
                                    dataPedido = localizaPedido.dataPedido,
                                    colaborador = localizaColaborador.nome,
                                    departamento = localizaDepartamento.titulo,
                                    vestimenta = localizaVestimenta.nome,
                                    tamanho = localizaRepositorio.tamanho,
                                    quantidade = localizaRepositorio.quantidade
                                });
                            }
                        }
                    }
                }

                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = RelatórioPDF.GetHTMLString(relatorio),
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retorna um item selecionado
        /// </summary>
        /// <param name="idRepositorio"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> getRepositorio(int idRepositorio)
        {
            try
            {
                var checkRepositorio = await _repositorio.getRepositorio(idRepositorio);

                if (checkRepositorio != null)
                {
                    return Ok(new { message = "Item encontrado", result = true, repositorio = checkRepositorio });
                }
                else
                {
                    return BadRequest(new { message = "Item não encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona todos os itens que serão enviados para compras
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("repositorio/{status}")]
        public async Task<IActionResult> getRepositorioStatus(string status)
        {
            try
            {
                var repositorio = await _repositorio.getRepositorioStatus(status);
                List<VestSortListDTO> list = new List<VestSortListDTO>();

                if (repositorio != null)
                {
                    foreach (var item in repositorio)
                    {
                        var checkNome = await _vestimenta.getVestimenta(item.idItem);
                        var precoTotal = checkNome.preco * item.quantidade;

                        list.Add(new VestSortListDTO
                        {
                            id = item.id,
                            idItem = item.idItem,
                            nome = checkNome.nome,
                            preco = checkNome.preco,
                            precoTotal = precoTotal,
                            idPedido = item.idPedido,
                            tamanho = item.tamanho,
                            quantidade = item.quantidade
                        });
                    }

                    list = list.OrderBy(n => n.nome).ThenBy(t => t.tamanho).ToList();

                    return Ok(new { message = "Produtos encontrados", result = true, lista = list });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum item encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Insere item avulso para compras
        /// </summary>
        /// <param name="repositorioAvulso"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("avulso")]
        public async Task<IActionResult> avulso([FromBody] IList<VestRepositorioDTO> repositorioAvulso)
        {
            try
            {
                if (repositorioAvulso != null || !repositorioAvulso.Equals(0))
                {
                    foreach (var item in repositorioAvulso)
                    {
                        await _repositorio.Insert(item);
                    }                    

                    return Ok(new { message = "Item avulso inserido com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum item avulso enviado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Envia itens para compra
        /// </summary>
        /// <param name="compras"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> enviarParaCompra([FromBody] VestComprasDTO compras)
        {
            try
            {
                if (compras != null)
                {
                    compras.dataCompra = DateTime.Now;

                    var insereCompra = await _compras.Insert(compras);

                    List<ConteudoEmailDTO> conteudoEmails = new List<ConteudoEmailDTO>();
                    ConteudoEmailColaboradorDTO conteudoEmailColaborador = new ConteudoEmailColaboradorDTO();
                    EmailRequestDTO email = new EmailRequestDTO();
                    RHEmpContatoDTO empContato = new RHEmpContatoDTO();
                    var checkUsuario = await _usuario.GetEmp(compras.idUsuario);

                    if (insereCompra != null)
                    {
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

                        return Ok(new { message = "Itens Enviados para compra com sucesso!!!", result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Nenhum item selecionado!!!", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
