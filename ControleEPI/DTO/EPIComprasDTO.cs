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
            builder.Property(e => e.idPedidosAprovados).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<IList<PedidosAprovados>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [JsonField]
        public IList<PedidosAprovados> idPedidosAprovados { get; set; }        
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

    public class ComprasProdutosDTO
    {
        public int idPedido { get; set; }
        public int idProduto { get; set; }
        public string nomeProduto { get; set; }
        public int idProdutoAprovado { get; set; }
        public int quantidade { get; set; }
        public decimal preco { get; set; }
        public int idTamanho { get; set; }
        public string tamanho { get; set; }
    }

    public class ComprasDTO
    {
        public int idCompra { get; set; }
        public IList<ComprasProdutosDTO> produtosAprovados { get; set; }
        public DateTime? dataCadastraCompra { get; set; }
        public DateTime? dataFinalizacaoCompra { get; set; }
        public decimal valorTotalCompra { get; set; }
        public int idStatus { get; set; }
        public string status { get; set; }
        public int idUsuario { get; set; }
        public string usuario { get; set; }
        public int idFornecedor { get; set; }
        public string fornecedor { get; set; }
    }
}
