using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace mySql
{
    class Program
    {
        static void Main(string[] args)
        {
            for(int i = 0;i<3;i++)
            {
                string name = Console.ReadLine();
                string num = Console.ReadLine();
                string cmd = "insert into book(姓名,电话号码) values('" + name + "','" + num + "')";
                MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, CommandType.Text, cmd, new MySqlParameter());
            }
            
            Console.ReadKey();
        }
    }
}
