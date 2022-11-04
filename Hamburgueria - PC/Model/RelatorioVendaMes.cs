using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;

namespace Hamburgueria.Model
{
    partial class Relatorio : Database
    {
        public class VendaMes
        {
            public string Day { get; set; }
            public decimal TotalBrute { get; set; }
            public decimal Discount { get; set; }
            public decimal Total { get; set; }

            public VendaMes(string day, decimal totalBrute, decimal discount, decimal total)
            {
                this.Day = day;
                this.TotalBrute = totalBrute;
                this.Discount = discount;
                this.Total = total;
            }
        }
        
        public static List<VendaMes> SaleMonth(string date)
        {
            List<VendaMes> s = new List<VendaMes>();

            connection.Open();

            for (int i = 1; i < 33; i++)
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
                    s.Add(new VendaMes(i.ToString("D2"), tb, d, t));
                }
                r.Close();
            }

            connection.Close();

            return s;
        }
        
        public static List<VendaMes> SaleMonthLocal(string date)
        {
            List<VendaMes> s = new List<VendaMes>();

            connection.Open();

            for (int i = 1; i < 33; i++)
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
                    s.Add(new VendaMes(i.ToString("D2"), tb, d, t));
                }
                r.Close();
            }

            connection.Close();

            return s;
        }
        
        public static List<VendaMes> SaleMonthDelivery(string date)
        {
            List<VendaMes> s = new List<VendaMes>();

            connection.Open();

            for (int i = 1; i < 33; i++)
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
                    s.Add(new VendaMes(i.ToString("D2"), tb, d, t));
                }
                r.Close();
            }

            connection.Close();

            return s;
        }
    }
}
