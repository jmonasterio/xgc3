using Microsoft.Xna.Framework.Graphics;
using xgc3.GameComponents;
using System; using System.Collections.Generic; using System.Text; using xgc3.Core; using xgc3.Resources; using Microsoft.Xna.Framework; using Microsoft.Xna.Framework.Graphics; namespace xgc3.Generated {public class GameInfo : xgc3.RuntimeEnv.BaseRuntimeEnvInstance { 
public String Title;
public GameInfo() { 
}
}
public class Sprite : xgc3.GameObjects.View { 
public String AssetName;
public Texture2D Texture;
private void On_Create( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Sprite self = selfInstance as xgc3.Generated.Sprite; 
            Console.WriteLine( "Spite - Create");
            string resource = self.Resource;
            if (resource != null)
            {
                Texture2D texturePointer = self.GameMgr.Game.Content.Load<Texture2D>(resource);
                self.Texture = texturePointer;
            }
      }
private void On_Destroy( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Sprite self = selfInstance as xgc3.Generated.Sprite; 
            Console.WriteLine( "Spite - Destroy");
            // TODO: Test Leak cleanup 
            if (self.Texture != null)
            {
                self.Texture.Dispose();
                self.Texture = null;
            }
      }
private void On_Draw( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Sprite self = selfInstance as xgc3.Generated.Sprite; 
            //Console.WriteLine( "Sprite - Draw");
            string resource = self.Resource;
            if (resource != null)
            {
                Vector2 p = new Vector2(self.X, self.Y);
                self.GameMgr.Game.GameComponent.SpriteB.Draw(self.Texture, p, Color.White);
            }
            else
            {
                // When no sprite? Just draw name.

                Vector2 p = new Vector2(self.X, self.Y);
                self.GameMgr.Game.GameComponent.SpriteB.DrawString(self.GameMgr.Game.FontMgr.Courier, self.Name, p, Color.Red);
            }
        }
public Sprite() { 
this.Create += On_Create;
this.Destroy += On_Destroy;
this.Draw += On_Draw;
}
}
public class Label : xgc3.GameObjects.View { 
public String Text;
public Microsoft.Xna.Framework.Graphics.Color Color = Microsoft.Xna.Framework.Graphics.Color.Black;
private void On_Create( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Label self = selfInstance as xgc3.Generated.Label; Console.WriteLine( "Hello world!" + self.Name); }
private void On_Draw( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Label self = selfInstance as xgc3.Generated.Label; 
          Vector2 p = new Vector2(self.X, self.Y);
          string text = self.Text;
          if( text == null)
          {
            text = self.Name;
          }
          self.GameMgr.Game.GameComponent.SpriteB.DrawString(self.GameMgr.Game.FontMgr.Courier, text, p, self.Color);
        }
public Label() { 
this.Create += On_Create;
this.Draw += On_Draw;
}
}
public class FPS : xgc3.Generated.Label { 
public string Format = "FPS: {0}";
private void On_Step( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.FPS self = selfInstance as xgc3.Generated.FPS; 
        xgc3.GameObjects.GameTimer gameTimer = other as xgc3.GameObjects.GameTimer;
        this.Text = string.Format( this.Format, Math.Round( gameTimer.FPS, 2));
        }
public FPS() { 
this.Step += On_Step;
}
}
public class TextBox : xgc3.GameObjects.View { 
public String Text;
public Microsoft.Xna.Framework.Graphics.Color Color = Microsoft.Xna.Framework.Graphics.Color.Pink;
public xgc3.GameComponents.ExtendedMouseButtonState MouseState;
private void On_Create( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.TextBox self = selfInstance as xgc3.Generated.TextBox; Console.WriteLine( "Hello world!" + self.Name); }
private void On_Draw( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.TextBox self = selfInstance as xgc3.Generated.TextBox; 
          Vector2 p = new Vector2(self.X, self.Y);
          self.GameMgr.Game.GameComponent.SpriteB.DrawString(self.GameMgr.Game.FontMgr.Courier, self.Text, p, self.Color);
        }
public TextBox() { 
this.Create += On_Create;
this.Draw += On_Draw;
}
}
public class BaseComponent : xgc3.GameObjects.View { 
protected Boolean _enabled = true;
public Boolean Clickable = true;
public BaseComponent() { 
}
}
public class Button : xgc3.Generated.BaseComponent { 
public xgc3.Resources.SpriteResource Resource;
public Boolean Focusable = false;
public Boolean _msdown = false;
public Boolean _msin = false;
public String Text;
public Microsoft.Xna.Framework.Graphics.Color Color;
static public xgc3.Resources.SpriteResource normal = new xgc3.Resources.SpriteResource( "normal", "button_normal");static public xgc3.Resources.SpriteResource down = new xgc3.Resources.SpriteResource( "down", "button_down");static public xgc3.Resources.SpriteResource over = new xgc3.Resources.SpriteResource( "over", "button_over");static public xgc3.Resources.SpriteResource disabled = new xgc3.Resources.SpriteResource( "disabled", "button_disabled");public void On_Click( xgc3.Core.Instance self, xgc3.Core.Instance other, String extra)
{}
public event EventDelegate Click;
public void Raise_Click(Instance self, Instance other, string extra)
	{ if (Click != null) { Click(self, other, extra); }}
public void _callShow()
{
            if ( this._msdown && this._msin ) this.ShowDown();
            else if ( this._msin ) this.ShowOver();
            else this.ShowUp();
        }
public void DoSpaceDown()
{
            if ( this._enabled) {
                this.ShowDown();
            }
            }
public void DoSpaceUp()
{
            if ( this._enabled) {
                this.Raise_Click( this, null, null);
                this.ShowUp();
            }
            }
public void DoEnterDown()
{
      if ( this._enabled ){
      this.ShowDown( );
      }
        }
public void DoEnterUp()
{
      if ( this._enabled ){
        this.Raise_Click( this, null, null);
        this.ShowUp( );
      }
        }
public void _showEnabled()
{
            this.Clickable = _enabled;
            ShowUp();
        }
public void ShowDown()
{
      this.Resource = down;
        }
public void ShowUp()
{
            if (!_enabled ) 
            { 
                this.Resource = disabled;
            } 
            else 
            {
                this.Resource = normal;
            }
        }
public void ShowOver()
{ 
      this.Resource = over;
        }
private void On_MouseOver( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Button self = selfInstance as xgc3.Generated.Button;
      this._msin = true;
      this._callShow();
        }
private void On_MouseOut( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Button self = selfInstance as xgc3.Generated.Button;
      this._msin = false;
      this._callShow();
        }
private void On_LeftMouseDown( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Button self = selfInstance as xgc3.Generated.Button;
      this._msdown = true;
      this._callShow();
        }
private void On_LeftMouseUp( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Button self = selfInstance as xgc3.Generated.Button;
      this._msdown = false;
      this._callShow();
      this.Raise_Click( this, null, null);
        }
private void On_Create( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Button self = selfInstance as xgc3.Generated.Button; 
            Console.WriteLine( "Sprite - Create");
            this.Resource = normal;
      }
private void On_Destroy( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Button self = selfInstance as xgc3.Generated.Button; 
            Console.WriteLine( "Sprite - Destroy");
            // TODO - Free up resources?
      }
private void On_Draw( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Button self = selfInstance as xgc3.Generated.Button; 
            //Console.WriteLine( "Sprite - Draw");
            if (self.Resource != null)
            {
                Vector2 p = new Vector2(self.X, self.Y);
                
                // Scale button graphic...
                DrawTiledTexture( self.GameMgr.Game.GameComponent.SpriteB, self.Resource.GetTexture( self.GameMgr), self.X, self.Y, self.Width, self.Height, Color.White, 5);
                
                // Now, draw the text inside the button.
                Rectangle textBounds;
                Rectangle r = new Rectangle( self.X, self.Y, self.Width, self.Height);
                DrawString( self.GameMgr.Game.GameComponent.SpriteB, self.GameMgr.Game.FontMgr.Courier, self.Text, r,
                    Color.Black, TextAlignment.Middle, true, new Vector2(0,0), out textBounds);

            }
            else
            {
                // When no resource, Just draw name.
                Vector2 p = new Vector2(self.X, self.Y);
                self.GameMgr.Game.GameComponent.SpriteB.DrawString(self.GameMgr.Game.FontMgr.Courier, self.Name, p, Color.Red);
            }
        }
public Button() { 
this.Click += On_Click;
this.MouseOver += On_MouseOver;
this.MouseOut += On_MouseOut;
this.LeftMouseDown += On_LeftMouseDown;
this.LeftMouseUp += On_LeftMouseUp;
this.Create += On_Create;
this.Destroy += On_Destroy;
this.Draw += On_Draw;
}
}
public class Test : xgc3.Core.Instance { 
private void On_Create( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Test self = selfInstance as xgc3.Generated.Test; Console.WriteLine( "Hello world!"); }
public Test() { 
this.Create += On_Create;
}
}
public class Application_GameObjects : xgc3.RuntimeEnv.Application { 
public Application_GameObjects() {}
public class StyleSheet_Sheet1 : xgc3.RuntimeEnv.StyleSheet { 
public StyleSheet_Sheet1() {}
public class Selector_D8F7907EFEA57020C16E3E1698D4F3D4 : xgc3.RuntimeEnv.Selector { 
public Selector_D8F7907EFEA57020C16E3E1698D4F3D4() {}
}
public class Selector_9AE55BF7AD6697C903DBA5F7C073B10E : xgc3.RuntimeEnv.Selector { 
public Selector_9AE55BF7AD6697C903DBA5F7C073B10E() {}
public class StyleAttribute_DE918806BBFCC9A84171C1243B06A0DA : xgc3.RuntimeEnv.StyleAttribute { 
public StyleAttribute_DE918806BBFCC9A84171C1243B06A0DA() {}
}
}
public class Selector_68E92007D6A6BDC06086AA9AAFDE9DAD : xgc3.RuntimeEnv.Selector { 
public Selector_68E92007D6A6BDC06086AA9AAFDE9DAD() {}
public class StyleAttribute_804C3F4AACC5696633A0DC41019F2C96 : xgc3.RuntimeEnv.StyleAttribute { 
public StyleAttribute_804C3F4AACC5696633A0DC41019F2C96() {}
}
public class StyleAttribute_0D69810C013E61C837891296A4B752C7 : xgc3.RuntimeEnv.StyleAttribute { 
public StyleAttribute_0D69810C013E61C837891296A4B752C7() {}
}
}
}
public class GameInfo_GameInfo : xgc3.Generated.GameInfo { 
public GameInfo_GameInfo() {}
private new void On_Create( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Application_GameObjects.GameInfo_GameInfo self = selfInstance as xgc3.Generated.Application_GameObjects.GameInfo_GameInfo; Console.WriteLine( "Hello world!"); }
}
public class SpriteResource_flower : xgc3.Resources.SpriteResource { 
public SpriteResource_flower() {}
}
public class Room_Editor : xgc3.GameObjects.Room { 
public Room_Editor() {}
private new void On_Create( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Application_GameObjects.Room_Editor self = selfInstance as xgc3.Generated.Application_GameObjects.Room_Editor; Console.WriteLine( "Hello world!"); }
public String Test;
public void Foo( )
{  { Console.WriteLine( "dog"); return;} }
private void On_BarEvent( xgc3.Core.Instance self, xgc3.Core.Instance other, String extra)
{ Console.WriteLine( "Hello world!"); }
public event EventDelegate BarEvent;
public void Raise_BarEvent(Instance self, Instance other, string extra)
	{ if (BarEvent != null) { BarEvent(self, other, extra); }}
public class Sprite_flower3 : xgc3.Generated.Sprite { 
public Sprite_flower3() {}
private new void On_Create( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Application_GameObjects.Room_Editor.Sprite_flower3 self = selfInstance as xgc3.Generated.Application_GameObjects.Room_Editor.Sprite_flower3; Console.WriteLine( "Hello world!" + self.Name); }
private new void On_Step( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Application_GameObjects.Room_Editor.Sprite_flower3 self = selfInstance as xgc3.Generated.Application_GameObjects.Room_Editor.Sprite_flower3; self.X = self.X+1; if( self.X > 200) { self.X = 0;} }
}
public class TextBox_flower4 : xgc3.Generated.TextBox { 
public TextBox_flower4() {}
private new void On_Create( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Application_GameObjects.Room_Editor.TextBox_flower4 self = selfInstance as xgc3.Generated.Application_GameObjects.Room_Editor.TextBox_flower4; Console.WriteLine( "Hello world!" + self.Name); }
private new void On_Step( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Application_GameObjects.Room_Editor.TextBox_flower4 self = selfInstance as xgc3.Generated.Application_GameObjects.Room_Editor.TextBox_flower4; self.Y = self.Y+1; if( self.Y > 200) { self.Y = 0;} }
}
public class Button_Button1 : xgc3.Generated.Button { 
public Button_Button1() {}
private new void On_Click( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Application_GameObjects.Room_Editor.Button_Button1 self = selfInstance as xgc3.Generated.Application_GameObjects.Room_Editor.Button_Button1; Console.WriteLine("Button click!"); }
}
public class Label_Label1 : xgc3.Generated.Label { 
public Label_Label1() {}
}
public class FPS_FramesPerSecond : xgc3.Generated.FPS { 
public FPS_FramesPerSecond() {}
}
public class TextBox_TextBox1 : xgc3.Generated.TextBox { 
public TextBox_TextBox1() {}
private new void On_Draw( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Application_GameObjects.Room_Editor.TextBox_TextBox1 self = selfInstance as xgc3.Generated.Application_GameObjects.Room_Editor.TextBox_TextBox1; 
          Vector2 p = new Vector2(self.X, self.Y);
          self.GameMgr.Game.GameComponent.SpriteB.DrawString(self.GameMgr.Game.FontMgr.Courier, self.Text, p, self.Color);
        }
}
}
public class Room_Editor2 : xgc3.Generated.Application_GameObjects.Room_Editor { 
public Room_Editor2() {}
private new void On_Create( xgc3.Core.Instance selfInstance, xgc3.Core.Instance other, String extra)
{ xgc3.Generated.Application_GameObjects.Room_Editor2 self = selfInstance as xgc3.Generated.Application_GameObjects.Room_Editor2; Console.WriteLine( "Hello world!"); }
}
}
}