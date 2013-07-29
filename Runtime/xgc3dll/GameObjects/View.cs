using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using xgc3.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace xgc3.GameObjects
{
    public class View : GameObject
    {
        public int X;
        public int Y;
        public bool MouseIsIn = false;
        public bool AcceptsFocus = false;
        public int Width = 50; 
        public int Height = 50;

        public event EventDelegate KeyPress;
        public event EventDelegate LeftMouseDown;
        public event EventDelegate LeftMouseUp;
        public event EventDelegate RightMouseDown;
        public event EventDelegate RightMouseUp;
        public event EventDelegate MouseOver;
        public event EventDelegate MouseIn;
        public event EventDelegate MouseOut;
        public event EventDelegate Draw;
        public event EventDelegate GotFocus;
        public event EventDelegate LostFocus;

        public void Raise_LeftMouseDown(Instance self, Instance other, string extra)
        {
            if (LeftMouseDown != null) { LeftMouseDown(self, other, extra); }
        }
        public void Raise_LeftMouseUp(Instance self, Instance other, string extra)
        {
            if (LeftMouseUp != null) { LeftMouseUp(self, other, extra); }
        }
        public void Raise_RightMouseDown(Instance self, Instance other, string extra)
        {
            if (RightMouseDown != null) { RightMouseDown(self, other, extra); }
        }
        public void Raise_RightMouseUp(Instance self, Instance other, string extra)
        {
            if (RightMouseUp != null) { RightMouseUp(self, other, extra); }
        }
        public void Raise_Draw(Instance self, Instance other, string extra)
        {
            if (Draw != null) { Draw(self, other, extra); }
        }
        public void Raise_MouseOver(Instance self, Instance other, string extra)
        {
            if (MouseOver != null) { MouseOver(self, other, extra); }
        }
        public void Raise_MouseIn(Instance self, Instance other, string extra)
        {
            if (MouseIn != null) { MouseIn(self, other, extra); }
        }
        public void Raise_MouseOut(Instance self, Instance other, string extra)
        {
            if (MouseOut != null) { MouseOut(self, other, extra); }
        }
        public void Raise_GotFocus(Instance self, Instance other, string extra)
        {
            if (GotFocus != null) { GotFocus(self, other, extra); }
        }
        public void Raise_LostFocus(Instance self, Instance other, string extra)
        {
            if (LostFocus != null) { LostFocus(self, other, extra); }
        }

        public bool HitTest(int x, int y)
        {
            return ((x > X) && (x < X + Width) && (y > Y) && (y < Y + Height));

            // TODO: Fasater?
            //Microsoft.Xna.Framework.Rectangle rc = new Microsoft.Xna.Framework.Rectangle((int)Location.X, (int)Location.Y, (int)W, (int)H);
            //if (rc.Contains(new Point((int)pos.X, (int)pos.Y)))
            //{
            //    return true;
            //}
            //return false;
#if TEST
            // Top-Left
            Microsoft.Xna.Framework.Graphics.Texture2D Texture;

            Microsoft.Xna.Framework.Rectangle rcSrc = new Microsoft.Xna.Framework.Rectangle( 0,0,3,3);
            Microsoft.Xna.Framework.Rectangle rcDst = new Microsoft.Xna.Framework.Rectangle( 0,0,3,3);;
            this.GameMgr.Game.GameComponent.SpriteB.Draw(self.Texture, rcDst, rcSrc, Color.White);

            Microsoft.Xna.Framework.Rectangle rcSrc = new Microsoft.Xna.Framework.Rectangle( 3, 0, 1, 3);
            Microsoft.Xna.Framework.Rectangle rcDst = new Microsoft.Xna.Framework.Rectangle(3, 0, this.Width - 6, 3);
            this.GameMgr.Game.GameComponent.SpriteB.Draw(self.Texture, rcDst, rcSrc, Color.White);

            Microsoft.Xna.Framework.Rectangle rcSrc = new Microsoft.Xna.Framework.Rectangle( Texture.Width-3, 0, 3, 3);
            Microsoft.Xna.Framework.Rectangle rcDst = new Microsoft.Xna.Framework.Rectangle( this.Width-3, 0, 3, 3);
            this.GameMgr.Game.GameComponent.SpriteB.Draw(self.Texture, rcDst, rcSrc, Color.White);
#endif


            //this.GameMgr.Game.GameComponent.SpriteB.Draw((self.Texture, rcDst, rcSrc, Color.White);
            //Microsoft.Xna.Framework.Graphics.Texture2D t = new Microsoft.Xna.Framework.Graphics.Texture2D();
        }

        /// <summary>
        /// Splits a texture into 3x3 grid (assuming top,left is tileSize x tileSize). Same for bottom,right.
        /// 
        /// Then scales the texture onto a sprite batch.
        /// 
        /// Good for scaling things like buttons.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="texture"></param>
        /// <param name="p"></param>
        /// <param name="color"></param>
        /// <param name="tileSize"></param>
        public void DrawTiledTexture(SpriteBatch sb, Texture2D texture, int x, int y, int w, int h, Color color, int tileSize)
        {
            Microsoft.Xna.Framework.Rectangle rcSrc = new Microsoft.Xna.Framework.Rectangle(0, 0, tileSize, tileSize);
            Microsoft.Xna.Framework.Rectangle rcDst = new Microsoft.Xna.Framework.Rectangle(x, y, tileSize, tileSize); ;
            this.GameMgr.Game.GameComponent.SpriteB.Draw(texture, rcDst, rcSrc, Color.White);

            rcSrc = new Microsoft.Xna.Framework.Rectangle(tileSize, 0, texture.Width - (tileSize + tileSize), tileSize);
            rcDst = new Microsoft.Xna.Framework.Rectangle(x + tileSize, y, this.Width - (tileSize + tileSize), tileSize);
            this.GameMgr.Game.GameComponent.SpriteB.Draw(texture, rcDst, rcSrc, Color.White);

            rcSrc = new Microsoft.Xna.Framework.Rectangle(texture.Width - tileSize, 0, tileSize, tileSize);
            rcDst = new Microsoft.Xna.Framework.Rectangle(x + this.Width - tileSize, y, tileSize, tileSize);
            this.GameMgr.Game.GameComponent.SpriteB.Draw(texture, rcDst, rcSrc, Color.White);

            // Middle row

            rcSrc = new Microsoft.Xna.Framework.Rectangle(0, tileSize, tileSize, texture.Height - (tileSize + tileSize));
            rcDst = new Microsoft.Xna.Framework.Rectangle(x, y + tileSize, tileSize, h - (tileSize + tileSize)); ;
            this.GameMgr.Game.GameComponent.SpriteB.Draw(texture, rcDst, rcSrc, Color.White);

            rcSrc = new Microsoft.Xna.Framework.Rectangle(tileSize, tileSize, texture.Width - (tileSize + tileSize), texture.Height - (tileSize + tileSize));
            rcDst = new Microsoft.Xna.Framework.Rectangle(x + tileSize, y + tileSize, this.Width - (tileSize + tileSize), h - (tileSize + tileSize));
            this.GameMgr.Game.GameComponent.SpriteB.Draw(texture, rcDst, rcSrc, Color.White);

            rcSrc = new Microsoft.Xna.Framework.Rectangle(texture.Width - tileSize, tileSize, tileSize, texture.Height - (tileSize + tileSize));
            rcDst = new Microsoft.Xna.Framework.Rectangle(x + this.Width - tileSize, y + tileSize, tileSize, h - (tileSize + tileSize));
            this.GameMgr.Game.GameComponent.SpriteB.Draw(texture, rcDst, rcSrc, Color.White);


            // Last row

            rcSrc = new Microsoft.Xna.Framework.Rectangle(0, texture.Height - tileSize, tileSize, tileSize);
            rcDst = new Microsoft.Xna.Framework.Rectangle(x, y + this.Height - tileSize, tileSize, tileSize); ;
            this.GameMgr.Game.GameComponent.SpriteB.Draw(texture, rcDst, rcSrc, Color.White);

            rcSrc = new Microsoft.Xna.Framework.Rectangle(tileSize, texture.Height - tileSize, 1, tileSize);
            rcDst = new Microsoft.Xna.Framework.Rectangle(x + tileSize, y + this.Height - tileSize, this.Width - 6, tileSize);
            this.GameMgr.Game.GameComponent.SpriteB.Draw(texture, rcDst, rcSrc, Color.White);

            rcSrc = new Microsoft.Xna.Framework.Rectangle(texture.Width - tileSize, texture.Height - tileSize, tileSize, tileSize);
            rcDst = new Microsoft.Xna.Framework.Rectangle(x + this.Width - tileSize, y + this.Height - tileSize, tileSize, tileSize);
            this.GameMgr.Game.GameComponent.SpriteB.Draw(texture, rcDst, rcSrc, Color.White);


            //this.GameMgr.Game.GameComponent.SpriteB.DrawString(self.GameMgr.Game.FontMgr.Courier, self.Text, p, Color.Black, 0, prop, 1, SpriteEffects.

        }



        public enum TextAlignment
        {
            Top,
            Left,
            Middle,
            Right,
            Bottom,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        /// <summary>
        /// Draws a string. 
        /// </summary>
        /// <param name="sb">A reference to a SpriteBatch object that will draw the text.</param>
        /// <param name="fnt">A reference to a SpriteFont object.</param>
        /// <param name="text">The text to be drawn. <remarks>If the text contains \n it
        /// will be treated as a new line marker and the text will drawn acordingy.</remarks></param>
        /// <param name="r">The screen rectangle that the rext should be drawn inside of.</param>
        /// <param name="col">The color of the text that will be drawn.</param>
        /// <param name="align">Specified the alignment within the specified screen rectangle.</param>
        /// <param name="performWordWrap">If true the words within the text will be aranged to rey and
        /// fit within the bounds of the specified screen rectangle.</param>
        /// <param name="offsett">Draws the text at a specified offset relative to the screen
        /// rectangles top left position. </param>
        /// <param name="textBounds">Returns a rectangle representing the size of the bouds of
        /// the text that was drawn.</param>
        public static void DrawString(SpriteBatch sb, SpriteFont fnt, string text, Rectangle r,
            Color col, TextAlignment align, bool performWordWrap, Vector2 offsett, out Rectangle textBounds)
        {
            // check if there is text to draw
            textBounds = r;
            if (text == null) return;
            if (text == string.Empty) return;

            List<string> lines = new List<string>();
            lines.AddRange(text.Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries));

            // calc the size of the rect for all the text
            Rectangle tmprect = ProcessLines(fnt, r, performWordWrap, lines);


            // setup the position where drawing will start
            Vector2 pos = new Vector2(r.X, r.Y);
            int aStyle = 0;

            switch (align)
            {
                case TextAlignment.Bottom:
                    pos.Y = r.Bottom - tmprect.Height;
                    aStyle = 1;
                    break;
                case TextAlignment.BottomLeft:
                    pos.Y = r.Bottom - tmprect.Height;
                    aStyle = 0;
                    break;
                case TextAlignment.BottomRight:
                    pos.Y = r.Bottom - tmprect.Height;
                    aStyle = 2;
                    break;
                case TextAlignment.Left:
                    pos.Y = r.Y + ((r.Height / 2) - (tmprect.Height / 2));
                    aStyle = 0;
                    break;
                case TextAlignment.Middle:
                    pos.Y = r.Y + ((r.Height / 2) - (tmprect.Height / 2));
                    aStyle = 1;
                    break;
                case TextAlignment.Right:
                    pos.Y = r.Y + ((r.Height / 2) - (tmprect.Height / 2));
                    aStyle = 2;
                    break;
                case TextAlignment.Top:
                    aStyle = 1;
                    break;
                case TextAlignment.TopLeft:
                    aStyle = 0;
                    break;
                case TextAlignment.TopRight:
                    aStyle = 2;
                    break;
            }

            // draw text
            for (int idx = 0; idx < lines.Count; idx++)
            {
                string txt = lines[idx];
                Vector2 size = fnt.MeasureString(txt);
                switch (aStyle)
                {
                    case 0:
                        pos.X = r.X;
                        break;
                    case 1:
                        pos.X = r.X + ((r.Width / 2) - (size.X / 2));
                        break;
                    case 2:
                        pos.X = r.Right - size.X;
                        break;
                }
                // draw the line of text
                sb.DrawString(fnt, txt, pos + offsett, col);
                pos.Y += fnt.LineSpacing;
            }

            textBounds = tmprect;
        }

        internal static Rectangle ProcessLines(SpriteFont fnt, Rectangle r, bool performWordWrap, List<string> lines)
        {
            // llop through each line in the collection
            Rectangle bounds = r;
            bounds.Width = 0;
            bounds.Height = 0;
            int index = 0;
            float Width = 0;
            bool lineInserted = false;
            while (index < lines.Count)
            {
                // get a line of text
                string linetext = lines[index];
                //measure the line of text
                Vector2 size = fnt.MeasureString(linetext);

                // check if the line of text is geater then then the rect we want to draw it inside of
                if (performWordWrap && size.X > r.Width)
                {
                    // find last space character in line
                    string endspace = string.Empty;
                    // deal with trailing spaces
                    if (linetext.EndsWith(" "))
                    {
                        endspace = " ";
                        linetext = linetext.TrimEnd();
                    }

                    // get the index of the last space character
                    int i = linetext.LastIndexOf(" ");
                    if (i != -1)
                    {
                        // if there was a space grab the last word in the line
                        string lastword = linetext.Substring(i + 1);
                        // move word to next line
                        if (index == lines.Count - 1)
                        {
                            lines.Add(lastword);
                            lineInserted = true;
                        }
                        else
                        {
                            // prepend last word to begining of next line
                            if (lineInserted)
                            {
                                lines[index + 1] = lastword + endspace + lines[index + 1];
                            }
                            else
                            {
                                lines.Insert(index + 1, lastword);
                                lineInserted = true;
                            }
                        }

                        // crop last word from the line that is being processed
                        lines[index] = linetext.Substring(0, i + 1);

                    }
                    else
                    {
                        // there appear to be no space characters on this line s move to the next line
                        lineInserted = false;
                        size = fnt.MeasureString(lines[index]);
                        if (size.X > bounds.Width) Width = size.X;
                        bounds.Height += fnt.LineSpacing;// size.Y - 1;
                        index++;
                    }
                }
                else
                {
                    // this line will fit so we can skip to the next line
                    lineInserted = false;
                    size = fnt.MeasureString(lines[index]);
                    if (size.X > bounds.Width) bounds.Width = (int)size.X;
                    bounds.Height += fnt.LineSpacing;//size.Y - 1;
                    index++;
                }
            }

            // returns the size of the text
            return bounds;
        }

    }
}
