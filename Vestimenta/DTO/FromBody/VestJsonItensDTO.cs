using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Vestimenta.DTO.FromBody
{
    public class VestJsonItensDTO : IEntityTypeConfiguration<VestJsonItensDTO>
    {
        public void Configure(EntityTypeBuilder<VestJsonItensDTO> builder)
        {
            builder.Property(e => e.id).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<int>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        [JsonField]
        public IList<int> id { get; set; }
    }
}
