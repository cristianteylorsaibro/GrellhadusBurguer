using Hamburgueria.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using m_Excel = Microsoft.Office.Interop.Excel;

namespace Hamburgueria.View
{
    /// <summary>
    /// Lógica interna para ExcelLoading.xaml
    /// </summary>
    public partial class ExcelLoading : Window
    {
        private readonly string date;
        private readonly string end;
        private readonly int style = 0;
        private readonly int period = 0;
        private int value = 0;

        public ExcelLoading(int style, int period, string date, string end = "")
        {
            InitializeComponent();

            DispatcherTimer timer;
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            this.style = style;
            this.period = period;
            this.date = date;
            this.end = end;
            this.Loaded += ExcelLoading_Loaded;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            loadingBar.Value = value;
        }

        private void ExcelLoading_Loaded(object sender, RoutedEventArgs e)
        {
            if (style == 0)
            {
                loadingBar.Maximum = 8;
                if (period == 0)
                    Task.Factory.StartNew(() => SaleDay(date));
                else if (period == 1)
                    Task.Factory.StartNew(() => SaleWeek(date, end));
                else if (period == 2)
                    Task.Factory.StartNew(() => SaleMonth(date));
                else if (period == 3)
                    Task.Factory.StartNew(() => SaleYear(date));
                else if (period == 4)
                    Task.Factory.StartNew(() => SaleCustom(date, end));
            }
            else if (style == 1)
            {
                DateTime dateTime = Convert.ToDateTime(date);

                if (period == 0)
                {
                    loadingBar.Maximum = 10;
                    Task.Factory.StartNew(() => Product(dateTime));
                }
            }
        }

        private m_Excel.Application app;
        private m_Excel.Workbook workbook;
        private m_Excel.Worksheet worksheet;
        private readonly object misValue = Missing.Value;

        private bool ok = false;

