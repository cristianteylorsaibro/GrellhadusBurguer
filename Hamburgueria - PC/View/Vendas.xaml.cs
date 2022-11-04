using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hamburgueria.View
{
    /// <summary>
    /// Lógica interna para Vendas.xaml
    /// </summary>
    public partial class Vendas : Window
    {
        public ObservableCollection<Sale> sales = new ObservableCollection<Sale>();

        public Vendas()
        {
            InitializeComponent();

            gridSales.DataContext = sales;

            this.Loaded += (sender, e) => UpdateGrid();

            this.gridSales.BeginningEdit += (sender, e) => e.Cancel = true;
            this.gridSales.PreviewMouseUp += GridSales_PreviewMouseUp;

            this.back.Click += Back_Click;
            filter.SelectionChanged += (sender, e) => UpdateGrid();
            this.addFast.Click += AddFast_Click;
            this.addLocal.Click += AddLocal_Click;
            this.addDelivery.Click += AddDelivery_Click;

            this.menuFast.Click += AddFast_Click;
            this.menuLocal.Click += AddLocal_Click;
            this.menuDelivery.Click += AddDelivery_Click;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        public void UpdateGrid()
        {
            Sales.Log.Select(sales, filter.SelectedIndex);
        }

        private void GridSales_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (gridSales.HasItems == false || gridSales.CurrentCell.IsValid == false)
                return;

            View.Sale it = (View.Sale)gridSales.SelectedItem;
            if (it == null)
                return;

            // Delete
            if (gridSales.CurrentCell.Column.DisplayIndex == 6)
            {
                if (it.Type == 0)
                {
                    if (MessageBox.Show("Tem certeza que deseja excluir a venda?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Sales.Log.Delete(it.Date);
                        sales.Remove(it);
                    }
                }
                else
                {
                    if (MessageBox.Show("Tem certeza que deseja excluir a venda?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Sales.Log.Delete(it.Date);
                        sales.Remove(it);
                    }
                }
            }
            // Edit
            else if (gridSales.CurrentCell.Column.DisplayIndex == 7)
            {
                if (it.Type == 0)
                {
                    VendasBalcao balcao = new VendasBalcao(this, Sales.Log.Products(it.Date), it.Date, true, Sales.Log.NumTable(it.Date));
                    balcao.ShowDialog();
                }
                else
                {
                    Tables.Client client = Sales.Log.Client(it.Date);
                    string[] info = Sales.Log.InfoDelivery(it.Date);

                    VendasDelivery delivery = new VendasDelivery(this, Sales.Log.Products(it.Date), client, it.Date, true, info[0], info[1], info[2], info[3]);
                    delivery.ShowDialog();
                }
            }
            // Print
            else if (gridSales.CurrentCell.Column.DisplayIndex == 8)
            {
                if (it.Type == 0)
                {
                    MessageBox.Show("Somente vendas Deliveries podem ser impressas antes de finalizadas!!!");
                    return;
                }
                else
                {
                    Tables.Client client = Sales.Log.Client(it.Date);
                    string[] info = Sales.Log.InfoDelivery(it.Date);

                    string payment = info[0];
                    decimal discount = Convert.ToDecimal(info[1]);
                    decimal valuePay = Convert.ToDecimal(info[2]);
                    decimal change = Convert.ToDecimal(info[3]);

                    ObservableCollection<Item> items = Sales.Log.Products(it.Date);

                    TXT.Sale(client, it.Date, it.Total + discount, discount, it.Total, payment, valuePay, change, items);
                    new Impressao().ShowDialog();
                }
            }
            // Confirm
            else if (gridSales.CurrentCell.Column.DisplayIndex == 9)
            {
                if (it.Type == 0)
                {
                    new VendasPagamento(it.Total)
                    {
                        typeSale = 1,
                        sales = this,
                        numTable = Sales.Log.NumTable(it.Date),
                        dateSale = it.Date
                    }.ShowDialog();

                    UpdateGrid();
                }
                else
                {
                    Tables.Client client = Sales.Log.Client(it.Date);
                    string[] info = Sales.Log.InfoDelivery(it.Date);

                    if (MessageBox.Show("Tem certeza de que deseja FINALIZAR a venda do Cliente " + client.Name + "??", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        return;

                    string payment = info[0];
                    decimal discount = Convert.ToDecimal(info[1]);
                    decimal valuePay = Convert.ToDecimal(info[2]);
                    decimal change = Convert.ToDecimal(info[3]);

                    ObservableCollection<Item> items = Sales.Log.Products(it.Date);

                    new Sql.Sale().Insert(client, it.Date, it.Total + discount, discount, it.Total, payment, items);

                    if (MessageBox.Show("Deseja imprimir o CUPOM NÃO FISCAL??", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        TXT.Sale(client, it.Date, it.Total + discount, discount, it.Total, payment, valuePay, change, items);
                        new Impressao().ShowDialog();
                    }

                    Sales.Log.Delete(it.Date);
                    sales.Remove(it);
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddFast_Click(object sender, RoutedEventArgs e)
        {
            VendasRapida rapida = new VendasRapida();
            rapida.ShowDialog();
        }

        private void AddLocal_Click(object sender, RoutedEventArgs e)
        {
            VendasBalcao balcao = new VendasBalcao(this, new ObservableCollection<Item>(), DateTime.Today);
            balcao.ShowDialog();
        }

        private void AddDelivery_Click(object sender, RoutedEventArgs e)
        {
            VendasDelivery delivery = new VendasDelivery(this, new ObservableCollection<Item>(), new Tables.Client(), DateTime.Today);
            delivery.ShowDialog();
        }
    }
}
