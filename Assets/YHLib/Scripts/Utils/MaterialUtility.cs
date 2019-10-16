using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH
{
    public class MaterialUtility
    {
        public static Material GetMaterial(Renderer renderer,int matIndex)
        {
            if (matIndex >= renderer.sharedMaterials.Length)
            {
                return null;
            }

            if (matIndex < 0)
            {
                matIndex = 0;
            }
            Material material = null;
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                material = new Material(renderer.sharedMaterials[matIndex]);
                List<Material> ms = new List<Material>();
                ms.AddRange(renderer.sharedMaterials);
                ms[matIndex] = material;
                renderer.sharedMaterials = ms.ToArray();
            }
            else
            {
                material = renderer.materials[matIndex];
            }
#else
                material = renderer.sharedMaterials[matIndex];
#endif
            return material;
        }

        public static void SetMaterial(Renderer renderer, int matIndex,Material mat)
        {
            List<Material> ms = new List<Material>();
            ms.AddRange(renderer.sharedMaterials);
            ms[matIndex] = mat;
            renderer.sharedMaterials = ms.ToArray();
        }
    }
}
