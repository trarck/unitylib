using UnityEngine;
using System.Collections;
using UnityEditor;

public class FindUtil{

//	private static readonly FindUtil s_Instance =new FindUtil();
//
//	static FindUtil()
//	{
//
//	}
//
//	public static FindUtil Instance
//	{
//		get
//		{
//			return s_Instance;
//		}
//	}

	public static GameObject SearchGameObject(string path,GameObject root)
	{
		return SearchGameObject (path, root.transform);
	}

	public static GameObject SearchGameObject(string path,Transform rootTransform)
	{
		Transform objTransform= rootTransform.Find (path);
		
		if (objTransform) {
			return objTransform.gameObject;
		} 
		else 
		{
			//在子元素中查找
			for(int i=0,l=rootTransform.childCount;i<l;++i)
			{
				GameObject childFind= SearchGameObject(path,rootTransform.GetChild(i));
				
				if(childFind)
				{
					return childFind;
				}
			}
			
			return null;
		}
	}

	public static Transform SearchTransform(string path,Transform rootTransform)
	{
		Transform objTransform= rootTransform.Find (path);
		
		if (objTransform) {
			return objTransform;
		} 
		else 
		{
			//在子元素中查找
			for(int i=0,l=rootTransform.childCount;i<l;++i)
			{
				Transform childFind= SearchTransform(path,rootTransform.GetChild(i));
				
				if(childFind)
				{
					return childFind;
				}
			}
			
			return null;
		}
	}

	public static ArrayList SearchGameObjects(string path,GameObject root)
	{
		return SearchGameObjects (path, root.transform);
	}

	public static ArrayList SearchGameObjects(string path,Transform rootTransform)
	{
		ArrayList list = new ArrayList ();

		Transform objTransform= rootTransform.Find (path);
		
		if (objTransform) {
			list.Add (objTransform.gameObject);
		} 
		else 
		{
			//在子元素中查找
			for(int i=0,l=rootTransform.childCount;i<l;++i)
			{
				ArrayList childFinds=SearchGameObjects(path,rootTransform.GetChild(i));
				list.AddRange(childFinds);
			}
		}
		
		return list;
	}

	public static ArrayList SearchTransforms(string path,Transform rootTransform)
	{
		ArrayList list = new ArrayList ();
		
		Transform objTransform= rootTransform.Find (path);
		
		if (objTransform) {
			list.Add (objTransform);
		} 
		else 
		{
			//在子元素中查找
			for(int i=0,l=rootTransform.childCount;i<l;++i)
			{
				ArrayList childFinds=SearchTransforms(path,rootTransform.GetChild(i));
				list.AddRange(childFinds);
			}
		}
		
		return list;
	}
}
