using System.Collections;
using System.Collections.Generic;
using Octrees;
using UnityEngine;

namespace Octrees
{
    public class DrivedOctree : Octree<IOctreeObject>
    {
        [SerializeField] private int maxWorldDimension = 1000;

        [SerializeField] private Vector3 treeOrigin;

        // Start is called before the first frame update
        void Awake()
        {
            RootNode = SpawnNode(maxWorldDimension, 0, treeOrigin, null, 0);

        }


    }
}
