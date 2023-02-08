using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vestimenta.DAL.VestEstoque;
using Vestimenta.DAL.VestVestimenta;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestVestimenta
{
    public class VestVestimentaBLL : IVestVestimentaBLL
    {
        private readonly IVestVestimentaDAL _vestimenta;
        private readonly IVestEstoqueDAL _estoque;

        public VestVestimentaBLL(IVestVestimentaDAL vestimenta, IVestEstoqueDAL estoque)
        {
            _vestimenta = vestimenta;
            _estoque = estoque;
        }

        public async Task<VestVestimentaDTO> getNomeVestimenta(VestVestimentaDTO vestimenta)
        {
            try
            {
                var checkVestimenta = await _vestimenta.getNomeVestimenta(vestimenta.nome);

                if (checkVestimenta != null)
                {
                    return null;
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

                        return vestimenta;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<VestVestimentaDTO> ativaVestimenta(int Id)
        {
            try
            {
                var checkVestimenta = await _vestimenta.getVestimenta(Id);

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

                        return checkVestimenta;
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

        public async Task<IList<TamanhoTotalDTO>> getVestimentas()
        {
            try
            {
                var vestimenta = await _vestimenta.getVestimentas();

                List<TamanhoTotalDTO> tamanhoTotal = new List<TamanhoTotalDTO>();

                if (vestimenta != null)
                {
                    foreach (var item in vestimenta)
                    {
                        var quantidadeEstoque = await _estoque.getItensExistentes(item.id);

                        List<TamanhosRam> tamanhosRam = new List<TamanhosRam>
                        {
                            new TamanhosRam
                            {
                                nome = item.nome,
                                idVestimenta = item.id,
                                tamanho = item.tamanho,
                                quantidade = quantidadeEstoque,
                                preco = item.preco,
                                foto = item.foto,
                                maximo = item.maximo,
                                ativo = item.ativo
                            }
                        };

                        tamanhoTotal.Add(new TamanhoTotalDTO { 
                            tRam = tamanhosRam
                        });
                    }

                    return tamanhoTotal;
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

        public async Task<VestVestimentaDTO> Update(VestVestimentaDTO vestimenta)
        {
            try
            {
                var checkVestimenta = await _vestimenta.getVestimenta(vestimenta.id);

                if (checkVestimenta != null)
                {
                    VestVestimentaDTO atualizaVestimenta = new VestVestimentaDTO();

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

                        atualizaVestimenta = await _vestimenta.Update(checkVestimenta);

                        if (atualizaVestimenta != null)
                        {
                            return atualizaVestimenta;
                        }
                        else
                        {
                            return null;   
                        }
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
                                return null;
                            }
                        }

                        return vestimenta;
                    }
                    else if (checkVestimenta != vestimenta)
                    {
                        atualizaVestimenta =await _vestimenta.Update(vestimenta);

                        if (atualizaVestimenta != null)
                        {
                            return atualizaVestimenta;
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

        public async Task<VestVestimentaDTO> desativaVestimenta(int id, int status)
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

                    return desativaVes;
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

        public async Task<TamanhosRam> getVestimenta(int Id)
        {
            try
            {
                var vestimenta = await _vestimenta.getVestimenta(Id);
                var quantidadeEstoque = await _estoque.getItensExistentes(vestimenta.id);

                var tamanhosRam = new TamanhosRam
                {
                    nome = vestimenta.nome,
                    idVestimenta = vestimenta.id,
                    tamanho = vestimenta.tamanho,
                    quantidade = quantidadeEstoque,
                    preco = vestimenta.preco,
                    foto = vestimenta.foto,
                    maximo = vestimenta.maximo,
                    ativo = vestimenta.ativo
                };

                if (tamanhosRam != null)
                {
                    return tamanhosRam;
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
