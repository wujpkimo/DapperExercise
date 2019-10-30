using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DapperExercise
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=dapperExercise;Integrated Security=true;");

            //DapperInsertMethod(conn);
            var data = DaperMethod(conn);
            //AdoMethod(conn);
            List<string> to = new List<string>() { };
            SendMail(data.Where(s => s.SysId > 20).ToList(), "EMEA", to);
            SendMail(data.Where(s => s.SysId > 10).ToList(), "China", to);
        }

        private static void DapperInsertMethod(SqlConnection conn)
        {
            int result = 0;
            using (conn)
            {
                var list =
                    new[] {
                        new { id = 81, desc = "Taiwan", remark="Test" },
                        new { id = 86, desc = "Mars" ,remark="Test2"},
                        new { id = 87, desc = "Mars" ,remark="Test2"},
                        new { id = 98, desc = "Mars" ,remark="Test2"},
                        new { id = 89, desc = "Mars" ,remark="Test2"},
                        new { id = 90, desc = "Mars" ,remark="Test2"}
                    };

                try
                {
                    result = conn.Execute(@"INSERT INTO Product VALUES (@id, @desc, @remark)", list);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Console.WriteLine($"inset {result} datas");
                }
            }
        }

        public static void SendMail(List<Product> products, string Region, List<string> ToList)
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"Start to execute {Region}");
            foreach (var item in products)
            {
                foreach (var vProperty in typeof(Product).GetProperties())
                {
                    Console.Write($"{vProperty.Name}:{vProperty.GetValue(item)} ");
                }
                Console.Write("\n");
            }
        }

        private static List<Product> DaperMethod(SqlConnection conn)
        {
            var vIndex = 3;
            var vId = "ID1";

            var obj = new { id = vId, index = vIndex };

            List<Product> list = new List<Product>();
            using (conn)
            {
                //var list = conn.Query<Product>("select * from Product where ProductId=@id or SysId = @index"
                //, obj);
                list = conn.Query<Product>("select * from Product ", obj).ToList();
                foreach (var item in list)
                {
                    Console.WriteLine($"{item.ProductId} , {item.ProductDesc} , {item.Remark}");
                }
                //Console.WriteLine(list.Where(a => a.SysId == 2).FirstOrDefault().ProductDesc);
            }

            Console.ReadLine();
            return list;
        }

        public class Product
        {
            public int SysId { get; set; }

            public string ProductId { get; set; }
            public string ProductDesc { get; set; }
            public string Remark { get; set; }
        }

        private static void AdoMethod(SqlConnection conn)
        {
            using (conn)
            {
                SqlCommand sql = new SqlCommand("select * from Product", conn);
                try
                {
                    conn.Open();
                    SqlDataReader reader = sql.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine("\t{0}\t{1}\t{2}",
                            reader[0], reader[1], reader[2]);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.ReadLine();
            }
            Console.WriteLine("Hello World!");
        }
    }
}