using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Octrees
{
    public class OctreeNode<T> where T : IOctreeObject
    {
        private int order;
        private OctreeNode<T> parentNode;
        private byte regionInParentNode;
        private BoundingBox boundingBox;
        public static Octree<T> octreeHandler;
        private List<T> objects;
        public int MemberCount => objects.Count;
        private const int Max_Allowed_Object = 3;
        private const float Min_Node_Dimension = 1;
        private int ObjectCount;
        private bool isSplit;

        public bool IsSplit => isSplit;

        private Dictionary<byte, OctreeNode<T>> childrenNodes;
        private bool running;

        public enum Regions
        {
            //B: Back F:Front L:Left R:Right D:Down U:Up
            BLD = 0,
            BLU = 1,
            BRD = 2,
            BRU = 3,
            FLD = 4,
            FLU = 5,
            FRD = 6,
            FRU = 7,
        }

        private Regions RegionEnum => (Regions) regionInParentNode;


        /// <summary>Byte: 0ZXY  Z{Back=0,Front=1} X{Left=0,Right=1} Y{Down=0,Up=1}</summary>
        public OctreeNode()
        {
        }

        public void InitiateNode(float dimension, int order, Vector3 center, OctreeNode<T> parentNode,
            byte regionInParentNode)
        {
            this.order = order;
            this.parentNode = parentNode;
            this.regionInParentNode = regionInParentNode;
            boundingBox = new BoundingBox(center, dimension);
            objects = new List<T>(Max_Allowed_Object);
            SetOctreeHandler();
            isSplit = false;
            ObjectCount = 0;
            running = true;
            octreeHandler.NodeBoundingBox.Add(boundingBox);
        }

        private static void SetOctreeHandler()
        {
            if (octreeHandler == null)
            {
                octreeHandler = GameObject.FindObjectOfType<Octree<T>>();
            }
        }

        public void Add(in T obj)
        {
            if (!boundingBox.Encapsulate(obj.Position))
            {
                Debug.LogError($"object with position {obj.Position} is out of current Node {this}");
                return;
            }

            if (isSplit)
            {
                AssingToChildNode(obj);
            }
            else
            {
                if (ObjectCount >= Max_Allowed_Object)
                {
                    Split();
                    MoveObjectsToChildrenNodes();
                    AssingToChildNode(obj);
                }
                else
                {
                    ObjectCount++;
                    objects.Add(obj);
                }
            }
        }

        public void Remove(in T octreeObject)
        {
            if (isSplit)
            {
                childrenNodes[GetRegion(octreeObject.Position)].Remove(octreeObject);
            }
            else
            {
                if (objects.Remove(octreeObject))
                {
                    if (CanMerge()) MergeChildren();
                }
                else
                {
                    Debug.LogError("Attend to remove an object before adding it to this Node.");
                }
            }
        }

        private bool CanMerge()
        {
            if (!isSplit) return false;
            if (ObjectCount <= Max_Allowed_Object)
            {
                foreach (var childNode in childrenNodes.Values.ToList())
                {
                    if (childNode.IsSplit)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void MergeChildren()
        {
            foreach (var child in childrenNodes.Values.ToList())
            {
                if (child.IsSplit)
                {
                    throw new InvalidOperationException(
                        "Despawning a node with children (split Node) can be expensive operations so that is not allowed.Consider despawning children at first");
                }

                child.GetObjectsAndDespawn(objects);
            }

            childrenNodes = new Dictionary<byte, OctreeNode<T>>(4);
            isSplit = false;
        }

        public void GetObjectsAndDespawn(in List<T> objectsList)
        {
            objectsList.AddRange(objects);
            objects.Clear();
            ObjectCount = 0;
            running = false;
           var bound= octreeHandler.NodeBoundingBox.FindIndex(x => x.Center == boundingBox.Center);
           octreeHandler.NodeBoundingBox.RemoveAt(bound);
            octreeHandler.DespawnNode(this);
        }

        private byte GetRegion(in Vector3 position)
        {
            //0ZXY  1:Front,Right,Up --- 0:Back,Left,Down
            byte region = 0;
            if (position.x >= boundingBox.Center.x)
            {
                region |= 2;
            }

            if (position.y >= boundingBox.Center.y)
            {
                region |= 1;
            }

            if (position.z >= boundingBox.Center.z)
            {
                region |= 4;
            }

            return region;
        }

        private void AssingToChildNode(T obj)
        {
            if (!isSplit)
            {
                throw new InvalidOperationException(
                    "Before Assign any object to children nodes, the parent node must be split");
            }

            childrenNodes[GetRegion(obj.Position)].Add(obj);
        }

        private void MoveObjectsToChildrenNodes()
        {
            foreach (var octreeObject in objects)
            {
                AssingToChildNode(octreeObject);
            }

            objects.Clear();
        }

        private void Split()
        {
            if (isSplit)
            {
                throw new InvalidOperationException("Attended to split a node which is already splited.");
            }

            if (boundingBox.Dimention / 2 < Min_Node_Dimension)
            {
                Debug.LogError("Fail to split OctreeNode.Splitting Octree has reached to its minimum dimension");
                return;
            }

            childrenNodes = new Dictionary<byte, OctreeNode<T>>(4);
            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    for (int k = -1; k <= 1; k += 2)
                    {
                        var center = boundingBox.Dimention / 4 * new Vector3(i, j, k) + boundingBox.Center;
                        var region = GetRegion(center);
                        Debug.Log($"i:{i} j:{j} k:{k}region {region}");
                        childrenNodes.Add(region,
                            octreeHandler.SpawnNode(boundingBox.Dimention / 2, order + 1, center, this, region));
                    }
                }

//               
            }

            isSplit = true;
        }

        public override string ToString()
        {
            return $"Node order:{order} center:{boundingBox.Center} region:{RegionEnum}";
        }

//        private void OnDrawGizmos()
//        {
//            if (!running) return;
//            Gizmos.DrawCube(boundingBox.Center,boundingBox.Dimention*Vector3.zero);
//        }
    }

    public interface IOctreeObject
    {
        Vector3 Position { get; }
    }
}