using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace NNI.Universe {

  public class CameraStack : MonoBehaviour {

    new public Camera camera;
    public Camera subcamera;
    public int depth;
    public float length = 1000;
    public float ratio = 1000;

    internal void OnEnable () {
    }

    internal void OnPreCull () {
      if (!enabled)
        return;
      if (!camera)
        camera = GetComponent<Camera>();
      if (!subcamera) {
        subcamera = new GameObject().AddComponent<Camera>();
        subcamera.name = "Subcamera";
        subcamera.transform.SetParent(transform, false);
        subcamera.CopyFrom(camera);
        var ppl = subcamera.gameObject.AddComponent<PostProcessLayer>();
        ppl.fog.enabled = true;
        ppl.fog.excludeSkybox = true;
      }
      camera.clearFlags = CameraClearFlags.Skybox;
      camera.nearClipPlane = length / ratio;
      camera.farClipPlane = length;
      subcamera.enabled = false;
      for (var i = depth; i > 0; --i) {
        subcamera.fieldOfView = camera.fieldOfView;
        subcamera.clearFlags = i == depth ? CameraClearFlags.Skybox : CameraClearFlags.Depth;
        subcamera.nearClipPlane = length * Mathf.Pow(ratio, i - 1);
        subcamera.farClipPlane = length * Mathf.Pow(ratio, i);
        subcamera.Render();
        camera.clearFlags = CameraClearFlags.Depth;
      }
    }

  }

  //   internal void Start () {
  //     var cam = GetComponent<Camera>();
  //     enabled = false;
  //     cam.farClipPlane = length;
  //     var near = cam.farClipPlane;
  //     for (var i = 0; i < levels; ++i) {
  //       var go = new GameObject();
  //       go.transform.SetParent(transform, false);
  //       go.tag = "Untagged";
  //       var newcam = go.AddComponent<Camera>();
  //       // var newcam = cam.Clone(go);
  //       newcam.CopyFrom(cam);
  //       newcam.clearFlags = CameraClearFlags.Depth;
  //       newcam.nearClipPlane = near;
  //       near *= exponent;
  //       newcam.farClipPlane = near;
  //       newcam.cullingMask -= 1 << LayerMask.NameToLayer("UI");
  //       newcam.depth = -i - 2;
  //       if (i == levels - 1)
  //         newcam.clearFlags = CameraClearFlags.Skybox;
  //     }
  //     cam.clearFlags = CameraClearFlags.Depth;
  //   }

  // }

}
