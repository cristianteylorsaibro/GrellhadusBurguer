using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hamburgueria.Tables
{
    [Table("venda_delivery")]
    public class SaleDelivery
    {
        [Key, Column(@"venda_id"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SaleId { get; set; }
        
        [Column(@"name")]
        public string Name { get; set; }
        
        [Column(@"address")]
        public string Address { get; set; }

        [ForeignKey("SaleId")]
        public virtual Sale Sale { get; set; }

        public SaleDelivery(int saleId, string name, string address)
        {
            SaleId = saleId;
            Name = name;
            Address = address;
        }
    }
}
