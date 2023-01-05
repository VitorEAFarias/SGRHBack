using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vestimenta.DTO
{
    public class VestComprasDTO : IEntityTypeConfiguration<VestComprasDTO>
    {
        public void Configure(EntityTypeBuilder<VestComprasDTO> builder)
        {
            builder.Property(e => e.itensRepositorio).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<Repositorio>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));            
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [JsonField]
        public IList<Repositorio> itensRepositorio { get; set; }
        public DateTime dataCompra { get; set; }
        public int idUsuario { get; set; }
        public int status { get; set; }
        public string descricao { get; set; }
    }

    public class Repositorio : IEntityTypeConfiguration<Repositorio>
    {
        public void Configure(EntityTypeBuilder<Repositorio> builder)
        {
            builder.Property(e => e.idRepositorio).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<int>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        [JsonField]
        public IList<int> idRepositorio { get; set; }
        public int idItem { get; set; }        
        public string tamanho { get; set; }
        public double preco { get; set; }
        public int quantidade { get; set; }
    }
}
