using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using System.Data.SQLite;

namespace Hamburgueria.Model
{
    partial class Relatorio : Database
    {
        public class VendaAnual
        {
            public string Month { get; set; }
            public decimal TotalBrute { get; set; }
            public decimal Discount { get; set; }
            public decimal Total { get; set; }

            public VendaAnual(string month, decimal totalBrute, decimal discount, decimal total)
            {
                this.Month = month;
                this.TotalBrute = totalBrute;
                this.Discount = discount;
                this.Total = total;
            }
        }
        
        public static List<VendaAnual> SaleYear(string date)
        {
            List<VendaAnual> s = new List<VendaAnual>();

            connection.Open();

            for (int i = 1; i <= 12; i++)
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = "SELECT Sum(v.total_bruto), SUM(v.desconto), SUM(v.total) from venda v " +
                    "WHERE v.date like '" + date + i.ToString("D2") + "%';";

                var r = command.ExecuteReader();
                while (r.Read())
                {
                    decimal tb = r.IsDBNull(0) ? 0 : r.GetDecimal(0);
                    decimal d = r.IsDBNull(1) ? 0 : r.GetDecimal(1);
                    decimal t = r.IsDBNull(2) ? 0 : r.GetDecimal(2);
                    string month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).ToUpper();
                    s.Add(new VendaAnual(month, tb, d, t));
                }
                r.Close();
            }

            connection.Close();

            return s;
        }
        
        public static List<VendaAnual> SaleYearLocal(string date)
        {
            List<VendaAnual> s = new List<VendaAnual>();

            connection.Open();

            for (int i = 1; i <= 12; i++)
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = "SELECT Sum(v.total_bruto), SUM(v.desconto), SUM(v.total) from venda_mesa vm " +
                    "INNER JOIN venda v ON vm.venda_id = v.id " +
                    "WHERE v.date like '" + date + i.ToString("D2") + "%';";

                var r = command.ExecuteReader();
                while (r.Read())
                {
                    decimal tb = r.IsDBNull(0) ? 0 : r.GetDecimal(0);
                    decimal d = r.IsDBNull(1) ? 0 : r.GetDecimal(1);
                    decimal t = r.IsDBNull(2) ? 0 : r.GetDecimal(2);
                    string month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).ToUpper();
                    s.Add(new VendaAnual(month, tb, d, t));
                }
                r.Close();
            }

            connection.Close();

            return s;
        }
        
        public static List<VendaAnual> SaleYearDelivery(string date)
        {
            List<VendaAnual> s = new List<VendaAnual>();

            connection.Open();

            for (int i = 1; i <= 12; i++)
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = "SELECT Sum(v.total_bruto), SUM(v.desconto), SUM(v.total) from venda_delivery vd " +
                    "INNER JOIN venda v ON vd.venda_id = v.id " +
                    "WHERE v.date like '" + date + i.ToString("D2") + "%';";

                var r = command.ExecuteReader();
                while (r.Read())
                {
                    decimal tb = r.IsDBNull(0) ? 0 : r.GetDecimal(0);
                    decimal d = r.IsDBNull(1) ? 0 : r.GetDecimal(1);
                    decimal t = r.IsDBNull(2) ? 0 : r.GetDecimal(2);
                    string month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i).ToUpper();
                    s.Add(new VendaAnual(month, tb, d, t));
                }
                r.Close();
            }

            connection.Close();

            return s;
        }
    }
}
