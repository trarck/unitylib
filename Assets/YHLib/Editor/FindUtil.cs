using UnityEngine;
using System.Collections;
using UnityEditor;

public class MyEditor : Editor {


	[MenuItem ("MyMenu/Find")]
	public static void Test(){
		Debug.Log ("test Find");

	}

	public GameObject Search(string path,GameObject root)
	{
		Transform objTransform= root.transform.Find (path);
		
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
		ArrayList list = new ArrayList ();

		Transform rootTransform = root.transform;

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
