using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using HalfEdgeDataStructure;
using HelixToolkit.Wpf;
using Triangle = HalfEdgeDataStructure.Triangle;

namespace Helper
{
    /// <summary>
    /// Enumeration for all Cube Sides.
    /// </summary>
    [Flags]
    public enum CubeSides
    {
        PositiveX = 0x100000,
        NegativeX = 0x010000,
        X = PositiveX | NegativeX,
        PositiveY = 0x001000,
        NegativeY = 0x000100,
        Y = PositiveY | NegativeY,
        PositiveZ = 0x000010,
        NegativeZ = 0x000001,
        Z = PositiveZ | NegativeZ,
        All = X | Y | Z
    }

    /// <summary>
    /// Helper Class to generate HalfEdgeMeshes.
    /// </summary>
    public static class HalfEdgeMeshGenerator
    {
        /// <summary>
        /// Generate a Test HalfEdgeMesh that was used to test the Creation and Modification of the HalfEdges.
        /// </summary>
        /// <returns>The generated HalfEdgeMesh.</returns>
        public static HalfEdgeMesh GenerateHalfEdgeTests()
        {
            var points = new List<Vertex> {
                new Vertex(0, 0, 0), new Vertex(2, 0, 0), new Vertex(1, 1, 0),
                new Vertex(4, 0, 0), new Vertex(3, 1, 0),
                new Vertex(2, 2, 0),
                new Vertex(4, 2, 0),
                new Vertex(6, 2, 0), new Vertex(5, 3, 0)

            };
            var triangles = new List<Triangle> {
                new Triangle(0, 1, 2),
                new Triangle(3, 4, 1),
                new Triangle(4, 5, 2),
                new Triangle(5, 4, 6),
                new Triangle(8, 6, 7),
                new Triangle(4, 3, 7),
                new Triangle(5, 6, 8),
                new Triangle(4, 2, 1),
                new Triangle(4, 7, 6)
            };
            return new HalfEdgeMesh(points, triangles);
        }

        /// <summary>
        /// Generate a HalfEdgeMesh of a Cube.
        /// </summary>
        /// <param name="origin">The Origin of the Cube.</param>
        /// <param name="size">The Size of the Cube.</param>
        /// <param name="sides">Defines which Sides of the Cube should be generated (default = All Sides).</param>
        /// <returns>The generated HalfEdgeMesh.</returns>
        public static HalfEdgeMesh GenerateCube(Vector origin, double size = 1, CubeSides sides = CubeSides.All)
        {
            var points = new List<Vertex>()
            {
                new Vertex(0, 0, 0),
                new Vertex(1, 0 ,0),
                new Vertex(1, 1 ,0),
                new Vertex(0, 1 ,0),
                new Vertex(0, 0, 1),
                new Vertex(1, 0 ,1),
                new Vertex(1, 1 ,1),
                new Vertex(0, 1 ,1),
            };
            points = points.Select(p => new Vertex(new Vector(p) * size + origin)).ToList();
            var triangles = new List<Triangle>();
            if(sides.HasFlag(CubeSides.NegativeZ))
            {
                triangles.Add(new Triangle(0, 2, 1));
                triangles.Add(new Triangle(0, 3, 2));
            }
            if(sides.HasFlag(CubeSides.PositiveZ))
            {
                triangles.Add(new Triangle(4, 5, 6));
                triangles.Add(new Triangle(4, 6, 7));
            }
            if(sides.HasFlag(CubeSides.NegativeY))
            {
                triangles.Add(new Triangle(0, 1, 5));
                triangles.Add(new Triangle(0, 5, 4));
            }
            if(sides.HasFlag(CubeSides.PositiveX))
            {
                triangles.Add(new Triangle(1, 2, 6));
                triangles.Add(new Triangle(1, 6, 5));
            }
            if(sides.HasFlag(CubeSides.PositiveY))
            {
                triangles.Add(new Triangle(2, 3, 7));
                triangles.Add(new Triangle(2, 7, 6));
            }
            if(sides.HasFlag(CubeSides.NegativeX))
            {
                triangles.Add(new Triangle(3, 0, 4));
                triangles.Add(new Triangle(3, 4, 7));
            }
            return new HalfEdgeMesh(points, triangles);
        }

        /// <summary>
        /// Generate a HalfEdgeMesh from an existing MeshGeometry3D Object.
        /// </summary>
        /// <param name="meshGeometry">The existing MeshGeometry3D Object.</param>
        /// <returns>The generated HalfEdgeMesh.</returns>
        public static HalfEdgeMesh GenerateFromMeshGeometry3D(MeshGeometry3D meshGeometry)
        {
            var points = meshGeometry.Positions.Select(p => new Vertex(p.X, p.Y, p.Z)).ToList();
            var triangles = new List<Triangle>();
            for(int i = 0; i < meshGeometry.TriangleIndices.Count; i += 3)
                triangles.Add(new Triangle(meshGeometry.TriangleIndices.ElementAt(i),
                    meshGeometry.TriangleIndices.ElementAt(i + 1),
                    meshGeometry.TriangleIndices.ElementAt(i + 2)));

            return new HalfEdgeMesh(points, triangles);
        }
    }
}