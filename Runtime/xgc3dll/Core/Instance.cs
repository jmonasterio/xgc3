using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using xgc3.RuntimeEnv;

namespace xgc3.Core
{
    /// <summary>
    /// Runtime instance of an object.
    /// </summary>
    public class Instance : BaseInstanceClass 
    {
        /// <summary>
        /// All instances are containers for more instances.
        /// </summary>
        public Map<Instance> Children = new Map<Instance>();

        public string ChildrenTypeName = null;
        public Type ChildrenType 
        {
            get
            {
                if (null == ChildrenTypeName)
                {
                    return null;
                }
                return GameMgr.SymbolTable.GetType(ChildrenTypeName);
            }
            set
            {
                if (null == value)
                {
                    ChildrenTypeName = null;
                    return;
                }
                ChildrenTypeName = value.ToString();
            }
        } // Internal -- if non-null, then indicates that Children must be of type "ChildrenType".

        /// <summary>
        /// Gives access back to top-level game object.
        /// </summary>
        public GameManager GameMgr;

        public string Name;
        public string CssClass = ""; // For styling. Assume no class unless specified.
        public string FullName; // Internal
        public string FullFileName; // Internal
        public string Clones; // Internal
        public Instance Parent;

        public bool IsJustCreated = false;
        public bool IsJustDestroyed = false;
        public int Lives = 3;
        public int Health = 100;
        public bool IsEndOfAnimation = false;
        public bool IsEndOfPath = false;
        public bool IsCloseButton = false;
        public bool IsJustAlarmed = false;

        /* Why I don't use the event keyword: http://books.google.com/books?id=kSchquQcPwwC&pg=PA300&lpg=PA300&dq=%22can+only+appear+on+the+left+hand+side+of+%2B%3D+or+-%3D+(except+when+used+from+within+the+type%22&source=web&ots=-tlYHqlpL-&sig=9JAn9TthQ_O4d-ub7qjlGU1eP7M&hl=en */
        /* Basically, I'd have to add an extra method to fire event from event manager */
        public event EventDelegate Create = Instance.On_Create;
        public event EventDelegate Destroy;
        public event EventDelegate GameStart;
        public event EventDelegate GameEnd;
        public event EventDelegate RoomStart;
        public event EventDelegate RoomEnd;
        public event EventDelegate NoMoreLives;
        public event EventDelegate NoMoreHealth;
        public event EventDelegate EndOfAnimation;
        public event EventDelegate EndOfPath;
        public event EventDelegate CloseButton;
        public event EventDelegate Step = Instance.On_Step;
        public event EventDelegate BeginStep;
        public event EventDelegate JustAlarmed;
        public event EventDelegate Alarm;
        public event EventDelegate PadButtonA;
        public event EventDelegate Collision;
        public event EventDelegate EndStep;

        public void Raise_Create(Instance self, Instance other, string extra)
        {
            if (Create != null) 
                { Create(self, other, extra); }
        }

        public void Raise_Destroy(Instance self, Instance other, string extra)
        {
            if (Destroy != null) { Destroy(self, other, extra); }
        }

        public void Raise_GameStart(Instance self, Instance other, string extra)
        {
            if (GameStart != null) { GameStart(self, other, extra); }
        }

        public void Raise_GameEnd(Instance self, Instance other, string extra)
        {
            if (GameEnd != null) { GameEnd(self, other, extra); }
        }

        public void Raise_RoomStart(Instance self, Instance other, string extra)
        {
            if (RoomStart != null) { RoomStart(self, other, extra); }
        }

        public void Raise_RoomEnd(Instance self, Instance other, string extra)
        {
            if (RoomEnd != null) { RoomEnd(self, other, extra); }
        }

        public void Raise_NoMoreLives(Instance self, Instance other, string extra)
        {
            if (NoMoreLives != null) { NoMoreLives(self, other, extra); }
        }

        public void Raise_NoMoreHealth(Instance self, Instance other, string extra)
        {
            if (NoMoreHealth != null) { NoMoreHealth(self, other, extra); }
        }

        public void Raise_EndOfAnimation(Instance self, Instance other, string extra)
        {
            if (EndOfAnimation != null) { EndOfAnimation(self, other, extra); }
        }

