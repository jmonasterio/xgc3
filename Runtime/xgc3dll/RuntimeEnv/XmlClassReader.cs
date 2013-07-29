using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using xgc3.Util; // Cryptography

namespace xgc3.RuntimeEnv
{
    /// <summary>
    /// Base class for classes that know how to read game XML format.
    /// </summary>
    public class XmlClassReader : BaseRuntimeEnvInstance
    {
        protected XmlFileDocument m_doc;


        /// <summary>
        /// Load XML file.
        /// </summary>
        /// <param name="fi"></param>
        public void OpenXml(FileInfo fi)
        {
            m_doc = new XmlFileDocument();
            m_doc.Load(fi.FullName);

            // Add a new onto the root, so it knows what filename it was loaded from.
            XmlAttribute attrFullFileName = m_doc.CreateAttribute( "FullFileName");
            attrFullFileName.Value = fi.FullName;
            m_doc.DocumentElement.Attributes.Append(attrFullFileName);

            // Load all includes -- flatten everything out into one big XML file.
            foreach (XmlNode nodeInclude in m_doc.DocumentElement.SelectNodes("//Include"))
            {
                XmlFileDocument docInclude = new XmlFileDocument();
                docInclude.Load(fi.Directory + @"\" + GetRequiredAttribute(nodeInclude, "Name"));
                nodeInclude.InnerXml = docInclude.DocumentElement.OuterXml;
            }
        }

        /// <summary>
        /// Get name for instance. May not have one -- if not we generate one from the unique ID (which is
        ///     the index of this child into its parent);
        /// </summary>
        /// <param name="node"></param>
        /// <param name="uniqueIndex"></param>
        /// <returns></returns>
        protected string GetInstanceId(XmlNode node, string parentName, int uniqueIndex)
        {
            string id = GetOptionalAttribute(node, "Name", string.Empty);
            if( id == string.Empty)
            {
                // Create a (reproducable) very unique name for the instance.
                string full_id = parentName + "_" + node.Name + "_" + uniqueIndex.ToString();

                byte[] input = Encoding.UTF8.GetBytes(full_id);
                byte[] output = MD5.Create().ComputeHash(input);
                id = BitConverter.ToString(output).Replace("-", "");
            }
            return id;
        }

        protected string GetRequiredAttribute(XmlNode node, string attrName)
        {
            XmlAttribute attr = node.Attributes[attrName];
            if (attr == null)
            {
                throw new Exceptions.CompileException("Could not find attribute {0} on {1}", attrName, node.Name);
            }
            return attr.Value;
        }

        protected string GetOptionalAttribute(XmlNode node, string attrName, string defValue)
        {
            XmlAttribute attr = node.Attributes[attrName];
            if (attr == null)
            {
                return defValue;
            }
            return attr.Value;
        }

        protected bool GetOptionalAttribute(XmlNode node, string attrName, bool defValue)
        {
            XmlAttribute attr = node.Attributes[attrName];
            if (attr == null)
            {
                return defValue;
            }
            return bool.Parse( attr.Value);
        }

        protected void UnallowedAttribute(XmlNode node, string attrName)
        {
            XmlAttribute attr = node.Attributes[attrName];
            if (attr != null)
            {
                throw new Exceptions.CompileException("Attribute not allowed {0} on {1}", attrName, node.Name);
            }
            return;
        }
    }
}
