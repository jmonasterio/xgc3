using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using xgc3.Core;
using xgc3.GameComponents;
using xgc3.RuntimeEnv;
using xgc3.GameObjects;

namespace xgc3
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class XgcGame : Microsoft.Xna.Framework.Game
    {
        private IMouseInputManager MouseMgr;
        public IFontManager FontMgr;

        GraphicsDeviceManager graphics;
        public XgcGameComponent GameComponent;
        public GameManager GameMgr;
        public Instance Room;

        public XgcGame( GameManager gm)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Create all game components.
            FontManagerComponent fm = new FontManagerComponent(this);
            this.Components.Add( fm);
            this.Services.AddService(typeof(IFontManager), fm);

            this.Components.Add(new FpsCounterComponent(this));

            this.Components.Add(new MousePointerComponent(this));

            MouseInputComponent mim = new MouseInputComponent(this);
            this.Components.Add(mim);
            this.Services.AddService(typeof(IMouseInputManager), mim);

            XgcGameComponent xgc = new XgcGameComponent(this, gm);
            this.Components.Add( xgc);
            GameComponent = xgc;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            MouseMgr = this.Services.GetService(typeof(IMouseInputManager)) as IMouseInputManager;
            FontMgr = this.Services.GetService(typeof(IFontManager)) as IFontManager;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Set the current room.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="symTable"></param>
        public void GotoRoom(Room room, GameManager gm)
        {
            Room = room;
            GameMgr = gm;
        }

    }
}
