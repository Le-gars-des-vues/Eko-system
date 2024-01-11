using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;
using TMPro;
#if (UNITY_EDITOR)
using UnityEditor;
#endif

namespace TerraTiler2D
{
	///<summary>A globally accessible data container. Contains generic variables and methods that are used all throughout TerraTiler2D.</summary>
	public class Glob : Singleton<Glob>
	{
		protected override void Initialize()
		{
			base.Initialize();

#if (UNITY_EDITOR)
			if (EditorPrefs.HasKey("DebugLevel"))
			{
				SetDebugLevelNonStatic((DebugLevel)EditorPrefs.GetInt("DebugLevel"));
			}
			else
			{
				SetDebugLevelNonStatic(DebugLevel.User);
			}

			if (EditorPrefs.HasKey("GiveTransparentTilesAPreviewColor"))
			{
				SetGiveTransparentTilesAPreviewColorNonStatic(EditorPrefs.GetBool("GiveTransparentTilesAPreviewColor"));
			}
			else
			{
				SetGiveTransparentTilesAPreviewColorNonStatic(true);
			}

			if (EditorPrefs.HasKey("PauseBetweenNodes"))
			{
				SetPauseBetweenNodesNonStatic(EditorPrefs.GetBool("PauseBetweenNodes"));
			}
			else
			{
				SetPauseBetweenNodesNonStatic(true);
			}
#endif
		}


		//==================== Tool Version ====================

		//1.    :   Should increment when TerraTiler2D receives a complete rework, which is hopefully never.
		private int TerraTiler2DVersionIndex = 1;
		//.1    :   Should increment when TerraTiler2D receives a content update.
		private int TerraTiler2DContentUpdateIndex = 1;
		//.001  :   Should increment when TerraTiler2D receives bugfixes following a content update. Resets back to 0 whenever the content update index increments.
		private int TerraTiler2DBugfixUpdateIndex = 0;
		//.00001:	Always stays the same. Only serves to prevent conflicts between BugFixIndex 1 and 10 etc., which would otherwise be represented the same since it would add a 0 to the end of a float.
		private int TerraTiler2DClosingIndex = 1;

		public float ToolVersion
		{
			get
			{
				//Multiply each index with a preset number so that they are placed on their default positions.
				float toolVersion = TerraTiler2DVersionIndex;
				float contentUpdateIndex = TerraTiler2DContentUpdateIndex * 0.1f;
				float bugfixUpdateIndex = TerraTiler2DBugfixUpdateIndex * 0.001f;
				float closingIndex = TerraTiler2DClosingIndex * 0.00001f;

				//If the ContentUpdateIndex has more than 1 digit
				for (int i = 1; i < TerraTiler2DContentUpdateIndex.ToString().Length; i++)
				{
					//For every extra digit, move the required indices 1 place to the right so that they don't overlap.
					contentUpdateIndex *= 0.1f;
					bugfixUpdateIndex *= 0.1f;
					closingIndex *= 0.1f;
				}
				//If the BugFixUpdateIndex has more than 1 digit
				for (int i = 1; i < TerraTiler2DBugfixUpdateIndex.ToString().Length; i++)
				{
					//For every extra digit, move the required indices 1 place to the right so that they don't overlap.
					bugfixUpdateIndex *= 0.1f;
					closingIndex *= 0.1f;
				}

				//Add all of the indices together to get the final tool version.
				toolVersion += contentUpdateIndex + bugfixUpdateIndex + closingIndex;

				return toolVersion;
			}
		}

		public string GetToolVersionAsString(float version)
		{
			//Convert the float to a string, and get the char array
			string versionString = version.ToString();
			char[] versionCharacters = versionString.ToCharArray();

			//If the version has the default length
            if (versionCharacters.Length == 7)
            {
				versionCharacters[3] = '.';
				return versionCharacters.ArrayToString().Remove(5,2);
            }
            else if (version <= 1)
            {
				return "1.0.0";
            }

			//Prepare the return string
			string output = "";

			//Toggles when to insert a section into the return string
			bool shouldAddSection = false;
			//Holds all the characters that should be added to the return string the next time a section should be added
			string sectionToAppend = "";

			//Iterate over each character in the version, starting from the end. Exclude the last 2 characters, since they will always be '01', and they are already being replaced with '.1f'
            for (int i = versionCharacters.Length-3; i >= 0; i--)
            {
				//If this character is a '0' or a '.'
                if (versionCharacters[i] == '0' || versionCharacters[i] == '.')
                {
					//If another section should be added to the return string
                    if (shouldAddSection)
                    {
						//Add a section to the return string
						output.Insert(0, "." + sectionToAppend);
						//Reset the section
						sectionToAppend = "";
						//Start reading the next section
						shouldAddSection = false;
					}
					else
                    {
						//Add this character to the section
						sectionToAppend.Insert(0, versionCharacters[i].ToString());
                    }
                }
				//If this is any other character
				else
                {
					//Add this character to the section
					sectionToAppend.Insert(0, versionCharacters[i].ToString());
					//Tell the loop to add a section to the return string the next time a '0' or a '.' is found.
					shouldAddSection = true;
				}
            }
			//Add the final section to the return string
			output.Insert(0, sectionToAppend);

			//If there are not exactly 3 '.' characters in the return string, something went wrong.
            if (output.Count(x => x == '.') != 3)
            {
				DebugString("Failed to correctly convert the requested version to a string, there should be exactly 3 '.' characters. Returning the unconverted version.", DebugCategories.Misc, DebugLevel.High, DebugTypes.Warning);
				return versionString;
            }

			return output;
		}

		//==================== Editor Settings ====================

		//How much information should be debugged? Higher values equals more debugging.
		private DebugLevel activeDebugLevel = DebugLevel.User;

