using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HalfEdgeDataStructure
{
    [Serializable]
    public class Triangle : AbstractMeshElement, ICloneable, ISerializable
    {
        private int _indexPoint1;
        private int _indexPoint2;
        private int _indexPoint3;
        private int _halfEdgeIndex;
        private Vector _normal;
        private double _area;


        public Vertex Point1 {
            get
            {
                if(TriangleMesh != null && _indexPoint1 > -1 && _indexPoint1 < TriangleMesh.Vertices.Count)
                    return TriangleMesh.Vertices[_indexPoint1];

                return null;
            }
        }

        public Vertex Point2 {
            get
            {
                if(TriangleMesh != null && _indexPoint2 > -1 && _indexPoint2 < TriangleMesh.Vertices.Count)
                    return TriangleMesh.Vertices[_indexPoint2];

                return null;
            }
        }

        public Vertex Point3 {
            get
            {
                if(TriangleMesh != null && _indexPoint3 > -1 && _indexPoint3 < TriangleMesh.Vertices.Count)
                    return TriangleMesh.Vertices[_indexPoint3];

                return null;
            }
        }

        public int[] PointIndizes {
            get
            {
                return new int[]
                {
                    _indexPoint1,
                    _indexPoint2,
                    _indexPoint3
                };
            }
        }

        public HalfEdge OutgoingHalfEdge {
            get
            {
                if(TriangleMesh != null && _halfEdgeIndex > -1 && _halfEdgeIndex < TriangleMesh.HalfEdges.Count)
                    return TriangleMesh.HalfEdges[_halfEdgeIndex];

                return null;
            }
        }

        public IEnumerable<HalfEdge> ExistingHalfEdgesBetweenVertices {
            get
            {
                var result = Point1.HalfEdgeTo(Point2);
                if(result != null)
                    yield return result;
                result = Point2.HalfEdgeTo(Point3);
                if(result != null)
                    yield return result;
                result = Point3.HalfEdgeTo(Point1);
                if(result != null)
                    yield return result;
            }
        }

        public int HalfEdgeIndex {
            set { _halfEdgeIndex = value; }
        }

        public Vector Normal {
            get { return _normal; }
            set { _normal = value; }
        }

        public double Area {
            get { return _area; }
        }

        public new HalfEdgeMesh TriangleMesh {
            get { return base.TriangleMesh; }
            set {
                base.TriangleMesh = value;

                if(value != null)
                    Calculate();
            }
        }

        public bool IsOnBorder {
            get
            {
                return OutgoingHalfEdge.IsOnBorder || OutgoingHalfEdge.NextHalfEdge.IsOnBorder ||
                    OutgoingHalfEdge.PreviousHalfEdge.IsOnBorder;
            }
        }


        public Triangle()
            :base()
        {
            _indexPoint1 = -1;
            _indexPoint2 = -1;
            _indexPoint3 = -1;
            _halfEdgeIndex = -1;

            _normal = new Vector();
            _area = 0;
        }

        public Triangle(int point1, int point2, int point3)
            :this()
        {
            _indexPoint1 = point1;
            _indexPoint2 = point2;
            _indexPoint3 = point3;
        }

        public Triangle(HalfEdgeMesh triangleMesh, int point1, int point2, int point3)
            :this(point1, point2, point3)
        {
            TriangleMesh = triangleMesh;

            Calculate();
        }

        public Triangle(Triangle triangle)
            : this(triangle.TriangleMesh, triangle._indexPoint1, triangle._indexPoint2, triangle._indexPoint3)
        { }

        public Triangle(SerializationInfo info, StreamingContext context)
        {
            TriangleMesh = (HalfEdgeMesh)info.GetValue("TriangleMesh", typeof(HalfEdgeMesh));
            Index = -1;
            _indexPoint1 = (int)info.GetValue("PointIndex1", typeof(int));
            _indexPoint2 = (int)info.GetValue("PointIndex2", typeof(int));
            _indexPoint3 = (int)info.GetValue("PointIndex3", typeof(int));
            _halfEdgeIndex = -1;

            Calculate();
        }


        public object Clone()
        {
            return new Triangle(this);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TriangleMesh", TriangleMesh, typeof(HalfEdgeMesh));
            info.AddValue("PointIndex1", _indexPoint1, typeof(int));
            info.AddValue("PointIndex2", _indexPoint2, typeof(int));
            info.AddValue("PointIndex3", _indexPoint3, typeof(int));
        }

        private void Calculate()
        {
            if(TriangleMesh == null)
                return;

            var vector12 = TriangleMesh.Vertices[_indexPoint2] - TriangleMesh.Vertices[_indexPoint1];
            var vector13 = TriangleMesh.Vertices[_indexPoint3] - TriangleMesh.Vertices[_indexPoint1];

            var cross = Vector.Cross(vector12, vector13);
            _area = cross.Length * 0.5;
            cross.Normalize();
            _normal = cross;
        }

        public void SetFirstPointIndex(int firstPointIndex)
        {
            if(_indexPoint1 != firstPointIndex && _indexPoint2 != firstPointIndex && _indexPoint3 != firstPointIndex)
                return;

            while(_indexPoint1 != firstPointIndex)
            {
                var tempIndex = _indexPoint1;
                _indexPoint1 = _indexPoint2;
                _indexPoint2 = _indexPoint3;
                _indexPoint3 = tempIndex;
            }
        }


        public override string ToString()
        {
            return $"Point1: [{Point1.X:0.00}; {Point1.Y:0.00}; {Point1.Z:0.00}] " +
                   $"Point2: [{Point2.X:0.00}; {Point2.Y:0.00}; {Point2.Z:0.00}] " +
                   $"Point3: [{Point3.X:0.00}; {Point3.Y:0.00}; {Point3.Z:0.00}] " +
                   $"Normal: [{_normal.X:0.00}; {_normal.Y:0.00}; {_normal.Z:0.00}] " +
                   $"Area: [{_area:0.00}]";
        }
    }
}