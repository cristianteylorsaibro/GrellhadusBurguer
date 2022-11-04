using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Hamburgueria.View
{
    /// <summary>
    /// Lógica interna para ProdutosAdd.xaml
    /// </summary>
    public partial class ProdutosAdd : Window
    {
        private readonly Sql.Product sqlProduct;
        private readonly int idProduct = -1;
        private readonly int codProduct = -1;

        public ProdutosAdd(Tables.Product product = null)
        {
            InitializeComponent();

            sqlProduct = new Sql.Product();

            if (product != null)
            {
                idProduct = product.Id;
                codProduct = product.Cod;
                Description.Text = product.Name;
                Code.Text = product.Cod.ToString();
                Price.Text = product.Price.ToString();
            }

            Loaded += ProdutosAdd_Loaded;

            Description.PreviewKeyDown += Description_PreviewKeyDown;
            Code.PreviewKeyDown += Code_PreviewKeyDown;
            Price.PreviewKeyDown += Price_PreviewKeyDown;

            Code.PreviewTextInput += (s, e) => e.Handled = new Regex("[^0-9]").IsMatch(e.Text);
            Price.PreviewTextInput += (s, e) => e.Handled = new Regex("[^0-9,.]").IsMatch(e.Text);

            ClearBtn.Click += ClearBtn_Click;
            SaveBtn.Click += SaveBtn_Click;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void Description_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Code.Focus();
        }

        private void Code_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Price.Focus();
        }

        private void Price_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SaveBtn_Click(null, null);
        }

        private void ProdutosAdd_Loaded(object sender, RoutedEventArgs e)
        {
            Description.Focus();
            Description.SelectAll();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            Description.Text = string.Empty;
            Code.Text = string.Empty;
            Price.Text = string.Empty;

            Description.Focus();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Price.Text) || 
                string.IsNullOrEmpty(Description.Text) || 
                string.IsNullOrEmpty(Code.Text))
            {
                MessageBox.Show("Todos os campos precisam serem preenchidos!!!", "ERRO", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            int newCod = Convert.ToInt32(Code.Text);
            if (idProduct == -1)
            {
                if (sqlProduct.Exist(newCod))
                {
                    MessageBox.Show("O Codigo digitado, já pertence a outro PRODUTO!!!", "ERRO", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Tables.Product product = new Tables.Product(newCod, Description.Text, Convert.ToDecimal(Price.Text));
                sqlProduct.AddOrUpdate(product);

                MessageBox.Show("Produto cadastrado com sucesso!", "", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearBtn_Click(null, null);
            }
            else
            {
                if (codProduct != newCod)
                {
                    if (sqlProduct.Exist(newCod))
                    {
                        MessageBox.Show("O Codigo digitado, já pertence a outro PRODUTO!!!", "ERRO", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                Tables.Product product = new Tables.Product(idProduct, newCod, Description.Text, Convert.ToDecimal(Price.Text));
                sqlProduct.AddOrUpdate(product);

                MessageBox.Show("Produto atualizado com sucesso!", "", MessageBoxButton.OK, MessageBoxImage.Information);

                Close();
            }
        }
    }
}