		public DebugLevel ActiveDebugLevel
		{
			get
			{
				if (TerraTiler2DBuildSettings.GetInstance() != null)
				{
					return TerraTiler2DBuildSettings.GetInstance().GetDebugLevel();
				}
				else
				{
					return activeDebugLevel;
				}
			}
			private set
			{
#if (UNITY_EDITOR)
				for (int i = 0; i <= (int)DebugLevel.All; i++)
				{
					Menu.SetChecked("Tools/TerraTiler2D/Settings/Debug Level/" + (DebugLevel)i, false);
				}
#endif
				activeDebugLevel = value;

				GenerateFolderStructure();

				if (TerraTiler2DBuildSettings.GetInstance() != null)
				{
					TerraTiler2DBuildSettings.GetInstance().SetDebugLevel(activeDebugLevel);
				}
#if (UNITY_EDITOR)
				EditorPrefs.SetInt("DebugLevel", (int)ActiveDebugLevel);

				Menu.SetChecked("Tools/TerraTiler2D/Settings/Debug Level/" + ActiveDebugLevel, true);
#endif
			}
		}

		private bool pauseBetweenNodes = true;

		public bool PauseBetweenNodes
		{
			get 
			{
				if (TerraTiler2DBuildSettings.GetInstance() != null)
				{
					return TerraTiler2DBuildSettings.GetInstance().GetPauseBetweenNodes();
				}
				else
				{
					return pauseBetweenNodes;
				}
			}
			private set
			{
				pauseBetweenNodes = value;

				if (TerraTiler2DBuildSettings.GetInstance() != null)
				{
					TerraTiler2DBuildSettings.GetInstance().SetPauseBetweenNodes(pauseBetweenNodes);
				}
#if (UNITY_EDITOR)
				EditorPrefs.SetBool("PauseBetweenNodes", PauseBetweenNodes);

				Menu.SetChecked("Tools/TerraTiler2D/Settings/Pause Between Nodes", PauseBetweenNodes);
#endif
			}
		}

		private bool giveTransparentTilesAPreviewColor = true;

		public bool GiveTransparentTilesAPreviewColor
		{
			get
			{
				return giveTransparentTilesAPreviewColor;
			}
			private set
			{
				giveTransparentTilesAPreviewColor = value;

#if (UNITY_EDITOR)
				EditorPrefs.SetBool("GiveTransparentTilesAPreviewColor", GiveTransparentTilesAPreviewColor);

				Menu.SetChecked("Tools/TerraTiler2D/Settings/Give Transparent Tiles A Preview Color", GiveTransparentTilesAPreviewColor);
#endif
			}
		}


		//Which categories should be debugged.
		private readonly Dictionary<DebugCategories, bool> DebugFilter = new Dictionary<DebugCategories, bool>()
		{
			{DebugCategories.Graph, true},
			{DebugCategories.Data, true},
			{DebugCategories.Node, true},
			{DebugCategories.Edge, true},
			{DebugCategories.Error, true},
			{DebugCategories.Misc, true},
			{DebugCategories.None, true},
		};

		private Dictionary<DebugCategories, string> DebugTags = new Dictionary<DebugCategories, string>()
		{
			 {DebugCategories.Graph, "<color=#6bf3d2>[Graph] </color>"},
			{DebugCategories.Data, "<color=#2dd121>[Data] </color>"},
			{DebugCategories.Node, "<color=#b066e8>[Node] </color>"},
			{DebugCategories.Edge, "<color=#f2abf5>[Edge] </color>"},
			{DebugCategories.Error, "<color=#ff0000>[ERROR] </color>"},
			{DebugCategories.Misc, "<color=#ebcc7c>[Misc] </color>"},
			{DebugCategories.None, ""},
		};

		public enum DebugLevel
		{
			None = 0,
			User = 1,
			Low = 2,
			Medium = 3,
			High = 4,
			All = 5
		}
		public enum DebugTypes
		{
			Default,
			Warning,
			Error
		}
		public enum DebugCategories
		{
			Graph,
			Data,
			Node,
			Edge,
			Error,
			Misc,
			None
		}

		//==================== Tool Settings ====================

		public enum NodeTypes
		{
			//=== Entry ===
			Entry = 0,

			//=== Variables ===
			Int = 100,
			IntBetween = 101,
			Float = 102,
			FloatBetween = 103,
			Bool = 104,
			Vector2 = 105,
			Vector3 = 106,
			Vector4 = 107,
			String = 108,
			Color = 109,
			Gradient = 110,
			Rect = 111,
			Tile = 112,
			TileLayer = 113,
			World = 114,

			//=== Conversion nodes ===
			ToString = 1000,
			IntToFloat = 1001,
			RoundToInt = 1002,
			FloorToInt = 1003,
			CeilToInt = 1004,
			BreakVector2 = 1005,
			BreakVector3 = 1006,
			BreakVector4 = 1007,
			Vector2ToVector3 = 1008,
			Vector2ToVector4 = 1009,
			Vector3ToVector2 = 1010,
			Vector3ToVector4 = 1011,
			Vector4ToVector2 = 1012,
			Vector4ToVector3 = 1013,

			//=== Math nodes ===
			//int
			IntAdd = 2000,
			IntSubtract = 2001,
			IntMultiply = 2002,
			IntDivide = 2003,
			IntModulo = 2004,
			IntPow = 2005,
			IntEquals = 2006,
			IntGreaterThan = 2007,
			IntEqualsOrGreaterThan = 2008,
			IntSqrt = 2009,
			IntClamp = 2010,
			IntMin = 2011,
			IntMax = 2012,
			IntCos = 2013,
			IntSin = 2014,
			IntTan = 2015,

