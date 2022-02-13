using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PriorityQueue;
using System.Collections;
using UnityEngine.Experimental.Rendering;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace NNI.Terrain {

  public class WorldPanner : MonoBehaviour {

    public World world;

    public Vector2 pan;

    public void FixedUpdate () {
      world.offset += pan * Time.deltaTime;
      // world.worldScale *= Mathf.Lerp(1, rate, Time.deltaTime);
      // world.worldScale = Mathf.Max(zoom.y, world.worldScale);
      world.Generate();
    }

    //     [Min(1)]
    //     public int halfPatchResolution;
    //     public float edgeLengthLimit = 10;
    //     public float splitMultiplier = 3;
    //     public bool dirty = false;
    //     public float textureScale = 1000;
    //     [Range(0, 100)]
    //     public int maxDepth = 0;
    //     public int minDepth = 0;

    //     public Vector3 terrainCentrePosition;

    //     public Node root;

    //     public IndexBufferSet indexBuffers;

    //     public int patchResolution => 2 * halfPatchResolution;

    //     public float patchLengthLimit => edgeLengthLimit * patchResolution;

    //     public SimplePriorityQueue<Node, Node.Priority> splitQueue = new();

    //     public bool rebuildTextures = false;
    //     public bool regeneratePatches = false;

    //     public bool generateNormals = false;

    //     public float normalScale = 1;

    //     public void OnHeightmapChanged (RenderTexture rt) {
    //       heightmap = rt;
    //       regeneratePatches = true;
    //       rebuildTextures = true;
    //     }

    //     public void Awake () {
    //       splitQueue.Clear();
    //     }

    //     public Texture2D heightmapTexture;

    //     public void OnDisable () {
    //       if (heightmap)
    //         heightmap.DiscardContents(true, true);
    //       splitQueue.Clear();
    //       root?.Destroy();
    //       root = null;
    //       transform.DestroyChildren(true);
    //       indexBuffers.Clear();
    //       DestroyImmediate(heightmapTexture);
    //       heightmapTexture = null;
    //     }

    //     public void Clear () {
    //       splitQueue.Clear();
    //       root?.Destroy();
    //       root = null;
    //       transform.DestroyChildren(true);
    //       terrainCentrePosition = Vector3.zero;
    //     }

    //     public void BuildTree () {
    //       if (root != null)
    //         return;
    //       root = Node.New().Setup(
    //         this, "R", NodeType.Root, null,
    //         new Vector3(-1, 0, +1) * 0.5f * size, // nw
    //         new Vector3(+1, 0, +1) * 0.5f * size, // ne
    //         new Vector3(-1, 0, -1) * 0.5f * size, // sw
    //         new Vector3(+1, 0, -1) * 0.5f * size  // se
    //       );
    //     }

    //     public int queueBudgetMS = 0;

    //     public void Update () {
    // #if UNITY_EDITOR
    //       if (PrefabStageUtility.GetCurrentPrefabStage())
    //         return;
    // #endif
    //       if (dirty) {
    //         dirty = false;
    //         Clear();
    //       }
    //       EnsureIndexBuffers();
    //       if (rebuildTextures) {
    //         rebuildTextures = false;
    //         heightmapTexture = new Texture2D(heightmap.width, heightmap.height, heightmap.graphicsFormat, TextureCreationFlags.None) {
    //           hideFlags = HideFlags.DontSaveInEditor,
    //           wrapMode = TextureWrapMode.Clamp
    //         };
    //         RenderTexture.active = heightmap;
    //         heightmapTexture.ReadPixels(new Rect(0, 0, heightmap.width, heightmap.height), 0, 0);
    //         heightmapTexture.Apply();
    //       }
    //       BuildTree();
    //       if (regeneratePatches) {
    //         regeneratePatches = false;
    //         root.ForEach(node => node.Regenerate());
    //       }
    //       ProcessQueue();
    //       UpdateChildren();
    //       UpdateNeighbours();
    //       FixCracks();
    //       CreateMeshes();
    //       UpdateVisibilities();
    //     }

    //     private void EnsureIndexBuffers () {
    //       if (!indexBuffers.isValid(patchResolution + 1))
    //         indexBuffers.Generate(patchResolution + 1);
    //     }

    //     private void ProcessQueue () {
    //       var s = System.Diagnostics.Stopwatch.StartNew();
    //       while (splitQueue.Count > 0) {
    //         var node = splitQueue.Dequeue();
    //         if (node)
    //           node.Split();
    //         if (s.ElapsedMilliseconds > queueBudgetMS)
    //           break;
    //       }
    //     }

    //     public void UpdateChildren () {
    //       var observers = FindObjectsOfType<Observer>();
    //       var positions = observers.Where(o => o.enabled).Select(o => o.transform.position - transform.position).ToArray();
    //       root?.UpdateChildren(positions);
    //     }

    //     public void UpdateNeighbours () {
    //       foreach (var node in root?.Collect())
    //         node.UpdateNeighbours();
    //     }

    //     private void FixCracks () {
    //       foreach (var node in root?.Collect())
    //         node.FixCracks();
    //     }

    //     private void CreateMeshes () {
    //       foreach (var node in root?.Collect())
    //         node.CreateMesh();
    //     }

    //     private void UpdateVisibilities () {
    //       foreach (var node in root?.Collect())
    //         node.UpdateVisibility();
    //     }

    //     internal void ConfigureMaterial (MaterialPropertyBlock block) {
    //       block.SetFloat("_Resolution", patchResolution);
    //       block.SetFloat("_WorldSize", size);
    //       block.SetFloat("_FeatureScale", featureScale);
    //       block.SetFloat("_HeightScale", heightScale);
    //       block.SetTexture("_Heightmap", heightmap);
    //       block.SetTexture("_MainTex", heightmap);
    //     }

    //     internal void ConfigureCompute (ComputeShader compute) {
    //       compute.SetFloat("_Resolution", patchResolution);
    //       compute.SetFloat("_WorldSize", size);
    //       compute.SetFloat("_FeatureScale", featureScale);
    //       compute.SetFloat("_HeightScale", heightScale);
    //     }

  }

}
