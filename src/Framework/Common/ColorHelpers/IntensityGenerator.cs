namespace Kinect.Common.ColorHelpers
{
    internal class IntensityGenerator
    {
        private IntensityValueWalker _walker;
        private int _current;

        public string NextIntensity(int index)
        {
            if (index == 0)
            {
                _current = 255;
            }
            else if (index % 7 == 0)
            {
                if (_walker == null)
                {
                    _walker = new IntensityValueWalker();
                }
                else
                {
                    _walker.MoveNext();
                }
                _current = _walker.Current.Value;
            }

            string currentText = _current.ToString("X");
            
            if (currentText.Length == 1)
            {
                currentText = "0" + currentText;
            }
            
            return currentText;
        }
    }
}
