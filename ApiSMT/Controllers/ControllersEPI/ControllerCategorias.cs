using ControleEPI.BLL.EPICategorias;
using ControleEPI.BLL.EPIProdutos;
using ControleEPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiSMT.Controllers.ControllersEPI
{
    /// <summary>
    /// Classe de categorias
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerCategorias : ControllerBase
    {
        private readonly IEPICategoriasBLL _categoria;
        private readonly IEPIProdutosBLL _produto;

        /// <summary>
        /// Construtos CategoriasController
        /// </summary>
        /// <param name="categoria"></param>
        /// <param name="produto"></param>
        public ControllerCategorias(IEPICategoriasBLL categoria, IEPIProdutosBLL produto)
        {
            _categoria = categoria;
            _produto = produto;
        }

        /// <summary>
        /// Insere nova categoria de produto
        /// </summary>
        /// <param name="categoria"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> insereCategoria([FromBody] EPICategoriasDTO categoria)
        {
            try
            {
                var verificaCategoria = await _categoria.verificaCategoria(categoria.nome);

                if (verificaCategoria == null)
                {
                    var insereCategoria = await _categoria.Insert(categoria);

                    if (insereCategoria != null)
                    {
                        return Ok(new { message = "Categoria inserida com sucesso!!!", result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Erro ao inserir nova categoria", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Ja existe uma categoria nomeada de '" + categoria.nome + "'", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza categoria
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> atualizaCategoria([FromBody] EPICategoriasDTO categoria)
        {
            try
            {
                var localizaCategoria = await _categoria.getCategoria(categoria.id);

                if (localizaCategoria != null)
                {
                    var verificaCategoria = await _categoria.verificaCategoria(categoria.nome);

                    if (verificaCategoria == null)
                    {
                        await _categoria.Update(categoria);

                        return Ok(new { message = "Categoria atualizada com sucesso!!!", result = true });
                    }
                    else
                    {
                        return BadRequest(new { message = "Já existe uma categoria chamada '" + categoria.nome + "'", result = false });
                    }
                }
                else
                {
                    return BadRequest(new { message = "Categoria não encontrada", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista todas categorias
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> listaCategorias()
        {
            try
            {
                var localizaCategorias = await _categoria.getCategorias();

                if (localizaCategorias != null)
                {
                    return Ok(new { message = "Categorias encontradas", lista = localizaCategorias, result = true });
                }
                else
                {
                    return BadRequest(new { message = "Nenhuma categoria encontrada", result = false});
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Localiza uma categoria
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> localizaCategoria(int id)
        {
            try
            {
                var localizaCategoria = await _categoria.getCategoria(id);

                if (localizaCategoria != null)
                {
                    return Ok(new { message = "Categoria encontrada!!!", result = true, categoria = localizaCategoria });
                }               
                else
                {
                    return BadRequest(new { message = "Categoria não encontrada", result = false });
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deleta categoria
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> deletaCategoria(int id)
        {
            try
            {
                var deletaCategoria = await _categoria.getCategoria(id);

                if (deletaCategoria == null)
                    return BadRequest(new { message = "Categoria não encontrada", result = false });

                var verificaProduto = await _produto.verificaCategoria(deletaCategoria.id);

                if (verificaProduto == null || verificaProduto.Equals(0))
                {
                    await _categoria.Delete(deletaCategoria.id);
                    return Ok(new { message = "Tamanho deletado com sucesso!!!", result = true });
                }
                else
                {
                    return BadRequest(new { message = "Não é possivel deletar categorias vinculadas a produtos", result = false });
                }
                
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
