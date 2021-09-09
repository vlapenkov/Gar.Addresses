using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarProject1
{
    class Program
    {


        #region not_used
        static async Task Compare(string[] files)
        {
            Dictionary<int, int> keys = new Dictionary<int, int>();

            var comparers = files.Select(filePath => new DistinctComparer(filePath)).ToArray();

            Console.WriteLine(comparers.Length);



            for (int i = 0; i < comparers.Length; i++)
            {
                var items = await comparers[i].GetAllNodes("HOUSE"); //"OBJECT"

                foreach (var item in items)
                {
                    keys.Add(item, item);
                }
            }


            Console.WriteLine(keys.Count());
            Console.ReadKey();
        }


        static async Task DoJob1()
        {
            DateTime sdate = DateTime.Now;
            string guidstr = "a84b2ef4-db03-474b-b552-6229e801ae9b";// Яр.область
            guidstr = "4e864448-447c-49ef-83f6-c9730cbfb809";  // некр. район

            //guidstr = "58888e78-2ec8-4424-bcfa-20462aa5928c"; // красный профинтерн
            //guidstr = "7c9f341d-5497-42e5-a8a7-b759849e2a98"; // ул. Советская

            //guidstr = "6b1bab7d-ee45-4168-a2a6-4ce2880d90d3"; // Ярославль
            //guidstr = "e98fbb42-bc8b-4067-bfa0-fd1957b35410"; // Ленинградский пр-т

            var parser = new AddrParser(
                 @"C:\Users\vlape\Downloads\gar_xml\76",
                 "AS_ADDR_OBJ_20210830_c7ef2261-0b6d-4c02-aa8a-7ebf01f7f5f3.XML",
                 "AS_ADM_HIERARCHY_20210830_ad888668-bf12-41ec-8c4d-b55abce16262.XML",
                 "AS_HOUSES_20210830_bdfc07e9-4802-49b0-84e3-50719e43c1e7.XML",
                 "AS_HOUSES_PARAMS_20210830_d8c9e82e-2626-4057-a2d3-485e0034b6b2.XML"
                 );

            var rootNode = await parser.GetNodeId(Guid.Parse(guidstr));

            var elementIds = await parser.GetNodeLinks(rootNode.Id);
            var nodes = (await parser.GetNodes(elementIds)).OrderBy(parser => parser.Name).ToArray();



            DateTime edate = DateTime.Now;

            Console.WriteLine($"{(edate - sdate).TotalSeconds} ==========Поиск окончен===========");
            Console.ReadKey();
        }
        #endregion

        static async Task GenerateFileForBulkInsert(AddrParser parser, string outFile)
        {
            DateTime sdate = DateTime.Now;
            string guidstr = "a84b2ef4-db03-474b-b552-6229e801ae9b";// Яр.область
             guidstr = "b6ba5716-eb48-401b-8443-b197c9578734";// Заб. край


            var rootNode = await parser.GetNodeId(Guid.Parse(guidstr));

            var logic = new NodeTreeLogic(parser, rootNode);

            await logic.FillChildren(rootNode);

            using (StreamWriter streamwriter = new StreamWriter(outFile, true, Encoding.UTF8, 65536))
                logic.BypassTree(rootNode, streamwriter);



            DateTime edate = DateTime.Now;

            Console.WriteLine($"{(edate - sdate).TotalSeconds} ==========Поиск окончен===========");
            Console.ReadKey();
        }
        static async Task Main(string[] args)
        {
            var parser = new DictAddrParser(args);

            await GenerateFileForBulkInsert(parser, Path.Combine(args[0], args.Last()));

        }
    }
}
