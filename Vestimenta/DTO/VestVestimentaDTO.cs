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
    public class VestVestimentaDTO : IEntityTypeConfiguration<VestVestimentaDTO>
    {
        public void Configure(EntityTypeBuilder<VestVestimentaDTO> builder)
        {
            builder.Property(e => e.tamanho).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<Tamanho>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string nome { get; set; }        
        public DateTime dataCadastro { get; set; }
        public byte[] foto { get; set; }
        public decimal preco { get; set; }
        public int ativo { get; set; }
        [JsonField]
        public IList<Tamanho> tamanho { get; set; }
        public int maximo { get; set; }
    }

    public class Tamanho
    {
        public string tamanho { get; set; }
    }
}
