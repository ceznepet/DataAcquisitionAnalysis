using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DatabaseModule
{
    [XmlRoot("Robot")]
    public class EthernetXmlSerilization
    {
        [XmlElement("Position")]
        public RobotJoint Position { get; set; }

        [XmlElement("Velocity")]
        public RobotJoint Velocity { get; set; }

        [XmlElement("Current")]
        public RobotJoint Current { get; set; }

    }

    public class RobotJoint
    {
        [XmlAttribute("A1")] public double A1 { get; set; }

        [XmlAttribute("A2")] public double A2 { get; set; }

        [XmlAttribute("A3")] public double A3 { get; set; }

        [XmlAttribute("A4")] public double A4 { get; set; }

        [XmlAttribute("A5")] public double A5 { get; set; }

        [XmlAttribute("A6")] public double A6 { get; set; }
    }
}