using System;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace NNI {

  [ExecuteAlways]
  public class MaterialToTexture : MonoBehaviour {

    public int resolution = 256;

    public Material material;

    public RenderTexture texture;

    public UnityEvent<RenderTexture> onGenerate;

    public void OnEnable () {
      Generate();
    }

    public void OnDisable () {
      texture.DiscardContents(true, true);
      texture = null;
    }

    public void Generate () {
      if (!material)
        return;
      var desc = new RenderTextureDescriptor {
        width = resolution,
        height = resolution,
        volumeDepth = 1,
        graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat,
        dimension = TextureDimension.Tex2D,
        depthBufferBits = 0,
        enableRandomWrite = true,
        autoGenerateMips = true,
        msaaSamples = 1,
        useMipMap = true,
      };
      texture = new RenderTexture(desc);
      Graphics.Blit(null, texture, material);
      onGenerate.Invoke(texture);
    }

  }

}
