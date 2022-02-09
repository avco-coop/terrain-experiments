using UnityAtoms.SceneMgmt;
using UnityEngine.SceneManagement;

public static class SceneExtensions {

  public static Scene ToScene (this ref SceneField sf) {
    return SceneManager.GetSceneByPath(sf.ScenePath);
  }

}
