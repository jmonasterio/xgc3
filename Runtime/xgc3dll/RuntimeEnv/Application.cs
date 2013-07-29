using System;
using System.Collections.Generic;
using System.Text;

using xgc3.Core;

namespace xgc3.RuntimeEnv
{
    public class Application : BaseRuntimeEnvInstance
    {
        // All direct child style sheets will be added to application.
        public Map<StyleSheet> StyleSheets = new Map<StyleSheet>();

        public Application()
        {
        }
    }
}
