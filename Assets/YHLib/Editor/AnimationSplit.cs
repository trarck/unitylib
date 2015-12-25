using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AnimationSplit : AssetPostprocessor {

	void OnPreprocessModel()
    {
        Debug.Log(assetPath);
        if (assetPath.ToLower().IndexOf(".fbx") > -1)
        {
            SplitFbxAniamtion();
        }
    }

    void SplitFbxAniamtion()
    {
        ModelImporter fbxModelImporter = assetImporter as ModelImporter;

        List<ModelImporterClipAnimation> clips = new List<ModelImporterClipAnimation>();

        clips.Add(CreateClipAnimation("Stand", 50, 149,true));
        clips.Add(CreateClipAnimation("Walk", 249, 258));
        clips.Add(CreateClipAnimation("GLstand", 358, 478));
        Debug.Log(fbxModelImporter.clipAnimations.Length);
        fbxModelImporter.clipAnimations = clips.ToArray();
        Debug.Log(fbxModelImporter.clipAnimations.Length);

    }

    ModelImporterClipAnimation CreateClipAnimation(string name,int firstFrame,int lastFrame,bool loop=false)
    {
        ModelImporterClipAnimation clip = new ModelImporterClipAnimation();
        clip.name = name;
        clip.firstFrame = firstFrame;
        clip.lastFrame = lastFrame;

        clip.loopTime = loop;
        clip.loop = loop;

        Debug.Log(loop);

        if (loop)
        {
            clip.wrapMode = WrapMode.Loop;
        }

        //else
        //{
        //    clip.wrapMode = WrapMode.Default;
        //}       
        return clip;
    }
}
