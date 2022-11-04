using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SQLite;

namespace Hamburgueria.Model
{
    public partial class Relatorio : Database
    {
        public class VendaDia
        {
            public string TYPE { get; set; }
            public DateTime Date { get; set; }
            public decimal TotalBrute { get; set; }
            public decimal Discount { get; set; }
            public decimal Total { get; set; }
            public string Payment { get; set; }

            public VendaDia(string type, DateTime date, decimal totalBrute, decimal discount, decimal total, string payment)
            {
                this.TYPE = type;
                this.Date = date;
                this.TotalBrute = totalBrute;
                this.Discount = discount;
                this.Total = total;
                this.Payment = payment;
            }
        }

        public static List<VendaDia> SaleDay(string date)
        {
            var dl = SaleDayLocal(date);
            var dd = SaleDayDelivery(date);

            for (int i = 0; i < dd.Count; i++)
                dl.Add(dd[i]);

            return dl;
        }

        public static List<VendaDia> SaleDayLocal(string date)
        {
            List<VendaDia> s = new List<VendaDia>();

            connection.Open();

            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = "" +
                "SELECT v.date, v.total_bruto, v.desconto, v.total, v.pagamento from venda_mesa vm " +
                "INNER JOIN venda v ON vm.venda_id = v.id " +
                "WHERE v.date like '" + date + "%' " +
                "GROUP BY v.id;";

            var r = command.ExecuteReader();
            while (r.Read())
                s.Add(new VendaDia("BALCÃO", r.GetDateTime(0), r.GetDecimal(1), r.GetDecimal(2), r.GetDecimal(3), r.GetString(4)));
            r.Close();

            connection.Close();

            return s;
        }

        public static List<VendaDia> SaleDayDelivery(string date)
        {
            List<VendaDia> s = new List<VendaDia>();

            connection.Open();

            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = "" +
                "SELECT v.date, v.total_bruto, v.desconto, v.total, v.pagamento from venda_delivery vd " +
                "INNER JOIN venda v ON vd.venda_id = v.id " +
                "WHERE v.date like '" + date + "%' " +
                "GROUP BY v.id;";

            var r = command.ExecuteReader();
            while (r.Read())
                s.Add(new VendaDia("DELIVERY", r.GetDateTime(0), r.GetDecimal(1), r.GetDecimal(2), r.GetDecimal(3), r.GetString(4)));
            r.Close();

            connection.Close();

            return s;
        }
    }
}
