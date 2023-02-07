using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vestimenta.DAL.VestRepositorio;
using Vestimenta.DTO;

namespace Vestimenta.BLL.VestRepositorio
{
    public class VestRepositorioBLL : IVestRepositorioBLL
    {
        private readonly IVestRepositorioDAL _repositorio;

        public VestRepositorioBLL(IVestRepositorioDAL repositorio)
        {
            _repositorio = repositorio;
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
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

        public Task<VestRepositorioDTO> getRepositorioItensPedidos(int idPedido, int idItem)
        {
            throw new NotImplementedException();
        }

        public Task<IList<VestRepositorioDTO>> getRepositorios()
        {
            throw new NotImplementedException();
        }

        public Task<IList<VestRepositorioDTO>> getRepositorioStatus(string status)
        {
            throw new NotImplementedException();
        }

        public Task<VestRepositorioDTO> Insert(VestRepositorioDTO repo)
        {
            throw new NotImplementedException();
        }

        public Task Update(VestRepositorioDTO repo)
        {
            throw new NotImplementedException();
        }
    }
}
