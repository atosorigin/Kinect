using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Kinect.ShowCase
{
    public class Ball : ModelVisual3D
    {
        public Ball()
        {
            Content = new GeometryModel3D();
            (Content as GeometryModel3D).Geometry = Tessellate();
        }

        public string ImageSource
        {
            set
            {
                var dm = new DiffuseMaterial();
                ImageSource imSrc = new
                    BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute));
                dm.Brush = new ImageBrush(imSrc);

                (Content as GeometryModel3D).Material = dm;
            }
        }

        public Point3D Offset
        {
            set
            {
                Transform = new
                    TranslateTransform3D(value.X, value.Y, value.Z);
            }
        }

        private static double DegToRad(double degrees)
        {
            return (degrees/180.0)*Math.PI;
        }

        internal Point3D GetPosition(double t, double y)
        {
            double r = Math.Sqrt(1 - y*y);
            double x = r*Math.Cos(t);
            double z = r*Math.Sin(t);

            return new Point3D(x, y, z);
        }

        private Vector3D GetNormal(double t, double y)
        {
            return (Vector3D) GetPosition(t, y);
        }

        private Point GetTextureCoordinate(double t, double y)
        {
            var TYtoUV = new Matrix();
            TYtoUV.Scale(1/(2*Math.PI), -0.5);

            var p = new Point(t, y);
            p = p*TYtoUV;

            return p;
        }

        internal Geometry3D Tessellate()
        {
            int tDiv = 750;
            int yDiv = 750;
            double maxTheta = DegToRad(360);
            double minY = -1.0;
            double maxY = 1.0;

            double dt = maxTheta/tDiv;
            double dy = (maxY - minY)/yDiv;

            var mesh = new MeshGeometry3D();

            for (int yi = 0; yi <= yDiv; yi++)
            {
                double y = minY + yi*dy;

                for (int ti = 0; ti <= tDiv; ti++)
                {
                    double t = ti*dt;
                    Point3D p = GetPosition(t, y);
                    if (p.Z > 0 && p.X > -.5 && p.X < .5 && p.Y > -.5 && p.Y < .5)
                    {
                        mesh.Positions.Add(p);
                        mesh.Normals.Add(GetNormal(t, y));
                        mesh.TextureCoordinates.Add(GetTextureCoordinate(t, y));
                    }
                }
            }

            for (int yi = 0; yi < yDiv; yi++)
            {
                for (int ti = 0; ti < tDiv; ti++)
                {
                    int x0 = ti;
                    int x1 = (ti + 1);
                    int y0 = yi*(tDiv + 1);
                    int y1 = (yi + 1)*(tDiv + 1);

                    mesh.TriangleIndices.Add(x0 + y0);
                    mesh.TriangleIndices.Add(x0 + y1);
                    mesh.TriangleIndices.Add(x1 + y0);

                    mesh.TriangleIndices.Add(x1 + y0);
                    mesh.TriangleIndices.Add(x0 + y1);
                    mesh.TriangleIndices.Add(x1 + y1);
                }
            }

            mesh.Freeze();
            return mesh;
        }
    }
}