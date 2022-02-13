using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace NNI.Terrain {

  [Serializable]
  public static class IndexBuffer {

    public static Dictionary<(int, bool, bool, bool), List<int>> cache = new();

    public static List<int> For (int resolution, bool north, bool east, bool south, bool west) {
      // if (cache.TryGetValue((resolution, selection), out var indices))
      //   return indices;
      return Generate(resolution, north, east, south, west);
    }

    private static List<int> Generate (int resolution, bool northCrackFix, bool eastCrackFix, bool southCrackFix, bool westCrackFix) {
      var indices = new List<int>();
      for (var y = 0; y < resolution - 1; y++) {
        var slantLeft = y % 2 == 0;
        for (var x = 0; x < resolution - 1; x++) {
          var nwIndex = x + (y * resolution);
          var neIndex = x + 1 + (y * resolution);
          var seIndex = x + 1 + ((y + 1) * resolution);
          var swIndex = x + ((y + 1) * resolution);
          var triangle1 = slantLeft ?
            new int[3] { nwIndex, swIndex, seIndex } :
            new int[3] { nwIndex, swIndex, neIndex };
          var triangle2 = slantLeft ?
            new int[3] { nwIndex, seIndex, neIndex } :
            new int[3] { swIndex, seIndex, neIndex };
          if (northCrackFix && y == 0) {
            if (x % 2 == 0)
              triangle2 = new int[3] { nwIndex, seIndex, neIndex + 1 };
            else
              triangle1 = null;
          }
          if (eastCrackFix && x == resolution - 2) {
            if (y % 2 == 0)
              triangle2 = new int[3] { neIndex, swIndex, seIndex + resolution };
            else
              triangle2 = null;
          }
          if (southCrackFix && y == resolution - 2) {
            if (x % 2 == 0)
              triangle2 = new int[3] { swIndex, seIndex + 1, neIndex };
            else
              triangle1 = null;
          }
          if (westCrackFix && x == 0) {
            if (y % 2 == 0)
              triangle1 = new int[3] { nwIndex, swIndex + resolution, seIndex };
            else
              triangle1 = null;
          }
          if (triangle1 != null) {
            (triangle1[1], triangle1[2]) = (triangle1[2], triangle1[1]);
            indices.AddRange(triangle1);
          }
          if (triangle2 != null) {
            (triangle2[1], triangle2[2]) = (triangle2[2], triangle2[1]);
            indices.AddRange(triangle2);
          }
          slantLeft = !slantLeft;
        }
      }
      return indices;
    }

  }

}
