using System;

namespace HalfEdgeDataStructure
{
    /// <summary>
    /// Math Class for floats.
    /// </summary>
    public static class MathF
    {
        public const float DegreeToRadians = (float)Math.PI / 180;
        public const float RadiansToDegree = 1f / DegreeToRadians;


        public static float Sqrt(float v) { return (float)Math.Sqrt(v); }
        public static float Abs(float v) { return Math.Abs(v); }
        public static float Pow(float a, float b) { return (float)Math.Pow(a, b); }
        public static float Sin(float v) { return (float)Math.Sin(v); }
        public static float Cos(float v) { return (float)Math.Cos(v); }
        public static float Acos(float v) { return (float)Math.Acos(v); }
        public static float Tan(float v) { return (float)Math.Tan(v); }
        public static float Max(float a, float b) { return Math.Max(a, b); }
    }

    /// <summary>
    /// Represents an Oject with 3-dimensional Data X, Y and Z.
    /// </summary>
    [Serializable]
    public struct Float3 : ICloneable
    {
        private float _x;
        private float _y;
        private float _z;

        /// <summary>
        /// X Value.
        /// </summary>
        public float X {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// Y Value.
        /// </summary>
        public float Y {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        /// Z Value.
        /// </summary>
        public float Z {
            get { return _z; }
            set { _z = value; }
        }

        /// <summary>
        /// U Value.
        /// </summary>
        public float U {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// V Value.
        /// </summary>
        public float V {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        /// W Value.
        /// </summary>
        public float W {
            get { return _z; }
            set { _z = value; }
        }


        /// <summary>
        /// Constructor with all Values.
        /// </summary>
        /// <param name="x">X Value.</param>
        /// <param name="y">Y Value.</param>
        /// <param name="z">Z Value.</param>

        public Float3(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        /// Constructor that uses an existing Double3 to create a new Double3.
        /// </summary>
        /// <param name="existing">Existing Double3.</param>
        public Float3(Float3 existing)
            : this(existing.X, existing.Y, existing.Z)
        { }


        /// <summary>
        /// Clone this Double3 and return a Copy of it.
        /// </summary>
        /// <returns>Cloned Object.</returns>
        public object Clone()
        {
            return new Float3(this);
        }

        /// <summary>
        /// Implicitly convert a Double3 to a Vector.
        /// </summary>
        /// <param name="values">The Double3 to convert.</param>
        public static implicit operator Vector(Float3 values)
        {
            return new Vector(values);
        }

        /// <summary>
        /// Implicitly convert a Double3 to a Vertex.
        /// </summary>
        /// <param name="values">The Double3 to convert.</param>
        public static implicit operator Vertex(Float3 values)
        {
            return new Vertex(values);
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