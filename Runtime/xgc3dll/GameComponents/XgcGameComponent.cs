using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

using xgc3.Core;
using xgc3.RuntimeEnv;
using xgc3.GameObjects;

namespace xgc3.GameComponents
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class XgcGameComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
#if !XBOX360
        private IMouseInputManager MouseMgr;
#endif
        public IFontManager FontMgr;

        /// <summary>
        /// Set to true if game just started, so we can fire the
        ///     GameStarted event for all instances (if, necessary)
        /// </summary>
        public bool IsJustStarted;
        public bool IsJustEnded;
        public bool IsJustCloseButton; // Close button just clicked.

        public Instance CurrentViewInstance;

        public SpriteBatch SpriteB;

        private MouseState m_MouseStateLast = new MouseState();
        private GamePadState m_GamePadStateLast = new GamePadState();

        private View m_focusedView = null;
        private View m_mouseInView = null;

        private GameTimer m_gameTimer = new GameTimer();


        public XgcGameComponent(Game game, GameManager gm)
            : base(game)
        {
            // TODO: Construct any child components here
            IsJustStarted = true;
            IsJustEnded = false;
        }

        /// <summary>
        /// Force ENDGAME event.
        /// </summary>
        public void EndGame()
        {
            IsJustEnded = true;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
#if !XBOX360
            MouseMgr = this.Game.Services.GetService(typeof(IMouseInputManager)) as IMouseInputManager;
#endif
            FontMgr = this.Game.Services.GetService(typeof(IFontManager)) as IFontManager;

            // TODO: Add your initialization logic here
            if (this.SpriteB == null)
            {
                this.SpriteB = new SpriteBatch(this.GraphicsDevice);
            }

            XnaGame xgcGame = this.Game as XnaGame;

        }

        public delegate void ChildRecursor(Instance instance);

        private void RecurseChildren(Instance instance, ChildRecursor d )
        {
            foreach( Instance child in instance.Children.Values)
            {
                RecurseChildren( child, d);

                d.Invoke( child);
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your drawing code here

            XnaGame xgcGame = this.Game as XnaGame;

            CurrentViewInstance = xgcGame.Room;

            FocusManager evm = xgcGame.GameMgr.FocusManager;


            // Fire events for every instance...
            /* Order of events 
            Begin step events 
            Alarm events 
            Keyboard, Key press, and Key release events 
            Mouse events 
            Normal step events 
            (now all instances are set to their new positions) 
            Collision events 
            End step events 
            Drawing events 
             */
            Keys[] keys = Keyboard.GetState().GetPressedKeys();
#if !XBOX360
            MouseState mouseState = Mouse.GetState();
#endif
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                if (instance.IsJustCreated)
                {
                    instance.IsJustCreated = false;
                    instance.Raise_Create(instance, null, null);
                }
            });
        

            // Fire DESTROY event. Should this be last???
            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                if (instance.IsJustDestroyed)
                {
                    instance.IsJustDestroyed = false;
                    instance.Raise_Destroy(instance, null, null);

                    // TODO: remove from instances...
                    CurrentViewInstance.Children.Remove(instance.Name);

                    // Don't do any more events -- we're destroyed.
                    //continue;
                }
            });

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                if (this.IsJustStarted)
                {
                    instance.Raise_GameStart(instance, null, null); // TODO: OTHER is game object?
                }
            });

            this.IsJustStarted = false;

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                if (this.IsJustEnded)
                {
                    instance.Raise_GameEnd(instance, null, null); // TODO: OTHER is game object?
                }
            });

            this.IsJustEnded = false;

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
                {
                    if (CurrentViewInstance.IsJustCreated)
                    {
                        instance.Raise_RoomStart(instance, CurrentViewInstance, null);
                    }
                });

            CurrentViewInstance.IsJustCreated = false;

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                if (CurrentViewInstance.IsJustDestroyed)
                {
                    instance.Raise_RoomEnd(instance, CurrentViewInstance, null);
                }
            });

            CurrentViewInstance.IsJustDestroyed = false;

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                if (instance.Lives <= 0)
                {
                    instance.Raise_NoMoreLives(instance, null, null);
                }
            });

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                if (instance.Health <= 0)
                {
                    instance.Raise_NoMoreHealth(instance, null, null);
                }
            });

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                instance.Raise_EndOfAnimation(instance, null, null);
            });

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                if (instance.IsEndOfPath)
                {
                    instance.Raise_EndOfPath(instance, null, null);
                }
            });

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                if (this.IsJustCloseButton)
                {
                    instance.Raise_CloseButton(instance, null, null);
                }
            });

            this.IsJustCloseButton = false;

            /* TODO
            foreach (Instance instance in CurrentViewInstance.Children.Values)
            {
                // Support more than 1...
                if (instance.HasUserDefinedEvent())
                {
                    instance.UserEvent( instance, (this, "UserEvent", null);
                }
            }
             * */

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                instance.Raise_BeginStep(instance, null, null);
            });

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {

                if (instance.IsJustAlarmed)
                {
                    instance.Raise_Alarm(instance, null, null);
                }
            });

