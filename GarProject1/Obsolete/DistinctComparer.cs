using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GarProject1
{
    class DistinctComparer
    {
        string _path;
        public DistinctComparer(string path)
        {
            _path = path;
        }

        public async Task<int[]> GetAllNodes(string name)
        {

            ICollection<int> result = new List<int>();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            using (XmlReader reader = XmlReader.Create(_path, settings))
            {
                while (await reader.ReadAsync())
                {
                    switch (reader.NodeType)
                    {

                        case XmlNodeType.Element:
                            {
                                if (reader.Name == name &&
                                    reader.GetAttribute("OBJECTID") != null
                                     && reader.GetAttribute("ISACTIVE") == "1"
                                    )


                                    result.Add(Int32.Parse(reader.GetAttribute("OBJECTID")));

                                break;
                            }
                    }
                }
                return result.ToArray();
            }
        }
    }
}
