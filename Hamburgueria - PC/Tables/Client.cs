using System.ComponentModel.DataAnnotations.Schema;

namespace Hamburgueria.Tables
{
    [Table("cliente")]
    public class Client
    {
        [Column(@"id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(@"nome")]
        public string Name { get; set; }

        [Column(@"rua")]
        public string Street { get; set; }

        [Column(@"numero")]
        public int Number { get; set; }

        [Column(@"bairro")]
        public string District { get; set; }

        [Column(@"complemento")]
        public string Complement { get; set; }

        [Column(@"telefone")]
        public string Telephone { get; set; }

        [Column(@"referencia")]
        public string Reference { get; set; }

        [Column(@"excluido")]
        public bool Deleted { get; set; }

        public Client()
        {

        }

        public Client(string name, string street, int number, string district)
        {
            Name = name;
            Street = street;
            Number = number;
            District = district;
        }

        public Client(string name, string street, int number, string district, string complement, string telephone, string reference)
        {
            Name = name;
            Street = street;
            Number = number;
            District = district;
            Complement = complement;
            Telephone = telephone;
            Reference = reference;
        }

        public Client(int id, string name, string street, int number, string district, string complement, string telephone, string reference)
        {
            Id = id;
            Name = name;
            Street = street;
            Number = number;
            District = district;
            Complement = complement;
            Telephone = telephone;
            Reference = reference;
            Deleted = false;
        }

    }
}
