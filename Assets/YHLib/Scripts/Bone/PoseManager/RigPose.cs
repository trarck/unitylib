using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
namespace YH.Bone
{
    [Serializable]
    public class RigPose : ScriptableObject
    {
        public List<PoseTransform> bonePoses;

        public static RigPose Create (string poseName, List<Transform> bones, Matrix4x4[] bindPoses) 
        {
            if (bones.Count != bindPoses.Length)
                throw new UnityException ("Cannot create Pose as bone- (" + bones.Count + ") and bindPose count (" + bindPoses.Length + ") do not match!");

            RigPose rigPose = RigPose.CreateInstance<RigPose> ();
            rigPose.name = poseName;
            rigPose.bonePoses = new List<PoseTransform> (bones.Count);
            for (int boneCnt = 0; boneCnt < bones.Count; boneCnt++) 
            {
                Transform bone = bones[boneCnt];
                Matrix4x4 localTrans;
                if (bone.parent != null)
                {
                    int parentBoneIndex = bones.FindIndex ((Transform b) => b.name == bone.parent.name);
                    if (parentBoneIndex >= 0)
                        localTrans = BoneManager.GetLocalMatrix (bindPoses[boneCnt], bindPoses[parentBoneIndex]);
                    else
                        localTrans = bindPoses[boneCnt].inverse;
                }
                else
                    localTrans = bindPoses[boneCnt].inverse;
                rigPose.bonePoses.Add (new PoseTransform (bones[boneCnt].name, localTrans));
            }
            return rigPose;
        }

        public static RigPose Create (string poseName, Transform[] bones) 
        {
            RigPose rigPose = RigPose.CreateInstance<RigPose> ();
            rigPose.name = poseName;
            rigPose.bonePoses = new List<PoseTransform> (bones.Length);
            for (int boneCnt = 0; boneCnt < bones.Length; boneCnt++)
                rigPose.bonePoses.Add (new PoseTransform (bones[boneCnt]));
            return rigPose;
        }
    }

    [Serializable]
    public struct PoseTransform
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public PoseTransform (Transform trans) 
        {
            name = trans.name;
            position = trans.localPosition;
            rotation = trans.localRotation;
            scale = trans.localScale;
        }

        public PoseTransform (string poseName, Matrix4x4 localTrans) 
        {
            name = poseName;
            position = BoneManager.DecodePosition (localTrans);
            rotation = BoneManager.DecodeRotation (localTrans);
            scale = BoneManager.DecodeScale (localTrans);
        }

        public PoseTransform (string poseName, Vector3 localPos, Quaternion localRot, Vector3 localScale) 
        {
            name = poseName;
            position = localPos;
            rotation = localRot;
            scale = localScale;
        }

        public void SetTransform (ref Transform trans) 
        {
            trans.localPosition = position;
            trans.localRotation = rotation;
            trans.localScale = scale;
        }
    }
}