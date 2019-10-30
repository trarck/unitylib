﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH;

namespace YH.UI
{
    public class UIManager : UnitySingleton<UIManager>
    {
        [SerializeField]
        RectTransform m_Root;
        List<UIPanel> m_Chidren = new List<UIPanel>();

        Dictionary<string, UIPanel> m_Actives = new Dictionary<string, UIPanel>();

        //panel管理。
        IDirector m_Director=null;

        bool m_IsInit = false;

        [SerializeField]
        string m_DefaultPanel = "";

        protected override  void Awake()
        {
            base.Awake();
            Init();
        }

        public void Init()
        {
            if (m_IsInit)
            {
                return;
            }
            m_IsInit = true;
            InitRoot();
            m_Director = new UIDirector(this);
        }

        public void InitRoot()
        {
            if (m_Root == null)
            {
                m_Root = GetComponentInChildren<RectTransform>();
            }
        }

        #region Panel
        public void AddPanel(UIPanel panel)
        {
            for (int i = 0, l = m_Chidren.Count; i < l; ++i)
            {
                if (panel.depth < m_Chidren[i].depth)
                {
                    m_Chidren.Insert(i, panel);
                    //check hierarchy
                    if (panel.transform.parent != transform)
                    {
                        panel.transform.SetParent(transform);
                    }
                    //set SiblingIndex
                    panel.transform.SetSiblingIndex(i);
                    return;
                }
            }

            m_Chidren.Add(panel);
        }

        public void RemovePanel(UIPanel panel)
        {
            m_Chidren.Remove(panel);
        }

        public void RemovePanel(string panelName)
        {
            for (int i = 0, l = m_Chidren.Count; i < l; ++i)
            {
                if (panelName == m_Chidren[i].name)
                {
                    m_Chidren.RemoveAt(i);
                    return;
                }
            }
        }

        public void RemovePanels(string panelName)
        {
            for (int i = m_Chidren.Count-1;i>=0;--i)
            {
                if (panelName == m_Chidren[i].name)
                {
                    m_Chidren.RemoveAt(i);
                }
            }
        }

        public UIPanel GetPanel(string panelName)
        {
            for (int i = 0, l = m_Chidren.Count; i < l; ++i)
            {
                if (panelName == m_Chidren[i].name)
                {
                    return m_Chidren[i];
                }
            }
            return null;
        }

        static List<UIPanel> TempPanels = new List<UIPanel>();
        public UIPanel[] GetPanels(string panelName)
        {
            TempPanels.Clear();

            for (int i = 0, l = m_Chidren.Count; i < l; ++i)
            {
                if (panelName == m_Chidren[i].name)
                {
                    TempPanels.Add(m_Chidren[i]);
                }
            }
            return TempPanels.ToArray();
        }
        #endregion

        #region Hierarchy
        public void LoadPanel(string path, System.Action<UIPanel> callback, Transform parent = null)
        {
            //get from asset
            YH.AssetManager.AssetManager.Instance.LoadAsset(path, (assetRef) =>
            {
                if (assetRef != null)
                {
                    UIPanel panel = InstantiatePanel(assetRef.asset as GameObject, parent);
                    panel.path = path;

                    if (panel != null)
                    {
                        assetRef.Monitor(panel.gameObject);

                        if (callback != null)
                        {
                            callback(panel);
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogErrorFormat("Prefab {0} not have UIPanel", path);
                    }
                }
                else
                {
                    Debug.LogErrorFormat("Can't Find Panel {0}", path);
                }

                if (callback != null)
                {
                    callback(null);
                }
            });
        }

        public void LoadPanel(string panelPath, System.Action<UIPanel> callback)
        {
            LoadPanel(panelPath,callback, root);
        }

        protected UIPanel InstantiatePanel(GameObject prefab, Transform parent)
        {
            GameObject panelObj = null;
            if (parent != null)
            {
                panelObj = GameObject.Instantiate<GameObject>(prefab, parent);
            }
            else
            {
                panelObj = GameObject.Instantiate<GameObject>(prefab);
            }

            if (panelObj != null)
            {
                panelObj.name = prefab.name;
                UIPanel panel = panelObj.GetComponent<UIPanel>();
                if (panel != null)
                {
                    return panel;
                }
            }
            return null;
        }
    
        public void ShowPanel(string panelPath, object data=null,int depth=0)
        {
            UIPanel activePanel = null;
            if (m_Actives.TryGetValue(panelPath, out activePanel))
            {
                if (depth > 0 && activePanel.depth < depth)
                {
                    activePanel.depth = depth;
                    RemovePanel(activePanel);
                    AddPanel(activePanel);
                }
                activePanel.Show();
            }
            else
            {
                //load and create new
                LoadPanel(panelPath, (panel) =>
                 {
                     if (panel != null)
                     {
                         panel.Init(data);
                         panel.Show();
                         if (depth > 0)
                         {
                             panel.depth = depth;
                         }
                         AddPanel(panel);
                     }
                 });
            }
        }

        public void ClosePanel(UIPanel panel)
        {
            RemovePanel(panel);
            panel.Close();
        }

        public void ClosePanel(string panelPath)
        {
            UIPanel panel = GetPanel(panelPath);
            if(panel!=null)
            {
                ClosePanel(panel);
            }
        }

        public void HidePanel(UIPanel panel)
        {
            panel.Hide();
        }

        public void HidePanel(string panelPath)
        {
            UIPanel panel = GetPanel(panelPath);
            if (panel != null)
            {
                HidePanel(panel);
            }
        }
        #endregion

        public void ShowDefault()
        {
            if (!string.IsNullOrEmpty(m_DefaultPanel))
            {
                m_Director.Replace(m_DefaultPanel);
            }
        }
        #region GetSet
        public RectTransform root
        {
            get
            {
                return m_Root;
            }
            set
            {
                m_Root = value;
            }
        }

        public IDirector director
        {
            get
            {
                return m_Director;
            }
        }


        #endregion
    }
}