using UnityAtoms.BaseAtoms;
using UnityEngine;
using Unity.Mathematics;

using static UnityEngine.Vector3;
using static UnityEngine.KeyCode;
using static UnityEngine.Time;

namespace NNI.Vehicle {

  public class SimpleShipController : MonoBehaviour {

    new public Rigidbody rigidbody;

    public float mouseFactor = 1;

    public float torqueFactor = 1;
    public float forceFactor = 1;

    private float GetAxis (KeyCode plus, KeyCode minus) {
      return (minus.Get() ? -1 : 0) + (plus.Get() ? 1 : 0);
    }

    private float GetAxisDown (KeyCode plus, KeyCode minus) {
      return (minus.GetDown() ? -1 : 0) + (plus.GetDown() ? 1 : 0);
    }

    public void Start () {
    }

    public void Update () {
      if (KeyCode.X.GetDown())
        rigidbody.drag = 1 - rigidbody.drag;
      var mouseDelta = CursorLockManager.delta;
      var torque = zero;
      torque.x += -mouseDelta.y * mouseFactor;
      torque.y += mouseDelta.x * mouseFactor;
      torque.z = GetAxis(Q, E);
      rigidbody.AddRelativeTorque(torque * torqueFactor);
      var force = zero;
      force.x = GetAxis(D, A);
      force.y = GetAxis(R, F);
      force.z = GetAxis(W, S);
      rigidbody.AddRelativeForce(force * forceFactor);
    }

  }

}
