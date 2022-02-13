using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProceduralToolkit;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Unity.Mathematics;
using static UnityEngine.Vector3;

namespace NNI.Terrain {

  [Serializable]
  [ExecuteAlways]
  [RequireComponent(typeof(MeshFilter))]
  [RequireComponent(typeof(MeshRenderer))]
  [RequireComponent(typeof(MeshCollider))]
  public class Patch : MonoBehaviour {

    public enum NormalMode {
      Mesh,
      Shader,
    }

    [Header("World Properties")]
    public float worldScale = 1;
    public float heightScale = 1;
    public Vector2 offset;

    [Header("Patch Properties")]
    [Min(0)]
    public float patchSize = 1;

    [Header("Neighbours")]
    public bool detectNeighboursEveryFrame;

    [SerializeField] private bool _north;
    public bool north {
      get => _north;
      set {
        if (_north == value)
          return;
        meshDirty = true;
        _north = value;
      }
    }

    [SerializeField] private bool _east;
    public bool east {
      get => _east;
      set {
        if (_east == value)
          return;
        meshDirty = true;
        _east = value;
      }
    }

    [SerializeField] private bool _south;
    public bool south {
      get => _south;
      set {
        if (_south == value)
          return;
        meshDirty = true;
        _south = value;
      }
    }

    [SerializeField] private bool _west;
    public bool west {
      get => _west;
      set {
        if (_west == value)
          return;
        meshDirty = true;
        _west = value;
      }
    }

    [Header("Texture Generation")]
    public Material textureMaterial;
    public int materialPass = -1;
    public int textureResolution = 512;
    public FilterMode filterMode = FilterMode.Bilinear;
    public TextureWrapMode wrapMode = TextureWrapMode.Clamp;

    [Header("Mesh Generation")]
    public bool meshDirty;
    public Material meshMaterial;
    [Min(1)]
    public int meshResolution = 1;
    public NormalMode normalMode;

    [Header("Debug")]
    public bool generateEveryFrame;
    public RenderTexture renderTexture;
    public Texture2D readableTexture;
    public bool renderGizmos;

    public PatchMap map;
    public MeshFilter filter;
    new public MeshRenderer renderer;
    new public MeshCollider collider;

    public void OnEnable () {
      this.LoadAbove(ref map);
      this.Load(ref filter);
      this.Load(ref renderer);
      this.Load(ref collider);
    }

    public void OnDestroy () {
      map.Unregister(this);
    }

    public void Dispose () {
      if (renderTexture)
        DestroyImmediate(renderTexture);
      if (readableTexture)
        DestroyImmediate(readableTexture);
      if (filter.sharedMesh)
        DestroyImmediate(filter.sharedMesh);
    }

    public void Update () {
      if (generateEveryFrame)
        Generate();
      if (detectNeighboursEveryFrame)
        DetectNeighbours();
      if (meshDirty) {
        meshDirty = false;
        Generate_CreateMesh();
      }
    }

    public void DetectNeighbours () {
      map.Register(this);
      north = map.FindNeighbour(this, Vector3.forward);
      east = map.FindNeighbour(this, Vector3.right);
      south = map.FindNeighbour(this, Vector3.back);
      west = map.FindNeighbour(this, Vector3.left);
    }

    public void Generate () {
      Dispose();
      if (worldScale > 0) {
        Generate_CreateRenderTexture();
        Generate_RenderHeights();
        Generate_TransferToTexture2D();
        Generate_ConfigureMaterial();
        Generate_CreateMesh();
      }
    }

    private void Generate_CreateRenderTexture () {
      renderTexture = new RenderTexture(new RenderTextureDescriptor {
        width = textureResolution,
        height = textureResolution,
        volumeDepth = 1,
        depthBufferBits = 0,
        graphicsFormat = GraphicsFormat.R32G32B32A32_SFloat,
        dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
        enableRandomWrite = true,
        msaaSamples = 1,
        autoGenerateMips = true,
        useMipMap = true,
      }) {
        filterMode = filterMode,
        wrapMode = wrapMode,
        hideFlags = HideFlags.DontSaveInEditor,
      };
    }

