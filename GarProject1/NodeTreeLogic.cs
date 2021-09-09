using GarProject1.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarProject1
{
    public class NodeTreeLogic
    {
        private AddrParser _parser;
        private Node _rootNode;

        public NodeTreeLogic(AddrParser parser, Node rootNode)
        {
            _parser = parser;
            _rootNode = rootNode;
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
            list.Add($"{house.TypeName} { house.Name}");
            while (node.Parent != null)
            {
                node = node.Parent;
                list.Add($"{node.TypeName}. { node.Name}");
            }
            list.Reverse();

            var sb = new StringBuilder();
            //sb.Append(house.Id);
            //sb.Append("|");
            if (!String.IsNullOrEmpty(index))
                sb.Append($"{index}, ");
            sb.Append(String.Join(", ", list.ToArray()));
            if (!String.IsNullOrEmpty(house.Addnum))
                sb.Append($", к. {house.Addnum}");
            sb.Append("|");
            sb.Append(house.Guid);
            return sb.ToString();
        }


        /// <summary>
        /// Обойти дерево
        /// </summary>

        public void BypassTree(Node node, StreamWriter stream)
        {
            //if (node is House)
            //{
            //    stream.WriteLine(GetHouseText(node as House));
            //    // Console.WriteLine(GetHouseText(node as House));
            //    return;
            //}

            if (node.Children.Any())
            {
                foreach (var child in node.Children)
                    BypassTree(child, stream);
            }
            if (node.Houses.Any())
                foreach (var house in node.Houses)
                    stream.WriteLine(GetHouseText(house));
            //        BypassTree(house, stream);

        }

        /// <summary>
        /// Заполнить потомков при обходе дерева
        /// </summary>        
        public async Task FillChildren(Node node)
        {
            var elementIds = await _parser.GetNodeLinks(node.Id);
            var childNodes = await _parser.GetNodes(elementIds);
            var houses = await _parser.GetHouses(elementIds);

            if (houses.Any())
            {
                node.Houses = houses;
                foreach (var house in node.Houses)
                {
                    house.Level = node.Level + 1;
                    house.Parent = node;
                }
            }

            if (node.Level == 1)
                Console.WriteLine(node);

            if (childNodes.Any())
            {
                foreach (var childNode in childNodes)
                {
                    childNode.Level = node.Level + 1;
                    childNode.Parent = node;
                }
                node.Children = childNodes.OrderBy(p => p.Name).ToList();

                foreach (var childnode in node.Children)
                    await FillChildren(childnode);
            }


        }



    }
}
