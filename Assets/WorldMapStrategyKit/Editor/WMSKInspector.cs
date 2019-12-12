using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using WorldMapStrategyKit.PathFinding;
using WorldMapStrategyKit.MapGenerator;

namespace WorldMapStrategyKit {
	[CustomEditor (typeof(WMSK))]
	public class WMSKInspector : Editor {

		enum KEY_PRESET {
			WASD = 0,
			ArrowKeys = 1
		}


		WMSK _map;
		Texture2D _headerTexture;
		string[] earthStyleOptions, frontiersDetailOptions;
		int[] earthStyleValues;
		GUIStyle sectionHeaderStyle, warningLabelStyle;
		bool expandWindowSection, expandViewportSection, expandEarthSection, expandCitiesSection, expandCountriesSection, expandProvincesSection, expandTilesSection, expandInteractionSection, expandPathFindingSection, expandGrid, expandMiscellanea;
		string[] pathFindingHeuristicOptions;
		int[] pathFindingHeuristicValues;
		SerializedProperty isDirty;
		TerrainData defaultTerrainData;
		KEY_PRESET keyPreset = KEY_PRESET.WASD;
		bool tileSizeComputed;

		void OnEnable () {

			_map = (WMSK)target;
			if (_map == null)
				return;

			_headerTexture = Resources.Load<Texture2D> ("WMSK/EditorHeader");
			if (_map.countries == null) {
				_map.Init ();
			}
			earthStyleOptions = new string[] {
				"Solid Color", "Texture", "Natural", "Natural HighRes", "Natural HighRes 16K", "Alternate Style 1", "Alternate Style 2", "Alternate Style 3", "Scenic", "Viewport Scenic Plus", "Viewport Scenic Plus Alt 1", "Viewport Scenic Plus 16K"
			};
			earthStyleValues = new int[] {
				(int)EARTH_STYLE.SolidColor,
				(int)EARTH_STYLE.Texture,
				(int)EARTH_STYLE.Natural,
				(int)EARTH_STYLE.NaturalHighRes,
				(int)EARTH_STYLE.NaturalHighRes16K,
				(int)EARTH_STYLE.Alternate1,
				(int)EARTH_STYLE.Alternate2,
				(int)EARTH_STYLE.Alternate3,
				(int)EARTH_STYLE.NaturalScenic,
				(int)EARTH_STYLE.NaturalScenicPlus,
				(int)EARTH_STYLE.NaturalScenicPlusAlternate1,
				(int)EARTH_STYLE.NaturalScenicPlus16K
			};
			frontiersDetailOptions = new string[] {
				"Low",
				"High"
			};
//			pathFindingHeuristicOptions = new string[] { "Manhattan", "MaxDXDY", "DiagonalShortCut", "Euclidean", "EuclideanNoSQR", "Custom" };
//			pathFindingHeuristicValues = new int[] { (int)HeuristicFormula.Manhattan, (int)HeuristicFormula.MaxDXDY, (int)HeuristicFormula.DiagonalShortCut, (int)HeuristicFormula.Euclidean, (int)HeuristicFormula.EuclideanNoSQR, (int)HeuristicFormula.Custom1 };
			pathFindingHeuristicOptions = new string[] {
				"Manhattan",
				"MaxDXDY",
				"DiagonalShortCut",
				"Euclidean"
			};
			pathFindingHeuristicValues = new int[] {
				(int)HeuristicFormula.Manhattan,
				(int)HeuristicFormula.MaxDXDY,
				(int)HeuristicFormula.DiagonalShortCut,
				(int)HeuristicFormula.Euclidean
			};

			defaultTerrainData = Resources.Load<TerrainData> ("WMSK/Terrain/Terrain");

			isDirty = serializedObject.FindProperty ("isDirty");

			// Restore folding sections statte
			expandWindowSection = EditorPrefs.GetBool ("expandWindowSection", false);
			expandViewportSection = EditorPrefs.GetBool ("expandViewportSection", false);
			expandEarthSection = EditorPrefs.GetBool ("expandEarthSection", false);
			expandCitiesSection = EditorPrefs.GetBool ("expandCitiesSection", false);
			expandCountriesSection = EditorPrefs.GetBool ("expandCountriesSection", false);
			expandProvincesSection = EditorPrefs.GetBool ("expandProvincesSection", false);
			expandTilesSection = EditorPrefs.GetBool ("expandTilesSection", false);
			expandInteractionSection = EditorPrefs.GetBool ("expandInteractionSection", false);
			expandPathFindingSection = EditorPrefs.GetBool ("expandPathFindingSection", false);
			expandGrid = EditorPrefs.GetBool ("expandGrid", false);
			expandMiscellanea = EditorPrefs.GetBool ("expandMiscellanea", false);
		}

		void OnDisable () {
			// Restore folding sections state
			EditorPrefs.SetBool ("expandWindowSection", expandWindowSection);
			EditorPrefs.SetBool ("expandViewportSection", expandViewportSection);
			EditorPrefs.SetBool ("expandEarthSection", expandEarthSection);
			EditorPrefs.SetBool ("expandCitiesSection", expandCitiesSection);
			EditorPrefs.SetBool ("expandCountriesSection", expandCountriesSection);
			EditorPrefs.SetBool ("expandProvincesSection", expandProvincesSection);
			EditorPrefs.SetBool ("expandTilesSection", expandTilesSection);
			EditorPrefs.SetBool ("expandInteractionSection", expandInteractionSection);
			EditorPrefs.SetBool ("expandPathFindingSection", expandPathFindingSection);
			EditorPrefs.SetBool ("expandGrid", expandGrid);
			EditorPrefs.SetBool ("expandMiscellanea", expandMiscellanea);
		}

