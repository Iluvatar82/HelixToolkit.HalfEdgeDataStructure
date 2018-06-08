using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HalfEdgeDataStructure
{
    [Serializable]
    public class Vertex : AbstractMeshElement, ICloneable, ISerializable
    {
        private double _x;
        private double _y;
        private double _z;
        private int _halfEdgeIndex;

        public double X {
            get { return _x; }
            set { _x = value; }
        }

        public double Y {
            get { return _y; }
            set { _y = value; }
        }

        public double Z {
            get { return _z; }
            set { _z = value; }
        }

        public HalfEdge OutgoingHalfEdge {
            get
            {
                if (TriangleMesh != null && _halfEdgeIndex > -1 && _halfEdgeIndex < TriangleMesh.HalfEdges.Count)
                    return TriangleMesh.HalfEdges[_halfEdgeIndex];

                return null;
            }
        }

        public int HalfEdgeIndex {
            set { _halfEdgeIndex = value; }
        }

        public bool IsOnBorder {
            get { return IncidentHalfEdges.Any(he => he.IsOnBorder); }
        }

        public IEnumerable<Vertex> NeighborVertices {
            get
            {
                foreach(var incidentHalfEdge in IncidentHalfEdges)
                    yield return incidentHalfEdge.EndPoint;
            }
        }

        public IEnumerable<HalfEdge> IncidentHalfEdges {
            get
            {
                var currentHalfEdge = OutgoingHalfEdge;
                if(OutgoingHalfEdge != default(HalfEdge))
                {
                    var firstHalfEdge = currentHalfEdge;
                    do
                    {
                        yield return currentHalfEdge;

                        currentHalfEdge = currentHalfEdge.PreviousHalfEdge.OppositeHalfEdge;
                    }
                    while(currentHalfEdge != firstHalfEdge);
                }
            }
        }


        public Vertex()
            :base()
        {
            _x = 0;
            _y = 0;
            _z = 0;
            _halfEdgeIndex = -1;
        }

        public Vertex(double x, double y, double z)
            :this()
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public Vertex(HalfEdgeMesh triangleMesh, double x, double y, double z)
            :this(x, y, z)
        {
            TriangleMesh = triangleMesh;
        }

        public Vertex(Vertex point)
            :this(point.TriangleMesh, point.X, point.Y, point.Z)
        { }

        public Vertex(Vector vector)
            : this(vector.X, vector.Y, vector.Z)
        { }

        public Vertex(SerializationInfo info, StreamingContext context)
        {
            TriangleMesh = (HalfEdgeMesh)info.GetValue("HalfEdgeMesh", typeof(HalfEdgeMesh));
            Index = -1;
            _x = (double)info.GetValue("X", typeof(double));
            _y = (double)info.GetValue("Y", typeof(double));
            _z = (double)info.GetValue("Z", typeof(double));
            _halfEdgeIndex = -1;
        }


        public object Clone()
        {
            return new Vertex(this);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("HalfEdgeMesh", TriangleMesh, typeof(HalfEdgeMesh));
            info.AddValue("X", _x, typeof(double));
            info.AddValue("Y", _x, typeof(double));
            info.AddValue("Z", _x, typeof(double));
        }

        public HalfEdge HalfEdgeTo(Vertex other)
        {
            foreach(var incidentHalfEdge in IncidentHalfEdges)
            {
                if (incidentHalfEdge.EndPoint == other)
                    return incidentHalfEdge;
            }

            return null;
        }



        public static Vector operator -(Vertex end, Vertex start)
        {
            return new Vector(end.X - start.X, end.Y - start.Y, end.Z - start.Z);
        }

        public override string ToString()
        {
            return $"[{_x:0.00}; {_y:0.00}; {_z:0.00}]";
        }
    }
}