//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;

namespace NNI.Terrain.Editor {

  [CustomEditor(typeof(Heightmap))]
  public class HeightmapEditor : UnityEditor.Editor {

    Heightmap heightmap;

    // private MaterialEditor materialEditor;

    public void OnEnable () {
      heightmap = target as Heightmap;
      // if (heightmap.material)
      //   materialEditor = (MaterialEditor) CreateEditor(heightmap.material);
    }

    public void OnDisable () {
      // DestroyImmediate(materialEditor);
    }

    public override void OnInspectorGUI () {
      EditorGUI.BeginChangeCheck();
      serializedObject.Update();
      GUILayout.Button("Regenerate");
      DrawDefaultInspector();
      if (EditorGUI.EndChangeCheck())
        heightmap.Generate();
      // if (materialEditor) {
      //   GUILayout.Space(4);
      //   materialEditor.DrawHeader();
      //   var isDefaultMaterial = !AssetDatabase.GetAssetPath(heightmap.material).StartsWith("Assets");
      //   using (new EditorGUI.DisabledGroupScope(isDefaultMaterial)) {
      //     materialEditor.OnInspectorGUI();
      //   }
      // }
    }
  }

}
