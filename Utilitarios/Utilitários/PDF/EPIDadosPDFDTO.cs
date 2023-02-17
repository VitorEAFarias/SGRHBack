using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Utilitarios.Utilitários.PDF
{
    public class EPIDadosPDFDTO : IEntityTypeConfiguration<EPIDadosPDFDTO>
    {
        public void Configure(EntityTypeBuilder<EPIDadosPDFDTO> builder)
        {
            builder.Property(e => e.produtos).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<HistoricoEPI>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public string nome { get; set; }
        public string departamento { get; set; }
        public string cargo { get; set; }
        [JsonField]
        public IList<HistoricoEPI> produtos { get; set; }
    }

    public class HistoricoEPI
    {
        public string nomeProduto { get; set; }
        public string tamanho { get; set; }
        public DateTime dataVinculo { get; set; }
        public DateTime dataDesvinculo { get; set; }
        public string statusAtual { get; set; }
        public DateTime validade { get; set; }
        public int quantidade { get; set; }
    }
}
