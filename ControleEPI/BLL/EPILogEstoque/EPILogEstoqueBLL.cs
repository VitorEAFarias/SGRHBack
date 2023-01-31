using ControleEPI.DAL.EPILogEstoque;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPILogEstoque
{
    public class EPILogEstoqueBLL : IEPILogEstoqueBLL
    {
        private readonly IEPILogEstoqueDAL _logEstoque;

        public EPILogEstoqueBLL(IEPILogEstoqueDAL logEstoque)
        {
            _logEstoque = logEstoque;
        }

        public async Task<EPILogEstoqueDTO> GetLogEstoque(int Id)
        {
            try
            {
                var localizaLogEstoque = await _logEstoque.GetLogEstoque(Id);

                if (localizaLogEstoque != null)
                {
                    return localizaLogEstoque;
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

        public async Task<IEnumerable<EPILogEstoqueDTO>> GetLogsEstoque()
        {
            try
            {
                var localizaLogsEstoque = await _logEstoque.GetLogsEstoque();

                if (localizaLogsEstoque != null)
                {
                    return localizaLogsEstoque;
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

        public Task<EPILogEstoqueDTO> Insert(EPILogEstoqueDTO logEstoque)
        {
            try
            {
                var insereLogEstoque = _logEstoque.Insert(logEstoque);

                if (insereLogEstoque != null)
                {
                    return insereLogEstoque;
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
