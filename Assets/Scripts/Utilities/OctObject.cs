using Octrees;
using UnityEngine;

public class OctObject:MonoBehaviour,IOctreeObject
{
    public Vector3 Position => transform.position;
}