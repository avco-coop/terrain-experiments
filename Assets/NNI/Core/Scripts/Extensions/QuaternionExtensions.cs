using Unity.Mathematics;
using UnityEngine;

public static class QuaternionExtensions {

  public static Quaternion Inverse (this Quaternion q) {
    return Quaternion.Inverse(q);
  }

  public static Vector3 ToScaledAxis (this Quaternion q) {
    q.ToAngleAxis(out var angle, out var axis);
    if (Mathf.Abs(axis.magnitude) == Mathf.Infinity)
      return Vector3.zero;
    return axis * Mathf.DeltaAngle(0, angle);
  }

}
