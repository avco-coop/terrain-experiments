using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ComponentExtensions
{

  public static void LoadAbove<T> (this Component thing, ref T place, bool keep = true) where T : Component {
    if (keep && place)
      return;
    place = thing.GetComponentInParent<T>();
  }

  public static void LoadBelow<T> (this Component thing, ref T place, bool keep = true) where T : Component {
    if (keep && place)
      return;
    place = thing.GetComponentInChildren<T>();
  }

  public static void LoadFind<T> (this Component _, ref T place, bool keep = true) where T : Component {
    if (keep && place)
      return;
    place = UnityEngine.Object.FindObjectOfType<T>();
  }

  public static void Load<T> (this Component thing, ref T place, bool keep = true) where T : Component {
    if (keep && place)
      return;
    place = thing.GetComponent<T>();
  }

  public static void Load<T> (this Component thing, ref T place, string name, bool keep = true) where T : Component {
    if (keep && place)
      return;
    place = thing.transform.GetComponentsInChildren<T>().FirstOrDefault(c => c.name == name);
  }

  public static bool IsPlayer (this Component go)
  {
    if (go.gameObject.tag.Equals("Player"))
      return true;
    else if (go.transform.parent)
      return go.transform.parent.IsPlayer();
    else
      return false;
  }

}
