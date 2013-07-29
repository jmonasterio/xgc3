using System;
using System.Collections.Generic;
using System.Text;

namespace xgc3.RuntimeEnv
{
    public class GameInfo : BaseRuntimeEnvInstance
    {
        public string Title;
        private string m_title;

        public string XTitle
        {
            get { return m_title; }
            set { m_title = value; }
        }
    }
}
