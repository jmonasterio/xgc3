using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace xgc3.GameComponents
{
    public interface IFontManager
    {
        SpriteFont Courier { get; }
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FontManagerComponent : Microsoft.Xna.Framework.GameComponent, IFontManager
    {
        private SpriteFont m_courier;

        public FontManagerComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            m_courier = this.Game.Content.Load<SpriteFont>("Courier New");

            base.Initialize();
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public SpriteFont Courier
        {
            get { return m_courier; }
        }
    }
}