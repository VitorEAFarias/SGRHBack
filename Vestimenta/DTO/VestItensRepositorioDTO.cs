using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Vestimenta.DTO
{
    public class VestItensRepositorioDTO : IEntityTypeConfiguration<VestItensRepositorioDTO>
    {
        public void Configure(EntityTypeBuilder<VestItensRepositorioDTO> builder)
        {
            builder.Property(e => e.idPedido).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<int>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [JsonField]
        public IList<int> idPedido { get; set; }
        public string tamanho { get; set; }
        public int quantidade { get; set; }
    }
}
