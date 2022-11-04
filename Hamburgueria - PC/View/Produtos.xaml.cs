using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Hamburgueria.View
{
    /// <summary>
    /// Lógica interna para Produtos.xaml
    /// </summary>
    public partial class Produtos : Window
    {
        private readonly Sql.Product sqlProduct;

        private bool isNumber = false;

        public Produtos()
        {
            InitializeComponent();

            sqlProduct = new Sql.Product();

            Search.Visibility = Visibility.Collapsed;

            Loaded += Produtos_Loaded;

            GridProdutos.PreviewMouseDoubleClick += GridProdutos_PreviewMouseDoubleClick;

            Search.GotFocus += Search_GotFocus;
            Search.PreviewKeyDown += Search_PreviewKeyDown;
            Search.PreviewTextInput += Search_PreviewTextInput;
            Search.TextChanged += Search_TextChanged;

            lupa.PreviewMouseDown += Lupa_PreviewMouseDown;

            BackProduto.Click += (sender, e) => Close();
            DelProduto.Click += DelProduto_Click;
            EditProduto.Click += (sender, e) => GridProdutos_PreviewMouseDoubleClick(null, null);
            AddProduto.Click += AddProduto_Click;

            menuDel.Click += DelProduto_Click;
            menuEdit.Click += (sender, e) => GridProdutos_PreviewMouseDoubleClick(null, null);
            menuAdd.Click += AddProduto_Click;
        }

        private void Lupa_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Search.Visibility == Visibility.Visible)
                    Search.Visibility = Visibility.Collapsed;
                else
                    Search.Visibility = Visibility.Visible;

                Search.Text = "Pesquisar";
                Search_TextChanged(null, null);
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        public void Produtos_Loaded(object sender, RoutedEventArgs e)
        {
            GridProdutos.Items.Clear();
            foreach (var p in sqlProduct.Select())
                GridProdutos.Items.Add(p);
        }

        private void GridProdutos_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GridProdutos.Items == null)
                return;

            if (GridProdutos.SelectedIndex != -1)
            {
                Tables.Product item = (Tables.Product)GridProdutos.SelectedItem;
                new ProdutosAdd(item).ShowDialog();

                Search_TextChanged(null, null);
            }
            else
            {
                MessageBox.Show("Selecione um PRODUTO para ser editado!", "ERRO", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void Search_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Search.Text == "Pesquisar")
                Search.Text = "";
        }

        private void Search_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (GridProdutos.HasItems)
            {
                if (e.Key == Key.Down)
                {
                    int index = GridProdutos.SelectedIndex + 1;
                    if (index == GridProdutos.Items.Count)
                        index = 0;
                    GridProdutos.SelectedIndex = index;
                }
                else if (e.Key == Key.Up)
                {
                    int index = GridProdutos.SelectedIndex - 1;
                    if (index < 0)
                        index = GridProdutos.Items.Count - 1;
                    GridProdutos.SelectedIndex = index;
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
            string text = Search.Text;
            GridProdutos.Items.Clear();
            if (string.IsNullOrEmpty(text) || text == "Pesquisar")
            {
                isNumber = false;
                foreach (var p in sqlProduct.Select())
                    GridProdutos.Items.Add(p);
            }
            else
            {
                GridProdutos.Visibility = Visibility.Visible;
                isNumber = char.IsDigit(text[0]);
                if (isNumber)
                    foreach (var p in sqlProduct.Select(Convert.ToInt32(text)))
                        GridProdutos.Items.Add(p);
                else
                    foreach (var p in sqlProduct.Select(text))
                        GridProdutos.Items.Add(p);
            }

            if (GridProdutos.HasItems)
                GridProdutos.SelectedItem = GridProdutos.Items[0];
        }
        
        private void DelProduto_Click(object sender, RoutedEventArgs e)
        {
            if (GridProdutos.SelectedIndex != -1)
            {
                if (MessageBox.Show("Tem certeza de que deseja excluir o produto selecionado?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return;

                int id = ((Tables.Product)GridProdutos.SelectedItem).Id;
                sqlProduct.Delete(id);

                Produtos_Loaded(null, null);
            }
            else
            {
                MessageBox.Show("Selecione um PRODUTO para ser editado!", "ERRO", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void AddProduto_Click(object sender, RoutedEventArgs e)
        {
#pragma warning disable IDE0017 // Simplificar a inicialização de objeto
            DispatcherTimer timerToUpdate = new DispatcherTimer();
#pragma warning restore IDE0017 // Simplificar a inicialização de objeto
            timerToUpdate.Interval = new TimeSpan(0, 0, 1);
            timerToUpdate.Tick += (s, se) => Search_TextChanged(null, null);
            timerToUpdate.Start();

            new ProdutosAdd().ShowDialog();
            timerToUpdate.Stop();
        }
    }
}
