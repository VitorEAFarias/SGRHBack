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
    public class EPIPedidosDTO : IEntityTypeConfiguration<EPIPedidosDTO>
    {
        public void Configure(EntityTypeBuilder<EPIPedidosDTO> builder) 
        {
            builder.Property(e => e.produtos).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<Produtos>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public DateTime? dataPedido { get; set; }
        public int idUsuario { get; set; }
        public string descricao { get; set; }
        public int motivo { get; set; }
        [JsonField]
        public IList<Produtos> produtos { get; set; }
        public int status { get; set; }
    }  

    public class Produtos
    {
        public int id { get; set; }
        public string nome { get; set; }
        public int quantidade { get; set; }
        public int status { get; set; }
        public int idTamanho { get; set; }
    }
}
