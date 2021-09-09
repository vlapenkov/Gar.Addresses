using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarProject1.Entities
{

    public class BaseNode
    {
        /// <summary>
        /// Указатель на родителя
        /// </summary>
        public Node Parent { get; set; }

        /// <summary>
        /// Id идентификатор из файла
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ФИАС идентификатор из файла
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public string TypeName { get; set; }

        public int Level = 0;

        public override string ToString()
        {
            return $"{Id} {Guid} {TypeName} {Name} ";
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
