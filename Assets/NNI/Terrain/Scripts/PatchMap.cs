using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NNI.Terrain {

  public class PatchMap : MonoBehaviour {

    // private Dictionary<Vector3, Patch> map = new();
    public List<Patch> map = new();

    public void OnEnable () {
      map.Clear();
    }

    internal void Register (Patch patch) {
      if (!map.Contains(patch))
        map.Add(patch);
      // map[patch.transform.position] = patch;
    }

    internal void Unregister (Patch patch) {
      if (map.Contains(patch))
        map.Remove(patch);
      // map[patch.transform.position] = patch;
    }

    internal Patch FindNeighbour (Patch patch, Vector3 direction) {
      var pos = patch.transform.position;
      pos += patch.patchSize * direction;
      return map.Find(p => p.transform.position == pos);
      // if (map.TryGetValue(pos, out var neighbour))
      //   return neighbour;
      // return null;
    }
  }

}
