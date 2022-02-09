using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace NNI {

  public class FloatingOriginManager : MonoBehaviour {

    public static FloatingOriginManager instance;

    public Transform focus;

    public float maxDistance = 1000;

    public UnityEvent onFloatingOriginUpdated;

    protected List<FloatingOriginObject> floatingOriginObjects = new();

    protected void Awake () {
      if (!instance)
        instance = this;
      else
        Destroy(gameObject);
    }

    public void Register (FloatingOriginObject floatingOriginObject) {
      if (!floatingOriginObjects.Contains(floatingOriginObject))
        floatingOriginObjects.Add(floatingOriginObject);
    }

    public void Unregister (FloatingOriginObject floatingOriginObject) {
      floatingOriginObjects.Remove(floatingOriginObject);
    }

    protected void Shift () {
      floatingOriginObjects.ForEach(o => o.BeforeShift());
      var delta = focus.position;
      focus.position = Vector3.zero;
      floatingOriginObjects.ForEach(o => o.Shift(-delta));
      onFloatingOriginUpdated.Invoke();
    }

    protected void Update () {
      if (!focus)
        return;
      if (focus.position.magnitude > maxDistance)
        Shift();
    }

  }

}
