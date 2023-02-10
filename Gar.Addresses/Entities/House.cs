using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gar.Addresses
{

    /// <summary>
    /// Дом
    /// </summary>
    public class House : BaseNode
    {
        public string Index { get; set; }

        public string Addnum { get; set; } // корпус если есть



        public override string ToString()
        {
            return $"{Index} " + base.ToString();
        }

    }
}
