using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace CGLibs.Serialization
{

    internal static class XmlNameSpaceRemovalExtension
    {

        public static string StripXmlNameSpacesFromRawXml(this string rawxml)
        {
            try
            {
                XDocument doc = XDocument.Parse(rawxml);
                if (doc != null)
                {
                    XElement root = doc.Root.WithoutNamespaces();
                    return root.ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());            
            }

            return rawxml;
        }

        /// <summary>
        /// Returns the specified <see cref="XElement"/>
        /// without namespace qualifiers on elements and attributes.
        /// </summary>
        /// <param name="element">The element</param>
        public static XElement WithoutNamespaces(this XElement element)
        {
            //method courtesy of rasx answer at https://stackoverflow.com/questions/987135/how-to-remove-all-namespaces-from-xml-with-c
            if (element == null) return null;

            #region delegates:

            Func<XNode, XNode> getChildNode = e => (e.NodeType == XmlNodeType.Element) ? (e as XElement).WithoutNamespaces() : e;

            Func<XElement, IEnumerable<XAttribute>> getAttributes = e => (e.HasAttributes) ?
                e.Attributes()
                    .Where(a => !a.IsNamespaceDeclaration)
                    .Select(a => new XAttribute(a.Name.LocalName, a.Value))
                :
                Enumerable.Empty<XAttribute>();

            #endregion

            return new XElement(element.Name.LocalName,
                element.Nodes().Select(getChildNode),
                getAttributes(element));
        }
    }
}
