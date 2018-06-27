using System;
using System.Runtime.Serialization;

namespace HalfEdgeDataStructure
{
    [Serializable]
    public class Plane: ICloneable, ISerializable
    {
        private Float3 _position;
        private Float3 _normal;

        /// <summary>
        /// The Position of the Origin of the Plane.
        /// </summary>
        public Float3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// The Normal of the Plane.
        /// </summary>
        public Float3 Normal
        {
            get { return _normal; }
            set { _normal = value; }
        }


        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Plane()
        {
            _position = default(Float3);
            _normal = default(Float3);
        }

        /// <summary>
        /// Constructor with Position and Normal.
        /// </summary>
        /// <param name="position">The Position of the Origin of the Plane.</param>
        /// <param name="normal">The Normal of the Plane.</param>
        public Plane(Float3 position, Float3 normal)
        {
            _position = position;
            _normal = normal;
        }

        /// <summary>
        /// Constructor with Position and Normal.
        /// </summary>
        /// <param name="position">The Position of the Origin of the Plane.</param>
        /// <param name="normal">The Normal of the Plane.</param>
        public Plane(Plane plane)
        {
            _position = new Float3(plane.Position);
            _normal = new Float3(plane.Normal);
        }

        /// <summary>
        /// Constructor that is used by the Deserialization.
        /// </summary>
        /// <param name="info">The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        public Plane(SerializationInfo info, StreamingContext context)
        {
            _position = (Float3)info.GetValue("Position", typeof(Float3));
            _normal = (Vector)info.GetValue("Normal", typeof(Vector));
        }


        /// <summary>
        /// Clone this Plane and return a Copy of it.
        /// </summary>
        /// <returns>Cloned Object.</returns>
        public object Clone()
        {
            return new Plane(this);
        }


        /// <summary>
        /// Serialize the Plane.
        /// </summary>
        /// <param name="info">The SerializationInfo.</param>
        /// <param name="context">The StreamingContext.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Position", Position, typeof(Float3));
            info.AddValue("Normal", Normal, typeof(Float3));
        }
    }
}