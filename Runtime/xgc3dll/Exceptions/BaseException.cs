using System;
using System.Collections.Generic;
using System.Text;

namespace xgc3.Exceptions
{
    public class BaseException : Exception
    {
        public BaseException(string msg)
            : base(msg)
        {
        }

        public BaseException(string msg, int lineNumber, string fullFileName, params object[] ps)
            : base(string.Format(msg, ps) + string.Format(": {0} : {1} ", lineNumber, fullFileName))
        {
        }

        public BaseException(string msg, params object[] ps) : base( string.Format( msg, ps))
        {
        }

    }
}