		public override void OnInspectorGUI () {

			if (sectionHeaderStyle == null) {
				sectionHeaderStyle = new GUIStyle (EditorStyles.foldout);
			}
			Color foldoutColor = EditorGUIUtility.isProSkin ? new Color (0.52f, 0.66f, 0.9f) : new Color (0.12f, 0.16f, 0.4f);
			sectionHeaderStyle.SetFoldoutColor (foldoutColor);

			EditorGUILayout.Separator ();
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;  
			GUILayout.Label (_headerTexture, GUILayout.ExpandWidth (true));
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;  
			EditorGUILayout.Separator ();

			EditorGUILayout.BeginVertical ();
			expandWindowSection = EditorGUILayout.Foldout (expandWindowSection, "Window Settings", sectionHeaderStyle);

			if (expandWindowSection) {
				if (!_map.renderViewportIsTerrain) {
					_map.wrapHorizontally = EditorGUILayout.Toggle ("Wrap Horizontally", _map.wrapHorizontally);
					if (_map.wrapHorizontally && !_map.renderViewportIsEnabled) {
						EditorGUILayout.HelpBox("Wrap requires render viewport object.", MessageType.Warning);
					}
				}

				if (!_map.renderViewportIsEnabled || _map.renderViewportIsTerrain) {
					_map.enableFreeCamera = EditorGUILayout.Toggle (new GUIContent ("Enable Free Camera", "In Terrain mode, enable this option to allow camera position and rotation changes outside of WMSK control."), _map.enableFreeCamera);
				}

				_map.fitWindowWidth = EditorGUILayout.Toggle ("Fit Window Width", _map.fitWindowWidth);
				_map.fitWindowHeight = EditorGUILayout.Toggle ("Fit Window Height",_map.fitWindowHeight);

				if (_map.renderViewportIsEnabled && !_map.renderViewportIsTerrain) {
					_map.renderViewportUIPanel = (RectTransform)EditorGUILayout.ObjectField (new GUIContent ("UI Panel", "Optionally assign a UI panel to make viewport adapt to the panel position and size."), _map.renderViewportUIPanel, typeof(RectTransform), true);
				}

				_map.flyToScreenCenter = EditorGUILayout.Vector2Field (new GUIContent ("Screen Center", "The assumed screen center defined in normalized coordinates 0-1 range (0.5, 0.5 = normal screen center). Customize to offset FlyTo() destination for example."), _map.flyToScreenCenter);

				EditorGUILayout.BeginHorizontal ();
				if (_map.gameObject.activeInHierarchy) {
					if (GUILayout.Button ("Redraw")) {
						_map.Redraw (true);
					}
					if (GUILayout.Button ("Center Map")) {
						_map.CenterMap ();
					}
				}
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				float left, top, width, height;
				EditorGUILayout.LabelField ("Left", GUILayout.Width (45));
				left = EditorGUILayout.FloatField (_map.windowRect.x, GUILayout.Width (50));
				EditorGUILayout.LabelField ("Bottom", GUILayout.Width (45));
				top = EditorGUILayout.FloatField (_map.windowRect.y, GUILayout.Width (50));
				if (GUILayout.Button ("Clear Constraints", GUILayout.Width (120))) {
					_map.windowRect = new Rect (-0.5f, -0.5f, 1, 1);
					_map.isDirty = true;
					EditorGUIUtility.ExitGUI ();
				}
				if (GUILayout.Button ("?", GUILayout.Width (20))) {
					EditorUtility.DisplayDialog ("Window Constraints", "Set rectangular coordinates for the map constraints (-0.5f=left/bottom, 0.5f=top/right). Note that window constraints only work when Fit To Window Width and/or Fit To Window Height are checked.", "Ok");
					EditorGUIUtility.ExitGUI ();
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField ("Width", GUILayout.Width (45));
				width = EditorGUILayout.FloatField (_map.windowRect.width, GUILayout.Width (50));
				EditorGUILayout.LabelField ("Height", GUILayout.Width (45));
				height = EditorGUILayout.FloatField (_map.windowRect.height, GUILayout.Width (50));
				_map.windowRect = new Rect (left, top, width, height);

				if (GUILayout.Button ("Set SceneView Rect", GUILayout.Width (120))) {
					_map.windowRect = new Rect (0, 0, 1, 1);
					_map.windowRect = _map.renderViewportRectFromSceneView;
					_map.isDirty = true;
					EditorGUIUtility.ExitGUI ();
				}
				EditorGUILayout.EndHorizontal ();

				if (_map.gameObject.activeInHierarchy) {
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("Zoom Level", GUILayout.Width (85));
					float zoomLevel = _map.GetZoomLevel ();
					EditorGUILayout.LabelField (zoomLevel.ToString ("F7"), GUILayout.Width (110));

					if (GUILayout.Button ("Copy to ClipBoard", GUILayout.Width (120))) {
						EditorGUIUtility.systemCopyBuffer = "map.windowRect = new Rect(" + left.ToString ("F7") + "f, " + top.ToString ("F7") + "f, " + width.ToString ("F7") + "f, " + height.ToString ("F7") + "f);\nmap.SetZoomLevel(" + zoomLevel.ToString ("F7") + "f);";
						EditorUtility.DisplayDialog ("", "Window rect and zoom level sample code copied to clipboard.", "Ok");
					}
					EditorGUILayout.EndHorizontal ();
				}
			}

			EditorGUILayout.EndVertical (); 
			EditorGUILayout.Separator ();

			EditorGUILayout.BeginVertical ();
			EditorGUILayout.BeginHorizontal ();
			expandViewportSection = EditorGUILayout.Foldout (expandViewportSection, "Viewport Settings", sectionHeaderStyle);
			EditorGUILayout.EndHorizontal ();
			
			if (expandViewportSection) {
				EditorGUILayout.BeginHorizontal ();
				_map.renderViewport = (GameObject)EditorGUILayout.ObjectField ("Render Viewport", _map.renderViewport, typeof(GameObject), true);

				if (_map.renderViewport != null) {
					if (GUILayout.Button ("Unassign")) {
						_map.renderViewport = null;
					}
				}

				if (GUILayout.Button ("?", GUILayout.Width (24))) {
					EditorUtility.DisplayDialog ("Render Viewport Help", "Render Viewport allows to display the map onto a Viewport GameObject, cropping the map according to the size of the viewport.\n\nTo use this feature drag a Viewport prefab into the scene and assign the viewport gameobject created to this property.", "Ok");
				}
				EditorGUILayout.EndHorizontal ();
				
				if (_map.renderViewportIsEnabled) {
					_map.renderViewportResolution = EditorGUILayout.Slider (new GUIContent ("Resolution", "Resolution of the render texture. A value of 2 offers the best quality as it produces a super-sampling effect of x2. A value of 1 will match screen size but won't provide any antialias. A value greater than 2 should be used only under special scenarios like using multiple-cameras, one near the terrain surface to grasp more detail. Don't use values greater than 2 if not neccesary to avoid performance issues."), _map.renderViewportResolution, 0.5f, 8f);
					_map.renderViewportResolutionMaxRTWidth = EditorGUILayout.IntField (new GUIContent ("Max Texture Width", "Maximum width for the render texture. Recommended value is 2048 for most cases."), _map.renderViewportResolutionMaxRTWidth);

					if (!_map.renderViewportIsTerrain) {
						_map.renderViewportLightingMode = (VIEWPORT_LIGHTING_MODE)EditorGUILayout.EnumPopup (new GUIContent ("Lighting", "Lighting mode used for render viewport in scene."), _map.renderViewportLightingMode);
						_map.renderViewportCurvature = EditorGUILayout.Slider (new GUIContent ("Curvature Max Zoom", "Curvature of the render viewport at max zoom. 0 = disabled."), _map.renderViewportCurvature, -100f, 100f);
						_map.renderViewportCurvatureMinZoom = EditorGUILayout.Slider (new GUIContent ("Curvature Min Zoom", "Curvature of the render viewport at min zoom."), _map.renderViewportCurvatureMinZoom, -100f, 100f);
					}

					_map.renderViewportRenderingPath = (RenderingPath)EditorGUILayout.EnumPopup ("Rendering Path", _map.renderViewportRenderingPath);
					_map.renderViewportFilterMode = (FilterMode)EditorGUILayout.EnumPopup ("Filter Mode", _map.renderViewportFilterMode);

					if (_map.renderViewportIsTerrain) {
						Terrain terrain = _map.renderViewport.GetComponent<Terrain> ();
						if (terrain != null) {
							if (terrain.terrainData == null) {
								EditorGUILayout.HelpBox ("Terrain lacks TerrainData.", MessageType.Warning);
							}
						}
						if (defaultTerrainData != null && (terrain.terrainData == null || terrain.terrainData != defaultTerrainData)) {
							if (GUILayout.Button ("Assign Default World Map Heightmap")) {
								terrain.terrainData = defaultTerrainData;
								_map.Redraw ();
								EditorGUIUtility.ExitGUI ();
							}
						}
						_map.renderViewportTerrainAlpha = EditorGUILayout.Slider (new GUIContent ("   Alpha", "Transparency for the WMSK texture projected onto the Unity terrain."), _map.renderViewportTerrainAlpha, 0f, 1f);
					} else {
						_map.earthElevation = EditorGUILayout.Slider ("Ground Elevation", _map.earthElevation, 0, 2.0f);
						_map.renderViewportGOAutoScaleMultiplier = EditorGUILayout.Slider (new GUIContent ("Unit Scale Multiplier", "Scale multiplier applied to all game objects put on top of the viewport."), _map.renderViewportGOAutoScaleMultiplier, 0.1f, 10f);
						_map.renderViewportGOAutoScaleMin = EditorGUILayout.Slider (new GUIContent ("   Minimum Scale", "Scale multiplier applied to all game objects put on top of the viewport."), _map.renderViewportGOAutoScaleMin, 0.1f, 100f);
						_map.renderViewportGOAutoScaleMax = EditorGUILayout.Slider (new GUIContent ("   Maximum Scale", "Scale multiplier applied to all game objects put on top of the viewport."), _map.renderViewportGOAutoScaleMax, 0.1f, 100f);
						_map.VGOGlobalSpeed = EditorGUILayout.Slider ("Unit Speed Factor", _map.VGOGlobalSpeed, 0.1f, 3f);

						_map.earthCloudLayer = EditorGUILayout.Toggle ("Cloud Layer", _map.earthCloudLayer);
						if (_map.earthCloudLayer) {
							_map.earthCloudLayerSpeed = EditorGUILayout.Slider ("Speed", _map.earthCloudLayerSpeed, -5f, 5f);
							_map.earthCloudLayerElevation = -EditorGUILayout.Slider ("   Elevation", -_map.earthCloudLayerElevation, 0.1f, 30.0f);
							_map.earthCloudLayerAlpha = EditorGUILayout.Slider ("   Brightness", _map.earthCloudLayerAlpha, 0f, 2f);
							_map.earthCloudLayerShadowStrength = EditorGUILayout.Slider ("   Shadow Strength", _map.earthCloudLayerShadowStrength, 0f, 1f);
						}
						_map.fogOfWarLayer = EditorGUILayout.Toggle ("Fog of War", _map.fogOfWarLayer);
						if (_map.fogOfWarLayer) {
							_map.fogOfWarColor = EditorGUILayout.ColorField ("   Color", _map.fogOfWarColor);
							_map.fogOfWarLayerElevation = -EditorGUILayout.Slider ("   Layer Elevation", -_map.fogOfWarLayerElevation, 0f, 1f);
						}

						_map.VGOBuoyancyAmplitude = EditorGUILayout.Slider (new GUIContent ("Buoyancy Amplitude", "Rotation amount for the buoyancy effect of naval units. Set to zero to deactivate."), _map.VGOBuoyancyAmplitude, 0f, 2f);
						if (_map.VGOBuoyancyAmplitude > 0) {
							_map.VGOBuoyancyMaxZoomLevel = EditorGUILayout.Slider (new GUIContent ("   Max Zoom Level", "Maximum zoom level to visualize buoyancy effect."), _map.VGOBuoyancyMaxZoomLevel, 0f, 0.25f);
						}
					}
				} 
												
				_map.sun = (GameObject)EditorGUILayout.ObjectField (new GUIContent ("Sun", "Assign a Game Object (usually a Directional Light that acts as the Sun) to automatically synchronize the light direction with the time of day parameter below."), _map.sun, typeof(GameObject), true);
				if (_map.sun != null) {
					_map.sunUseTimeOfDay = EditorGUILayout.Toggle ("   Use Time Of Day", _map.sunUseTimeOfDay);
					if (_map.sunUseTimeOfDay) {
						_map.timeOfDay = EditorGUILayout.Slider ("   Time Of Day", _map.timeOfDay, 0f, 24f);
					}
				}
			}
			
			EditorGUILayout.EndVertical (); 
			EditorGUILayout.Separator ();

			EditorGUILayout.BeginVertical ();
			EditorGUILayout.BeginHorizontal ();
			expandEarthSection = EditorGUILayout.Foldout (expandEarthSection, "Earth Settings", sectionHeaderStyle);
			EditorGUILayout.EndHorizontal ();

			if (expandEarthSection) {
				_map.showEarth = EditorGUILayout.Toggle ("Show Earth", _map.showEarth);

				if (_map.showEarth) {
					EditorGUILayout.BeginHorizontal ();
					_map.earthStyle = (EARTH_STYLE)EditorGUILayout.IntPopup ("Style", (int)_map.earthStyle, earthStyleOptions, earthStyleValues);
					if (GUILayout.Button ("Reset", GUILayout.Width (60))) {
						_map.ResetEarthStyle ();
					}
					EditorGUILayout.EndHorizontal ();

					if (_map.earthStyle == EARTH_STYLE.SolidColor || _map.earthStyle == EARTH_STYLE.Texture) {
						_map.earthColor = EditorGUILayout.ColorField ("   Tint Color", _map.earthColor);
					}
					if (_map.earthStyle == EARTH_STYLE.Texture) {
						_map.earthTexture = (Texture2D)EditorGUILayout.ObjectField ("   World Texture", _map.earthTexture, typeof(Texture2D), false);
					}

					if (_map.earthStyle.isScenicPlus ()) {
						_map.waterColor = EditorGUILayout.ColorField ("   Water Color", _map.waterColor);
						_map.waterLevel = EditorGUILayout.Slider ("   Water Level", _map.waterLevel, 0.001f, 1f);
						_map.waterFoamThreshold = EditorGUILayout.Slider ("   Foam Threshold", _map.waterFoamThreshold, 0f, 0.2f);
						_map.waterFoamIntensity = EditorGUILayout.Slider ("   Foam Intensity", _map.waterFoamIntensity, 0f, 100f);
					}
					if (_map.earthStyle.supportsBumpMap ()) {
						_map.earthBumpEnabled = EditorGUILayout.Toggle ("NormalMap Enabled", _map.earthBumpEnabled);
						if (_map.earthBumpEnabled) {
							_map.earthBumpAmount = EditorGUILayout.Slider ("   Amount", _map.earthBumpAmount, 0, 1f);
						}
					}
				}

				_map.showLatitudeLines = EditorGUILayout.Toggle ("Show Latitude Lines", _map.showLatitudeLines);
				if (_map.showLatitudeLines) {
					_map.latitudeStepping = EditorGUILayout.IntSlider ("   Stepping", _map.latitudeStepping, 5, 45);
				}
				_map.showLongitudeLines = EditorGUILayout.Toggle ("Show Longitude Lines", _map.showLongitudeLines);
				if (_map.showLongitudeLines) {
					_map.longitudeStepping = EditorGUILayout.IntSlider ("   Stepping", _map.longitudeStepping, 5, 45);
				}
				if (_map.showLatitudeLines || _map.showLongitudeLines) {
					_map.imaginaryLinesColor = EditorGUILayout.ColorField ("   Lines Color", _map.imaginaryLinesColor);
				}
			}

			EditorGUILayout.EndVertical (); 
			EditorGUILayout.Separator ();

			EditorGUILayout.BeginVertical ();
			EditorGUILayout.BeginHorizontal ();
			expandCitiesSection = EditorGUILayout.Foldout (expandCitiesSection, "Cities Settings", sectionHeaderStyle);
			EditorGUILayout.EndHorizontal ();
			
			if (expandCitiesSection) {
				_map.showCities = EditorGUILayout.Toggle ("Show Cities", _map.showCities);

				if (_map.showCities) {
					_map.citiesColor = EditorGUILayout.ColorField ("Cities Color", _map.citiesColor);
					_map.citiesRegionCapitalColor = EditorGUILayout.ColorField ("Region Capital", _map.citiesRegionCapitalColor);
					_map.citiesCountryCapitalColor = EditorGUILayout.ColorField ("Country Capital", _map.citiesCountryCapitalColor);
					_map.cityIconSize = EditorGUILayout.Slider ("Icon Size", _map.cityIconSize, 0.1f, 5.0f);
					EditorGUILayout.BeginHorizontal ();
					_map.minPopulation = EditorGUILayout.IntSlider ("Min Population (K)", _map.minPopulation, 0, 3000);
					GUILayout.Label (_map.numCitiesDrawn + "/" + _map.cities.Count);
					EditorGUILayout.EndHorizontal ();

					GUILayout.Label ("Always Visible:");

					int cityClassFilter = 0;
					bool cityBit;
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("", GUILayout.Width(10));
					cityBit = EditorGUILayout.ToggleLeft ("Region Capitals", (_map.cityClassAlwaysShow & WMSK.CITY_CLASS_FILTER_REGION_CAPITAL) != 0);
					EditorGUILayout.EndHorizontal ();
					if (cityBit) {
						cityClassFilter += WMSK.CITY_CLASS_FILTER_REGION_CAPITAL;
					}
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField ("", GUILayout.Width(10));
					cityBit = EditorGUILayout.ToggleLeft ("Country Capitals", (_map.cityClassAlwaysShow & WMSK.CITY_CLASS_FILTER_COUNTRY_CAPITAL) != 0);
					EditorGUILayout.EndHorizontal ();
					if (cityBit) {
						cityClassFilter += WMSK.CITY_CLASS_FILTER_COUNTRY_CAPITAL;
					}
					_map.cityClassAlwaysShow = cityClassFilter;

					_map.citySpot = (GameObject)EditorGUILayout.ObjectField ("Normal City Icon", _map.citySpot, typeof(GameObject), false);
					_map.citySpotCapitalRegion = (GameObject)EditorGUILayout.ObjectField ("Region Capital Icon", _map.citySpotCapitalRegion, typeof(GameObject), false);
					_map.citySpotCapitalCountry = (GameObject)EditorGUILayout.ObjectField ("Country Capital Icon", _map.citySpotCapitalCountry, typeof(GameObject), false);
				}
			}

			EditorGUILayout.EndVertical (); 
			EditorGUILayout.Separator ();

			EditorGUILayout.BeginVertical ();
			EditorGUILayout.BeginHorizontal ();
			expandCountriesSection = EditorGUILayout.Foldout (expandCountriesSection, "Countries Settings", sectionHeaderStyle);
			EditorGUILayout.EndHorizontal ();
			
			if (expandCountriesSection) {
				_map.showFrontiers = EditorGUILayout.Toggle ("Show Countries", _map.showFrontiers);

				if (_map.showFrontiers) {
					EditorGUILayout.BeginHorizontal ();
					_map.frontiersDetail = (FRONTIERS_DETAIL)EditorGUILayout.Popup ("   Frontiers Detail", (int)_map.frontiersDetail, frontiersDetailOptions);
					GUILayout.Label (_map.countries.Length.ToString ());
					EditorGUILayout.EndHorizontal ();

					_map.frontiersCoastlines = EditorGUILayout.Toggle ("   Coastlines", _map.frontiersCoastlines);

					_map.frontiersColor = EditorGUILayout.ColorField ("   Frontiers Color", _map.frontiersColor);
					if (!_map.thickerFrontiers && !_map.renderViewportIsTerrain) {
						_map.frontiersColorOuter = EditorGUILayout.ColorField ("   Outer Color", _map.frontiersColorOuter);
					}
					_map.thickerFrontiers = EditorGUILayout.Toggle ("   Thicker Borders", _map.thickerFrontiers);
					if (_map.thickerFrontiers) {
						_map.frontiersWidth = EditorGUILayout.Slider ("   Width", _map.frontiersWidth, 0.01f, 1f);
					}
					if (!_map.renderViewportIsTerrain) {
						_map.frontiersDynamicWidth = EditorGUILayout.Toggle (new GUIContent ("   Dynamic Width", "Changes country frontiers width when zooming in/out"),_map.frontiersDynamicWidth);
					}
				}

				_map.enableCountryHighlight = EditorGUILayout.Toggle ("Country Highlight", _map.enableCountryHighlight);
				if (_map.enableCountryHighlight) {
					_map.fillColor = EditorGUILayout.ColorField ("   Highlight Color", _map.fillColor);
					_map.highlightMaxScreenAreaSize = EditorGUILayout.Slider (new GUIContent ("   Max Screen Size", "Defines the maximum screen area of a highlighted country. To prevent filling the whole screen with the highlight color, you can reduce this value and if the highlighted screen area size is greater than this factor (1=whole screen) the country won't be filled at all (it will behave as selected though)"), _map.highlightMaxScreenAreaSize, 0, 1f);
					_map.highlightAllCountryRegions = EditorGUILayout.Toggle ("   Include All Regions", _map.highlightAllCountryRegions);

					_map.showOutline = EditorGUILayout.Toggle ("   Draw Outline", _map.showOutline);
					if (_map.showOutline) {
						_map.outlineColor = EditorGUILayout.ColorField ("      Outline Color", _map.outlineColor);
						EditorGUILayout.BeginHorizontal ();
						_map.outlineDetail = (OUTLINE_DETAIL)EditorGUILayout.EnumPopup ("      Detail", _map.outlineDetail);
						if (GUILayout.Button ("?", GUILayout.Width (25))) {
							EditorUtility.DisplayDialog ("Outline Detail", "Simple: uses an optimized shader that draws 1-pixel width single color lines.\n\nTextured: uses a more complex method that accepts custom width and transparent textures. To use another texture, assign it to the OutlineMatTex material located in Resources/WMSK/Material folder.", "Ok");
						}
						EditorGUILayout.EndHorizontal ();
						if (_map.outlineDetail == OUTLINE_DETAIL.Textured) {
							EditorGUILayout.BeginHorizontal ();
							_map.outlineTexture = (Texture2D)EditorGUILayout.ObjectField ("      Texture", _map.outlineTexture, typeof(Texture2D), false);
							if (GUILayout.Button ("?", GUILayout.Width (25))) {
								EditorUtility.DisplayDialog ("Outline Texture", "Choose a factory line texture from Resources/WMSK/Textures/Outline folder or assign a custom texture from your own (recommended max size of 32 pixels).", "Ok");
							}
							EditorGUILayout.EndHorizontal ();
							_map.outlineTilingScale = EditorGUILayout.Slider ("      Tiling", _map.outlineTilingScale, 0.01f, 20f);
							_map.outlineWidth = EditorGUILayout.Slider ("      Width", _map.outlineWidth, 0.01f, 5f);
						}
						_map.outlineAnimationSpeed = EditorGUILayout.Slider ("      Animation Speed", _map.outlineAnimationSpeed, -5f, 5f);
					}
				}

				_map.showCountryNames = EditorGUILayout.Toggle ("Show Country Names", _map.showCountryNames);

				if (_map.showCountryNames) {
					EditorGUILayout.BeginHorizontal ();
					_map.countryLabelsTextEngine = (TEXT_ENGINE)EditorGUILayout.EnumPopup ("   Text Engine", _map.countryLabelsTextEngine);
					if (GUILayout.Button ("?", GUILayout.Width (20))) {
						EditorUtility.DisplayDialog ("TextMesh Pro integration", "Please check the manual for instructions on how to enable this integration.", "Ok");
					}
					EditorGUILayout.EndHorizontal ();

					_map.countryLabelsSize = EditorGUILayout.Slider ("   Size", _map.countryLabelsSize, 0.01f, 0.9f);
					_map.countryLabelsAbsoluteMinimumSize = EditorGUILayout.Slider ("   Minimum Size", _map.countryLabelsAbsoluteMinimumSize, 0.01f, 2.5f);

					if (_map.countryLabelsTextEngine == TEXT_ENGINE.TextMeshStandard) {
						EditorGUILayout.BeginHorizontal ();
						_map.countryLabelsFont = (Font)EditorGUILayout.ObjectField ("   Font", _map.countryLabelsFont, typeof(Font), false);
						if (GUILayout.Button ("Unicode")) {
							EditorUtility.DisplayDialog ("Unicode Support", "The characters included in the font are determined by the Font Importer settings. Please select the font and customize the import settings to match your requirements (eg. Unicode or special characters support.", "Ok");
						}
						EditorGUILayout.EndHorizontal ();
					} else {
						_map.countryLabelsFontTMPro = (Object)EditorGUILayout.ObjectField (new GUIContent ("   Font (TMPro)", "Font asset to be used with TextMesh Pro - see manual"), _map.countryLabelsFontTMPro, typeof(UnityEngine.Object), false);
						#if UNITY_2018_2_OR_NEWER
                        if (_map.countryLabelsFontTMPro == null) {
                            EditorGUILayout.HelpBox("Please assign an SDF Font to World Map Strategy Kit inspector. You can create SDF Fonts using TextMesh Pro Font Asset creator tool.", MessageType.Warning);
                        }
						#endif
					}

					if (!_map.renderViewportIsEnabled) {
						_map.labelsElevation = EditorGUILayout.FloatField ("   Elevation", _map.labelsElevation);
					}

					_map.countryLabelsColor = EditorGUILayout.ColorField ("   Labels Color", _map.countryLabelsColor);

					if (_map.countryLabelsTextEngine == TEXT_ENGINE.TextMeshStandard) {
						_map.showLabelsShadow = EditorGUILayout.Toggle ("   Draw Shadow", _map.showLabelsShadow);
						if (_map.showLabelsShadow) {
							_map.countryLabelsShadowColor = EditorGUILayout.ColorField ("      Shadow Color", _map.countryLabelsShadowColor);
						}
					} else {
						_map.countryLabelsLength = EditorGUILayout.Slider ("   Length", _map.countryLabelsLength, 0.1f, 1f);
						_map.countryLabelsHorizontality = EditorGUILayout.Slider ("   Horizontality", _map.countryLabelsHorizontality, 0.25f, 4f);
						_map.countryLabelsCurvature = EditorGUILayout.Slider ("   Curve Multiplier", _map.countryLabelsCurvature, 0, 2f);
						_map.countryLabelsOutlineColor = EditorGUILayout.ColorField ("   Outline Color", _map.countryLabelsOutlineColor);
						_map.countryLabelsOutlineWidth = EditorGUILayout.Slider ("   Outline Width", _map.countryLabelsOutlineWidth, 0, 0.5f);
					}

					_map.countryLabelsEnableAutomaticFade = EditorGUILayout.Toggle ("   Auto Fade Labels", _map.countryLabelsEnableAutomaticFade);

					if (_map.countryLabelsEnableAutomaticFade) {
						_map.countryLabelsAutoFadeMinHeight = EditorGUILayout.Slider ("      Min Height", _map.countryLabelsAutoFadeMinHeight, 0.01f, 0.25f);
						_map.countryLabelsAutoFadeMinHeightFallOff = EditorGUILayout.Slider ("      Min Height Fall Off", _map.countryLabelsAutoFadeMinHeightFallOff, 0.001f, _map.countryLabelsAutoFadeMinHeight);
						_map.countryLabelsAutoFadeMaxHeight = EditorGUILayout.Slider ("      Max Height", _map.countryLabelsAutoFadeMaxHeight, 0.1f, 1.0f);
						_map.countryLabelsAutoFadeMaxHeightFallOff = EditorGUILayout.Slider ("      Max Height Fall Off", _map.countryLabelsAutoFadeMaxHeightFallOff, 0.01f, 1f);
					}

				}
			}
			EditorGUILayout.EndVertical (); 
			EditorGUILayout.Separator ();

			EditorGUILayout.BeginVertical ();
			EditorGUILayout.BeginHorizontal ();
			expandProvincesSection = EditorGUILayout.Foldout (expandProvincesSection, "Provinces Settings", sectionHeaderStyle);
			EditorGUILayout.EndHorizontal ();
			
			if (expandProvincesSection) {
				_map.showProvinces = EditorGUILayout.Toggle ("Show Provinces", _map.showProvinces);
				if (_map.showProvinces) {
					_map.provincesCoastlines = EditorGUILayout.Toggle ("   Coastlines", _map.provincesCoastlines);
					_map.drawAllProvinces = EditorGUILayout.Toggle ("   Draw All Provinces", _map.drawAllProvinces);
					_map.provincesColor = EditorGUILayout.ColorField ("   Borders Color", _map.provincesColor);
				}

				_map.enableProvinceHighlight = EditorGUILayout.Toggle ("Province Highlight", _map.enableProvinceHighlight);
				if (_map.enableProvinceHighlight) {
					_map.provincesFillColor = EditorGUILayout.ColorField ("   Highlight Color", _map.provincesFillColor);
					_map.highlightAllProvinceRegions = EditorGUILayout.Toggle ("   Include All Regions", _map.highlightAllProvinceRegions);
				}
				_map.showProvinceNames = EditorGUILayout.Toggle ("Show Province Names", _map.showProvinceNames);

				if (_map.showProvinceNames) {
					_map.provinceLabelsVisibility = (PROVINCE_LABELS_VISIBILITY)EditorGUILayout.EnumPopup ("   Visibility Control", _map.provinceLabelsVisibility);
					if (_map.provinceLabelsVisibility == PROVINCE_LABELS_VISIBILITY.Automatic) {
						_map.showAllCountryProvinceNames = EditorGUILayout.Toggle ("   All Country Provinces", _map.showAllCountryProvinceNames);
					}

					_map.provinceLabelsSize = EditorGUILayout.Slider ("   Relative Size", _map.provinceLabelsSize, 0.01f, 0.9f);
					_map.provinceLabelsAbsoluteMinimumSize = EditorGUILayout.Slider ("   Minimum Size", _map.provinceLabelsAbsoluteMinimumSize, 0.01f, 2.5f);

					EditorGUILayout.BeginHorizontal ();
					_map.provinceLabelsFont = (Font)EditorGUILayout.ObjectField ("   Font", _map.provinceLabelsFont, typeof(Font), false);
					if (GUILayout.Button ("Unicode")) {
						EditorUtility.DisplayDialog ("Unicode Support", "The characters included in the font are determined by the Font Importer settings. Please select the font and customize the import settings to match your requirements (eg. Unicode or special characters support.", "Ok");
					}
					EditorGUILayout.EndHorizontal ();

					_map.provinceLabelsColor = EditorGUILayout.ColorField ("   Labels Color", _map.provinceLabelsColor);

					_map.showProvinceLabelsShadow = EditorGUILayout.Toggle ("   Draw Shadow", _map.showProvinceLabelsShadow);
					if (_map.showProvinceLabelsShadow) {
						_map.provinceLabelsShadowColor = EditorGUILayout.ColorField ("      Shadow Color", _map.provinceLabelsShadowColor);
					}
				}
			}

			EditorGUILayout.EndVertical (); 
			EditorGUILayout.Separator ();

			EditorGUILayout.BeginVertical ();
			expandTilesSection = EditorGUILayout.Foldout (expandTilesSection, "Tile System Settings", sectionHeaderStyle); 
			if (expandTilesSection) {
				
				EditorGUILayout.BeginHorizontal ();
				_map.showTiles = EditorGUILayout.Toggle ("Show Tiles",_map.showTiles);

				if (_map.showTiles) {
					if (!Application.isPlaying) {
						GUI.enabled = false;
					}
					if (GUILayout.Button ("Reload Tiles")) {
						_map.ResetTiles ();
					}
					GUI.enabled = true;
					EditorGUILayout.EndHorizontal ();
					float x = _map.transform.localScale.x;
					if (x < 2000f && !Mathf.Approximately (2000f, x) && _map.currentCamera != null) {
						EditorGUILayout.HelpBox ("To allow higher zoom levels, it's recommended to use a greater scale for the map gameobject. Do you want to switch the scale of Map gameobject to 2000 x 1000 ?", MessageType.Info);
						EditorGUILayout.BeginHorizontal ();
						if (GUILayout.Button ("Change Map Scale")) {
							_map.transform.localScale = new Vector3 (2000, 1000, 1f);
							_map.currentCamera.farClipPlane = _map.currentCamera.farClipPlane < 1000 ? 1000f : _map.currentCamera.farClipPlane;
							;
							_map.CenterMap ();
						}
						EditorGUILayout.EndHorizontal ();
					}
					_map.tileServer = (TILE_SERVER)EditorGUILayout.IntPopup ("Server", (int)_map.tileServer, WMSK.tileServerNames, WMSK.tileServerValues);

					if (_map.tileServer == TILE_SERVER.Custom) {
						GUILayout.Label ("Url Template");
						_map.tileServerCustomUrl = EditorGUILayout.TextField (_map.tileServerCustomUrl);
						EditorGUILayout.HelpBox ("Use:\n$N$ for random [a-c] node (optional)\n$Z$ for zoom level (required)\n$X$ and $Y$ for X/Y tile indices (required).", MessageType.Info);
					} else if (_map.tileServer == TILE_SERVER.AerisWeather) {
						GUILayout.Label ("Copyright Notice");
						EditorGUILayout.SelectableLabel (_map.tileServerCopyrightNotice);
						_map.tileServerClientId = EditorGUILayout.TextField (new GUIContent ("Client Id", "The client id of your Aeris Weather account."), _map.tileServerClientId);
						_map.tileServerAPIKey = EditorGUILayout.TextField (new GUIContent ("Secret Key", "Secret key linked to your Aeris Weather account."), _map.tileServerAPIKey);
						_map.tileServerLayerTypes = EditorGUILayout.TextField (new GUIContent ("Layer Types", "Enter the desired layer types (eg: radar,radar-2m,fradar,satellite-visible,satellite,satellite-infrared-color,satellite-water-vapor,fsatellite"), _map.tileServerLayerTypes);
						if (string.IsNullOrEmpty (_map.tileServerClientId) || string.IsNullOrEmpty (_map.tileServerAPIKey) || string.IsNullOrEmpty (_map.tileServerLayerTypes)) {
							EditorGUILayout.HelpBox ("To access AerisWeather service, ClientId, SecretKey as well as one or more Layer Types must be specified.", MessageType.Warning);
						}
						_map.tileServerTimeOffset = EditorGUILayout.TextField (new GUIContent ("Time Offset", "Enter the map time offset from now (eg. current or -10min or +1hour) or an exact date with format: YYYYMMDDhhiiss."), _map.tileServerTimeOffset);
					} else {
						GUILayout.Label ("Copyright Notice");
						EditorGUILayout.SelectableLabel (_map.tileServerCopyrightNotice);
						_map.tileServerAPIKey = EditorGUILayout.TextField (new GUIContent ("API Key", "Custom portion added to tile request url. For example: apikey=1234589"), _map.tileServerAPIKey);
					}

					_map.tileResolutionFactor = EditorGUILayout.Slider (new GUIContent ("Tile Resolution", "A value of 2 provides de best quality whereas a lower value will reduce downloads."),_map.tileResolutionFactor, 1f, 2f);
					_map.tileMaxZoomLevel = EditorGUILayout.IntSlider (new GUIContent ("Max Zoom Level", "Allowed maximum zoom level. Also check Zoom Distance Min under Interaction section."), _map.tileMaxZoomLevel, WMSK.TILE_MIN_ZOOM_LEVEL, WMSK.TILE_MAX_ZOOM_LEVEL);
					_map.tileLinesMaxZoomLevel = EditorGUILayout.IntSlider (new GUIContent ("Lines Max Zoom Level", "Allowed maximum zoom level for drawing frontiers and other lines. Provided frontiers are computed for a certain detail level. When zooming in too much, the lines are no longer accurate therefore can be omitted."), _map.tileLinesMaxZoomLevel, WMSK.TILE_MIN_ZOOM_LEVEL, WMSK.TILE_MAX_ZOOM_LEVEL);
					_map.tileTransparentLayer = EditorGUILayout.Toggle (new GUIContent ("Transparent Tiles", "Enable this option to render tiles with transparency (slower)."), _map.tileTransparentLayer);
					if (_map.tileTransparentLayer) {
						_map.tileMaxAlpha = EditorGUILayout.Slider (new GUIContent ("   Max Alpha", "Maximum level of opacity."), _map.tileMaxAlpha, 0, 1f);
					}

					_map.tileMaxConcurrentDownloads = EditorGUILayout.IntField (new GUIContent ("Max Concurrent Downloads", "Maximum number of web downloads at any given time."), _map.tileMaxConcurrentDownloads);
					_map.tileMaxTileLoadsPerFrame = EditorGUILayout.IntField (new GUIContent ("Max Loads Per Frame", "Maximum number of tiles showing up per frame."), _map.tileMaxTileLoadsPerFrame);
					_map.tilePreloadTiles = EditorGUILayout.Toggle (new GUIContent ("Preload Main Tiles", "Enable this option to quickly load from local cache all tiles belonging to first zoom level (Local cache must be enabled)."), _map.tilePreloadTiles);
					_map.tileKeepAlive = EditorGUILayout.FloatField (new GUIContent ("Tiles Keep Alive", "Time in seconds to keep an inactive/hidden tile in memory before releasing it."), _map.tileKeepAlive);
					_map.tileDebugErrors = EditorGUILayout.Toggle ("Show Console Errors", _map.tileDebugErrors);
					_map.tileEnableLocalCache = EditorGUILayout.Toggle ("Enable Local Cache", _map.tileEnableLocalCache);

					if (_map.tileEnableLocalCache) {
						_map.tileMaxLocalCacheSize = EditorGUILayout.LongField ("   Cache Size (Mb)", _map.tileMaxLocalCacheSize);
					}

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("Cache Usage", GUILayout.Width (120));
					if (tileSizeComputed) {
						GUILayout.Label ((_map.tileCurrentCacheUsage / (1024f * 1024f)).ToString ("F1") + " Mb");
					}
					if (GUILayout.Button ("Recalculate")) {
						_map.TileRecalculateCacheUsage ();
						tileSizeComputed = true;
						GUIUtility.ExitGUI ();
					}
					if (GUILayout.Button ("Purge")) {
						_map.PurgeTileCache ();
					}
					EditorGUILayout.EndHorizontal ();

					_map.tileEnableOfflineTiles = EditorGUILayout.Toggle ("Enable Offline Tiles", _map.tileEnableOfflineTiles);
					if (_map.tileEnableOfflineTiles) {
						_map.tileResourcePathBase = EditorGUILayout.TextField ("   Resources Path", _map.tileResourcePathBase);
						_map.tileOfflineTilesOnly = EditorGUILayout.Toggle (new GUIContent ("   Only Offline Tiles", "If enabled, only existing tiles from Resources path will be loaded - cache and online tiles will be ignored."), _map.tileOfflineTilesOnly);
						if (_map.tileEnableOfflineTiles) {
							_map.tileResourceFallbackTexture = (Texture2D)EditorGUILayout.ObjectField (new GUIContent ("   Fallback Texture", "Fallback texture if the tile is not found in Resources path."), _map.tileResourceFallbackTexture, typeof(Texture2D), false);
						}

						if (GUILayout.Button ("Open Tiles Downloader")) {
							WMSKTilesDownloader.ShowWindow ();
						}
					}

				} else {
					EditorGUILayout.EndHorizontal ();
				}

			}

			EditorGUILayout.EndVertical (); 
			EditorGUILayout.Separator ();

			EditorGUILayout.BeginVertical ();
			EditorGUILayout.BeginHorizontal ();
			expandInteractionSection = EditorGUILayout.Foldout (expandInteractionSection, "Interaction Settings", sectionHeaderStyle);
			EditorGUILayout.EndHorizontal ();
			
			if (expandInteractionSection) {

				_map.showCursor = EditorGUILayout.Toggle ("Show Cursor", _map.showCursor);
				if (_map.showCursor) {
					_map.cursorColor = EditorGUILayout.ColorField ("   Cursor Color", _map.cursorColor);
					_map.cursorFollowMouse = EditorGUILayout.Toggle ("   Follow Mouse", _map.cursorFollowMouse);
					_map.cursorAlwaysVisible = EditorGUILayout.Toggle ("   Always Visible", _map.cursorAlwaysVisible);
				}
				_map.respectOtherUI = EditorGUILayout.Toggle ("Respect Other UI", _map.respectOtherUI);

				_map.allowUserDrag = EditorGUILayout.Toggle ("Allow User Drag", _map.allowUserDrag);
				if (_map.allowUserDrag) {
					_map.mouseDragSensitivity = EditorGUILayout.Slider ("   Speed", _map.mouseDragSensitivity, 0.1f, 3);
					_map.mouseDragThreshold = EditorGUILayout.IntField (new GUIContent ("   Drag Threshold", "Enter a threshold value to avoid accidental map dragging when clicking on HiDpi screens. Values of 5, 10, 20 or more, depending on the sensitivity of the screen."), _map.mouseDragThreshold);
					_map.centerOnRightClick = EditorGUILayout.Toggle ("   Right Click Centers", _map.centerOnRightClick);
					_map.dragConstantSpeed = EditorGUILayout.Toggle ("   Constant Drag Speed", _map.dragConstantSpeed);
					_map.dragDampingDuration = EditorGUILayout.FloatField (new GUIContent("   Damping Duration", "The duration of the damping rotation after a drag until the Earth stops completely."), _map.dragDampingDuration);
					_map.allowScrollOnScreenEdges = EditorGUILayout.Toggle ("   Screen Edge Scroll", _map.allowScrollOnScreenEdges);
					if (_map.allowScrollOnScreenEdges) {
						_map.screenEdgeThickness = EditorGUILayout.IntSlider ("      Edge Thickness", _map.screenEdgeThickness, 1, 10);
					}
				}

				_map.allowUserKeys = EditorGUILayout.Toggle ("Allow Keys", _map.allowUserKeys);
				if (_map.allowUserKeys) {
					_map.dragKeySpeedMultiplier = EditorGUILayout.FloatField ("   Speed Multiplier", _map.dragKeySpeedMultiplier);
					_map.dragFlipDirection = EditorGUILayout.Toggle ("   Flip Direction", _map.dragFlipDirection);

					EditorGUILayout.BeginHorizontal ();
					keyPreset = (KEY_PRESET)EditorGUILayout.EnumPopup ("   Key Presets", keyPreset);
					if (GUILayout.Button ("Apply", GUILayout.Width (60))) {
						switch (keyPreset) {
						case KEY_PRESET.WASD:
							_map.keyUp = KeyCode.W;
							_map.keyDown = KeyCode.S;
							_map.keyLeft = KeyCode.A;
							_map.keyRight = KeyCode.D;
							break;
						case KEY_PRESET.ArrowKeys:
							_map.keyUp = KeyCode.UpArrow;
							_map.keyDown = KeyCode.DownArrow;
							_map.keyLeft = KeyCode.LeftArrow;
							_map.keyRight = KeyCode.RightArrow;
							break;
						}
					}
					EditorGUILayout.EndHorizontal ();
					_map.keyUp = (KeyCode)EditorGUILayout.EnumPopup ("   Up Key", _map.keyUp);
					_map.keyDown = (KeyCode)EditorGUILayout.EnumPopup ("   Down Key", _map.keyDown);
					_map.keyLeft = (KeyCode)EditorGUILayout.EnumPopup ("   Left Key", _map.keyLeft);
					_map.keyRight = (KeyCode)EditorGUILayout.EnumPopup ("   Right Key", _map.keyRight);
				}

				_map.allowUserZoom = EditorGUILayout.Toggle ("Allow User Zoom", _map.allowUserZoom);
				if (_map.allowUserZoom) {
					_map.mouseWheelSensitivity = EditorGUILayout.Slider ("   Speed", _map.mouseWheelSensitivity, 0.1f, 3);
					_map.invertZoomDirection = EditorGUILayout.Toggle ("   Invert Direction", _map.invertZoomDirection);
					_map.zoomMinDistance = EditorGUILayout.FloatField (new GUIContent ("Distance Min", "0 = default min distance. This is a multiplier to the height of the map."), _map.zoomMinDistance);
					_map.zoomMaxDistance = EditorGUILayout.FloatField (new GUIContent ("Max", "10m = default max distance. This is a multiplier to the height of the map."), _map.zoomMaxDistance);
				}

				_map.navigationTime = EditorGUILayout.Slider ("Navigation Time", _map.navigationTime, 0, 10);
			}

			EditorGUILayout.EndVertical (); 
			EditorGUILayout.Separator ();

			if (!_map.renderViewportIsTerrain) {
				EditorGUILayout.BeginVertical ();
				expandPathFindingSection = EditorGUILayout.Foldout (expandPathFindingSection, "Path Finding Settings", sectionHeaderStyle);
			
				if (expandPathFindingSection) {
					_map.pathFindingHeuristicFormula = (HeuristicFormula)EditorGUILayout.IntPopup ("Heuristic", (int)_map.pathFindingHeuristicFormula, pathFindingHeuristicOptions, pathFindingHeuristicValues);
					_map.pathFindingMaxCost = EditorGUILayout.IntField (new GUIContent ("Default Max Cost", "Maximum total route cost for any unit that doesn't have a specific pathFindingMaxCost value in the GameObjectAnimator."), _map.pathFindingMaxCost);
					_map.pathFindingMaxSteps = EditorGUILayout.IntField (new GUIContent ("Default Max Steps", "Maximum total steps for any unit that doesn't have a specific pathFindingMaxSteps value in the GameObjectAnimator."), _map.pathFindingMaxSteps);
					_map.waterMaskLevel = (byte)EditorGUILayout.IntSlider ("Water Level", (byte)_map.waterMaskLevel, 0, 255);
					_map.pathFindingVisualizeMatrixCost = EditorGUILayout.Toggle ("Show Matrix Cost", _map.pathFindingVisualizeMatrixCost);
				}
				EditorGUILayout.EndVertical (); 
				EditorGUILayout.Separator ();
			}
			
			EditorGUILayout.BeginVertical ();
			expandGrid = EditorGUILayout.Foldout (expandGrid, "Grid Settings", sectionHeaderStyle);

			if (expandGrid) {
				_map.showGrid = EditorGUILayout.Toggle ("Show Grid", _map.showGrid);
				if (_map.showGrid) {
					_map.gridColumns = EditorGUILayout.IntSlider ("Columns", _map.gridColumns, 32, 2048);
					_map.gridRows = EditorGUILayout.IntSlider ("Rows", _map.gridRows, 16, 1024);
					_map.gridColor = EditorGUILayout.ColorField ("Color", _map.gridColor);
					_map.gridAphaOnWater = EditorGUILayout.Slider ("Alpha On Water", _map.gridAphaOnWater, 0, 1f);
					_map.enableCellHighlight = EditorGUILayout.Toggle ("Enable Highlight", _map.enableCellHighlight);
					if (_map.enableCellHighlight) {
						_map.cellHighlightColor = EditorGUILayout.ColorField ("   Highlight Color", _map.cellHighlightColor);
						_map.highlightFadeAmount = EditorGUILayout.Slider ("   Highlight Fade", _map.highlightFadeAmount, 0.0f, 1.0f);
						_map.exclusiveHighlight = EditorGUILayout.Toggle (new GUIContent ("   Exclusive", "Enable this option to only highlight either a country/province or a cell, but not both"), _map.exclusiveHighlight);
					}
					_map.gridMinDistance = EditorGUILayout.FloatField ("Visible Distance Min", _map.gridMinDistance);
					_map.gridMaxDistance = EditorGUILayout.FloatField (new GUIContent ("Max", "10m = default max distance"), _map.gridMaxDistance);
				}
			}
			
			EditorGUILayout.EndVertical (); 
			EditorGUILayout.Separator ();

			EditorGUILayout.BeginVertical ();
			expandMiscellanea = EditorGUILayout.Foldout (expandMiscellanea, "Miscellanea", sectionHeaderStyle);

			if (expandMiscellanea) {
				EditorGUILayout.BeginHorizontal ();
				_map.customCamera = (Camera)EditorGUILayout.ObjectField ("Custom Camera", _map.customCamera, typeof(Camera), true);
				if (_map.customCamera == null) {
					GUILayout.Label ("(Using Main Camera)");
				}
				EditorGUILayout.EndHorizontal ();
				_map.prewarm = EditorGUILayout.Toggle (new GUIContent ("Prewarm At Start", "Precomputes big country surfaces and path finding matrices during initialization to allow smoother performance during play."), _map.prewarm);
				_map.enableEnclaves = EditorGUILayout.Toggle (new GUIContent ("Enable Enclaves", "Enclave: a portion of territory surrounded by a larger territory belonging to a different country."), _map.enableEnclaves);
				_map.dontLoadGeodataAtStart = EditorGUILayout.Toggle (new GUIContent ("Don't Load Geodata", "Skips geodata loading at start: map will be empty and you need to call ReloadData() to manually load geodata files or create map procedurally using API."), _map.dontLoadGeodataAtStart);
				EditorGUILayout.BeginHorizontal ();
				_map.geodataResourcesPath = EditorGUILayout.TextField (new GUIContent ("Geodata Folder", "Path after any Resources folder where geodata files reside."), _map.geodataResourcesPath);
				if (GUILayout.Button ("Show", GUILayout.Width(60))) {
					string path = _map.editor.GetGenerationMapOutputPath ();
					if (System.IO.Directory.Exists (path)) {
						UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath (_map.editor.GetGenerationMapOutputPath (), typeof(UnityEngine.Object));
						if (obj != null) {
							EditorGUIUtility.PingObject (obj);
						}
					} else {
						EditorUtility.DisplayDialog ("Geodata Folder", "Folder not found.", "Ok");
					}
				}
				EditorGUILayout.EndHorizontal ();
				_map.countryAttributeFile = EditorGUILayout.TextField ("Country Attribute File", _map.countryAttributeFile);
				_map.provinceAttributeFile = EditorGUILayout.TextField ("Province Attribute File", _map.provinceAttributeFile);
				_map.cityAttributeFile = EditorGUILayout.TextField ("City Attribute File", _map.cityAttributeFile);
				_map.earthTexture = (Texture2D)EditorGUILayout.ObjectField ("Custom World Texture", _map.earthTexture, typeof(Texture2D), false);
				_map.earthBumpMapTexture = (Texture2D)EditorGUILayout.ObjectField ("Custom NormalMap", _map.earthBumpMapTexture, typeof(Texture2D), false);
				_map.waterMask = (Texture2D)EditorGUILayout.ObjectField (new GUIContent ("Custom Water Mask", "Path and file name of the water mask (must reside inside any Resource folder, don't include Resource prefix as part of the path). A water mask is a height map (RGBA texture) where red channel contains the height of the terrain, green/blue channel contains water motion vectors and the alpha channel is used for foam effect. To use a different water mask texture, specify the location of the texture and process it using the 'Generate Water Motion Vectors' option from the Map Editor, which will modify the texture to add water motion vectors and foam data. The elevation in the red channel is also used for path finding calculations to determine if the pixel contains water or not."), _map.waterMask, typeof(Texture2D), false);
				_map.heightMapTexture = (Texture2D)EditorGUILayout.ObjectField (new GUIContent ("Custom Height Map", "Heightmap is used to draw elevation in Render Viewport and in pathfinding."), _map.heightMapTexture, typeof(Texture2D), false);
			}

			EditorGUILayout.EndVertical (); 

			// Extra components opener
			EditorGUILayout.Separator ();
			float buttonWidth = EditorGUIUtility.currentViewWidth * 0.4f;
			if (_map.gameObject.activeInHierarchy) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();

				if (_map.gameObject.GetComponent<WMSK_Calculator> () == null) {
					if (GUILayout.Button ("Open Calculator", GUILayout.Width (buttonWidth))) {
						_map.gameObject.AddComponent<WMSK_Calculator> ();
					}
				} else {
					if (GUILayout.Button ("Close Calculator", GUILayout.Width (buttonWidth))) {
						DestroyImmediate (_map.gameObject.GetComponent<WMSK_Calculator> ());
						EditorGUIUtility.ExitGUI ();
					}
				}

				if (_map.gameObject.GetComponent<WMSK_Ticker> () == null) {
					if (GUILayout.Button ("Open Ticker", GUILayout.Width (buttonWidth))) {
						_map.gameObject.AddComponent<WMSK_Ticker> ();
					}
				} else {
					if (GUILayout.Button ("Close Ticker", GUILayout.Width (buttonWidth))) {
						DestroyImmediate (_map.gameObject.GetComponent<WMSK_Ticker> ());
						EditorGUIUtility.ExitGUI ();
					}
				}
				GUILayout.FlexibleSpace ();
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (_map.gameObject.GetComponent<WMSK_Editor> () == null) {
					if (GUILayout.Button ("Open Map Editor", GUILayout.Width (buttonWidth))) {
						// Unity 5.3.1 prevents raycasting in the scene view if rigidbody is present
						_map.gameObject.AddComponent<WMSK_Editor> ();
						Rigidbody rb = _map.gameObject.GetComponent<Rigidbody> ();
						if (rb != null) {
							DestroyImmediate (rb);
							EditorGUIUtility.ExitGUI ();
							return;
						}
					}
				} else {
					if (GUILayout.Button ("Close Map Editor", GUILayout.Width (buttonWidth))) {
						_map.HideProvinces ();
						_map.HideCountrySurfaces ();
						_map.HideProvinceSurfaces ();
						_map.Redraw ();
						DestroyImmediate (_map.gameObject.GetComponent<WMSK_Editor> ());
						EditorGUIUtility.ExitGUI ();
					}
				}

				if (_map.gameObject.GetComponent<WMSK_Decorator> () == null) {
					if (GUILayout.Button ("Open Decorator", GUILayout.Width (buttonWidth))) {
						_map.gameObject.AddComponent<WMSK_Decorator> ();
					}
				} else {
					if (GUILayout.Button ("Close Decorator", GUILayout.Width (buttonWidth))) {
						DestroyImmediate (_map.gameObject.GetComponent<WMSK_Decorator> ());
						EditorGUIUtility.ExitGUI ();
					}
				}

				GUILayout.FlexibleSpace ();
				EditorGUILayout.EndHorizontal ();
			}

			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("About", GUILayout.Width (buttonWidth * 2f))) {
				WMSKAbout.ShowAboutWindow ();
			}
			GUILayout.FlexibleSpace ();
			EditorGUILayout.EndHorizontal ();

 
			if (_map.isDirty) {
#if UNITY_5_6_OR_NEWER
				serializedObject.UpdateIfRequiredOrScript();
#else
				serializedObject.UpdateIfDirtyOrScript ();
#endif
				isDirty.boolValue = false;
				serializedObject.ApplyModifiedProperties ();
				EditorUtility.SetDirty (target);
				UnityEditorInternal.InternalEditorUtility.RepaintAllViews ();
			}
		}


		void DrawWarningLabel (string s) {
			if (warningLabelStyle == null)
				warningLabelStyle = new GUIStyle (GUI.skin.label);
			warningLabelStyle.normal.textColor = EditorGUIUtility.isProSkin ? new Color (0.52f, 0.66f, 0.9f) : new Color (0.22f, 0.36f, 0.6f);
			GUILayout.Label (s, warningLabelStyle);
		}

		[MenuItem ("CONTEXT/WMSK/Tiles Downloader")]
		static void TilesDownloaderMenuOption (MenuCommand command) {
			WMSKTilesDownloader.ShowWindow ();
		}


	}

}