    private void Generate_RenderHeights () {
      if (textureMaterial) {
        var mat = new Material(textureMaterial);
        mat.SetVector("_WorldPosition", transform.position);
        mat.SetVector("_LocalPosition", transform.localPosition);
        mat.SetVector("_Offset", offset);
        mat.SetFloat("_Resolution", textureResolution);
        mat.SetFloat("_HeightScale", heightScale);
        mat.SetFloat("_WorldScale", worldScale);
        mat.SetFloat("_PatchSize", patchSize);
        Graphics.Blit(null, renderTexture, mat, materialPass);
      }
    }

    private void Generate_TransferToTexture2D () {
      if (readableTexture)
        DestroyImmediate(readableTexture);
      readableTexture = new Texture2D(renderTexture.width, renderTexture.height, renderTexture.graphicsFormat, TextureCreationFlags.MipChain) {
        filterMode = filterMode,
        wrapMode = wrapMode,
        hideFlags = HideFlags.DontSaveInEditor,
      };
      readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
      RenderTexture.active = null;
    }

    private void Generate_ConfigureMaterial () {
      if (meshMaterial) {
        var mat = new Material(meshMaterial);
        var props = new MaterialPropertyBlock();
        props.SetTexture("_Heightmap", renderTexture);
        props.SetTexture("_EmissionMap", renderTexture);
        props.SetFloat("_Resolution", textureResolution);
        props.SetFloat("_HeightScale", heightScale);
        props.SetFloat("_WorldScale", worldScale);
        props.SetFloat("_PatchSize", patchSize);
        props.SetFloat("_ComputeNormals", normalMode == NormalMode.Shader ? 1 : 0);
        renderer.SetPropertyBlock(props);
        if (normalMode == NormalMode.Shader) {
          mat.DisableKeyword("_NORMALS_MESH");
          mat.EnableKeyword("_NORMALS_SHADER");
        } else {
          mat.EnableKeyword("_NORMALS_MESH");
          mat.DisableKeyword("_NORMALS_SHADER");
        }
        renderer.sharedMaterial = mat;
      }
    }

    private void Generate_CreateMesh () {
      if (meshResolution > 0) {
        var mesh = CreateMesh();
        filter.sharedMesh = mesh;
        collider.sharedMesh = mesh;
      }
    }

    public void OnDrawGizmosSelected () {
      if (!renderGizmos)
        return;
      Gizmos.color = Color.yellow;
      Gizmos.DrawSphere(transform.position + new Vector3(-patchSize / 2, 0, -patchSize / 2), patchSize / 10);
      Gizmos.DrawSphere(transform.position + new Vector3(-patchSize / 2, 0, +patchSize / 2), patchSize / 10);
      Gizmos.DrawSphere(transform.position + new Vector3(+patchSize / 2, 0, -patchSize / 2), patchSize / 10);
      Gizmos.DrawSphere(transform.position + new Vector3(+patchSize / 2, 0, +patchSize / 2), patchSize / 10);
    }

    public Mesh CreateMesh () {
      var positions = new List<Vector3>();
      var uvs = new List<Vector2>();
      var normals = new List<Vector3>();
      var tangents = new List<Vector4>();
      var res = meshResolution;
      var nw = new Vector3(-0.5f, 0, +0.5f);
      var ne = new Vector3(+0.5f, 0, +0.5f);
      var sw = new Vector3(-0.5f, 0, -0.5f);
      var se = new Vector3(+0.5f, 0, -0.5f);
      if (res % 2 == 1)
        res++;
      for (var y = 0f; y <= res; y++) {
        for (var x = 0f; x <= res; x++) {
          var pos = Lerp(Lerp(nw, ne, x / res), Lerp(sw, se, x / res), y / res);
          var tpos = new float2(pos.x, pos.z);
          tpos -= 1f / (textureResolution * 2f);
          tpos *= textureResolution / (textureResolution - 1f);
          tpos += 0.5f;
          var pixel = readableTexture.GetPixelBilinear(tpos.x, tpos.y);
          pos.y = pixel.r * heightScale / worldScale;
          if (normalMode == NormalMode.Shader) {
            normals.Add(up);
            tangents.Add(new Vector4(1, 0, 0, -1));
          }
          positions.Add(pos * patchSize);
          pos /= textureResolution / (textureResolution - 1f);
          uvs.Add(new float2(pos.x, pos.z) + 0.5f);
        }
      }
      var indices = IndexBuffer.For(res + 1, !north, !east, !south, !west);
      var mesh = new Mesh {
        indexFormat = UnityEngine.Rendering.IndexFormat.UInt32,
        hideFlags = HideFlags.DontSaveInEditor,
      };
      mesh.SetVertices(positions);
      mesh.SetUVs(0, uvs);
      if (normalMode == NormalMode.Shader) {
        mesh.SetNormals(normals);
        mesh.SetTangents(tangents);
      }
      mesh.SetIndices(indices, MeshTopology.Triangles, 0);
      if (normalMode == NormalMode.Mesh) {
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
      }
      return mesh;
    }

