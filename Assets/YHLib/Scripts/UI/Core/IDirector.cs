using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH;

namespace UI
{
    public interface IDirector
    {
        void Push(string panelAsset, object data = null);
        void Pop();
        void Replace(string panelAsset, object data = null);
        void PopToStackLevel(int level);
        void PopToRoot();
        UIPanel active{ get; }
    }
}