using Unity.Mathematics;
using UnityEngine;

public static class MathExtensions {

  public static Vector3 Lerp (Vector3 a, Vector3 b, Vector3 t) {
    return math.lerp(a, b, t);
  }

}
