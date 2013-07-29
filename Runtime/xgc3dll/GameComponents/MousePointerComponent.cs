using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace xgc3.GameComponents
{
#if !XBOX360
    public class MousePointerComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Texture2D m_texturePointer;
        private IMouseInputManager m_mim;

        public MousePointerComponent(Game game)
            : base(game)
        {
            // Make sure this is on top of all other components.
            this.DrawOrder = 10000;

        }

        protected override void LoadContent()
        {
            m_texturePointer = this.Game.Content.Load<Texture2D>("pointer");

#if !XBOX360
            m_mim = this.Game.Services.GetService(typeof(IMouseInputManager)) as IMouseInputManager;
#endif

            base.LoadContent();
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            using (SpriteBatch sb = new SpriteBatch(this.GraphicsDevice))
            {
                sb.Begin(SpriteBlendMode.AlphaBlend);
                sb.Draw(m_texturePointer, m_mim.MouseLocation, Color.White);
                //System.Diagnostics.Debug.WriteLine(m_mim.MouseLocation.X);
                sb.End();
            }
            base.Draw(gameTime);
        }

    }
#endif
}
