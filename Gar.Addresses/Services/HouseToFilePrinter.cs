using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gar.Addresses
{
    /// <summary>
    /// Печать house в файл
    /// </summary>
    internal class HouseToFilePrinter : IHousePrinter, IDisposable
    {
        StreamWriter _streamWriter;
        public HouseToFilePrinter(string outFile)
        {
            _streamWriter = new StreamWriter(outFile, true, Encoding.UTF8, 65536);
        }

        public void Dispose()
        {
            _streamWriter.Dispose();
        }

        public void Print(House house)
        {
            _streamWriter.WriteLine(GetHouseText(house));
        }

        /// <summary>
        /// Получить текст для дома
        /// </summary>
        /// <param name="house"></param>
        /// <returns></returns>
        string GetHouseText(House house)
        {
            List<string> list = new List<string>();
            string index = house.Index;
            var node = house as BaseNode;
            list.Add($"{house.TypeName} {house.Name}");
            while (node.Parent != null)
            {
                node = node.Parent;
                list.Add($"{node.TypeName}. {node.Name}");
            }
            list.Reverse();

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(index))
                sb.Append($"{index}, ");
            sb.Append(string.Join(", ", list.ToArray()));
            if (!string.IsNullOrEmpty(house.Addnum))
                sb.Append($", к. {house.Addnum}");
            sb.Append("|");
            sb.Append(house.Guid);
            return sb.ToString();
        }
    }
}
