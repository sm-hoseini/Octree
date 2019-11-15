using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

namespace Octrees
{
    public class Octree<T> : MonoBehaviour where T : IOctreeObject
    {
        private List<OctreeNode<T>> NodeObjectPool = new List<OctreeNode<T>>(20);
        public List<BoundingBox> NodeBoundingBox=new List<BoundingBox>(100);//TODO: for now the idea of this list is just for debug asseses(showing gizmos and stuff).Consider a better solution 

        protected OctreeNode<T> RootNode { get; set; }

        public void DespawnNode(OctreeNode<T> node)
        {
            NodeObjectPool.Add(node);
        }

        public OctreeNode<T> SpawnNode(float dimension, int order, Vector3 pos, OctreeNode<T> parent,
            byte regionInParent)
        {
            if (NodeObjectPool.Count == 0)
            {
                AddObjectToPool();
            }

            var obj = NodeObjectPool[NodeObjectPool.Count-1];
            NodeObjectPool.RemoveAt(NodeObjectPool.Count-1);
            obj.InitiateNode(dimension, order, pos, parent, regionInParent);
            return obj;
        }

        private void AddObjectToPool()
        {
            for (int i = 0; i < 20; i++)
            {
                var obj = new OctreeNode<T>();
                NodeObjectPool.Add(obj);
            }
        }


        public void Add(T obj)
        {
            RootNode.Add(obj);
        }

    
        private void OnDrawGizmos()
        {
            if (NodeBoundingBox.Count == 0) return;
            foreach (var box in NodeBoundingBox)
            {
                Gizmos.DrawWireCube(box.Center,box.Dimention*Vector3.one);
            }
        }
    }
}