using System.ComponentModel.DataAnnotations.Schema;

namespace Hamburgueria.Tables
{
    [Table("produto")]
    public class Product
    {
        [Column(@"id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(@"cod")]
        public int Cod { get; set; }

        [Column(@"nome")]
        public string Name { get; set; }

        [Column(@"preco")]
        public decimal Price { get; set; }

        [Column(@"excluido")]
        public bool Deleted { get; set; }

        public Product()
        {

        }

        public Product(int cod, string name, decimal price)
        {
            Cod = cod;
            Name = name;
            Price = price;
            Deleted = false;
        }

        public Product(int id, int cod, string name, decimal price)
        {
            Id = id;
            Cod = cod;
            Name = name;
            Price = price;
            Deleted = false;
        }

    }
}
