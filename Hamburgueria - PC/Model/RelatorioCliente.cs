﻿using System.Collections.Generic;
using System.Data.SQLite;

namespace Hamburgueria.Model
{
    partial class Relatorio : Database
    {
        public class Cliente
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public decimal TotalBruto { get; set; }
            public decimal Desconto { get; set; }
            public decimal Total { get; set; }

            public Cliente(string name, string address, decimal totalBruto, decimal desconto, decimal total)
            {
                this.Name = name;
                this.Address = address;
                this.TotalBruto = totalBruto;
                this.Desconto = desconto;
                this.Total = total;
            }
        }

        public static List<Cliente> Client(string date)
        {
            List<Cliente> p = new List<Cliente>();

            connection.Open();

            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = "" +
                "SELECT v.date, vd.name, vd.address, v.total_bruto, v.desconto, v.total " +
                "FROM venda_delivery vd " +
                "INNER JOIN venda v ON v.id = vd.venda_id " +
                "WHERE v.date like '" + date + "%';";

            var r = command.ExecuteReader();
            while (r.Read())
            {
                p.Add(new Relatorio.Cliente(r.GetString(1), r.GetString(2), r.GetDecimal(3), r.GetDecimal(4), r.GetDecimal(5)));
            }
            r.Close();

            connection.Close();

            for (int i = 0; i < p.Count - 1; i++)
            {
                if (p[i].Name == p[i + 1].Name && p[i].Address == p[i + 1].Address)
                {
                    p[i].TotalBruto += p[i + 1].TotalBruto;
                    p[i].Desconto += p[i + 1].Desconto;
                    p[i].Total += p[i + 1].Total;

                    p.RemoveAt(i + 1);
                    i -= 1;
                }
            }

            return p;
        }

        public static List<Cliente> Client(string startDate, string endDate)
        {
            List<Cliente> p = new List<Cliente>();

            connection.Open();

            SQLiteCommand command = new SQLiteCommand(connection);
            command.CommandText = "" +
                "SELECT v.date, vd.name, vd.address, v.total_bruto, v.desconto, v.total, v.pagamento " +
                "FROM venda_delivery vd " +
                "INNER JOIN venda v ON v.id = vd.venda_id " +
                "WHERE v.date >= '" + startDate + "%' and v.date <= '" + endDate + "%';";

            var r = command.ExecuteReader();
            while (r.Read())
                p.Add(new Relatorio.Cliente(r.GetString(1), r.GetString(2), r.GetDecimal(3), r.GetDecimal(4), r.GetDecimal(5)));
            r.Close();

            connection.Close();

            for (int i = 0; i < p.Count - 1; i++)
            {
                if (p[i].Name == p[i + 1].Name && p[i].Address == p[i + 1].Address)
                {
                    p[i].TotalBruto += p[i + 1].TotalBruto;
                    p[i].Desconto += p[i + 1].Desconto;
                    p[i].Total += p[i + 1].Total;

                    p.RemoveAt(i + 1);
                    i -= 1;
                }
            }

            return p;
        }
    }
}