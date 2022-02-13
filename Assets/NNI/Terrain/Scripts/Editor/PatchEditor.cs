//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace NNI.Terrain.Editor {

  [CustomEditor(typeof(Patch))]
  [CanEditMultipleObjects]
  public class PatchEditor : UnityEditor.Editor {

    Patch patch;

    private MaterialEditor materialEditor;

    public void OnEnable () {
      patch = target as Patch;
      if (patch.textureMaterial)
        materialEditor = (MaterialEditor) CreateEditor(patch.textureMaterial);
    }

    public void OnDisable () {
      // DestroyImmediate(materialEditor);
    }

    public override bool HasPreviewGUI () {
      return patch && patch.renderTexture;
    }

    public override void OnPreviewGUI (Rect r, GUIStyle background) {
      var patch = target as Patch;
      GUI.DrawTexture(r, patch.renderTexture, ScaleMode.ScaleToFit, false);
    }

    public override void OnInspectorGUI () {
      EditorGUI.BeginChangeCheck();
      serializedObject.Update();
      GUILayout.Button("Regenerate");
      DrawDefaultInspector();
      if (materialEditor) {
        GUILayout.Space(4);
        materialEditor.DrawHeader();
        var isDefaultMaterial = !AssetDatabase.GetAssetPath(patch.textureMaterial).StartsWith("Assets");
        using (new EditorGUI.DisabledGroupScope(isDefaultMaterial))
          materialEditor.OnInspectorGUI();
      }
      if (EditorGUI.EndChangeCheck())
        targets.OfType<Patch>().ForEach(p => p.Generate());
    }

  }

}
