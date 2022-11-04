using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Hamburgueria.Sales
{
    public class Log
    {
        private static string DefaultPath()
        {
            string pathData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TWO Sistemas";

            if (Directory.Exists(pathData) == false)
                pathData = Directory.CreateDirectory(pathData).FullName;

            pathData += "\\Log";
            if (Directory.Exists(pathData) == false)
                pathData = Directory.CreateDirectory(pathData).FullName;

            return pathData + "\\";
        }

        public static void Create(DateTime dateSale, int numTable, ObservableCollection<Item> items)
        {
            string path = DefaultPath() + dateSale.ToString("yyyy-MM-dd HH+mm+ss") + ".bin";

            string content = "BALCÃO\n";
            content += numTable + "\n";
            content += "-PRODUTOS-\n";
            foreach (Item it in items)
            {
                string observation = it.Name.Substring(new Sql.Product().GetProduct(it.Id).Name.Length + 1);
                content += it.Id + ">" + observation + ">" + it.Quantity + "\n";
            }

            File.WriteAllText(path, content, Encoding.UTF8);
        }

        public static void Create(DateTime dateSale, Tables.Client client, string payment, decimal discount, decimal valuePay, decimal change, ObservableCollection<Item> items)
        {
            string path = DefaultPath() + dateSale.ToString("yyyy-MM-dd HH+mm+ss") + ".bin";

            string content = "DELIVERY\n";
            content += client.Name + "\n";
            content += client.Street + "\n";
            content += client.Number + "\n";
            content += client.District + "\n";
            content += client.Complement + "\n";
            content += client.Telephone + "\n";
            content += client.Reference + "\n";
            content += payment + "\n";
            content += discount + "\n";
            content += valuePay + "\n";
            content += change + "\n";
            content += "-PRODUTOS-\n";
            foreach (Item it in items)
            {
                string observation = it.Name.Substring(new Sql.Product().GetProduct(it.Id).Name.Length + 1);
                content += it.Id + ">" + observation + ">" + it.Quantity + "\n";
            }

            File.WriteAllText(path, content, Encoding.UTF8);
        }

        public static void Select(ObservableCollection<View.Sale> sales, int index = 0)
        {
            sales.Clear();
            string[] files = Directory.GetFiles(DefaultPath(), "*.bin", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < files.Length; i++)
            {
                string[] lines = File.ReadAllLines(files[i]);
                string fileName = Path.GetFileNameWithoutExtension(files[i]);
                fileName = fileName.Replace('+', ':');
                DateTime dateSale = Convert.ToDateTime(fileName);

                if (lines[0].StartsWith("BALCÃO") && (index == 0 || index == 2))
                {
                    string numTable = lines[1];

                    string info = "MESA: Nº" + numTable + "\n\n";
                    decimal totalSale = 0;
                    for (int j = 3; j < lines.Length; j++)
                    {
                        string[] requests = lines[j].Split('>');

                        int id = Convert.ToInt32(requests[0]);
                        string obs = requests[1];
                        int quantity = Convert.ToInt32(requests[2]);

                        var p = new Sql.Product().GetProduct(id);
                        info += (p.Price * quantity).ToString("C2").PadRight(12, ' ') + quantity + "x " + p.Name + " " + obs + "\n";
                        totalSale += p.Price * quantity;
                    }

                    sales.Add(new View.Sale() { Type = 0, Value = "BALCÃO", File = numTable, Info = info, Date = dateSale, Total = totalSale });
                }
                else if (lines[0].StartsWith("DELIVERY") && (index == 0 || index == 1))
                {
                    string name = lines[1];
                    string street = lines[2];
                    string number = lines[3];
                    string district = lines[4];
                    string complement = lines[5];
                    string telephone = lines[6];
                    string reference = lines[7];
                    string payment = lines[8];
                    string discount = lines[9];
                    string valuePay = lines[10];
                    string change = lines[11];

                    string info = name + "\n\n";
                    info += "ENDEREÇO: " + street + ", Nº" + number + ", " + district + ", " + complement + "\n";
                    info += "TELEFONE: " + telephone + "\n";
                    info += "REFERÊNCIA: " + reference + "\n";
                    info += "FORMA DE PAGAMENTO: " + payment + "\n";
                    info += "DESCONTO: R$" + discount + "\n";

                    if (payment == "À VISTA")
                    {
                        info += "VALOR PAGO: R$" + valuePay + "\n";
                        info += "TROCO: R$" + change + "\n";
                    }

                    info += "\n";

                    info += "PEDIDOS\n";
                    decimal totalSale = 0;
                    for (int j = 13; j < lines.Length; j++)
                    {
                        string[] requests = lines[j].Split('>');

                        int id = Convert.ToInt32(requests[0]);
                        string obs = requests[1];
                        int quantity = Convert.ToInt32(requests[2]);

                        var p = new Sql.Product().GetProduct(id);
                        info += (p.Price * quantity).ToString("C2").PadRight(12, ' ') + quantity + "x " + p.Name + " " + obs + "\n";
                        totalSale += p.Price * quantity;
                    }

                    sales.Add(new View.Sale() { Type = 1, Value = "DELIVERY", File = name, Info = info, Date = dateSale, Total = totalSale - Convert.ToDecimal(discount) });
                }
            }

            sales.Rearrange();
        }

        public static ObservableCollection<Item> Products(DateTime dateSale)
        {
            ObservableCollection<Item> it = new ObservableCollection<Item>();

            string[] lines = File.ReadAllLines(DefaultPath() + dateSale.ToString("yyyy-MM-dd HH+mm+ss") + ".bin");

            int index = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("-PRODUTOS-"))
                {
                    index = i + 1;
                    break;
                }
            }

            for (int i = index; i < lines.Length; i++)
            {
                string[] requests = lines[i].Split('>');

                int id = Convert.ToInt32(requests[0]);
                string obs = requests[1];
                int quantity = Convert.ToInt32(requests[2]);

                var p = new Sql.Product().GetProduct(id);
                it.Add(new Item(id, p.Cod, p.Name + " " + obs, p.Price, quantity));
            }

            return it;
        }

        public static int NumTable(DateTime dateSale)
        {
            string[] lines = File.ReadAllLines(DefaultPath() + dateSale.ToString("yyyy-MM-dd HH+mm+ss") + ".bin");
            return Convert.ToInt32(lines[1]);
        }

        public static Tables.Client Client(DateTime dateSale)
        {
            string[] lines = File.ReadAllLines(DefaultPath() + dateSale.ToString("yyyy-MM-dd HH+mm+ss") + ".bin");
            Tables.Client c = new Tables.Client();
            c.Name = lines[1];
            c.Street = lines[2];
            c.Number = Convert.ToInt32(lines[3]);
            c.District = lines[4];
            c.Complement = lines[5];
            c.Telephone = lines[6];
            c.Reference = lines[7];

            return c;
        }

        public static string[] InfoDelivery(DateTime dateSale)
        {
            string[] lines = File.ReadAllLines(DefaultPath() + dateSale.ToString("yyyy-MM-dd HH+mm+ss") + ".bin");
            string[] info = new string[4];
            info[0] = lines[8];
            info[1] = lines[9];
            info[2] = lines[10];
            info[3] = lines[11];

            return info;
        }

        public static void Delete(DateTime dateSale)
        {
            File.Delete(DefaultPath() + dateSale.ToString("yyyy-MM-dd HH+mm+ss") + ".bin");
        }
    }
}
