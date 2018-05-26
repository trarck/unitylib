using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace YH
{
    /// <summary>
    /// Tools for the editor
    /// </summary>
    public class YHEditorTools
    {
        static Texture2D mWhiteTex;
        static Texture2D mBackdropTex;
        static Texture2D mContrastTex;
        static Texture2D mGradientTex;
        static GameObject mPrevious;

        static Stack<float> mLabelWidths;
        static Stack<float> mFieldWidths;
        static Stack<int> mIndentLevels;
        /// <summary>
        /// Returns a blank usable 1x1 white texture.
        /// </summary>

        static public Texture2D blankTexture
        {
            get
            {
                return EditorGUIUtility.whiteTexture;
            }
        }

        /// <summary>
        /// Returns a usable texture that looks like a dark checker board.
        /// </summary>

        static public Texture2D backdropTexture
        {
            get
            {
                if (mBackdropTex == null)
                    mBackdropTex = CreateCheckerTex(
new Color(0.1f, 0.1f, 0.1f, 0.5f),
new Color(0.2f, 0.2f, 0.2f, 0.5f));
                return mBackdropTex;
            }
        }

        /// <summary>
        /// Returns a usable texture that looks like a high-contrast checker board.
        /// </summary>

        static public Texture2D contrastTexture
        {
            get
            {
                if (mContrastTex == null)
                    mContrastTex = CreateCheckerTex(
new Color(0f, 0.0f, 0f, 0.5f),
new Color(1f, 1f, 1f, 0.5f));
                return mContrastTex;
            }
        }

        /// <summary>
        /// Gradient texture is used for title bars / headers.
        /// </summary>

        static public Texture2D gradientTexture
        {
            get
            {
                if (mGradientTex == null) mGradientTex = CreateGradientTex();
                return mGradientTex;
            }
        }

        /// <summary>
        /// Create a white dummy texture.
        /// </summary>

        static Texture2D CreateDummyTex()
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.name = "[Generated] Dummy Texture";
            tex.hideFlags = HideFlags.DontSave;
            tex.filterMode = FilterMode.Point;
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Create a checker-background texture
        /// </summary>

        static Texture2D CreateCheckerTex(Color c0, Color c1)
        {
            Texture2D tex = new Texture2D(16, 16);
            tex.name = "[Generated] Checker Texture";
            tex.hideFlags = HideFlags.DontSave;

            for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c1);
            for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c0);
            for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c0);
            for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c1);

            tex.Apply();
            tex.filterMode = FilterMode.Point;
            return tex;
        }

        /// <summary>
        /// Create a gradient texture
        /// </summary>

        static Texture2D CreateGradientTex()
        {
            Texture2D tex = new Texture2D(1, 16);
            tex.name = "[Generated] Gradient Texture";
            tex.hideFlags = HideFlags.DontSave;

            Color c0 = new Color(1f, 1f, 1f, 0f);
            Color c1 = new Color(1f, 1f, 1f, 0.4f);

            for (int i = 0; i < 16; ++i)
            {
                float f = Mathf.Abs((i / 15f) * 2f - 1f);
                f *= f;
                tex.SetPixel(0, i, Color.Lerp(c0, c1, f));
            }

            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            return tex;
        }

        /// <summary>
        /// Draws the tiled texture. Like GUI.DrawTexture() but tiled instead of stretched.
        /// </summary>

        static public void DrawTiledTexture(Rect rect, Texture tex)
        {
            GUI.BeginGroup(rect);
            {
                int width = Mathf.RoundToInt(rect.width);
                int height = Mathf.RoundToInt(rect.height);

                for (int y = 0; y < height; y += tex.height)
                {
                    for (int x = 0; x < width; x += tex.width)
                    {
                        GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
                    }
                }
            }
            GUI.EndGroup();
        }

        /// <summary>
        /// Draw a single-pixel outline around the specified rectangle.
        /// </summary>

        static public void DrawOutline(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Texture2D tex = contrastTexture;
                GUI.color = Color.white;
                DrawTiledTexture(new Rect(rect.xMin, rect.yMax, 1f, -rect.height), tex);
                DrawTiledTexture(new Rect(rect.xMax, rect.yMax, 1f, -rect.height), tex);
                DrawTiledTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
                DrawTiledTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
            }
        }

        /// <summary>
        /// Draw a single-pixel outline around the specified rectangle.
        /// </summary>

        static public void DrawOutline(Rect rect, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Texture2D tex = blankTexture;
                GUI.color = color;
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), tex);
                GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, 1f, rect.height), tex);
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
                GUI.color = Color.white;
            }
        }

        /// <summary>
        /// Draw a selection outline around the specified rectangle.
        /// </summary>

        static public void DrawOutline(Rect rect, Rect relative, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                // Calculate where the outer rectangle would be
                float x = rect.xMin + rect.width * relative.xMin;
                float y = rect.yMax - rect.height * relative.yMin;
                float width = rect.width * relative.width;
                float height = -rect.height * relative.height;
                relative = new Rect(x, y, width, height);

                // Draw the selection
                DrawOutline(relative, color);
            }
        }

        /// <summary>
        /// Draw a selection outline around the specified rectangle.
        /// </summary>

        static public void DrawOutline(Rect rect, Rect relative)
        {
            if (Event.current.type == EventType.Repaint)
            {
                // Calculate where the outer rectangle would be
                float x = rect.xMin + rect.width * relative.xMin;
                float y = rect.yMax - rect.height * relative.yMin;
                float width = rect.width * relative.width;
                float height = -rect.height * relative.height;
                relative = new Rect(x, y, width, height);

                // Draw the selection
                DrawOutline(relative);
            }
        }

        /// <summary>
        /// Draw a 9-sliced outline.
        /// </summary>

        static public void DrawOutline(Rect rect, Rect outer, Rect inner)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Color green = new Color(0.4f, 1f, 0f, 1f);

                DrawOutline(rect, new Rect(outer.x, inner.y, outer.width, inner.height));
                DrawOutline(rect, new Rect(inner.x, outer.y, inner.width, outer.height));
                DrawOutline(rect, outer, green);
            }
        }

        /// <summary>
        /// Draw a checkered background for the specified texture.
        /// </summary>

        static public Rect DrawBackground(Texture2D tex, float ratio)
        {
            Rect rect = GUILayoutUtility.GetRect(0f, 0f);
            rect.width = Screen.width - rect.xMin;
            rect.height = rect.width * ratio;
            GUILayout.Space(rect.height);

            if (Event.current.type == EventType.Repaint)
            {
                Texture2D blank = blankTexture;
                Texture2D check = backdropTexture;

                // Lines above and below the texture rectangle
                GUI.color = new Color(0f, 0f, 0f, 0.2f);
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMin - 1, rect.width, 1f), blank);
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), blank);
                GUI.color = Color.white;

                // Checker background
                DrawTiledTexture(rect, check);
            }
            return rect;
        }

        /// <summary>
        /// Draw a visible separator in addition to adding some padding.
        /// </summary>

        static public void DrawSeparator()
        {
            GUILayout.Space(12f);

            if (Event.current.type == EventType.Repaint)
            {
                Texture2D tex = blankTexture;
                Rect rect = GUILayoutUtility.GetLastRect();
                GUI.color = new Color(0f, 0f, 0f, 0.25f);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
                GUI.color = Color.white;
            }
        }

        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>

        //static public Rect DrawHeader (string text)
        //{
        //    GUILayout.Space(28f);
        //    Rect rect = GUILayoutUtility.GetLastRect();
        //    rect.yMin += 5f;
        //    rect.yMax -= 4f;
        //    rect.width = Screen.width;

        //    if (Event.current.type == EventType.Repaint)
        //    {
        //        GUI.color = Color.black;
        //        GUI.DrawTexture(new Rect(0f, rect.yMin, Screen.width, rect.yMax - rect.yMin), gradientTexture);
        //        GUI.color = new Color(0f, 0f, 0f, 0.25f);
        //        GUI.DrawTexture(new Rect(0f, rect.yMin, Screen.width, 1f), blankTexture);
        //        GUI.DrawTexture(new Rect(0f, rect.yMax - 1, Screen.width, 1f), blankTexture);
        //        GUI.color = Color.white;
        //        GUI.Label(new Rect(rect.x + 4f, rect.y, rect.width - 4, rect.height), text, EditorStyles.boldLabel);
        //    }
        //    return rect;
        //}

        /// <summary>
        /// Convenience function that displays a list of sprites and returns the selected value.
        /// </summary>

        static public string DrawList(string field, string[] list, string selection, params GUILayoutOption[] options)
        {
            if (list != null && list.Length > 0)
            {
                int index = 0;
                if (string.IsNullOrEmpty(selection)) selection = list[0];

                // We need to find the sprite in order to have it selected
                if (!string.IsNullOrEmpty(selection))
                {
                    for (int i = 0; i < list.Length; ++i)
                    {
                        if (selection.Equals(list[i], System.StringComparison.OrdinalIgnoreCase))
                        {
                            index = i;
                            break;
                        }
                    }
                }

                // Draw the sprite selection popup
                index = string.IsNullOrEmpty(field) ?
                    EditorGUILayout.Popup(index, list, options) :
                    EditorGUILayout.Popup(field, index, list, options);

                return list[index];
            }
            return null;
        }

        /// <summary>
        /// Convenience function that displays a list of sprites and returns the selected value.
        /// </summary>

        static public string DrawAdvancedList(string field, string[] list, string selection, params GUILayoutOption[] options)
        {
            if (list != null && list.Length > 0)
            {
                int index = 0;
                if (string.IsNullOrEmpty(selection)) selection = list[0];

                // We need to find the sprite in order to have it selected
                if (!string.IsNullOrEmpty(selection))
                {
                    for (int i = 0; i < list.Length; ++i)
                    {
                        if (selection.Equals(list[i], System.StringComparison.OrdinalIgnoreCase))
                        {
                            index = i;
                            break;
                        }
                    }
                }

                // Draw the sprite selection popup
                index = string.IsNullOrEmpty(field) ?
                    EditorGUILayout.Popup(index, list, "DropDownButton", options) :
                    EditorGUILayout.Popup(field, index, list, "DropDownButton", options);

                return list[index];
            }
            return null;
        }

        static public void LookLikeControls(float labelWidth = 0, float fieldWidth = 0)
        {
            EditorGUIUtility.fieldWidth = fieldWidth;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        /// <summary>
        /// Helper function that checks to see if this action would break the prefab connection.
        /// </summary>

        static public bool WillLosePrefab(GameObject root)
        {
            if (root == null) return false;

            if (root.transform != null)
            {
                // Check if the selected object is a prefab instance and display a warning
                PrefabType type = PrefabUtility.GetPrefabType(root);

                if (type == PrefabType.PrefabInstance)
                {
                    return EditorUtility.DisplayDialog("Losing prefab",
                        "This action will lose the prefab connection. Are you sure you wish to continue?",
                        "Continue", "Cancel");
                }
            }
            return true;
        }

        /// <summary>
        /// Change the import settings of the specified texture asset, making it readable.
        /// </summary>

        static bool MakeTextureReadable(string path, bool force)
        {
            if (string.IsNullOrEmpty(path)) return false;
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) return false;

            TextureImporterSettings settings = new TextureImporterSettings();
            ti.ReadTextureSettings(settings);

            if (force ||
                settings.mipmapEnabled ||
                !settings.readable ||
                settings.maxTextureSize < 4096 ||
                settings.filterMode != FilterMode.Point ||
                settings.wrapMode != TextureWrapMode.Clamp ||
                settings.npotScale != TextureImporterNPOTScale.None)
            {
                settings.mipmapEnabled = false;
                settings.readable = true;
                settings.maxTextureSize = 4096;
                settings.textureFormat = TextureImporterFormat.ARGB32;
                settings.filterMode = FilterMode.Point;
                settings.wrapMode = TextureWrapMode.Clamp;
                settings.npotScale = TextureImporterNPOTScale.None;

                ti.SetTextureSettings(settings);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            }
            return true;
        }

        /// <summary>
        /// Change the import settings of the specified texture asset, making it suitable to be used as a texture atlas.
        /// </summary>

        static bool MakeTextureAnAtlas(string path, bool force)
        {
            if (string.IsNullOrEmpty(path)) return false;
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) return false;

            TextureImporterSettings settings = new TextureImporterSettings();
            ti.ReadTextureSettings(settings);

            if (force ||
                settings.readable ||
                settings.maxTextureSize < 4096 ||
                settings.wrapMode != TextureWrapMode.Clamp ||
                settings.npotScale != TextureImporterNPOTScale.ToNearest)
            {
                //settings.mipmapEnabled = true;
                settings.readable = false;
                settings.maxTextureSize = 4096;
                settings.textureFormat = TextureImporterFormat.RGBA32;
                settings.filterMode = FilterMode.Trilinear;
                settings.aniso = 4;
                settings.wrapMode = TextureWrapMode.Clamp;
                settings.npotScale = TextureImporterNPOTScale.ToNearest;

                ti.SetTextureSettings(settings);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            }
            return true;
        }

        /// <summary>
        /// Fix the import settings for the specified texture, re-importing it if necessary.
        /// </summary>

        static public Texture2D ImportTexture(string path, bool forInput, bool force)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (forInput) { if (!MakeTextureReadable(path, force)) return null; }
                else if (!MakeTextureAnAtlas(path, force)) return null;
                //return AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;

                Texture2D tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                return tex;
            }
            return null;
        }

        /// <summary>
        /// Fix the import settings for the specified texture, re-importing it if necessary.
        /// </summary>

        static public Texture2D ImportTexture(Texture tex, bool forInput, bool force)
        {
            if (tex != null)
            {
                string path = AssetDatabase.GetAssetPath(tex.GetInstanceID());
                return ImportTexture(path, forInput, force);
            }
            return null;
        }



        /// <summary>
        /// Helper function that returns the folder where the current selection resides.
        /// </summary>

        static public string GetSelectionFolder()
        {
            if (Selection.activeObject != null)
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

                if (!string.IsNullOrEmpty(path))
                {
                    int dot = path.LastIndexOf('.');
                    int slash = Mathf.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
                    if (slash > 0) return (dot > slash) ? path.Substring(0, slash + 1) : path + "/";
                }
            }
            return "Assets/";
        }

        /// <summary>
        /// Struct type for the integer vector field below.
        /// </summary>

        public struct IntVector
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// Integer vector field.
        /// </summary>

        static public IntVector IntPair(string prefix, string leftCaption, string rightCaption, int x, int y)
        {
            GUILayout.BeginHorizontal();

            if (string.IsNullOrEmpty(prefix))
            {
                GUILayout.Space(82f);
            }
            else
            {
                GUILayout.Label(prefix, GUILayout.Width(74f));
            }

            LookLikeControls(48f);

            IntVector retVal;
            retVal.x = EditorGUILayout.IntField(leftCaption, x, GUILayout.MinWidth(30f));
            retVal.y = EditorGUILayout.IntField(rightCaption, y, GUILayout.MinWidth(30f));

            LookLikeControls(80f);

            GUILayout.EndHorizontal();
            return retVal;
        }

        /// <summary>
        /// Integer rectangle field.
        /// </summary>

        static public Rect IntRect(string prefix, Rect rect)
        {
            int left = Mathf.RoundToInt(rect.xMin);
            int top = Mathf.RoundToInt(rect.yMin);
            int width = Mathf.RoundToInt(rect.width);
            int height = Mathf.RoundToInt(rect.height);

            IntVector a = IntPair(prefix, "Left", "Top", left, top);
            IntVector b = IntPair(null, "Width", "Height", width, height);

            return new Rect(a.x, a.y, b.x, b.y);
        }

        /// <summary>
        /// Integer vector field.
        /// </summary>

        static public Vector4 IntPadding(string prefix, Vector4 v)
        {
            int left = Mathf.RoundToInt(v.x);
            int top = Mathf.RoundToInt(v.y);
            int right = Mathf.RoundToInt(v.z);
            int bottom = Mathf.RoundToInt(v.w);

            IntVector a = IntPair(prefix, "Left", "Top", left, top);
            IntVector b = IntPair(null, "Right", "Bottom", right, bottom);

            return new Vector4(a.x, a.y, b.x, b.y);
        }

        /// <summary>
        /// Create an undo point for the specified objects.
        /// </summary>

        static public void RegisterUndo(string name, params Object[] objects)
        {
            if (objects != null && objects.Length > 0)
            {
                foreach (Object obj in objects)
                {
                    if (obj == null) continue;
                    Undo.RecordObject(obj, name);
                    EditorUtility.SetDirty(obj);
                }
            }
            //		else
            //		{
            //			Undo.RegisterSceneUndo(name);
            //		}
        }

        /// <summary>
        /// Find all scene components, active or inactive.
        /// </summary>

        static public List<T> FindInScene<T>() where T : Component
        {
            T[] comps = Resources.FindObjectsOfTypeAll(typeof(T)) as T[];

            List<T> list = new List<T>();

            foreach (T comp in comps)
            {
                if (comp.gameObject.hideFlags == 0)
                {
                    string path = AssetDatabase.GetAssetPath(comp.gameObject);
                    if (string.IsNullOrEmpty(path)) list.Add(comp);
                }
            }
            return list;
        }

        /// <summary>
        /// Draw the specified sprite.
        /// </summary>

        public static void DrawSprite(Texture2D tex, Rect rect, Rect outer, Rect inner, Rect uv, Color color)
        {
            DrawSprite(tex, rect, outer, inner, uv, color, null);
        }

        /// <summary>
        /// Draw the specified sprite.
        /// </summary>

        public static void DrawSprite(Texture2D tex, Rect rect, Rect outer, Rect inner, Rect uv, Color color, Material mat)
        {
            // Create the texture rectangle that is centered inside rect.
            Rect outerRect = rect;
            outerRect.width = outer.width;
            outerRect.height = outer.height;

            if (outerRect.width > 0f)
            {
                float f = rect.width / outerRect.width;
                outerRect.width *= f;
                outerRect.height *= f;
            }

            if (rect.height > outerRect.height)
            {
                outerRect.y += (rect.height - outerRect.height) * 0.5f;
            }
            else if (outerRect.height > rect.height)
            {
                float f = rect.height / outerRect.height;
                outerRect.width *= f;
                outerRect.height *= f;
            }

            if (rect.width > outerRect.width) outerRect.x += (rect.width - outerRect.width) * 0.5f;

            // Draw the background
            DrawTiledTexture(outerRect, YHEditorTools.backdropTexture);

            // Draw the sprite
            GUI.color = color;

            if (mat == null)
            {
                GUI.DrawTextureWithTexCoords(outerRect, tex, uv, true);
            }
            else
            {
                // NOTE: There is an issue in Unity that prevents it from clipping the drawn preview
                // using BeginGroup/EndGroup, and there is no way to specify a UV rect... le'suq.
                UnityEditor.EditorGUI.DrawPreviewTexture(outerRect, tex, mat);
            }

            // Draw the border indicator lines
            GUI.BeginGroup(outerRect);
            {
                tex = YHEditorTools.contrastTexture;
                GUI.color = Color.white;

                if (inner.xMin != outer.xMin)
                {
                    float x0 = (inner.xMin - outer.xMin) / outer.width * outerRect.width - 1;
                    YHEditorTools.DrawTiledTexture(new Rect(x0, 0f, 1f, outerRect.height), tex);
                }

                if (inner.xMax != outer.xMax)
                {
                    float x1 = (inner.xMax - outer.xMin) / outer.width * outerRect.width - 1;
                    YHEditorTools.DrawTiledTexture(new Rect(x1, 0f, 1f, outerRect.height), tex);
                }

                if (inner.yMin != outer.yMin)
                {
                    float y0 = (inner.yMin - outer.yMin) / outer.height * outerRect.height - 1;
                    YHEditorTools.DrawTiledTexture(new Rect(0f, y0, outerRect.width, 1f), tex);
                }

                if (inner.yMax != outer.yMax)
                {
                    float y1 = (inner.yMax - outer.yMin) / outer.height * outerRect.height - 1;
                    YHEditorTools.DrawTiledTexture(new Rect(0f, y1, outerRect.width, 1f), tex);
                }
            }
            GUI.EndGroup();

            // Draw the lines around the sprite
            Handles.color = Color.black;
            Handles.DrawLine(new Vector3(outerRect.xMin, outerRect.yMin), new Vector3(outerRect.xMin, outerRect.yMax));
            Handles.DrawLine(new Vector3(outerRect.xMax, outerRect.yMin), new Vector3(outerRect.xMax, outerRect.yMax));
            Handles.DrawLine(new Vector3(outerRect.xMin, outerRect.yMin), new Vector3(outerRect.xMax, outerRect.yMin));
            Handles.DrawLine(new Vector3(outerRect.xMin, outerRect.yMax), new Vector3(outerRect.xMax, outerRect.yMax));

            // Sprite size label
            string text = string.Format("Sprite Size: {0}x{1}",
                Mathf.RoundToInt(Mathf.Abs(outer.width)),
                Mathf.RoundToInt(Mathf.Abs(outer.height)));
            EditorGUI.DropShadowLabel(GUILayoutUtility.GetRect(Screen.width, 18f), text);
        }


        /// <summary>
        /// Select the specified game object and remember what was selected before.
        /// </summary>

        static public void Select(GameObject go)
        {
            mPrevious = Selection.activeGameObject;
            Selection.activeGameObject = go;
        }

        /// <summary>
        /// Select the previous game object.
        /// </summary>

        static public void SelectPrevious()
        {
            if (mPrevious != null)
            {
                Selection.activeGameObject = mPrevious;
                mPrevious = null;
            }
        }

        /// <summary>
        /// Previously selected game object.
        /// </summary>

        static public GameObject previousSelection { get { return mPrevious; } }

        /// <summary>
        /// Helper function that checks to see if the scale is uniform.
        /// </summary>

        static public bool IsUniform(Vector3 scale)
        {
            return Mathf.Approximately(scale.x, scale.y) && Mathf.Approximately(scale.x, scale.z);
        }

        /// <summary>
        /// Check to see if the specified game object has a uniform scale.
        /// </summary>

        static public bool IsUniform(GameObject go)
        {
            if (go == null) return true;

            return IsUniform(go.transform.lossyScale);
        }

        /// <summary>
        /// Fix uniform scaling of the specified object.
        /// </summary>

        static public void FixUniform(GameObject go)
        {
            //Transform t = go.transform;

            //while (t != null && t.gameObject.GetComponent<UIRoot>() == null)
            //{
            //    if (!YHEditorTools.IsUniform(t.localScale))
            //    {
            //        Undo.RegisterUndo(t, "Uniform scaling fix");
            //        t.localScale = Vector3.one;
            //        EditorUtility.SetDirty(t);
            //    }
            //    t = t.parent;
            //}
        }

        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>

        static public bool DrawHeader(string text, bool forceOn = false) { return DrawHeader(text, text, forceOn); }

        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>

        static public bool DrawHeader(string text, string key, bool forceOn = false)
        {
            bool state = EditorPrefs.GetBool(key, true);

            GUILayout.Space(3f);
            if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(3f);

            GUI.changed = false;
#if UNITY_3_5
		if (!GUILayout.Toggle(true, text, "dragtab")) state = !state;
#else
            if (!GUILayout.Toggle(true, "<b><size=11>" + text + "</size></b>", "dragtab")) state = !state;
#endif
            if (GUI.changed) EditorPrefs.SetBool(key, state);

            GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!forceOn && !state) GUILayout.Space(3f);
            return state;
        }

        /// <summary>
        /// Begin drawing the content area.
        /// </summary>

        static public void BeginContents()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }

        /// <summary>
        /// End drawing the content area.
        /// </summary>

        static public void EndContents()
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }

        static public void PushLabelWidth(float width)
        {
            if (mLabelWidths == null)
            {
                mLabelWidths = new Stack<float>();
                //Root
                mLabelWidths.Push(EditorGUIUtility.labelWidth);
            }
            mLabelWidths.Push(width);
            EditorGUIUtility.labelWidth = width;
        }

        static public void PopLabelWidth()
        {
            if (mLabelWidths!=null && mLabelWidths.Count > 1)
            {
                mLabelWidths.Pop();
                EditorGUIUtility.labelWidth = mLabelWidths.Peek();
            }
        }

        static public void PushFieldWidth(float width)
        {
            if (mFieldWidths == null)
            {
                mFieldWidths = new Stack<float>();
                //Root
                mFieldWidths.Push(EditorGUIUtility.fieldWidth);
            }
            mFieldWidths.Push(width);
            EditorGUIUtility.fieldWidth = width;
        }

        static public void PopFieldWidth()
        {
            if (mFieldWidths!=null && mFieldWidths.Count > 1)
            {
                mFieldWidths.Pop();
                EditorGUIUtility.fieldWidth = mFieldWidths.Peek();
            }
        }

        static public void PushLabelFieldWidth(float labelWidth, float fieldWidth)
        {
            if (mLabelWidths == null)
            {
                mLabelWidths = new Stack<float>();
                //Root
                mLabelWidths.Push(EditorGUIUtility.labelWidth);
            }

            if (mFieldWidths == null)
            {
                mFieldWidths = new Stack<float>();
                //Root
                mFieldWidths.Push(EditorGUIUtility.fieldWidth);
            }
            mLabelWidths.Push(labelWidth);
            mFieldWidths.Push(fieldWidth);
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUIUtility.fieldWidth = fieldWidth;
        }

        static public void PopLabelFieldWidth()
        {
            if (mLabelWidths!=null && mLabelWidths.Count > 1)
            {
                mLabelWidths.Pop();
                EditorGUIUtility.labelWidth = mLabelWidths.Peek();
            }

            if (mFieldWidths!=null && mFieldWidths.Count > 1)
            {
                mFieldWidths.Pop();
                EditorGUIUtility.fieldWidth = mFieldWidths.Peek();
            }
        }

        public static void PushIndentLevel(int level)
        {
            if (mIndentLevels == null)
            {
                mIndentLevels = new Stack<int>();

                mIndentLevels.Push(EditorGUI.indentLevel);
            }

            mIndentLevels.Push(level);
            EditorGUI.indentLevel = level;
        }


        public static void PopIndentLevel()
        {
            if (mIndentLevels != null && mIndentLevels.Count > 1)
            {
                mIndentLevels.Pop();

                EditorGUI.indentLevel = mIndentLevels.Peek();
            }
        }
    }
}