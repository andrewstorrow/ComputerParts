using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using ComputerParts.Models;
using ComputerParts.Data;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace ComputerParts
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Computer Parts!");

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            DataContextDapper dapper = new DataContextDapper(config);
            DataContextEF entityFramework = new DataContextEF(config);

            DateTime rightNow = dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
            Console.WriteLine(rightNow);

            //Write to log file from database
            Console.WriteLine("--Getting Computers from database, storing in log.txt--");
            using StreamWriter openFile = new(path: "log.txt", append: true);
            IEnumerable<Computer>? computers = entityFramework.Computers?.ToList<Computer>();
            if (computers != null)
            {
                openFile.WriteLine("--Computers in Database at: "
                 + dapper.LoadDataSingle<DateTime>("SELECT GETDATE()").ToString()
                 + "--"
                 );
                foreach (Computer computer in computers)
                {
                    string computerJson = JsonConvert.SerializeObject(computer, Formatting.Indented);
                    openFile.WriteLine(computerJson);
                }
            }
            openFile.Close();

            //Read computer objects from JSON file
            string computersJson = File.ReadAllText("Computers.json");
            computers = JsonConvert.DeserializeObject<IEnumerable<Computer>>(computersJson);
            if (computers != null)
            {
                Console.WriteLine("--Getting Computers from file, adding to database--");
                foreach (Computer computer in computers)
                {
                    string sql = @"INSERT INTO TutorialAppSchema.Computer (
                        Motherboard,
                        HasWifi,
                        HasLTE,
                        ReleaseDate,
                        Price,
                        VideoCard
                    ) VALUES ('" + EscapeSingleQuote(computer.Motherboard)
                            + "','" + computer.HasWifi
                            + "','" + computer.HasLTE
                            + "','" + computer.ReleaseDate
                            + "','" + computer.Price
                            + "','" + EscapeSingleQuote(computer.VideoCard)
                    + "')";

                    dapper.ExecuteSqlWithRowCount(sql);
                }
            }
        }

        static string EscapeSingleQuote(string input)
        {
            string output = input.Replace("'","''");

            return output;
        }
    }
}