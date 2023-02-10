using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gar.Addresses
{
    /// <summary>
    /// Любой адресный объект (включая дома)
    /// </summary>
    public class Node : BaseNode
    {

        /// <summary>
        /// Дочерние адресные объекты
        /// </summary>
        public ICollection<Node> Children { get; set; } = new List<Node>();

        /// <summary>
        /// Дома
        /// </summary>
        public ICollection<House> Houses { get; set; } = new List<House>();


    }
}
