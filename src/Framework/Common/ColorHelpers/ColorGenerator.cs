using System.Windows.Media;
using System.Globalization;

namespace Kinect.Common.ColorHelpers
{
    /// <summary>
    /// ColorGenerator
    /// </summary>
    public class ColorGenerator
    {
        private int _index = 0;
        private IntensityGenerator _intensityGenerator = new IntensityGenerator();

        /// <summary>
        /// Nexts the color string.
        /// </summary>
        /// <returns></returns>
        public string NextColorString()
        {
            string color = string.Format(PatternGenerator.NextPattern(_index), _intensityGenerator.NextIntensity(_index));
            _index++;
            return color;
        }

        /// <summary>
        /// Nexts the color.
        /// </summary>
        /// <returns></returns>
        public Color NextColor()
        {
            string colorString = NextColorString();
            Color color = Color.FromRgb(
                (byte)int.Parse(colorString.Substring(0, 2), NumberStyles.HexNumber),
                (byte)int.Parse(colorString.Substring(2, 2), NumberStyles.HexNumber),
                (byte)int.Parse(colorString.Substring(4, 2), NumberStyles.HexNumber));
            
            return color;
        }
    }
}
