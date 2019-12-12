using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMapStrategyKit.ClipperLib {
	public partial class Clipper {

		const float MULTIPLIER = 5000000;

		Region subject;

		public void AddPaths (List<Region> regions, PolyType polyType) {
			int regionCount = regions.Count;
			for (int k = 0; k < regionCount; k++) {
				AddPath (regions [k], polyType);
			}
		}

		public void AddPath (Region region, PolyType polyType) {
			if (region == null || region.points == null)
				return;
			int count = region.points.Length;
			List<IntPoint> points = new List<IntPoint> (count);
			for (int k = 0; k < count; k++) {
				float ix = region.points [k].x * MULTIPLIER;
				float iy = region.points [k].y * MULTIPLIER;
				IntPoint p = new IntPoint (ix, iy);
				points.Add (p);
			}
			AddPath (points, polyType, true);

			if (polyType == PolyType.ptSubject) {
				subject = region;
			}
		}

		public void AddPath (Vector2[] points, PolyType polyType) {
			if (points == null)
				return;
			if (polyType == PolyType.ptSubject) {
				Debug.LogError ("Subject polytype needs a Region object.");
				return;
			}
			int count = points.Length;
			List<IntPoint> newPoints = new List<IntPoint> (count);
			for (int k = 0; k < count; k++) {
				float ix = points [k].x * MULTIPLIER;
				float iy = points [k].y * MULTIPLIER;
				IntPoint p = new IntPoint (ix, iy);
				newPoints.Add (p);
			}
			AddPath (newPoints, polyType, true);
		}

		public void Execute (ClipType clipType, IAdminEntity entity) {
			List<List<IntPoint>> solution = new List<List<IntPoint>> ();
			Execute (clipType, solution);
			int contourCount = solution.Count;
			entity.regions.Clear ();
			for (int c = 0; c < contourCount; c++) {
				List<IntPoint> points = solution [c];
				int count = points.Count;
				Vector2[] newPoints = new Vector2[count];
				for (int k = 0; k < count; k++) {
					newPoints [k].x = (float)points [k].X / MULTIPLIER;
					newPoints [k].y = (float)points [k].Y / MULTIPLIER;
				}
				Region region = new Region (entity, entity.regions.Count);
				region.UpdatePointsAndRect (newPoints);
				region.sanitized = true;
				entity.regions.Add (region);
			}
		}

		public void Execute (ClipType clipType, Region output) {
			List<List<IntPoint>> solution = new List<List<IntPoint>> ();
			Execute (clipType, solution);
			int contourCount = solution.Count;
			if (contourCount == 0) {
				output.Clear ();
			} else {
				// Use the largest contour
				int best = 0;
				int pointCount = solution [0].Count;
				for (int k = 1; k < contourCount; k++) {
					int candidatePointCount = solution [k].Count;
					if (candidatePointCount > pointCount) {
						pointCount = candidatePointCount;
						best = k;
					}
				}
				List<IntPoint> points = solution [best];
				Vector2[] newPoints = new Vector2[pointCount];
				for (int k = 0; k < pointCount; k++) {
					newPoints [k].x = (float)points [k].X / MULTIPLIER;
					newPoints [k].y = (float)points [k].Y / MULTIPLIER;
				}
				output.UpdatePointsAndRect (newPoints);
			}
		}

		public void Execute (ClipType clipType) {
			if (clipType == ClipType.ctUnion && subject == null || clipType != ClipType.ctUnion) {
				Debug.LogError ("Clipper.Execute called without defined subject");
				return;
			}
			Execute (clipType, subject);
		}
	

	}

}