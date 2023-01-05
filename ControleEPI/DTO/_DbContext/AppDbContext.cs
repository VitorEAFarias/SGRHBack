using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;

namespace ControleEPI.DTO._DbContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Produtos>();
            modelBuilder.AddJsonFields();
        }

        public DbSet<EPICategoriasDTO> EPICategoria { get; set; }
        public DbSet<EPICertificadoAprovacaoDTO> EPICertificadoAprovacao { get; set; }
        public DbSet<EPIComprasDTO> EPICompras { get; set; }        
        public DbSet<EPIFornecedoresDTO> EPIFornecedores { get; set; }
        public DbSet<EPILogComprasDTO> EPILogCompras { get; set; }        
        public DbSet<EPILogEstoqueDTO> EPILogEstoque { get; set; }
        public DbSet<EPIMotivoDTO> EPIMotivos { get; set; }
        public DbSet<EPIPedidosDTO> EPIPedidos { get; set; }
        public DbSet<EPIPedidosAprovadosDTO> EPIPedidosAprovados { get; set; }
        public DbSet<EPIProdutosDTO> EPIProdutos { get; set; }
        public DbSet<EPIProdutosEstoqueDTO> EPIProdutosEstoque { get; set; }        
        public DbSet<EPIStatusDTO> EPIStatus { get; set; }
        public DbSet<EPITamanhosDTO> EPITamanhos { get; set; }
        public DbSet<EPIVinculoDTO> EPIVinculo { get; set; }
    }
}
