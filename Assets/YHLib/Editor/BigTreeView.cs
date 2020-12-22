using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class BigTreeView : TreeView
{
	public int num = 1;

	public BigTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) :base(state,multiColumnHeader)
	{
		useScrollView = true;
	}

	protected override TreeViewItem BuildRoot()
	{
		TreeViewItem root = new TreeViewItem(1,-1, "tt");

		TreeViewItem item = null;

		for (int i = 0; i < num; ++i)
		{
			item = new TreeViewItem(i + 1, 0, i.ToString());
			root.AddChild(item);
		}

		return root;
	}

	//protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
	//{

	//	List<TreeViewItem> rows = new List<TreeViewItem>();
	//	TreeViewItem item = null;

	//	for (int i = 0; i < num; ++i)
	//	{
	//		item = new TreeViewItem(i+1, 0, i.ToString());
	//		rows.Add(item);
	//	}


	//	//SetupParentsAndChildrenFromDepths(root, rows);

	//	return rows;
	//}

	public void GetTest(out int a, out int b)
	{
		GetFirstAndLastVisibleRows(out a, out b);
	}
}
