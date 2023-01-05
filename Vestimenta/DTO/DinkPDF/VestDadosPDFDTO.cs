using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Vestimenta.DTO.DinkPDF
{
    public class VestDadosPDFDTO : IEntityTypeConfiguration<VestDadosPDFDTO>
    {
        public void Configure(EntityTypeBuilder<VestDadosPDFDTO> builder)
        {
            builder.Property(e => e.vestimentas).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<Historico>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public string nome { get; set; }
        public string departamento { get; set; }
        public string cargo { get; set; }
        [JsonField]
        public IList<Historico> vestimentas { get; set; }        
    }

    public class Historico
    {
        public string nomeVestimenta { get; set; }
        public string tamanho { get; set; }
        public DateTime dataVinculo { get; set; }
        public DateTime dataDesvinculo { get; set; }
        public string statusAtual { get; set; }
        public string usado { get; set; }
        public int quantidade { get; set; }
    }
}
