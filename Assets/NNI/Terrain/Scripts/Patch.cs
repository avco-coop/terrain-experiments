using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using static UnityEngine.Vector3;

namespace NNI.Terrain {

  [Serializable]
  [ExecuteAlways]
  [RequireComponent(typeof(MeshFilter))]
  [RequireComponent(typeof(MeshRenderer))]
  [RequireComponent(typeof(MeshCollider))]
  public class Patch : MonoBehaviour {

    public Node node;

    public bool dirty;

    public MeshFilter filter;
    new public MeshRenderer renderer;
    new public MeshCollider collider;

    public Vector3 nw => node.nw;
    public Vector3 ne => node.ne;
    public Vector3 sw => node.sw;
    public Vector3 se => node.se;

    public void OnEnable () {
      this.Load(ref filter);
      this.Load(ref renderer);
      this.Load(ref collider);
    }

    public void Destroy () {
      DestroyImmediate(gameObject);
    }

    public void Setup (Node node) {
      this.gameObject.hideFlags = HideFlags.DontSaveInEditor;
      this.node = node;
      gameObject.layer = node.gameObject.layer;
      dirty = false;
      this.name = node.name;
      var data = new VertexData();
      ComputeVertexData(data);
      var mesh = createMesh(data, node.world.indexBuffers[node.activeIndexBuffer]);
      if (node.world.generateNormals)
        mesh.RecalculateNormals();
      filter.sharedMesh = mesh;
      // collider.sharedMesh = mesh;
      renderer.sharedMaterial = node.world.material;
      var block = new MaterialPropertyBlock();
      node.ConfigureMaterial(block);
      renderer.SetPropertyBlock(block);
    }

    public void Awake () {
      if (node) {
        var block = new MaterialPropertyBlock();
        node.ConfigureMaterial(block);
        renderer.SetPropertyBlock(block);
      }
    }

    public void ComputeVertexData (VertexData data) {
      var res = (float) node.world.patchResolution;
      var size = node.world.size;
      var tex = node.world.heightmapTexture;
      for (var y = 0; y <= res; y++) {
        for (var x = 0; x <= res; x++) {
          var pos = Lerp(Lerp(nw, ne, x / res), Lerp(sw, se, x / res), y / res);
          var pixel = tex.GetPixelBilinear(pos.x / size + 0.5f, pos.z / size + 0.5f);
          var height = pixel.a;
          pos.y = height * node.world.heightScale;
          data.positions.Add(pos - node.centrePoint);
          data.colors.Add(pixel);
          data.uvs.Add((new Vector2(pos.x, pos.z) / size) + new Vector2(0.5f, 0.5f));
        }
      }
    }

    public Mesh createMesh (VertexData data, List<int> indices) {
      var mesh = new Mesh();
      mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
      mesh.SetVertices(data.positions);
      mesh.SetColors(data.colors);
      mesh.SetUVs(0, data.uvs);
      mesh.SetIndices(indices, MeshTopology.Triangles, 0);
      return mesh;
    }

    // public IEnumerator ComputeVertexData (Task task, VertexData data) {
    //   var compute = node.world.compute;
    //   var res = (float) node.world.patchResolution;
    //   var size = node.world.patchResolution + 1;
    //   var map = node.world.heightmap;
    //   var tex = new Texture2D(map.width, map.height, map.graphicsFormat, TextureCreationFlags.None);
    //   RenderTexture.active = map;
    //   tex.ReadPixels(new Rect(0, 0, map.width, map.height), 0, 0);

    //   var kid = compute.FindKernel("TransferHeightmap");
    //   compute.GetKernelThreadGroupSizes(kid, out var x, out var y, out var z);
    //   if (size % x != 0 || size % y != 0 || z != 1)
    //     throw new Exception($"patch resolution + 1 must be multiple of {x}");
    //   var heights = new float[size, size];
    //   using (var buffer = new ComputeBuffer(size * size, 4)) {
    //     compute.SetBuffer(kid, "_Heights", buffer);
    //     compute.SetTexture(kid, "_Heightmap", node.world.heightmap);
    //     node.ConfigureCompute(compute);
    //     for (var v = 0; v < size; v += (int) y) {
    //       compute.SetVector("_Offset", new Vector2(0, v));
    //       compute.Dispatch(kid, size / (int) x, 1, 1);
    //       task?.Update((float) v / size, "Transfering heightmap");
    //       yield return new WaitForEndOfFrame();
    //     }
    //     buffer.GetData(heights);
    //   }
    //   for (var u = 0; u < size; u++) {
    //     for (var v = 0; v < size; v++) {
    //       var pos = Lerp(Lerp(nw, ne, u / res), Lerp(sw, se, u / res), v / res);
    //       pos.y = heights[u, v];
    //       data.positions.Add(pos);
    //     }
    //   }
    //   node.centrePoint = data.positions.Aggregate((a, b) => a + b) / data.positions.Count;
    //   node.transform.localPosition = node.centrePoint;
    //   if (node.nodeType == NodeType.Root)
    //     node.world.transform.position = new Vector3(0, -heights[size / 2, size / 2], 0);
    //   for (var i = 0; i < heights.Length; ++i) {
    //     var pos = data.positions[i];
    //     data.uvs.Add(new Vector2(pos.x, pos.z) / node.world.size);
    //     data.positions[i] -= node.centrePoint;
    //   }
    //   yield break;
    // }

  }

}
