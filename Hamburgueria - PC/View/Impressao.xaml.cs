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
using System.Drawing;
using System.IO;
using System.Drawing.Printing;

namespace Hamburgueria.View
{
    /// <summary>
    /// Lógica interna para Impressao.xaml
    /// </summary>
    public partial class Impressao : Window
    {
        private static StreamReader arquivoParaImprimir;
        private Font _font;

        public Impressao()
        {
            InitializeComponent();

            this.Loaded += Impressao_Loaded;

            this.printsList.SelectionChanged += PrintsList_SelectedIndexChanged;

            this.print.Click += Print_Click;
            this.cancel.Click += Cancel_Click;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void Impressao_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string printerName in PrinterSettings.InstalledPrinters)
                printsList.Items.Add(printerName);

            Properties.Settings.Default.Reload();
            int index = Properties.Settings.Default.printIndex;
            printsList.SelectedIndex = index >= printsList.Items.Count ? 0 : index;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Print_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = "Impressao-Hamburgueria" + TXT.IdFile + ".txt";
                string printerName = printsList.Text;

                arquivoParaImprimir = new StreamReader(TXT.Path() + "\\sale" + TXT.IdFile + ".txt");
                var pd = new PrintDocument();
                _font = new Font("Consolas", 9f);
                pd.PrinterSettings.PrinterName = printerName;
                pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                pd.DocumentName = fileName;
                pd.PrintPage += PD_PrintPage;
                pd.Print();

                TXT.IdFile++;

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PD_PrintPage(object sender, PrintPageEventArgs e)
        {
            float linhasPorPagina;
            float PosicaoY = 0;
            int count = 0;
            float MargemEsquerda = e.MarginBounds.Left;
            float MargemTopo = e.MarginBounds.Top;
            string Linha = null;

            linhasPorPagina = e.MarginBounds.Height / _font.GetHeight(e.Graphics);

            while (count < linhasPorPagina && ((Linha = arquivoParaImprimir.ReadLine()) != null))
            {
                PosicaoY = MargemTopo + (count * _font.GetHeight(e.Graphics));
                e.Graphics.DrawString(Linha, _font, System.Drawing.Brushes.Black, MargemEsquerda, PosicaoY, new StringFormat());
                count++;
            }

            e.HasMorePages = Linha != null;
        }

        private void PrintsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.printIndex = printsList.SelectedIndex;
            Properties.Settings.Default.Save();
        }
    }
}
