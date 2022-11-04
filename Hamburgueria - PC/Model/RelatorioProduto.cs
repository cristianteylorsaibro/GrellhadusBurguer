using System.Collections.Generic;

using System.Data.SQLite;
using System.Windows;
namespace Hamburgueria.Model
{
    partial class Relatorio : Database
    {
        public class P
        {
            public int Cod { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal Total { get; set; }

            public P(int cod, string name, decimal price, int quantity, decimal total)
            {
                this.Cod = cod;
                this.Name = name;
                this.Price = price;
                this.Quantity = quantity;
                this.Total = total;
            }
        }

        public static List<P> Product(string date)
        {
            List<P> p = new List<P>();

            connection.Open();

            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = "" +
                "SELECT p.cod, pv.nome, pv.preco, sum(pv.quantidade), sum(pv.total) FROM venda v " +
                "INNER JOIN produto_venda pv ON v.id = pv.venda_id " +
                "INNER JOIN produto p ON p.id = pv.produto_id " +
                "WHERE v.date like '" + date + "%' " +
                "GROUP BY pv.nome;";

            var r = command.ExecuteReader();
            while (r.Read())
                p.Add(new Relatorio.P(r.GetInt32(0), r.GetString(1), r.GetDecimal(2), r.GetInt32(3), r.GetDecimal(4)));
            r.Close();

            connection.Close();

            return p;
        }

        public static List<P> Product(string startDate, string endDate)
        {
            List<P> p = new List<P>();

            connection.Open();

            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = "" +
                "SELECT p.cod, pv.nome, pv.preco, sum(pv.quantidade), sum(pv.total) FROM venda v " +
                "INNER JOIN produto_venda pv ON v.id = pv.venda_id " +
                "INNER JOIN produto p ON p.id = pv.produto_id " +
                "WHERE v.date BETWEEN '" + startDate + "%' and '" + endDate + "%' " +
                "GROUP BY pv.nome;";

            var r = command.ExecuteReader();
            while (r.Read())
                p.Add(new Relatorio.P(r.GetInt32(0), r.GetString(1), r.GetDecimal(2), r.GetInt32(3), r.GetDecimal(4)));
            r.Close();

            connection.Close();

            return p;
        }
    }
}
