// World Strategy Kit for Unity - Main Script
// (C) 2016-2019 by Ramiro Oliva (Kronnect)
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WorldMapStrategyKit {

	public partial class WMSK : MonoBehaviour {

		#region Public properties

		/// <summary>
		/// Tiggered when mouse enters a Viewport GameObject.
		/// </summary>
		public Action<GameObjectAnimator> OnVGOPointerEnter;

		/// <summary>
		/// Tiggered when mouse exits a Viewport GameObject.
		/// </summary>
		public Action<GameObjectAnimator> OnVGOPointerExit;

		/// <summary>
		/// Tiggered when left mouse button is pressed on a Viewport GameObject.
		/// </summary>
		public Action<GameObjectAnimator> OnVGOPointerDown;

		/// <summary>
		/// Tiggered when left mouse button is released on a Viewport GameObject.
		/// </summary>
		public Action<GameObjectAnimator> OnVGOPointerUp;

		/// <summary>
		/// Tiggered when right mouse button is pressed on a Viewport GameObject.
		/// </summary>
		public Action<GameObjectAnimator> OnVGOPointerRightDown;
		
		/// <summary>
		/// Tiggered when right mouse button is released on a Viewport GameObject.
		/// </summary>
		public Action<GameObjectAnimator> OnVGOPointerRightUp;

		/// <summary>
		/// Tiggered when an unit starts moving.
		/// </summary>
		public Action<GameObjectAnimator> OnVGOMoveStart;

		/// <summary>
		/// Tiggered when an unit stops moving.
		/// </summary>
		public Action<GameObjectAnimator> OnVGOMove;

		/// <summary>
		/// Tiggered when an unit stops moving.
		/// </summary>
		public Action<GameObjectAnimator> OnVGOMoveEnd;

		/// <summary>
		/// Triggered when an unit enters a country
		/// </summary>
		public Action<GameObjectAnimator> OnVGOCountryEnter;
		
		/// <summary>
		/// Triggered when an unit enters a country
		/// </summary>
		public Action<GameObjectAnimator> OnVGOProvinceEnter;

		/// <summary>
		/// Tiggered when the unit is destroyed.
		/// </summary>
		public Action<GameObjectAnimator> OnVGOKilled;


		/// <summary>
		/// The VGO buoyancy amplitude (rotation amount). Set to zero to deactivate.
		/// </summary>
		public float VGOBuoyancyAmplitude = 0.1f;
		
		/// <summary>
		/// The minimum zoom level required to see buoyancy effects on naval units
		/// </summary>
		public float VGOBuoyancyMaxZoomLevel = 0.15f;

		/// <summary>
		/// The VGO global speed multiplier for movement. Useful to adjust speed of unit movement globally.
		/// </summary>
		public float VGOGlobalSpeed = 0.5f;

		#endregion

		#region Viewport GameObject (VGOs) APIs

		
		/// <summary>
		/// Returns last clicked Viewport Gameobject.
		/// </summary>
		[NonSerialized]
		public GameObjectAnimator VGOLastClicked;
		
		/// <summary>
		/// Returns last highlighted Viewport Gameobject.
		/// </summary>
		[NonSerialized]
		public GameObjectAnimator VGOLastHighlighted;

		int lastUniqueIdUsed = 0;

		/// <summary>
		/// Registers the game object in the viewport collection. It's position will be monitored by the viewport updates.
		/// </summary>
		public void VGORegisterGameObject (GameObjectAnimator o) {
			if (vgos == null || o == null)
				return;
			if (o.uniqueId == 0) {
				while(vgos.ContainsKey(++lastUniqueIdUsed)) {
				}
				o.uniqueId = lastUniqueIdUsed;
			}
			vgos [o.uniqueId] = o;
		}

		/// <summary>
		/// Unregisters the game object in the viewport collection.
		/// </summary>
		public void VGOUnRegisterGameObject (GameObjectAnimator o) {
			if (vgos == null || o == null || o.uniqueId == 0)
				return;
			if (vgos.ContainsKey (o.uniqueId)) {
				vgos.Remove (o.uniqueId);
			}
		}

		/// <summary>
		/// Returns true if the game object is already registered in the viewport collection.
		/// </summary>
		/// <returns><c>true</c>, if viewport is registered was rendered, <c>false</c> otherwise.</returns>
		internal bool VGOIsRegistered (GameObjectAnimator o) {
			if (vgos == null)
				return false;
			return vgos.ContainsKey (o.uniqueId);
		}

		/// <summary>
		/// Toggles the visibility of a group of GameObjects in the viewport.
		/// </summary>
		public void VGOToggleGroupVisibility (int group, bool isVisible) {
			if (vgos == null)
				return;
			foreach (KeyValuePair<int, GameObjectAnimator> keyValue in vgos) {
				GameObjectAnimator go = keyValue.Value;
				if (go != null && go.group == group) {
					go.visible = isVisible;
					GameObject o = go.gameObject;
					if (o.activeSelf != isVisible) {
						o.SetActive (isVisible);
						go.UpdateVisibility (true);
						go.UpdateTransformAndVisibility ();
					}
				}
			}
		}

		
		public float VGOBuoyancyCurrentAngle {
			get { return buoyancyCurrentAngle; }
		}

		/// <summary>
		/// Returns the registered Game Object with a given unique identifier
		/// </summary>
		public GameObjectAnimator VGOGet (int uniqueId) {
			GameObjectAnimator o = null;
			vgos.TryGetValue (uniqueId, out o);
			return o;
		}

		/// <summary>
		/// Returns a list of all registered Game Objects
		/// </summary>
		/// <param name="gos">User supplied list for getting the game object animators</param>
		public int VGOGet(List<GameObjectAnimator> gos) {
			if (gos != null) {
				gos.AddRange (vgos.Values);
				return gos.Count;
			}
			return 0;
		}

		/// <summary>
		/// Returns the registered Game Object near a given position
		/// </summary>
		public GameObjectAnimator VGOGet (Vector2 mapPos, float distance) {
			distance *= distance;
			List<GameObjectAnimator> gos = new List<GameObjectAnimator> (vgos.Values);
			int gosCount = gos.Count;
			for (int k = 0; k < gosCount; k++) {
				GameObjectAnimator go = gos [k];
				float d = FastVector.SqrDistanceByValue (go.currentMap2DLocation, mapPos); // Vector2.SqrMagnitude (go.currentMap2DLocation - mapPos);
				if (d <= distance)
					return go;
			}
			return null;
		}

		/// <summary>
		/// Get a list of registered Game Objects inside a rectangle.
		/// </summary>
		/// <returns>The get.</returns>
		/// <param name="rect">Rect.</param>
		/// <param name="predicate">Predicate.</param>
		public List<GameObjectAnimator> VGOGet (Rect rect) {

			List<GameObjectAnimator> gos = new List<GameObjectAnimator> (vgos.Values);
			List<GameObjectAnimator> selected = new List<GameObjectAnimator> ();
			int gosCount = gos.Count;
			for (int k = 0; k < gosCount; k++) {
				GameObjectAnimator go = gos [k];
				if (rect.Contains (go.currentMap2DLocation))
					selected.Add (go);
			}
			return selected;
		}


		/// <summary>
		/// Get a list of registered Game Objects inside a rectangle. Optionally provide a predicate function to filer units.
		/// </summary>
		/// <returns>The get.</returns>
		/// <param name="rect">Rect.</param>
		/// <param name="predicate">Predicate function that returns true for each unit passed.</param>
		public int VGOGet (Rect rect, List<GameObjectAnimator> results, AttribPredicate predicate = null) {

			List<GameObjectAnimator> gos = new List<GameObjectAnimator> (vgos.Values);
			results.Clear ();
			int gosCount = gos.Count;
			for (int k = 0; k < gosCount; k++) {
				GameObjectAnimator go = gos [k];
				if (rect.Contains (go.currentMap2DLocation) && (predicate == null || predicate (go.attrib))) {
					results.Add (go);
				}
			}
			return results.Count;
		}




		#endregion

	}

}