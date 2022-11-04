using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hamburgueria.View
{
    /// <summary>
    /// Interação lógica para VendasBalcao.xam
    /// </summary>
    public partial class VendasBalcao : Window
    {
        public Vendas sales;

        public ObservableCollection<Item> Items;

        private readonly Sql.Product sqlProduct;
        private bool isNumber = false;
        private Tables.Product product = null;

        private readonly bool isEditing = false;
        private readonly DateTime dateSale;
        private readonly int oldNumTable;

        public VendasBalcao(Vendas sales, ObservableCollection<Item> items, DateTime dateSale, bool editing = false, int num = -1, string observations = "")
        {
            InitializeComponent();

            sqlProduct = new Sql.Product();

            Items = new ObservableCollection<Item>();
            gridProduct.DataContext = Items;

            foreach (Item it in items)
                Items.Add(it);

            this.sales = sales;

            Closed += VendasBalcao_Closed;
            Loaded += VendasBalcao_Loaded;

            search.PreviewKeyDown += Search_PreviewKeyDown;
            search.PreviewTextInput += Search_PreviewTextInput;
            search.TextChanged += Search_TextChanged;

            gridSearch.PreviewKeyDown += GridSearch_PreviewKeyDown;
            gridSearch.MouseDoubleClick += GridSearch_MouseDoubleClick;

            quantity.PreviewKeyDown += Quantity_PreviewKeyDown;
            quantity.PreviewTextInput += (s, e) => e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);

            numTable.PreviewTextInput += (s, e) => e.Handled = new Regex("[^0-9]+").IsMatch(e.Text); ;

            gridProduct.BeginningEdit += (sender, e) => e.Cancel = true;
            gridProduct.PreviewKeyDown += GridProduct_PreviewKeyDown;

            numTable.PreviewKeyDown += (sender, e) => { if (e.Key == Key.Enter) Confirm_Click(null, null); };
            observation.PreviewKeyDown += (sender, e) => { if (e.Key == Key.Enter) quantity.Focus(); };

            confirm.Click += Confirm_Click;

            if (editing)
            {
                isEditing = true;
                oldNumTable = num;
                numTable.Text = num.ToString();
                labelTotalSale.Content = "TOTAL:" + TotalSale().ToString("C2");
                this.dateSale = dateSale;
                observation.Text = observations;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void VendasBalcao_Closed(object sender, EventArgs e)
        {
            sales.UpdateGrid();
        }

        private void VendasBalcao_Loaded(object sender, RoutedEventArgs e)
        {
            search.Focus();

            gridSearch.Visibility = Visibility.Hidden;
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
                return;
            }

            gridSearch.Visibility = Visibility.Visible;
            isNumber = char.IsDigit(text[0]);
            gridSearch.Items.Clear();
            if (isNumber)
                foreach (var p in sqlProduct.Select(Convert.ToInt32(text)))
                    gridSearch.Items.Add(p);
            else
                foreach (var p in sqlProduct.Select(text))
                    gridSearch.Items.Add(p);

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
            observation.Focus();
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
                    if (i.Name == product.Name + " " + observation.Text)
                    {
                        i.Quantity += q;

                        exist = true;
                        break;
                    }
                }

                if (exist == false)
                    Items.Add(new Item(product.Id, product.Cod, product.Name + " " + observation.Text, product.Price, q));

                labelTotalSale.Content = "TOTAL:" + TotalSale().ToString("C2");

                product = null;

                observation.Text = "";
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
            if (string.IsNullOrEmpty(numTable.Text))
            {
                MessageBox.Show("É necessário informar o número da mesa");
                return;
            }
            else if (gridProduct.HasItems == false)
            {
                MessageBox.Show("É necessário ter produtos a venda");
                return;
            }

            if (isEditing == false)
            {
                int table = Convert.ToInt32(numTable.Text);
                bool exist = false;
                for (int i = 0; i < sales.sales.Count; i++)
                {
                    if (sales.sales[i].File == table.ToString())
                    {
                        exist = true;
                        break;
                    }
                }
                if (exist)
                {
                    MessageBox.Show("Essa mesa já está sendo utilizada!!!");
                    return;
                }

                Sales.Log.Create(DateTime.Now, table, Items);

                product = null;

                Items.Clear();
                labelTotalSale.Content = "TOTAL:R$0,00";
                quantity.Text = "";
                numTable.Text = "";
                search.Focus();

                MessageBox.Show("Venda adicionada com sucesso!!!");

                sales.UpdateGrid();
            }
            else
            {
                int table = Convert.ToInt32(numTable.Text);
                if (oldNumTable != table)
                {
                    bool exist = false;
                    for (int i = 0; i < sales.sales.Count; i++)
                    {
                        if (sales.sales[i].File == table.ToString())
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (exist)
                    {
                        MessageBox.Show("Essa mesa já está sendo utilizada!!!");
                        return;
                    }
                }

                Sales.Log.Create(dateSale, table, Items);

                MessageBox.Show("Venda alterada com sucesso!!!");

                Close();
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
