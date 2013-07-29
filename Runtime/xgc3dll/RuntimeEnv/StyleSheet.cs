using System;
using System.Collections.Generic;
using System.Text;
using xgc3.Core;

namespace xgc3.RuntimeEnv
{
    public class StyleAttribute : Instance
    {
        public string Property;
        public string Value;
    }

    public enum SelectorType { Id = 0, Type = 1, CssClass = 2, Universal = 3 }; // By order of specificity

    public class Selector : Instance
    {
        public string Selects;

        /// <summary>
        /// TODO: Generate an accessor like this for any class that has the ChildrenType attribute
        /// </summary>
        public IEnumerable<StyleAttribute> StyleAttributes
        {
            get { return Children.Convert<StyleAttribute>(); }
        }

        public SelectorType Type;


        public Selector Copy()
        {
            Selector s = new Selector();
            s.Selects = this.Selects;
            s.Type = this.Type;
            foreach (KeyValuePair<string, Instance> pair in Children)
            {
                s.Children.Add(pair.Key, pair.Value);
            }
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src">Style to merge into THIS one.</param>
        /// <param name="selector">Which particular selector to merge</param>
        public void Merge(Selector s)
        {
            foreach (KeyValuePair<string, Instance> pair in s.Children)
            {
                if (this.Children.ContainsKey(pair.Key))
                {
                    this.Children[pair.Key] = pair.Value;
                }
                else
                {
                    this.Children.Add(pair.Key, pair.Value);
                }
            }
        }

        /// <summary>
        /// Apply a selectors style to an instance.
        /// </summary>
        /// <param name="instance"></param>
        public void Apply(Instance instance)
        {
            foreach (StyleAttribute attr in this.StyleAttributes)
            {
                instance.TrySetProperty(attr.Property, attr.Value);
            }
        }

    }

    public class StyleSheet : Instance
    {
        public IEnumerable<Selector> Selectors
        {
            get
            {
                return Children.Convert<Selector>();
            }
        }

    }

    public class StyleSheetManager : BaseRuntimeEnvInstance
    {
        private StyleSheet m_StylesForId = new StyleSheet();
        private StyleSheet m_StylesForCssClass = new StyleSheet();
        private StyleSheet m_StylesForType = new StyleSheet();
        private StyleSheet m_UniversalStyle = new StyleSheet();

        public void AddStyleSheet(StyleSheet newStyleSheet)
        {
            // Split selectors from style sheet into different groups to make it easier to apply the
            //  cascade later
            foreach (Selector selector in newStyleSheet.Children.Convert<Selector>())
            {
                if (selector.Type == SelectorType.Id)
                {
                    // NOTE: THe use of selector.Selects -- we've changed the key to make lookup of the "id" quick.
                    // Same for other types of selectors below.
                    m_StylesForId.Children.Add(selector.Selects, selector);
                }
                else if( selector.Type == SelectorType.CssClass )
                {
                    m_StylesForCssClass.Children.Add(selector.Selects, selector);
                }
                else if( selector.Type == SelectorType.Type)
                {
                    m_StylesForType.Children.Add(selector.Selects, selector);
                }
                else if( selector.Type == SelectorType.Universal)
                {
                    m_UniversalStyle.Children.Add(selector.Selects, selector);
                }
                else
                {
                    throw new Exceptions.RuntimeEnvException( "Uknown stylesheet selector: " + selector.Type);
                }

            }
        }

        /// <summary>
        /// First implementation is a very simple cascade:
        /// (1) Apply universal styles, if applicable.
        /// (2) If found, apply styles
        /// </summary>
        /// <param name="instance"></param>
        public void ApplyStyleCascadeToInstance( Instance instance )
        {
            // Create selector by building up cascade
            Selector selectorToApply = new Selector();

            // Start with all universal selectors
            // TODO: Why do this on every call? Can't we precalculate the universal ones?
            foreach( Selector selector in m_UniversalStyle.Children.Convert<Selector>())
            {
                selectorToApply.Merge( selector as Selector);
            }

            // Replace with type styles, if applicable
            if (m_StylesForType.Children.ContainsKey(instance.GetSimpleClassName()))
            {
                selectorToApply.Merge(m_StylesForType.Children[instance.GetSimpleClassName()] as Selector);
            }

            // CssClass attribute styles override all previous 
            if (m_StylesForCssClass.Children.ContainsKey(instance.CssClass))
            {
                selectorToApply.Merge(m_StylesForCssClass.Children[instance.CssClass] as Selector);
            }

            // Id attributes override all previous
            if (m_StylesForId.Children.ContainsKey(instance.Name))
            {
                selectorToApply.Merge(m_StylesForId.Children[instance.Name] as Selector);
            }

            // We now have a list of styles to apply to this instance, but not all of them
            //  may be real properties of the instance. Try each one.
            selectorToApply.Apply(instance);
        }
    }
}
