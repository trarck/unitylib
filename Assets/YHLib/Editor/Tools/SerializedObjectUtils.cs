using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YH
{
    public class SerializedObject2Text
    {
        public static void Convert(string filePath, Object obj)
        {
            List<string> props = new List<string>();
            Object2Lines(obj, props);
            File.WriteAllLines(filePath, props.ToArray());
        }

        public static void Object2Lines(Object obj, List<string> propsLines, bool onlyComponet = true)
        {
            Queue<Object> objects = new Queue<Object>();
            HashSet<Object> visitedObjs = new HashSet<Object>();
            objects.Enqueue(obj);
            while (objects.Count > 0)
            {
                obj = objects.Dequeue();
                if (visitedObjs.Contains(obj))
                {
                    continue;
                }
                visitedObjs.Add(obj);

                propsLines.Add(string.Format("{0}[{1}]:", obj.GetType().Name, obj.GetInstanceID()));
                SerializedObject so = new SerializedObject(obj);
                SerializedProperty prop = so.GetIterator();
                bool needEnterChild = true;
                while (prop.Next(needEnterChild))
                {
                    needEnterChild = ParsePropertyValue(prop, propsLines, objects, visitedObjs, "  ", onlyComponet);
                }
            }
        }

        private static bool ParsePropertyValue(SerializedProperty prop, List<string> propsLines, Queue<Object> objects, HashSet<Object> visitedObjs, string prefix = "  ", bool onlyComponet = true)
        {
            string propVal = null;
            bool needEnterChild = true;
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Integer:
                    propVal += prop.intValue;
                    break;
                case SerializedPropertyType.Boolean:
                    propVal += prop.boolValue;
                    break;
                case SerializedPropertyType.Float:
                    propVal += prop.floatValue;
                    break;
                case SerializedPropertyType.String:
                    {
                        propVal += prop.stringValue;
                        needEnterChild = false;
                        break;
                    }
                case SerializedPropertyType.Color:
                    propVal += prop.colorValue;
                    break;
                case SerializedPropertyType.Enum:
                    propVal += prop.enumValueIndex;
                    break;
                case SerializedPropertyType.Vector2:
                    {
                        propVal += prop.vector2Value;
                        needEnterChild = false;
                        break;
                    }
                case SerializedPropertyType.Vector3:
                    {
                        propVal += prop.vector3Value;
                        needEnterChild = false;
                        break;
                    }
                case SerializedPropertyType.Vector4:
                    {
                        propVal += prop.vector4Value;
                        needEnterChild = false;
                        break;
                    }
                case SerializedPropertyType.Rect:
                    {
                        propVal += prop.rectValue;
                        needEnterChild = false;
                        break;
                    }
                case SerializedPropertyType.Bounds:
                    {
                        propVal += prop.boundsValue;
                        needEnterChild = false;
                        break;
                    }

                case SerializedPropertyType.Quaternion:
                    {
                        propVal += prop.quaternionValue;
                        needEnterChild = false;
                        break;
                    }
                case SerializedPropertyType.Vector2Int:
                    {
                        propVal += prop.vector2IntValue;
                        needEnterChild = false;
                        break;
                    }
                case SerializedPropertyType.Vector3Int:
                    {
                        propVal += prop.vector3IntValue;
                        needEnterChild = false;
                        break;
                    }
                case SerializedPropertyType.RectInt:
                    {
                        propVal += prop.rectIntValue;
                        needEnterChild = false;
                        break;
                    }
                case SerializedPropertyType.BoundsInt:
                    {
                        propVal += prop.boundsIntValue;
                        needEnterChild = false;
                        break;
                    }
                case SerializedPropertyType.ArraySize:
                    {
                        propVal += prop.intValue;
                        needEnterChild = false;
                        break;
                    }
                case SerializedPropertyType.ObjectReference:
                    {
                        propVal += GetPPtr(prop);
                        needEnterChild = false;
                        if (prop.objectReferenceValue != null)
                        {
                            if (onlyComponet)
                            {
                                Component comp = prop.objectReferenceValue as Component;
                                if (comp)
                                {
                                    if (!visitedObjs.Contains(comp.gameObject))
                                    {
                                        objects.Enqueue(comp.gameObject);
                                    }
                                    objects.Enqueue(comp);
                                }
                            }
                            else
                            {
                                objects.Enqueue(prop.objectReferenceValue);
                            }

                        }
                        break;
                    }
                default:
                    break;
            }

            if (propVal != null)
            {
                propsLines.Add(string.Format("{0}{1}: {2}", prefix, prop.propertyPath, propVal));
            }
            return needEnterChild;
        }

        public static string GetPPtr(SerializedProperty prop)
        {
            SerializedProperty fileIdProp = prop.FindPropertyRelative("m_FileID");
            int fileId = fileIdProp.intValue;
            SerializedProperty pathIdProp = prop.FindPropertyRelative("m_PathID");
            long pathId = pathIdProp.longValue;
            string s = "{fileID:" + fileId;
            if (pathId > 0)
            {
                s += "pathID:" + pathId;
            }
            s += "}";
            return s;
        }
    }
    public enum ClassIDType
    {
        UnknownType = -1,
        Object = 0,
        GameObject = 1,
        Component = 2,
        LevelGameManager = 3,
        Transform = 4,
        TimeManager = 5,
        GlobalGameManager = 6,
        Behaviour = 8,
        GameManager = 9,
        AudioManager = 11,
        ParticleAnimator = 12,
        InputManager = 13,
        EllipsoidParticleEmitter = 15,
        Pipeline = 17,
        EditorExtension = 18,
        Physics2DSettings = 19,
        Camera = 20,
        Material = 21,
        MeshRenderer = 23,
        Renderer = 25,
        ParticleRenderer = 26,
        Texture = 27,
        Texture2D = 28,
        OcclusionCullingSettings = 29,
        GraphicsSettings = 30,
        MeshFilter = 33,
        OcclusionPortal = 41,
        Mesh = 43,
        Skybox = 45,
        QualitySettings = 47,
        Shader = 48,
        TextAsset = 49,
        Rigidbody2D = 50,
        Physics2DManager = 51,
        Collider2D = 53,
        Rigidbody = 54,
        PhysicsManager = 55,
        Collider = 56,
        Joint = 57,
        CircleCollider2D = 58,
        HingeJoint = 59,
        PolygonCollider2D = 60,
        BoxCollider2D = 61,
        PhysicsMaterial2D = 62,
        MeshCollider = 64,
        BoxCollider = 65,
        CompositeCollider2D = 66,
        EdgeCollider2D = 68,
        CapsuleCollider2D = 70,
        ComputeShader = 72,
        AnimationClip = 74,
        ConstantForce = 75,
        WorldParticleCollider = 76,
        TagManager = 78,
        AudioListener = 81,
        AudioSource = 82,
        AudioClip = 83,
        RenderTexture = 84,
        CustomRenderTexture = 86,
        MeshParticleEmitter = 87,
        ParticleEmitter = 88,
        Cubemap = 89,
        Avatar = 90,
        AnimatorController = 91,
        GUILayer = 92,
        RuntimeAnimatorController = 93,
        ScriptMapper = 94,
        Animator = 95,
        TrailRenderer = 96,
        DelayedCallManager = 98,
        TextMesh = 102,
        RenderSettings = 104,
        Light = 108,
        CGProgram = 109,
        BaseAnimationTrack = 110,
        Animation = 111,
        MonoBehaviour = 114,
        MonoScript = 115,
        MonoManager = 116,
        Texture3D = 117,
        NewAnimationTrack = 118,
        Projector = 119,
        LineRenderer = 120,
        Flare = 121,
        Halo = 122,
        LensFlare = 123,
        FlareLayer = 124,
        HaloLayer = 125,
        NavMeshAreas = 126,
        NavMeshProjectSettings = 126,
        HaloManager = 127,
        Font = 128,
        PlayerSettings = 129,
        NamedObject = 130,
        GUITexture = 131,
        GUIText = 132,
        GUIElement = 133,
        PhysicMaterial = 134,
        SphereCollider = 135,
        CapsuleCollider = 136,
        SkinnedMeshRenderer = 137,
        FixedJoint = 138,
        RaycastCollider = 140,
        BuildSettings = 141,
        AssetBundle = 142,
        CharacterController = 143,
        CharacterJoint = 144,
        SpringJoint = 145,
        WheelCollider = 146,
        ResourceManager = 147,
        NetworkView = 148,
        NetworkManager = 149,
        PreloadData = 150,
        MovieTexture = 152,
        ConfigurableJoint = 153,
        TerrainCollider = 154,
        MasterServerInterface = 155,
        TerrainData = 156,
        LightmapSettings = 157,
        WebCamTexture = 158,
        EditorSettings = 159,
        InteractiveCloth = 160,
        ClothRenderer = 161,
        EditorUserSettings = 162,
        SkinnedCloth = 163,
        AudioReverbFilter = 164,
        AudioHighPassFilter = 165,
        AudioChorusFilter = 166,
        AudioReverbZone = 167,
        AudioEchoFilter = 168,
        AudioLowPassFilter = 169,
        AudioDistortionFilter = 170,
        SparseTexture = 171,
        AudioBehaviour = 180,
        AudioFilter = 181,
        WindZone = 182,
        Cloth = 183,
        SubstanceArchive = 184,
        ProceduralMaterial = 185,
        ProceduralTexture = 186,
        Texture2DArray = 187,
        CubemapArray = 188,
        OffMeshLink = 191,
        OcclusionArea = 192,
        Tree = 193,
        NavMeshObsolete = 194,
        NavMeshAgent = 195,
        NavMeshSettings = 196,
        LightProbesLegacy = 197,
        ParticleSystem = 198,
        ParticleSystemRenderer = 199,
        ShaderVariantCollection = 200,
        LODGroup = 205,
        BlendTree = 206,
        Motion = 207,
        NavMeshObstacle = 208,
        SortingGroup = 210,
        SpriteRenderer = 212,
        Sprite = 213,
        CachedSpriteAtlas = 214,
        ReflectionProbe = 215,
        ReflectionProbes = 216,
        Terrain = 218,
        LightProbeGroup = 220,
        AnimatorOverrideController = 221,
        CanvasRenderer = 222,
        Canvas = 223,
        RectTransform = 224,
        CanvasGroup = 225,
        BillboardAsset = 226,
        BillboardRenderer = 227,
        SpeedTreeWindAsset = 228,
        AnchoredJoint2D = 229,
        Joint2D = 230,
        SpringJoint2D = 231,
        DistanceJoint2D = 232,
        HingeJoint2D = 233,
        SliderJoint2D = 234,
        WheelJoint2D = 235,
        ClusterInputManager = 236,
        BaseVideoTexture = 237,
        NavMeshData = 238,
        AudioMixer = 240,
        AudioMixerController = 241,
        AudioMixerGroupController = 243,
        AudioMixerEffectController = 244,
        AudioMixerSnapshotController = 245,
        PhysicsUpdateBehaviour2D = 246,
        ConstantForce2D = 247,
        Effector2D = 248,
        AreaEffector2D = 249,
        PointEffector2D = 250,
        PlatformEffector2D = 251,
        SurfaceEffector2D = 252,
        BuoyancyEffector2D = 253,
        RelativeJoint2D = 254,
        FixedJoint2D = 255,
        FrictionJoint2D = 256,
        TargetJoint2D = 257,
        LightProbes = 258,
        LightProbeProxyVolume = 259,
        SampleClip = 271,
        AudioMixerSnapshot = 272,
        AudioMixerGroup = 273,
        NScreenBridge = 280,
        AssetBundleManifest = 290,
        UnityAdsManager = 292,
        RuntimeInitializeOnLoadManager = 300,
        CloudWebServicesManager = 301,
        UnityAnalyticsManager = 303,
        CrashReportManager = 304,
        PerformanceReportingManager = 305,
        UnityConnectSettings = 310,
        AvatarMask = 319,
        PlayableDirector = 320,
        VideoPlayer = 328,
        VideoClip = 329,
        ParticleSystemForceField = 330,
        SpriteMask = 331,
        WorldAnchor = 362,
        OcclusionCullingData = 363,
        //kLargestRuntimeClassID = 364
        SmallestEditorClassID = 1000,
        PrefabInstance = 1001,
        EditorExtensionImpl = 1002,
        AssetImporter = 1003,
        AssetDatabaseV1 = 1004,
        Mesh3DSImporter = 1005,
        TextureImporter = 1006,
        ShaderImporter = 1007,
        ComputeShaderImporter = 1008,
        AudioImporter = 1020,
        HierarchyState = 1026,
        GUIDSerializer = 1027,
        AssetMetaData = 1028,
        DefaultAsset = 1029,
        DefaultImporter = 1030,
        TextScriptImporter = 1031,
        SceneAsset = 1032,
        NativeFormatImporter = 1034,
        MonoImporter = 1035,
        AssetServerCache = 1037,
        LibraryAssetImporter = 1038,
        ModelImporter = 1040,
        FBXImporter = 1041,
        TrueTypeFontImporter = 1042,
        MovieImporter = 1044,
        EditorBuildSettings = 1045,
        DDSImporter = 1046,
        InspectorExpandedState = 1048,
        AnnotationManager = 1049,
        PluginImporter = 1050,
        EditorUserBuildSettings = 1051,
        PVRImporter = 1052,
        ASTCImporter = 1053,
        KTXImporter = 1054,
        IHVImageFormatImporter = 1055,
        AnimatorStateTransition = 1101,
        AnimatorState = 1102,
        HumanTemplate = 1105,
        AnimatorStateMachine = 1107,
        PreviewAnimationClip = 1108,
        AnimatorTransition = 1109,
        SpeedTreeImporter = 1110,
        AnimatorTransitionBase = 1111,
        SubstanceImporter = 1112,
        LightmapParameters = 1113,
        LightingDataAsset = 1120,
        GISRaster = 1121,
        GISRasterImporter = 1122,
        CadImporter = 1123,
        SketchUpImporter = 1124,
        BuildReport = 1125,
        PackedAssets = 1126,
        VideoClipImporter = 1127,
        ActivationLogComponent = 2000,
        //kLargestEditorClassID = 2001
        //kClassIdOutOfHierarchy = 100000
        //int = 100000,
        //bool = 100001,
        //float = 100002,
        MonoObject = 100003,
        Collision = 100004,
        Vector3f = 100005,
        RootMotionData = 100006,
        Collision2D = 100007,
        AudioMixerLiveUpdateFloat = 100008,
        AudioMixerLiveUpdateBool = 100009,
        Polygon2D = 100010,
        //void = 100011,
        TilemapCollider2D = 19719996,
        AssetImporterLog = 41386430,
        VFXRenderer = 73398921,
        SerializableManagedRefTestClass = 76251197,
        Grid = 156049354,
        ScenesUsingAssets = 156483287,
        ArticulationBody = 171741748,
        Preset = 181963792,
        EmptyObject = 277625683,
        IConstraint = 285090594,
        TestObjectWithSpecialLayoutOne = 293259124,
        AssemblyDefinitionReferenceImporter = 294290339,
        SiblingDerived = 334799969,
        TestObjectWithSerializedMapStringNonAlignedStruct = 342846651,
        SubDerived = 367388927,
        AssetImportInProgressProxy = 369655926,
        PluginBuildInfo = 382020655,
        EditorProjectAccess = 426301858,
        PrefabImporter = 468431735,
        TestObjectWithSerializedArray = 478637458,
        TestObjectWithSerializedAnimationCurve = 478637459,
        TilemapRenderer = 483693784,
        ScriptableCamera = 488575907,
        SpriteAtlasAsset = 612988286,
        SpriteAtlasDatabase = 638013454,
        AudioBuildInfo = 641289076,
        CachedSpriteAtlasRuntimeData = 644342135,
        RendererFake = 646504946,
        AssemblyDefinitionReferenceAsset = 662584278,
        BuiltAssetBundleInfoSet = 668709126,
        SpriteAtlas = 687078895,
        RayTracingShaderImporter = 747330370,
        RayTracingShader = 825902497,
        LightingSettings = 850595691,
        PlatformModuleSetup = 877146078,
        VersionControlSettings = 890905787,
        AimConstraint = 895512359,
        VFXManager = 937362698,
        VisualEffectSubgraph = 994735392,
        VisualEffectSubgraphOperator = 994735403,
        VisualEffectSubgraphBlock = 994735404,
        LocalizationImporter = 1027052791,
        Derived = 1091556383,
        PropertyModificationsTargetTestObject = 1111377672,
        ReferencesArtifactGenerator = 1114811875,
        AssemblyDefinitionAsset = 1152215463,
        SceneVisibilityState = 1154873562,
        LookAtConstraint = 1183024399,
        SpriteAtlasImporter = 1210832254,
        MultiArtifactTestImporter = 1223240404,
        GameObjectRecorder = 1268269756,
        LightingDataAssetParent = 1325145578,
        PresetManager = 1386491679,
        TestObjectWithSpecialLayoutTwo = 1392443030,
        StreamingManager = 1403656975,
        LowerResBlitTexture = 1480428607,
        StreamingController = 1542919678,
        RenderPassAttachment = 1571458007,
        TestObjectVectorPairStringBool = 1628831178,
        GridLayout = 1742807556,
        AssemblyDefinitionImporter = 1766753193,
        ParentConstraint = 1773428102,
        FakeComponent = 1803986026,
        PositionConstraint = 1818360608,
        RotationConstraint = 1818360609,
        ScaleConstraint = 1818360610,
        Tilemap = 1839735485,
        PackageManifest = 1896753125,
        PackageManifestImporter = 1896753126,
        TerrainLayer = 1953259897,
        SpriteShapeRenderer = 1971053207,
        NativeObjectType = 1977754360,
        TestObjectWithSerializedMapStringBool = 1981279845,
        SerializableManagedHost = 1995898324,
        VisualEffectAsset = 2058629509,
        VisualEffectImporter = 2058629510,
        VisualEffectResource = 2058629511,
        VisualEffectObject = 2059678085,
        VisualEffect = 2083052967,
        LocalizationAsset = 2083778819,
        ScriptedImporter = 2089858483
    }
    public class SerializedObject2Yaml
	{

		public static void Convert(string filePath, Object obj)
		{
			List<string> props = new List<string>();
			Object2Lines(obj, props);
			File.WriteAllLines(filePath, props.ToArray());
		}

		public static void Object2Lines(Object obj, List<string> propsLines, bool onlyComponet = true)
		{
			Queue<Object> objects = new Queue<Object>();
			HashSet<Object> visitedObjs = new HashSet<Object>();

            propsLines.Add("%YAML 1.1");
            propsLines.Add("%TAG !u! tag:unity3d.com,2011:");

            objects.Enqueue(obj);
			while (objects.Count > 0)
			{
				obj = objects.Dequeue();
				if (visitedObjs.Contains(obj))
				{
					continue;
				}
				visitedObjs.Add(obj);

                string typeName = obj.GetType().Name;
                ClassIDType classIdType =(ClassIDType) Enum.Parse(typeof(ClassIDType),typeName);

                propsLines.Add(string.Format("--- !u!{0} &{1}",(int)classIdType, obj.GetInstanceID()));
                propsLines.Add(string.Format("{0}:", typeName));
				SerializedObject so = new SerializedObject(obj);
				SerializedProperty prop = so.GetIterator();
				bool needEnterChild = true;
				while (prop.Next(needEnterChild))
				{
					needEnterChild = ParsePropertyValue(prop,propsLines,objects,visitedObjs, PadDepth(prop.depth+1), true, onlyComponet);
				}
			}
		}

		private static bool ParsePropertyValue(SerializedProperty prop, List<string> propsLines, Queue<Object> objects, HashSet<Object> visitedObjs, string prefix = "  ", bool withName=true,  bool onlyComponet = true)
		{
			string propVal = null;
			bool needEnterChild = true;
			switch (prop.propertyType)
			{
				case SerializedPropertyType.Integer:
					propVal += prop.intValue;
					break;
				case SerializedPropertyType.Boolean:
					propVal += prop.boolValue;
					break;
				case SerializedPropertyType.Float:
					propVal += prop.floatValue;
					break;
				case SerializedPropertyType.String:
					{
						propVal += prop.stringValue;
						needEnterChild = false;
						break;
					}
				case SerializedPropertyType.Color:
					propVal += prop.colorValue;
					break;
				case SerializedPropertyType.Enum:
					propVal += prop.enumValueIndex;
					break;
				case SerializedPropertyType.Vector2:
					{
						propVal += prop.vector2Value;
						needEnterChild = false;
						break;
					}
				case SerializedPropertyType.Vector3:
					{
						propVal += prop.vector3Value;
						needEnterChild = false;
						break;
					}
				case SerializedPropertyType.Vector4:
					{
						propVal += prop.vector4Value;
						needEnterChild = false;
						break;
					}
				case SerializedPropertyType.Rect:
					{
						propVal += prop.rectValue;
						needEnterChild = false;
						break;
					}
				case SerializedPropertyType.Bounds:
					{
						propVal += prop.boundsValue;
						needEnterChild = false;
						break;
					}

				case SerializedPropertyType.Quaternion:
					{
						propVal += prop.quaternionValue;
						needEnterChild = false;
						break;
					}
				case SerializedPropertyType.Vector2Int:
					{
						propVal += prop.vector2IntValue;
						needEnterChild = false;
						break;
					}
				case SerializedPropertyType.Vector3Int:
					{
						propVal += prop.vector3IntValue;
						needEnterChild = false;
						break;
					}
				case SerializedPropertyType.RectInt:
					{
						propVal += prop.rectIntValue;
						needEnterChild = false;
						break;
					}
				case SerializedPropertyType.BoundsInt:
					{
						propVal += prop.boundsIntValue;
						needEnterChild = false;
						break;
					}
				case SerializedPropertyType.ArraySize:
					{
						propVal += prop.intValue;
						needEnterChild = false;
						break;
					}
				case SerializedPropertyType.ObjectReference:
					{
						propVal += GetPPtr(prop);
						needEnterChild = false;
						if (prop.objectReferenceValue != null)
						{
							if (onlyComponet)
							{
								Component comp = prop.objectReferenceValue as Component;
								if (comp)
								{
									if (!visitedObjs.Contains(comp.gameObject))
									{
										objects.Enqueue(comp.gameObject);
									}
									objects.Enqueue(comp);
								}
							}
							else
							{
								objects.Enqueue(prop.objectReferenceValue);
							}

						}
						break;
					}
				case SerializedPropertyType.Generic:
					{
						if (prop.isArray)
						{
							ParseArray(prop, propsLines, objects, visitedObjs, prefix, withName, onlyComponet);
						}
						else
						{
                            ParseGeneric(prop, propsLines, objects, visitedObjs, prefix, withName, onlyComponet);
                        }
                        needEnterChild = false;
                        break;
					}
					
				default:
					break;
			}

			if (propVal != null)
			{
				if (withName)
				{
					propsLines.Add(string.Format("{0}{1}: {2}", prefix, prop.name, propVal));
				}
				else
				{
                    propsLines.Add(string.Format("{0}{1}", prefix, propVal));
                }
			}
			return needEnterChild;
		}


		public static void ParseArray(SerializedProperty arrProp, List<string> propsLines, Queue<Object> objects, HashSet<Object> visitedObjs, string prefix = "  ", bool withName = true, bool onlyComponet = true)
		{
           SerializedProperty arrayProp = arrProp.FindPropertyRelative("Array");
			if (arrayProp.arraySize > 0)
			{
                propsLines.Add(string.Format("{0}{1}: ", prefix, arrProp.propertyPath));
                prefix += "- ";
                //var iter = arrayProp.GetEnumerator();
                //while (iter.MoveNext())
                //{
                //             ParsePropertyValue(iter.Current as SerializedProperty, propsLines, objects, visitedObjs, prefix, onlyComponet);
                //         }

                SerializedProperty end = arrayProp.GetEndProperty(true);
				SerializedProperty prop = arrProp.FindPropertyRelative("Array.data[0]");
				bool enterChild = true;
				while (prop != null && !SerializedProperty.EqualContents(prop, end))
				{
					enterChild = ParsePropertyValue(prop, propsLines, objects, visitedObjs, prefix, false, onlyComponet);
					prop.Next(false);
				}

				//int arraySize = arrayProp.arraySize;

				//bool enterChild = true;
				//for (int i = 0; i < arraySize; i++)
				//{
				//             SerializedProperty dataProp = prop.FindPropertyRelative(string.Format("Array.data[{0}]",i));
				//             //dataProp.Next(enterChild);
				//	enterChild = ParsePropertyValue(dataProp, propsLines, objects, visitedObjs, prefix, onlyComponet);
				//}
			}
			else
			{
                propsLines.Add(string.Format("{0}{1}: []", prefix, arrProp.name));
            }
        }

        public static void ParseGeneric(SerializedProperty genericProp, List<string> propsLines, Queue<Object> objects, HashSet<Object> visitedObjs, string prefix = "  ",  bool withName=true,  bool onlyComponet = true)
		{
			if (withName)
			{
                propsLines.Add(prefix + genericProp.name+":");
                prefix += "  ";
            }

            SerializedProperty end = genericProp.GetEndProperty(true);
            SerializedProperty prop = genericProp.Copy();
			bool enterChild = true;
			//prefix += "  ";
            while (prop.Next(enterChild) && !SerializedProperty.EqualContents(prop, end))
            {
                enterChild=ParsePropertyValue(prop, propsLines, objects, visitedObjs, prefix, true, onlyComponet);
            }
        }

        private static string PadDepth(int depth)
		{
			string pad = "";
			for(int i = 0; i < depth; ++i)
			{
				pad = string.Concat(pad, "  ");
			}
			return pad;
		}

        private static string GetPPtr(SerializedProperty prop)
        {
            SerializedProperty fileIdProp = prop.FindPropertyRelative("m_FileID");
            int fileId = fileIdProp.intValue;
            SerializedProperty pathIdProp = prop.FindPropertyRelative("m_PathID");
            long pathId = pathIdProp.longValue;
            string s = "{fileID:" + fileId;
            if (pathId > 0)
            {
                s += "pathID:" + pathId;
            }
            s += "}";
            return s;
        }
    }
}