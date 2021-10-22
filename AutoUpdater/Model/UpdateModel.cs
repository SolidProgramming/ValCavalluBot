using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutoUpdaterClient.Model
{
    [XmlRoot("UpdateDetails")]
    public class UpdateModel
    {
        [XmlElement(ElementName = "version")]
        public string Version { get; set; }
        [XmlElement(ElementName = "url")]
        public string AssemblyUrl { get; set; }
        [XmlElement(ElementName = "changelog")]
        public string Changelog { get; set; }
        [XmlElement(ElementName = "mandatory")]
        public bool Mandatory { get; set; }
    }
}
