using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GarProject1
{
    /// <summary>
    /// Поиск напрямую из xml файлов без мемоизации
    /// </summary>
    public class AddrParser
    {
        protected string _addrFile;
        protected string _hierFile;
        protected string _directory;
        protected string _housesFile;
        protected string _houseParams;

        // public Node RootNode { get; set; }


        public AddrParser(string directory, string addrFile, string hierFile, string housesFile, string houseParams)
        {
            _addrFile = addrFile;
            _hierFile = hierFile;
            _directory = directory;
            _housesFile = housesFile;
            _houseParams = houseParams;

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
                                        Id = Int32.Parse(reader.GetAttribute("OBJECTID")),
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

        //public virtual async Task<IList<Node>> GetChildrenNodes(int nodeId)
        //{
        //    var elementIds = await GetNodeLinks(nodeId);
        //    var nodes = await GetNodes(elementIds);
        //    //if (nodes.Any()) nodes = await GetHouses(elementIds);
        //    return nodes;
        //}

        public virtual async Task<IList<int>> GetNodeLinks(int nodeId)
        {
            IList<int> result = new List<int>();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            using (XmlReader reader = XmlReader.Create(Path.Combine(_directory, _hierFile), settings))
            {
                while (await reader.ReadAsync())
                {
                    switch (reader.NodeType)
                    {

                        case XmlNodeType.Element:
                            {
                                if (reader.Name == "ITEM" &&
                                    reader.GetAttribute("PARENTOBJID") == nodeId.ToString()
                                    && reader.GetAttribute("ISACTIVE") == "1"
                                    )
                                    result.Add(Int32.Parse(reader.GetAttribute("OBJECTID")));

                                break;
                            }
                    }
                }
                return result;
            }
        }

        public virtual async Task<IList<Node>> GetNodes(IList<int> nodeIds)
        {
            IList<Node> result = new List<Node>();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            using (XmlReader reader = XmlReader.Create(Path.Combine(_directory, _addrFile), settings))
            {
                while (await reader.ReadAsync())
                {
                    switch (reader.NodeType)
                    {

                        case XmlNodeType.Element:
                            {
                                if (reader.Name == "OBJECT" &&
                                    reader.GetAttribute("OBJECTID") != null
                                    && nodeIds.Contains(int.Parse(reader.GetAttribute("OBJECTID")))
                                     && reader.GetAttribute("ISACTIVE") == "1"
                                    )
                                {
                                    var newNode = new Node
                                    {
                                        Id = Int32.Parse(reader.GetAttribute("OBJECTID")),
                                        Guid = Guid.Parse(reader.GetAttribute("OBJECTGUID")),
                                        Name = reader.GetAttribute("NAME"),
                                        TypeName = reader.GetAttribute("TYPENAME")
                                    };

                                    result.Add(newNode);
                                }

                                break;
                            }
                    }
                }
                return result;
            }
        }

        public virtual async Task<ICollection<House>> GetHouses(IEnumerable<int> nodeIds)
        {
            var nodeIdsHashset = nodeIds.ToHashSet();
            ICollection<House> result = new List<House>();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            using (XmlReader reader = XmlReader.Create(Path.Combine(_directory, _housesFile), settings))
            {
                while (await reader.ReadAsync())
                {
                    switch (reader.NodeType)
                    {

                        case XmlNodeType.Element:
                            {
                                if (reader.Name == "HOUSE" &&
                                    reader.GetAttribute("OBJECTID") != null
                                    && nodeIdsHashset.Contains(Int32.Parse(reader.GetAttribute("OBJECTID")))
                                     && reader.GetAttribute("ISACTIVE") == "1"
                                    )
                                {
                                    var newNode = new House
                                    {
                                        Id = Int32.Parse(reader.GetAttribute("OBJECTID")),
                                        Guid = Guid.Parse(reader.GetAttribute("OBJECTGUID")),
                                        Name = reader.GetAttribute("HOUSENUM"),
                                        TypeName = reader.GetAttribute("HOUSETYPE")
                                    };

                                    result.Add(newNode);
                                }

                                break;
                            }
                    }
                }
                return result;
            }
        }

        public virtual async Task FillHouseIndexes(ICollection<House> houses)
        {
            if (houses == null || !houses.Any()) return;
            HashSet<int> houseIds = houses.Select(house => house.Id).ToHashSet();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            using (XmlReader reader = XmlReader.Create(Path.Combine(_directory, _houseParams), settings))
            {
                while (await reader.ReadAsync())
                {
                    switch (reader.NodeType)
                    {

                        case XmlNodeType.Element:
                            {
                                if (reader.Name == "PARAM" &&
                                    reader.GetAttribute("OBJECTID") != null
                                    && reader.GetAttribute("TYPEID") == "5"
                                    && houseIds.Contains(Int32.Parse(reader.GetAttribute("OBJECTID")))

                                    )

                                    houses.First(x => x.Id == Int32.Parse(reader.GetAttribute("OBJECTID"))).Index = reader.GetAttribute("VALUE");



                                break;
                            }
                    }
                }

            }
        }
    }
}
