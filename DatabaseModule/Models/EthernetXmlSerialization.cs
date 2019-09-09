using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DatabaseModule.Models
{
    [XmlRoot("Robot")]
    public class EthernetXmlSerialization
    {
        [XmlElement("Position")]
        public RobotJoint Position { get; set; }

        [XmlElement("Velocity")]
        public RobotJoint Velocity { get; set; }

        [XmlElement("Current")]
        public RobotJoint Current { get; set; }

        [XmlElement("Temp")]
        public RobotJoint Temp { get; set; }

        [XmlElement("Torque")]
        public RobotJoint Torque { get; set; }

        [XmlElement("Time")]
        public XmlTime Time { get; set; }

        [XmlElement("ProgramNum")]
        public XmlTagValue ProgramNumber { get; set; }

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

    public class XmlTime
    {
        [XmlAttribute("Year")] public double Year { get; set; }

        [XmlAttribute("Month")] public double Month { get; set; }

        [XmlAttribute("Day")] public double Day { get; set; }

        [XmlAttribute("Hour")] public double Hour { get; set; }

        [XmlAttribute("Minute")] public double Minute { get; set; }

        [XmlAttribute("Second")] public double Second { get; set; }

        [XmlAttribute("Millisecond")] public double Millisecond { get; set; }
        
    }

    public class XmlTagValue
    {
        [XmlAttribute("Value")] public double Value { get; set; }
        

    }
}