#if TBD
            // Key events go to focused control.
            // TODO: TAB/SHIFT-TAB can change focused control.
            if (keys.Length > 0)
            {
                View view = evm.GetFocusedView();
                if( view.KeyPress != null)
                {
                    view.KeyPress( view, null, keys.ToString() );
                }
            }
#endif


#if !XBOX360
            // Mouse events go to?
            //   HitTest?
            //   Bubble?
            //

            /*
             * Per .NET specs...
             *  Mouse events occur in the following order:

                   1.      MouseEnter
                   2.      MouseMove
                   3.      MouseHover / MouseDown / MouseWheel
                   4.      MouseUp
                   5.      MouseLeave 
             */

            if (m_mouseInView != null)
            {
                if ((mouseState.LeftButton == ButtonState.Pressed) && (m_MouseStateLast.LeftButton == ButtonState.Released))
                {
                    m_mouseInView.Raise_LeftMouseDown(m_mouseInView, null, null);

                    // Focus change?
                    if (m_mouseInView != m_focusedView)
                    {
                        if (m_mouseInView.AcceptsFocus)
                        {
                            if ((m_focusedView != null))
                            {
                                m_focusedView.Raise_LostFocus(m_focusedView, m_mouseInView, null);
                            }

                            m_focusedView = m_mouseInView;

                            m_focusedView.Raise_GotFocus(m_focusedView, null, null);
                        }
                    }
                }
                else if ((mouseState.LeftButton == ButtonState.Released) && (m_MouseStateLast.LeftButton == ButtonState.Pressed))
                {
                    m_mouseInView.Raise_LeftMouseUp(m_mouseInView, null, null);
                }
            }


            // Right mouse button handling. Does not affect focus, currently.
            if (m_mouseInView != null)
            {
                if ((mouseState.RightButton == ButtonState.Pressed) && (m_MouseStateLast.RightButton == ButtonState.Released))
                {
                    m_mouseInView.Raise_RightMouseDown(m_mouseInView, null, null);
                }
                else if ((mouseState.RightButton == ButtonState.Released) && (m_MouseStateLast.RightButton == ButtonState.Pressed))
                {
                    m_mouseInView.Raise_RightMouseUp(m_mouseInView, null, null);
                }
            }

            int mx = mouseState.X;
            int my = mouseState.Y;

            // Find mouse in....
            // LAST: OUT, CURRENT: IN -> MOUSE IN + MOUSE OVER
            // LAST: IN, CURRENT: OUT -> MOUSE OUT
            // LAST: IN, CURRENT: IN, LASTPOS!=CURPOS -> MOUSE OVER (NOT IMPLEMENTED THIS WAY!)
            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                View view = instance as View;
                if (view != null)
                {
                    if (view.MouseIsIn)
                    {
                        if (!view.HitTest(mx, my))
                        {
                            view.Raise_MouseOut(view, null, null);
                            view.MouseIsIn = false;
                            m_mouseInView = null;
                        }
                        else
                        {
                            // TODO: Should we only do this if mouse moved?
                            view.Raise_MouseOver(view, null, null);

                        }
                    }
                    else if (!view.MouseIsIn)
                    {
                        if (view.HitTest(mx, my))
                        {
                            view.MouseIsIn = true;
                            view.Raise_MouseIn(view, null, null);
                            m_mouseInView = view;
                        }
                    }
                }
            });
             



