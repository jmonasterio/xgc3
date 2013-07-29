using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace xgc3.Exceptions
{
    public class CompileException: BaseException
    {
        public CompileException(string msg, XmlNode node, params object[] ps) : base( msg, (node as IXmlLineInfo).LineNumber, node.OwnerDocument.DocumentElement.Attributes["FullFileName"], ps )
        {
        }

        public CompileException(string msg, params object[] ps)
            : base(msg, ps)
        {
        }
    }
}
