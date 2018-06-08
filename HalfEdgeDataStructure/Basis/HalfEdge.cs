using System;

namespace HalfEdgeDataStructure
{
    public class HalfEdge : AbstractMeshElement, ICloneable
    {
        private int _indexStartPoint;
        private int _triangleIndex;
        private double _length;
        private int _nextHalfEdgeIndex;
        private int _previousHalfEdgeIndex;
        private int _oppositeHalfeEdgeIndex;


        public Vertex StartPoint {
            get
            {
                if(TriangleMesh != null && _indexStartPoint > -1 && _indexStartPoint < TriangleMesh.Vertices.Count)
                    return TriangleMesh.Vertices[_indexStartPoint];

                return null;
            }
        }

        public Vertex EndPoint {
            get
            {
                var opposite = OppositeHalfEdge;
                if(TriangleMesh != null && opposite != null)
                    return opposite.StartPoint;

                return null;
            }
        }

        public int[] PointIndizes {
            get
            {
                return new int[]
                {
                    _indexStartPoint,
                    EndPoint.Index
                };
            }
        }

        public Triangle Triangle {
            get
            {
                if(TriangleMesh != null && _triangleIndex > -1 && _triangleIndex < TriangleMesh.Triangles.Count)
                    return TriangleMesh.Triangles[_triangleIndex];

                return null;
            }
        }

        public int TriangleIndex {
            set { _triangleIndex = value; }
        }

        public HalfEdge NextHalfEdge {
            get
            {
                if(TriangleMesh != null && _nextHalfEdgeIndex > -1 && _nextHalfEdgeIndex < TriangleMesh.HalfEdges.Count)
                    return TriangleMesh.HalfEdges[_nextHalfEdgeIndex];

                return null;
            }
        }

        public int NextHalfEdgeIndex {
            set { _nextHalfEdgeIndex = value; }
        }

        public HalfEdge OppositeHalfEdge {
            get
            {
                if(TriangleMesh != null && _oppositeHalfeEdgeIndex > -1 && _oppositeHalfeEdgeIndex < TriangleMesh.HalfEdges.Count)
                    return TriangleMesh.HalfEdges[_oppositeHalfeEdgeIndex];

                return null;
            }
        }

        public int OppositeHalfEdgeIndex {
            set { _oppositeHalfeEdgeIndex = value; }
        }

        public HalfEdge PreviousHalfEdge {
            get
            {
                if(TriangleMesh != null && _previousHalfEdgeIndex > -1 && _previousHalfEdgeIndex < TriangleMesh.HalfEdges.Count)
                    return TriangleMesh.HalfEdges[_previousHalfEdgeIndex];

                return null;
            }
        }

        public int PreviousHalfEdgeIndex {
            set { _previousHalfEdgeIndex = value; }
        }

        public double Length {
            get { return _length; }
        }

        public new HalfEdgeMesh TriangleMesh {
            get { return base.TriangleMesh; }
            set
            {
                base.TriangleMesh = value;

                if(value != null)
                    Calculate();
            }
        }

        public bool IsOnBorder {
            get { return Triangle == null || OppositeHalfEdge.Triangle == null; }
        }



        public HalfEdge()
            :base()
        {
            _indexStartPoint = -1;
            _length = 0;
            _triangleIndex = -1;
            _nextHalfEdgeIndex = -1;
            _previousHalfEdgeIndex = -1;
            _oppositeHalfeEdgeIndex = -1;
        }

        public HalfEdge(int startIndex)
            :this()
        {
            _indexStartPoint = startIndex;
        }

        public HalfEdge(HalfEdgeMesh triangleMesh, int startIndex)
            :this(startIndex)
        {
            TriangleMesh = triangleMesh;

            Calculate();
        }

        public HalfEdge(HalfEdgeMesh triangleMesh, int index, int startIndex, int triangleIndex)
            :this(triangleMesh, startIndex)
        {
            Index = index;
            _triangleIndex = triangleIndex;
        }

        public HalfEdge(HalfEdge halfEdge)
            : this(halfEdge.TriangleMesh, halfEdge._indexStartPoint)
        { }


        public object Clone()
        {
            return new HalfEdge(this);
        }

        private void Calculate()
        {
            if (OppositeHalfEdge != null)
                _length = (EndPoint - StartPoint).Length;
        }


        public override string ToString()
        {
            return $"From: [{StartPoint.X:0.00}; {StartPoint.Y:0.00}; {StartPoint.Z:0.00}] " +
                   $"To: [{EndPoint.X:0.00}; {EndPoint.Y:0.00}; {EndPoint.Z:0.00}]";
        }
    }
}