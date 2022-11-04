using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hamburgueria.View
{
    /// <summary>
    /// Interação lógica para VendasRapida.xam
    /// </summary>
    public partial class VendasRapida : Window
    {
        public ObservableCollection<Item> Items = new ObservableCollection<Item>();

        private readonly Sql.Product sqlProduct;
        private bool isNumber = false;
        private Tables.Product product = null;

        public VendasRapida()
        {
            InitializeComponent();

            sqlProduct = new Sql.Product();
            gridProduct.DataContext = Items;

            Loaded += VendasRapida_Loaded;

            search.PreviewKeyDown += Search_PreviewKeyDown;
            search.PreviewTextInput += Search_PreviewTextInput;
            search.TextChanged += Search_TextChanged;

            gridSearch.PreviewKeyDown += GridSearch_PreviewKeyDown;
            gridSearch.MouseDoubleClick += GridSearch_MouseDoubleClick;

            quantity.PreviewKeyDown += Quantity_PreviewKeyDown;
            quantity.PreviewTextInput += (sender, e) => e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);

            gridProduct.PreviewKeyDown += GridProduct_PreviewKeyDown;

            confirm.Click += Confirm_Click;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void VendasRapida_Loaded(object sender, RoutedEventArgs e)
        {
            search.Focus();
        }

        private void Search_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GridSearch_MouseDoubleClick(null, null);
            }
            else if (gridSearch.HasItems)
            {
                if (e.Key == Key.Down)
                {
                    int index = gridSearch.SelectedIndex + 1;
                    if (index == gridSearch.Items.Count)
                        index = 0;
                    gridSearch.SelectedIndex = index;
                }
                else if (e.Key == Key.Up)
                {
                    int index = gridSearch.SelectedIndex - 1;
                    if (index < 0)
                        index = gridSearch.Items.Count - 1;
                    gridSearch.SelectedIndex = index;
                }
            }
        }

        private void Search_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (isNumber)
                e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = search.Text;
            if (string.IsNullOrEmpty(text))
            {
                isNumber = false;
                gridSearch.Visibility = Visibility.Hidden;
            }
            else
            {
                gridSearch.Visibility = Visibility.Visible;
                isNumber = char.IsDigit(text[0]);
                gridSearch.Items.Clear();
                if (isNumber)
                    foreach (var p in sqlProduct.Select(Convert.ToInt32(text)))
                        gridSearch.Items.Add(p);
                else
                    foreach (var p in sqlProduct.Select(text))
                        gridSearch.Items.Add(p);
            }

            if (gridSearch.HasItems)
                gridSearch.SelectedItem = gridSearch.Items[0];
        }

        private void GridSearch_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                GridSearch_MouseDoubleClick(null, null);
        }

        private void GridSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (gridSearch.HasItems == false)
                return;

            product = (Tables.Product)gridSearch.SelectedItem;
            search.Text = product.Name;
            gridSearch.Visibility = Visibility.Hidden;
            quantity.Focus();
        }

        private void Quantity_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int q;
                try
                {
                    q = Convert.ToInt32(quantity.Text);
                    if (q == 0)
                    {
                        MessageBox.Show("Valor na quantidade precisa ser maior que 0!!!");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Informe alguma valor no campo Quantidade!!!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (product == null)
                {
                    MessageBox.Show("Selecione um produto!!!", "ERRO", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool exist = false;
                foreach (Item i in Items)
                {
                    if (i.Id == product.Id)
                    {
                        i.Quantity += q;

                        exist = true;
                        break;
                    }
                }

                if (exist == false)
                    Items.Add(new Item(product.Id, product.Cod, product.Name, product.Price, q));

                labelTotalSale.Content = "TOTAL:" + TotalSale().ToString("C2");

                product = null;
                quantity.Text = "";
                search.Text = "";
                gridSearch.Visibility = Visibility.Hidden;

                search.Focus();
            }
        }

        private void GridProduct_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Items.Count == 0)
                return;

            int index = gridProduct.SelectedIndex;

            if (e.Key == Key.Add)
            {
                Items[index].Quantity++;
            }
            else if (e.Key == Key.Subtract)
            {
                Items[index].Quantity--;
                if (Items[index].Quantity == 0)
                    Items.RemoveAt(index);
            }
            else if (e.Key == Key.Delete)
            {
                Items.RemoveAt(index);
            }

            labelTotalSale.Content = "TOTAL:" + TotalSale().ToString("C2");
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (Items.Count == 0)
            {
                MessageBox.Show("É necessário ter produtos a venda");
                return;
            }

            VendasPagamento pagamento = new VendasPagamento(TotalSale())
            {
                items = Items,
                dateSale = DateTime.Now,
            };
            pagamento.ShowDialog();

            if (pagamento.Confirmed)
            {
                MessageBox.Show("Venda realizada com sucesso!!!");

                Items.Clear();
                search.Text = "";
                quantity.Text = "";
                product = null;
                labelTotalSale.Content = "TOTAL:" + TotalSale().ToString("C2");
            }
        }

        private decimal TotalSale()
        {
            decimal t = 0;
            foreach (Item i in Items)
                t += i.Total;

            return t;
        }
    }
}
