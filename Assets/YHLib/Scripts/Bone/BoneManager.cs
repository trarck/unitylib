using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace YH.Bone
{
    public static class BoneManager 
    {
        /// <summary>
        /// Searches for a transform with the given name in the given parent transformation recursively
        /// </summary>
        public static Transform Find (Transform parentTrans, string name) 
        {
            if (parentTrans.name == name)
                return parentTrans;
            foreach (Transform child in parentTrans)
            {
                Transform foundChild = Find (child, name);
                if (foundChild != null) 
                    return foundChild;
            }
            return null;
        }

        // BindPose Utility

        /// <summary>
        /// Creates a bindPose for the bone in it's current pose
        /// RootBone has to be the parent bone in most cases: The space of the bindPose
        /// </summary>
        public static Matrix4x4 CreateBoneBindPose (Transform bone, Transform rootBone) 
        {
            return bone.worldToLocalMatrix * rootBone.localToWorldMatrix;
        }

        /// <summary>
        /// Gets the local transformation matrix from the given bindPose
        /// </summary>
        public static Matrix4x4 GetLocalMatrix (Matrix4x4 bindPose, Matrix4x4 parentBindPose) 
        {
            return (bindPose * parentBindPose.inverse).inverse;
        }
        /// <summary>
        /// Gets the local transformation matrix from the given bindPose
        /// </summary>
        public static Matrix4x4 GetLocalMatrix (Matrix4x4 bindPose) 
        {
            return bindPose.inverse;
        }

        /// <summary>
        /// Restores the bone pose from the given bindPose
        /// </summary>
        public static void RestoreBonePose (ref Transform bone, Matrix4x4 bindPose, Matrix4x4 parentBindPose) 
        {
            RestorePosition (ref bone, GetLocalMatrix (bindPose, parentBindPose));
        }
        /// <summary>
        /// Restores the bone pose from the given bindPose
        /// </summary>
        public static void RestoreBonePose (ref Transform bone, Matrix4x4 bindPose) 
        {
            RestorePosition (ref bone, bindPose.inverse);
        }
        private static void RestorePosition (ref Transform objTrans, Matrix4x4 localTrans) 
        {
            objTrans.localPosition = localTrans.DecodePosition ();
            objTrans.localRotation = localTrans.DecodeRotation ();
            objTrans.localScale = localTrans.DecodeScale ();
        }

        /// <summary>
        /// Decodes the position from the given transformation matrix
        /// </summary>
        public static Vector3 DecodePosition (this Matrix4x4 matrix) 
        {
            return matrix.MultiplyPoint (Vector3.zero);
        }
        /// <summary>
        /// Decodes the rotation from the given transformation matrix
        /// </summary>
        public static Quaternion DecodeRotation (this Matrix4x4 matrix) 
        {
            return Quaternion.LookRotation (matrix.GetColumn (2), matrix.GetColumn (1));
        }
        /// <summary>
        /// Decodes the scale from the given transformation matrix
        /// </summary>
        public static Vector3 DecodeScale (this Matrix4x4 matrix) 
        {
            return new Vector3 (matrix.GetColumn (0).magnitude, matrix.GetColumn (1).magnitude, matrix.GetColumn (2).magnitude);
        }
    }
}