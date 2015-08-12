using UnityEngine;
using System.Collections;
using UnityEditor;

public class FindUtil{
	private static readonly FindUtil s_Instance =new FindUtil();

	static FindUtil()
	{

	}

	public static FindUtil Instance
	{
		get
		{
			return s_Instance;
		}
	}

	public GameObject Search(string path,GameObject root)
	{
		return Search (path, root.transform);
	}

	public GameObject Search(string path,Transform rootTransform)
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
				GameObject childFind= Search(path,rootTransform.GetChild(i));
				
				if(childFind)
				{
					return childFind;
				}
			}
			
			return null;
		}
	}

	public ArrayList SearchAll(string path,GameObject root)
	{
		return SearchAll (path, root.transform);
	}

	public ArrayList SearchAll(string path,Transform rootTransform)
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
				ArrayList childFinds=SearchAll(path,rootTransform.GetChild(i));
				list.AddRange(childFinds);
			}
		}
		
		return list;
	}
}
