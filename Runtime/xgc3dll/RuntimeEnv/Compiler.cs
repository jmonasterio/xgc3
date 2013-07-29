using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Xml.XPath;

#if !XBOX360
using CSScriptLibrary;

using csscript; 
#endif

namespace xgc3.RuntimeEnv
{
    /// <summary>
    /// Wrapper for Game XML. Outside this object, no one should be doing
    /// direct xml access?
    /// </summary>
    public class Compiler : XmlClassReader 
    {
        private StringBuilder m_using = new StringBuilder();

#if !XBOX360
        private AsmHelper m_helper;

        private void CompileClasses(string code)
        {
            //    FileInfo fi = new FileInfo( @"C:\src\xgc3\xgc3\Generated.cs");
            //  string code = File.ReadAllText(fi.FullName); 
            m_helper = new AsmHelper(CSScriptLibrary.CSScript.LoadCode(code)); //, null, true));
            //CreateInstance("test", "xgc3.Generated.Test");
        }
#endif

        /// <summary>
        /// Convert code into a DLL.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="outputDllName"></param>
        /// <param name="bDebug"></param>
        public void CompileClassesToDll( GameManager gm, string outputDllName, bool bDebug)
        {
            // TODO: This seems so lame.
            gm.SymbolTable.AddClassSymbol( new Symbol("Application", "xgc3.RuntimeEnv.Application", "xgc3.Core.Instance"));

            StringBuilder csharp = new StringBuilder();

            // Start of file
            csharp.Append(makeheader());

            csharp.Append(CompileChildClasses(m_doc.DocumentElement, gm, "xgc3.Core.BaseInstanceClass"));

            // Pass #2.
            csharp.Append(CompileChildInstance(m_doc.DocumentElement, null, gm, "xgc3.Generated", 0));

            // End of file
            csharp.Append(makefooter());

#if !XBOX360
            try
            {
                string intermediateFile = outputDllName + ".cs";
                using (FileStream fs = new FileStream(intermediateFile, FileMode.Create))
                {
                    using (TextWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(m_using.ToString() + csharp.ToString());

                    }
                }
                string[] assy = new string[] { @"c:\Program Files\Microsoft XNA\XNA Game Studio\v2.0\References\Windows\x86\Microsoft.Xna.Framework.dll",
                    @"c:\Program Files\Microsoft XNA\XNA Game Studio\v2.0\References\Windows\x86\Microsoft.Xna.Framework.Game.dll"};
                CSScript.Compile(intermediateFile, outputDllName, bDebug, assy);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
#endif


        }

        /// <summary>
        /// PASS#2: Some instances have handlers and other code that needs to be compiled. Do that here.
        /// </summary>
        /// <param name="gm"></param>
        /// <param name="?"></param>
        public void CompileInstances( GameManager gm)
        {

            StringBuilder csharp = new StringBuilder();

            // Start of file
            csharp.Append(makeheader());

            csharp.Append( CompileChildInstance(m_doc.DocumentElement, null, gm, "xgc3.Generated", 0 ));

            // End of file
            csharp.Append(makefooter());

#if !XBOX360
            CompileClasses( m_using.ToString() + csharp.ToString());
#endif
        }

        /// <summary>
        /// Classes can be nested, so we need to reurse
        /// </summary>
        /// <param name="parentNode"></param>
        private string CompileChildInstances( XmlNode parentNode, GameManager gm, string parentClassName )
        {
            StringBuilder csharp = new StringBuilder();

            XmlNodeList childNodes = parentNode.ChildNodes;
            if (childNodes != null)
            {
                int uniqueIndex = 0;
                foreach (XmlNode node in childNodes)
                {
                    csharp.Append( CompileChildInstance(node, parentNode, gm, parentClassName, uniqueIndex));
                    uniqueIndex++;
                }
            }
            return csharp.ToString();
        }

        public interface IGuess
        {
        }

        private string CompileChildInstance(XmlNode node, XmlNode parentNode, GameManager gm, string parentClassName, int uniqueIndex )
        {
            StringBuilder csharp = new StringBuilder();

            if (node.Name == "Attribute")
            {
                string name = GetInstanceId(node, parentClassName, uniqueIndex);
                if (name == parentNode.Name)
                {
                    throw new Exceptions.CompileException("Attribute name cannot be same as class: " + name);
                }
                string type = GetRequiredAttribute(node, "Type");
                string defaultValue = GetOptionalAttribute(node, "DefaultValue", null);
                if ( defaultValue != null && (type == "string" || type == "String"))
                {
                    defaultValue = "\"" + defaultValue + "\"";
                }
                string protection = GetOptionalAttribute(node, "Protection", "public"); // Public/Private/Protected
                bool isConstant = GetOptionalAttribute(node, "Constant", false);
                if (defaultValue == null)
                {
                    if (isConstant)
                    {
                        throw new Exceptions.CompileException("Constant attribute missing default value");
                    }
                    csharp.Append(String.Format("{0} {1} {2};\r\n", protection, type, name));
                }
                else
                {
                    // Special handling for default value
                    if (!isConstant)
                    {
                        csharp.Append(String.Format("{0} {1} {2} = \"{3}\";\r\n", protection, type, name, defaultValue));
                    }
                    else
                    {
                        csharp.Append(String.Format("{0} const {1} {2} = \"{3}\";\r\n", protection, type, name, defaultValue));
                    }
                }
            }
            else if (node.Name == "Resource")
            {
                // Resoures are constant and one-per-class (static).
                string name = GetInstanceId(node, parentClassName, uniqueIndex);
                if (name == parentNode.Name)
                {
                    throw new Exceptions.CompileException("Resource name cannot be same as class: " + name);
                }
                string type = GetRequiredAttribute(node, "Type");
                string assetName = GetRequiredAttribute(node, "AssetName");
                
                // public const static ResourceSprite X = new ResourceSprite( "X", "XXX");
                csharp.Append(String.Format("static public xgc3.Resources.{0}Resource {1} = new xgc3.Resources.{2}Resource( \"{3}\", \"{4}\");", type, name, type, name, assetName));

            }
            else if (node.Name == "Event")
            {
                // TODO: At compile-time, how can I tell that I already have an event with the same name defined
                //  in the base class? You'll see error in CODE compilation, but I'd like to give the user a better warning.
                // I think what I need to do is make a data structure that keeps track of all classes created, and what
                //  attributes, events, and methods each has. Then check against that.
                // Another way is to indicate the line of XML that generated each line of code, so I can point back to it later.

                string name = GetInstanceId(node, parentClassName, uniqueIndex);
                string code = node.InnerText;
                csharp.Append(String.Format("private void On_{0}( xgc3.Core.Instance self, xgc3.Core.Instance other, String extra)\r\n", name));
                csharp.Append("{" + code + "}\r\n");
                csharp.Append(String.Format("public event EventDelegate {0};\r\n", name));
                csharp.Append(String.Format("public void Raise_{0}(Instance self, Instance other, string extra)\r\n", name));
                csharp.Append("\t{ if (" + name + " != null) { " + name + "(self, other, extra); }}\r\n");

                // TODO: Need to += when creating instance.
            }
            else if (node.Name == "Handler")
            {
                string name = GetInstanceId(node, parentClassName, uniqueIndex);
                string code = node.InnerText;
                csharp.Append(String.Format("private new void On_{0}( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)\r\n", name));

                // Generate code for local var, self, which is a typesafe version of selfInstance.
                // self, also happens to be the safe as "this".
                string fixedParentClassName = parentClassName.Replace("+", ".");
                csharp.Append("{ " + fixedParentClassName + " self = selfInstance as " + fixedParentClassName + ";" + code + "}\r\n");

                // TODO: Need to += when creating instance.

            }
            else if (node.Name == "Method")
            {
                string name = GetInstanceId(node, parentClassName, uniqueIndex);
                string args = GetOptionalAttribute(node, "Args", "");
                string returns = GetOptionalAttribute(node, "Returns", "void");
                string code = node.InnerText;
                csharp.Append(String.Format("public {0} {1}( " + args + ")\r\n", returns, name));
                csharp.Append("{ " + code + "}\r\n");
            }
            else if (node.Name == "Using")
            {
                // using namespace
                string ns = GetRequiredAttribute(node, "Namespace");

                // Prepend the USING statement at top of file.
                m_using.Append("using " + ns + ";\r\n");
            }
            else if ((node.Name != "Class") && (node.Name != "Include") && node.Name != "Library" && node.Name != "#comment")
            {
                // This is an instance
                XmlNode classNode = node;

                string name = GetInstanceId(node, parentClassName, uniqueIndex);
                string className = classNode.Name; // _ makes unique, so class for instance doesn't conflict with instance.
                string fullBaseClass = gm.SymbolTable.LookupFullTypeName(className); // Not full base class name.

                string uniqueClassName = className + "_" + name;

                string fullClassName;
                if (node.Name == "Application") // TODO
                {
                    // Special naming for first level.
                    fullClassName = parentClassName + "." + uniqueClassName;
                }
                else
                {
                    fullClassName = parentClassName + "+" + uniqueClassName;
                }

                // "Clones" allows you to inherit from an instance.
                string clones = GetOptionalAttribute(classNode, "Clones", null);
                if (clones != null)
                {
                    // How can I lookup an instance to find out what its class is?
                    fullBaseClass = gm.SymbolTable.LookupFullTypeNameOfInstance(clones);

                    // When cloning a class, the typename in our symbol table is the one used for creation using reflection.
                    //  We will be generating new code, so we need to change the + to .
                    fullBaseClass = fullBaseClass.Replace("+", ".");
                }

                // Keep track of all symbols
                // Make a unique name for class -- there could be two classes with same type but different names.
                gm.SymbolTable.AddClassSymbol(new Symbol(fullClassName, fullClassName, fullBaseClass));

                // So we can lookup the class of an instance (kinda like ROT, but compile time).
                // Used for clones.
                gm.SymbolTable.AddClassInstanceName(name, fullClassName);

                // start of class
                csharp.Append("public class " + uniqueClassName + " : " + fullBaseClass + " { \r\n");

                // Need an empty PUBLIC constructor for COMPACT FRAMEWORK on XBOX -- Nope.
                csharp.Append("public " + uniqueClassName + "() {}\r\n");

                // Recurse for nested instances
                csharp.Append(CompileChildInstances(node, gm, fullClassName));

                // End of class
                csharp.Append("}\r\n");

            }
            else
            {
                // Recurse for nested instances
                if ((node.Name != "Class"))
                {
                    csharp.Append(CompileChildInstances(node, gm, parentClassName));
                }
            }


            return csharp.ToString();
        }



        /// <summary>
        /// PASS#1: Convert all classes in XML into C# code.
        /// Read all classes from XML and generate C# code for classes that do not have an assembly attribute
        /// </summary>
        /// <returns></returns>
        public void CompileClasses(GameManager gm)
        {
            // TODO: This seems so lame.
            gm.SymbolTable.AddClassSymbol( new Symbol("Application", "xgc3.RuntimeEnv.Application", "xgc3.Core.Instance"));

            StringBuilder csharp = new StringBuilder();

            // Start of file
            csharp.Append(makeheader());

            csharp.Append(CompileChildClasses(m_doc.DocumentElement, gm, "xgc3.Core.BaseInstanceClass"));

            // End of file
            csharp.Append(makefooter());

#if !XBOX360
            try
            {
                CompileClasses(csharp.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
#endif


        }

        /// <summary>
        /// Classes can be nested, so we need to reurse
        /// </summary>
        /// <param name="parentNode"></param>
        private string CompileChildClasses( XmlNode parentNode, GameManager gm, string parentClassName)
        {
            StringBuilder csharp = new StringBuilder();

            XmlNodeList childNodes = parentNode.ChildNodes;
            if (childNodes != null)
            {

                foreach (XmlNode node in childNodes)
                {
                    if (node.Name == "Class")
                    {
                        XmlNode classNode = node;

                        Symbol symbol = Symbol.Parse(classNode); 
                        
                        // GetRequiredAttribute(classNode, "Name");
                        // GetOptionalAttribute(classNode, "CodeBehind", null); // Return NULL if none.
                        // GetOptionalAttribute(classNode, "ChildrenType", null); // Return NULL if none.

                        string clones = symbol.Clones; // GetOptionalAttribute(classNode, "Clones", null);
                        if (clones != null)
                        {
                            System.Diagnostics.Debug.WriteLine("'Clones' attribute not implemented. Skipping class.");
                            continue;
                        }

                        if (symbol.CodeBehind == null)
                        {

                            //baseClass = symbol.BaseClass; // GetRequiredAttribute(classNode, "BaseClass");

                            // Add namespace.
                            symbol.FullBaseClassName = gm.SymbolTable.LookupFullTypeName(symbol.BaseClass);
                            symbol.FullClassName = "xgc3.Generated." + symbol.Name; // st.LookupFullTypeName(className);

                            // start of class
                            csharp.Append("public class " + symbol.Name + " : " + symbol.FullBaseClassName + " { \r\n");

                            // Generate a constructor that attaches on all the events and handlers, as we go along
                            // LIKE:
                            // public Sprite()
                            //{
                            //    this.Draw += On_Draw;
                            //    this.Create += On_Create;
                            //    this.Destroy += On_Destroy;
                            //}
                            StringBuilder constructor = new StringBuilder("public " + symbol.Name + "() { \r\n");

                            // Add attribute
                            // Example <Attribute Name="Title" Type="String" />
                            XmlNodeList attributeNodes = classNode.SelectNodes("Attribute");
                            foreach (XmlNode attributeNode in attributeNodes)
                            {
                                string name = GetRequiredAttribute(attributeNode, "Name");
                                string protection = GetOptionalAttribute(attributeNode, "Protection", "public"); // Public, Private, Protected
                                bool isConstant = GetOptionalAttribute(attributeNode, "Constant", false);
                                if( name == symbol.Name)
                                {
                                    throw new Exceptions.CompileException( "Attribute name cannot be same as class: " + name);
                                }
                                string type = GetRequiredAttribute(attributeNode, "Type");

                                string defaultValue = GetOptionalAttribute(attributeNode, "DefaultValue", null);
                                if (defaultValue != null && (type == "string" || type == "String"))
                                {
                                    defaultValue = "\"" + defaultValue + "\"";
                                }

                                if (defaultValue == null)
                                {
                                    if (isConstant)
                                    {
                                        throw new Exceptions.CompileException("Constant attribute missing default value");
                                    }
                                    csharp.Append(String.Format("{0} {1} {2};\r\n", protection, type, name));
                                }
                                else
                                {
                                    if (!isConstant)
                                    {
                                        csharp.Append(String.Format("{0} {1} {2} = {3};\r\n", protection, type, name, defaultValue));
                                    }
                                    else
                                    {
                                        csharp.Append(String.Format("{0} const {1} {2} = {3};\r\n", protection, type, name, defaultValue));
                                    }
                                }
                            }

                            XmlNodeList resourceNodes = classNode.SelectNodes("Resource");
                            foreach (XmlNode resourceNode in resourceNodes)
                            {
                                // Resources are constant and one-per-class (static).
                                string name = GetRequiredAttribute(resourceNode, "Name");
                                if (name == parentNode.Name)
                                {
                                    throw new Exceptions.CompileException("Resource name cannot be same as class: " + name);
                                }
                                string type = GetRequiredAttribute(resourceNode, "Type");
                                string assetName = GetRequiredAttribute(resourceNode, "AssetName");

                                // public const static ResourceSprite X = new ResourceSprite( "X", "XXX");
                                csharp.Append(String.Format("static public xgc3.Resources.{0}Resource {1} = new xgc3.Resources.{2}Resource( \"{3}\", \"{4}\");", type, name, type, name, assetName));
                            }


                            XmlNodeList eventNodes = classNode.SelectNodes("Event");
                            foreach (XmlNode eventNode in eventNodes)
                            {
                                string name = GetRequiredAttribute(eventNode, "Name");
                                string code = eventNode.InnerText;
                                csharp.Append(emitDebugLine(eventNode));
                                csharp.Append(String.Format("public void On_{0}( xgc3.Core.Instance self, xgc3.Core.Instance other, String extra)\r\n", name));
                                csharp.Append("{" + code + "}\r\n");
                                csharp.Append(String.Format("public event EventDelegate {0};\r\n", name));
                                csharp.Append(String.Format("public void Raise_{0}(Instance self, Instance other, string extra)\r\n", name));
                                csharp.Append("\t{ if (" + name + " != null) { " + name+"(self, other, extra); }}\r\n");

                                // TODO: Need to += when creating instance.

                                constructor.Append( string.Format("this.{0} += On_{1};\r\n", name, name));
                            }

                            XmlNodeList methodNodes = classNode.SelectNodes("Method");
                            foreach (XmlNode methodNode in methodNodes)
                            {
                                string name = GetRequiredAttribute(methodNode, "Name");
                                string code = methodNode.InnerText;
                                string args = GetOptionalAttribute(methodNode, "Args", "");
                                string returns = GetOptionalAttribute(methodNode, "Returns", "void");
                                csharp.Append(emitDebugLine(methodNode));
                                csharp.Append(String.Format("public {0} {1}(" + args + ")\r\n", returns, name));
                                csharp.Append("{" + code + "}\r\n");
                            }


                            XmlNodeList handlerNodes = classNode.SelectNodes("Handler");
                            foreach (XmlNode handlerNode in handlerNodes)
                            {
                                string name = GetRequiredAttribute(handlerNode, "Name");
                                string code = handlerNode.InnerText;
                                csharp.Append(String.Format("private void On_{0}( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)\r\n", name));

                                // Generate code for local var, self, which is a typesafe version of selfInstance.
                                // self, also happens to be the safe as "this".
                                string fixedFullClassName = symbol.FullClassName.Replace("+", ".");
                                csharp.Append("{ " + fixedFullClassName + " self = selfInstance as " + fixedFullClassName + ";" + code + "}\r\n");

                                constructor.Append( string.Format("this.{0} += On_{1};\r\n", name, name));
                            }

                            constructor.Append("}\r\n");

                            // Recurse for nested classes
                            csharp.Append(CompileChildClasses(node, gm, symbol.FullClassName));

                            csharp.Append(constructor);

                            // End of class
                            csharp.Append("}\r\n");

                        }
                        else
                        {
                            UnallowedAttribute(classNode, "BaseClass");

                            if (classNode.ChildNodes.Count > 0)
                            {
                                throw new Exceptions.CompileException("Classes with 'CodeBehind' attribute cannot declare attributes, events, or methods");
                            }
                            // Class already exists in this project. No code behind needs to be generated.

                            // Figure out the BaseClass automatically...
                            Assembly assembly = Assembly.Load("xgc3dll");

                            // Get all Types available in the assembly in an array
                            symbol.FullBaseClassName = assembly.GetType(symbol.CodeBehind + "." + symbol.Name).BaseType.ToString();
                            if (symbol.FullBaseClassName == "System.Object")
                            {
                                // Special handler for root object.
                                symbol.FullBaseClassName = null;
                            }

                            symbol.FullClassName = symbol.CodeBehind + "." + symbol.Name;
                        }

                        // Keep track of all symbols
                        //if (childrenType != null)
                        //{
                        //    childrenType = gm.SymbolTable.GetType(childrenTypeName);
                        //}
                        gm.SymbolTable.AddClassSymbol( symbol );
                    }
                    else
                    {
                        // Recurse for classes nested in instances, etc.
                        csharp.Append(CompileChildClasses(node, gm, parentClassName));
                    }


                }
            }

           
            return csharp.ToString();
        }

        /// <summary>
        /// Emit info to help with debugging. Doesn't work now.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string emitDebugLine(XmlNode node)
        {
            int nLineNumber = (node as IXmlLineInfo).LineNumber;
            if( nLineNumber > 0)
            {
                return "\r\n#line " + nLineNumber + " \"" + node.OwnerDocument.DocumentElement.Attributes["FullFileName"] + "\"" + "\r\n";
            }
            return "";
        }
        
        private string makeheader()
        {
            return "using System; using System.Collections.Generic; using System.Text; using xgc3.Core; using xgc3.Resources; using Microsoft.Xna.Framework; using Microsoft.Xna.Framework.Graphics; namespace xgc3.Generated {";
        }

        private string makefooter()
        {
            return "}";
        }


#if COMPILER_API
       // TODO: Could be an iterator
        /// <summary>
        /// Get all the classes defined in XML, so they can be added to the 
        ///     symbol table.
        /// </summary>
        /// <returns></returns>
        public ClassDefs GetAllClassDefs()
        {
            return new ClassDefs(); // TODO: Parse from XML.
        }
#endif

        /// <summary>
        /// One section of the xml contains an instance that 
        ///     tells us what 
        /// </summary>
        /// <returns></returns>
        public GameInfo GetGameInfo()
        {
            GameInfo gi = new GameInfo();

            XmlNode giNode = m_doc.SelectSingleNode("//application/gameinfo");
            foreach (XmlAttribute attr in giNode.Attributes)
            {
                gi.SetProperty(attr.Name, attr.Value);
            }

            // TODO
            return new GameInfo();
        }

        public void BuildSymbolTable(GameManager gm)
        {
            System.Diagnostics.Debug.WriteLine("BuildSymboldTable");
            // TODO: This seems so lame.
            gm.SymbolTable.AddClassSymbol( new Symbol("Application", "xgc3.RuntimeEnv.Application", "xgc3.Core.Instance"));

            // Ignore the generated CS code, and don't compile into a DLL.
            // But we DO want the gm.symbolTable generated.
            CompileChildClasses(m_doc.DocumentElement, gm, "xgc3.Core.BaseInstanceClass");
            CompileChildInstance(m_doc.DocumentElement, null, gm, "xgc3.Generated", 0);
        }

#if COMPILER_API
        /// <summary>
        /// Return all instances defined in a room.
        /// 
        /// For efficiency we only create the instances in the active room.
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns></returns>
        public InstanceDefs GetRoomInstances( string roomName)
        {
            return null;
        }

        /// <summary>
        /// Add/create a new class to XML.
        /// </summary>
        /// <param name="classDef"></param>
        public void AlterClassDef( string oldName, ClassDef classDef)
        {
        }

        public void AlterInstanceDef(string oldName, InstanceDef instanceDef)
        {
        }

        /// <summary>
        /// This is a complex XML operation that requires not-only renaming
        ///     the class, but looking for any class that inherits from it, and
        ///     changing the "inherits" attribute of the derived class. Also
        ///     any instances need to have their class renamed.
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="?"></param>
        public void RenameClass( string oldName, string newName)
        {

        }
#endif
    }
}
