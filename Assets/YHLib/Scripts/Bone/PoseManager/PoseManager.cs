using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
namespace YH.Bone
{
    [ExecuteInEditMode]
    public class PoseManager : MonoBehaviour 
    {
        public SkinnedMeshRenderer skinnedRenderer;
        public List<RigPose> poses = new List<RigPose> ();

        public void OnEnable () 
        {
            if (skinnedRenderer == null)
                skinnedRenderer = GetComponentInChildren<SkinnedMeshRenderer> ();
        }


        public void AssureSetup () 
        {
            if (skinnedRenderer == null)
                skinnedRenderer = GetComponentInChildren<SkinnedMeshRenderer> ();
            if (skinnedRenderer == null)
                throw new UnityException ("Please select a SkinnedMeshRenderer!");
        }

        public void TryFixMesh () 
        {
            if (skinnedRenderer.bones.Length != skinnedRenderer.sharedMesh.bindposes.Length)
            {
                Transform[] newBones = new Transform[skinnedRenderer.sharedMesh.bindposes.Length];
                Array.Copy (skinnedRenderer.bones, newBones, skinnedRenderer.sharedMesh.bindposes.Length);
                skinnedRenderer.bones = newBones;
            }
        }

        public void RestoreRestPose () 
        {
            RigPose restPose = RigPose.Create ("Rest Pose", skinnedRenderer.bones.ToList (), skinnedRenderer.sharedMesh.bindposes);
            SetPose (restPose);
        }

        public RigPose SaveCurrentPose (string name) 
        {
            AssureSetup ();
            RigPose curPose = RigPose.Create (name, skinnedRenderer.bones);
            poses.Add (curPose);
            return curPose;
        }

        public void LoadPose (RigPose pose)
        {
            AssureSetup ();
            if (pose.bonePoses.Count == skinnedRenderer.bones.Length)
                poses.Add (pose);
            else
                Debug.LogError ("Cannot load pose as bone count does not match: Mesh: " + skinnedRenderer.bones.Length + "; Pose: " + pose.bonePoses.Count);
        }

        public void SetPose (RigPose pose)
        {
            if (pose == null)
                throw new UnityException ("Trying to set null pose to PoseManager " + name + "!");
            AssureSetup ();
            
            foreach (PoseTransform bonePose in pose.bonePoses) 
            {
                Transform poseBone = BoneUtils.Find (skinnedRenderer.rootBone, bonePose.name);
                if (poseBone != null) 
                {
                    #if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject (poseBone, "Set Pose " + pose.name + " on " + skinnedRenderer.name);
                    #endif
                    bonePose.SetTransform (ref poseBone);
                }
                else
                    Debug.LogError ("Pose bone " + bonePose.name + " has no valid associated bone!");
            }
            #if UNITY_EDITOR
            UnityEditor.Undo.FlushUndoRecordObjects ();
            #endif
        }
    }
}