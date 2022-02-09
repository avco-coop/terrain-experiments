using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{

  public static Bounds InverseTransformBounds (this Transform t, Bounds b) {
    return new Bounds(
      t.InverseTransformPoint(b.center),
      t.InverseTransformVector(b.size)
    );
  }

  public static void DestroyChildren (this Transform t, bool includeInactive = false) {
    foreach (var child in t.GetChildren(includeInactive))
      Object.DestroyImmediate(child.gameObject);
  }

  public static IEnumerable<Transform> GetChildren (this Transform t, bool includeInactive = false) {
    var result = new List<Transform>();
    for (var i = 0; i < t.childCount; ++i) {
      var child = t.GetChild(i);
      if (includeInactive || child?.gameObject.activeSelf == true)
        result.Add(child);
    }
    return result;
  }

}
