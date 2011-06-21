using Kinect.Common;

namespace Kinect.Core.Gestures.Model
{
    /// <summary>
    /// Semaphore
    /// </summary>
    public class Semaphore : ICopyAble<Semaphore>
    {
        private string _name;

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(this._name))
                {
                    this.Name = this.Char.ToString();
                }

                return this._name;
            }
            internal set
            {
                this._name = value;
            }
        }

        /// <summary>
        /// Gets the char.
        /// </summary>
        public char Char { get; internal set; }

        /// <summary>
        /// Gets the left angle.
        /// </summary>
        public int LeftAngle { get; internal set; }

        /// <summary>
        /// Gets the right angle.
        /// </summary>
        public int RightAngle { get; internal set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return this.Equals(obj as Semaphore);
        }

        /// <summary>
        /// Equalses the specified sem.
        /// </summary>
        /// <param name="sem">The sem.</param>
        /// <returns></returns>
        public bool Equals(Semaphore sem)
        {
            if (null != sem)
            {
                var equals = sem.Char == this.Char;
                return equals;
            }

            return false;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}: Left: {1} | Right: {2}", this.Char, this.LeftAngle, this.RightAngle);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Creates the copy.
        /// </summary>
        /// <returns></returns>
        public Semaphore CreateCopy()
        {
            Semaphore sem = new Semaphore();
            sem.Char = this.Char;
            sem.Name = this.Name;
            sem.LeftAngle = this.LeftAngle;
            sem.RightAngle = this.RightAngle;
            return sem;
        }
    }
}
