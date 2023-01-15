using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foundry
{
    public class ScollUV : MonoBehaviour
    {
        SkinnedMeshRenderer meshRenderer;
        Material material;
        public float speed;
        void Start()
        {
        
            meshRenderer = GetComponent<SkinnedMeshRenderer>();
            material = meshRenderer.material;
            
        }

        // Update is called once per frame
        void Update()
        {
            ScrollUVs();
        }
        
        //This function scrolls the UVs of the mesh over time
        void ScrollUVs()
        {
            Vector2 offset = material.mainTextureOffset;
            //Scroll the UVs
            offset.x += Time.deltaTime * speed;
            //Scroll the y UVs
            offset.y += Time.deltaTime * speed;
            
            //Set the new offset
            material.mainTextureOffset = offset;
        }
    }
}