    // public Mesh createMesh (VertexData data, List<int> indices) {
    //   var mesh = new Mesh();
    //   mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    //   mesh.SetVertices(data.positions);
    //   mesh.SetColors(data.colors);
    //   mesh.SetNormals(data.normals);
    //   mesh.SetTangents(data.tangents);
    //   mesh.SetUVs(0, data.uvs);
    //   mesh.SetIndices(indices, MeshTopology.Triangles, 0);
    //   return mesh;
    // }

    // public Vector3 nw;
    // public Vector3 ne;
    // public Vector3 sw;
    // public Vector3 se;

    // public void OnEnable () {
    //   this.Load(ref filter);
    //   this.Load(ref renderer);
    //   this.Load(ref collider);
    // }

    // public void Destroy () {
    //   DestroyImmediate(gameObject);
    // }

    // public void Setup (Node node) {
    //   this.gameObject.hideFlags = HideFlags.DontSaveInEditor;
    //   this.node = node;
    //   gameObject.layer = node.gameObject.layer;
    //   dirty = false;
    //   this.name = node.name;
    //   var data = new VertexData();
    //   ComputeVertexData(data);
    //   var mesh = createMesh(data, node.world.indexBuffers[node.activeIndexBuffer]);
    //   if (node.world.generateNormals)
    //     mesh.RecalculateNormals();
    //   filter.sharedMesh = mesh;
    //   // collider.sharedMesh = mesh;
    //   renderer.sharedMaterial = node.world.material;
    //   var block = new MaterialPropertyBlock();
    //   node.ConfigureMaterial(block);
    //   renderer.SetPropertyBlock(block);
    // }

    // public void Awake () {
    //   if (node) {
    //     var block = new MaterialPropertyBlock();
    //     node.ConfigureMaterial(block);
    //     renderer.SetPropertyBlock(block);
    //   }
    // }

    // public void ComputeVertexData (VertexData data) {
    //   var res = (float) node.world.patchResolution;
    //   var size = node.world.size;
    //   var tex = node.world.heightmapTexture;
    //   for (var y = 0; y <= res; y++) {
    //     for (var x = 0; x <= res; x++) {
    //       var pos = Lerp(Lerp(nw, ne, x / res), Lerp(sw, se, x / res), y / res);
    //       var pixel = tex.GetPixelBilinear(pos.x / size + 0.5f, pos.z / size + 0.5f);
    //       var height = pixel.a;
    //       pos.y = height * node.world.heightScale;
    //       // var normal = -new Vector3(pixel.g * tex.width, -node.world.heightScale * node.world.normalScale, pixel.b * tex.height).normalized;
    //       // normalize(float3(f.color.gb * _Heightmap_TexelSize.zw, -_HeightScale).xzy)
    //       data.positions.Add(pos - node.centrePoint);
    //       data.normals.Add(Vector3.up);
    //       data.tangents.Add(new Vector4(1, 0, 0, -1));
    //       data.colors.Add(pixel);
    //       data.uvs.Add((new Vector2(pos.x, pos.z) / size) + new Vector2(0.5f, 0.5f));
    //     }
    //   }
    // }

    // public Mesh createMesh (VertexData data, List<int> indices) {
    //   var mesh = new Mesh();
    //   mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    //   mesh.SetVertices(data.positions);
    //   mesh.SetColors(data.colors);
    //   mesh.SetNormals(data.normals);
    //   mesh.SetTangents(data.tangents);
    //   mesh.SetUVs(0, data.uvs);
    //   mesh.SetIndices(indices, MeshTopology.Triangles, 0);
    //   return mesh;
    // }

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
