using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace NNI.Terrain {

  [Serializable]
  public class IndexBufferSet {

    private List<int> indicesBase;
    private List<int> indicesNCrackFix;
    private List<int> indicesECrackFix;
    private List<int> indicesSCrackFix;
    private List<int> indicesWCrackFix;
    private List<int> indicesNwCrackFix;
    private List<int> indicesNeCrackFix;
    private List<int> indicesSeCrackFix;
    private List<int> indicesSwCrackFix;

    public int resolution;

    public bool isValid (int resolution) {
      return this.resolution == resolution && indicesBase != null && indicesBase.Count > 0;
    }

    public List<int> this[IndexBufferSelection i] => i switch {
      IndexBufferSelection.Base => indicesBase,
      IndexBufferSelection.NCrackFix => indicesNCrackFix,
      IndexBufferSelection.ECrackFix => indicesECrackFix,
      IndexBufferSelection.SCrackFix => indicesSCrackFix,
      IndexBufferSelection.WCrackFix => indicesWCrackFix,
      IndexBufferSelection.NwCrackFix => indicesNwCrackFix,
      IndexBufferSelection.NeCrackFix => indicesNeCrackFix,
      IndexBufferSelection.SeCrackFix => indicesSeCrackFix,
      IndexBufferSelection.SwCrackFix => indicesSwCrackFix,
      _ => throw new ArgumentOutOfRangeException(nameof(i), $"Bad index selection: {i}"),
    };

    internal void Clear () {
      indicesBase = null;
      indicesNCrackFix = null;
      indicesECrackFix = null;
      indicesSCrackFix = null;
      indicesWCrackFix = null;
      indicesNwCrackFix = null;
      indicesNeCrackFix = null;
      indicesSeCrackFix = null;
      indicesSwCrackFix = null;
    }

    internal void Generate (int newResolution) {
      this.resolution = newResolution;
      indicesBase       = CreateIndexBuffer(resolution, false, false, false, false);
      indicesNCrackFix  = CreateIndexBuffer(resolution, true, false, false, false);
      indicesECrackFix  = CreateIndexBuffer(resolution, false, true, false, false);
      indicesSCrackFix  = CreateIndexBuffer(resolution, false, false, true, false);
      indicesWCrackFix  = CreateIndexBuffer(resolution, false, false, false, true);
      indicesNwCrackFix = CreateIndexBuffer(resolution, true, false, false, true);
      indicesNeCrackFix = CreateIndexBuffer(resolution, true, true, false, false);
      indicesSeCrackFix = CreateIndexBuffer(resolution, false, true, true, false);
      indicesSwCrackFix = CreateIndexBuffer(resolution, false, false, true, true);
    }

    public static List<int> CreateIndexBuffer (int resolution, bool northCrackFix, bool eastCrackFix, bool southCrackFix, bool westCrackFix) {
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