			//float
			FloatAdd = 2100,
			FloatSubtract = 2101,
			FloatMultiply = 2102,
			FloatDivide = 2103,
			FloatModulo = 2104,
			FloatPow = 2105,
			FloatEquals = 2106,
			FloatGreaterThan = 2107,
			FloatEqualsOrGreaterThan = 2108,
			FloatSqrt = 2109,
			FloatClamp = 2110,
			FloatMin = 2111,
			FloatMax = 2112,
			FloatCos = 2113,
			FloatSin = 2114,
			FloatTan = 2115,

			//Vector2
			Vector2Add = 2200,
			Vector2Subtract = 2201,
			Vector2Multiply = 2202,
			Vector2Divide = 2203,
			Vector2Length = 2204,
			Vector2SetLength = 2205,
			Vector2Clamp = 2206,
			Vector2Min = 2207,
			Vector2Max = 2208,
			Vector2Dot = 2209,

			//Vector3
			Vector3Add = 2300,
			Vector3Subtract = 2301,
			Vector3Multiply = 2302,
			Vector3Divide = 2303,
			Vector3Length = 2304,
			Vector3SetLength = 2305,
			Vector3Clamp = 2306,
			Vector3Min = 2307,
			Vector3Max = 2308,
			Vector3Dot = 2309,

			//Vector4
			Vector4Add = 2400,
			Vector4Subtract = 2401,
			Vector4Multiply = 2402,
			Vector4Divide = 2403,
			Vector4Length = 2404,
			Vector4SetLength = 2405,
			Vector4Clamp = 2406,
			Vector4Min = 2407,
			Vector4Max = 2408,
			Vector4Dot = 2409,

			//Misc.
			PI = 3000,

			//=== Flow Nodes ===
			//Tile placer
			TilePlacerNoisemap = 4000,
			//TilePlacerLines = 4001,
			TilePlacerRectangles = 4002,
			TilePlacerEllipses = 4003,
			TilePlacerTriangles = 4004,
			TilePlacerTunnelerAStar = 4005,
			TilePlacerTunnelerShortestPath = 4006,
			//Other
			SetSeed = 4500,
			AddLayerToWorld = 4501,
			MergeTileLayers = 4502,

			//=== Noisemaps ===
			NoisemapPerlin = 5000,
			NoisemapValue = 5001,
			NoisemapGradient = 5002,
			NoisemapRandom = 5003,

			//=== Texture2D ===
			Texture2D = 6000,
			CreateTexture2D = 6001,
			MultiplyTexture2D = 6002,
			InverseTexture2D = 6003,
			ExtractTexture2D = 6004,
			ClampTexture2D = 6005,
			SetColorTexture2D = 6006,
			GetSizeTexture2D = 6007,
			SetSizeTexture2D = 6008,

			//=== Misc. ===
			TileMask = 7000,
			NoisemapDimension = 7001,

			//=== Property getters and setters ===
			IntProperty = 8000,
			IntPropertySet = 8001,
			FloatProperty = 8002,
			FloatPropertySet = 8003,
			BoolProperty = 8004,
			BoolPropertySet = 8005,
			Vector2Property = 8006,
			Vector2PropertySet = 8007,
			Vector3Property = 8008,
			Vector3PropertySet = 8009,
			Vector4Property = 8010,
			Vector4PropertySet = 8011,
			StringProperty = 8012,
			StringPropertySet = 8013,
			ColorProperty = 8014,
			ColorPropertySet = 8015,
			GradientProperty = 8016,
			GradientPropertySet = 8017,
			Texture2DProperty = 8018,
			Texture2DPropertySet = 8019,
			TileProperty = 8020,
			TilePropertySet = 8021,
			WorldProperty = 8022,
			WorldPropertySet = 8023,

			//=== Flow Control ===
			IfBranch = 9000,
			//SwitchBranch = 9001,
			ForLoop = 9002,
			//ForEachLoop = 9003,
			WhileLoop = 9004,
			Redirect = 9005,
			Array = 9006,
			GetFromArray = 9007,
			AddToArray = 9008,
			ArrayLength = 9009,
			GetArrayIndex = 9010,
			IsNull = 9011,

			//=== Debug ===
			DebugLog = 10000,

