using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KGERP.Utility
{
    public static class ListToXml
    {
        public static string ToXml<T>(this T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringWriter sw = new StringWriter())
            {
                serializer.Serialize(sw, obj);
                return sw.ToString();
            }
        }
        public static string ToXml<T>(this T obj, string rootName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));

            var xmlNs = new XmlSerializerNamespaces();
            xmlNs.Add(string.Empty, string.Empty);

            using (StringWriter sw = new StringWriter())
            {
                serializer.Serialize(sw, obj, xmlNs);
                return sw.ToString();
            }
        }
    }
}
