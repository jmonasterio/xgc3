using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using xgc3.Core;

namespace xgc3.RuntimeEnv
{
    public class Loader : XmlClassReader
    {
        private StyleSheetManager m_styleSheetManager;

        /// <summary>
        /// Instantiate and hookup all instances.
        /// </summary>
        /// <param name="gm"></param>
        /// <param name="st"></param>
        /// <param name="rot"></param>
        public void LoadInstances(GameManager mgr)
        {
            // Create a new style sheet manager for this loader.
            m_styleSheetManager = new StyleSheetManager();

            if (m_doc.DocumentElement.Name != "Application")
            {
                throw new Exceptions.CompileException("Expected 'Application' as root of code");
            }

            // Load the application node and all it's children.
            LoadNode(null, m_doc.DocumentElement, mgr, null, 0);
        }

        // The include tag is left there, but INSIDE the include tag is all the included content.
        private void LoadAllChildren(Instance parentInstance, XmlNode parentNode, GameManager gm, string parentClassName)
        {
            // Algo...
            // Traverse entire tree...
            //   If <tagname> is one of "class", "include", "library", or "application" ignore it.
            //   If <tagname> is in symbol table, then create instance of it. Add all children to parent.
            //   If <tagname> is EVENT, METHOD, or ATTR
            //   If <tagname> is not in symbol table, then error.
            int uniqueIndex = 0;
            foreach (XmlNode childNode in parentNode.ChildNodes)
            {
                LoadNode(parentInstance, childNode, gm, parentClassName, uniqueIndex);
                uniqueIndex++;
            }

            return;
        }

        private void LoadNode(Instance parentInstance, XmlNode childNode, GameManager gm, string parentClassName, int uniqueIndex)
        {
            // skip comments
            if (childNode.NodeType == XmlNodeType.Comment)
            {
                return;
            }

            switch (childNode.Name)
            {
                case "Event":
                    {
                        // There should be a method on the script class with
                        // the name: 
                        // instance.GetType().ToString() + "_" + name;
                        string name = GetInstanceId(childNode, parentClassName, uniqueIndex);
                        parentInstance.AddEventHandler(name);
                        break;

                    }
                case "Handler":
                    {
                        string name = GetInstanceId(childNode, parentClassName, uniqueIndex);
                        parentInstance.AddEventHandler(name);
                        break;
                    }
                case "Attribute":
                    {
                        // Add all attributes for instance.
                        //string name = GetRequiredAttribute(node, "Name");
                        //string t = GetRequiredAttribute(node, "Type");
                        //parentInstance.SetProperty(attr.Name, attr.Value);
                        break;
                    }
                case "Class":
                case "Method":
                case "Using":
                case "Resource":
                    // Skip
                    break;
                case "Include":
                    {
                        // Skip over these structural nodes.
                        if (childNode.HasChildNodes)
                        {
                            // Recurse
                            LoadAllChildren(parentInstance, childNode, gm, parentClassName);
                        }
                        break;
                    }
                case "Library":
                    // Skip over these structural nodes.
                    if (childNode.HasChildNodes)
                    {
                        // Recurse
                        LoadAllChildren(parentInstance, childNode, gm, parentClassName);
                    }
                    break;
                default:
                    {
                        string instanceId = GetInstanceId(childNode, parentClassName, uniqueIndex);

                        // Recurse
                        string nestClassName;
                        if (parentClassName == null)
                        {
                            // Yuck! Special case for constructing root object.
                            nestClassName = "xgc3.Generated." + childNode.Name + "_" + instanceId;
                        }
                        else
                        {
                            nestClassName = parentClassName + "+" + childNode.Name + "_" + instanceId;
                        }
                        Instance instance = gm.SymbolTable.Construct(instanceId, nestClassName, gm);

                        if (instance != null)
                        {
                            // Keep track of it by ID for quick lookup.
                            gm.RunningObjectTable.AddInstance(instance);

                            instance.Parent = parentInstance;

                            // Add instance to parent (root has no parent).
                            if (parentInstance != null)
                            {
                                if ( instance.ChildrenType != null)
                                {
                                    if ( !instance.GetType().IsSubclassOf( instance.ChildrenType))
                                    {
                                        throw new Exceptions.RuntimeEnvException("Unexpected child type. Expected: " + instance.ChildrenType.ToString());
                                    }
                                }

                                parentInstance.Children.Add(instanceId, instance);
                            }

                            // Set all properties on node.
                            foreach (XmlAttribute attr in childNode.Attributes)
                            {
                                instance.SetProperty(attr.Name, attr.Value);
                            }

                            if (childNode.HasChildNodes)
                            {
                                LoadAllChildren(instance, childNode, gm, nestClassName);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Could not load instance of: " + nestClassName);
                        }

                        // If new styles found, add them to the stylsheet manager
                        if (childNode.Name == "StyleSheet")
                        {
                            StyleSheet sh = instance as StyleSheet;
                            m_styleSheetManager.AddStyleSheet(sh);
                        }

                        // Apply style sheet to just loaded instance.
                        m_styleSheetManager.ApplyStyleCascadeToInstance(instance);

                        //instance.Raise_Load(instance, null, null);
                    }


                    break;

                }
            }
        }
    }

