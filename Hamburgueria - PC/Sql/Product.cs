using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Hamburgueria.Sql
{
    public class Product : IDisposable
    {
        private Connection con;

        public Product()
        {
            con = new Connection();
        }

        public void AddOrUpdate(Tables.Product product)
        {
            con.Products.AddOrUpdate(product);
            con.SaveChanges();
        }

        public void Delete(int id)
        {
            var product = con.Products.SingleOrDefault(p => p.Id == id);
            product.Deleted = true;
            con.SaveChanges();
        }

        public bool Exist(int cod)
        {
            return con.Products.SingleOrDefault(p => p.Cod == cod && p.Deleted == false) == null ? false : true;
        }

        public List<Tables.Product> Select()
        {
            return con.Products.Where(p => p.Deleted == false).AsNoTracking().ToList();
        }

        public List<Tables.Product> Select(string text)
        {
            return con.Products.Where(p => p.Deleted == false && p.Name.Contains(text)).AsNoTracking().ToList();
        }

        public List<Tables.Product> Select(int cod)
        {
            return con.Products.Where(p => p.Deleted == false && p.Cod == cod).AsNoTracking().ToList();
        }

        public Tables.Product GetProduct(int id)
        {
            return con.Products.SingleOrDefault(p => p.Id == id);
        }

        void IDisposable.Dispose()
        {
            con.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
