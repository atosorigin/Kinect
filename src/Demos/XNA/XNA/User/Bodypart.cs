using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Windows.Media.Media3D;

namespace Kinect.XNA.User
{
    public class Bodypart
    {
        private Texture2D _texture;
        public Point3D? LeftPoint { get; set; }
        public Point3D? RightPoint  { get; set; }

        public Bodypart(Texture2D texture)
        {
            _texture = texture;
            LeftPoint = null;
            RightPoint = null;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (!LeftPoint.HasValue && !RightPoint.HasValue)
            {
                return;
            }
            spritebatch.Begin();
            if (!RightPoint.HasValue)
            {
                spritebatch.Draw(_texture, new Vector2((float)LeftPoint.Value.X, (float)LeftPoint.Value.Y), Color.White);
            }
            else
            {
                var height = _texture.Bounds.Height;
                //var width = 

                //TODO hier de hoek in verwerken

                var rectangle = new Rectangle((int)LeftPoint.Value.X, (int)LeftPoint.Value.Y, 
                    (int)Math.Abs(LeftPoint.Value.X - RightPoint.Value.X), 
                    (int)Math.Abs(LeftPoint.Value.Y - RightPoint.Value.Y));
                spritebatch.Draw(_texture, rectangle, _texture.Bounds, Color.White);
            }
            spritebatch.End();
        }
    }
}
