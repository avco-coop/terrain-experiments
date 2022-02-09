using UnityEngine;

public static class MaterialExtensions
{

  public static void SetKeyword(this Material m, string name, bool value)
  {
    if (m.IsKeywordEnabled(name) == value)
      return;
    if (value)
      m.EnableKeyword(name);
    else
      m.DisableKeyword(name);
  }

}
