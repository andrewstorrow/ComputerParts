using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using ComputerParts.Models;
using ComputerParts.Data;
using Newtonsoft.Json;

namespace ComputerParts
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Computer Parts!");

            DataContextDapper dapper = new DataContextDapper();

            DateTime rightNow = dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
            Console.WriteLine(rightNow);

            DataContextEF entityFramework = new DataContextEF();

            IEnumerable<Computer>? computers = entityFramework.Computers?.ToList<Computer>();

            if (computers != null)
            {
                foreach (Computer computer in computers)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(computer, Formatting.Indented));
                }
            }
        }
    }
}