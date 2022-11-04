using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hamburgueria.Tables
{
    [Table("produto_venda")]
    public class ProductSale
    {
        [Column(@"id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key, Column(@"venda_id", Order = 0)]
        public int SaleId { get; set; }

        [Key, Column(@"produto_id", Order = 1)]
        public int ProductId { get; set; }

        [Column(@"nome")]
        public string Name { get; set; }

        [Column(@"preco")]
        public decimal Price { get; set; }

        [Column(@"quantidade")]
        public int Quantity { get; set; }

        [Column(@"total")]
        public decimal Total { get; set; }

        [ForeignKey("SaleId")]
        public virtual Sale Sale { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public ProductSale(int saleId, int productId, string name, decimal price, int quantity, decimal total)
        {
            SaleId = saleId;
            ProductId = productId;
            Name = name;
            Price = price;
            Quantity = quantity;
            Total = total;
        }
    }
}
