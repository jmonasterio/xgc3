using System;
using System.Collections.Generic;
using System.Text;
using xgc3.Core;
using xgc3.GameObjects; 

namespace xgc3.RuntimeEnv
{

    /// <summary>
    /// Keeps track of all "focused" instances. Usually there is only one. 
    /// So that:
    /// 1) We can send "focus-only" events like Mouse/keyboard/controller to only object(s) with focus.
    /// 
    /// NOTE: Only objects that descend from GameObjects.View can get focus.
    /// </summary>
    public class FocusManager : BaseRuntimeEnvInstance
    {
        //private View m_focusedView;

        /// <summary>
        /// Return all controls that fire the specified event.
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        //public View GetFocusedView()
      //  {
           // return m_focusedView;
     //   }

#if TBD
        /// <summary>
        /// Add a control to focus list.
        /// </summary>
        /// <param name="instance"></param>
        public void SetFocus(View view)
        {
            View oldView = m_focusedView;
            View newView = view;
            m_focusedView = newView;

            // Throw away old list
            if (oldView != null)
            {
                oldView.LostFocus(oldView, newView, null);
            }
            newView.GotFocus(newView, oldView, null);
        }

        /// <summary>
        /// Remove a control from focus list. Set focus to "next" control.
        /// </summary>
        /// <param name="instance"></param>
        public void ClearFocus()
        {
            m_focusedView.LostFocus( m_focusedView, null, null);
            m_focusedView = null;

            System.Diagnostics.Debug.Assert(false, "How do I figure out what next focus control is?");

        }
#endif
    }
}
