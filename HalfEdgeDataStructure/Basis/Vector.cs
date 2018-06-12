using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HalfEdgeDataStructure
{
    /// <summary>
    /// Represents a Vector.
    /// </summary>
    [Serializable]
    public struct Vector : ICloneable, ISerializable, IEqualityComparer<Vector>
    {
        private Float3 _direction;
        private float _length;
        private float _lengthSquared;


        /// <summary>
        /// X Direction.
        /// </summary>
        public float X {
            get { return _direction.X; }
            set { _direction.X = value; }
        }

        /// <summary>
        /// Y Direction.
        /// </summary>
        public float Y {
            get { return _direction.Y; }
            set { _direction.Y = value; }
        }

        /// <summary>
        /// Z Direction.
        /// </summary>
        public float Z {
            get { return _direction.Z; }
            set { _direction.Z = value; }
        }

        /// <summary>
        /// Direction of the Vector.
        /// </summary>
        public Float3 Direction {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// Length of the Vector.
        /// </summary>
        public float Length {
            get {
                if(_length == -1)
                    UpdateLengthInformation();

                return _length;
            }
            set { _length = value; }
        }

        /// <summary>
        /// Squared Length of the Vector.
        /// </summary>
        public float LengthSquared {
            get {
                if(_lengthSquared == -1)
                    UpdateLengthSquared();

                return _lengthSquared;
            }
            set { _lengthSquared = value; }
        }

        /// <summary>
        /// Create a Vector collinear with the X Axis.
        /// </summary>
        /// <returns>New Vector.</returns>
        public static Vector UnitX => new Vector(1, 0, 0);

        /// <summary>
        /// Create a Vector collinear with the Y Axis.
        /// </summary>
        /// <returns>New Vector.</returns>
        public static Vector UnitY => new Vector(0, 1, 0);

        /// <summary>
        /// Create a Vector collinear with the Z Axis.
        /// </summary>
        /// <returns>New Vector.</returns>
        public static Vector UnitZ => new Vector(0, 0, 1);

        /// <summary>
        /// Create a Vector pointing nowhere.
        /// </summary>
        /// <returns>New Vector.</returns>
        public static Vector Zero => new Vector(0, 0, 0);

        /// <summary>
        /// Create a Vector pointing to 1 / 1 / 1.
        /// </summary>
        /// <returns>New Vector.</returns>
        public static Vector One => new Vector(1, 1, 1);

        /// <summary>
        /// Is the Vector normalized (i.e. it's Length == 1.0) or not.
        /// </summary>
        public bool IsNormalized {
            get { return _lengthSquared == 1f; }
        }


        /*/// <summary>
        /// Default Constructor.
        /// </summary>
        public Vector()
        {
            _direction = new Float3(0, 0, 0);
            _length = -1;
            _lengthSquared = -1;
        }*/

        /// <summary>
        /// Constructor with Position Values.
        /// </summary>
        /// <param name="x">X Direction of the Vertex.</param>
        /// <param name="y">Y Direction of the Vertex.</param>
        /// <param name="z">Z Direction of the Vertex.</param>
        public Vector(float x, float y, float z)
        {
            _direction = new Float3(x, y, z);
            _length = -1;
            _lengthSquared = -1;
        }

        /// <summary>
        /// Constructor with Direction Values.
        /// </summary>
        /// <param name="direction">The Direction of the Vector.</param>
        public Vector(Float3 direction)
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
            :this((Float3)info.GetValue("Direction", typeof(Float3)))
        { }


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
            info.AddValue("Direction", Direction, typeof(Float3));
        }

        /// <summary>
        /// Calculate the Lenght Information of this Vector.
        /// </summary>
        private void UpdateLengthInformation()
        {
            UpdateLengthSquared();
            _length = MathF.Sqrt(_lengthSquared);
        }

        /// <summary>
        /// Calculate the squared Lenght Information of this Vector.
        /// </summary>
        private void UpdateLengthSquared()
        {
            _lengthSquared = X * X + Y * Y + Z * Z;
        }

        /// <summary>
        /// Bring the Length of this Vector to 1.0 while not modifying the Direction.
        /// </summary>
        public void Normalize()
        {
            if(Length == 0.0)
                return;

            if(Length == 1.0)
                return;

            X /= _length;
            Y /= _length;
            Z /= _length;

            _lengthSquared = 1f;
            _length = 1f;
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
        public static float Dot(Vector first, Vector second)
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
        public static float CalculateAngle(Vector first, Vector second)
        {
            return MathF.Acos(Dot(first, second) / (first.Length * second.Length));
        }

        /// <summary>
        /// Calculate the Angle between this Vector and the <paramref name="other"/> Vector.
        /// The Vectors don't need to be normalized.
        /// </summary>
        /// <param name="other">The other Vector.</param>
        /// <returns>Angle between the two Vectors in Radians.</returns>
        public float CalculateAngle(Vector other)
        {
            return CalculateAngle(this, other);
        }

        /// <summary>
        /// Reflect a Vector on a Plane defined by it's Normal Vector.
        /// </summary>
        /// <param name="vector">The Vector to reflect.</param>
        /// <param name="normal">The Normal Vector of the Plane to project the Vector on.</param>
        /// <returns>Reflected Vector.</returns>
        public static Vector ReflectVector(Vector vector, Vector normal)
        {
            var projectedVector = vector.ProjectToVector(normal);
            return vector - 2 * projectedVector;
        }

        /// <summary>
        /// Reflect a Vector on a Plane defined by it's Normal Vector.
        /// </summary>
        /// <param name="normal">The Normal Vector of the Plane to project the Vector on.</param>
        /// <returns>Reflected Vector.</returns>
        public Vector ReflectVector(Vector normal)
        {
            return ReflectVector(this, normal);
        }

        /// <summary>
        /// Project a Vector to a Plane defined by it's Normal Vector.
        /// </summary>
        /// <param name="vector">The Vector to project.</param>
        /// <param name="normal">The Normal Vector of the Plane to project the Vector on.</param>
        /// <returns>Projected Vector.</returns>
        public static Vector ProjectToPlane(Vector vector, Vector normal)
        {
            var projectedVector = vector.ProjectToVector(normal);
            return vector - projectedVector;
        }

        /// <summary>
        /// Project this Vector to a Plane defined by it's Normal Vector.
        /// </summary>
        /// <param name="normal">The Normal Vector of the Plane to project the Vector on.</param>
        /// <returns>Projected Vector.</returns>
        public Vector ProjectToPlane(Vector normal)
        {
            return ProjectToPlane(this, normal);
        }


        /// <summary>
        /// Project a Vector to another Vector.
        /// </summary>
        /// <param name="vector">The Vector to project.</param>
        /// <param name="other">The Vector to project to.</param>
        /// <returns>Projected Vector.</returns>
        public static Vector ProjectToVector(Vector vector, Vector other)
        {
            other.Normalize();
            return Dot(vector, other) * other;
        }

        /// <summary>
        /// Project this Vector to another Vector.
        /// </summary>
        /// <param name="other">The Vector to project to.</param>
        /// <returns>Projected Vector.</returns>
        public Vector ProjectToVector(Vector other)
        {
            return ProjectToVector(this, other);
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
        public static Vector operator *(float factor, Vector vector)
        {
            return new Vector(vector.X * factor, vector.Y * factor, vector.Z * factor);
        }

        /// <summary>
        /// Calculate the Multiplication of a Vector. Essentially making it longer or shorter.
        /// </summary>
        /// <param name="vector">The Vector.</param>
        /// <param name="factor">The Scaling Factor.</param>
        /// <returns>The scaled Vector.</returns>
        public static Vector operator *(Vector vector, float factor)
        {
            return new Vector(vector.X * factor, vector.Y * factor, vector.Z * factor);
        }

        /// <summary>
        /// Calculate the Division of a Vector. Essentially making it longer or shorter.
        /// </summary>
        /// <param name="vector">The Vector.</param>
        /// <param name="divisor">The dividing Value.</param>
        /// <returns>The divided Vector.</returns>
        public static Vector operator /(Vector vector, float divisor)
        {
            if(divisor == 0)
                return default(Vector);

            var invDivisor = 1f / divisor;

            return new Vector(vector.X * invDivisor, vector.Y * invDivisor, vector.Z * invDivisor);
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
        public static implicit operator Float3(Vector vector)
        {
            return new Float3(vector.Direction);
        }



        /// <summary>
        /// Creates a String Representation of this Vector.
        /// </summary>
        /// <returns>String representation of this Vector.</returns>
        public override string ToString()
        {
            return $"[{_direction.ToString()}; Length: {_length:0.00}]";
        }

        public bool Equals(Vector first, Vector second)
        {
            if(first.Equals(default(Vector)) && second.Equals(default(Vector)))
                return true;

            if(first.Equals(default(Vector)) && !second.Equals(default(Vector)) ||
               !first.Equals(default(Vector)) && second.Equals(default(Vector)))
                return false;

            if((first - second).LengthSquared < 0.001f)
                return true;

            return false;
        }

        public int GetHashCode(Vector obj)
        {
            return obj.GetHashCode();
        }
    }
}