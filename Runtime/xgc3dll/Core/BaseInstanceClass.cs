using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using xgc3.RuntimeEnv;

namespace xgc3.Core
{

    /// <summary>
    /// This is the base class for all meta-class (Instance classes) that are 
    ///     dynamically generated (cs-script) while loading XML game description
    ///     file. Actual instances are "newed" from this classes descendents.
    /// </summary>
    public class BaseInstanceClass
    {
        public BaseInstanceClass()
        {
        }

        internal string GetSimpleClassName()
        {
            string fullType = this.GetType().ToString();

            return fullType.Substring(fullType.LastIndexOf(".")+1);
        }

        public void TrySetProperty(string propName, string value)
        {
            SetProperty(propName, value, false);
        }

        private void SetProperty(string propName, string value, bool bThrowIfDoesntExist)
        {
            PropertyInfo pi = this.GetType().GetProperty(propName);
            if (pi != null)
            {
                this.GetType().GetProperty(propName).SetValue(this, value, null);
                return;
            }
            FieldInfo fi = this.GetType().GetField(propName);
            if (fi != null)
            {
                if (fi.FieldType.UnderlyingSystemType == typeof(Int32))
                {
                    fi.SetValue(this, (object)Int32.Parse(value));
                }
                else if (fi.FieldType.UnderlyingSystemType == typeof(string))
                {
                    fi.SetValue(this, value);
                }
                else if (fi.FieldType.UnderlyingSystemType == typeof(Microsoft.Xna.Framework.Graphics.Color))
                {
                    // Special handler for colors. Convert color "name" to static property on the XNA color object.
                    PropertyInfo si = new Microsoft.Xna.Framework.Graphics.Color().GetType().GetProperty(value);
                    fi.SetValue(this, si.GetValue(null, null)); // Access static properties.
                }
                else if (fi.FieldType.UnderlyingSystemType.IsSubclassOf(typeof(Enum)))
                {
                    // Enums items are actually fields...
                    Type enumType = fi.FieldType.UnderlyingSystemType;
                    FieldInfo enumInfo = enumType.GetField(value);

                    // Get value of enum
                    object enumValue = enumInfo.GetValue(fi.FieldType.UnderlyingSystemType);

                    //object newEnumValue = Enum.ToObject( enumType, enumValue );
                    fi.SetValue(this, enumValue); //newEnumValue );
                }
                else if (fi.FieldType.UnderlyingSystemType is Type)
                {
                    Assembly asm = Assembly.LoadFrom("GameEditor.xgc.dll"); // TODO - Use passed in name.
                    fi.SetValue( this, asm.GetType(value));

                }
                else
                {
                    throw new Exceptions.CompileException("Unsupported type for value " + propName + " with type of " + fi.FieldType);
                }
                return;
            }
            else
            {
                if (bThrowIfDoesntExist)
                {
                    throw new xgc3.Exceptions.RuntimeEnvException("Unknown property: " + this.GetType().ToString() + "." + propName + "  -- Check capitalization.");
                }
            }

        }

        // The loader needs some helpers to set properties
        public void SetProperty(string propName, string value)
        {
            SetProperty(propName, value, true);
        }

        // I may not need XML persistence, if the XML is always the SOURCE!
        //
        // GOOD: In other words the "GAME EDITOR" always adds items to XML when a new
        //  instance is added to a game during design process.

        // BAD: Alternative is that the SYMBOL-TABLE and RUNNING-OBJECT-TABLE are always
        //  assumed to be correct. I just "dump" those object to generate the XML.
        //  The problem is that the event handlers aren't easy to "get" from objects
        //  and classes.

#if OLD_WAY
        public string ToXml()
        {
            _values = new System.Collections.Hashtable();
            System.Type t = this.GetType();
            foreach (System.Reflection.PropertyInfo pi in t.GetProperties())
            {
                if (pi.CanWrite && pi.CanRead)
                {
                    _values.Add(pi, pi.GetValue(this, null));
                }
            }
        }
        public void FromXml(string xml)
        {
            foreach (System.Reflection.PropertyInfo key in _values.Keys)
            {
                key.SetValue(this, _values[key], null);
            }
        }
        private System.Collections.Hashtable _values;
#endif
    }

}

