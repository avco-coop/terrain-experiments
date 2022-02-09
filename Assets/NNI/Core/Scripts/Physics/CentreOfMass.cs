using UnityEngine;

public class CentreOfMass : MonoBehaviour
{

  new private Rigidbody rigidbody;

  internal void Awake () {
    this.LoadAbove(ref rigidbody);
  }

  internal void Update()
  {
    rigidbody.centerOfMass = rigidbody.transform.InverseTransformPoint(transform.position);
  }

}
