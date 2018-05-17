using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

namespace YH
{
    public class FindAsset
    {
        public static List<string> FindAllAssets(string path,string filter)
        {
            List<string> result = new List<string>();

            string[] allGuids = AssetDatabase.FindAssets(filter, new string[] { path });
            for(int i = 0; i < allGuids.Length; ++i)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(allGuids[i]);
                result.Add(assetPath);
            }

            return result;
        }
    }
}