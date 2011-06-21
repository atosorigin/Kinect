using System;
using System.Windows.Media.Media3D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kinect.XNA.User
{
    public class Bodypart
    {
        private readonly Texture2D _texture;

        public Bodypart(Texture2D texture)
        {
            _texture = texture;
            LeftPoint = null;
            RightPoint = null;
        }

        public Point3D? LeftPoint { get; set; }
        public Point3D? RightPoint { get; set; }

        public void Draw(SpriteBatch spritebatch)
        {
            if (!LeftPoint.HasValue && !RightPoint.HasValue)
            {
                return;
            }
            spritebatch.Begin();
            if (!RightPoint.HasValue)
            {
                spritebatch.Draw(_texture, new Vector2((float) LeftPoint.Value.X, (float) LeftPoint.Value.Y),
                                 Color.White);
            }
            else
            {
                int height = _texture.Bounds.Height;
                //var width = 

                //TODO hier de hoek in verwerken

                var rectangle = new Rectangle((int) LeftPoint.Value.X, (int) LeftPoint.Value.Y,
                                              (int) Math.Abs(LeftPoint.Value.X - RightPoint.Value.X),
                                              (int) Math.Abs(LeftPoint.Value.Y - RightPoint.Value.Y));
                spritebatch.Draw(_texture, rectangle, _texture.Bounds, Color.White);
            }
            spritebatch.End();
        }
    }
}