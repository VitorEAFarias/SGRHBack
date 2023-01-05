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
    public class VestPedidosDTO : IEntityTypeConfiguration<VestPedidosDTO>
    {
        public void Configure(EntityTypeBuilder<VestPedidosDTO> builder)
        {
            builder.Property(e => e.item).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<ItemDTO>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [JsonField]
        public IList<ItemDTO> item { get; set; }
        public int idUsuario { get; set; }
        public DateTime dataPedido { get; set; }
        public int status { get; set; }
        public DateTime dataAlteracao { get; set; }
        public string observacoes { get; set; }
        public int idUsuarioAlteracao { get; set; }

        public VestPedidosDTO(string observacoes = "")
        {
            this.observacoes = observacoes;
        }
    }    

    public class ItemDTO
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string tamanho { get; set; }
        public int quantidade { get; set; }
        public int status { get; set; }
        public DateTime dataAlteracao { get; set; }
        public string usado { get; set; }

        public ItemDTO(string usado = "N")
        {
            this.usado = usado;
        }
    }
}