        private void SaleDay(string date)
        {
            ok = false;

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Exportar para Excel";
            fileDialog.FileName = "Vendas Diárias - " + date;
            fileDialog.DefaultExt = "*.xlsx";
            fileDialog.Filter = "*.xlsx | *.xlsx";
            fileDialog.FileOk += FileDialog_FileOk;
            fileDialog.ShowDialog();

            if (ok == false)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Close()));
                return;
            }

            string fileName = fileDialog.FileName;
            try
            {
                File.Copy("Excel\\vendasDiarias.xlsx", fileName, true);
                value += 1;
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                app = new m_Excel.Application();
                workbook = app.Workbooks.Open(fileName);
                worksheet = workbook.Worksheets[1];

                List<Relatorio.VendaDia> all = Relatorio.SaleDay(date);
                value += 1;
                for (int i = 0; i < all.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = all[i].TYPE;
                    worksheet.Cells[i + 3, 2] = all[i].Date;
                    worksheet.Cells[i + 3, 3] = all[i].TotalBrute;
                    worksheet.Cells[i + 3, 4] = all[i].Discount;
                    worksheet.Cells[i + 3, 5] = all[i].Total;
                    worksheet.Cells[i + 3, 6] = all[i].Payment;
                }
                value += 1;

                worksheet = workbook.Worksheets[2];
                List<Relatorio.VendaDia> delivery = Relatorio.SaleDayDelivery(date);
                value += 1;
                for (int i = 0; i < delivery.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = delivery[i].TYPE;
                    worksheet.Cells[i + 3, 2] = delivery[i].Date;
                    worksheet.Cells[i + 3, 3] = delivery[i].TotalBrute;
                    worksheet.Cells[i + 3, 4] = delivery[i].Discount;
                    worksheet.Cells[i + 3, 5] = delivery[i].Total;
                    worksheet.Cells[i + 3, 6] = delivery[i].Payment;
                }
                value += 1;

                worksheet = workbook.Worksheets[3];
                List<Relatorio.VendaDia> local = Relatorio.SaleDayLocal(date);
                value += 1;
                for (int i = 0; i < local.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = local[i].TYPE;
                    worksheet.Cells[i + 3, 2] = local[i].Date;
                    worksheet.Cells[i + 3, 3] = local[i].TotalBrute;
                    worksheet.Cells[i + 3, 4] = local[i].Discount;
                    worksheet.Cells[i + 3, 5] = local[i].Total;
                    worksheet.Cells[i + 3, 6] = local[i].Payment;
                }
                value += 1;

                workbook.Save();
                workbook.Close(true, misValue, misValue);
                value += 1;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Close()));

                Process.Start(fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                app.Quit();
            }
        }

        private void SaleWeek(string start, string end)
        {
            ok = false;

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Exportar para Excel";
            fileDialog.FileName = "Venda Semanal - " + start + " - " + end;
            fileDialog.DefaultExt = "*.xlsx";
            fileDialog.Filter = "*.xlsx | *.xlsx";
            fileDialog.FileOk += FileDialog_FileOk;
            fileDialog.ShowDialog();

            if (ok == false)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Close()));
                return;
            }

            string fileName = fileDialog.FileName;
            try
            {
                File.Copy("Excel\\vendasSemanais.xlsx", fileName, true);
                value += 1;
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                app = new m_Excel.Application();
                workbook = app.Workbooks.Open(fileName);
                worksheet = workbook.Worksheets[1];

                List<Relatorio.VendaSemanal> all = Relatorio.SaleWeek(start, end);
                value += 1;
                for (int i = 0; i < all.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = all[i].Date;
                    worksheet.Cells[i + 3, 2] = all[i].TotalBrute;
                    worksheet.Cells[i + 3, 3] = all[i].Discount;
                    worksheet.Cells[i + 3, 4] = all[i].Total;
                }
                value += 1;

                worksheet = workbook.Worksheets[2];
                List<Relatorio.VendaSemanal> delivery = Relatorio.SaleWeekDelivery(start, end);
                value += 1;
                for (int i = 0; i < delivery.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = delivery[i].Date;
                    worksheet.Cells[i + 3, 2] = delivery[i].TotalBrute;
                    worksheet.Cells[i + 3, 3] = delivery[i].Discount;
                    worksheet.Cells[i + 3, 4] = delivery[i].Total;
                }
                value += 1;

                worksheet = workbook.Worksheets[3];
                List<Relatorio.VendaSemanal> local = Relatorio.SaleWeekLocal(start, end);
                value += 1;
                for (int i = 0; i < local.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = local[i].Date;
                    worksheet.Cells[i + 3, 2] = local[i].TotalBrute;
                    worksheet.Cells[i + 3, 3] = local[i].Discount;
                    worksheet.Cells[i + 3, 4] = local[i].Total;
                }
                value += 1;

                workbook.Save();
                workbook.Close(true, misValue, misValue);
                value += 1;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Close()));

                Process.Start(fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                app.Quit();
            }
        }

        private void SaleMonth(string date)
        {
            ok = false;

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Exportar para Excel";
            fileDialog.FileName = "Venda Mensal - " + date.Substring(0, date.Length - 1);
            fileDialog.DefaultExt = "*.xlsx";
            fileDialog.Filter = "*.xlsx | *.xlsx";
            fileDialog.FileOk += FileDialog_FileOk;
            fileDialog.ShowDialog();

            if (ok == false)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Close()));
                return;
            }

            string fileName = fileDialog.FileName;
            try
            {
                File.Copy("Excel\\vendasMensais.xlsx", fileName, true);
                value += 1;
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                app = new m_Excel.Application();
                workbook = app.Workbooks.Open(fileName);
                worksheet = workbook.Worksheets[1];

                List<Relatorio.VendaMes> all = Relatorio.SaleMonth(date);
                value += 1;
                for (int i = 0; i < all.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = all[i].Day;
                    worksheet.Cells[i + 3, 2] = all[i].TotalBrute;
                    worksheet.Cells[i + 3, 3] = all[i].Discount;
                    worksheet.Cells[i + 3, 4] = all[i].Total;
                }
                value += 1;

                worksheet = workbook.Worksheets[2];
                List<Relatorio.VendaMes> delivery = Relatorio.SaleMonthDelivery(date);
                value += 1;
                for (int i = 0; i < delivery.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = delivery[i].Day;
                    worksheet.Cells[i + 3, 2] = delivery[i].TotalBrute;
                    worksheet.Cells[i + 3, 3] = delivery[i].Discount;
                    worksheet.Cells[i + 3, 4] = delivery[i].Total;
                }
                value += 1;

                worksheet = workbook.Worksheets[3];
                List<Relatorio.VendaMes> local = Relatorio.SaleMonthLocal(date);
                value += 1;
                for (int i = 0; i < local.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = local[i].Day;
                    worksheet.Cells[i + 3, 2] = local[i].TotalBrute;
                    worksheet.Cells[i + 3, 3] = local[i].Discount;
                    worksheet.Cells[i + 3, 4] = local[i].Total;
                }
                value += 1;

                workbook.Save();
                workbook.Close(true, misValue, misValue);
                value += 1;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Close()));

                Process.Start(fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                app.Quit();
            }
        }

        private void SaleYear(string date)
        {
            ok = false;

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Exportar para Excel";
            fileDialog.FileName = "Venda Anual - " + date.Substring(0, date.Length - 1);
            fileDialog.DefaultExt = "*.xlsx";
            fileDialog.Filter = "*.xlsx | *.xlsx";
            fileDialog.FileOk += FileDialog_FileOk;
            fileDialog.ShowDialog();

            if (ok == false)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Close()));
                return;
            }

            string fileName = fileDialog.FileName;
            try
            {
                File.Copy("Excel\\vendasAnuais.xlsx", fileName, true);
                value += 1;
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                app = new m_Excel.Application();
                workbook = app.Workbooks.Open(fileName);
                worksheet = workbook.Worksheets[1];

                List<Relatorio.VendaAnual> all = Relatorio.SaleYear(date);
                value += 1;
                for (int i = 0; i < all.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = all[i].Month;
                    worksheet.Cells[i + 3, 2] = all[i].TotalBrute;
                    worksheet.Cells[i + 3, 3] = all[i].Discount;
                    worksheet.Cells[i + 3, 4] = all[i].Total;
                }
                value += 1;

                worksheet = workbook.Worksheets[2];
                List<Relatorio.VendaAnual> delivery = Relatorio.SaleYearDelivery(date);
                value += 1;
                for (int i = 0; i < delivery.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = delivery[i].Month;
                    worksheet.Cells[i + 3, 2] = delivery[i].TotalBrute;
                    worksheet.Cells[i + 3, 3] = delivery[i].Discount;
                    worksheet.Cells[i + 3, 4] = delivery[i].Total;
                }
                value += 1;

                worksheet = workbook.Worksheets[3];
                List<Relatorio.VendaAnual> local = Relatorio.SaleYearLocal(date);
                value += 1;
                for (int i = 0; i < local.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = local[i].Month;
                    worksheet.Cells[i + 3, 2] = local[i].TotalBrute;
                    worksheet.Cells[i + 3, 3] = local[i].Discount;
                    worksheet.Cells[i + 3, 4] = local[i].Total;
                }
                value += 1;

                workbook.Save();
                workbook.Close(true, misValue, misValue);
                value += 1;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Close()));

                Process.Start(fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                app.Quit();
            }
        }

        private void SaleCustom(string start, string end)
        {
            ok = false;

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Exportar para Excel";
            fileDialog.FileName = "Venda Customizada - " + start + " - " + end;
            fileDialog.DefaultExt = "*.xlsx";
            fileDialog.Filter = "*.xlsx | *.xlsx";
            fileDialog.FileOk += FileDialog_FileOk;
            fileDialog.ShowDialog();

            if (ok == false)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Close()));
                return;
            }

            string fileName = fileDialog.FileName;
            try
            {
                File.Copy("Excel\\vendasCustomizadas.xlsx", fileName, true);
                value += 1;
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                app = new m_Excel.Application();
                workbook = app.Workbooks.Open(fileName);
                worksheet = workbook.Worksheets[1];

                List<Relatorio.VendaSemanal> all = Relatorio.SaleWeek(start, end);
                value += 1;
                for (int i = 0; i < all.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = all[i].Date;
                    worksheet.Cells[i + 3, 2] = all[i].TotalBrute;
                    worksheet.Cells[i + 3, 3] = all[i].Discount;
                    worksheet.Cells[i + 3, 4] = all[i].Total;
                }
                value += 1;

                worksheet = workbook.Worksheets[2];
                List<Relatorio.VendaSemanal> delivery = Relatorio.SaleWeekDelivery(start, end);
                value += 1;
                for (int i = 0; i < delivery.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = delivery[i].Date;
                    worksheet.Cells[i + 3, 2] = delivery[i].TotalBrute;
                    worksheet.Cells[i + 3, 3] = delivery[i].Discount;
                    worksheet.Cells[i + 3, 4] = delivery[i].Total;
                }
                value += 1;

                worksheet = workbook.Worksheets[3];
                List<Relatorio.VendaSemanal> local = Relatorio.SaleWeekLocal(start, end);
                value += 1;
                for (int i = 0; i < local.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = local[i].Date;
                    worksheet.Cells[i + 3, 2] = local[i].TotalBrute;
                    worksheet.Cells[i + 3, 3] = local[i].Discount;
                    worksheet.Cells[i + 3, 4] = local[i].Total;
                }
                value += 1;

                workbook.Save();
                workbook.Close(true, misValue, misValue);
                value += 1;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Close()));

                Process.Start(fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                app.Quit();
            }
        }

        private void Product(DateTime date)
        {
            ok = false;

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Exportar para Excel";
            fileDialog.FileName = "Produtos - " + date.ToString("yyyy-MM-dd");
            fileDialog.DefaultExt = "*.xlsx";
            fileDialog.Filter = "*.xlsx | *.xlsx";
            fileDialog.FileOk += FileDialog_FileOk;
            fileDialog.ShowDialog();

            if (ok == false)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Close()));
                return;
            }

            string fileName = fileDialog.FileName;
            try
            {
                File.Copy("Excel\\produtos.xlsx", fileName, true);
                value += 1;
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                app = new m_Excel.Application();
                workbook = app.Workbooks.Open(fileName);
                
                worksheet = workbook.Worksheets[1];
                string day = date.ToString("yyyy-MM-dd");
                worksheet.Cells[1, 1] = "DIÁRIO - " + day;
                List < Relatorio.P> products = Relatorio.Product(day);
                value += 1;
                for (int i = 0; i < products.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = products[i].Cod;
                    worksheet.Cells[i + 3, 2] = products[i].Name;
                    worksheet.Cells[i + 3, 3] = products[i].Price;
                    worksheet.Cells[i + 3, 4] = products[i].Quantity;
                    worksheet.Cells[i + 3, 5] = products[i].Total;
                }
                value += 1;

                worksheet = workbook.Worksheets[2];
                DateTime date1 = date.StartOfWeek(DayOfWeek.Saturday);
                DateTime date2 = date1.AddDays(6);
                date1 = date1.AddDays(-1);
                date2 = date2.AddDays(1);
                string startDate = date1.ToString("yyyy-MM-dd");
                string endDate = date2.ToString("yyyy-MM-dd");
                worksheet.Cells[1, 1] = "SEMANAL - " + startDate + " - " + endDate;
                products = Relatorio.Product(startDate, endDate);
                value += 1;
                for (int i = 0; i < products.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = products[i].Cod;
                    worksheet.Cells[i + 3, 2] = products[i].Name;
                    worksheet.Cells[i + 3, 3] = products[i].Price;
                    worksheet.Cells[i + 3, 4] = products[i].Quantity;
                    worksheet.Cells[i + 3, 5] = products[i].Total;
                }
                value += 1;

                worksheet = workbook.Worksheets[3];
                string month = date.ToString("yyyy-MM");
                worksheet.Cells[1, 1] = "MENSAL - " + month;
                products = Relatorio.Product(month);
                value += 1;
                for (int i = 0; i < products.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = products[i].Cod;
                    worksheet.Cells[i + 3, 2] = products[i].Name;
                    worksheet.Cells[i + 3, 3] = products[i].Price;
                    worksheet.Cells[i + 3, 4] = products[i].Quantity;
                    worksheet.Cells[i + 3, 5] = products[i].Total;
                }
                value += 1;

                worksheet = workbook.Worksheets[4];
                string year = date.ToString("yyyy");
                worksheet.Cells[1, 1] = "DIÁRIO - " + year;
                products = Relatorio.Product(year);
                value += 1;
                for (int i = 0; i < products.Count; i++)
                {
                    worksheet.Cells[i + 3, 1] = products[i].Cod;
                    worksheet.Cells[i + 3, 2] = products[i].Name;
                    worksheet.Cells[i + 3, 3] = products[i].Price;
                    worksheet.Cells[i + 3, 4] = products[i].Quantity;
                    worksheet.Cells[i + 3, 5] = products[i].Total;
                }
                value += 1;

                workbook.Save();
                workbook.Close(true, misValue, misValue);
                value += 1;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.Close()));

                Process.Start(fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                app.Quit();
            }
        }

        private void FileDialog_FileOk(object sender, CancelEventArgs e)
        {
            ok = true;
        }

    }
}