			Default
		}
		public Dictionary<NodeTypes, string> DefaultNodeNames = new Dictionary<NodeTypes, string>()
		{
			{NodeTypes.Entry, "Start"},

			{NodeTypes.Int, "Int"},
			{NodeTypes.IntBetween, "Random range int"},
			{NodeTypes.Float, "Float"},
			{NodeTypes.FloatBetween, "Random range float"},
			{NodeTypes.Bool, "Bool"},
			{NodeTypes.Vector2, "Vector2"},
			{NodeTypes.Vector3, "Vector3"},
			{NodeTypes.Vector4, "Vector4"},
			{NodeTypes.String, "String"},
			{NodeTypes.Color, "Color"},
			{NodeTypes.Gradient, "Gradient"},
			{NodeTypes.Rect, "Rect"},
			{NodeTypes.Tile, "Tile"},
			{NodeTypes.TileLayer, "Tile Layer"},
			{NodeTypes.World, "World"},

			{NodeTypes.ToString, "To String"},
			{NodeTypes.IntToFloat, "Int to Float"},
			{NodeTypes.RoundToInt, "Round to int"},
			{NodeTypes.FloorToInt, "Floor to int"},
			{NodeTypes.CeilToInt, "Ceil to int"},
			{NodeTypes.BreakVector2, "Break Vector2"},
			{NodeTypes.BreakVector3, "Break Vector3"},
			{NodeTypes.BreakVector4, "Break Vector4"},
			{NodeTypes.Vector2ToVector3, "Vector2 to Vector3"},
			{NodeTypes.Vector2ToVector4, "Vector2 to Vector4"},
			{NodeTypes.Vector3ToVector2, "Vector3 to Vector2"},
			{NodeTypes.Vector3ToVector4, "Vector3 to Vector4"},
			{NodeTypes.Vector4ToVector2, "Vector4 to Vector2"},
			{NodeTypes.Vector4ToVector3, "Vector4 to Vector3"},

			{NodeTypes.IntAdd, "Add (Int)"},
			{NodeTypes.IntSubtract, "Subtract (Int)"},
			{NodeTypes.IntMultiply, "Multiply (Int)"},
			{NodeTypes.IntDivide, "Divide (Int)"},
			{NodeTypes.IntModulo, "Modulo (Int)"},
			{NodeTypes.IntPow, "Pow (Int)"},
			{NodeTypes.IntEquals, "Equals (Int)"},
			{NodeTypes.IntGreaterThan, "Greater Than (Int)"},
			{NodeTypes.IntEqualsOrGreaterThan, "Greater or Equals (Int)"},
			{NodeTypes.IntSqrt, "Sqrt (Int)"},
			{NodeTypes.IntClamp, "Clamp (Int)"},
			{NodeTypes.IntMin, "Min (Int)"},
			{NodeTypes.IntMax, "Max (Int)"},
			{NodeTypes.IntCos, "Cos (Int)"},
			{NodeTypes.IntSin, "Sin (Int)"},
			{NodeTypes.IntTan, "Tan (Int)"},

			{NodeTypes.FloatAdd, "Add (float)"},
			{NodeTypes.FloatSubtract, "Subtract (float)"},
			{NodeTypes.FloatMultiply, "Multiply (float)"},
			{NodeTypes.FloatDivide, "Divide (float)"},
			{NodeTypes.FloatModulo, "Modulo (float)"},
			{NodeTypes.FloatPow, "Pow (float)"},
			{NodeTypes.FloatEquals, "Equals (float)"},
			{NodeTypes.FloatGreaterThan, "Greater Than (float)"},
			{NodeTypes.FloatEqualsOrGreaterThan, "Greater or Equals (float)"},
			{NodeTypes.FloatSqrt, "Sqrt (float)"},
			{NodeTypes.FloatClamp, "Clamp (float)"},
			{NodeTypes.FloatMin, "Min (float)"},
			{NodeTypes.FloatMax, "Max (float)"},
			{NodeTypes.FloatCos, "Cos (float)"},
			{NodeTypes.FloatSin, "Sin (float)"},
			{NodeTypes.FloatTan, "Tan (float)"},

			{NodeTypes.Vector2Add, "Add (Vector2)"},
			{NodeTypes.Vector2Subtract, "Subtract (Vector2)"},
			{NodeTypes.Vector2Multiply, "Multiply (Vector2)"},
			{NodeTypes.Vector2Divide, "Divide (Vector2)"},
			{NodeTypes.Vector2Length, "Length (Vector2)"},
			{NodeTypes.Vector2SetLength, "Set Length (Vector2)"},
			{NodeTypes.Vector2Clamp, "Clamp (Vector2)"},
			{NodeTypes.Vector2Min, "Min (Vector2)"},
			{NodeTypes.Vector2Max, "Max (Vector2)"},
			{NodeTypes.Vector2Dot, "Dot (Vector2)"},

			{NodeTypes.Vector3Add, "Add (Vector3)"},
			{NodeTypes.Vector3Subtract, "Subtract (Vector3)"},
			{NodeTypes.Vector3Multiply, "Multiply (Vector3)"},
			{NodeTypes.Vector3Divide, "Divide (Vector3)"},
			{NodeTypes.Vector3Length, "Length (Vector3)"},
			{NodeTypes.Vector3SetLength, "Set Length (Vector3)"},
			{NodeTypes.Vector3Clamp, "Clamp (Vector3)"},
			{NodeTypes.Vector3Min, "Min (Vector3)"},
			{NodeTypes.Vector3Max, "Max (Vector3)"},
			{NodeTypes.Vector3Dot, "Dot (Vector3)"},

			{NodeTypes.Vector4Add, "Add (Vector4)"},
			{NodeTypes.Vector4Subtract, "Subtract (Vector4)"},
			{NodeTypes.Vector4Multiply, "Multiply (Vector4)"},
			{NodeTypes.Vector4Divide, "Divide (Vector4)"},
			{NodeTypes.Vector4Length, "Length (Vector4)"},
			{NodeTypes.Vector4SetLength, "Set Length (Vector4)"},
			{NodeTypes.Vector4Clamp, "Clamp (Vector4)"},
			{NodeTypes.Vector4Min, "Min (Vector4)"},
			{NodeTypes.Vector4Max, "Max (Vector4)"},
			{NodeTypes.Vector4Dot, "Dot (Vector4)"},

			{NodeTypes.PI, "PI"},

			{NodeTypes.TilePlacerNoisemap, "Tile Placer (Noisemap)"},
			{NodeTypes.TilePlacerRectangles, "Tile Placer (Rectangles)"},
			{NodeTypes.TilePlacerEllipses, "Tile Placer (Ellipses)"},
			{NodeTypes.TilePlacerTriangles, "Tile Placer (Triangles)"},
			{NodeTypes.TilePlacerTunnelerAStar, "Tunneler (A*)"},
			{NodeTypes.TilePlacerTunnelerShortestPath, "Tunneler (Shortest Path)"},
			{NodeTypes.SetSeed, "Set Seed"},
			{NodeTypes.AddLayerToWorld, "Add Layer to World"},
			{NodeTypes.MergeTileLayers, "Merge TileLayers"},

			{NodeTypes.NoisemapPerlin, "Noisemap Perlin"},
			{NodeTypes.NoisemapValue, "Noisemap Value"},
			{NodeTypes.NoisemapGradient, "Noisemap Gradient"},
			{NodeTypes.NoisemapRandom, "Noisemap Random"},

			{NodeTypes.Texture2D, "Texture2D"},
			{NodeTypes.CreateTexture2D, "Create Texture2D"},
			{NodeTypes.MultiplyTexture2D, "Multiply Texture2D"},
			{NodeTypes.InverseTexture2D, "Inverse Texture2D"},
			{NodeTypes.ExtractTexture2D, "Extract Texture2D"},
			{NodeTypes.ClampTexture2D, "Clamp Texture2D"},
			{NodeTypes.SetColorTexture2D, "Set Color Texture2D"},
			{NodeTypes.GetSizeTexture2D, "Get Size Texture2D"},
			{NodeTypes.SetSizeTexture2D, "Set Size Texture2D"},

			{NodeTypes.TileMask, "Tile Mask"},
			{NodeTypes.NoisemapDimension, "Noisemap Dimension"},

			{NodeTypes.IfBranch, "If branch"},
			//{NodeTypes.SwitchBranch, "Switch branch"},
			{NodeTypes.ForLoop, "For loop"},
			//{NodeTypes.ForEachLoop, "For each loop"},
			{NodeTypes.WhileLoop, "While loop"},
			{NodeTypes.Redirect, "Redirect"},
			{NodeTypes.Array, "Array"},
			{NodeTypes.GetFromArray, "Get from array"},
			{NodeTypes.AddToArray, "Add to array"},
			{NodeTypes.ArrayLength, "Array length"},
			{NodeTypes.GetArrayIndex, "Get array index"},
			{NodeTypes.IsNull, "Is Null"},

			{NodeTypes.DebugLog, "Debug Log"},

			{NodeTypes.Default, "Default"},
		};

