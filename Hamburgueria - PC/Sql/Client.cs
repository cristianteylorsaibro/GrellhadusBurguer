using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Hamburgueria.Sql
{
    public class Client : IDisposable
    {
        private readonly Connection con;

        public Client()
        {
            con = new Connection();
        }

        public void AddOrUpdate(Tables.Client client)
        {
            con.Clients.AddOrUpdate(client);
            con.SaveChanges();
        }

        public List<Tables.Client> Select()
        {
            return con.Clients.Where(c => c.Deleted == false).AsNoTracking().ToList();
        }

        public List<Tables.Client> Select(string text)
        {
            return con.Clients.Where(c => c.Deleted == false && c.Name.Contains(text)).AsNoTracking().ToList();
        }

        public bool Exist(string name, string address, int number, string district)
        {
            return con.Clients.SingleOrDefault(c => c.Name == name && c.Street == address && c.Number == number && c.District == district && c.Deleted == false) == null ? false : true;
        }

        public void Delete(int id)
        {
            var client = con.Clients.SingleOrDefault(c => c.Id == id);
            client.Deleted = true;
            con.SaveChanges();
        }

        void IDisposable.Dispose()
        {
            con.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
