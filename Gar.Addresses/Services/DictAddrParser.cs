using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Gar.Addresses
{
    /// <summary>
    /// Сперва заполняем dictionaries , затем поиск из них
    /// </summary>
    public class DictAddrParser : AddrParser
    {

        Dictionary<int, int> _childToParentDict = new Dictionary<int, int>();

        ILookup<int, int> _lookup;

        IDictionary<int, House> _dictHouses = new Dictionary<int, House>();

        IDictionary<int, Node> _nodesDict = new Dictionary<int, Node>();


        public DictAddrParser(Settings settings) : base(settings)
        {

        }

        /// <summary>
        /// Заполнить словари
        /// </summary>
        public void FillDictionaries()
        {

            FillNodeLinks();
            FillNodes();
            FillHouses();
            FillHouseIndexes();
        }


        public string GetHouseType(string type) =>

          type switch
          {
              "1" => "влд.",
              "2" => "д.",
              "3" => "двлд.",
              "4" => "г-ж",
              "5" => "зд.",
              "6" => "шахта",
              "7" => "стр.",
              "8" => "соор.",
              "9" => "литера",
              "10" => "к.",
              "11" => "подв.",
              "12" => "кот.",
              "13" => "п-б",
              _ => "",

          };

        private void FillHouses()
        {


            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            using (XmlReader reader = XmlReader.Create(Path.Combine(_directory, _housesFile), settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {

                        case XmlNodeType.Element:
                            {
                                if (reader.Name == "HOUSE" &&
                                    reader.GetAttribute("OBJECTID") != null

                                     && reader.GetAttribute("ISACTIVE") == "1"
                                    )
                                {
                                    var newNode = new House
                                    {
                                        Id = int.Parse(reader.GetAttribute("OBJECTID")),
                                        Guid = Guid.Parse(reader.GetAttribute("OBJECTGUID")),
                                        Name = reader.GetAttribute("HOUSENUM"),
                                        TypeName = GetHouseType(reader.GetAttribute("HOUSETYPE")),
                                        Addnum = reader.GetAttribute("ADDNUM1"),

                                    };


                                    _dictHouses.Add(newNode.Id, newNode);
                                }

                                break;
                            }
                    }
                }

            }
        }

        private void FillNodes()
        {

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            using (XmlReader reader = XmlReader.Create(Path.Combine(_directory, _addrFile), settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {

                        case XmlNodeType.Element:
                            {
                                if (reader.Name == "OBJECT" &&
                                    reader.GetAttribute("OBJECTID") != null
                                     // && nodeIds.Contains(int.Parse(reader.GetAttribute("OBJECTID")))
                                     && reader.GetAttribute("ISACTIVE") == "1"
                                    )
                                {
                                    var newNode = new Node
                                    {
                                        Id = int.Parse(reader.GetAttribute("OBJECTID")),
                                        Guid = Guid.Parse(reader.GetAttribute("OBJECTGUID")),
                                        Name = reader.GetAttribute("NAME"),
                                        TypeName = reader.GetAttribute("TYPENAME")
                                    };

                                    //  _nodes.Add(newNode);
                                    _nodesDict.Add(newNode.Id, newNode);
                                }

                                break;
                            }
                    }
                }
            }


        }
        private void FillNodeLinks()
        {

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            using (XmlReader reader = XmlReader.Create(Path.Combine(_directory, _hierFile), settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {

                        case XmlNodeType.Element:
                            {
                                if (reader.Name == "ITEM" &&
                                    reader.GetAttribute("PARENTOBJID") != null
                                    && reader.GetAttribute("OBJECTID") != null
                                    && reader.GetAttribute("ISACTIVE") == "1"
                                    )
                                {
                                    int key = int.Parse(reader.GetAttribute("OBJECTID"));
                                    int value = int.Parse(reader.GetAttribute("PARENTOBJID"));

                                    _childToParentDict.Add(key, value);

                                }
                                break;
                            }
                    }
                }

            }
            _lookup = _childToParentDict.ToLookup(x => x.Value, x => x.Key);
        }


        public virtual async Task<Node> GetNodeId(Guid fias)
        {
            Node newNode = null;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            string fiasStr = fias.ToString();

            using (XmlReader reader = XmlReader.Create(Path.Combine(_directory, _addrFile), settings))
            {
                while (await reader.ReadAsync())
                {
                    switch (reader.NodeType)
                    {

                        case XmlNodeType.Element:
                            {
                                if (reader.Name == "OBJECT" &&
                                    reader.GetAttribute("OBJECTGUID") == fiasStr)
                                {
                                    // return Int32.Parse(reader.GetAttribute("OBJECTID"));

                                    newNode = new Node
                                    {
                                        Id = int.Parse(reader.GetAttribute("OBJECTID")),
                                        Guid = Guid.Parse(reader.GetAttribute("OBJECTGUID")),
                                        Name = reader.GetAttribute("NAME"),
                                        TypeName = reader.GetAttribute("TYPENAME")
                                    };
                                    return newNode;
                                }
                                break;
                            }
                    }
                }
                return newNode;
            }
        }



        public override async Task<IList<int>> GetNodeLinks(int nodeId)
        {
            return _lookup[nodeId].ToList();
        }

        public override async Task<IList<Node>> GetNodes(IList<int> nodeIds)
        {

            return _nodesDict.Where(node => nodeIds.Contains(node.Key)).Select(p => p.Value).ToList();
        }

        public override async Task<ICollection<House>> GetHouses(IEnumerable<int> nodeIds)
        {
            return _dictHouses.Where(dict => nodeIds.Contains(dict.Key)).Select(p => p.Value).ToList();

        }

        public void FillHouseIndexes()
        {

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            using (XmlReader reader = XmlReader.Create(Path.Combine(_directory, _houseParams), settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {

                        case XmlNodeType.Element:
                            {
                                if (reader.Name == "PARAM" &&
                                    reader.GetAttribute("OBJECTID") != null
                                    && reader.GetAttribute("TYPEID") == "5")

                                {

                                    if (_dictHouses.TryGetValue(int.Parse(reader.GetAttribute("OBJECTID")), out House house))

                                        house.Index = reader.GetAttribute("VALUE");
                                }
                                // houses.First(x => x.Id == Int32.Parse(reader.GetAttribute("OBJECTID"))).Index = reader.GetAttribute("VALUE");



                                break;
                            }
                    }
                }

            }

        }
    }
}
