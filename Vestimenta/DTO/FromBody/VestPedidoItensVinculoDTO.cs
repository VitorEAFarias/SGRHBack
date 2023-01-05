using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Vestimenta.DTO.FromBody
{
    public class VestPedidoItensVinculoDTO : IEntityTypeConfiguration<VestPedidoItensVinculoDTO>
    {
        public void Configure(EntityTypeBuilder<VestPedidoItensVinculoDTO> builder)
        {
            builder.Property(e => e.idItens).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<Itens>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public int idPedido { get; set; }
        [JsonField]
        public IList<Itens> idItens { get; set; }
    }

    public class Itens
    {
        public int idItem { get; set; }
        public string tamanho { get; set; }
    }
}
