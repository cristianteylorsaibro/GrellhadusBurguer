using System.Data.SQLite;

namespace Hamburgueria.Model
{
    public class Database
    {
        private static readonly string DATASOURCE = "DATASOURCE=hamburgueria.db";
        protected static SQLiteConnection connection = new SQLiteConnection(DATASOURCE);
    }
}
