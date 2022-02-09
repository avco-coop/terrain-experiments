using System;
using System.Collections.Generic;
using UnityEngine;

namespace NNI.Terrain {

  public class VertexData {
    public List<Vector3> positions = new();
    public List<Color>   colors = new();
    public List<Vector2> uvs = new();
    public List<Vector3> normals = new();
    public List<Vector4> tangents = new();
  }

}
