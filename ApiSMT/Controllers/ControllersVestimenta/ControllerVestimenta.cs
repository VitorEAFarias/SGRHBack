using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vestimenta.BLL;
using Vestimenta.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ApiSMT.Controllers.ControllersVestimenta
{
    /// <summary>
    /// Classe de vestimentas
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerVestimenta : ControllerBase
    {
        private readonly IVestimentaBLL _vestimenta;
        private readonly IEstoqueBLL _estoque;

        /// <summary>
        /// Construtor VestimentaController
        /// </summary>
        /// <param name="vestimenta"></param>
        /// <param name="estoque"></param>
        public ControllerVestimenta(IVestimentaBLL vestimenta, IEstoqueBLL estoque)
        {
            _vestimenta = vestimenta;
            _estoque = estoque;
        }

        /// <summary>
        /// Insere uma nova vestimenta
        /// </summary>
        /// <param name="vestimenta"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<VestVestimentaDTO>> postVestimenta([FromForm] VestVestimentaDTO vestimenta)
        {
            try
            {
                if (vestimenta != null)
                {
                    var checkVestimenta = await _vestimenta.getNomeVestimenta(vestimenta.nome);

                    if (checkVestimenta != null)
                    {
                        return BadRequest(new { message = "Ja existe uma vestimenta chamada: " + vestimenta.nome, result = false });
                    }
                    else
                    {
                        VestVestimentaDTO inserirVestimenta = new VestVestimentaDTO();

                        inserirVestimenta.ativo = 1;
                        inserirVestimenta.foto = vestimenta.foto;
                        inserirVestimenta.preco = vestimenta.preco;
                        inserirVestimenta.dataCadastro = DateTime.Now;
                        inserirVestimenta.tamanho = vestimenta.tamanho;
                        inserirVestimenta.nome = vestimenta.nome;
                        inserirVestimenta.maximo = vestimenta.maximo;

                        var novaVestimenta = await _vestimenta.Insert(inserirVestimenta);

                        if (novaVestimenta != null)
                        {
                            foreach (var tamanho in novaVestimenta.tamanho)
                            {
                                VestEstoqueDTO estoque = new VestEstoqueDTO();

                                estoque.idItem = novaVestimenta.id;
                                estoque.quantidade = 0;
                                estoque.quantidadeVinculado = 0;
                                estoque.dataAlteracao = DateTime.Now;
                                estoque.tamanho = tamanho.tamanho;
                                estoque.quantidadeUsado = 0;
                                estoque.ativado = "Y";

                                var attEstoque = await _estoque.Insert(estoque);
                            }

                            return Ok(new { message = "Vestimenta inserido com sucesso!!!", result = true, data = novaVestimenta });
                        }
                        else
                        {
                            return BadRequest(new { message = "Erro ao inserir vestimenta!!!", result = false });
                        }
                    }
                }
                else
                {
                    return BadRequest(new { message = "Erro ao inserir vestimenta " + vestimenta.nome, result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Ativa vestimenta
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("ativaVestimenta/{id}")]
        public async Task<ActionResult> ativaVestimenta(int id)
        {
            try
            {
                var checkVestimenta = await _vestimenta.getVestimenta(id);

                if (checkVestimenta != null)
                {
                    if (checkVestimenta.ativo != 1)
                    {
                        checkVestimenta.ativo = 1;

                        await _vestimenta.Update(checkVestimenta);

                        foreach (var tamanho in checkVestimenta.tamanho)
                        {
                            var getEstoque = await _estoque.getDesativados(checkVestimenta.id, tamanho.tamanho);

                            getEstoque.ativado = "Y";

                            await _estoque.Update(getEstoque);
                        }

                        return Ok(new { message = "Vestimenta ativada com sucesso!!!", result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Vestimenta ja esta ativa", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Vestimenta não encontrada", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza Vestimenta
        /// </summary>
        /// <param name="vestimenta"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> putVestimenta([FromForm] VestVestimentaDTO vestimenta)
        {
            try
            {
                VestVestimentaDTO checkVestimenta = await _vestimenta.getVestimenta(vestimenta.id);

                if (checkVestimenta != null)
                {
                    if (vestimenta.tamanho.Count > checkVestimenta.tamanho.Count)
                    {
                        foreach (var tamanho in vestimenta.tamanho)
                        {
                            VestEstoqueDTO estoque = await _estoque.getItemExistente(checkVestimenta.id, tamanho.tamanho);
                            VestEstoqueDTO newEstoque = new VestEstoqueDTO();

                            if (estoque == null)
                            {
                                newEstoque.idItem = checkVestimenta.id;
                                newEstoque.quantidade = 0;
                                newEstoque.tamanho = tamanho.tamanho;
                                newEstoque.dataAlteracao = DateTime.Now;
                                newEstoque.quantidadeVinculado = 0;
                                newEstoque.quantidadeUsado = 0;
                                newEstoque.ativado = "Y";

                                await _estoque.Insert(newEstoque);                               
                            }
                        }

                        checkVestimenta.tamanho = vestimenta.tamanho;

                        await _vestimenta.Update(checkVestimenta);

                        return Ok(new { message = "Tamanhos atualizados com sucesso!!!", result = true });                        
                        
                    }
                    else if (vestimenta.tamanho.Count < checkVestimenta.tamanho.Count)
                    {
                        var listaProdutosDiferentes = checkVestimenta.tamanho.Where(x => !vestimenta.tamanho.Any(x1 => x1.tamanho == x.tamanho))
                            .Union(vestimenta.tamanho.Where(x => !checkVestimenta.tamanho.Any(x1 => x1.tamanho == x.tamanho)));

                        foreach (var item in listaProdutosDiferentes)
                        {
                            var estoque = await _estoque.getItemExistente(checkVestimenta.id, item.tamanho);

                            if (estoque.quantidade == 0 && estoque.quantidadeUsado == 0 && estoque.quantidadeVinculado == 0)
                            {
                                estoque.ativado = "N";

                                await _estoque.Update(estoque);
                                await _vestimenta.Update(vestimenta);
                            }
                            else
                            {
                                return BadRequest(new { message = "Não é possivel desativar um tamanho com itens disponiveis em estoque", result = false });
                            }
                        }

                        return Ok(new { message = "Vestimenta desativada com sucesso!!!", result = true });
                    }
                    else if (checkVestimenta != vestimenta)
                    {
                        await _vestimenta.Update(vestimenta);

                        return Ok(new { message = "Vestimenta alterado com sucesso!!!", result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Erro ao atualizar informações!!!", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Nenhum item encontrado!!!", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Desativa vestimenta
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}/{status}")]
        public async Task<ActionResult> desativaVestimenta(int id, int status)
        {
            try
            {
                var desativaVes = await _vestimenta.getVestimenta(id);                

                if (desativaVes != null)
                {
                    desativaVes.ativo = status;                        
                    
                    await _vestimenta.Update(desativaVes);

                    foreach (var tamanho in desativaVes.tamanho)
                    {
                        var getEstoque = await _estoque.getItemExistente(desativaVes.id, tamanho.tamanho);

                        getEstoque.ativado = "N";

                        await _estoque.Update(getEstoque);
                    }

                    return Ok(new { message = desativaVes.nome + " Desativado com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Nenhuma vestimenta encontrada!!!", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista todos as vestimentas
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> getVestimentas()
        {
            try
            {
                var vestimenta = await _vestimenta.getVestimentas();
                                               
                List<object> tamanhoTotal = new List<object>();                

                if (vestimenta != null)
                {
                    foreach (var item in vestimenta)
                    {
                        var quantidadeEstoque = await _estoque.getItensExistentes(item.id);

                        List<object> tamanhosRam = new List<object>();

                        tamanhosRam.Add(new
                        {
                            nome = item.nome,
                            idVestimenta = item.id,
                            tamanho = item.tamanho,
                            quantidade = quantidadeEstoque,
                            preco = item.preco,
                            Foto = item.foto,
                            Maximo = item.maximo,
                            Ativo = item.ativo
                        });                         

                        tamanhoTotal.AddRange(tamanhosRam);                        
                    }

                    return Ok(new { message = "lista encontrada", result = true, lista = tamanhoTotal });
                }
                else
                {
                    return BadRequest(new { message = "Nenhuma vestimenta encontrada!!!", result = false });
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Seleciona uma vestimenta
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<VestVestimentaDTO>> getVestimenta(int id)
        {
            try
            {
                if (id != 0)
                {
                    var vestimenta = await _vestimenta.getVestimenta(id);
                    var quantidadeEstoque = await _estoque.getItensExistentes(vestimenta.id);

                    var tamanhosRam  = new
                    {
                        nome = vestimenta.nome,
                        idVestimenta = vestimenta.id,
                        tamanho = vestimenta.tamanho,
                        quantidade = quantidadeEstoque,
                        preco = vestimenta.preco,
                        Foto = vestimenta.foto,
                        Maximo = vestimenta.maximo,
                        Ativo = vestimenta.ativo
                    };

                    return Ok(new { message = "Vestimenta encontrada", vestimenta = tamanhosRam, result = true });
                }
                else
                {
                    return BadRequest(new { message = "Vestimenta não encontrado", result = false });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
