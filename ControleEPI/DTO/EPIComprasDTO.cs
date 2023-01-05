using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControleEPI.DTO
{
    public class EPIComprasDTO : IEntityTypeConfiguration<EPIComprasDTO>
    {
        public void Configure(EntityTypeBuilder<EPIComprasDTO> builder)
        {
            builder.Property(e => e.pedidosAprovados).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<PedidosAprovados>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [JsonField]
        public IList<PedidosAprovados> pedidosAprovados { get; set; }        
        public DateTime? dataCadastroCompra { get; set; }
        public decimal valorTotalCompra { get; set; }
        public int status { get; set; }
        public int idUsuario { get; set; }
        public DateTime dataFinalizacaoCompra { get; set; }
        public int idFornecedor { get; set; }
    }

    public class PedidosAprovados
    {
        public int idPedidosAprovados { get; set; }
    }
}