		public Dictionary<Type, Color> TypeColors = new Dictionary<Type, Color>()
		{
			{typeof(Flow),                  new Color(1,        1,          1)},
			{typeof(object),                new Color(0.75f,    0.75f,      0.75f)},

			{typeof(World),                 new Color(0,        1f,         1f)},
			{typeof(TileLayer),             new Color(0,        0.75f,      1f)},
			{typeof(TileLayerMask),         new Color(0,        0.75f,      1f)},
			{typeof(TileBase),              new Color(0,        0.75f,      0.75f)},
			{typeof(TileMask),              new Color(0,        0.75f,      0.75f)},

			{typeof(int),                   new Color(0.2f,     0.75f,      0.4f)},
			{typeof(float),                 new Color(0.6f,     0.75f,      0.2f)},
			{typeof(bool),                  new Color(1,        0.1f,       0.1f)},
			{typeof(Vector2),               new Color(1,        0.75f,      0)},
			{typeof(Vector3),               new Color(1,        0.6f,       0)},
			{typeof(Vector4),               new Color(1,        0.45f,      0)},
			{typeof(string),                new Color(1,        1,          0.2f)},
			{typeof(Color),                 new Color(0.6f,     0.2f,       1)},
			{typeof(Gradient),              new Color(0.75f,    0,          0.75f)},
			{typeof(Texture2D),             new Color(1,        0.6f,       0.2f)},
			{typeof(Rect),                  new Color(0.2f,     0.75f,      0.2f)},
			{typeof(NoisemapDimension),     new Color(0.6f,     0.6f,       0.6f)},
			{typeof(TileBase[]),            new Color(0,        0.5f,       0.5f)},
		};

		public float AutoSaveDelay = 3.0f;

		public int MaxWorldSize = 50000;

		public int DefaultNullTileIndex = 0;
		public int InvalidTileIndex = 9999999;

		public Vector2 DefaultEntryNodePosition = new Vector2(300, 400);
		public Vector2 DefaultNodeSize = new Vector2(150, 200);

		public Vector2 DefaultPreviewSize = new Vector2(300, 300);

		public Vector2 BlackboardSize = new Vector2(250, 400);

		public Vector2 MiniMapSize = new Vector2(200, 150);

		public int MinNodeFieldSize = 25;
		public int MaxNodeFieldSize = 150;

		public Color UITextColor = new Color(0.75f, 0.75f, 0.75f, 1f);
		public Color UIContainerColor = new Color(0.3f, 0.3f, 0.3f, 1f);
		public Color UISubContainerColor = new Color(0.2f, 0.2f, 0.2f, 1f);

		public Color UIContainerBorderColor = new Color(0f, 0f, 0f, 1f);
		public Color UIContainerDepthBorderColorTop = new Color(0.45f, 0.45f, 0.45f, 1f);
		public Color UIContainerDepthBorderColorSide = new Color(0.35f, 0.35f, 0.35f, 1f);
		public Color UIContainerDepthBorderColorBottom = new Color(0.25f, 0.25f, 0.25f, 1f);

		public int UIContainerBorderWidth = 1;
		public int UIContainerDepthBorderWidth = 4;
		public int UIContainerOuterPadding = 4;
		public int UIContainerInnerPadding = 2;
		public int UIContainerCornerRadius = 5;

		public int NodeTitleContainerHeight = 20;

		public StyleSheet NodeStyleSheet = Resources.Load<StyleSheet>("Editor/Stylesheets/Node");
		public StyleSheet GraphStyleSheet = Resources.Load<StyleSheet>("Editor/Stylesheets/Graph");

		public string DefaultGraphName = "New TerraTiler2D Graph";
		public string TerraTiler2DWindowName = "TerraTiler2D";

		private string TempGraphDataPath = "Assets/TerraTiler2D/Resources/Editor/Temp";
		public string TempGraphDataName = "TempTTGraphData.asset";

		private string DefaultGraphSavePath = "Assets/Resources/TerraTiler2D/Graphs";

