using Microsoft.EntityFrameworkCore;

namespace RH.DTO._DbContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<RHEmpregadoDTO> rh_empregados { get; set; }
        public DbSet<RHDocumentoDTO> rh_empregados_documentos { get; set; }
        public DbSet<RHSenhaDTO> rh_empregados_senhas { get; set; }
        public DbSet<RHCargosDTO> rh_cargos { get; set; }
        public DbSet<RHDepartamentosDTO> rh_departamentos { get; set; }
        public DbSet<RHEmpContratosDTO> rh_empregados_contratos { get; set; }
        public DbSet<RHEmpContatoDTO> rh_empregados_contatos { get; set; }
    }
}
