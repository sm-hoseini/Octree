using System.Collections;
using System.Collections.Generic;
using Octrees;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [SerializeField] private OctObject ItemPrefab;

    [SerializeField] private DrivedOctree octree;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5000; i++)
        {
            var go= Instantiate(ItemPrefab, Random.insideUnitSphere * 500,Quaternion.identity);
            var octObj = go.GetComponent<OctObject>();
            octree.Add(octObj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}