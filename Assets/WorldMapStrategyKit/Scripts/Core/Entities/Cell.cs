using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

	public partial class Cell: IFader {

		public int row, column;

		/// <summary>
		/// Center of this cell in local space coordinates (-0.5..0.5)
		/// </summary>
		public Vector2 center;

		JSONObject _attrib;

		/// <summary>
		/// Use this property to add/retrieve custom attributes for this country
		/// </summary>
		public JSONObject attrib {
			get {
				if (_attrib == null) {
					_attrib = new JSONObject ();
				}
				return _attrib;
			}
			set {
				_attrib = value;
			}
		}

		Vector2[] _points;

		/// <summary>
		/// List of vertices of this cell.
		/// </summary>
		public Vector2[] points {
			get {
				if (_points != null)
					return _points;
				int pointCount = segments.Length;
				_points = new Vector2[pointCount];
				for (int k = 0; k < pointCount; k++) {
					_points [k] = segments [k].start;
				}
				return _points;
			}
		}

		/// <summary>
		/// Segments of this cell. Internal use.
		/// </summary>
		public CellSegment[] segments;
		public Rect rect2D;

		/// <summary>
		/// Temporary material used by Cell*Temporary* functions
		/// </summary>
		public Material tempMaterial;

		public Material customMaterial { get; set; }

		public Vector2 customTextureScale, customTextureOffset;
		public float customTextureRotation;

		public bool isFading { get; set; }

		public bool isWrapped;


		public Cell (int row, int column, Vector2 center) {
			this.row = row;
			this.column = column;
			this.center = center;
			this.segments = new CellSegment[6];
		}

		public bool Contains (Vector2 position) { 
			if (!rect2D.Contains (position))
				return false;
			int numPoints = points.Length;
			int j = numPoints - 1; 
			bool inside = false; 
			float x = position.x;
			float y = position.y;
			for (int i = 0; i < numPoints; j = i++) { 
				if (((_points [i].y <= y && y < _points [j].y) || (_points [j].y <= y && y < _points [i].y)) &&
				                (x < (_points [j].x - _points [i].x) * (y - _points [i].y) / (_points [j].y - _points [i].y) + _points [i].x))
					inside = !inside; 
			} 
			return inside; 
		}

	}
}

