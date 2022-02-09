using System.Collections;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

[ExecuteAlways]
[CreateAssetMenu(menuName = "Heightmap Generator")]
public class HeightmapGenerator : ScriptableObject {

  public int seed;

  [Min(1)]
  public int resolution = 8192;

  [Min(0)]
  public float scale = 1;

  [Min(1)]
  public int octaves = 5;

  [Range(0, 1)]
  public float gain = 0.55f;

  [Range(0, 2)]
  public float lacunarity = 1.6f;

  public ComputeShader computer;

  public IEnumerator Generate (Task task) {
    var desc = new RenderTextureDescriptor {
      width = resolution,
      height = resolution,
      volumeDepth = 1,
      graphicsFormat = GraphicsFormat.R32_SFloat,
      dimension = UnityEngine.Rendering.TextureDimension.Tex2D,
      depthBufferBits = 0,
      enableRandomWrite = true,
      autoGenerateMips = true,
      msaaSamples = 1,
    };
    var renderTexture = new RenderTexture(desc);
    if (!computer || !computer.HasKernel("GenerateHeightmap"))
      yield break;
    var kid = computer.FindKernel("GenerateHeightmap");
    computer.GetKernelThreadGroupSizes(kid, out var x, out var y, out var z);
    if (resolution % x != 0 || resolution % y != 0 || z != 1)  {
      Debug.LogError($"resolution must be multiple of {x}");
      yield break;
    }
    computer.SetTexture(kid, "_Heightmap", renderTexture);
    computer.SetFloat("_Seed", seed);
    computer.SetFloat("_Resolution", resolution);
    computer.SetFloat("_Scale", scale);
    computer.SetFloat("_Octaves", octaves);
    computer.SetFloat("_Gain", gain);
    computer.SetFloat("_Lacunarity", lacunarity);
    for (var v = 0; v < resolution; v += (int) y) {
      computer.SetVector("_Offset", new Vector2(0, v));
      computer.Dispatch(kid, resolution / (int) x, 1, 1);
      task?.Update((float) v / resolution);
      yield return new WaitForEndOfFrame();
    }
    RenderTexture.active = renderTexture;
    var texture = new Texture2D(desc.width, desc.height, desc.graphicsFormat, TextureCreationFlags.MipChain) {
      wrapMode = TextureWrapMode.Clamp
    };
    texture.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0, true);
    texture.Apply();
    RenderTexture.active = null;
    renderTexture.Release();
    texture.name = "Heightmap";
    var path = EditorUtility.SaveFilePanel(
      "Save heightmap as EXR",
      Path.GetDirectoryName(AssetDatabase.GetAssetPath(this)),
      name + ".exr",
      "exr"
    );
    if (path.Length != 0) {
      var data = texture.EncodeToEXR();
      File.WriteAllBytes(path, data);
    }
    task?.Finish();
  }

}
