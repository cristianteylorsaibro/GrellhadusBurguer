using System;
using System.ComponentModel;

namespace Hamburgueria.View
{
    public class Sale : INotifyPropertyChanged
    {
        public int Type { get; set; }
        public string Value { get; set; }
        public string File { get; set; }
        public string Info { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public string Remove { get; } = "/Hamburgueria;component/Resources/remove.png";
        public string Modify { get; } = "/Hamburgueria;component/Resources/modify.png";
        public string Print { get; } = "/Hamburgueria;component/Resources/print.png";
        public string Confirm { get; } = "/Hamburgueria;component/Resources/confirm.png";

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
