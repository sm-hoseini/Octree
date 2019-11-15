using UnityEngine;

namespace Octrees
{
    public struct BoundingBox
    {
        public Vector3 Center { get; private set; }
        public float Dimention { get; private set; }
        public Vector3 MinPoint { get; private set; }
        public Vector3 MaxPoint { get; private set; }
        private float xMin, xMax, yMin, yMax, zMin, zMax;

        public BoundingBox(Vector3 center, float dimention) 
        {
            Center = center;
            Dimention = dimention;
            MinPoint = center - Vector3.one * dimention / 2;
            MaxPoint = center + Vector3.one * dimention / 2;
            xMin = MinPoint.x;
            yMin = MinPoint.y;
            zMin = MinPoint.z;
            xMax = MaxPoint.x;
            yMax = MaxPoint.y;
            zMax = MaxPoint.z;
        }

        public bool Encapsulate(Vector3 pos)
        {
            return (pos.CompairTo(MaxPoint) <= 0 && pos.CompairTo(MinPoint) >= 0);
        }

    

       
    }
}