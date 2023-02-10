using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gar.Addresses
{
    public class NodeTreeLogic
    {
        private AddrParser _parser;
        //private Node _rootNode;
        private IHousePrinter _printer;

        public NodeTreeLogic(AddrParser parser, IHousePrinter printer)
        {
            _parser = parser;
            //_rootNode = rootNode;
            _printer = printer;
        }




        /// <summary>
        /// Обойти дерево
        /// </summary>

        public void BypassTree(Node node)
        {

            if (node.Children.Any())
            {
                foreach (var child in node.Children)
                    BypassTree(child);
            }

            if (node.Houses.Any())
                foreach (var house in node.Houses)
                    _printer.Print(house);
          

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
