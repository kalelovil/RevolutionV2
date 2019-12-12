using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace WorldMapStrategyKit {

	public delegate void OnMarkerEvent (int buttonIndex);

	public class MarkerClickHandler : MonoBehaviour {

		public OnMarkerEvent OnMarkerMouseDown;
		public OnMarkerEvent OnMarkerMouseUp;
		public WMSK map;

		void Start () {
			// Get a reference to the World Map API:
			if (map == null)
				map = WMSK.instance;
		}


		void LateUpdate () {
			if ((OnMarkerMouseDown == null && OnMarkerMouseUp == null) || map == null)
				return;

			bool leftButtonPressed = Input.GetMouseButtonDown (0);
			bool rightButtonPressed = Input.GetMouseButtonDown (1);
			bool leftButtonReleased = Input.GetMouseButtonUp (0);
			bool rightButtonReleased = Input.GetMouseButtonUp (1);

			if (leftButtonPressed || rightButtonPressed || leftButtonReleased || rightButtonReleased) {
				// Check if cursor location is inside marker rect
				Vector2 cursorLocation = map.cursorLocation;
				Rect rect = new Rect (transform.localPosition - transform.localScale * 0.5f, transform.localScale);

				if (rect.Contains (cursorLocation)) {
					if (OnMarkerMouseDown != null && leftButtonPressed)
						OnMarkerMouseDown (0);
					if (OnMarkerMouseDown != null && rightButtonPressed)
						OnMarkerMouseDown (1);
					if (OnMarkerMouseUp != null && leftButtonReleased)
						OnMarkerMouseUp (0);
					if (OnMarkerMouseUp != null && rightButtonReleased)
						OnMarkerMouseUp (1);
				}
			}
		}
	}

}

