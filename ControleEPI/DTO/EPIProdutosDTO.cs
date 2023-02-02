using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ControleEPI.DTO
{
    public class EPIProdutosDTO
    {
        public int id { get; set; }
        public string nome { get; set; }
        public int idCategoria { get; set; }
        public decimal preco { get; set; }
        public int idCertificadoAprovacao { get; set; }
        public int validadeEmUso { get; set; }
        public string ativo { get; set; }
        public byte[] foto { get; set; }
        public int maximo { get; set; }
    }

    public class ProdutosTamanhosDTO : IEntityTypeConfiguration<ProdutosTamanhosDTO>
    {
        public void Configure(EntityTypeBuilder<ProdutosTamanhosDTO> builder)
        {
            builder.Property(e => e.tamanhos).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<Tamanho>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public int id { get; set; }
        public string nome { get; set; }
        public int idCategoria { get; set; }
        public decimal preco { get; set; }
        public int idCertificadoAprovacao { get; set; }
        public int validadeEmUso { get; set; }
        public string ativo { get; set; }
        public byte[] foto { get; set; }
        public int maximo { get; set; }
        public IList<Tamanho> tamanhos { get; set; }
    }

    public class Tamanho
    {
        public int idTamanho { get; set; }
        public string tamanho { get; set; }
    }
}
