using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NNI {

  public class FloatingOriginObject : MonoBehaviour {

    public UnityEvent onBeforeShift;
    public UnityEvent onAfterShift;

    protected void OnEnable () => Register();
    protected void OnDisable () => Unregister();

    public FloatingOriginManager manager => FloatingOriginManager.instance;

    public void Register () {
      if (manager != null)
        manager.Register(this);
    }

    public void Unregister () {
      if (manager != null)
        manager.Unregister(this);
    }

    public void BeforeShift () {
      if (!enabled)
        return;
      onBeforeShift.Invoke();
    }

    public void Shift (Vector3 delta) {
      if (!enabled)
        return;
      transform.position += delta;
      onAfterShift.Invoke();
    }

  }

}
