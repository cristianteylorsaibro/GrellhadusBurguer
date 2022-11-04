using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hamburgueria.View
{
    /// <summary>
    /// Lógica interna para Relatorios.xaml
    /// </summary>
    public partial class Relatorios : Window
    {
        public Relatorios()
        {
            InitializeComponent();

            this.Loaded += Relatorios_Loaded;

            this.styleBox.SelectionChanged += StyleBox_SelectionChanged;
            this.periodBox.SelectionChanged += PeriodBox_SelectionChanged;

            this.Filter.Click += Filter_Click;
            this.Back.Click += Back_Click;

            this.toExcel.MouseLeftButtonDown += ToExcel_MouseLeftButtonDown;
        }

        private void ToExcel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Vendas
            if (styleBox.SelectedIndex == 0)
            {
                // Diario
                if (periodBox.SelectedIndex == 0)
                {
                    string date = dateOne.SelectedDate.Value.ToString("yyyy-MM-dd");
                    new ExcelLoading(0, 0, date).ShowDialog();
                }
                // Semanal
                else if (periodBox.SelectedIndex == 1)
                {
                    DateTime start = dateOne.SelectedDate.Value.StartOfWeek(DayOfWeek.Sunday);
                    DateTime end = start.AddDays(6);
                    new ExcelLoading(0, 1, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd")).ShowDialog();
                }
                // Mensal
                else if (periodBox.SelectedIndex == 2)
                {
                    string date = dateOne.SelectedDate.Value.ToString("yyyy-MM-");
                    new ExcelLoading(0, 2, date).ShowDialog();
                }
                // Anual
                else if (periodBox.SelectedIndex == 3)
                {
                    string date = dateOne.SelectedDate.Value.ToString("yyyy-");
                    new ExcelLoading(0, 3, date).ShowDialog();
                }
                // Customizado
                else if (periodBox.SelectedIndex == 4)
                {
                    new ExcelLoading(0, 4, dateOne.SelectedDate.Value.ToString("yyyy-MM-dd"), dateTwo.SelectedDate.Value.ToString("yyyy-MM-dd")).ShowDialog();
                }
            }
            // Produtos
            else if (styleBox.SelectedIndex == 1)
            {
                string d = dateOne.SelectedDate.Value.ToShortDateString();
                new ExcelLoading(1, 0, d).ShowDialog();
                
                // Customizado
                if (periodBox.SelectedIndex == 4)
                {
                    DateTime date1 = dateOne.SelectedDate.Value;
                    DateTime date2 = dateOne.SelectedDate.Value;
                    date1 = date1.AddDays(-1);
                    date2 = date2.AddDays(1);

                    string startDate = date1.ToString("yyyy-MM-dd");
                    string endDate = date2.ToString("yyyy-MM-dd");
                    grid.ItemsSource = Model.Relatorio.Product(startDate, endDate);
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void Relatorios_Loaded(object sender, RoutedEventArgs e)
        {
            styleBox.SelectedIndex = 0;
            periodBox.SelectedIndex = 0;
            typeBox.SelectedIndex = 0;

            dateOne.DisplayDateStart = Model.Relatorio.GetMinDate();
            dateTwo.DisplayDateStart = Model.Relatorio.GetMinDate();

            dateOne.SelectedDate = DateTime.Now;
            dateTwo.SelectedDate = DateTime.Now;

            dateOne.DisplayDate = DateTime.Now;
            dateTwo.DisplayDate = DateTime.Now;

            dateOne.DisplayDateEnd = DateTime.Now;
            dateTwo.DisplayDateEnd = DateTime.Now;
        }

        private void StyleBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            typeBox.Visibility = styleBox.SelectedIndex == 0 ? Visibility.Visible : Visibility.Hidden;

            if (styleBox.SelectedIndex == 0 || styleBox.SelectedIndex == 1)
                btnExcel.Visibility = Visibility.Visible;
            else
                btnExcel.Visibility = Visibility.Collapsed;

        }

        private void PeriodBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dateTwo.Visibility = periodBox.SelectedIndex == 4 ? Visibility.Visible : Visibility.Hidden;
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            // Vendas
            if (styleBox.SelectedIndex == 0)
            {
                // Diario
                if (periodBox.SelectedIndex == 0)
                {
                    string date = dateOne.SelectedDate.Value.ToString("yyyy-MM-dd");
                    bool typeOn = false;

                    // Todos
                    if (typeBox.SelectedIndex == 0)
                    {
                        grid.ItemsSource = Model.Relatorio.SaleDay(date);
                        typeOn = true;
                    }
                    // Delivery
                    else if (typeBox.SelectedIndex == 1)
                    {
                        grid.ItemsSource = Model.Relatorio.SaleDayDelivery(date);
                    }
                    // Balcao
                    else if (typeBox.SelectedIndex == 2)
                    {
                        grid.ItemsSource = Model.Relatorio.SaleDayLocal(date);
                    }

                    grid.Columns[0].Header = "TIPO";
                    grid.Columns[1].Header = "DATA";
                    grid.Columns[2].Header = "TOTAL BRUTO";
                    grid.Columns[3].Header = "DESCONTO";
                    grid.Columns[4].Header = "TOTAL";
                    grid.Columns[5].Header = "PAGAMENTO";

                    if (typeOn == false)
                        grid.Columns[0].Visibility = Visibility.Collapsed;

                    grid.Columns[2].ClipboardContentBinding.StringFormat = "C2";
                    grid.Columns[3].ClipboardContentBinding.StringFormat = "C2";
                    grid.Columns[4].ClipboardContentBinding.StringFormat = "C2";

                    decimal sumTB = 0;
                    decimal sumD = 0;
                    decimal sumT = 0;
                    for (int i = 0; i < grid.Items.Count; i++)
                    {
                        var it = (Model.Relatorio.VendaDia)grid.Items[i];
                        sumTB += it.TotalBrute;
                        sumD += it.Discount;
                        sumT += it.Total;
                    }

                    labelBruteTotal.Content = "BRUTO TOTAL:" + sumTB.ToString("C2");
                    labelDiscount.Content = "DESCONTO TOTAL:" + sumD.ToString("C2");
                    labelTotal.Content = "TOTAL:" + sumT.ToString("C2");
                }
                // Semanal
                else if (periodBox.SelectedIndex == 1)
                {
                    // Todos
                    if (typeBox.SelectedIndex == 0)
                    {
                        DateTime start = dateOne.SelectedDate.Value.StartOfWeek(DayOfWeek.Sunday);
                        DateTime end = start.AddDays(6);
                        grid.ItemsSource = Model.Relatorio.SaleWeek(start.ToShortDateString(), end.ToShortDateString());
                    }
                    // Delivery
                    else if (typeBox.SelectedIndex == 1)
                    {
                        DateTime start = dateOne.SelectedDate.Value.StartOfWeek(DayOfWeek.Sunday);
                        DateTime end = start.AddDays(6);
                        grid.ItemsSource = Model.Relatorio.SaleWeekDelivery(start.ToShortDateString(), end.ToShortDateString());
                    }
                    // Balcao
                    else if (typeBox.SelectedIndex == 2)
                    {
                        DateTime start = dateOne.SelectedDate.Value.StartOfWeek(DayOfWeek.Sunday);
                        DateTime end = start.AddDays(6);
                        grid.ItemsSource = Model.Relatorio.SaleWeekLocal(start.ToShortDateString(), end.ToShortDateString());
                    }

                    grid.Columns[0].Header = "DATA";
                    grid.Columns[1].Header = "TOTAL BRUTO";
                    grid.Columns[2].Header = "DESCONTO";
                    grid.Columns[3].Header = "TOTAL";

                    grid.Columns[1].ClipboardContentBinding.StringFormat = "C2";
                    grid.Columns[2].ClipboardContentBinding.StringFormat = "C2";
                    grid.Columns[3].ClipboardContentBinding.StringFormat = "C2";

                    decimal sumTB = 0;
                    decimal sumD = 0;
                    decimal sumT = 0;
                    for (int i = 0; i < grid.Items.Count; i++)
                    {
                        var it = (Model.Relatorio.VendaSemanal)grid.Items[i];
                        sumTB += it.TotalBrute;
                        sumD += it.Discount;
                        sumT += it.Total;
                    }

                    labelBruteTotal.Content = "BRUTO TOTAL:" + sumTB.ToString("C2");
                    labelDiscount.Content = "DESCONTO TOTAL:" + sumD.ToString("C2");
                    labelTotal.Content = "TOTAL:" + sumT.ToString("C2");
                }
                // Mensal
                else if (periodBox.SelectedIndex == 2)
                {
                    // Todos
                    if (typeBox.SelectedIndex == 0)
                    {
                        string date = dateOne.SelectedDate.Value.ToString("yyyy-MM-");
                        grid.ItemsSource = Model.Relatorio.SaleMonth(date);
                    }
                    // Delivery
                    else if (typeBox.SelectedIndex == 1)
                    {
                        string date = dateOne.SelectedDate.Value.ToString("yyyy-MM-");
                        grid.ItemsSource = Model.Relatorio.SaleMonthDelivery(date);
                    }
                    // Balcao
                    else if (typeBox.SelectedIndex == 2)
                    {
                        string date = dateOne.SelectedDate.Value.ToString("yyyy-MM-");
                        grid.ItemsSource = Model.Relatorio.SaleMonthLocal(date);
                    }

                    grid.Columns[0].Header = "DIA";
                    grid.Columns[1].Header = "TOTAL BRUTO";
                    grid.Columns[2].Header = "DESCONTO";
                    grid.Columns[3].Header = "TOTAL";

                    grid.Columns[1].ClipboardContentBinding.StringFormat = "C2";
                    grid.Columns[2].ClipboardContentBinding.StringFormat = "C2";
                    grid.Columns[3].ClipboardContentBinding.StringFormat = "C2";

                    decimal sumTB = 0;
                    decimal sumD = 0;
                    decimal sumT = 0;
                    for (int i = 0; i < grid.Items.Count; i++)
                    {
                        var it = (Model.Relatorio.VendaMes)grid.Items[i];
                        sumTB += it.TotalBrute;
                        sumD += it.Discount;
                        sumT += it.Total;
                    }

                    labelBruteTotal.Content = "BRUTO TOTAL:" + sumTB.ToString("C2");
                    labelDiscount.Content = "DESCONTO TOTAL:" + sumD.ToString("C2");
                    labelTotal.Content = "TOTAL:" + sumT.ToString("C2");
                }
                // Anual
                else if (periodBox.SelectedIndex == 3)
                {
                    // Todos
                    if (typeBox.SelectedIndex == 0)
                    {
                        string date = dateOne.SelectedDate.Value.ToString("yyyy-");
                        grid.ItemsSource = Model.Relatorio.SaleYear(date);
                    }
                    // Delivery
                    else if (typeBox.SelectedIndex == 1)
                    {
                        string date = dateOne.SelectedDate.Value.ToString("yyyy-");
                        grid.ItemsSource = Model.Relatorio.SaleYearDelivery(date);
                    }
                    // Balcao
                    else if (typeBox.SelectedIndex == 2)
                    {
                        string date = dateOne.SelectedDate.Value.ToString("yyyy-");
                        grid.ItemsSource = Model.Relatorio.SaleYearLocal(date);
                    }

                    grid.Columns[0].Header = "MÊS";
                    grid.Columns[1].Header = "TOTAL BRUTO";
                    grid.Columns[2].Header = "DESCONTO";
                    grid.Columns[3].Header = "TOTAL";

                    grid.Columns[1].ClipboardContentBinding.StringFormat = "C2";
                    grid.Columns[2].ClipboardContentBinding.StringFormat = "C2";
                    grid.Columns[3].ClipboardContentBinding.StringFormat = "C2";

                    decimal sumTB = 0;
                    decimal sumD = 0;
                    decimal sumT = 0;
                    for (int i = 0; i < grid.Items.Count; i++)
                    {
                        var it = (Model.Relatorio.VendaAnual)grid.Items[i];
                        sumTB += it.TotalBrute;
                        sumD += it.Discount;
                        sumT += it.Total;
                    }

                    labelBruteTotal.Content = "BRUTO TOTAL:" + sumTB.ToString("C2");
                    labelDiscount.Content = "DESCONTO TOTAL:" + sumD.ToString("C2");
                    labelTotal.Content = "TOTAL:" + sumT.ToString("C2");
                }
                // Customizado
                else if (periodBox.SelectedIndex == 4)
                {
                    // Todos
                    if (typeBox.SelectedIndex == 0)
                    {
                        grid.ItemsSource = Model.Relatorio.SaleWeek(dateOne.SelectedDate.Value.ToShortDateString(), dateTwo.SelectedDate.Value.ToShortDateString());
                    }
                    // Delivery
                    else if (typeBox.SelectedIndex == 1)
                    {
                        grid.ItemsSource = Model.Relatorio.SaleWeekDelivery(dateOne.SelectedDate.Value.ToShortDateString(), dateTwo.SelectedDate.Value.ToShortDateString());
                    }
                    // Balcao
                    else if (typeBox.SelectedIndex == 2)
                    {
                        grid.ItemsSource = Model.Relatorio.SaleWeekLocal(dateOne.SelectedDate.Value.ToShortDateString(), dateTwo.SelectedDate.Value.ToShortDateString());
                    }

                    grid.Columns[0].Header = "DATA";
                    grid.Columns[1].Header = "TOTAL BRUTO";
                    grid.Columns[2].Header = "DESCONTO";
                    grid.Columns[3].Header = "TOTAL";

                    grid.Columns[1].ClipboardContentBinding.StringFormat = "C2";
                    grid.Columns[2].ClipboardContentBinding.StringFormat = "C2";
                    grid.Columns[3].ClipboardContentBinding.StringFormat = "C2";

                    decimal sumTB = 0;
                    decimal sumD = 0;
                    decimal sumT = 0;
                    for (int i = 0; i < grid.Items.Count; i++)
                    {
                        var it = (Model.Relatorio.VendaSemanal)grid.Items[i];
                        sumTB += it.TotalBrute;
                        sumD += it.Discount;
                        sumT += it.Total;
                    }

                    labelBruteTotal.Content = "BRUTO TOTAL:" + sumTB.ToString("C2");
                    labelDiscount.Content = "DESCONTO TOTAL:" + sumD.ToString("C2");
                    labelTotal.Content = "TOTAL:" + sumT.ToString("C2");
                }
            }
            // Produtos
            else if (styleBox.SelectedIndex == 1)
            {
                // Diario
                if (periodBox.SelectedIndex == 0)
                {
                    string date = dateOne.SelectedDate.Value.ToString("yyyy-MM-dd");
                    grid.ItemsSource = Model.Relatorio.Product(date);
                }
                // Semanal
                else if (periodBox.SelectedIndex == 1)
                {
                    DateTime date1 = dateOne.SelectedDate.Value.StartOfWeek(DayOfWeek.Saturday);
                    DateTime date2 = date1.AddDays(6);
                    date1 = date1.AddDays(-1);
                    date2 = date2.AddDays(1);

                    string startDate = date1.ToString("yyyy-MM-dd");
                    string endDate = date2.ToString("yyyy-MM-dd");
                    grid.ItemsSource = Model.Relatorio.Product(startDate, endDate);
                }
                // Mensal
                else if (periodBox.SelectedIndex == 2)
                {
                    string date = dateOne.SelectedDate.Value.ToString("yyyy-MM");
                    grid.ItemsSource = Model.Relatorio.Product(date);
                }
                // Anual
                else if (periodBox.SelectedIndex == 3)
                {
                    string date = dateOne.SelectedDate.Value.ToString("yyyy");
                    grid.ItemsSource = Model.Relatorio.Product(date);
                }
                // Customizado
                else if (periodBox.SelectedIndex == 4)
                {
                    DateTime date1 = dateOne.SelectedDate.Value;
                    DateTime date2 = dateOne.SelectedDate.Value;
                    date1 = date1.AddDays(-1);
                    date2 = date2.AddDays(1);

                    string startDate = date1.ToString("yyyy-MM-dd");
                    string endDate = date2.ToString("yyyy-MM-dd");
                    grid.ItemsSource = Model.Relatorio.Product(startDate, endDate);
                }

                grid.Columns[0].Header = "CÓDIGO";
                grid.Columns[1].Header = "NOME";
                grid.Columns[2].Header = "PREÇO";
                grid.Columns[3].Header = "QUANTIDADE";
                grid.Columns[4].Header = "TOTAL";

                grid.Columns[2].ClipboardContentBinding.StringFormat = "C2";
                grid.Columns[4].ClipboardContentBinding.StringFormat = "C2";

                labelBruteTotal.Content = "BRUTO TOTAL:0,00";
                labelDiscount.Content = "DESCONTO TOTAL:0,00";
                labelTotal.Content = "TOTAL:0,00";
            }
            // Clientes
            else if (styleBox.SelectedIndex == 2)
            {
                // Diario
                if (periodBox.SelectedIndex == 0)
                {
                    string date = dateOne.SelectedDate.Value.ToString("yyyy-MM-dd");
                    grid.ItemsSource = Model.Relatorio.Client(date);
                }
                // Semanal
                else if (periodBox.SelectedIndex == 1)
                {
                    DateTime date1 = dateOne.SelectedDate.Value.StartOfWeek(DayOfWeek.Saturday);
                    DateTime date2 = date1.AddDays(6);
                    date1 = date1.AddDays(-1);
                    date2 = date2.AddDays(1);

                    string startDate = date1.ToString("yyyy-MM-dd");
                    string endDate = date2.ToString("yyyy-MM-dd");
                    grid.ItemsSource = Model.Relatorio.Client(startDate, endDate);
                }
                // Mensal
                else if (periodBox.SelectedIndex == 2)
                {
                    string date = dateOne.SelectedDate.Value.ToString("yyyy-MM");
                    grid.ItemsSource = Model.Relatorio.Client(date);
                }
                // Anual
                else if (periodBox.SelectedIndex == 3)
                {
                    string date = dateOne.SelectedDate.Value.ToString("yyyy");
                    grid.ItemsSource = Model.Relatorio.Client(date);
                }
                // Customizado
                else if (periodBox.SelectedIndex == 4)
                {
                    DateTime date1 = dateOne.SelectedDate.Value;
                    DateTime date2 = dateOne.SelectedDate.Value;
                    date1 = date1.AddDays(-1);
                    date2 = date2.AddDays(1);

                    string startDate = date1.ToString("yyyy-MM-dd");
                    string endDate = date2.ToString("yyyy-MM-dd");
                    grid.ItemsSource = Model.Relatorio.Client(startDate, endDate);
                }

                grid.Columns[0].Header = "NOME";
                grid.Columns[1].Header = "ENDEREÇO";
                grid.Columns[2].Header = "TOTAL BRUTO";
                grid.Columns[3].Header = "DESCONTO";
                grid.Columns[4].Header = "TOTAL";

                grid.Columns[2].ClipboardContentBinding.StringFormat = "C2";
                grid.Columns[3].ClipboardContentBinding.StringFormat = "C2";
                grid.Columns[4].ClipboardContentBinding.StringFormat = "C2";

                labelBruteTotal.Content = "BRUTO TOTAL:0,00";
                labelDiscount.Content = "DESCONTO TOTAL:0,00";
                labelTotal.Content = "TOTAL:0,00";
            }

            for (int i = 0; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].MinWidth = 150;
                grid.Columns[i].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
