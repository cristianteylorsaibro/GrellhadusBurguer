using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;
using System.Windows;

namespace Hamburgueria.Model
{
    partial class Relatorio : Database
    {
        public class VendaSemanal
        {
            public string Date { get; set; }
            public decimal TotalBrute { get; set; }
            public decimal Discount { get; set; }
            public decimal Total { get; set; }

            public VendaSemanal(string date, decimal totalBrute, decimal discount, decimal total)
            {
                this.Date = date;
                this.TotalBrute = totalBrute;
                this.Discount = discount;
                this.Total = total;
            }
        }

        public static List<VendaSemanal> SaleWeek(string dateStart, string dateEnd)
        {
            List<VendaSemanal> s = new List<VendaSemanal>();

            connection.Open();

            DateTime start = Convert.ToDateTime(dateStart);
            DateTime end = Convert.ToDateTime(dateEnd);

            while (start <= end)
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = "SELECT Sum(v.total_bruto), SUM(v.desconto), SUM(v.total) from venda v " +
                    "WHERE v.date like '" + start.ToString("yyyy-MM-dd") + "%';";

                var r = command.ExecuteReader();
                while (r.Read())
                {
                    decimal tb = r.IsDBNull(0) ? 0 : r.GetDecimal(0);
                    decimal d = r.IsDBNull(1) ? 0 : r.GetDecimal(1);
                    decimal t = r.IsDBNull(2) ? 0 : r.GetDecimal(2);
                    s.Add(new VendaSemanal(start.ToShortDateString(), tb, d, t));
                }
                r.Close();

                start = start.AddDays(1);
            }
            connection.Close();

            return s;
        }

        public static List<VendaSemanal> SaleWeekDelivery(string dateStart, string dateEnd)
        {
            List<VendaSemanal> s = new List<VendaSemanal>();

            connection.Open();

            DateTime start = Convert.ToDateTime(dateStart);
            DateTime end = Convert.ToDateTime(dateEnd);

            while (start <= end)
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = "SELECT Sum(v.total_bruto), SUM(v.desconto), SUM(v.total) from venda v " +
                    "INNER JOIN venda_delivery dv ON v.id == dv.venda_id " +
                    "WHERE v.date like '" + start.ToString("yyyy-MM-dd") + "%';";

                var r = command.ExecuteReader();
                while (r.Read())
                {
                    decimal tb = r.IsDBNull(0) ? 0 : r.GetDecimal(0);
                    decimal d = r.IsDBNull(1) ? 0 : r.GetDecimal(1);
                    decimal t = r.IsDBNull(2) ? 0 : r.GetDecimal(2);
                    s.Add(new VendaSemanal(start.ToShortDateString(), tb, d, t));
                }
                r.Close();

                start = start.AddDays(1);
            }
            connection.Close();

            return s;
        }

        public static List<VendaSemanal> SaleWeekLocal(string dateStart, string dateEnd)
        {
            List<VendaSemanal> s = new List<VendaSemanal>();

            connection.Open();

            DateTime start = Convert.ToDateTime(dateStart);
            DateTime end = Convert.ToDateTime(dateEnd);

            while (start <= end)
            {
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = "SELECT Sum(v.total_bruto), SUM(v.desconto), SUM(v.total) from venda v " +
                    "INNER JOIN venda_mesa vm ON v.id == vm.venda_id " +
                    "WHERE v.date like '" + start.ToString("yyyy-MM-dd") + "%';";

                var r = command.ExecuteReader();
                while (r.Read())
                {
                    decimal tb = r.IsDBNull(0) ? 0 : r.GetDecimal(0);
                    decimal d = r.IsDBNull(1) ? 0 : r.GetDecimal(1);
                    decimal t = r.IsDBNull(2) ? 0 : r.GetDecimal(2);
                    s.Add(new VendaSemanal(start.ToShortDateString(), tb, d, t));
                }
                r.Close();

                start = start.AddDays(1);
            }
            connection.Close();

            return s;
        }
    }
}
