using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace NNI.Universe {

  public class Gravity : MonoBehaviour {

    public float mass;

    public void FixedUpdate () {
      foreach (var r in FindObjectsOfType<Rigidbody>()) {
        var delta = transform.position - r.position;
        r.AddForce(delta.normalized * (mass / delta.sqrMagnitude), ForceMode.Acceleration);
      }
    }

  }

}
