using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vestimenta.DAL.VestRepositorio;
using Vestimenta.DAL.VestVestimenta;
using Vestimenta.DTO;
using Vestimenta.DTO.FromBody;

namespace Vestimenta.BLL.VestRepositorio
{
    public class VestRepositorioBLL : IVestRepositorioBLL
    {
        private readonly IVestRepositorioDAL _repositorio;
        private readonly IVestVestimentaDAL _vestimenta;

        public VestRepositorioBLL(IVestRepositorioDAL repositorio, IVestVestimentaDAL vestimenta)
        {
            _repositorio = repositorio;
            _vestimenta = vestimenta;
        }

        public async Task<VestRepositorioDTO> getRepositorio(int Id)
        {
            try
            {
                var localizaRepositorio = await _repositorio.getRepositorio(Id);

                if (localizaRepositorio != null)
                {
                    return localizaRepositorio;
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

        public async Task<IList<VestSortListDTO>> getRepositorioStatus(string status)
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

                    if (list != null)
                    {
                        return list;
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

        public async Task<VestRepositorioDTO> Insert(VestRepositorioDTO repo)
        {
            try
            {
                var insereRepositorio = await _repositorio.Insert(repo);

                if (insereRepositorio != null)
                {
                    return insereRepositorio;
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
