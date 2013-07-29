using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using xgc3.Core;
using xgc3.RuntimeEnv; 

using CSScriptLibrary; 

namespace xgc3.Scripting
{
    public class ScriptingEngine
    {
        private AsmHelper m_helper;

        public ScriptingEngine( string code)
        {
        //    FileInfo fi = new FileInfo( @"C:\src\xgc3\xgc3\Generated.cs");
          //  string code = File.ReadAllText(fi.FullName); 

            m_helper = new AsmHelper(CSScript.LoadCode(code) ); //, null, true));
            CreateInstance("test", "xgc3.Generated.Test");
        }

        public Instance CreateInstance( string id, string typeName)
        {
            Instance bi = m_helper.CreateObject(typeName) as Instance;
            bi.ID = id;
            return bi;
        }

    }


}
