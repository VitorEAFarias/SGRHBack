using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ControleEPI.DTO;
using System;
using Microsoft.AspNetCore.Authorization;
using ControleEPI.DTO.E_Mail;
using ControleEPI.DTO.email;
using ApiSMT.Utilitários;
using ControleEPI.BLL.EPIMotivos;
using ControleEPI.BLL.EPIPedidosAprovados;
using ControleEPI.BLL.EPIPedidos;
using ControleEPI.BLL.EPIProdutosEstoque;
using ControleEPI.BLL.EPIStatus;
using ControleEPI.BLL.EPITamanhos;
using ControleEPI.BLL.RHUsuarios;
using ControleEPI.BLL.RHDepartamentos;
using ControleEPI.BLL.RHContratos;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe de manipulação de pedidos de epi's
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerPedidos : ControllerBase
    {
        private readonly IEPIPedidosBLL _pedidos;
        private readonly IEPIStatusBLL _status;
        private readonly IEPIMotivosBLL _motivos;
        private readonly IRHConUserBLL _conUser;
        private readonly IEPIProdutosEstoqueBLL _produtosEstoque;
        private readonly IEPIPedidosAprovadosBLL _pedidosAprovados;
        private readonly IRHConUserBLL _usuario;
        private readonly IRHEmpContratosBLL _contrato;
        private readonly IRHDepartamentosBLL _departamento;
        private readonly IMailService _mail;
        private readonly IEPITamanhosBLL _tamanho;

        /// <summary>
        /// Construtor PedidosController
        /// </summary>
        /// <param name="pedidos"></param>
        /// <param name="status"></param>
        /// <param name="motivos"></param>
        /// <param name="conUser"></param>
        /// <param name="produtosEstoque"></param>
        /// <param name="pedidosAprovados"></param>
        /// <param name="usuario"></param>
        /// <param name="contrato"></param>
        /// <param name="departamento"></param>
        /// <param name="mail"></param>
        /// <param name="tamanho"></param>
        public ControllerPedidos(IEPIPedidosBLL pedidos, IEPIStatusBLL status, IEPIMotivosBLL motivos, IRHConUserBLL conUser, IEPIProdutosEstoqueBLL produtosEstoque,
            IEPIPedidosAprovadosBLL pedidosAprovados, IRHConUserBLL usuario, IRHEmpContratosBLL contrato, IRHDepartamentosBLL departamento, IMailService mail,
            IEPITamanhosBLL tamanho)
        {
            _pedidos = pedidos;
            _status = status;
            _motivos = motivos;
            _conUser = conUser;
            _produtosEstoque = produtosEstoque;
            _pedidosAprovados = pedidosAprovados;
            _usuario = usuario;
            _contrato = contrato;
            _departamento = departamento;
            _mail = mail;
            _tamanho = tamanho;
        }

        /// <summary>
        /// insere um pedido a ser feito
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> inserePedido([FromBody] EPIPedidosDTO pedido)
        {
            try
            {
                var checkUsuario = await _usuario.GetEmp(pedido.idUsuario);

                if (checkUsuario != null || !checkUsuario.Equals(0))
                {
                    if (pedido.produtos != null || !pedido.produtos.Equals(0))
                    {
                        string tamanho = string.Empty;

                        EPIEmailRequestDTO email = new EPIEmailRequestDTO();
                        EPIConteudoEmailColaboradorDTO conteudoEmailColaborador = new EPIConteudoEmailColaboradorDTO();
                        List<EPIConteudoEmailDTO> conteudoEmails = new List<EPIConteudoEmailDTO>();

                        pedido.dataPedido = DateTime.Now;
                        pedido.status = 1;

                        var novoPedido = await _pedidos.Insert(pedido);

                        if (novoPedido != null || !novoPedido.Equals(0))
                        {
                            var getEmail = await _usuario.getEmail(checkUsuario.id);

                            if (getEmail != null || !getEmail.Equals(0))
                            {
                                var getContrato = await _contrato.getEmpContrato(pedido.idUsuario);

                                if (getContrato != null && !getContrato.Equals(0))
                                {
                                    var getDepartamento = await _departamento.getDepartamento(getContrato.id_departamento);

                                    foreach (var produto in pedido.produtos)
                                    {
                                        var checkStatusItem = await _status.getStatus(produto.status);
                                        var localizaTamanho = await _tamanho.localizaTamanho(produto.tamanho);

                                        if (localizaTamanho != null || !localizaTamanho.Equals(0))
                                        {
                                            tamanho = "";
                                        }
                                        else
                                        {
                                            tamanho = localizaTamanho.tamanho;
                                        }

                                        conteudoEmails.Add(new EPIConteudoEmailDTO
                                        {
                                            nome = produto.nome,
                                            tamanho = tamanho,
                                            status = checkStatusItem.nome,
                                            quantidade = produto.quantidade
                                        });
                                    }

                                    conteudoEmailColaborador = new EPIConteudoEmailColaboradorDTO
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

                                    return Ok(new { message = "Pedido realizado com sucesso!!!", result = true });
                                }
                                else
                                {
                                    return BadRequest(new { message = "Colaborador '" + checkUsuario.nome + "' tem mais de um contrato ativo ou nenhum, verifique no RH", result = false });
                                }
                            }
                            else
                            {
                                return BadRequest(new { message = "É obrigatório colaborador ter e-mail funcional vinculado, verifique no RH", result = false });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "Erro ao efetuar pedido", result = false });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Nenhum produto selecionado para o pedido", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Verifique se o colaborador esta ativo no portal do RH e se nao tem data de demissao e/ou desligamento atribuida", result = false });
                }                                  
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }        
        }

        /// <summary>
        /// Aprovar pedido
        /// </summary>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("aprovaPedido")]
        public async Task<IActionResult> aprovaPedido([FromBody] EPIPedidosDTO pedido)
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

                        EPIPedidosAprovadosDTO aprovado = new EPIPedidosAprovadosDTO();
                        EPIEmailRequestDTO email = new EPIEmailRequestDTO();
                        EPIConteudoEmailColaboradorDTO conteudoEmailColaborador = new EPIConteudoEmailColaboradorDTO();
                        List<EPIConteudoEmailDTO> conteudoEmails = new List<EPIConteudoEmailDTO>();

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
                                            tamanho = "";
                                        }
                                        else
                                        {
                                            tamanho = localizaTamanho.tamanho;
                                        }

                                        conteudoEmails.Add(new EPIConteudoEmailDTO
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
                                        aprovado.enviadoCompra = "N";

                                        await _pedidosAprovados.Insert(aprovado);
                                    }
                                }

                                conteudoEmailColaborador = new EPIConteudoEmailColaboradorDTO
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

                                return Ok(new { message = "Pedido aprovado com sucesso!!!", result = true });
                            }
                            else
                            {
                                return BadRequest(new { message = "O colaborador '" + checkUsuario.nome + "' tem mais de um contrato ativo ou nenhum, verifique no portal do RH", result = false });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "É necessário ter um email funcional vinculado ao colaborador" });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Pedido não encontrado", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Verifique se o colaborador esta ativo no portal do RH e se nao tem data de demissao e/ou desligamento atribuida", result = false });
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Aprova um produto do pedido
        /// </summary>
        /// <param name="idProduto"></param>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("aprovaProdutoPedido/{idProduto}")]
        public async Task<IActionResult> aprovaProdutoPedido(int idProduto, [FromBody] EPIPedidosDTO pedido)
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

                        EPIPedidosAprovadosDTO aprovados = new EPIPedidosAprovadosDTO();
                        List<Produtos> produtos = new List<Produtos>();
                        EPIEmailRequestDTO email = new EPIEmailRequestDTO();
                        EPIConteudoEmailColaboradorDTO conteudoEmailColaborador = new EPIConteudoEmailColaboradorDTO();
                        List<EPIConteudoEmailDTO> conteudoEmails = new List<EPIConteudoEmailDTO>();

                        var getEmail = await _usuario.getEmail(checkUsuario.id);

                        if (getEmail != null || !getEmail.Equals(0))
                        {
                            var getContrato = await _contrato.getEmpContrato(pedido.idUsuario);

                            if (getContrato != null || !getContrato.Equals(0))
                            {
                                var getDepartamento = await _departamento.getDepartamento(getContrato.id_departamento);

                                foreach (var produto in localizaPedido.produtos)
                                {
                                    if (produto.id == idProduto)
                                    {
                                        var checkStatusItem = await _status.getStatus(produto.status);
                                        var localizaTamanho = await _tamanho.localizaTamanho(produto.tamanho);

                                        if (localizaTamanho != null || !localizaTamanho.Equals(0))
                                        {
                                            tamanho = "";
                                        }
                                        else
                                        {
                                            tamanho = localizaTamanho.tamanho;
                                        }

                                        conteudoEmails.Add(new EPIConteudoEmailDTO
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
                                            status = 2,
                                            tamanho = produto.tamanho
                                        });

                                        aprovados.idProduto = produto.id;
                                        aprovados.idPedido = localizaPedido.id;
                                        aprovados.idTamanho = produto.tamanho;
                                        aprovados.quantidade = produto.quantidade;
                                        aprovados.enviadoCompra = "N";

                                        await _pedidosAprovados.Insert(aprovados);
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

                                conteudoEmailColaborador = new EPIConteudoEmailColaboradorDTO
                                {
                                    idPedido = localizaPedido.id.ToString(),
                                    nomeColaborador = checkUsuario.nome,
                                    departamento = getDepartamento.titulo
                                };

                                int contador = 0;

                                foreach (var item in produtos)
                                {
                                    if (item.status == 3 || item.status == 13)
                                    {
                                        contador++;
                                    }
                                }

                                localizaPedido.produtos = produtos;

                                if (contador == produtos.Count)
                                {
                                    localizaPedido.status = 10;
                                }
                                else
                                {
                                    localizaPedido.status = pedido.status;
                                }

                                email.EmailDe = getEmail.valor;
                                email.EmailPara = "fabiana.lie@reisoffice.com.br";
                                email.ConteudoColaborador = conteudoEmailColaborador;
                                email.Conteudo = conteudoEmails;
                                email.Assunto = "Produto de pedido de EPI aprovado";

                                await _mail.SendEmailAsync(email);

                                await _pedidos.Update(localizaPedido);

                                return Ok(new { message = "Produto aprovado com sucesso", result = true });
                            }
                            else
                            {
                                return BadRequest(new { message = "O colaborador '" + checkUsuario.nome + "' tem mais de um contrato ativo ou nenhum, verifique no portal do RH", result = false });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "É obrigatório colaborador ter e-mail funcional vinculado", result = false });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Pedido não encontrado", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Verifique se o colaborador esta ativo no portal do RH e se nao tem data de demissao e/ou desligamento atribuida", result = false });
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Reprovar produto do pedido
        /// </summary>
        /// <param name="idProduto"></param>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("reprovarProdutoPedido/{idProduto}")]
        public async Task<IActionResult> reprovaProdutoPedido(int idProduto, [FromBody] EPIPedidosDTO pedido)
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

                        List<Produtos> produtos = new List<Produtos>();
                        EPIEmailRequestDTO email = new EPIEmailRequestDTO();
                        EPIConteudoEmailColaboradorDTO conteudoEmailColaborador = new EPIConteudoEmailColaboradorDTO();
                        List<EPIConteudoEmailDTO> conteudoEmails = new List<EPIConteudoEmailDTO>();

                        var getEmail = await _usuario.getEmail(checkUsuario.id);

                        if (getEmail != null || !getEmail.Equals(0))
                        {
                            var getContrato = await _contrato.getEmpContrato(pedido.idUsuario);

                            if (getContrato != null || !getContrato.Equals(0))
                            {
                                var getDepartamento = await _departamento.getDepartamento(getContrato.id_departamento);

                                foreach (var produto in localizaPedido.produtos)
                                {
                                    if (produto.id == idProduto)
                                    {
                                        var checkStatusItem = await _status.getStatus(produto.status);
                                        var localizaTamanho = await _tamanho.localizaTamanho(produto.tamanho);

                                        if (localizaTamanho != null || !localizaTamanho.Equals(0))
                                        {
                                            tamanho = "";
                                        }
                                        else
                                        {
                                            tamanho = localizaTamanho.tamanho;
                                        }

                                        conteudoEmails.Add(new EPIConteudoEmailDTO
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

                                conteudoEmailColaborador = new EPIConteudoEmailColaboradorDTO
                                {
                                    idPedido = localizaPedido.id.ToString(),
                                    nomeColaborador = checkUsuario.nome,
                                    departamento = getDepartamento.titulo
                                };

                                int contador = 0;

                                foreach (var item in produtos)
                                {
                                    if (item.status == 3 || item.status == 13)
                                    {
                                        contador++;
                                    }
                                }

                                localizaPedido.produtos = produtos;

                                if (contador == produtos.Count)
                                {
                                    localizaPedido.status = 10;
                                }
                                else
                                {
                                    localizaPedido.status = pedido.status;
                                }

                                await _pedidos.Update(localizaPedido);

                                email.EmailDe = getEmail.valor;
                                email.EmailPara = "fabiana.lie@reisoffice.com.br";
                                email.ConteudoColaborador = conteudoEmailColaborador;
                                email.Conteudo = conteudoEmails;
                                email.Assunto = "Produto de pedido de EPI reprovado";

                                await _mail.SendEmailAsync(email);

                                return Ok(new { message = "Produto reprovado com sucesso", result = true });
                            }
                            else
                            {
                                return BadRequest(new { message = "O colaborador '" + checkUsuario.nome + "' tem mais de um contrato ativo ou nenhum, verifique no portal do RH", result = false });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "É obrigatório colaborador ter e-mail funcional vinculado", result = false });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Pedido não encontrado", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Verifique se o colaborador esta ativo no portal do RH e se nao tem data de demissao e/ou desligamento atribuida", result = false });
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }    
        }

        /// <summary>
        /// Reprova pedido de EPI
        /// </summary>
        /// <param name="status"></param>
        /// <param name="pedido"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("reprovarPedido/{status}")]
        public async Task<IActionResult> reprovaPedido(int status, [FromBody] EPIPedidosDTO pedido)
        {
            try
            {
                var checkUsuario = await _conUser.GetEmp(pedido.idUsuario);

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
                                EPIEmailRequestDTO email = new EPIEmailRequestDTO();
                                EPIConteudoEmailColaboradorDTO conteudoEmailColaborador = new EPIConteudoEmailColaboradorDTO();
                                List<EPIConteudoEmailDTO> conteudoEmails = new List<EPIConteudoEmailDTO>();

                                var idStatus = await _status.getStatus(status);

                                localizaPedido.status = idStatus.id;

                                foreach (var produto in localizaPedido.produtos)
                                {
                                    var checkStatusItem = await _status.getStatus(produto.status);
                                    var localizaTamanho = await _tamanho.localizaTamanho(produto.tamanho);

                                    if (localizaTamanho != null || !localizaTamanho.Equals(0))
                                    {
                                        tamanho = "";
                                    }
                                    else
                                    {
                                        tamanho = localizaTamanho.tamanho;
                                    }

                                    conteudoEmails.Add(new EPIConteudoEmailDTO
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

                                conteudoEmailColaborador = new EPIConteudoEmailColaboradorDTO
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

                                return Ok(new { message = "Pedido reprovado com sucesso!!!", result = true });
                            }
                            else
                            {
                                return BadRequest(new { message = "O colaborador '" + checkUsuario.nome + "' tem mais de um contrato ativo ou nenhum, verifique no portal do RH", result = false });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "É obrigatório colaborador ter e-mail funcional vinculado", result = false });
                        }
                    }
                    else
                    {
                        return BadRequest(new { message = "Pedido não encontrado", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Verifique se o colaborador esta ativo no portal do RH e se nao tem data de demissao e/ou desligamento atribuida", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona um pedido
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("localizaPedido/{id}")]
        public async Task<IActionResult> GetPedido(int id)
        {
            try
            {
                var pedido = await _pedidos.getPedido(id);

                if (pedido != null)
                {
                    var motivo = await _motivos.getMotivo(pedido.motivo);
                    var usuario = await _conUser.GetEmp(pedido.idUsuario);
                    var status = await _status.getStatus(pedido.status);

                    var lista = new List<object>();

                    foreach (var value in pedido.produtos)
                    {
                        var query = await _produtosEstoque.getProdutoEstoque(value.id);

                        lista.Add(new
                        {
                            value.id,
                            value.quantidade,
                            value.nome,
                            estoque = query.quantidade
                        });
                    }

                    var item = new
                    {
                        pedido.id,
                        pedido.dataPedido,
                        pedido.descricao,
                        produtos = lista,
                        motivo = motivo.nome,
                        usuario = usuario.nome,
                        status = status.nome
                    };

                    return Ok(new { message = "Pedido encontrado", data = item, result = true });
                }
                else
                {
                    return BadRequest(new { message = "Pedido não encontrado", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona os produto ao qual fazem parte de uma categoria especifica
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> getPedidosUsuario([FromRoute] int idUsuario)
        {
            try
            {
                List<object> listaPedidos = new List<object>();

                var pedidos = await _pedidos.getPedidosUsuario(idUsuario);

                foreach(var item in pedidos)
                {
                    var motivo = await _motivos.getMotivo(item.motivo);
                    var usuario = await _conUser.GetEmp(item.idUsuario);

                    listaPedidos.Add(new
                    {
                        item.id,
                        item.dataPedido,
                        item.descricao,
                        item.produtos,
                        motivo = motivo.nome,
                        usuario = usuario.nome
                    });
                }

                return Ok(new { message = "Lista de pedidos encontrado", lista = listaPedidos, result = true });

            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista todos os pedidos
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> getTodosPedidos()
        {
            try
            {
                var localizaPedidos = await _pedidos.getPedidos();

                List<object> pedidosEncontrados = new List<object>();

                if (localizaPedidos != null)
                {
                    foreach (var pedido in localizaPedidos)
                    {
                        var nomeColaborador = await _conUser.GetEmp(pedido.idUsuario);
                        var motivo = await _motivos.getMotivo(pedido.motivo);
                        var status = await _status.getStatus(pedido.status);

                        pedidosEncontrados.Add(new
                        {
                            pedido.id,
                            colaborador = nomeColaborador.nome,
                            pedido.idUsuario,
                            pedido.dataPedido,
                            motivo = motivo.nome,
                            status = status.nome,
                            pedido.produtos
                        });
                    }

                    return Ok(new { message = "Pedidos encontrados!!!", result = true, data = pedidosEncontrados });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum pedido encontrado", result = false });   
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista todos os pedidos
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{status}")]
        public async Task<IActionResult> getPedidos(int status)
        {
            try
            {
                var pedidos = await _pedidos.getTodosPedidos(status);
                var localizaNomeStatus = await _status.getStatus(status);

                if (pedidos != null)
                {
                    return Ok(new { message = "lista encontrada", result = true, lista = pedidos });
                }
                else
                {
                    return BadRequest(new { message = "Nenhum pedido encontrado com statuso '" + localizaNomeStatus.nome + "'", result = false});
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
