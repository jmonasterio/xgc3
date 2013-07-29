using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace xgc3.RuntimeEnv
{
    /// <summary>
    /// Line Number Information from XmlDocument
    /// 
    /// http://www2.zdo.com/archives/25-Line-Number-Information-from-XmlDocument.html
    /// </summary>
    public class XmlFileDocument : XmlDocument
    {
        IXmlLineInfo lineInfo;

        #region Constructor
        public XmlFileDocument()
            : base()
        {
        }
        #endregion

        #region Methods
        public override XmlElement CreateElement(string prefix,
            string localName, string namespaceURI)
        {
            return (lineInfo != null)
                ? new XmlFileElement(prefix, localName, namespaceURI, this,
                                     lineInfo.LineNumber, lineInfo.LinePosition)
                : new XmlFileElement(prefix, localName, namespaceURI, this);
        }

        public override void Load(XmlReader reader)
        {
            if (reader is IXmlLineInfo)
                lineInfo = (IXmlLineInfo)reader;

            base.Load(reader);

            lineInfo = null;
        }
        #endregion
    }

    public class XmlFileElement : XmlElement, IXmlLineInfo
    {
        #region Fields
        int lineNumber;
        int linePosition;
        #endregion

        #region Constructor
        public XmlFileElement(string prefix, string localName,
            string namespaceURI, XmlDocument doc)
            : this(prefix, localName, namespaceURI, doc, 0, 0)
        {
        }

        public XmlFileElement(string prefix, string localName,
            string namespaceURI, XmlDocument doc,
            int lineNumber, int linePosition)
            : base(prefix, localName, namespaceURI, doc)
        {
            this.lineNumber = lineNumber;
            this.linePosition = linePosition;
        }
        #endregion

        #region IXmlLineInfo Members
        public bool HasLineInfo()
        {
            return true;
        }

        public int LineNumber
        {
            get { return lineNumber; }
        }

        public int LinePosition
        {
            get { return linePosition; }
        }
        #endregion
    }
}
