using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using xgc3.Core;
using xgc3.RuntimeEnv;

namespace xgc3.Resources
{
    public class SpriteResource : BaseResource
    {
        private Texture2D Texture;

        protected SpriteResource()
        {
        }

        public SpriteResource(  string name, string assetName)
        {
            this.Name = name;
            this.AssetName = assetName;

            // TODO: If lazy-load is no good, then another method would be to "register" the class here
            //  with some global "loader" that would then load all the resources at load time (instead of 
            //  while the game/app is already running).

            // Another alternative would be to modify the loader to look for all <RESOURCE> tags on CLASSES and INSTANCES
            //  and load all of them ONCE at the start of the app.
        }

        /// <summary>
        /// Texture is lazy loaded, when needed.
        /// </summary>
        /// <param name="gm"></param>
        /// <returns></returns>
        public Texture2D GetTexture(GameManager gm)
        {
            if (Texture != null)
            {
                return this.Texture;
            }
            Texture = gm.Game.Content.Load<Texture2D>(this.AssetName);
            return this.Texture;
        }
    }
}
