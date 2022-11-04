using System.ComponentModel;

namespace Hamburgueria
{
    public class Item : INotifyPropertyChanged
    {
        private int quantity;
        private decimal total;

        public int Id { get; set; }
        public int Cod { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                quantity = value;
                OnPropertyChanged("Quantity");

                Total = Quantity * Price;
                OnPropertyChanged("Total");
            }
        }
        public decimal Total 
        { 
            get
            {
                return total;
            }
            private set
            {
                total = value;
                OnPropertyChanged("Total");
            }
        }
        public Item(int id, int cod, string name, decimal price, int quantity)
        {
            Id = id;
            Cod = cod;
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