        public void Raise_EndOfPath(Instance self, Instance other, string extra)
        {
            if (EndOfPath != null) { EndOfPath(self, other, extra); }
        }

        public void Raise_CloseButton(Instance self, Instance other, string extra)
        {
            if (CloseButton != null) { CloseButton(self, other, extra); }
        }

        public void Raise_BeginStep(Instance self, Instance other, string extra)
        {
            if (BeginStep != null) { BeginStep(self, other, extra); }
        }

        public void Raise_JustAlarmed(Instance self, Instance other, string extra)
        {
            if (JustAlarmed != null) { JustAlarmed(self, other, extra); }
        }

        public void Raise_Alarm(Instance self, Instance other, string extra)
        {
            if (Alarm != null) { Alarm(self, other, extra); }
        }


        public void Raise_PadButtonA(Instance self, Instance other, string extra)
        {
            if (PadButtonA != null) { PadButtonA(self, other, extra); }
        }

        public void Raise_Step(Instance self, Instance other, string extra)
        {
            if (Step != null) { Step(self, other, extra); }
        }

        public void Raise_Collision(Instance self, Instance other, string extra)
        {
            if (Collision != null) { Collision(self, other, extra); }
        }
        public void Raise_EndStep(Instance self, Instance other, string extra)
        {
            if (EndStep != null) { EndStep(self, other, extra); }
        }

        public Instance()
        {
            /* Why listen, if I do nothing? */
            /*
            Create += new EventDelegate(On_Create);
            Destroy += new EventDelegate(On_Destroy);
            GameStart += new EventDelegate(On_GameStart);
            GameEnd += new EventDelegate(On_GameEnd);
            RoomStart += new EventDelegate(On_RoomStart);
            RoomEnd += new EventDelegate(On_RoomEnd);
             * */
        }

        public bool IsCollision()
        {
            // TODO: This is not going to work.
            // Need to know what kinds of objects to check for collisions against.
            // Only care if there is an event handler for that type of collision.
            return false;
        }

        /*
        private void On_Destroy(Instance self, Instance other, string extra)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private void On_Create(Instance self, Instance other, string extra)
        {
            throw new Exception("The method or operation is not implemented.");
        }
         * */


        // The loader needs some helpers to set properties
        internal void AddEventHandler( string eventName)
        {
            // See this example: http://msdn.microsoft.com/en-us/library/system.reflection.eventinfo.addeventhandler.aspx

            // eventName must be: public event eventdelgate...
            EventInfo ei = (this).GetType().GetEvent(eventName);
            if (ei != null)
            {
                // Instances class has a method called: On_XXXX. Where XXXX is the event name.
                // This is typically PRIVATE, so that each instance can override base classes.
                MethodInfo implOfHandler = this.GetType().GetMethod("On_" + eventName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (implOfHandler != null)
                {

                    // Use the MethodInfo to create a delegate of the correct 
                    // type, and call the AddEventHandler method to hook up 
                    // the event.
                    Delegate d = Delegate.CreateDelegate(ei.EventHandlerType, this, implOfHandler);
                    ei.AddEventHandler(this, d);
                }
                else
                {
                    throw new xgc3.Exceptions.RuntimeEnvException("Cannot find event implementation: " + eventName);
                }
            }
            else
            {
                throw new xgc3.Exceptions.RuntimeEnvException("Unknown event: " + eventName + "  -- Check capitalization.");
            }
        }

        /// <summary>
        /// Return array of all events supported by class.
        /// </summary>
        /// <returns></returns>
        public List<string> AllEvents
        {
            get
            {
                List<string> events = new List<string>();
                EventInfo[] eis = this.GetType().GetEvents();
                foreach (EventInfo ei in eis)
                {
                    // TODO: Fullname?
                    events.Add(ei.Name);
                }
                return events;
            }
        }

        /// <summary>
        /// Base implementation.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="extra"></param>
        static private void On_Create(xgc3.Core.Instance self, xgc3.Core.Instance other, String extra)
        {

        }

        /// <summary>
        /// Base implementation.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <param name="extra"></param>
        static private void On_Step(xgc3.Core.Instance self, xgc3.Core.Instance other, String extra)
        {

        }

        public Instance This
        {
            get
            {
                return this;
            }
        }




    }
}
