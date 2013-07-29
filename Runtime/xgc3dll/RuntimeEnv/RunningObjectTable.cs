using System;
using System.Collections.Generic;
using System.Text;

using xgc3.Core;

namespace xgc3.RuntimeEnv
{
    public class RunningObjectTable : BaseRuntimeEnvInstance
    {
        private Dictionary<string, Instance> m_objects = new Dictionary<string, Instance>();

        public void AddInstance( Instance instance)
        {
            if (m_objects.ContainsKey(instance.Name))
            {
                throw new Exceptions.CompileException("There is already another instance with the name: " + instance.Name);
            }
            m_objects.Add( instance.Name, instance);
        }

        public Instance GetInstance(string name)
        {
            return m_objects[name];
        }
    }
}
