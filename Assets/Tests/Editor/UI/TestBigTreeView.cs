using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class TestBigTreeView : EditorWindow
{
	TreeViewState _state;
	BigTreeView _bigTreeView;
	Vector2 _scrollPosition;

	// Add menu item named "My Window" to the Window menu
	[MenuItem("MyMenu/TestBigTreeView")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(TestBigTreeView));
	}

	void OnGUI()
	{
		if (_bigTreeView == null)
		{
			_state = new TreeViewState();

			_bigTreeView = new BigTreeView(_state, null);
			_bigTreeView.num = 100000;
			_bigTreeView.Reload();
		}



		Rect r = new Rect(20,20,300,150);
		_bigTreeView.OnGUI(r);

		int a = 0;
		int b = 0;
		_bigTreeView.GetTest(out a, out b);
		Debug.LogFormat("{0},{1}", a, b);
 	}
}
