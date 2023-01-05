using ControleEPI.DTO._DbContext;
using ControleEPI.DTO;
using ControleEPI.BLL;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ControleEPI.DAL
{
    public class RHConUserDAL : IRHConUserBLL
    {
        public readonly AppDbContextRH _context;
        public RHConUserDAL(AppDbContextRH context)
        {
            _context = context;
        }

        public async Task<List<RHEmpregadoDTO>> getColaboradores(int idSuperior)
        {
            var contratos = await _context.rh_empregados_contratos.FromSqlRaw("SELECT * FROM rh_empregados_contratos WHERE contrato_atual = 1 AND contrato_principal = 1 " +
                "AND id_empregado_superior = '"+ idSuperior + "'").ToListAsync();

            List<RHEmpregadoDTO> colaboradores = new List<RHEmpregadoDTO>();

            foreach (var item in contratos)
            {
                var colaborador = await _context.rh_empregados.FromSqlRaw("SELECT id, nome, ativo FROM rh_empregados WHERE id = '" + item.id_empregado + "' AND ativo = 1").OrderBy(c => c.id).FirstOrDefaultAsync();

                if (colaborador != null)
                {
                    colaboradores.Add(new RHEmpregadoDTO
                    {
                        id = colaborador.id,
                        nome = colaborador.nome
                    });
                }                
            }

            return colaboradores;
        }

        public async Task<RHEmpContatoDTO> getEmail(int idEmpregado)
        {
            return await _context.rh_empregados_contatos.FromSqlRaw("SELECT id, id_empregado, valor FROM rh_empregados_contatos WHERE id_empregado = '"+idEmpregado+ "' AND tipo_contato = 13").OrderBy(c => c.id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<RHDocumentoDTO>> GetDoc()
        {
            return await _context.rh_empregados_documentos.FromSqlRaw("SELECT id, id_empregado, tipo_documento, numero FROM rh_empregados_documentos WHERE tipo_documento = 2").ToListAsync();
        }

        public async Task<IEnumerable<RHEmpregadoDTO>> GetColaboradores()
        {
            return await _context.rh_empregados.FromSqlRaw("SELECT id, nome, ativo FROM rh_empregados WHERE ativo = 1").ToListAsync();
        }

        public async Task<RHEmpregadoDTO> GetEmp(int Id)
        {
            return await _context.rh_empregados.FromSqlRaw("SELECT id, nome, ativo FROM rh_empregados WHERE ativo = 1 AND id = '" + Id + "'").OrderBy(x => x.id).FirstOrDefaultAsync();
        }

        public async Task<RHSenhaDTO> GetSenha(int Id)
        {
            return await _context.rh_empregados_senhas.FromSqlRaw("SELECT id, id_empregado, senha FROM rh_empregados_senhas WHERE id_empregado = '"+Id+ "' AND ativo = 1").OrderBy(c => c.id).FirstOrDefaultAsync();           
        }

        public async Task<RHSenhaDTO> Get(int Id)
        {
            return await _context.rh_empregados_senhas.FindAsync(Id);
        }

        public async Task<RHEmpContatoDTO> GetEmpCont(int Id) 
        {
            return await _context.rh_empregados_contatos.FromSqlRaw("SELECT id, id_empregado, valor FROM rh_empregados_contatos WHERE tipo_contato = 13").OrderBy(c => c.id).FirstOrDefaultAsync();
        }
    }
}
