using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DAL.VestEstoque;
using Vestimenta.DAL.VestLog;
using Vestimenta.DAL.VestVestimenta;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestEstoque
{
    public class VestEstoqueBLL : IVestEstoqueBLL
    {
        private readonly IVestEstoqueDAL _estoque;
        private readonly IVestLogDAL _log;
        private readonly IVestVestimentaDAL _vestimenta;

        public VestEstoqueBLL(IVestEstoqueDAL estoque, IVestLogDAL log, IVestVestimentaDAL vestimenta)
        {
            _estoque = estoque;
            _log = log;
            _vestimenta = vestimenta;
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<VestEstoqueDTO>> atualizaLogEstoque(int id, List<VestEstoqueDTO> estoque)
        {
            try
            {
                if (estoque != null)
                {
                    foreach (var item in estoque)
                    {
                        var checkEstoque = await _estoque.getItemExistente(item.idItem, item.tamanho);

                        if (item.quantidadeUsado == checkEstoque.quantidadeUsado)
                        {
                            item.dataAlteracao = DateTime.Now;

                            await _estoque.Update(item);

                            VestLogDTO log = new VestLogDTO();

                            log.data = DateTime.Now;
                            log.idUsuario = id;
                            log.idItem = item.idItem;
                            log.quantidadeAnt = checkEstoque.quantidade;
                            log.quantidadeDep = checkEstoque.quantidade + item.quantidade;
                            log.tamanho = checkEstoque.tamanho;
                            log.usado = "N";

                            await _log.Insert(log);
                        }
                        else
                        {
                            item.dataAlteracao = DateTime.Now;

                            await _estoque.Update(item);

                            VestLogDTO log = new VestLogDTO();

                            log.data = DateTime.Now;
                            log.idUsuario = id;
                            log.idItem = item.idItem;
                            log.quantidadeAnt = checkEstoque.quantidadeUsado;
                            log.quantidadeDep = checkEstoque.quantidadeUsado + item.quantidadeUsado;
                            log.tamanho = checkEstoque.tamanho;
                            log.usado = "N";

                            var insereLog = await _log.Insert(log);
                        }
                    }

                    return estoque;
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

        public Task<VestEstoqueDTO> getDesativados(int idItem, string tamanho)
        {
            throw new NotImplementedException();
        }

        public Task<IList<VestEstoqueDTO>> getEstoque()
        {
            throw new NotImplementedException();
        }

        public async Task<VestEstoqueDTO> getItemEstoque(int Id)
        {
            try
            {
                var estoque = await _estoque.getItemEstoque(Id);

                if (estoque != null)
                {
                    return estoque;
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

        public Task<VestEstoqueDTO> getItemExistente(int idItem, string tamanho)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<EstoqueDTO>> getItensExistentes(int idItens)
        {
            try
            {
                var itens = await _estoque.getItensExistentes(idItens);

                List<EstoqueDTO> lista = new List<EstoqueDTO>();

                if (itens != null)
                {
                    foreach (var item in itens)
                    {
                        var getNome = await _vestimenta.getVestimenta(item.idItem);

                        lista.Add(new EstoqueDTO
                        {
                            id = item.id,
                            idItem = item.idItem,
                            nome = getNome.nome,
                            quantidade = item.quantidade,
                            quantidadeUsado = item.quantidadeUsado
                        });
                    }

                    return lista;
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

        public Task<VestEstoqueDTO> Insert(VestEstoqueDTO estoque)
        {
            throw new NotImplementedException();
        }

        public Task Update(VestEstoqueDTO estoque)
        {
            throw new NotImplementedException();
        }
    }
}
