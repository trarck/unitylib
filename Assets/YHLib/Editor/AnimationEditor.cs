using UnityEngine;
using System.Collections;
using UnityEditor;

public class AnimationEditor : Editor
{
	[MenuItem ("Animation/Fix Path")]
	public static void FixPath(){

		AnimationClip ac = Selection.activeObject as AnimationClip;

		GameObject panel = GameObject.Find("panel");
	

		if (ac is AnimationClip) {

			Debug.Log("****************---->");
			EditorCurveBinding[] bindings=AnimationUtility.GetCurveBindings (ac);
			for (int i=0;i<bindings.Length;++i)
			{
				EditorCurveBinding binding=bindings[i];
				Debug.Log("binding path="+binding.path+",propName="+binding.propertyName+",PPtrCurve="+binding.isPPtrCurve+",type="+binding.type);

				AnimationCurve aniCurve=AnimationUtility.GetEditorCurve(ac,binding);
	
				Object t=AnimationUtility.GetAnimatedObject(panel,binding);

				if(t){
					Debug.Log("test:"+t);
				}else{
					Debug.Log("can't find:"+binding.path);
					string path=binding.path;

					int pos=path.IndexOf("/");

					path=path.Substring(pos+1);

					Debug.Log("lookFor:"+path);

					t=panel.transform.Find(path).gameObject;

					string newPath=AnimationUtility.CalculateTransformPath((t as GameObject).transform,panel.transform);
					Debug.Log("path:"+newPath);

					AnimationUtility.SetEditorCurve(ac,binding,null);
					binding.path=newPath;
					AnimationUtility.SetEditorCurve(ac,binding,aniCurve);
				}
			}

			AssetDatabase.SaveAssets();

			Debug.Log("****************<----");

		}else{
			Debug.Log("not AnimationClip");
			Debug.Log((Selection.activeObject));
		}
		
//		AnimationUtility.onCurveWasModified = OnCurveWasModified;
		/*
		AnimationClipCurveData[] datas = AnimationUtility.GetAllCurves ();
		for (int i=0; i<datas.Length; ++i) {
			Debug.Log(datas[i].ToString());
		}
		*/
	}

	public static void OnCurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType deleted)
	{
		Debug.Log("onEvent:"+binding.path+","+deleted);
		binding.path = "Cube";
	}
}
