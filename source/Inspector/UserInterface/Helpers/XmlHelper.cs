using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ChristianMoser.WpfInspector.UserInterface.Helpers
{
    public static class XmlHelper
    {
        /// <summary>
        /// Indents the specifide XML string.
        /// </summary>
        public static string Indent(this string xml)
        {
            var reder = new XmlTextReader(new StringReader(xml));
            var sw = new StringWriter();
            var xmlTextWriter = new XmlTextWriter(sw) {Formatting = Formatting.Indented};
            while (reder.Read())
                xmlTextWriter.WriteNode(reder, false);
            xmlTextWriter.Close();
            reder.Close();
            return sw.ToString();
        }

        /// <summary>
        /// Removes the namespaces.
        /// </summary>
        public static string RemoveNamespaces(this string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));
            return xmlDocumentWithoutNs.ToString();
        }

        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                var xElement = new XElement(xmlDocument.Name.LocalName) {Value = xmlDocument.Value};
                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }
    }
}
