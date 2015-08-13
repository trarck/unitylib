using UnityEngine;
using System.Collections;
using UnityEditor;

public class MyWindow : EditorWindow {

	string m_Path ="";

	// Add menu item named "My Window" to the Window menu
	[MenuItem("MyMenu/My Window")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(MyWindow));
	}
	
	void OnGUI()
	{
		GUILayout.Label ("Base Settings", EditorStyles.boldLabel);

		m_Path =	EditorGUILayout.TextField ("path", m_Path);

		if (GUILayout.Button ("click")) 
		{
			TestFind(m_Path);
		}
	}

	void TestFind(string path)
	{
		Debug.Log("look for "+ path);
		GameObject root = Selection.activeGameObject;
		GameObject obj = FindUtil.SearchGameObject(path, root);
		Debug.Log ("searched:"+obj);
	}
}
