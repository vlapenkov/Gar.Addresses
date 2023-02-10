using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gar.Addresses
{
    class Program
    {

        static async Task GenerateFile(AddrParser parser, string outFile)
        {
            DateTime sdate = DateTime.Now;
            string guidstr = "a84b2ef4-db03-474b-b552-6229e801ae9b";// Яр.область



            var rootNode = await parser.GetNodeByFias(Guid.Parse(guidstr));

            using (var printer = new HouseToFilePrinter(outFile))

            {
                var logic = new NodeTreeIterator(parser, rootNode);

                await logic.FillChildren();

                var houses = logic.GetHouses();

                foreach (var house in houses)
                    printer.Print(house);
            }

            DateTime edate = DateTime.Now;

            Console.WriteLine($"{(edate - sdate).TotalSeconds} ==========Поиск окончен===========");

            Console.WriteLine($"Сгенерирован файл {outFile}");

            Console.ReadKey();
        }
        static async Task Main(string[] args)
        {

            var builder = new ConfigurationBuilder()
         .SetBasePath(@"C:\Users\vlape\source\repos\Gar.Addresses\Gar.Addresses")
         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
         .AddEnvironmentVariables();

            var config = builder.Build();

            var settings = config.GetSection("Settings").Get<Settings>();

            var parser = new DictAddrParser(settings);

            parser.FillDictionaries();

            await GenerateFile(parser, Path.Combine(settings.Directory, settings.OutputFilename));

        }
    }
}
