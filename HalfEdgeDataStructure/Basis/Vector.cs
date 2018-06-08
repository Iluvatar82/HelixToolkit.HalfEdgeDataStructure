using System;
using System.Runtime.Serialization;

namespace HalfEdgeDataStructure
{
    [Serializable]
    public class Vector : ICloneable, ISerializable
    {
        private double _x;
        private double _y;
        private double _z;
        private double _length;
        private double _lengthSquared;
        private bool _keepSynchronized;


        public double X {
            get { return _x; }
            set {
                _x = value;
                UpdateLengthInformation();
            }
        }

        public double Y {
            get { return _y; }
            set {
                _y = value;
                UpdateLengthInformation();
            }
        }

        public double Z {
            get { return _z; }
            set {
                _z = value;
                UpdateLengthInformation();
            }
        }

        public double Length {
            get { return _length; }
            set { _length = value; }
        }

        public double LengthSquared {
            get { return _lengthSquared; }
            set { _lengthSquared = value; }
        }

        public bool IsNormalized {
            get { return _lengthSquared == 1.0; }
        }

        public bool KeepSynchronized {
            get { return _keepSynchronized; }
            set {
                _keepSynchronized = value;
                if(_keepSynchronized)
                    UpdateLengthInformation();
            }
        }


        public Vector()
        {
            _x = 0;
            _y = 0;
            _z = 0;
            _keepSynchronized = true;
        }

        public Vector(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
            _keepSynchronized = true;
            UpdateLengthInformation();
        }

        public Vector(Vector vector)
            : this(vector.X, vector.Y, vector.Z)
        { }

        public Vector(Vertex point)
            : this(point.X, point.Y, point.Z)
        { }

        public Vector(SerializationInfo info, StreamingContext context)
        {
            _x = (double)info.GetValue("X", typeof(double));
            _y = (double)info.GetValue("Y", typeof(double));
            _z = (double)info.GetValue("Z", typeof(double));
            _keepSynchronized = true;
            UpdateLengthInformation();
        }

        public static Vector UnitX()
        {
            return new Vector(1, 0, 0);
        }

        public static Vector UnitY()
        {
            return new Vector(0, 1, 0);
        }

        public static Vector UnitZ()
        {
            return new Vector(0, 0, 1);
        }

        public static Vector Zero()
        {
            return new Vector(0, 0, 0);
        }


        public object Clone()
        {
            return new Vector(this);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("X", _x, typeof(double));
            info.AddValue("Y", _x, typeof(double));
            info.AddValue("Z", _x, typeof(double));
        }

        private void UpdateLengthInformation()
        {
            if(_keepSynchronized)
            {
                _lengthSquared = _x * _x + _y * _y + _z * _z;
                _length = Math.Sqrt(_lengthSquared);
            }
        }

        public void Normalize()
        {
            if(_length == 0.0)
                return;

            if(_length == 1.0)
                return;

            _x /= _length;
            _y /= _length;
            _z /= _length;
            UpdateLengthInformation();
        }

        public static double AngleBetweenVectors(Vector first, Vector second)
        {
            return Math.Acos(Dot(first, second) / (first.Length * second.Length));
        }

        public static Vector Cross(Vector first, Vector second, bool normalizeResult = false)
        {
            var newVec = new Vector(first.Y * second.Z - first.Z * second.Y,
                                      first.Z * second.X - first.X * second.Z,
                                      first.X * second.Y - first.Y * second.X);

            if(normalizeResult)
                newVec.Normalize();

            return newVec;
        }

        public static double Dot(Vector first, Vector second, bool useNormalizedVectors = false)
        {
            var firstToUse = first;
            var secondToUse = second;
            if(useNormalizedVectors)
            {
                firstToUse = (Vector)first.Clone();
                firstToUse.Normalize();
                secondToUse = (Vector)second.Clone();
                secondToUse.Normalize();
            }

            return firstToUse.X * secondToUse.X + firstToUse.Y * secondToUse.Y + firstToUse.Z * secondToUse.Z;
        }


        public static Vector operator +(Vector end, Vector start)
        {
            return new Vector(end.X + start.X, end.Y + start.Y, end.Z + start.Z);
        }

        public static Vector operator -(Vector end, Vector start)
        {
            return new Vector(end.X - start.X, end.Y - start.Y, end.Z - start.Z);
        }

        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector.X, -vector.Y, -vector.Z);
        }

        public static Vector operator *(double factor, Vector vector)
        {
            return new Vector(vector.X * factor, vector.Y * factor, vector.Z * factor);
        }

        public static Vector operator *(Vector vector, double factor)
        {
            return new Vector(vector.X * factor, vector.Y * factor, vector.Z * factor);
        }

        public static Vector operator /(double divisor, Vector vector)
        {
            if(divisor == 0)
                return new Vector();

            return new Vector(vector.X / divisor, vector.Y / divisor, vector.Z / divisor);
        }

        public static Vector operator /(Vector vector, double divisor)
        {
            if(divisor == 0)
                return new Vector();

            return new Vector(vector.X / divisor, vector.Y / divisor, vector.Z / divisor);
        }


        public override string ToString()
        {
            return $"[{_x:0.00}; {_y:0.00}; {_z:0.00}; Length: {_length:0.00}]";
        }
    }
}