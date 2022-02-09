using UnityEngine;

public static class Vector3Extensions
{

  public static Vector3 ClampMagnitude(this Vector3 v, float limit)
  {
    return Vector3.ClampMagnitude(v, limit);
  }

}
