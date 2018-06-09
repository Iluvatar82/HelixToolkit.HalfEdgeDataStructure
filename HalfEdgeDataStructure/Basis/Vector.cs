using System;
using System.Runtime.Serialization;

namespace HalfEdgeDataStructure
{
    /// <summary>
    /// Represents a Vector.
    /// </summary>
    [Serializable]
    public class Vector : ICloneable, ISerializable
    {
        private Double3 _direction;
        private double _length;
        private double _lengthSquared;


        /// <summary>
        /// X Direction.
        /// </summary>
        public double X {
            get { return _direction.X; }
            set { _direction.X = value; }
        }

        /// <summary>
        /// Y Direction.
        /// </summary>
        public double Y {
            get { return _direction.Y; }
            set { _direction.Y = value; }
        }

        /// <summary>
        /// Z Direction.
        /// </summary>
        public double Z {
            get { return _direction.Z; }
            set { _direction.Z = value; }
        }

        /// <summary>
        /// Direction of the Vector.
        /// </summary>
        public Double3 Direction {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// Length of the Vector.
        /// </summary>
        public double Length {
            get { return _length; }
            set { _length = value; }
        }

        /// <summary>
        /// Squared Length of the Vector.
        /// </summary>
        public double LengthSquared {
            get { return _lengthSquared; }
            set { _lengthSquared = value; }
        }

        /// <summary>
        /// Is the Vector normalized (i.e. it's Length == 1.0) or not.
        /// </summary>
        public bool IsNormalized {
            get { return _lengthSquared == 1.0; }
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Vector()
        {
            _direction = new Double3();
        }

        /// <summary>
        /// Constructor with Position Values.
        /// </summary>
        /// <param name="x">X Direction of the Vertex.</param>
        /// <param name="y">Y Direction of the Vertex.</param>
        /// <param name="z">Z Direction of the Vertex.</param>
        public Vector(double x, double y, double z)
        {
            _direction = new Double3(x, y, z);

            UpdateLengthInformation();
        }

        /// <summary>
        /// Constructor with Direction Values.
        /// </summary>
        /// <param name="direction">The Direction of the Vector.</param>
        public Vector(Double3 direction)
            : this(direction.X, direction.Y, direction.Z)
        { }


        /// <summary>
        /// Constructor that uses an existing Vector to create a new Vector.
        /// </summary>
        /// <param name="vertex">Existing Vector.</param>
        public Vector(Vector vector)
            : this(vector.X, vector.Y, vector.Z)
        { }

        /// <summary>
        /// Constructor that uses an existing Vertex to create a new Vector.
        /// </summary>
        /// <param name="vertex">Existing Vertex.</param>
        public Vector(Vertex vertex)
            : this(vertex.Position)
        { }

        /// <summary>
        /// Constructor that is used by the Deserialization.
        /// </summary>
        /// <param name="info">The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        public Vector(SerializationInfo info, StreamingContext context)
        {
            _direction = (Double3)info.GetValue("Direction", typeof(Double3));

            UpdateLengthInformation();
        }

        /// <summary>
        /// Create a Vector collinear with the X Axis.
        /// </summary>
        /// <returns>New Vector.</returns>
        public static Vector UnitX()
        {
            return new Vector(1, 0, 0);
        }

        /// <summary>
        /// Create a Vector collinear with the Y Axis.
        /// </summary>
        /// <returns>New Vector.</returns>
        public static Vector UnitY()
        {
            return new Vector(0, 1, 0);
        }

        /// <summary>
        /// Create a Vector collinear with the Z Axis.
        /// </summary>
        /// <returns>New Vector.</returns>
        public static Vector UnitZ()
        {
            return new Vector(0, 0, 1);
        }

        /// <summary>
        /// Create a Vector pointing nowhere.
        /// </summary>
        /// <returns>New Vector.</returns>
        public static Vector Zero()
        {
            return new Vector(0, 0, 0);
        }

        /// <summary>
        /// Create a Vector pointing to 1 / 1 / 1.
        /// </summary>
        /// <returns>New Vector.</returns>
        public static Vector One()
        {
            return new Vector(1, 1, 1);
        }


        /// <summary>
        /// Clone this Vector and return a Copy of it.
        /// </summary>
        /// <returns>Cloned Object.</returns>
        public object Clone()
        {
            return new Vector(this);
        }

        /// <summary>
        /// Serialize the Vector.
        /// </summary>
        /// <param name="info">The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Direction", Direction, typeof(Double3));
        }

        /// <summary>
        /// Calculate the Lenght Information of this Vector.
        /// </summary>
        private void UpdateLengthInformation()
        {
            _lengthSquared = X * X + Y * Y + Z * Z;
            _length = Math.Sqrt(_lengthSquared);
        }

        /// <summary>
        /// Bring the Length of this Vector to 1.0 while not modifying the Direction.
        /// </summary>
        public void Normalize()
        {
            if(_length == 0.0)
                return;

            if(_length == 1.0)
                return;

            X /= _length;
            Y /= _length;
            Z /= _length;
            UpdateLengthInformation();
        }

        /// <summary>
        /// Calculate the Cross Product of the two Vectors <paramref name="first"/> and <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The first Vector.</param>
        /// <param name="second">The second Vector.</param>
        /// <returns>Vector perpendicular to the <paramref name="first"/> and <paramref name="second"/> Vector.</returns>
        public static Vector Cross(Vector first, Vector second)
        {
            var newVec = new Vector(first.Y * second.Z - first.Z * second.Y,
                                    first.Z * second.X - first.X * second.Z,
                                    first.X * second.Y - first.Y * second.X);

            return newVec;
        }

        /// <summary>
        /// Calculate the Dot Product (Scalar Product) of the two Vectors <paramref name="first"/> and <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The first Vector.</param>
        /// <param name="second">The second Vector.</param>
        /// <returns>Scalar Product of the two Vectors.</returns>
        public static double Dot(Vector first, Vector second)
        {
            return first.X * second.X + first.Y * second.Y + first.Z * second.Z;
        }

        /// <summary>
        /// Calculate the Angle between two Vectors <paramref name="first"/> and <paramref name="second"/>.
        /// The Vectors don't need to be normalized.
        /// </summary>
        /// <param name="first">The first Vector.</param>
        /// <param name="second">The second Vector.</param>
        /// <returns>Angle between the two Vectors in Radians.</returns>
        public static double CalculateAngle(Vector first, Vector second)
        {
            return Math.Acos(Dot(first, second) / (first.Length * second.Length));
        }

        /// <summary>
        /// Project a Vector to a Plane defined by it's Normal Vector.
        /// </summary>
        /// <param name="vector">The Vector to project.</param>
        /// <param name="normal">The Normal Vector of the Plane to project the Vector on.</param>
        /// <returns></returns>
        public static Vector ProjectOntoPlane(Vector vector, Vector normal)
        {
            normal.Normalize();
            var normalVectorVN = Cross(normal, vector);
            normalVectorVN.Normalize();
            var projectionVector = Cross(normalVectorVN, normal);
            return ProjectOntoVector(vector, projectionVector);
        }

        /// <summary>
        /// Project a Vector to another Vector.
        /// </summary>
        /// <param name="vector">The Vector to project.</param>
        /// <param name="other">The Vector to project onto.</param>
        /// <returns></returns>
        public static Vector ProjectOntoVector(Vector vector, Vector other)
        {
            other.Normalize();
            return Dot(vector, other) * other;
        }


        /// <summary>
        /// Calculate the Sum of two Vectors.
        /// </summary>
        /// <param name="first">The first Vector.</param>
        /// <param name="second">The second Vector.</param>
        /// <returns>Sum of the two Vectors.</returns>
        public static Vector operator +(Vector first, Vector second)
        {
            return new Vector(first.X + second.X, first.Y + second.Y, first.Z + second.Z);
        }

        /// <summary>
        /// Calculate the Difference of two Vectors.
        /// </summary>
        /// <param name="end">The end Vector.</param>
        /// <param name="start">The start Vector.</param>
        /// <returns>Difference of the two Vectors. Represents the Vector that ranges from the <paramref name="start"/> to the <paramref name="end"/>.</returns>
        public static Vector operator -(Vector end, Vector start)
        {
            return new Vector(end.X - start.X, end.Y - start.Y, end.Z - start.Z);
        }

        /// <summary>
        /// Calculate the Negation of the Vector. Essentially the Vector pointing in the opposite Direction.
        /// </summary>
        /// <param name="vector">The Vector to negate.</param>
        /// <returns></returns>
        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector.X, -vector.Y, -vector.Z);
        }

