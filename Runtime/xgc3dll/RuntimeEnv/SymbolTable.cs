using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;

using xgc3.Core;

namespace xgc3.RuntimeEnv
{
    public class Symbol : Instance
    {
        // Flags that can describe a 
        public string CodeBehind = null;
        public string BaseClass = null;
        public string FullClassName;
        public string FullBaseClassName;

        public static Symbol Parse(XmlNode node)
        {
            Symbol symbol = new Symbol();
            foreach (XmlAttribute attr in node.Attributes)
            {
                symbol.SetProperty(attr.Name, attr.Value);
            }
            return symbol;
        }

        private Symbol()
        {
        }

        public Symbol(string className, string fullClassName, string fullBaseClassName, Type childrenType )
        {
            Name = className;
            FullBaseClassName = fullBaseClassName;
            FullClassName = fullClassName;
            ChildrenType = childrenType;
        }

        public Symbol(string className, string fullClassName, string fullBaseClassName)
        {
            Name = className;
            FullBaseClassName = fullBaseClassName;
            FullClassName = fullClassName;
        }

    }

    
    public class SymbolTable : BaseRuntimeEnvInstance
    {

        private Dictionary<string /*classname*/, Symbol > m_symbols = new Dictionary<string,Symbol>();
        private Dictionary<string /*instancename*/, string /*full class name*/> m_instanceTypes = new Dictionary<string,string>();

        /// <summary>
        /// Manage all the classes in the system.
        /// </summary>
#if LOAD_WITH_CSSCRIPT
        private AsmHelper m_helper;
#endif

        /// <summary>
        /// Returns type of statically defined instance. Use RunningObjectTable for dynamic instances.
        /// </summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public string LookupFullTypeNameOfInstance( string instanceName)
        {
            return m_instanceTypes[instanceName];
        }
         

        /// <summary>
        /// So we can lookup the class of an instance (kinda like ROT, but compile time).
        /// Used for clones.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fullClassName"></param>
        public void AddClassInstanceName( string name, string fullClassName)
        {
            if (m_instanceTypes.ContainsKey(name))
            {
                throw new Exceptions.CompileException("Duplicate 'Name' attribute: " + name);
            }
            m_instanceTypes.Add( name, fullClassName);
        }


        /// <summary>
        /// Helps to detect duplicate symbols, for example.
        /// </summary>
        /// <param name="className"></param>
        public void AddClassSymbol(Symbol symbol )
        {
            if (m_symbols.ContainsKey( symbol.Name))
            {
                throw new Exceptions.CompileException("Duplicate class name: {0}", symbol.Name);
            }

            if (m_symbols.ContainsKey(symbol.FullClassName))
            {
                throw new Exceptions.CompileException("Duplicate full class name: {0}", symbol.FullClassName);
            }

            //if ( (fullBaseClass != null) && !m_symbols.ContainsKey(fullBaseClass))
            //{
            //    throw new Exceptions.CompileException("Unknown base class: {0}", fullBaseClass);
            //}

            m_symbols.Add(symbol.Name, symbol);
#if OLD_WAY
             Assembly assembly = Assembly.Load( new AssemblyName( "xgc3.Generated"));

              // Get all Types available in the assembly in an array
              Type[] typeArray = assembly.GetTypes ();

              Type t=typeArray[className];
               t.FullName, t.BaseType
#endif
        }

        /// <summary>
        /// Does class have specified attribute?
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="attribute"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public bool HasCustomAttribute(string typeName, Type attribute, bool inherit)
        {
            System.Reflection.MemberInfo inf = GetType( typeName);
            return inf.GetCustomAttributes(attribute, inherit).Length > 0;
        }


        // Convert a value/type int C# code string
        public string ValueToCode(string value, string typeString)
        {
            string code;

            Type type = this.GetType(typeString);

            if (type == typeof(Int32))
            {
                code = value;
            }
            else if (type == typeof(string))
            {
                code = "\"" + value + "\"";
            }
            else if (type == typeof(Microsoft.Xna.Framework.Graphics.Color))
            {
                // Special handler for colors. Convert color "name" to static property on the XNA color object.
                code = "Microsoft.Xna.Framework.Graphics.Color." + value;
            }
            else if (type.IsSubclassOf(typeof(Enum)))
            {
                code = typeString + "." + value;
            }
            else if (type is Type)
            {
                code = type.ToString();
            }
            else
            {
                throw new Exceptions.CompileException("Unsupported value for DefaultValue: " + value);
            }

            return code;

        }



        /// <summary>
        /// Useful for checking if a type implements an interface.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="typeName"></param>
        /// <param name="gm"></param>
        /// <returns></returns>
        public Type GetType( string typeName)
        {
            string fulltypename;
            try
            {
                fulltypename = m_symbols[typeName].FullClassName;
            }
            catch (Exception)
            {
                throw new Exceptions.CompileException("Unknown type: {0}", typeName);
            }
            try
            {
                // Problems instantiating flower 4
                //MAYBE I NEED A PUBLIC CONSTRUCTOR.
                // Also fails: Assembly.Load("GameEditor.xgc, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"); //
                Assembly asm = Assembly.LoadFrom("GameEditor.xgc.dll"); // TODO - Use passed in name.
                return asm.GetType(fulltypename);
            }
            catch (Exception ex)
            {
                throw new Exceptions.CompileException(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="typeName"></param>
        /// <param name="gm">Populate every instance with a call back to this top-level-object.</param>
        /// <returns></returns>
        public Instance Construct(string id, string typeName, GameManager gm)
        {
            string fulltypename;
            try
            {
                fulltypename = m_symbols[typeName].FullClassName;
            }
            catch (Exception)
            {
                throw new Exceptions.CompileException("Unknown type: {0}",  typeName);
            }
            try
            {
                // Problems instantiating flower 4
                //MAYBE I NEED A PUBLIC CONSTRUCTOR.
                // Also fails: Assembly.Load("GameEditor.xgc, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"); //
                Assembly asm = Assembly.LoadFrom( "GameEditor.xgc.dll"); // TODO - Use passed in name.
                 object oo = asm.CreateInstance(fulltypename);
                 Instance bi = oo as Instance;

#if LOAD_WITH_CSSCRIPT
                else
                {
                    // Generated classes must be instantiated this way.
                    bi = (Instance)m_helper.CreateObject(fulltypename);
                }
#endif
                if (bi != null)
                {
                    bi.Name = id;
                    bi.GameMgr = gm;
                    bi.IsJustCreated = true;
                    bi.ChildrenType = m_symbols[typeName].ChildrenType;

                    // TODO: Set defaults on all properties from class here.

                }

                return bi;
            }
            catch (Exception ex)
            {
                throw new Exceptions.CompileException( ex.Message);
                //"Only types that inherit from Instance can be constructed. ID={0} Type={1}", id, typeName);
            }

        }

        public string LookupFullTypeName(string className)
        {
            return m_symbols[className].FullClassName;
        }

        public string LookupFullBaseTypeName(string className)
        {
            try
            {
                string fullBaseClass = m_symbols[className].FullBaseClassName;
                if (fullBaseClass == null)
                {
                    throw new Exceptions.CompileException("Invalid root object: {0}", className);
                }
                return fullBaseClass;
            }
            catch
            {
                throw new Exceptions.CompileException("Unknown base class for class: {0}", className);
            }

        }

    }
}
