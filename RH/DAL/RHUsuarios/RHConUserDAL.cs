using RH.DTO._DbContext;
using RH.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RH.DAL.RHUsuarios
{
    public class RHConUserDAL : IRHConUserDAL
    {
        public readonly AppDbContext _context;
        public RHConUserDAL(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IList<RHEmpregadoDTO>> getColaboradores(int idSuperior)
        {
            var contratos = await _context.rh_empregados_contratos.FromSqlRaw("SELECT * FROM rh_empregados_contratos WHERE contrato_atual = 1 AND contrato_principal = 1 " +
                "AND id_empregado_superior = '" + idSuperior + "'").ToListAsync();

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
            return await _context.rh_empregados_contatos.FromSqlRaw("SELECT id, id_empregado, valor FROM rh_empregados_contatos WHERE id_empregado = '" + idEmpregado + "' AND tipo_contato = 13").OrderBy(c => c.id).FirstOrDefaultAsync();
        }

        public async Task<RHDocumentoDTO> GetDoc(string numero)
        {
            return await _context.rh_empregados_documentos.FromSqlRaw("SELECT id, id_empregado, tipo_documento, numero FROM rh_empregados_documentos WHERE tipo_documento = 2 AND " +
                "numero = '" + numero + "'").OrderBy(x => x.id).FirstOrDefaultAsync();
        }

        public async Task<IList<RHEmpregadoDTO>> GetColaboradores()
        {
            return await _context.rh_empregados.FromSqlRaw("SELECT id, nome, ativo FROM rh_empregados WHERE ativo = 1").ToListAsync();
        }

        public async Task<RHEmpregadoDTO> GetEmp(int Id)
        {
            return await _context.rh_empregados.FromSqlRaw("SELECT id, nome, ativo FROM rh_empregados WHERE ativo = 1 AND id = '" + Id + "'").OrderBy(x => x.id).FirstOrDefaultAsync();
        }

        public async Task<RHSenhaDTO> GetSenha(int Id)
        {
            return await _context.rh_empregados_senhas.FromSqlRaw("SELECT id, id_empregado, senha FROM rh_empregados_senhas WHERE id_empregado = '" + Id + "' AND ativo = 1").OrderBy(c => c.id).FirstOrDefaultAsync();
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
