using System;
using System.Collections.Generic;
using System.Text;

using xgc3.Core;

namespace xgc3.GameObjects
{
    public class Room : Container
    {
        /// <summary>
        /// Keep looking up the stack to find the room that an instance is in.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public static Room FindRoomContaining(Instance child)
        {
            // Special case for self
            if (child is Room)
            {
                return child as Room;
            }

            while (child.Parent != null)
            {
                if (child.Parent is Room)
                {
                    return child.Parent as Room;
                }
                else
                {
                    // Recurse
                    FindRoomContaining(child.Parent);
                }
            }

            // Room not found. Probably unlikely.
            return null;
        }      

    }
}
