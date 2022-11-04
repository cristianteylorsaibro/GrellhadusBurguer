using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hamburgueria.Tables
{
    [Table("venda")]
    public class Sale
    {
        [Column(@"id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(@"date")]
        public DateTime Date { get; set; }

        [Column(@"total_bruto")]
        public decimal TotalBrute { get; set; }

        [Column(@"desconto")]
        public decimal Discount { get; set; }

        [Column(@"total")]
        public decimal Total { get; set; }

        [Column(@"pagamento")]
        public string Payment { get; set; }

        public Sale()
        {

        }

        public Sale(DateTime date, decimal totalBrute, decimal discount, decimal total, string payment)
        {
            Date = date;
            TotalBrute = totalBrute;
            Discount = discount;
            Total = total;
            Payment = payment;
        }

    }
}
