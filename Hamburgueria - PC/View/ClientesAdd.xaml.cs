using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Hamburgueria.View
{
    /// <summary>
    /// Lógica interna para ClientesAdd.xaml
    /// </summary>
    public partial class ClientesAdd : Window
    {
        private readonly Sql.Client sqlClient;
        private readonly int idClient = -1;

        public ClientesAdd(Tables.Client client = null)
        {
            InitializeComponent();

            sqlClient = new Sql.Client();
            
            if (client != null)
            {
                idClient = client.Id;
                clientName.Text = client.Name;
                Adress.Text = client.Street;
                District.Text = client.District;
                Number.Text = client.Number.ToString();
                Complement.Text = client.Complement;
                Telephone.Text = client.Telephone;
                Reference.Text = client.Reference;
            }

            Loaded += ClientesAdd_Loaded;

            Number.PreviewTextInput += (sender, e) => e.Handled = new Regex("[^0-9]").IsMatch(e.Text);

            clientName.GotFocus += delegate { clientName.SelectAll(); };
            Adress.GotFocus += delegate { Adress.SelectAll(); };
            District.GotFocus += delegate { District.SelectAll(); };
            Number.GotFocus += delegate { Number.SelectAll(); };
            Complement.GotFocus += delegate { Complement.SelectAll(); };
            Telephone.GotFocus += delegate { Complement.SelectAll(); };
            Reference.GotFocus += delegate { Reference.SelectAll(); };

            clientName.PreviewKeyDown += (sender, e) => { if (e.Key == Key.Enter) Adress.Focus(); };
            Adress.PreviewKeyDown += (sender, e) => { if (e.Key == Key.Enter) Number.Focus(); };
            Number.PreviewKeyDown += (sender, e) => { if (e.Key == Key.Enter) District.Focus(); };
            District.PreviewKeyDown += (sender, e) => { if (e.Key == Key.Enter) Complement.Focus(); };
            Complement.PreviewKeyDown += (sender, e) => { if (e.Key == Key.Enter) Telephone.Focus(); };
            Telephone.PreviewKeyDown += (sender, e) => { if (e.Key == Key.Enter) Reference.Focus(); };
            Reference.PreviewKeyDown += (sender, e) => { if (e.Key == Key.Enter) SaveBtn_Click(null, null); };

            SaveBtn.Click += SaveBtn_Click;
            ClearBtn.Click += ClearBtn_Click;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void ClientesAdd_Loaded(object sender, RoutedEventArgs e)
        {
            clientName.Focus();
            clientName.SelectAll();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(clientName.Text) ||
                string.IsNullOrEmpty(Adress.Text) ||
                string.IsNullOrEmpty(District.Text) ||
                string.IsNullOrEmpty(Number.Text))
            {
                MessageBox.Show("Os campos com * não podem estar vazios!!!", "ALERTA", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (sqlClient.Exist(clientName.Text, Adress.Text, Convert.ToInt32(Number.Text), District.Text))
            {
                return;
            }

            if (idClient == -1)
            {
                Tables.Client client = new Tables.Client(clientName.Text, Adress.Text, Convert.ToInt32(Number.Text), District.Text, Complement.Text, Telephone.Text, Reference.Text);
                sqlClient.AddOrUpdate(client);

                MessageBox.Show("Cliente cadastrado com Sucesso!!!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearBtn_Click(null, null);
            }
            else
            {
                Tables.Client client = new Tables.Client(idClient, clientName.Text, Adress.Text, Convert.ToInt32(Number.Text), District.Text, Complement.Text, Telephone.Text, Reference.Text);
                sqlClient.AddOrUpdate(client);

                MessageBox.Show("Cliente atualizado com Sucesso!!!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            clientName.Text = String.Empty;
            Adress.Text = String.Empty;
            District.Text = String.Empty;
            Number.Text = String.Empty;
            Complement.Text = String.Empty;
            Telephone.Text = String.Empty;
            Reference.Text = String.Empty;

            clientName.Focus();
        }
    }
}
