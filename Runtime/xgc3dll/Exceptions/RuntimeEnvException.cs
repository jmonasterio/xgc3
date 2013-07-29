using System;
using System.Collections.Generic;
using System.Text;

namespace xgc3.Exceptions
{
    public class RuntimeEnvException : BaseException 
    {
        public RuntimeEnvException(string msg, params object[] ps)
            : base( msg, ps)
        {

        }

    }
}