		public Texture2D GraphFlowPortIcon = Resources.Load<Texture2D>("Editor/Icons/GraphFlowPort_Icon");
		public Texture2D MultiCapacityPortIcon = Resources.Load<Texture2D>("Editor/Icons/MultiCapacityPort_Icon");
		public Texture2D IsRequiredPortIcon = Resources.Load<Texture2D>("Editor/Icons/IsRequiredPort_Icon");
		public Texture2D IsRequiredMultiCapacityPortIcon = Resources.Load<Texture2D>("Editor/Icons/IsRequiredMultiCapacityPort_Icon");
		public Texture2D ArrayPortIcon = Resources.Load<Texture2D>("Editor/Icons/ArrayPort_Icon");
		public Texture2D MaskPortIcon = Resources.Load<Texture2D>("Editor/Icons/MaskPort_Icon");

		public Texture2D NodeSearchGroupIcon = Resources.Load<Texture2D>("Editor/Icons/MenuGroup_Icon");
		public Texture2D NodeSearchGroupBackIcon = Resources.Load<Texture2D>("Editor/Icons/MenuGroupBack_Icon");
		public Texture2D SearchEntryTooltipIcon = Resources.Load<Texture2D>("Editor/Icons/SearchEntryTooltip_Icon");

		public Texture2D Property_9Tile_Icon = Resources.Load<Texture2D>("Editor/Icons/PropertyFade");

		private string GraphResourcesFolder = "Graph Resources";

		//Max length is clamped, because float and int input fields act strangely with more than 7 characters.
		public int MaxFieldInputLength = 11;

		//==================== Getters/Setters ====================
		public string GetDefaultGraphSavePath()
		{
			GenerateFolderStructure();

			return DefaultGraphSavePath + "/";
		}
		public string GetTempGraphDataPath()
		{
			GenerateFolderStructure();

			return TempGraphDataPath + "/" + TempGraphDataName;
		}
		public string GetGraphResourcesPath()
        {
			GenerateFolderStructure();

			return "TerraTiler2D/" + GraphResourcesFolder;
        }

