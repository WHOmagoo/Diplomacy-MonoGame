using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoGameCross_PlatformDesktopApplication1
{
    public class Dropdown
    {

        IEnumerable<string> options;
        SpriteFont font;

        Texture2D background;

        bool IsShowing { get; set; }

        Point? clickedOn { get; set; }

        float Width { get; set; }
        float Height { get; set; }

        public Dropdown(IEnumerable<string> options, SpriteFont font, Texture2D background = null)
        {
            this.options = options;
            this.font = font;
            this.background = background;
        }

        public void Update(GameTime gt)
        {
        }

        public bool Select(Point position)
        {
            if (clickedOn == null)
            {
                clickedOn = position;
                IsShowing = true;
                return false;
            }

            // var clickedOn = this.clickedOn.Value;

            if (clickedOn?.X <= position.X && position.X <= Width + clickedOn?.X
                && clickedOn?.Y <= position.Y && position.X <= Height + clickedOn?.Y)
            {
                clickedOn = null;
                IsShowing = false;
                return true;
            }

            clickedOn = position;
            IsShowing = true;
            return true;
        }

        public void Draw(SpriteBatch sb, GameTime gt)
        {
            int i = 0;

            float width = 0;
            float height = 0;

            if (IsShowing && clickedOn.HasValue)
            {
                sb.Draw(background, new Rectangle(clickedOn.Value.X, clickedOn.Value.Y, (int)Width, (int)Height), Color.Gray);
                foreach (var opt in options)
                {
                    var size = font.MeasureString(opt);

                    width = Math.Max(width, size.X);

                    sb.DrawString(font, opt, new Vector2((float)(clickedOn.Value.X), clickedOn.Value.Y + height), Color.Black);

                    height += size.Y;

                }
            }

            Width = width;
            Height = height;
        }
    }
}