using System;
using System.Collections;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

[ExecuteAlways]
public class Heightmap : MonoBehaviour {

  public int size;

  public Texture2D texture;

  // public bool dirty;

  // public void UPdate ()

  public void Generate () {
    if (texture)
      DestroyImmediate(texture);
    texture = new Texture2D(size, size, GraphicsFormat.R32_SFloat, TextureCreationFlags.None);
  }

  // public RenderTexture source;

  // public int seed;

  // [Min(0)]
  // public float scale = 1;

  // [Min(1)]
  // public int octaves = 5;

  // [Range(0, 1)]
  // public float gain = 0.55f;

  // [Range(0, 2)]
  // public float lacunarity = 1.6f;

  // public ComputeShader compute;

  // public UnityEvent onChanged;

  // internal void ConfigureCompute (ComputeShader compute) {
  //   compute.SetFloat("_Seed", seed);
  //   compute.SetFloat("_Scale", scale);
  //   compute.SetFloat("_Octaves", octaves);
  //   compute.SetFloat("_Gain", gain);
  //   compute.SetFloat("_Lacunarity", lacunarity);
  // }

  // internal void ConfigureMaterial (MaterialPropertyBlock block) {
  //   block.SetFloat("_Seed", seed);
  //   block.SetFloat("_Scale", scale);
  //   block.SetFloat("_Octaves", octaves);
  //   block.SetFloat("_Gain", gain);
  //   block.SetFloat("_Lacunarity", lacunarity);
  // }

  // public void OnValidate () {
  //   onChanged.Invoke();
  // }

}
