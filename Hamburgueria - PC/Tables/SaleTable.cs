using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hamburgueria.Tables
{
    [Table("venda_mesa")]
    public class SaleTable
    {
        [Key, Column(@"venda_id"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SaleId { get; set; }

        [Column(@"num_table")]
        public int NumTable { get; set; }

        [ForeignKey("SaleId")]
        public virtual Sale Sale { get; set; }

        public SaleTable(int saleId, int numTable)
        {
            SaleId = saleId;
            NumTable = numTable;
        }

    }
}
