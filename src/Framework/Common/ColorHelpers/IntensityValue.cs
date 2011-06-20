using Kinect.Common.ColorHelpers;

namespace Common.ColorHelpers
{
    internal class IntensityValue
    {
        private IntensityValue _ChildA;

        private IntensityValue _ChildB;

        public IntensityValue(IntensityValue parent, int value, int level)
        {
            if (level > 7)
            {
                throw new IntensityException("There are no more colours left");
            }
            Value = value;
            Parent = parent;
            Level = level;
        }

        public IntensityValue ChildA
        {
            get { return _ChildA ?? (_ChildA = new IntensityValue(this, Value - (1 << (7 - Level)), Level + 1)); }
        }

        public IntensityValue ChildB
        {
            get { return _ChildB ?? (_ChildB = new IntensityValue(this, Value + (1 << (7 - Level)), Level + 1)); }
        }

        public int Level { get; set; }

        public int Value { get; set; }

        public IntensityValue Parent { get; set; }
    }
}