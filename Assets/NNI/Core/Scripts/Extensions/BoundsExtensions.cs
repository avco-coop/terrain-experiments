using System.Linq;
using UnityEngine;

using static UnityEngine.Mathf;
using static UnityEngine.Vector3;

public static class BoundsExtensions {

  public static Rect ScreenRect (this Bounds bounds, Camera camera) {
    var c = bounds.center;
    var e = bounds.extents;
    var worldCorners = new[] {
      new Vector3( c.x + e.x, c.y + e.y, c.z + e.z ),
      new Vector3( c.x + e.x, c.y + e.y, c.z - e.z ),
      new Vector3( c.x + e.x, c.y - e.y, c.z + e.z ),
      new Vector3( c.x + e.x, c.y - e.y, c.z - e.z ),
      new Vector3( c.x - e.x, c.y + e.y, c.z + e.z ),
      new Vector3( c.x - e.x, c.y + e.y, c.z - e.z ),
      new Vector3( c.x - e.x, c.y - e.y, c.z + e.z ),
      new Vector3( c.x - e.x, c.y - e.y, c.z - e.z ),
    };
    var screenCorners = worldCorners.Select(corner => Camera.main.WorldToScreenPoint(corner));
    return new Rect {
      xMax = screenCorners.Max(corner => corner.x),
      xMin = screenCorners.Min(corner => corner.x),
      yMax = screenCorners.Max(corner => corner.y),
      yMin = screenCorners.Min(corner => corner.y),
    };
  }

  public static Rect ScreenRect (this Bounds bounds, Transform reference, Camera camera) {
    var c = bounds.center;
    var e = bounds.extents;
    var worldCorners = new[] {
      new Vector3( c.x + e.x, c.y + e.y, c.z + e.z ),
      new Vector3( c.x + e.x, c.y + e.y, c.z - e.z ),
      new Vector3( c.x + e.x, c.y - e.y, c.z + e.z ),
      new Vector3( c.x + e.x, c.y - e.y, c.z - e.z ),
      new Vector3( c.x - e.x, c.y + e.y, c.z + e.z ),
      new Vector3( c.x - e.x, c.y + e.y, c.z - e.z ),
      new Vector3( c.x - e.x, c.y - e.y, c.z + e.z ),
      new Vector3( c.x - e.x, c.y - e.y, c.z - e.z ),
    };
    var screenCorners = worldCorners.
      Select(corner => reference.TransformPoint(corner)).
      Select(corner => Camera.main.WorldToScreenPoint(corner));
    return new Rect {
      xMax = screenCorners.Max(corner => corner.x),
      xMin = screenCorners.Min(corner => corner.x),
      yMax = screenCorners.Max(corner => corner.y),
      yMin = screenCorners.Min(corner => corner.y),
    };
  }

  public static Rect ViewportRect (this Bounds bounds, Camera camera) {
    var origin = camera.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.max.y, 0f));
    var extent = camera.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.min.y, 0f));
    return new Rect(origin.x, origin.y, extent.x - origin.x, origin.y - extent.y);
  }

}
