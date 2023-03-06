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
        public DateTime dataPedido { get; set; }
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
        public int tamanho { get; set; }
    }

    public class PedidosDTO : IEntityTypeConfiguration<PedidosDTO>
    {
        public void Configure(EntityTypeBuilder<PedidosDTO> builder)
        {
            builder.Property(e => e.produtos).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<ProdutosEstoqueDTO>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public int idPedido { get; set; }
        public DateTime? dataPedido { get; set; }
        public string descricao { get; set; }
        public int idMotivo { get; set; }
        public IList<ProdutosEstoqueDTO> produtos { get; set; }
        public string motivo { get; set; }
        public string usuario { get; set; }
        public int idUsuario { get; set; }
        public int idStatus { get; set; }
        public string status { get; set; }
    }

    public class ProdutosEstoqueDTO
    {
        public int id { get; set; }
        public int quantidade { get; set; }
        public string nome { get; set; }
        public int idTamanho { get; set; }
        public string tamanho { get; set; }
        public int idStatus { get; set; }
        public string nomeStatus { get; set; }
        public int estoque { get; set; }
        public int idCertificado { get; set; }
        public string numeroCertificado { get; set; }
    }

    public class PedidosUsuarioDTO
    {
        public int idPedido { get; set; }
        public DateTime? dataPedido { get; set; }
        public string descricao { get; set; }
        public IList<Produtos> produtos { get; set; }
        public string motivo { get; set; }
        public int idUsuario { get; set; }
        public string nomeUsuario { get; set; }
        public int idStatus { get; set; }
        public string status { get; set; }
    }
}
