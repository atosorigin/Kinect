using Common.ColorHelpers;

namespace Kinect.Common.ColorHelpers
{
    internal class IntensityValueWalker
    {
        public IntensityValueWalker()
        {
            Current = new IntensityValue(null, 1 << 7, 1);
        }

        public IntensityValue Current { get; set; }

        public void MoveNext()
        {
            if (Current.Parent == null)
            {
                Current = Current.ChildA;
            }
            else if (Current.Parent.ChildA == Current)
            {
                Current = Current.Parent.ChildB;
            }
            else
            {
                int levelsUp = 1;
                Current = Current.Parent;
                while (Current.Parent != null && Current == Current.Parent.ChildB)
                {
                    Current = Current.Parent;
                    levelsUp++;
                }
                if (Current.Parent != null)
                {
                    Current = Current.Parent.ChildB;
                }
                else
                {
                    levelsUp++;
                }
                for (int i = 0; i < levelsUp; i++)
                {
                    Current = Current.ChildA;
                }
            }
        }
    }
}