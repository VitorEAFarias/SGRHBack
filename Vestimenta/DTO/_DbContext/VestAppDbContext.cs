using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;

namespace Vestimenta.DTO._DbContext
{
    public class VestAppDbContext : DbContext
    {
        public VestAppDbContext(DbContextOptions<VestAppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Tamanho>();
            modelBuilder.AddJsonFields();
        }

        public DbSet<VestComprasDTO> VestCompra { get; set; }
        public DbSet<VestEstoqueDTO> VestEstoque { get; set; }
        public DbSet<VestLogDTO> VestLog { get; set; }
        public DbSet<VestPedidosDTO> VestPedidos { get; set; }
        public DbSet<VestStatusDTO> VestStatus { get; set; }
        public DbSet<VestVestimentaDTO> VestVestimenta { get; set; }
        public DbSet<VestVinculoDTO> VestVinculo { get; set; }
        public DbSet<VestRepositorioDTO> VestRepositorio { get; set; }
    }
}
