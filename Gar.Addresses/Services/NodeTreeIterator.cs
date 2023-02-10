using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gar.Addresses
{
    public class NodeTreeIterator
    {
        private AddrParser _parser;
        private Node _rootNode;



        public NodeTreeIterator(AddrParser parser, Node rootNode)
        {
            _parser = parser;
            _rootNode = rootNode;

        }

        public async Task FillChildren() => await FillChildren(_rootNode);


        /// <summary>
        /// Обойти дерево, получить дома
        /// </summary>
        public IEnumerable<House> GetHouses()
        {
            IEnumerable<House> GetHouses(Node node)
            {

                if (node.Children.Any())
                {
                    foreach (var child in node.Children)
                        foreach (var house in GetHouses(child))
                            yield return house;
                }

                if (node.Houses.Any())
                    foreach (var house in node.Houses)
                        yield return house;

            }

            return GetHouses(_rootNode);
        }

        /// <summary>
        /// Заполнить от рутового узла (области) до домов рекурсивно
        /// </summary>        
        private async Task FillChildren(Node node)
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
