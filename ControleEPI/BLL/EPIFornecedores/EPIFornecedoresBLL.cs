using ControleEPI.DAL.EPIFornecedores;
using ControleEPI.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEPI.BLL.EPIFornecedores
{
    public class EPIFornecedoresBLL : IEPIFornecedoresBLL
    {
        private readonly IEPIFornecedoresDAL _fornecedor;

        public EPIFornecedoresBLL(IEPIFornecedoresDAL fornecedor)
        {
            _fornecedor = fornecedor;
        }

        public async Task<EPIFornecedoresDTO> getFornecedor(int Id)
        {
            try
            {
                var localizaFornecedor = await _fornecedor.getFornecedor(Id);

                if (localizaFornecedor != null)
                {
                    return localizaFornecedor;
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

        public async Task<IEnumerable<EPIFornecedoresDTO>> getFornecedores()
        {
            try
            {
                var localizaFornecedores = await _fornecedor.getFornecedores();

                if (localizaFornecedores != null)
                {
                    return localizaFornecedores;
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

        public Task<EPIFornecedoresDTO> Insert(EPIFornecedoresDTO fornecedor)
        {
            throw new NotImplementedException();
        }

        public Task Update(EPIFornecedoresDTO fornecedor)
        {
            throw new NotImplementedException();
        }

        public Task<EPIFornecedoresDTO> verificaFornecedor(string nome, string cnpj)
        {
            throw new NotImplementedException();
        }
    }
}
