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

    public class ItemUsuarioDTO
    {
        public int id { get; set; }
        public string nome { get; set; }
        public IList<ItemDTO> pedido { get; set; }
        public string status { get; set; }
        public DateTime dataPedido { get; set; }
    }

    public class PedidosPententesDTO
    {
        public VestPedidosDTO pedido { get; set; }
        public int idItem { get; set; }
        public string emitente { get; set; }
        public string nomeItem { get; set; }
        public string tamanhoItem { get; set; }
        public int quantidade { get; set; }
        public int quantidadeEstoque { get; set; }
        public int quantidadeEstoqueUsado { get; set; }
        public int status { get; set; }
    }

    public class CompraDTO
    {
        public int id { get; set; }
        public string nome { get; set; }
        public IList<ItensCompraDTO> pedido { get; set; }
        public int idStatus { get; set; }
        public string status { get; set; }
        public int idUsuario { get; set; }
        public int idUsuarioAlteracao { get; set; }
        public DateTime dataAlteracao { get; set; }
        public string observacoes { get; set; }
        public DateTime dataPedido { get; set; }
    }

    public class ItensCompraDTO
    {
        public int id { get; set; }
        public DateTime dataAlteracao { get; set; }
        public string nome { get; set; }
        public int quantidade { get; set; }
        public int status { get; set; }
        public string tamanho { get; set; }
        public string usado { get; set; }
        public string enviadoCompra { get; set; }
        public string statusNome { get; set; }
        public int estoque { get; set; }
        public int estoqueUsado { get; set; }
    }
}