		public IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
		{
			List<T> objects = new List<T>();
			foreach (Type type in
				Assembly.GetAssembly(typeof(T)).GetTypes()
				.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
			{
				objects.Add((T)Activator.CreateInstance(type, constructorArgs));
			}
			return objects;
		}

		public IEnumerable<Node> GetAllNodeClasses(bool includeEntryNode = false, params object[] constructorArgs)
		{
			var allNodeClasses = GetEnumerableOfType<Node>(constructorArgs);

			if (includeEntryNode)
			{
				return allNodeClasses;
			}

			//Remove the entry node.
			return allNodeClasses.Where(x => x.GetType() != typeof(Entry_Node));
		}

#if (UNITY_EDITOR)
		[MenuItem("Tools/TerraTiler2D/Settings/Debug Level/None", priority = 300)]
		private static void SetDebugLevelNone()
		{
			SetDebugLevel(DebugLevel.None);
		}
		[MenuItem("Tools/TerraTiler2D/Settings/Debug Level/User", priority = 300)]
		private static void SetDebugLevelUser()
		{
			SetDebugLevel(DebugLevel.User);
		}
		[MenuItem("Tools/TerraTiler2D/Settings/Debug Level/Low", priority = 300)]
		private static void SetDebugLevelLow()
		{
			SetDebugLevel(DebugLevel.Low);
		}
		[MenuItem("Tools/TerraTiler2D/Settings/Debug Level/Medium", priority = 300)]
		private static void SetDebugLevelMedium()
		{
			SetDebugLevel(DebugLevel.Medium);
		}
		[MenuItem("Tools/TerraTiler2D/Settings/Debug Level/High", priority = 300)]
		private static void SetDebugLevelHigh()
		{
			SetDebugLevel(DebugLevel.High);
		}
		[MenuItem("Tools/TerraTiler2D/Settings/Debug Level/All", priority = 300)]
		private static void SetDebugLevelAll()
		{
			SetDebugLevel(DebugLevel.All);
		}
		private static void SetDebugLevel(DebugLevel level)
		{
			GetInstance().ActiveDebugLevel = level;
		}
		private void SetDebugLevelNonStatic(DebugLevel level)
		{
			ActiveDebugLevel = level;
		}

		[MenuItem("Tools/TerraTiler2D/Settings/Give Transparent Tiles A Preview Color", priority = 300)]
		private static void SetGiveTransparentTilesAPreviewColor()
		{
			bool toggle = !EditorPrefs.GetBool("GiveTransparentTilesAPreviewColor");

			GetInstance().GiveTransparentTilesAPreviewColor = toggle;
		}
		private void SetGiveTransparentTilesAPreviewColorNonStatic(bool toggle)
		{
			GiveTransparentTilesAPreviewColor = toggle;
		}

		[MenuItem("Tools/TerraTiler2D/Settings/Pause Between Nodes", priority = 300)]
		private static void SetPauseBetweenNodes()
		{
			bool toggle = !EditorPrefs.GetBool("PauseBetweenNodes");

			GetInstance().PauseBetweenNodes = toggle;
		}
		private void SetPauseBetweenNodesNonStatic(bool toggle)
		{
			PauseBetweenNodes = toggle;
		}
#endif

		//==================== Methods ====================

		public void DebugString(string text, DebugCategories debugCategory = DebugCategories.None, DebugLevel debugLevel = DebugLevel.None, DebugTypes debugType = DebugTypes.Default)
		{
			if (debugLevel <= ActiveDebugLevel && DebugFilter[debugCategory])
			{
				switch (debugType)
				{
					case DebugTypes.Default:
						Debug.Log(DebugTags[debugCategory] + text);
						break;
					case DebugTypes.Warning:
						Debug.LogWarning(DebugTags[debugCategory] + text);
						break;
					case DebugTypes.Error:
						Debug.LogError(DebugTags[debugCategory] + text);
						break;
					default:
						break;
				}
			}
		}

		public void DebugChildren(VisualElement parent, int iter = 0)
		{
			string tabIndex = "";
			for (int i = 0; i < iter; i++)
			{
				tabIndex += "\t";
			}

			Debug.Log(iter + tabIndex + "- " + parent);
			for (int i = 0; i < parent.childCount; i++)
			{
				DebugChildren(parent.Children().ToArray()[i], iter + 1);
			}
		}

		//Is the type a List?
		public bool IsList(Type type)
		{
			if (type != null && type.IsGenericType)
			{
				if (type.GetGenericTypeDefinition() == typeof(List<>))
				{
					return true;
				}
			}

			return false;
		}
		//Turn the type into a List type using reflection
		public Type GetListType(Type type)
		{
			if (type == null)
			{
				return type;
			}
			
			//Get the type of a List without type
			var listOpenGenericType = typeof(List<>);
			//Make the type a generic type
			var listClosedGenericType = listOpenGenericType.MakeGenericType(type);

			//Return the List with the type
			return listClosedGenericType;
		}

		public bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
		{
			while (toCheck != null && toCheck != typeof(object))
			{
				var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
				if (generic == cur)
				{
					return true;
				}
				toCheck = toCheck.BaseType;
			}
			return false;
		}

		public List<T> SetObjectListType<T>(List<object> objects)
		{
			List<T> newList = new List<T>();
			for (int i = 0; i < objects.Count; i++)
			{
				newList.Insert(i, (T)objects[i]);
			}

			return newList;
		}

		public bool ContainsVisualElementIterative(VisualElement parent, VisualElement child)
		{
			if (parent == child)
			{
				return true;
			}
			if (parent.Contains(child))
			{
				return true;
			}

			var enumerator = parent.Children().GetEnumerator();

			while (enumerator.MoveNext())
			{
				if (ContainsVisualElementIterative(enumerator.Current, child))
				{
					return true;
				}
			}

			return false;
		}

		public VisualElement GetFirstChildVisualElementOfType(VisualElement parent, Type type)
		{
			List<VisualElement> children = parent.Children().ToList();

			foreach (VisualElement child in children)
			{
				if (child.GetType() == type)
				{
					return child;
				}
			}

			foreach (VisualElement child in children)
			{
				return GetFirstChildVisualElementOfType(child, type);
			}

			return null;
		}
		public VisualElement GetFirstParentVisualElementOfType(VisualElement child, Type type)
		{
			if (child.parent != null)
			{
				if (child.parent.GetType() == type)
				{
					return child.parent;
				}
				else
				{
					return GetFirstParentVisualElementOfType(child.parent, type);
				}
			}

			return null;
		}

//#if (UNITY_EDITOR)
		public Color[] GetTileGraphPreview(TileBase tile, int sizeX, int sizeY)
		{
			//Prepare the Color[] that will be returned.
			Color[] pixelBlock = new Color[sizeX * sizeY];
			Texture2D tileTexture = null;
			Color rendererColor = Color.white;

			//Create an empty GameObject to parent all the Tiles to.
			GameObject tempTilemapGameobject = new GameObject("Temp Tilemap");
			tempTilemapGameobject.SetActive(false);
			tempTilemapGameobject.AddComponent<TilemapRenderer>();

			//Create a 3x3 tilemap, and fill all the tiles with the target tile.
			Tilemap tempTilemap = tempTilemapGameobject.GetComponent<Tilemap>();
			tempTilemap.size = new Vector3Int(1, 1, 0);
			tempTilemap.SetTile(Vector3Int.zero, tile);

			if (tempTilemap.GetSprite(Vector3Int.zero) != null)
			{
				tileTexture = tempTilemap.GetSprite(Vector3Int.zero).texture;
				rendererColor = tempTilemap.GetColor(Vector3Int.zero);
			}

			//If a texture was found
			if (tileTexture != null)
			{
				//If the requested size is larger than the size of the texture
				if (sizeX > tileTexture.width || sizeY > tileTexture.height)
				{
					//Get as large of a block of pixels as possible from the texture
					Vector2Int copyPixelBlockSize = new Vector2Int(Mathf.Min(tileTexture.width, sizeX), Mathf.Min(tileTexture.height, sizeY));
					Color[] copyPixelBlock = new Color[copyPixelBlockSize.x * copyPixelBlockSize.y];
					copyPixelBlock = tileTexture.GetPixels(0, 0, copyPixelBlockSize.x, copyPixelBlockSize.y);

					//Multiply the color of these pixels by the color of the image renderer, and leave any remaining pixels empty.
					for (int i = 0; i < sizeY; i++)
					{
						for (int j = 0; j < sizeX; j++)
						{
							if (i < copyPixelBlockSize.y)
							{
								if (j < copyPixelBlockSize.x)
								{
									pixelBlock[(i * sizeX) + j] = copyPixelBlock[(i * copyPixelBlockSize.x) + j] * rendererColor;
								}
							}
						}
					}
				}
				else
				{
					//Take a block of pixels from the center of the texture.
					pixelBlock = tileTexture.GetPixels(Mathf.RoundToInt((tileTexture.width - sizeX) * 0.5f), Mathf.RoundToInt((tileTexture.height - sizeY) * 0.5f), sizeX, sizeY);

					bool hasAColor = false;

					//Do we have to change anything about the color of the preview
					if (GiveTransparentTilesAPreviewColor || rendererColor != Color.white)
					{
						for (int i = 0; i < pixelBlock.Length; i++)
						{
							//If the renderer has a color other than white, multiply the pixels by that color.
							if (rendererColor != Color.white)
							{
								pixelBlock[i] *= rendererColor;
							}
							else if(hasAColor)
							{
								break;
							}

							//Check if the pixelBlock is completely transparent or not
							if (GiveTransparentTilesAPreviewColor)
							{
								//Check if this pixel is not completely transparent. As soon as one non-transparent pixel is found, this pixelBlock is deemed fine for previewing.
								if (pixelBlock[i].a > 0)
								{
									hasAColor = true;
								}
							}
						}
					}

					//If this pixelBlock is completely transparent
					if (GiveTransparentTilesAPreviewColor && !hasAColor)
					{
						for (int i = 0; i < pixelBlock.Length; i++)
						{
							pixelBlock[i].a = 1;
						}
					}
				}
			}

			GameObject.DestroyImmediate(tempTilemapGameobject);

			return pixelBlock;
		}
//#endif

		//Resizes a texture.
		//TODO: Is not great performance wise.
		public Texture2D ResizeTexture2D(Texture2D source, int newWidth, int newHeight)
		{
			source.filterMode = FilterMode.Point;
			RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
			rt.filterMode = FilterMode.Point;
			RenderTexture.active = rt;
			Graphics.Blit(source, rt);
			Texture2D nTex = new Texture2D(newWidth, newHeight);
			nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
			nTex.Apply();
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(rt);
			return nTex;
		}
		public Texture2D ScaleToFitTexture2D(Texture2D source, int targetWidth, int targetHeight)
        {
            if (source.width == targetWidth && source.height == targetHeight)
            {
				return source;
            }

			//Calculate the ratios of both dimensions
			float widthRatio = (float)targetWidth / (float)source.width;
			float heightRatio = (float)targetHeight / (float)source.height;

			//Resize the texture by the smallest ration between width and height, so that at least one of the dimensions has the target size
			Texture2D fittedTexture = ResizeTexture2D(source, Mathf.RoundToInt(source.width * (Mathf.Min(widthRatio, heightRatio))), Mathf.RoundToInt(source.height * (Mathf.Min(widthRatio, heightRatio))));

			Texture2D centeredTexture = new Texture2D(targetWidth, targetHeight);

			//Calculate how much blank space there will be for X and Y
			Vector2Int blankSpace = new Vector2Int((targetWidth - fittedTexture.width) / 2, (targetHeight - fittedTexture.height) / 2);

			//Set all the pixels.
			for (int x = 0; x < targetWidth; x++)
			{
				for (int y = 0; y < targetHeight; y++)
				{
					//Center the Texture, by putting blank space around it.
                    if (x >= blankSpace.x && x < targetWidth - blankSpace.x &&
						y >= blankSpace.y && y < targetHeight - blankSpace.y)
                    {
						centeredTexture.SetPixel(x, y, fittedTexture.GetPixel(x - blankSpace.x, y - blankSpace.y));
                    }
				}
			}

			centeredTexture.Apply();

			return centeredTexture;
        }

		private void GenerateFolderStructure()
		{
#if (UNITY_EDITOR)
			if (!AssetDatabase.IsValidFolder("Assets/TerraTiler2D"))
			{
				AssetDatabase.CreateFolder("Assets", "TerraTiler2D");
			}
			if (!AssetDatabase.IsValidFolder("Assets/TerraTiler2D/Resources"))
			{
				AssetDatabase.CreateFolder("Assets/TerraTiler2D", "Resources");
			}
			if (!AssetDatabase.IsValidFolder("Assets/TerraTiler2D/Resources/Editor"))
			{
				AssetDatabase.CreateFolder("Assets/TerraTiler2D/Resources", "Editor");
			}
			if (!AssetDatabase.IsValidFolder("Assets/TerraTiler2D/Resources/Editor/Icons"))
			{
				AssetDatabase.CreateFolder("Assets/TerraTiler2D/Resources/Editor", "Icons");
			}
			if (!AssetDatabase.IsValidFolder("Assets/TerraTiler2D/Resources/Editor/Stylesheets"))
			{
				AssetDatabase.CreateFolder("Assets/TerraTiler2D/Resources/Editor", "Stylesheets");
			}
			if (!AssetDatabase.IsValidFolder("Assets/TerraTiler2D/Resources/Editor/Temp"))
			{
				AssetDatabase.CreateFolder("Assets/TerraTiler2D/Resources/Editor", "Temp");
			}


			if (!AssetDatabase.IsValidFolder("Assets/Resources"))
			{
				AssetDatabase.CreateFolder("Assets", "Resources");
			}
			if (!AssetDatabase.IsValidFolder("Assets/Resources/TerraTiler2D"))
			{
				AssetDatabase.CreateFolder("Assets/Resources", "TerraTiler2D");
			}
			if (!AssetDatabase.IsValidFolder("Assets/Resources/TerraTiler2D/" + GraphResourcesFolder))
			{
				AssetDatabase.CreateFolder("Assets/Resources/TerraTiler2D", GraphResourcesFolder);
			}
			if (!AssetDatabase.IsValidFolder("Assets/Resources/TerraTiler2D/Graphs"))
			{
				AssetDatabase.CreateFolder("Assets/Resources/TerraTiler2D", "Graphs");
			}
			if (!AssetDatabase.IsValidFolder("Assets/Resources/TerraTiler2D/Editor"))
			{
				AssetDatabase.CreateFolder("Assets/Resources/TerraTiler2D", "Editor");
			}
			if (!AssetDatabase.IsValidFolder("Assets/Resources/TerraTiler2D/Editor/Settings"))
			{
				AssetDatabase.CreateFolder("Assets/Resources/TerraTiler2D/Editor", "Settings");
			}

			AssetDatabase.Refresh();
			AssetDatabase.SaveAssets();
#endif
		}
	}
}

