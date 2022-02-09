using UnityEngine;
using UnityEditor;

namespace NNI {

  [CustomEditor(typeof(MaterialToTexture))]
  public class MaterialToTextureEditor : Editor {

    private MaterialToTexture mtt;

    private MaterialEditor materialEditor;

    public void OnEnable () {
      mtt = (MaterialToTexture) target;
      if (mtt.material)
        materialEditor = (MaterialEditor) CreateEditor(mtt.material);
    }

    public void OnDisable () {
      DestroyImmediate(materialEditor);
    }

    public override bool HasPreviewGUI () {
      return true;
    }

    public override void OnPreviewGUI (Rect r, GUIStyle background) {
      if (mtt.texture)
        GUI.DrawTexture(r, mtt.texture, ScaleMode.ScaleToFit, false);
    }

    public override void OnInspectorGUI () {
      EditorGUI.BeginChangeCheck();
      GUILayout.Button("Regenerate");
      DrawDefaultInspector();
      if (materialEditor) {
        GUILayout.Space(4);
        materialEditor.DrawHeader();
        var isDefaultMaterial = !AssetDatabase.GetAssetPath(mtt.material).StartsWith("Assets");
        using (new EditorGUI.DisabledGroupScope(isDefaultMaterial)) {
          materialEditor.OnInspectorGUI();
        }
      }
      if (EditorGUI.EndChangeCheck()) {
        mtt.Generate();
        if (materialEditor)
          DestroyImmediate(materialEditor);
        OnEnable();
      }
    }

  }

}
