using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
namespace YH.Bone
{
    [CustomEditor(typeof(PoseManager))]
    public class PoseManagerEditor : Editor 
    {
        PoseManager poseManager;

        public void OnEnable () 
        {
            poseManager = (PoseManager)target;
        }

        public override void OnInspectorGUI () 
        {
            if (poseManager.skinnedRenderer == null)
                poseManager.skinnedRenderer = poseManager.GetComponentInChildren<SkinnedMeshRenderer> ();
            poseManager.skinnedRenderer = EditorGUILayout.ObjectField ("Skinned Mesh", poseManager.skinnedRenderer, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
            if (poseManager.skinnedRenderer == null || poseManager.skinnedRenderer.sharedMesh == null)
                return;

            EditorGUILayout.Space ();
            
            // Display associated Poses	
            if (poseManager.poses.Count > 0)
            {
                for (int poseCnts = 0; poseCnts < poseManager.poses.Count; poseCnts++) 
                {
                    RigPose pose = poseManager.poses[poseCnts];

                    if (pose == null) 
                    {
                        poseManager.poses.RemoveAt (poseCnts);
                        poseCnts--;
                        continue;
                    }

                    GUILayout.BeginHorizontal ();
                    GUILayout.Label (pose.name, new GUILayoutOption[] { GUILayout.MinWidth (100), GUILayout.MaxWidth (Screen.width/2) });
                    if (GUILayout.Button ("Set Pose", GUILayout.ExpandWidth (true)))
                        poseManager.SetPose (pose);
                    GUILayout.FlexibleSpace ();
                    if (GUILayout.Button ("^", GUILayout.Width (20)) && poseCnts > 0) 
                    {
                        poseManager.poses.RemoveAt (poseCnts);
                        poseManager.poses.Insert (poseCnts-1, pose);
                    }
                    if (GUILayout.Button ("v", GUILayout.Width (20)) && poseCnts < poseManager.poses.Count-1) 
                    {
                        poseManager.poses.RemoveAt (poseCnts);
                        poseManager.poses.Insert (poseCnts+1, pose);
                    }
                    GUILayout.FlexibleSpace ();
                    if (GUILayout.Button ("X", GUILayout.Width (20))) 
                    {
                        poseManager.poses.RemoveAt (poseCnts);
                        poseCnts--;
                    }
                    GUILayout.EndHorizontal ();
                }
            }
            else
                GUILayout.Label ("No Poses for this mesh!");

            EditorGUILayout.Space ();

            // Manage Poses

            if (GUILayout.Button ("Load Pose")) 
            {
                string path = EditorUtility.OpenFilePanel ("Load Pose", Application.dataPath, "asset");
                if (!string.IsNullOrEmpty (path))
                {
                    path = path.Replace (Application.dataPath, "Assets");
                    RigPose pose = AssetDatabase.LoadAssetAtPath<RigPose> (path);
                    if (pose != null)
                        poseManager.LoadPose (pose);
                    else
                        Debug.Log ("Chosen asset is not a valid RigPose!");
                }
            }

            if (GUILayout.Button ("Save Current Pose")) 
            {
                string savePath = EditorUtility.SaveFilePanelInProject ("Save Current Pose", "Pose", "asset", "Choose path to save the current pose in!");
                if (!string.IsNullOrEmpty (savePath))
                {
                    string poseName = Path.GetFileNameWithoutExtension (savePath);
                    RigPose curPose = poseManager.SaveCurrentPose (poseName);
                    AssetDatabase.CreateAsset (curPose, savePath);
                }
            }

            if (GUILayout.Button ("Restore Rest Pose")) 
            {
                poseManager.RestoreRestPose ();
            }

            if (GUILayout.Button ("Fix Mesh")) 
            {
                poseManager.TryFixMesh ();
            }
        }

        /// <summary>
        /// Adds the item to the array
        /// </summary>
        public static T[] AddArrayItem<T> (T[] array, T item) 
        {
            System.Array.Resize<T> (ref array, array.Length+1);
            array [array.Length-1] = item;
            return array;
        }
    }
}