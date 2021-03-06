// World Map Strategy Kit for Unity - Main Script
// (C) 2016-2019 by Ramiro Oliva (Kronnect)
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WorldMapStrategyKit {

	public partial class WMSK : MonoBehaviour {

		#region Internal variables

		const string MAPPER_CAM = "WMSKMapperCam";
		const string MAPPER_CAM_WRAP = "WMSKMapperCamWrap";

		// resources
		Material fogOfWarMat;
		Dictionary<int, GameObjectAnimator> vgos;
		// viewport game objects

		// Overlay & Viewport
		RenderTexture overlayRT;
		Camera _currentCamera, _wrapCamera, mapperCam;
		GameObject _wrapCameraObj;

		// Terrain support
		Material terrainMat;
		Terrain terrain;
		bool _renderViewportIsTerrain;
		Vector3 lastMainCameraPos;
		Quaternion lastMainCameraRot;
		RaycastHit[] terrainHits;

		// Earth effects
		float earthLastElevation = -1;
		const int EARTH_ELEVATION_STRIDE = 256;
		const int EARTH_ELEVATION_WIDTH = EARTH_ELEVATION_STRIDE * 8;
		const int EARTH_ELEVATION_HEIGHT = EARTH_ELEVATION_STRIDE * 4;
		int viewportColliderNeedsUpdate;
		float[] viewportElevationPoints;
		Color[] heightMapColors;
		float renderViewportOffsetX, renderViewportOffsetY, renderViewportScaleX, renderViewportScaleY, _renderViewportElevationFactor;
		Vector3 renderViewportClip0, renderViewportClip1;
		float renderViewportClipWidth, renderViewportClipHeight;
		bool lastRenderViewportGood;
		float _renderViewportScaleFactor;
		Vector3 lastRenderViewportRotation, lastRenderViewportPosition;
		List<Region> extrudedRegions;
		Vector2[] viewportUV;
		Vector3[] viewportElevationPointsAdjusted;
		int[] viewportIndices;

		public float renderViewportScaleFactor { get { return renderViewportIsEnabled ? _renderViewportScaleFactor : 1f; } }

		public float renderViewportElevationFactor { get { return _renderViewportElevationFactor; } }

		// Water effects
		float buoyancyCurrentAngle;

		// Curvature
		Mesh quadPrefab, flexQuadPrefab, flexQuad;
		float flexQuadCurvature;
		float[] curvatureOffsets;
		float currentCurvature;


		public Camera currentCamera {
			get {
				if (_currentCamera == null) {
					SetupViewport ();
				}
				return _currentCamera;
			}
		}

		#endregion

		#region Viewport mesh building

		/// <summary>
		/// Build an extruded mesh for the viewport
		/// </summary>
		void EarthBuildMesh () {
			// Real Earth relief is only available when viewport is enabled
			if (_renderViewport == null || _renderViewport == gameObject)
				return;

			EarthGetElevationInfo ();

			EarthUpdateElevation ();
			earthLastElevation = _earthElevation;

			// Updates objects elevation
			UpdateViewportObjects ();
		}

		void EarthGetElevationInfo () {
			int size = EARTH_ELEVATION_WIDTH * EARTH_ELEVATION_HEIGHT;
			if (viewportElevationPoints == null || viewportElevationPoints.Length != size) {
				viewportElevationPoints = new float[size];

				// Get elevation info
				if (_heightMapTexture == null) {
					_heightMapTexture = Resources.Load<Texture2D> ("WMSK/Textures/EarthHeightMap");	// default
				}

				heightMapColors = _heightMapTexture.GetPixels ();
			} else if (earthLastElevation >= 0) { // data already loaded
				return;
			}

			float baseElevation = 24.0f / 255.0f;
			int tw = _heightMapTexture.width;
			int extrudedRegionCount = extrudedRegions != null ? extrudedRegions.Count : 0;

			if (extrudedRegionCount > 0) {
				Vector2 p;
				for (int j = 0; j < EARTH_ELEVATION_HEIGHT; j++) {
					int jj = j * EARTH_ELEVATION_WIDTH;
					int texjj = (j * _heightMapTexture.height / EARTH_ELEVATION_HEIGHT) * _heightMapTexture.width;
					p.y = (j + 0.5f) / EARTH_ELEVATION_HEIGHT - 0.5f;
					for (int k = 0; k < EARTH_ELEVATION_WIDTH; k++) {
						int pos = texjj + k * tw / EARTH_ELEVATION_WIDTH;
						float gCol = heightMapColors [pos].g - baseElevation;
						if (gCol < 0)
							gCol = 0;
						p.x = (k + 0.5f) / EARTH_ELEVATION_WIDTH - 0.5f;
						for (int e = 0; e < extrudedRegionCount; e++) {
							Region region = extrudedRegions [e];
							if (region.Contains (p)) {
								gCol = region.extrusionAmount;
								break;
							}
						}
						viewportElevationPoints [jj + k] = gCol;
					}
				}
			} else {
				for (int j = 0; j < EARTH_ELEVATION_HEIGHT; j++) {
					int jj = j * EARTH_ELEVATION_WIDTH;
					int texjj = (j * _heightMapTexture.height / EARTH_ELEVATION_HEIGHT) * _heightMapTexture.width;
					for (int k = 0; k < EARTH_ELEVATION_WIDTH; k++) {
						int pos = texjj + k * tw / EARTH_ELEVATION_WIDTH;
						float gCol = heightMapColors [pos].g - baseElevation;
						if (gCol < 0)
							gCol = 0;
						viewportElevationPoints [jj + k] = gCol;
					}
				}
			}

			// Remind to compute cell costs again
			cellsCostsComputed = false;

			// Create and assign a quad mesh
			MeshFilter mf = _renderViewport.GetComponent<MeshFilter> ();
			Mesh mesh = mf.sharedMesh;
			if (mesh == null) {
				mesh = new Mesh ();
				if (disposalManager != null)
					disposalManager.MarkForDisposal (mesh); // mesh.hideFlags = HideFlags.DontSave;
			}
			mesh.Clear ();
			mesh.vertices = new Vector3[] {
				new Vector2 (-0.5f, 0.5f),
				new Vector2 (0.5f, 0.5f),
				new Vector2 (0.5f, -0.5f),
				new Vector2 (-0.5f, -0.5f)
			};
			mesh.SetIndices (new int[] { 0, 1, 2, 3 }, MeshTopology.Quads, 0);
			mesh.uv = new Vector2[] {
				new Vector2 (0, 1),
				new Vector2 (1, 1),
				new Vector2 (1, 0),
				new Vector2 (0, 0)
			};
			mesh.RecalculateNormals ();
			mf.sharedMesh = mesh;
		}

		/// <summary>
		/// Similar to EarthGetElevationInfo but feeds from Terrain heightmap itself
		/// </summary>
		void TerrainGetElevationData () {
			if (viewportElevationPoints == null || viewportElevationPoints.Length == 0) {
				viewportElevationPoints = new float[EARTH_ELEVATION_WIDTH * EARTH_ELEVATION_HEIGHT];
			}

			if (terrain.terrainData == null) {
				Debug.LogError ("Terrain does not have heightmap information (TerrainData is missing!). For a world heightmap, you can use the TerrainData from the demo scenes of WMSK.");
				return;
			}
			int sizeX = terrain.terrainData.heightmapResolution;
			int sizeY = terrain.terrainData.heightmapResolution;
			float[,] heights = terrain.terrainData.GetHeights (0, 0, sizeX, sizeY);

			for (int j = 0; j < EARTH_ELEVATION_HEIGHT; j++) {
				int jj = j * EARTH_ELEVATION_WIDTH;
				int hj = j * sizeY / EARTH_ELEVATION_HEIGHT;
				for (int k = 0; k < EARTH_ELEVATION_WIDTH; k++) {
					int hk = k * sizeX / EARTH_ELEVATION_WIDTH;
					float gCol = heights [hj, hk];
					viewportElevationPoints [jj + k] = gCol;
				}
			}
		}

		void EarthUpdateElevation () {

			// Curvature
			currentCurvature = Mathf.Lerp (_renderViewportCurvatureMinZoom, _renderViewportCurvature, lastKnownZoomLevel);

			// Compute MIP
			int visibleElevationColumns = Mathf.FloorToInt (_renderViewportRect.width * EARTH_ELEVATION_WIDTH);
			if (visibleElevationColumns < 1)
				return;
			
			int mip = Mathf.CeilToInt (visibleElevationColumns / (float)EARTH_ELEVATION_STRIDE);
			int earthElevationHeight = EARTH_ELEVATION_HEIGHT;
			int earthElevationWidth = EARTH_ELEVATION_WIDTH;

			int arrayLength;

			// Get window rect
			float dy = renderViewportClipHeight / (float)earthElevationHeight;
			float dx = renderViewportClipWidth / (float)earthElevationWidth;
			int rmin = int.MaxValue;
			int rmax = int.MinValue;
			for (int j = 0; j < earthElevationHeight; j++) {
				float jf = (float)j;
				float j0 = renderViewportClip1.y + dy * jf;
				float j1 = renderViewportClip1.y + dy * (jf + 1.0f);
				if ((j0 >= 0f && j0 <= 1.0f) || (j1 >= 0f && j1 <= 1.0f) || (j0 < 0f && j1 > 1.0f)) {
					if (j < rmin)
						rmin = j;
					if (j > rmax)
						rmax = j;
				}
			}
			int cmin = int.MaxValue;
			int cmax = int.MinValue;
			int cols = _wrapHorizontally ? earthElevationWidth * 2 : earthElevationWidth;
			for (int k = 0; k < cols; k++) {
				float kf = (float)k;
				float k0 = renderViewportClip0.x + dx * kf;
				float k1 = renderViewportClip0.x + dx * (kf + 1.0f);
				if ((k0 >= 0f && k0 <= 1.0f) || (k1 >= 0f && k1 <= 1.0f) || (k0 < 0f && k1 > 1.0f)) {
					if (k < cmin)
						cmin = k;
					if (k > cmax)
						cmax = k;
				}
			}

			if (cmin >= cols)
				cmin = 0;
			if (rmin >= earthElevationHeight)
				rmin = 0;
			if (cmax < 0)
				cmax = cols - 1;
			if (rmax < 0)
				rmax = earthElevationHeight - 1;
			if (rmax < earthElevationHeight - 1)
				rmax++;
			if (cmax < cols - 1)
				cmax++;

			int cmin0 = cmin;
			int rmin0 = rmin;
			do {
				rmin = (rmin0 / mip) * mip;
				cmin = (cmin0 / mip) * mip;
				rmax = Mathf.CeilToInt (rmax / (float)mip) * mip;
				cmax = Mathf.CeilToInt (cmax / (float)mip) * mip;

				arrayLength = 0;
				int rangeY = (rmax - rmin) / mip + 1;
				int rangeX = (cmax - cmin) / mip + 1;
				arrayLength = Mathf.Max (rangeY * rangeX, 0);
				if (arrayLength > 65000)
					mip++;
			} while(arrayLength > 65000);

			// Compute surface vertices and uv
			_renderViewportScaleFactor = transform.localScale.y / (lastDistanceFromCamera + 1f);
			_renderViewportElevationFactor = _earthElevation * _renderViewportScaleFactor;

			int arrayIndex = -1;
			if (viewportUV == null || viewportUV.Length != arrayLength) {
				viewportUV = new Vector2[arrayLength];
				viewportElevationPointsAdjusted = new Vector3[arrayLength];
			}
			Vector2 uv;
			Vector3 v;
			int earthElevationWidthMinus1 = earthElevationWidth - mip;
			int earthElevationHeightMinus1 = earthElevationHeight - mip;

			if (curvatureOffsets == null || curvatureOffsets.Length <= cmax) {
				curvatureOffsets = new float[cmax + 1];
			}
			if (currentCurvature != 0) {
				for (int k = cmin; k <= cmax; k += mip) {
					float kk0 = renderViewportClip0.x + dx * (float)k;
					float k0; 
					if (kk0 <= 0) {
						k0 = 0;
					} else if (kk0 >= 1) {
						k0 = 1;
					} else {
						k0 = kk0;
					}
					float x = k0 - 0.5f;
					curvatureOffsets [k] = Mathf.Cos (x * 3.1415927f) * currentCurvature;
				}
			} else {
				for (int k = cmin; k <= cmax; k += mip) {
					curvatureOffsets [k] = 0;
				}
			}

			for (int j = rmin; j <= rmax; j += mip) {
				float jj0 = renderViewportClip1.y + dy * (float)j;
				float j0;
				if (jj0 <= 0) {
					j0 = 0; 
				} else if (jj0 >= 1) {
					j0 = 1;
				} else {
					j0 = jj0;
				}
				uv.y = j0;
				v.y = j0 - 0.5f;

				int jj = earthElevationWidth;
				if (j < earthElevationHeightMinus1) {
					jj *= j;
				} else {
					jj *= earthElevationHeightMinus1;
				}

				for (int k = cmin; k <= cmax; k += mip) {
					float kk0 = renderViewportClip0.x + dx * (float)k;
					float k0; 
					if (kk0 <= 0) {
						k0 = 0;
					} else if (kk0 >= 1) {
						k0 = 1;
					} else {
						k0 = kk0;
					}

					arrayIndex++;

					// add uv mapping
					uv.x = k0;
					viewportUV [arrayIndex] = uv;

					if (_renderViewportElevationFactor != 0) {
						// add vertex location
						int kw = (_wrapHorizontally && k >= earthElevationWidth) ? k - earthElevationWidth : k;
						int pos = jj;
						if (kw < earthElevationWidthMinus1) {
							pos += kw;
						} else {
							pos += earthElevationWidthMinus1;
						}
						float elev = viewportElevationPoints [pos];
						// as this pos get clamped at borders, interpolate with previous row or col
						if (j == rmin && rmin < earthElevationHeightMinus1) {
							float jj1 = renderViewportClip1.y + dy * (j + mip);
							float t = (j0 - jj0) / (jj1 - jj0);
							if (t > 0) {
								float elev1 = viewportElevationPoints [pos + earthElevationWidth];
								elev = t >= 1f ? elev1 : elev * (1f - t) + elev1 * t;
							}
						} else if (j == rmax && rmax > 0) {
							float jj1 = renderViewportClip1.y + dy * (j - mip);
							float t = (jj0 - j0) / (jj0 - jj1);
							if (t > 0) {
								float elev1 = viewportElevationPoints [pos - earthElevationWidth];
								elev = t >= 1f ? elev1 : elev * (1f - t) + elev1 * t;
							}
						} else if (j < rmax) {
							float elev1 = viewportElevationPoints [pos + earthElevationWidth];
							elev = (elev + elev1) * 0.5f;
						}

						if (k == cmin && cmin < earthElevationWidthMinus1) {
							float kk1 = renderViewportClip0.x + dx * (kw + mip);
							float t = (k0 - kk0) / (kk1 - kk0);
							if (t > 0) {
								float elev1 = viewportElevationPoints [pos + 1];
								elev = t >= 1f ? elev1 : elev * (1f - t) + elev1 * t;
							}
						} else if (k == cmax && cmax > 0 && pos > 0) {
							float kk1 = renderViewportClip0.x + dx * (kw - mip);
							float t = (kk0 - k0) / (kk0 - kk1);
							if (t > 0) {
								float elev1 = viewportElevationPoints [pos - 1];
								elev = t >= 1f ? elev1 : elev * (1f - t) + elev1 * t;
							}
						} else if (k < cmax) {
							float elev1 = viewportElevationPoints [pos + 1];
							elev = (elev + elev1) * 0.5f;
						}
						v.z = -elev * _renderViewportElevationFactor;
					} else {
						v.z = 0;
					}
					v.x = k0 - 0.5f;

					v.z += curvatureOffsets [k]; 
					viewportElevationPointsAdjusted [arrayIndex] = v;
				}
			}

			// Set surface geometry
			int h = (rmax - rmin) / mip;
			int w = (cmax - cmin) / mip;
			int row = w + 1;
			int bindex = 0;
			int viewportIndicesLength = w * h * 6;
			if (viewportIndices == null || viewportIndices.Length != viewportIndicesLength) {
				viewportIndices = new int[viewportIndicesLength];
			}
			for (int j = 0; j < h; j++) {
				int pos = j * row;
				int posEnd = pos + w;
				while (pos < posEnd) {
					viewportIndices [bindex++] = pos + 1; 
					viewportIndices [bindex++] = pos;
					viewportIndices [bindex++] = pos + row + 1;
					viewportIndices [bindex++] = pos;
					viewportIndices [bindex++] = pos + row;
					viewportIndices [bindex++] = pos + row + 1;
					pos++;
				}
			}

			// Create and assign mesh
			if (arrayLength > 0 && arrayLength <= 65000) { 
				MeshFilter mf = _renderViewport.GetComponent<MeshFilter> ();
				Mesh mesh = mf.sharedMesh;
				if (mesh == null) {
					mesh = new Mesh ();
					if (disposalManager != null)
						disposalManager.MarkForDisposal (mesh);
				}
				if (mesh.vertexCount != viewportElevationPointsAdjusted.Length) {
					mesh.Clear ();
				}
				mesh.vertices = viewportElevationPointsAdjusted;
				mesh.uv = viewportUV; 
				mesh.SetIndices (viewportIndices, MeshTopology.Triangles, 0);
				mesh.RecalculateNormals ();
				mf.sharedMesh = mesh;
			}
			viewportColliderNeedsUpdate = 5;
		}

		#endregion


		#region Render viewport setup

		void AssignRenderViewport (GameObject o) {
			if (o != null) {
				_renderViewport = o;
				renderViewportIsEnabled = _renderViewport != gameObject;

			} else {
				_renderViewport = gameObject;
				renderViewportIsEnabled = false;
			}
		}

		void DetachViewport () {
			_renderViewportIsTerrain = false;
			if (overlayRT != null) {
				if (_currentCamera != null && _currentCamera.targetTexture != null)
					_currentCamera.targetTexture = null;
				RenderTexture.active = null;
				overlayRT.Release ();
				DestroyImmediate (overlayRT);
				overlayRT = null;
			}
			_currentCamera = cameraMain; // Camera main;
			if (_currentCamera == null) {
				Debug.LogWarning ("Camera main not found. Ensure you have a camera in the scene tagged as MainCamera.");
			}
			if (this.overlayLayer != null) {
				DestroyMapperCam ();
			}
			if (_renderViewport != gameObject) {
				AssignRenderViewport (gameObject);
				CenterMap ();
			}
		}


		void SetupViewport () {

			if (vgos == null)
				vgos = new Dictionary<int, GameObjectAnimator> ();
			if (extrudedRegions == null) {
				extrudedRegions = new List<Region> ();
			}

			if (!gameObject.activeInHierarchy) {
				return;
			}

			// Check correct window rect
			if (_windowRect.width == 0 || _windowRect.height == 0) {
				_windowRect = new Rect (-0.5f, -0.5f, 1, 1);
			}

			terrain = null;

			// updates renderViewportIsEnabled
			AssignRenderViewport (_renderViewport);

			if (_renderViewport == gameObject) {
				DetachViewport ();
				return;
			}

			terrain = _renderViewport.GetComponent<Terrain> ();
			_renderViewportIsTerrain = terrain != null;

			// Setup Render texture
			int imageWidth, imageHeight;
			imageWidth = Camera.main.pixelWidth;
			if (imageWidth < 1024)
				imageWidth = 1024;
			imageWidth = (int)(imageWidth * _renderViewportResolution);
			_renderViewportResolutionMaxRTWidth = Mathf.Clamp (_renderViewportResolutionMaxRTWidth, 1024, 8192);
			if (imageWidth > _renderViewportResolutionMaxRTWidth)
				imageWidth = _renderViewportResolutionMaxRTWidth;
			imageHeight = imageWidth / 2;

			SetupViewportUIPanel ();
			CheckViewportScaleAndCurvature ();

			if (overlayRT != null && (overlayRT.width != imageWidth || overlayRT.height != imageHeight || overlayRT.filterMode != _renderViewportFilterMode)) {
				if (_currentCamera != null && _currentCamera.targetTexture != null) {
					_currentCamera.targetTexture = null;
				}
				RenderTexture.active = null;
				overlayRT.Release ();
				DestroyImmediate (overlayRT);
				overlayRT = null;
			}
			GameObject overlayLayer = GetOverlayLayer (true);
			if (overlayRT == null) {
				overlayRT = new RenderTexture (imageWidth, imageHeight, 24, RenderTextureFormat.ARGB32);
				overlayRT.hideFlags = HideFlags.DontSave;	// don't add to the disposal manager
				overlayRT.filterMode = _renderViewportFilterMode; // FilterMode.Trilinear; -> trilinear causes blurry issues with NGUI
				overlayRT.anisoLevel = 0;
				overlayRT.useMipMap = (_renderViewportFilterMode == FilterMode.Trilinear);
			}
			
			// Camera
			GameObject camObj = GameObject.Find (MAPPER_CAM);
			if (camObj == null) {
				camObj = new GameObject (MAPPER_CAM, typeof(Camera));
				camObj.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
			}
			camObj.layer = overlayLayer.layer;
			mapperCam = camObj.GetComponent<Camera> ();
			mapperCam.aspect = 2;
			mapperCam.cullingMask = 1 << camObj.layer;
			mapperCam.clearFlags = CameraClearFlags.SolidColor;
			mapperCam.backgroundColor = new Color (0, 0, 0, _renderViewportIsTerrain ? 0f : 1f);
			mapperCam.targetTexture = overlayRT;
			mapperCam.nearClipPlane = _renderViewportIsTerrain ? 0.3f : 0.01f;
			mapperCam.farClipPlane = Mathf.Min (cameraMain.farClipPlane, 1000);
			mapperCam.renderingPath = _renderViewportRenderingPath;
			mapperCam.enabled = true;

			#if UNITY_5_6_OR_NEWER
												if (_wrapHorizontally) {
												mapperCam.allowMSAA = false;
												}
			#endif
			if (_currentCamera != mapperCam) {
				_currentCamera = mapperCam;
				CenterMap ();
			}

			if (_renderViewportIsTerrain) {
				// Additionals setup steps for Terrain support
				if (terrainMat == null) {
					terrainMat = Instantiate<Material> (Resources.Load<Material> ("WMSK/Materials/Terrain"));
				}
				if (disposalManager != null)
					disposalManager.MarkForDisposal (terrainMat); // terrainMat.hideFlags = HideFlags.DontSave;
				terrain.materialType = Terrain.MaterialType.Custom;
				terrain.materialTemplate = terrainMat;
				Shader.SetGlobalTexture ("_WMSK_Overlay", overlayRT);
			} else {
				// Additional setup steps for Viewport
				// Create wrapper cam
				_wrapCameraObj = GameObject.Find (MAPPER_CAM_WRAP);
				if (_wrapCameraObj == null) {
					_wrapCameraObj = Instantiate (camObj);
					_wrapCameraObj.hideFlags = HideFlags.DontSave;
					_wrapCameraObj.hideFlags |= HideFlags.HideInHierarchy;
					_wrapCameraObj.layer = overlayLayer.layer;
					_wrapCameraObj.name = MAPPER_CAM_WRAP;
				}
				if (_wrapCamera == null) {
					_wrapCamera = _wrapCameraObj.GetComponent<Camera> ();
					#if UNITY_5_6_OR_NEWER
																				_wrapCamera.allowMSAA = false;
					#endif
					_wrapCamera.clearFlags = CameraClearFlags.Nothing;
					_wrapCamera.tag = "Untagged";
				}
				_wrapCamera.enabled = _wrapHorizontally;
				if (_wrapHorizontally)
					UpdateWrapCam ();

				// Assigns render texture to current material and recreates the camera
				Material viewportMat = _renderViewport.GetComponent<Renderer> ().sharedMaterial;
				if (viewportMat != null) {
					viewportMat.mainTexture = overlayRT;
					if (_renderViewportLightingMode == VIEWPORT_LIGHTING_MODE.Unlit) {
						viewportMat.EnableKeyword ("WMSK_VIEWPORT_UNLIT");
					} else {
						viewportMat.DisableKeyword ("WMSK_VIEWPORT_UNLIT");
					}
				}
			}
			PointerTrigger pt = _renderViewport.GetComponent<PointerTrigger> () ?? _renderViewport.AddComponent<PointerTrigger> ();
			pt.map = this;

			// Setup 3d surface, cloud and other visual effects
			UpdateViewport ();

			// Shot!
			mapperCam.Render ();
		}


		void DestroyMapperCam () {
			if (isMiniMap) {
				return;
			}
			mapperCam = null;
												
			GameObject o = GameObject.Find (MAPPER_CAM);
			if (o != null) {
				DestroyImmediate (o);
			}
			o = GameObject.Find (MAPPER_CAM_WRAP);
			if (o != null) {
				DestroyImmediate (o);
			}
		}


		/// <summary>
		/// Ensure the proportions of the main map fit the aspect ratio of the render viewport
		/// </summary>
		void CheckViewportScaleAndCurvature () {
			if (_renderViewport == null || _renderViewport == gameObject || _renderViewportIsTerrain)
				return;

			float aspect = _renderViewport.transform.lossyScale.x / _renderViewport.transform.lossyScale.y;
			Vector3 scale = new Vector3 (transform.localScale.y * 2f * 2f / aspect, transform.localScale.y, 1f);
			if (transform.localScale != scale) {
				transform.localScale = scale;
			}
		}

		void SyncMapperCamWithMainCamera () {

			if (mapperCam != null) {
				Quaternion camRot = cameraMain.transform.rotation;
				Vector3 camPos = cameraMain.transform.position;
				if (camPos != lastMainCameraPos || camRot != lastMainCameraRot || !Application.isPlaying) {
					lastMainCameraPos = camPos;
					lastMainCameraRot = camRot;
					if (terrain.terrainData == null)
						return;
					float sx = terrain.terrainData.size.x * 0.5f;
					float sz = terrain.terrainData.size.z * 0.5f;
					transform.position = terrain.transform.position + new Vector3 (sx, WMSK_TERRAIN_MODE_Y_OFFSET, sz);
					transform.rotation = Misc.QuaternionX90;
					transform.localScale = new Vector3 (terrain.terrainData.size.x, terrain.terrainData.size.z, 1f);
					Vector3 center = new Vector3 (sx, 0, sz);
					Vector4 data = transform.position - center;
					data.w = _renderViewportTerrainAlpha;
					Shader.SetGlobalVector ("_WMSK_Data", data);
					Vector3 deltaPos = terrain.transform.position + center - lastMainCameraPos;
					_currentCamera.transform.position = transform.position - deltaPos;
					_currentCamera.transform.rotation = lastMainCameraRot;
				}
			}
		}

		void SyncMainCameraWithMapperCam () {

			if (mapperCam != null) {
				Shader.SetGlobalMatrix ("_WMSK_Clip", _currentCamera.projectionMatrix * _currentCamera.worldToCameraMatrix);
			}

			cameraMain.transform.rotation = _currentCamera.transform.rotation;
			cameraMain.transform.position = _currentCamera.transform.position + Misc.Vector3down * WMSK_TERRAIN_MODE_Y_OFFSET;
		}

		#endregion


		#region Wrap camera setup

		void UpdateWrapCam () {
			if (_wrapCameraObj == null || !renderViewportIsEnabled)
				return;

			// Reduce floating-point errors
			Vector3 apos = transform.position;
			transform.position -= apos;
			_currentCamera.transform.position -= apos;

			// Get clip bounds
			Vector3 v0 = _currentCamera.WorldToViewportPoint (transform.TransformPoint (Misc.Vector3left * 0.5f));
			Vector3 v1 = _currentCamera.WorldToViewportPoint (transform.TransformPoint (Misc.Vector3right * 0.5f));

			float x0 = v0.x;
			float x1 = v1.x;
			if ((x0 < 0 && x1 > 1) || (x0 >= 0 && x1 <= 1)) {
				// disable wrap cam as current camera is not over the edges or the zoom is too far
				_wrapCamera.enabled = false;
				transform.position += apos;
				_currentCamera.transform.position += apos;
				return;
			}

			if (x0 > 1) {
				// shifts current camera to the other side of the map
				Vector3 v = new Vector3 (x1 - x0 + 0.5f, 0.5f, v0.z);
				_currentCamera.transform.position = _currentCamera.ViewportToWorldPoint (v);
				_currentCamera.transform.position -= _currentCamera.transform.forward * lastDistanceFromCamera;
			} else if (x1 < 0) {
				// shifts current camera to the other side of the map
				Vector3 v = new Vector3 (x0 - x1 + 0.5f, 0.5f, v0.z);
				_currentCamera.transform.position = _currentCamera.ViewportToWorldPoint (v);
				_currentCamera.transform.position -= _currentCamera.transform.forward * lastDistanceFromCamera;
			}

			if (x0 > 0) {
				// wrap on the left
				Vector3 v = new Vector3 (x1 - x0 + 0.499f, 0.5f, v0.z);
				_wrapCameraObj.transform.position = _currentCamera.ViewportToWorldPoint (v);
			} else if (x1 < 1) {
				// wrap on the right
				Vector3 v = new Vector3 (x0 - x1 + 0.501f, 0.5f, v0.z);
				_wrapCameraObj.transform.position = _currentCamera.ViewportToWorldPoint (v);
			}

			_wrapCameraObj.transform.rotation = _currentCamera.transform.rotation;
			_wrapCameraObj.transform.position -= _currentCamera.transform.forward * lastDistanceFromCamera;

			// Restore positions
			transform.position += apos;
			_currentCamera.transform.position += apos;
			_wrapCameraObj.transform.position += apos;

			if (!_wrapCamera.enabled) {
				_wrapCamera.enabled = true;
				_wrapCamera.targetTexture = overlayRT;
			}
		}

		#endregion

		#region Viewport FX

		void UpdateCloudLayer () {
			if (_renderViewportIsTerrain || _renderViewport == null || _renderViewport == gameObject)
				return;

			Transform t = _renderViewport.transform.Find ("CloudLayer1");
			if (t == null) {
				Debug.Log ("Cloud layer not found under Viewport gameobject. Remove it and create it again from prefab.");
				return;
			}
			Renderer renderer = t.GetComponent<MeshRenderer> ();
			renderer.enabled = _earthCloudLayer;

			if (lastDistanceFromCamera <= 0)
				return;

			// Compute cloud layer position and texture scale and offset
			Vector3 clip0 = _currentCamera.WorldToViewportPoint (transform.TransformPoint (-0.5f, 0.5f, 0));
			Vector3 clip1 = _currentCamera.WorldToViewportPoint (transform.TransformPoint (0.5f, -0.5f, 0));

			float dx = clip1.x - clip0.x;
			float scaleX = 1.0f / dx;
			float offsetX = -clip0.x / dx;
			float dy = clip0.y - clip1.y;
			float scaleY = 1.0f / dy;
			float offsetY = -clip0.y / dy;

			t.transform.localPosition = new Vector3 (0, 0, _earthCloudLayerElevation * (_renderViewportElevationFactor + 0.01f));
			Material cloudMat = renderer.sharedMaterial;
			cloudMat.mainTextureScale = new Vector2 (scaleX, scaleY);
			float brightness = Mathf.Clamp01 ((lastDistanceFromCamera + t.transform.localPosition.z - 5f) / 5f);
			renderer.enabled = _earthCloudLayer && brightness > 0f;	// optimization: hide cloud layer entirely if it's 100% transparent
			cloudMat.SetFloat ("_EmissionColor", brightness * _earthCloudLayerAlpha);
			earthMat.SetFloat ("_CloudShadowStrength", _earthCloudLayer ? _earthCloudLayerShadowStrength * _earthCloudLayerAlpha : 0f);
			CloudLayerAnimator cla = t.GetComponent<CloudLayerAnimator> ();
			cla.earthMat = earthMat;
			cla.cloudMainTextureOffset = new Vector2 (offsetX, offsetY);
			cla.speed = _earthCloudLayerSpeed;
			cla.Update ();

			UpdateCurvature (t, renderer.sharedMaterial);
		}


		void UpdateFogOfWarLayer () {
			if (_renderViewportIsTerrain || _renderViewport == null || _renderViewport == gameObject)
				return;
			
			Transform t = _renderViewport.transform.Find ("FogOfWarLayer");
			if (t == null) {
				Debug.Log ("Fog of War layer not found under Viewport gameobject. Remove it and create it again from prefab.");
				return;
			}
			Renderer renderer = t.GetComponent<MeshRenderer> ();
			renderer.enabled = _fogOfWarLayer;
			
			if (lastDistanceFromCamera <= 0)
				return;
			
			// Compute fog layer position and texture scale and offset
			float elevationFactor = _earthElevation * 100.0f / lastDistanceFromCamera;
			float absElevation = Mathf.Abs (_fogOfWarLayerElevation);
			t.transform.localPosition = new Vector3 (0, 0, _earthCloudLayerElevation * absElevation * elevationFactor * 0.99f); // make it behind clouds
			t.transform.localScale = new Vector3 (1f + 0.05f * absElevation, 1f + 0.05f * absElevation, 1f);
			if (fogOfWarMat == null) {
				fogOfWarMat = Instantiate (Resources.Load <Material> ("WMSK/Materials/FogOfWar"));
				if (disposalManager != null)
					disposalManager.MarkForDisposal (fogOfWarMat);
			}
			renderer.sharedMaterial = fogOfWarMat;
			fogOfWarMat.mainTextureScale = new Vector2 (renderViewportScaleX, renderViewportScaleY);
			fogOfWarMat.mainTextureOffset = new Vector2 (renderViewportOffsetX, renderViewportOffsetY);
			fogOfWarMat.SetColor ("_EmissionColor", _fogOfWarColor);

			UpdateCurvature (t, renderer.sharedMaterial);
		}


		void UpdateSun () {
			if (!_sunUseTimeOfDay || _sun == null)
				return;
			_sun.transform.rotation = _renderViewport.transform.rotation;
			_sun.transform.Rotate (Vector3.up, 180f + _timeOfDay * 360f / 24f, Space.Self);
		}

		void UpdateViewportObjectsBuoyancy () {
			buoyancyCurrentAngle = Mathf.Sin (time) * VGOBuoyancyAmplitude * Mathf.Rad2Deg;
		}


		void UpdateCurvature (Transform layer, Material mat) {
			if (layer == null)
				return;

			#if UNITY_EDITOR
			#if UNITY_2018_3_OR_NEWER
            UnityEditor.PrefabInstanceStatus prefabInstanceStatus = UnityEditor.PrefabUtility.GetPrefabInstanceStatus(_renderViewport);
            if (prefabInstanceStatus != UnityEditor.PrefabInstanceStatus.NotAPrefab) {
                UnityEditor.PrefabUtility.UnpackPrefabInstance(_renderViewport, UnityEditor.PrefabUnpackMode.Completely, UnityEditor.InteractionMode.AutomatedAction);
            }
#else
			UnityEditor.PrefabType prefabType = UnityEditor.PrefabUtility.GetPrefabType (_renderViewport);
			if (prefabType != UnityEditor.PrefabType.None && prefabType != UnityEditor.PrefabType.DisconnectedPrefabInstance && prefabType != UnityEditor.PrefabType.DisconnectedModelPrefabInstance) {
				UnityEditor.PrefabUtility.DisconnectPrefabInstance (_renderViewport);
			}
#endif
			#endif

			MeshFilter mf = layer.GetComponent<MeshFilter> ();
			if (mf == null)
				return;
			if (currentCurvature == 0) {
				// Disable
				if (quadPrefab == null) {
					quadPrefab = Instantiate (Resources.Load<GameObject> ("WMSK/Prefabs/Quad").GetComponent<MeshFilter> ().sharedMesh);
				}
				flexQuad = quadPrefab;
			} else {
				// Enable
				if (flexQuadPrefab == null) {
					flexQuadPrefab = Resources.Load<Mesh> ("WMSK/Meshes/PlaneMesh");
				}
				flexQuad = flexQuadPrefab;
				if (flexQuadCurvature != currentCurvature) {
					// Updates flex quad z-positions
					Vector3[] vertices = flexQuad.vertices;
					for (int k = 0; k < vertices.Length; k++) {
						vertices [k].z = Mathf.Cos (vertices [k].x * 3.1415927f) * currentCurvature;
					}
					flexQuad.vertices = vertices;
					mf.sharedMesh = null;
					flexQuadCurvature = currentCurvature;
				}
			}
			if (mf.sharedMesh == null || mf.sharedMesh != flexQuad) {
				mf.mesh = flexQuad;
			}
		}

		#endregion


		#region internal viewport API

		void UpdateViewport () {

			if (_renderViewportIsTerrain) {
				if (earthLastElevation < 0) {
					earthLastElevation = 1f;
					TerrainGetElevationData ();
				}
				return;
			}

			// Update wrapping
			if (_wrapHorizontally)
				UpdateWrapCam ();

			// Calculates viewport rect
			ComputeViewportRect ();

			// Generates 3D surface
			EarthBuildMesh ();
			
			// Updates cloud layer
			UpdateCloudLayer ();
			
			// Update fog layer
			UpdateFogOfWarLayer ();
		}


		/// <summary>
		/// Updates renderViewportRect field
		/// </summary>
		void ComputeViewportRect (bool useSceneViewWindow = false) {
			if (!useSceneViewWindow && lastRenderViewportGood && Application.isPlaying)
				return;

			lastRenderViewportGood = true;


			#if UNITY_EDITOR
			Vector3 oldPos = _currentCamera.transform.position;
			Quaternion oldRot = _currentCamera.transform.rotation;
			float oldFoV = _currentCamera.fieldOfView;
			if (useSceneViewWindow && UnityEditor.SceneView.lastActiveSceneView != null) {
				Camera sceneCam = UnityEditor.SceneView.lastActiveSceneView.camera;
				if (sceneCam != null) {
					oldPos = _currentCamera.transform.position;
					oldRot = _currentCamera.transform.rotation;
					_currentCamera.transform.position = sceneCam.transform.position;
					_currentCamera.transform.rotation = sceneCam.transform.rotation;
					_currentCamera.fieldOfView = sceneCam.fieldOfView;
				}
			}
			#endif

			// Get clip rect
			if (!_enableFreeCamera) {
				_currentCamera.transform.forward = transform.forward;
			}
			Vector3 topLeft = transform.TransformPoint (-0.5f, 0.5f, 0);
			renderViewportClip0 = _currentCamera.WorldToViewportPoint (topLeft);
			Vector3 bottomRight = transform.TransformPoint (0.5f, -0.5f, 0);
			renderViewportClip1 = _currentCamera.WorldToViewportPoint (bottomRight);
			renderViewportClipWidth = renderViewportClip1.x - renderViewportClip0.x;
			renderViewportClipHeight = renderViewportClip0.y - renderViewportClip1.y;

			// Computes and saves current viewport scale, offset and rect
			renderViewportScaleX = 1.0f / renderViewportClipWidth;
			renderViewportOffsetX = -renderViewportClip0.x / renderViewportClipWidth;
			renderViewportScaleY = 1.0f / renderViewportClipHeight;
			renderViewportOffsetY = -renderViewportClip0.y / renderViewportClipHeight;
			_renderViewportRect = new Rect (renderViewportOffsetX - 0.5f, renderViewportOffsetY + 0.5f, renderViewportScaleX, renderViewportScaleY);

			if (_wrapHorizontally && renderViewportClip0.x > 0) {	// need to offset clip0x and clip1x to extract correct heights later
				renderViewportClip0.x -= renderViewportClipWidth; 
				renderViewportClip1.x = renderViewportClip0.x + renderViewportClipWidth;
			}
			#if UNITY_EDITOR
			_currentCamera.transform.position = oldPos;
			_currentCamera.transform.rotation = oldRot;
			_currentCamera.fieldOfView = oldFoV;
			#endif
		}

		void UpdateViewportObjects () {
			// Update animators
			if (vgos == null)
				return;
			foreach (KeyValuePair<int, GameObjectAnimator> entry in vgos) {
				if (entry.Value != null) {
					entry.Value.UpdateTransformAndVisibility ();
				}
			}
		}

		void RepositionViewportObjects () {
			if (renderViewportIsEnabled) {
				foreach (KeyValuePair<int, GameObjectAnimator> entry in vgos) {
					GameObjectAnimator go = entry.Value;
					if (go != null) {
						go.transform.SetParent (null, true);
						go.UpdateTransformAndVisibility (true);
					}
				}
			} else {
				foreach (KeyValuePair<int, GameObjectAnimator> entry in vgos) {
					GameObjectAnimator go = entry.Value;
					if (go != null) {
						go.transform.localScale = go.originalScale;
						go.UpdateTransformAndVisibility (true);
					}
				}
			}
		}

		#endregion

		#region UI Fitter


		Vector3[] wc;
		Vector3 panelUIOldPosition;
		Vector2 panelUIOldSize;


		void FitViewportToUIPanel () {

			if (!renderViewportIsEnabled || _renderViewportIsTerrain || _renderViewportUIPanel == null)
				return;

			if (Application.isPlaying && panelUIOldPosition == _renderViewportUIPanel.position && panelUIOldSize == _renderViewportUIPanel.sizeDelta) {
				return;
			}
			panelUIOldPosition = _renderViewportUIPanel.position;
			panelUIOldSize = _renderViewportUIPanel.sizeDelta;

			// Check if positions are different
			Rect rect = GetWorldRect (_renderViewportUIPanel);
			Camera cam = cameraMain;
			float zDistance = cam.farClipPlane - 1f;
			Vector3 bl = new Vector3 (rect.xMin, rect.yMax, zDistance);
			Vector3 tr = new Vector3 (rect.xMax, rect.yMin, zDistance);
			Vector3 br = new Vector3 (rect.xMax, rect.yMax, zDistance);
			bl = cam.ScreenToWorldPoint (bl);
			br = cam.ScreenToWorldPoint (br);
			tr = cam.ScreenToWorldPoint (tr);

			Transform t = _renderViewport.transform;

			Vector3 pos = (bl + tr) * 0.5f;
			float width = Vector3.Distance (bl, br);
			float height = Vector3.Distance (br, tr);

			t.position = pos;
			t.localScale = new Vector3 (width, height, 1f);
			t.forward = cam.transform.forward;
			CenterMap ();
		}

		Rect GetWorldRect (RectTransform rt) {
			rt.GetWorldCorners (wc);
			return new Rect (wc [0].x, wc [0].y, wc [2].x - wc [0].x, wc [2].y - wc [0].y);
		}

		void SetupViewportUIPanel () {
			if (wc == null || wc.Length != 4) {
				wc = new Vector3[4];
			}
			panelUIOldPosition = Misc.Vector3max;
			ToggleUIPanel (false);
		}

		void ToggleUIPanel (bool visible) {
			if (_renderViewportUIPanel == null)
				return;
			Image img = _renderViewportUIPanel.GetComponent<Image> ();
			if (img != null) {
				img.enabled = visible;
			}
		}

		#endregion

	}
}
