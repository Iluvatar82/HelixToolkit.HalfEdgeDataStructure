using System;

namespace HalfEdgeDataStructure
{
    /// <summary>
    /// Represents an Oject with 3-dimensional Data X, Y and Z.
    /// </summary>
    [Serializable]
    public struct Double3 : ICloneable
    {
        private double _x;
        private double _y;
        private double _z;

        /// <summary>
        /// X Value.
        /// </summary>

        public double X {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// Y Value.
        /// </summary>
        public double Y {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        /// Z Value.
        /// </summary>
        public double Z {
            get { return _z; }
            set { _z = value; }
        }

        /// <summary>
        /// Constructor with all Values.
        /// </summary>
        /// <param name="x">X Value.</param>
        /// <param name="y">Y Value.</param>
        /// <param name="z">Z Value.</param>

        public Double3(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        /// Constructor that uses an existing Double3 to create a new Double3.
        /// </summary>
        /// <param name="existing">Existing Double3.</param>
        public Double3(Double3 existing)
            :this(existing.X, existing.Y, existing.Z)
        { }

        /// <summary>
        /// Clone this Double3 and return a Copy of it.
        /// </summary>
        /// <returns>Cloned Object.</returns>
        public object Clone()
        {
            return new Double3(this);
        }

        /// <summary>
        /// Creates a String Representation of this Double3.
        /// </summary>
        /// <returns>String representation of this Double3.</returns>
        public override string ToString()
        {
            return $"{_x:0.00}; {_y:0.00}; {_z:0.00}";
        }
    }
}