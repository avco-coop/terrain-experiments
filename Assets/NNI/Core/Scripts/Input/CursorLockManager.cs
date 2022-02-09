using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CursorLockManager : MonoBehaviour, IPointerDownHandler
{

  public static CursorLockManager instance;

  public float sensitivity = 1;

  public static bool locked;

  public static Vector2 delta;

  public bool lockedState;

  public Vector2 deltaState;
  
  public bool doLock;

  void LateUpdate()
  {
    if (doLock) {
      Lock();
      doLock = false;
    }
    instance = this;
    if (locked && Input.GetKeyDown(KeyCode.Escape))
      Unlock();
    if (locked && Cursor.visible)
      Unlock();
    if (locked)
      delta = Pointer.current.delta.ReadValue() * sensitivity;
    else
      delta = Vector2.zero;
    lockedState = locked;
    deltaState = delta;
  }

  void Lock()
  {
    locked = true;
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
  }

  void Unlock()
  {
    locked = false;
    Cursor.visible = true;
    Cursor.lockState = CursorLockMode.None;
  }

  void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
  {
    if (GUIUtility.hotControl != 0)
      return;
    GUIUtility.keyboardControl = 0;
    if (eventData.button == 0)
      doLock = true;
  }

}
