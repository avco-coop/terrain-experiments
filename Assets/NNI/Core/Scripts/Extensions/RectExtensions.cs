
using UnityEngine;

public static class RectExtensions {

  public static Rect Union(this Rect a, Rect b)
  {
      return Rect.MinMaxRect(
        Mathf.Min(a.xMin, b.xMin),
        Mathf.Min(a.yMin, b.yMin),
        Mathf.Max(a.xMax, b.xMax),
        Mathf.Max(a.yMax, b.yMax)
      );
  }

  public static Vector2 Clip (this Rect r, Vector2 p, bool validate = true) {
    var x = p.x;
    var y = p.y;
    var minX = r.xMin;
    var maxX = r.xMax;
    var minY = r.yMin;
    var maxY = r.yMax;
    if (validate && minX < x && x < maxX && minY < y && y < maxY)
      return p;
    var midX = (minX + maxX) / 2;
    var midY = (minY + maxY) / 2;
    var m = (midY - y) / (midX - x);
    if (x <= midX) {
      var minXy = (m * (minX - x)) + y;
      if (minY <= minXy && minXy <= maxY)
        return new Vector2(minX, minXy);
    }
    if (x >= midX) {
      var maxXy = (m * (maxX - x)) + y;
      if (minY <= maxXy && maxXy <= maxY)
        return new Vector2(maxX, maxXy);
    }
    if (y <= midY) {
      var minYx = ((minY - y) / m) + x;
      if (minX <= minYx && minYx <= maxX)
        return new Vector2(minYx, minY);
    }
    if (y >= midY) {
      var maxYx = ((maxY - y) / m) + x;
      if (minX <= maxYx && maxYx <= maxX)
        return new Vector2(maxYx, maxY);
    }
    return p;
  }

}