        /// <summary>
        /// Calculate the Multiplication of a Vector. Essentially making it longer or shorter.
        /// </summary>
        /// <param name="factor">The Scaling Factor.</param>
        /// <param name="vector">The Vector.</param>
        /// <returns>The scaled Vector.</returns>
        public static Vector operator *(double factor, Vector vector)
        {
            return new Vector(vector.X * factor, vector.Y * factor, vector.Z * factor);
        }

        /// <summary>
        /// Calculate the Multiplication of a Vector. Essentially making it longer or shorter.
        /// </summary>
        /// <param name="vector">The Vector.</param>
        /// <param name="factor">The Scaling Factor.</param>
        /// <returns>The scaled Vector.</returns>
        public static Vector operator *(Vector vector, double factor)
        {
            return new Vector(vector.X * factor, vector.Y * factor, vector.Z * factor);
        }

        /// <summary>
        /// Calculate the Division of a Vector. Essentially making it longer or shorter.
        /// </summary>
        /// <param name="divisor">The dividing Value.</param>
        /// <param name="vector">The Vector.</param>
        /// <returns>The divided Vector.</returns>
        public static Vector operator /(double divisor, Vector vector)
        {
            if(divisor == 0)
                return new Vector();

            return new Vector(vector.X / divisor, vector.Y / divisor, vector.Z / divisor);
        }

        /// <summary>
        /// Calculate the Division of a Vector. Essentially making it longer or shorter.
        /// </summary>
        /// <param name="vector">The Vector.</param>
        /// <param name="divisor">The dividing Value.</param>
        /// <returns>The divided Vector.</returns>
        public static Vector operator /(Vector vector, double divisor)
        {
            if(divisor == 0)
                return new Vector();

            return new Vector(vector.X / divisor, vector.Y / divisor, vector.Z / divisor);
        }

        /// <summary>
        /// Implicitly create a Vertex from the <see cref="Direction"/> of a Vector.
        /// </summary>
        /// <param name="vertex"></param>
        public static implicit operator Vertex(Vector vector)
        {
            return new Vertex(vector.Direction);
        }

        /// <summary>
        /// Implicitly create a Double3 from the <see cref="Direction"/> of a Vector.
        /// </summary>
        /// <param name="vertex"></param>
        public static implicit operator Double3(Vector vector)
        {
            return new Double3(vector.Direction);
        }



        /// <summary>
        /// Creates a String Representation of this Vector.
        /// </summary>
        /// <returns>String representation of this Vector.</returns>
        public override string ToString()
        {
            return $"[{_direction.ToString()}; Length: {_length:0.00}]";
        }
    }
}