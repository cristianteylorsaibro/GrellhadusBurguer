using Hamburgueria.Model;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using m_Excel = Microsoft.Office.Interop.Excel;

namespace Hamburgueria
{
    public class Excel
    {
        private m_Excel.Application app;
        private m_Excel.Workbook workbook;
        private m_Excel.Worksheet worksheet;
        private readonly object misValue = Missing.Value;

        private bool ok = false;

        public void SaleDay(string date)
        {
            ok = false;

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Exportar para Excel";
            fileDialog.FileName = "Vendas " + date;
            fileDialog.DefaultExt = "*.xlsx";
            fileDialog.Filter = "*.xlsx | *.xlsx";
            fileDialog.FileOk += FileDialog_FileOk;
            fileDialog.ShowDialog();

            if (ok == false)
                return;

            string fileName = fileDialog.FileName;
            try
            {
                File.Copy("Excel\\testingModel.xlsx", fileName, true);
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
                //Relatorio.SaleDayDelivery(date);
                //Relatorio.SaleDayLocal(date)

                for (int i = 0; i < all.Count; i++)
                {
                    worksheet.Cells[i + 2, 1] = all[i].TYPE;
                    worksheet.Cells[i + 2, 2] = all[i].Date;
                    worksheet.Cells[i + 2, 3] = all[i].TotalBrute;
                    worksheet.Cells[i + 2, 4] = all[i].Discount;
                    worksheet.Cells[i + 2, 5] = all[i].Total;
                    worksheet.Cells[i + 2, 6] = all[i].Payment;
                }

                workbook.Save();
                workbook.Close(true, misValue, misValue);

                Process.Start(fileName);
            }
            catch
            {
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