#endif

#if TBD
            // TODO: Add more events here
            if (padState.IsButtonDown(Buttons.A))
            {
                View view = evm.GetFocusedView();
                if (view != null)
                {
                    view.Raise_PadButtonA(view, null, null);
                }
            }
#endif

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                m_gameTimer.Step(gameTime);
                instance.Raise_Step(instance, m_gameTimer, null);
            });

            // (now all instances are set to their new positions)

            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                // TODO: Need to check collisions against all objects.
                // Can be made more efficient by only checking collisions
                //  if there is a handler for that collision.
                if (instance.IsCollision())
                {
                    instance.Raise_Collision(instance, null, null);
                }
            });

#if ROOM_SUPPORT
            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                if ( (CurrentRoomInstance).IsInstanceBoundaryHit(instance))
                {
                    instance.CallEventHandler(this, "Boundary", CurrentRoomInstance);
                }
            });
            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                if ( (CurrentRoomInstance).IsInstanceOutside(instance))
                {
                    instance.CallEventHandler(this, "Outside", CurrentRoomInstance);
                }
            });
#endif

#if HANDLE_VIEWS
            TODO
            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                if ( CurrentView.Bou())
                {
                    instance.CallEventHandler(this, "BoundaryView", null);
                }
            });
            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                if ( CurrentView.OutsideView())
                {
                    instance.CallEventHandler(this, "OutsideView", null);
                }
            });
#endif
            RecurseChildren(CurrentViewInstance, delegate(Instance instance)
            {
                instance.Raise_EndStep(instance, null, null);
            });

            m_MouseStateLast = CopyMouseState(mouseState);
            m_GamePadStateLast = CopyGamePadState(padState);

            // TODO: Add your update code here
            base.Update(gameTime);

        }

        private MouseState CopyMouseState(MouseState oldMouseState)
        {
            return new MouseState(oldMouseState.X, oldMouseState.Y, oldMouseState.ScrollWheelValue,
                oldMouseState.LeftButton, oldMouseState.MiddleButton,
                oldMouseState.RightButton, oldMouseState.XButton1,
                oldMouseState.XButton2);
        }

        private GamePadState CopyGamePadState(GamePadState oldPadState)
        {
            return new GamePadState(oldPadState.ThumbSticks, oldPadState.Triggers, oldPadState.Buttons, oldPadState.DPad);
        }

        public override void Draw(GameTime gameTime)
        {


            // TODO: Add your drawing code here
                XnaGame xgcGame = this.Game as XnaGame;

                SpriteB.Begin(SpriteBlendMode.AlphaBlend);

                // TODO: Shouldn't it be every instance?
                foreach (Instance instance in CurrentViewInstance.Children.Values )
                {
                    // TODO: Maybe we should be deleting from children when destroyed?
                    if (instance.IsJustDestroyed)
                    {
                        continue;
                    }


                    // Only sprite instances are drawable...
                    // Would design be better with IDrawable()...
                    if (instance is View )
                    {
                        View view = instance as View;
                        view.Raise_Draw(instance, null, null);
                    }
                }
                SpriteB.End();

                base.Draw(gameTime);
 
        }
    }
}