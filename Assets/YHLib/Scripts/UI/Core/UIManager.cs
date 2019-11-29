using System.Collections;
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
        Dictionary<string, PanelInfo> m_PanelInfos = new Dictionary<string, PanelInfo>();

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
                        panel.transform.SetParent(transform,false);
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

        public void LoadPanel(string path, System.Action<UIPanel> callback, Transform parent = null)
        {
            //get from asset
            YH.AssetManager.AssetManager.Instance.LoadAsset(path, (assetRef) =>
            {
                if (assetRef != null)
                {
                    UIPanel panel = InstantiatePanel(assetRef.asset as GameObject, parent);
                    if (panel != null)
                    {
                        panel.path = path;
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
            LoadPanel(panelPath, callback, root);
        }

        protected UIPanel InstantiatePanel(GameObject prefab, Transform parent)
        {
            GameObject panelObj = null;
            panelObj = GameObject.Instantiate<GameObject>(prefab, parent, false);

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
        #endregion

        #region Visible
        public void ShowPanel(string panelPath, object data=null,int depth=0)
        {
            PanelInfo panelInfo =null;
            if (m_PanelInfos.TryGetValue(panelPath, out panelInfo))
            {
                if (panelInfo.IsLoaded)
                {
                    if (depth > 0 && panelInfo.panel.depth < depth)
                    {
                        panelInfo.panel.depth = depth;
                        RemovePanel(panelInfo.panel);
                        AddPanel(panelInfo.panel);
                    }
                    panelInfo.panel.Show();
                    return;
                }
                else if (panelInfo.IsLoading)
                {
                    panelInfo.visible = true;
                    return;
                }
            }

            //load and create new
            panelInfo = new PanelInfo(panelPath,  data);
            panelInfo.state = PanelInfo.State.Loading;
            LoadPanel(panelPath, (panel) =>
            {
                Debug.LogFormat("{0} loaded", panelPath);
                panelInfo.state = PanelInfo.State.Loaded;
                if (panel != null)
                {
                    panelInfo.panel = panel;
                         
                    if (panelInfo.closble)
                    {
                        ClosePanel(panel);
                    }
                    else
                    {
                        panel.Init(data);
                        if (depth > 0)
                        {
                            panel.depth = depth;
                        }
                        AddPanel(panel);
                        if (panelInfo.visible)
                        {
                            panel.Show();
                        }
                        else
                        {
                            HidePanel(panel);
                        }
                    }
                }
            });
            m_PanelInfos[panelPath] = panelInfo;
        }

        public void ClosePanel(UIPanel panel)
        {
            if (panel)
            {
                if (m_PanelInfos.ContainsKey(panel.path))
                {
                    m_PanelInfos.Remove(panel.path);
                }
                RemovePanel(panel);
                panel.Close();            
            }
        }

        public void ClosePanel(string panelPath)
        {
            PanelInfo panelInfo = null;
            if (m_PanelInfos.TryGetValue(panelPath, out panelInfo))
            {
                ClosePanel(panelInfo);
            }
        }
        
        void ClosePanel(PanelInfo panelInfo)
        {
            if (panelInfo.IsLoaded)
            {
                if (panelInfo.panel)
                {
                    ClosePanel(panelInfo.panel);
                }
            }
            else
            {
                panelInfo.closble = true;
            }
        }

        public void HidePanel(UIPanel panel)
        {
            if (panel)
            {
                panel.Hide();
            }
        }

        public void HidePanel(string panelPath)
        {
            PanelInfo panelInfo = null;
            if (m_PanelInfos.TryGetValue(panelPath, out panelInfo))
            {
                HidePanel(panelInfo);
            }
        }

        void HidePanel(PanelInfo panelStack)
        {
            if (panelStack.IsLoaded)
            {
                if (panelStack.panel)
                {
                    panelStack.panel.Hide();
                }
            }
            else
            {
                panelStack.visible = false;
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