using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace xgc3.GameComponents
{
#if !XBOX360

    public enum ExtendedMouseButtonState
    {
        None = 0,
        MouseDown = 1,
        MouseUp = 2,
        //MouseClick = 3,
        //MouseDblClick = 4
    }


    /// <summary>
    /// Service Interface
    /// </summary>
    public interface IMouseInputManager
    {
        ExtendedMouseButtonState LeftButtonStateEx { get; }
        ExtendedMouseButtonState RightButtonStateEx { get; }
        Vector2 MouseLocation { get; }
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public partial class MouseInputComponent : Microsoft.Xna.Framework.GameComponent, IMouseInputManager
    {
        private ButtonState m_lastLeftMouseState;
        private ButtonState m_lastRightMouseState;

        private ExtendedMouseButtonState m_leftButtonExtendedState;
        private ExtendedMouseButtonState m_rightButtonExtendedState;

        private Vector2 m_loc;

        public MouseInputComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here

        }

        public override void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();

            m_loc = new Vector2(ms.X, ms.Y);

            if (ms.LeftButton != m_lastLeftMouseState)
            {
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    m_leftButtonExtendedState = ExtendedMouseButtonState.MouseDown;
                }
                else if (ms.LeftButton == ButtonState.Released)
                {
                    m_leftButtonExtendedState = ExtendedMouseButtonState.MouseUp;
                }
                else
                {
                    m_leftButtonExtendedState = ExtendedMouseButtonState.None;
                }
            }

            if (ms.RightButton != m_lastRightMouseState)
            {
                if (ms.RightButton == ButtonState.Pressed)
                {
                    m_rightButtonExtendedState = ExtendedMouseButtonState.MouseDown;
                }
                else if (ms.RightButton ==ButtonState.Released)
                {
                    m_rightButtonExtendedState = ExtendedMouseButtonState.MouseUp;
                }
                else
                {
                    m_rightButtonExtendedState = ExtendedMouseButtonState.None;
                }
            }

            m_lastLeftMouseState = ms.LeftButton;
            m_lastRightMouseState = ms.RightButton;


            base.Update(gameTime);
        }

        public ExtendedMouseButtonState LeftButtonStateEx
        {
            get
            {
                return m_leftButtonExtendedState;
            }
        }

        public ExtendedMouseButtonState RightButtonStateEx
        {
            get
            {
                return m_rightButtonExtendedState;
            }
        }

        public Vector2 MouseLocation
        {
            get
            {
                return m_loc;
            }
        }
    }
#endif
}



