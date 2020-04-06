using System;
using System.Collections.Generic;
using UnityEngine;

namespace YHEditor.Timeline
{
    internal class SkinUtil
    {
        class SkinData
        {
            public GUIStyle verticalScrollbarDownButton { get; set; }
            public GUIStyle verticalScrollbarUpButton { get; set; }
            public GUIStyle verticalScrollbarThumb { get; set; }
            public GUIStyle verticalScrollbar { get; set; }
            public GUIStyle horizontalScrollbarRightButton { get; set; }
            public GUIStyle horizontalScrollbarLeftButton { get; set; }
            public GUIStyle horizontalScrollbarThumb { get; set; }
            public GUIStyle horizontalScrollbar { get; set; }
        };

        static Stack<SkinData> m_Skins=new Stack<SkinData>();

        public static void PushHorizontalScrollerSkin(ZoomableArea.Styles styles)
        {
            PushHorizontalScrollerSkin(styles.horizontalScrollbar, styles.horizontalMinMaxScrollbarThumb, styles.horizontalScrollbarLeftButton, styles.horizontalScrollbarRightButton);
        }

        public static void PushHorizontalScrollerSkin(GUIStyle horizontalScrollbar, GUIStyle horizontalScrollbarThumb, GUIStyle horizontalScrollbarLeftButton, GUIStyle horizontalScrollbarRightButton)
        {
            SkinData skin = new SkinData();

            skin.horizontalScrollbar = GUI.skin.horizontalScrollbar;
            skin.horizontalScrollbar = GUI.skin.horizontalScrollbar;
            skin.horizontalScrollbar = GUI.skin.horizontalScrollbar;
            skin.horizontalScrollbar = GUI.skin.horizontalScrollbar;

            GUI.skin.horizontalScrollbar = horizontalScrollbar;
            GUI.skin.horizontalScrollbarThumb = horizontalScrollbarThumb;
            GUI.skin.horizontalScrollbarLeftButton = horizontalScrollbarLeftButton;
            GUI.skin.horizontalScrollbarRightButton = horizontalScrollbarRightButton;

            m_Skins.Push(skin);
        }

        public static void PopHorizontalScrollerSkin()
        {
            if (m_Skins.Count > 0)
            {
                SkinData skin = m_Skins.Pop();

                GUI.skin.horizontalScrollbar = skin.horizontalScrollbar;
                GUI.skin.horizontalScrollbarThumb = skin.horizontalScrollbarThumb;
                GUI.skin.horizontalScrollbarLeftButton = skin.horizontalScrollbarLeftButton;
                GUI.skin.horizontalScrollbarRightButton = skin.horizontalScrollbarRightButton;

            }
        }

        public static void PushVerticalScrollerSkin(ZoomableArea.Styles styles)
        {
            PushVerticalScrollerSkin(styles.verticalScrollbar, styles.verticalMinMaxScrollbarThumb, styles.verticalScrollbarUpButton, styles.verticalScrollbarDownButton);
        }

        public static void PushVerticalScrollerSkin(GUIStyle verticalScrollbar, GUIStyle verticalScrollbarThumb, GUIStyle verticalScrollbarUpButton, GUIStyle verticalScrollbarDownButton)
        {
            SkinData skin = new SkinData();

            skin.verticalScrollbar = GUI.skin.verticalScrollbar;
            skin.verticalScrollbarThumb = GUI.skin.verticalScrollbarThumb;
            skin.verticalScrollbarUpButton = GUI.skin.verticalScrollbarUpButton;
            skin.verticalScrollbarDownButton = GUI.skin.verticalScrollbarDownButton;

            GUI.skin.verticalScrollbar = verticalScrollbar;
            GUI.skin.verticalScrollbarThumb = verticalScrollbarThumb;
            GUI.skin.verticalScrollbarUpButton = verticalScrollbarUpButton;
            GUI.skin.verticalScrollbarDownButton = verticalScrollbarDownButton;

            m_Skins.Push(skin);
        }

        public static void PopVerticalScrollerSkin()
        {
            if (m_Skins.Count > 0)
            {
                SkinData skin = m_Skins.Pop();

                GUI.skin.verticalScrollbar = skin.verticalScrollbar;
                GUI.skin.verticalScrollbarThumb = skin.verticalScrollbarThumb;
                GUI.skin.verticalScrollbarUpButton = skin.verticalScrollbarUpButton;
                GUI.skin.verticalScrollbarDownButton = skin.verticalScrollbarDownButton;

            }
        }
    }
}
