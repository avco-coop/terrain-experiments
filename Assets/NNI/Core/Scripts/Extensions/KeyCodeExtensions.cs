using UnityEngine;
using UnityEngine.EventSystems;

public static class KeyCodeExtensions {

  // public static int business;

  // public static bool IsBusy () => business > 0 || EventSystem.current.IsPointerOverGameObject();

  public static bool GetDown (this KeyCode keyCode) => Input.GetKeyDown(keyCode);

  public static bool GetUp (this KeyCode keyCode) => Input.GetKeyUp(keyCode);

  public static bool Get (this KeyCode keyCode) => Input.GetKey(keyCode);

  public static int GetInt (this KeyCode keyCode) => Input.GetKey(keyCode) ? 1 : 0;

}
