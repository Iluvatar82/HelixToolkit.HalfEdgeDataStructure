using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfEdgeDataStructure
{
    public class VectorComparer : IEqualityComparer<Vector>
    {

        public bool Equals(Vector first, Vector second)
        {
            if(first == default(Vector) && second == default(Vector))
                return true;

            if(first == default(Vector) && second == default(Vector))
                return false;

            if((first - second).LengthSquared < 0.0001)
                return true;

            return false;
        }

        public int GetHashCode(Vector obj)
        {
            return obj.GetHashCode();
        }
    }
}
