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

        public async Task<EPIFornecedoresDTO> Insert(EPIFornecedoresDTO fornecedor)
        {
            try
            {
                var insereFornecedor = await _fornecedor.Insert(fornecedor);

                if (insereFornecedor != null)
                {
                    return insereFornecedor;
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

        public async Task<EPIFornecedoresDTO> Update(EPIFornecedoresDTO fornecedor)
        {
            try
            {
                var atualizaFornecedor = await _fornecedor.Update(fornecedor);

                if (atualizaFornecedor != null)
                {
                    return atualizaFornecedor;
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

        public async Task<EPIFornecedoresDTO> verificaFornecedor(string nome, string cnpj)
        {
            try
            {
                var verificaFornecedor = await _fornecedor.verificaFornecedor(nome, cnpj);

                if (verificaFornecedor != null)
                {
                    return verificaFornecedor;
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
